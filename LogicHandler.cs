using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace ValorantStreamOverlay
{


    class LogicHandler
    {
        public static string AccessToken { get; set; }
        public static string EntitlementToken { get; set; }
        public static string UserID { get; set; }

        public static string username, tag, region;
        public static int refreshTimeinSeconds;
        public Timer relogTimer, pointTimer;

        public static ValorantOverStream ValorantOver;
        public LogicHandler logic;
        public RankDetection rankDetect;

        //Twitch Bot Variables
        public static int currentRankPoints, currentMMRorELO;
        private bool botEnabled;

        public LogicHandler(ValorantOverStream instance)
        {
            logic = this;
            ValorantOver = instance;

            Trace.Write("Reading Settings");
            ReadSettings();
        }

         void ReadSettings()
        {

            if (string.IsNullOrEmpty(Properties.Settings.Default.tag) || string.IsNullOrEmpty(Properties.Settings.Default.username))
                MessageBox.Show("Welcome, you have to set your username and tag in the settings menu");
            else
            {
                username = Properties.Settings.Default.username;
                tag = Properties.Settings.Default.tag;
                region = new SettingsParser().ReadRegion(Properties.Settings.Default.region).GetAwaiter().GetResult();
                refreshTimeinSeconds = new SettingsParser().ReadDelay(Properties.Settings.Default.region).GetAwaiter().GetResult();
                new SettingsParser().ReadSkin(Properties.Settings.Default.skin).GetAwaiter();
                botEnabled = new SettingsParser().ReadTwitchBot().GetAwaiter().GetResult();

                RiotGamesLogin();

                UpdateToLatestGames();
                new RankDetection();

                StartPointRefresh();
                StartRELOGTimer();
                StartTwitchBot();
            }

        }


        void RiotGamesLogin()
        {
            try
            {
                CookieContainer cookie = new CookieContainer();
                var authJson = JsonConvert.DeserializeObject(Authentication.Authenticate(username, tag));
                JToken authObj = JObject.FromObject(authJson);

                if (!authObj["status"].ToString().Contains("200"))
                {
                    // error time lmfao
                    MessageBox.Show("Nick and/or Tag is incorrect, please fix info in settings.");
                }
                else
                {
                    UserID = authObj["data"]["puuid"].ToString();
                }


            }
            catch (Exception e)
            {
                MessageBox.Show("Your Login was invalid, please check your settings.");
            }
        }



        async Task UpdateToLatestGames()
        {
            Trace.Write("UPDATING");
            dynamic response = GetCompApiAsync().GetAwaiter().GetResult();
            Debug.WriteLine(response.StatusCode);
            if (response.StatusCode == 200)
            {
                int[] points = new int[3];
                dynamic matches = response["data"];
                int count = 0, i  = 0;
                foreach (var game in matches)
                {

                    if (game["currenttier"] == 0)
                    {
                        // riot said fuck off to this one i guess LMAO
                    }
                    // else if (game["currenttier"] > matches[i+1]["currenttier"]) // Promoted meaning, that afterupdate is more than beforeupdate
                    // {
                    //     player promoted
                    //         int before = matches[i+1]["ranking_in_tier"];
                    //         int after = game["ranking_in_tier"];
                    //         int differ = game["elo"] - matches[i+1]["elo"]; 
                    //     points[i++] = game["mmr_change_to_last_game"];
                    //     count++;
                    // }
                    // else if (game["currenttier"] < matches[i+1]["currenttier"])
                    // {
                    //     player demoted
                    //     int before = game["RankedRatingBeforeUpdate"];
                    //     int after = game["RankedRatingAfterUpdate"];
                    //     int differ = (after - before) - 100; 
                    //     points[i++] = differ;
                    //     count++;
                    // }
                    else
                    {
                        points[i++] = game["mmr_change_to_last_game"];
                        count++;
                    }

                    if (count >= 3) // 3 recent matches found
                        break;
                }
                //Send Points to Function that changes the UI
                SetChangesToOverlay(points).GetAwaiter();
            }

        }


        async Task<JObject> GetCompApiAsync()
        {
            string url = "https://api.henrikdev.xyz/valorant/v1/by-puuid/mmr-history/";
            string url_add = url + Properties.Settings.Default.region + "/" + UserID;

            RestClient client = new RestClient(url_add);
            RestRequest request = new RestRequest(Method.GET);

            return client.Execute(request).IsSuccessful ? JsonConvert.DeserializeObject<JObject>(client.Execute(request).Content) : null;
        }


        private async Task SetChangesToOverlay(int[] pointchange)
        {
            Label[] rankChanges = { ValorantOver.recentGame1, ValorantOver.recentGame2, ValorantOver.recentGame3 };
            for (int i = 0; i < pointchange.Length; i++)
            {
                // neg num represents decrease in pts
                if (pointchange[i] < 0)
                {
                    //In the case of a demotion or a loss
                    pointchange[i] *= -1;
                    string change;
                    change = pointchange[i] <= 9 ? $"0{pointchange[i]}" : pointchange[i].ToString();
                    
                    rankChanges[i].ForeColor = Color.Red;
                    rankChanges[i].Text = $"-{change}";
                }
                else if (pointchange[i] > 0)
                {
                    //int checker = pointchange[i] * -1;
                    string change;
                    change = pointchange[i] <= 9 ? $"0{pointchange[i]}" : pointchange[i].ToString();

                    rankChanges[i].ForeColor = Color.LimeGreen;
                    rankChanges[i].Text = $"+{change}";
                }
                else
                {
                    rankChanges[i].ForeColor = Color.SlateGray;
                    rankChanges[i].Text = "0";
                }
            }
        }


        private void StartPointRefresh()
        {
            pointTimer = new Timer();
            pointTimer.Tick += new EventHandler(pointTimer_Tick);
            pointTimer.Interval = refreshTimeinSeconds * 1000;
            pointTimer.Start();
        }

        private void pointTimer_Tick(object sender, EventArgs e)
        {
            UpdateToLatestGames().GetAwaiter();

            if (rankDetect == null)
            {
                rankDetect = new RankDetection();
            }
            else
            {
                rankDetect.UpdateRank();
            }
        }

        private void StartRELOGTimer()
        {
            relogTimer = new Timer();
            relogTimer.Tick += new EventHandler(relogTimer_Tick);
            relogTimer.Interval = 2700 * 1000;
            relogTimer.Start();
            
        }

        private void relogTimer_Tick(object sender, EventArgs e)
        {
            pointTimer.Stop();
            RiotGamesLogin();
            pointTimer.Start();
        }

        public void StartTwitchBot()
        {
            if (botEnabled)
            {
                Trace.WriteLine("Bot enabled");
                TwitchIntegration bot = new TwitchIntegration();
            }
            else
            {
                Trace.WriteLine("Bot not enabled");
            }
        }
    }
}
