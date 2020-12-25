<h1 align="center">
  <img src="https://cdn.discordapp.com/attachments/748560845230964869/791900465632641034/ValoverlayLogo.png">
  <br>
</h1>

<h4 align="center">A Valorant Program, that runs on your computer, and acts like an overlay for OBS</h4>

<p align="center">
  </a>
  <a href="https://twitter.com/rumblemikee"><img src="https://img.shields.io/badge/Twitter-@RumbleMikee-1da1f2.svg?logo=twitter?style=for-the-badge&logo=appveyor"></a>
  <a href="https://twitter.com/Valorleaks"><img src="https://img.shields.io/badge/Twitter-@ValorLeaks-1da1f2.svg?logo=twitter?style=for-the-badge&logo=appveyor"></a>
  
</p>

## About
Hey everyone, this program was made after i was bothered every other second on what my rank was. I could have implemented a `!rank` command, but i thought of doing something unique. A program that checks your account every few seconds and sees your current rank and your point movement (still in the air). All of the code is open source, so anyone can check if the program is safe or not as you need to type in your Username and Password in the configuation file. As of writing this, there are no alternatives. I will say it now, i will not verify if alternatives are safe. I know my program is safe, and anyone can check if that reason is vaild.

## External Packages Used
Some External Packages were used to create this project with ease and implement cool features:
  - [RestSharp](https://www.nuget.org/packages/RestSharp/)
  - [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
  - Adding Soon: [AutoUpdater](https://github.com/ravibpatel/AutoUpdater.NET)

## How to use program
Welcome to the Tutorial in a way, there will be a video soon but, for now there are instructions layed out in text below.

# Installation
Step 1: Download Latest Version [here](https://github.com/RumbleMike/ValorantStreamOverlay/releases/download/1.0/ValorSteamOverlay.zip)

Step 2: Extract Zip file to your perfered location (If you dont know how to do this... well.. google it)

Step 3: Open your extracted folder and make your way over to the `refrences` folder, here you will find a TTF File (Font File) and a Config.Json. Open Config.Json with your respected text editor.

Step 4: Enter your Riot Games Username (One you use to login, not in-game name) and your Riot Games Password.

Step 5: Set your region, please refer to the table below for your region, if your country/region is not on this list please try every location shown in the table.
| Region Name | Region Prefix |
| - | - |
| North America, Latam, Brazil | `na` |
| Europe | `eu` |
| Asia Pacific, Japan, OCE, Africa, Australia  | `ap` |
| Korea | `ko` |

These prefixes will be set in the part of the config file below.
```
{
"region": "PREFIXHERE"
}
```

Step 6: Set your delay. This is how often your rank and ranked points update, Recommendation is 5-20 Seconds.
For example if your config file looks like this:
```
{
"username" : "ValorantAdmin",
"password" : "Hunter2",
"region" : "na",
"refreshtime" : 10
}
```
This means that my region is North America & the overlay will update every 10 seconds.

**DO NOT PUT 1 SECOND, THE API WILL NOT LIKE THAT**

Step 7: Run the Application, you will get an pop up, from Windows Smart Screen. This is normal as this program is not [signed](https://en.wikipedia.org/wiki/Code_signing) `tdlr too complicated and i think it costs money` From here you have a possiblity of getting a popup from microsoft asking you to install something. Install it. It is called the .net Runtime it is the framework that allows the program to run.

Step 8: When the program successfully launches, you will see your Rank and your point gains/losses for your last 3 matches. Red Numbers indicate a Loss while Green numbers indicate a win.

Step 9: Create a window capture in OBS and you are done.

This is the entire process to get the overlay working perfectly and how it was designed.
