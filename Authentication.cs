using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ValorantStreamOverlay
{
    class Authentication
    {

        public static string Authenticate(string user, string pass)
        {
            string url = "https://api.henrikdev.xyz/valorant/v1/account/";
            string url_add = url + user + "/" + pass;
            RestClient client = new RestClient(url_add);

            RestRequest request = new RestRequest(Method.GET);
            //var response = JObject.Parse(client.Execute(request).Content);
            //Debug.WriteLine(response.ToString());
            //Debug.WriteLine("GETTING PUUID");
            
            return client.Execute(request).Content.ToString();
        }
    }
}
