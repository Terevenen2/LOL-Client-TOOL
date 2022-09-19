using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xceed.Wpf.Toolkit;
using FontFamily = System.Drawing.FontFamily;
using Image = System.Windows.Controls.Image;
using Point = System.Drawing.Point;

namespace LOL_Client_TOOL_WPF
{
    public partial class MainWindow : Window
    {
        public static bool debug = false;//set true to display inputs for testing lcu requests
        public static bool preview = false;
        public static ConfigurationData configData = new ConfigurationData();

        public static Dictionary<string, string> lesArguments = new Dictionary<string, string>();
        public static Dictionary<string, string> riotCredntials = new Dictionary<string, string>();
        public static Dictionary<string, string> shardsRunes = new Dictionary<string, string>();
        public static Dictionary<int, string> currentSummonerTestedIcon = new Dictionary<int, string>();
        public static Dictionary<int, Point> posSubMainRune = new Dictionary<int, Point>();
        public static SortedDictionary<string, string> champs = new SortedDictionary<string, string>();
        public static List<string> summonerIcons = new List<string>();
        public static List<string> currentSummonerOwnedIcons = new List<string>();
        public static List<string> conversationsMessageSent = new List<string>();
        public static List<championPick> pickPriorities = new List<championPick>();
        public static string port = "";
        public static string portRiotClient = "";
        public static string autorization = "";
        public static string autorizationRiotClient = "";
        public static string version = "";
        public static string lang = "en_US";
        public static string DDragonImg = "https://ddragon.leagueoflegends.com/cdn/img/";
        public static string pathRunes = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\LOL_Client_TOOL\\runes\\";
        public static string instaLockChampId = "266";
        public static string position1 = "TOP";
        public static string position2 = "JUNGLE";
        public static string[] chare = { "/" };
        public static string[] postions = new string[] { "TOP", "JUNGLE", "MID", "ADC", "SUPPORT", "FILL" };//  TOP JUNGLE MIDDLE BOTTOM UTILITY FILL
        public static string runeSource = "U.GG";
        public static string lastRunePageBuilt = "";
        public static string lastSummonerBuilt = "";
        public static int champPickId = 0;
        public static int instalockCount = 0;
        public static int downloadingRunes = 0;
        public static int downloadedRunes = 0;
        public static int AutoQPlayAgianIntervalle = 0;

        public static JArray runesReforged = new JArray();

        public static bool formSummonerIconLoadComplete = false;
        public static bool processingAllIcon = false;
        public static bool formSummonerRuneIsSetup = false;
        public static bool instalockIsEnabled = false;

        public string ClientReadyCheckState
        {
            get { return _clientReadyCheckState; }
            set
            {
                apiEndPointResponse.Text += DateTime.Now + " : " + value + Environment.NewLine;
                if (_clientReadyCheckState != value)
                {
                    _clientReadyCheckState = value;
                }
            }
        }
        public static string _clientReadyCheckState;

        public static List<leagueRune> leagueOfLegendsRunes = new List<leagueRune>();
        public static List<champion> leagueOfLegendsChampions = new List<champion>();

        public class ConfigurationData
        {
            public bool autoAccept { get; set; }
            public bool autoPick { get; set; }
            public bool autoBan { get; set; }
            public bool autoPlayAgain { get; set; }
            public bool autoHonor { get; set; }
            public bool autoRole { get; set; }
            public string autoRolePosition1 { get; set; }
            public string autoRolePosition2 { get; set; }
            public string autoPickChampion { get; set; }
            public bool autoMessage { get; set; }
            public string autoMessageText { get; set; }
            public bool autoReroll { get; set; }
            public bool autoSkin { get; set; }
            public bool autoPrePick { get; set; }
            public bool autoAramBenchSwap { get; set; }
            public bool autoRunes { get; set; }
            public bool autoSummoners { get; set; }
            public string autoRunesSource { get; set; }
            public string autoSummonersSource { get; set; }
            public List<championPick> championPickList { get; set; }
            public int prePickDelay { get; set; }
            public int pickDelay { get; set; }
            public int banDelay { get; set; }
        }

        public class championsPrio
        {
            int id { get; set; }
            int pick { get; set; }
            int ban { get; set; }
            int aram { get; set; }
        }
        public class championPick
        {
            public int Id { get; set; }
            public int Pick { get; set; }
            public int Ban { get; set; }
            public int Aram { get; set; }
        }
        public class RunePage
        {
            public bool current { get; set; }
            public int id { get; set; }
            public bool isActive { get; set; }
            public bool isDeletable { get; set; }
            public bool isEditable { get; set; }
            public bool isValid { get; set; }
            public double lastModified { get; set; }
            public string name { get; set; }
            public int order { get; set; }
            public int primaryStyleId { get; set; }
            public int subStyleId { get; set; }
            public Dictionary<int, int> selectedPerkIds { get; set; }
        }
        public class SummonerRunes
        {
            public Dictionary<int, RunePage> RunePages { get; set; }
        }

        public class Rune
        {
            public int id { get; set; }
            public string key { get; set; }
            public string icon { get; set; }
            public string name { get; set; }
            public string shortDesc { get; set; }
            public string longDesc { get; set; }
        }

        public class leagueRune
        {
            public int id { get; set; }
            public string key { get; set; }
            public string icon { get; set; }
            public string name { get; set; }
            public Dictionary<int, Dictionary<int, Rune>> slots { get; set; }
        }

        public class champion
        {
            public string name { get; set; }
            public string version { get; set; }
            public string id { get; set; }
            public string key { get; set; }
            public string title { get; set; }
            public string blurb { get; set; }
            public info info { get; set; }
            public image image { get; set; }
            public List<string> tags { get; set; }
            public string partype { get; set; }
            public stats stats { get; set; }
        }

        public class info
        {
            public string attack { get; set; }
            public string defense { get; set; }
            public string magic { get; set; }
            public string difficulty { get; set; }
        }

        public class image
        {
            public string full { get; set; }
            public string sprite { get; set; }
            public string group { get; set; }
            public string x { get; set; }
            public string y { get; set; }
            public string w { get; set; }
            public string h { get; set; }
        }

        public class tags
        {
            public string tag1 { get; set; }
            public string tag2 { get; set; }
        }

        [Conditional("DEBUG")]
        public static void isDebug()
        {
             debug = true;
        }

        public class stats
        {
            public string hp { get; set; }
            public string hpperlevel { get; set; }
            public string mp { get; set; }
            public string mpperlevel { get; set; }
            public string movespeed { get; set; }
            public string armor { get; set; }
            public string armorperlevel { get; set; }
            public string spellblock { get; set; }
            public string spellblockperlevel { get; set; }
            public string attackrange { get; set; }
            public string hpregen { get; set; }
            public string hpregenperlevel { get; set; }
            public string mpregen { get; set; }
            public string mpregenperlevel { get; set; }
            public string crit { get; set; }
            public string critperlevel { get; set; }
            public string attackdamage { get; set; }
            public string attackdamageperlevel { get; set; }
            public string attackspeedoffset { get; set; }
            public string attackspeedperlevel { get; set; }
            public string attackdamageperleveloffset { get; set; }
        }


        public class ComboBoxIdName
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public class RerollPoints
        {
            public string currentPoints { get; set; }
            public string maxRolls { get; set; }
            public string numberOfRolls { get; set; }
            public string pointsCostToRoll { get; set; }
            public string pointsToReroll { get; set; }
        }

        public class Summoner
        {
            public string accountId { get; set; }
            public string displayName { get; set; }
            public string internalName { get; set; }
            public string nameChangeFlag { get; set; }
            public string percentCompleteForNextLevel { get; set; }
            public string profileIconId { get; set; }
            public string puuid { get; set; }
            public RerollPoints rerollPoints { get; set; }
            public string summonerId { get; set; }
            public string summonerLevel { get; set; }
            public string unnamed { get; set; }
            public string xpSinceLastLevel { get; set; }
            public string xpUntilNextLevel { get; set; }
        }

        public static Summoner currentSummoner = new Summoner();
        public static RerollPoints currentSummonerRerollPoints = new RerollPoints();

        public static SummonerRunes currentSummonerRunes = new SummonerRunes();

        //Form objects
        public static Grid FormSummonerIcon = new Grid();
        public static Grid FormSummonerRunes = new Grid();
        public static Grid FormChampSelect = new Grid();
        public static Grid formLogin = new Grid();
        public static Window formChampions = new Window();
        public static Grid gridChampions = new Grid();
        public static Window formInformation = new Window();
        public static Grid gridInformations = new Grid();
        public static Window formDelays = new Window();
        public static Grid gridDelays = new Grid();

        public static ComboBox comboBoxRunesPages = new ComboBox();
        public static ComboBox comboBoxSummonerStatus = new ComboBox();
        public static ComboBox comboBoxAutoPosition1 = new ComboBox();
        public static ComboBox comboBoxAutoPosition2 = new ComboBox();
        public static ComboBox comboBoxAutoRunesSources = new ComboBox();
        public static ComboBox comboBoxAutoSummonersSources = new ComboBox();

        public static List<ComboBoxIdName> summonerStatus = new List<ComboBoxIdName>();

        public static Label labelSummonerDisplayName = new Label();
        public static Label labelInfo = new Label();

        public static ToolTip tooltip = new ToolTip();

        public static CheckBox verifyOwnedIcons = new CheckBox();
        public static CheckBox checkBoxAutoAccept = new CheckBox();
        public static CheckBox checkBoxAutoPick = new CheckBox();
        public static CheckBox checkBoxAutoBan = new CheckBox();
        public static CheckBox checkBoxAutoQPlayAgain = new CheckBox();
        public static CheckBox checkBoxAutoHonor = new CheckBox();
        public static CheckBox checkBoxAutoPosition = new CheckBox();
        public static CheckBox checkBoxAutoMessage = new CheckBox();
        public static CheckBox checkBoxAutoReroll = new CheckBox();
        public static CheckBox checkBoxAutoSkin = new CheckBox();
        public static CheckBox checkBoxAutoPrePick = new CheckBox();
        public static CheckBox checkBoxAutoAramBenchSwap = new CheckBox();
        public static CheckBox checkBoxAutoRunes = new CheckBox();
        public static CheckBox checkBoxAutoSummoners = new CheckBox();

        public static TextBox apiEnpoint = new TextBox();
        public static TextBox apiRequestType = new TextBox();
        public static TextBox apiRequestJson = new TextBox();
        public static TextBox apiEndPointResponse = new TextBox();
        public static TextBox textBoxId = new TextBox();
        public static TextBox textBoxPassword = new TextBox();
        public static TextBox textBoxConversationMessage = new TextBox();

        public static Button apiEndpointCall = new Button();
        public static Button buttonRune = new Button();
        public static Button buttonSetupFrom = new Button();
        public static Button buttonChampions = new Button();
        public static Button buttonDelays = new Button();

        public static Image summonerIcon = new Image();
        //Form object EVENTS
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private void SetTimer()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
            //aTimer.AutoReset = true;
        }

        public static void getVersion()
        {
            string data = DDragonRequest("https://ddragon.leagueoflegends.com/api/versions.json");
            var LeagueVersion = JArray.Parse(data);
            version = LeagueVersion.First.ToString();
        }

        private static string DDragonRequest(string url = "", string json = "")
        {
            if (!url.Contains("version"))
            {
                url = "https://ddragon.leagueoflegends.com/cdn/" + version + url;
            }

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";
            if (json != "")
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }
            }
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream receiveStream = httpResponse.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                return readStream.ReadToEnd();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "DDragonRequest");
                return e.Message;
            }
        }

        private static KeyValuePair<string, string> LCURequest(string url = "", string method = "", string json = "")
        {
            method = method.ToUpper();
            url = "https://127.0.0.1:" + port + url.Trim();
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = method;
            httpWebRequest.Headers.Add("Authorization", "Basic " + autorization);
            if (method == "POST" && json != "")
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }
            }
            if (method == "PUT")
            {
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PUT";
                httpWebRequest.Headers.Add("Authorization", "Basic " + autorization);
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }
            }
            if (method == "PATCH")
            {
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PATCH";
                httpWebRequest.Headers.Add("Authorization", "Basic " + autorization);
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }
            }
            if (method == "DELETE")
            {
                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "DELETE";
                httpWebRequest.Headers.Add("Authorization", "Basic " + autorization);
            }
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                Stream receiveStream = httpResponse.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                return new KeyValuePair<string, string>(httpResponse.StatusCode.ToString(), readStream.ReadToEnd());
            }
            catch (Exception e)
            {
                return new KeyValuePair<string, string>("", e.Message);
            }
        }

        public static void AutoQPlayAgian()
        {
            if (AutoQPlayAgianIntervalle == 0)
            {
                if (configData.autoPlayAgain)
                {
                    JObject json = JObject.Parse(LCURequest("/lol-gameflow/v1/gameflow-metadata/player-status", "GET").Value);
                    if (json["currentLobbyStatus"]["allowedPlayAgain"].ToString().ToUpper() == "True".ToUpper())
                    {
                        LCURequest("/lol-lobby/v2/play-again", "POST");
                        var data = LCURequest("/lol-gameflow/v1/gameflow-phase", "GET").Value;
                        if (data.ToString().Contains("Lobby"))
                        {
                            LCURequest("/lol-lobby/v2/lobby/matchmaking/search", "POST");
                        }
                    }
                }
            }
            AutoQPlayAgianIntervalle++;
            if (AutoQPlayAgianIntervalle > 3)
            {
                AutoQPlayAgianIntervalle = 0;
            }
        }

        public static ImageSource getIcon(string iconId)
        {
            string url = "http://ddragon.leagueoflegends.com/cdn/" + version + "/img/profileicon/" + iconId + ".png";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            Bitmap bitmap2 = new Bitmap(responseStream);
            //If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool DeleteObject([In] IntPtr hObject);

            ImageSource ImageSourceFromBitmap(Bitmap bmp)
            {
                var handle = bmp.GetHbitmap();
                try
                {
                    return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                finally { DeleteObject(handle); }
            }
            return ImageSourceFromBitmap(bitmap2);
        }

        public static async Task getSummoner()
        {
            JObject summoner = JObject.Parse(LCURequest("/lol-summoner/v1/current-summoner", "GET", "").Value);
            currentSummoner.accountId = (string)summoner["accountId"];
            currentSummoner.displayName = (string)summoner["displayName"];
            currentSummoner.internalName = (string)summoner["internalName"];
            currentSummoner.nameChangeFlag = (string)summoner["nameChangeFlag"];
            currentSummoner.percentCompleteForNextLevel = (string)summoner["percentCompleteForNextLevel"];
            if (currentSummoner.profileIconId != (string)summoner["profileIconId"])
            {
                summonerIcon.Source = getIcon((string)summoner["profileIconId"]);
            }
            currentSummoner.profileIconId = (string)summoner["profileIconId"];
            currentSummoner.puuid = (string)summoner["puuid"];
            currentSummoner.summonerId = (string)summoner["summonerId"];
            currentSummoner.summonerLevel = (string)summoner["summonerLevel"];
            currentSummoner.unnamed = (string)summoner["unnamed"];
            currentSummoner.xpSinceLastLevel = (string)summoner["xpSinceLastLevel"];
            currentSummoner.xpUntilNextLevel = (string)summoner["xpUntilNextLevel"];
            currentSummonerRerollPoints.currentPoints = (string)summoner["rerollPoints"]["currentPoints"];
            currentSummonerRerollPoints.maxRolls = (string)summoner["rerollPoints"]["maxRolls"];
            currentSummonerRerollPoints.numberOfRolls = (string)summoner["rerollPoints"]["numberOfRolls"];
            currentSummonerRerollPoints.pointsCostToRoll = (string)summoner["rerollPoints"]["pointsCostToRoll"];
            currentSummonerRerollPoints.pointsToReroll = (string)summoner["rerollPoints"]["pointsCostToReroll"];
        }
        public static async Task configSaveAsync()
        {
            string config = Newtonsoft.Json.JsonConvert.SerializeObject(configData);
            string path = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\LOL_Client_TOOL\\config\\";
            Directory.CreateDirectory(path);
            using (FileStream fs = File.Create(path + "config.json"))
            {
                byte[] data = new UTF8Encoding(true).GetBytes(config);
                fs.Write(data, 0, data.Length);
            }
        }

        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //Keep json for the loop START
            var lolChampSelectV1Session = new KeyValuePair<string, string>();
            int timer = 0;
            int delayMax = 0;
            if (configData.autoReroll || configData.autoPick || configData.autoSkin || configData.autoSkin)
            {
                lolChampSelectV1Session = LCURequest("/lol-champ-select/v1/session", "GET");
                if (lolChampSelectV1Session.Key == "OK")
                {
                    JObject json = JObject.Parse(lolChampSelectV1Session.Value);
                    timer = Convert.ToInt32(json["timer"]["adjustedTimeLeftInPhase"]);
                    delayMax = Convert.ToInt32(json["timer"]["totalTimeInPhase"]);
                }
            }
            //Keep json for the loop END
            //auto accept start
            if (configData.autoAccept)
            {
                LCURequest("/lol-matchmaking/v1/ready-check/accept", "POST", "");
            }
            //auto accept end

            //Honor start
            if (configData.autoHonor)
            {
                //get best player for honor start
                string tempBestSummonerId = "";
                double tempBestSummonerKDA = 0;
                var gameData = LCURequest("/lol-end-of-game/v1/eog-stats-block", "GET", "");
                if (gameData.Key == "OK")
                {
                    JObject gameDataJson = JObject.Parse(gameData.Value);
                    string localPlayerTeam = gameDataJson["localPlayer"]["teamId"].ToString();
                    //calculate player team KDA
                    foreach (var team in gameDataJson["teams"])
                    {
                        if (team["teamId"].ToString() == localPlayerTeam)
                        {
                            foreach (var player in team["players"])
                            {
                                if (player["summonerId"].ToString() != currentSummoner.summonerId.ToString())
                                {
                                    double KDA = (Convert.ToDouble(player["stats"]["ASSISTS"].ToString()) + Convert.ToDouble(player["stats"]["CHAMPIONS_KILLED"].ToString()) / Convert.ToDouble(player["stats"]["NUM_DEATHS"].ToString()));
                                    if (KDA > tempBestSummonerKDA)
                                    {
                                        tempBestSummonerKDA = KDA;
                                        tempBestSummonerId = player["summonerId"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                var honorData = LCURequest("/lol-honor-v2/v1/ballot", "GET");
                JObject json = JObject.Parse(honorData.Value);
                if (honorData.Key == "OK")
                {
                    string[] honorCategory = { "COOL", "SHOTCALLER", "HEART" };
                    string honorDataBody = "{\"gameId\": " + json["gameId"].ToString() + ",\"honorCategory\": \"" + honorCategory[2] + "\",\"summonerId\": " + tempBestSummonerId + "}";
                    LCURequest("/lol-honor-v2/v1/honor-player", "POST", honorDataBody);
                }
            }
            //honor end

            //auto pick/ban start
            string actorCellId = "";
            List<string> bans = new List<string>();
            List<string> championPickIntent = new List<string>();
            int selectedChampionId = 0;
            string assignedPosition = "";
            if (configData.autoPick || configData.autoBan || configData.autoPrePick)
            {
                if (lolChampSelectV1Session.Key == "OK")
                {
                    List<int> championPlayable = new List<int>();
                    var ownedChampionsMinimal = JArray.Parse(LCURequest("/lol-champions/v1/owned-champions-minimal", "GET", "").Value);
                    foreach (var champ in ownedChampionsMinimal)
                    {
                        //MessageBox.Show(champ["ownership"]["owned"].ToString());
                        if (champ["active"].ToString().ToLower() == "True".ToLower())
                        {
                            if (champ["ownership"]["owned"].ToString().ToLower() == "true".ToLower() || champ["ownership"]["freeToPlayReward"].ToString().ToLower() == "True".ToLower() || champ["freeToPlay"].ToString().ToLower() == "True".ToLower())
                            {
                                championPlayable.Add(Convert.ToInt32(champ["id"]));
                            }
                        }
                    }

                    JObject json = JObject.Parse(lolChampSelectV1Session.Value);

                    foreach (var tempBan in json["bans"]["myTeamBans"])
                    {
                        bans.Add(tempBan.ToString());
                    }
                    foreach (var tempBan in json["bans"]["theirTeamBans"])
                    {
                        bans.Add(tempBan.ToString());
                    }
                    foreach (var prePickedChamp in json["myTeam"])
                    {
                        championPickIntent.Add(prePickedChamp["championPickIntent"].ToString());
                    }

                    foreach (var player in json["myTeam"])
                    {
                        if (player["summonerId"].ToString() == currentSummoner.summonerId.ToString())
                        {
                            actorCellId = player["cellId"].ToString();
                            selectedChampionId = Convert.ToInt32(player["championId"]);
                            assignedPosition = player["assignedPosition"].ToString();
                        }
                    }
                    foreach (var actions in json["actions"])
                    {
                        foreach (var team in actions)
                        {
                            if (team["actorCellId"].ToString() == actorCellId)
                            {

                                string isInProgress = team["isInProgress"].ToString();
                                //PICK
                                if (isInProgress == "True" && team["type"].ToString().Contains("pick") && configData.autoPrePick)
                                {
                                    await Task.Delay(configData.pickDelay);
                                    //get the champ based on prioritie and bans
                                    int maxPrio = 0;
                                    string champId = "";
                                    foreach (var player in json["myTeam"])
                                    {
                                        if (championPlayable.Contains(Convert.ToInt32(player["championId"].ToString())) && currentSummoner.summonerId != player["summonerId"].ToString())
                                        {
                                            championPlayable.Remove(Convert.ToInt32(player["championId"].ToString()));
                                        }
                                    }
                                    foreach (var player in json["theirTeam"])
                                    {
                                        if (championPlayable.Contains(Convert.ToInt32(player["championId"].ToString())))
                                        {
                                            championPlayable.Remove(Convert.ToInt32(player["championId"].ToString()));
                                        }
                                    }
                                    foreach (championPick tempPick in configData.championPickList)
                                    {
                                        if (!bans.Contains(tempPick.Id.ToString()) && tempPick.Pick > maxPrio && championPlayable.Contains(tempPick.Id))
                                        {
                                            maxPrio = tempPick.Pick;
                                            champId = tempPick.Id.ToString();
                                        }
                                    }

                                    //build and make the pick request
                                    string jsonDataForLock = "{  \"actorCellId\": " + actorCellId + ",  \"championId\": " + champId + ",  \"completed\": true,  \"id\": " + team["id"].ToString() + ",  \"isAllyAction\": true,  \"type\": \"string\"}";
                                    var resp = LCURequest("/lol-champ-select/v1/session/actions/" + team["id"].ToString(), "PATCH", jsonDataForLock);
                                }
                                //BAN
                                if (isInProgress == "True" && team["type"].ToString().Contains("ban") && configData.autoBan)
                                {
                                    await Task.Delay(configData.banDelay);
                                    //get the champ based on prioritie and bans
                                    int maxPrio = 0;
                                    string champId = "";
                                    foreach (championPick tempBan in configData.championPickList)
                                    {
                                        if (!bans.Contains(tempBan.Id.ToString()) && !championPickIntent.Contains(tempBan.Id.ToString()) && tempBan.Ban > maxPrio)
                                        {
                                            maxPrio = tempBan.Ban;
                                            champId = tempBan.Id.ToString();
                                        }
                                    }
                                    //build and make the pick request
                                    string jsonDataForLock = "{  \"actorCellId\": " + actorCellId + ",  \"championId\": " + champId + ",  \"completed\": true,  \"id\": " + team["id"].ToString() + ",  \"isAllyAction\": true,  \"type\": \"string\"}";
                                    var resp = LCURequest("/lol-champ-select/v1/session/actions/" + team["id"].ToString(), "PATCH", jsonDataForLock);
                                }
                                //pre pick
                                if (configData.autoPrePick && !team["type"].ToString().Contains("ban") && !team["type"].ToString().Contains("pick"))
                                {
                                    await Task.Delay(configData.prePickDelay);
                                    //get the champ based on prioritie and bans
                                    int maxPrio = 0;
                                    string champId = "";
                                    foreach (championPick tempPick in configData.championPickList)
                                    {
                                        if (!bans.Contains(tempPick.Id.ToString()) && tempPick.Pick > maxPrio && championPlayable.Contains(tempPick.Id))
                                        {
                                            maxPrio = tempPick.Pick;
                                            champId = tempPick.Id.ToString();
                                        }
                                    }
                                    //build and make the pick request
                                    string jsonDataForLock = "{  \"actorCellId\": " + actorCellId + ",  \"championId\": " + champId + ",  \"completed\": true,  \"id\": " + team["id"].ToString() + ",  \"isAllyAction\": true,  \"type\": \"string\"}";
                                    var resp = LCURequest("/lol-champ-select/v1/session/actions/" + team["id"].ToString(), "PATCH", jsonDataForLock);
                                }

                                //if (isInProgrss == "True" && team["type"].ToString().Contains("ban") && !bans.Contains(instaLockChampId) && checkBoxAutoBan.Checked)
                                //{
                                //    string jsonDataForLock = "{  \"actorCellId\": " + actorCellId + ",  \"championId\": " + instaLockChampId + ",  \"completed\": true,  \"id\": " + team["id"].ToString() + ",  \"isAllyAction\": true,  \"type\": \"string\"}";
                                //    var resp = LCURequest("/lol-champ-select/v1/session/actions/" + team["id"].ToString(), "PATCH", jsonDataForLock);
                                //}
                            }
                        }
                    }
                    //ARAM swap bench
                    if (configData.autoAramBenchSwap)
                    {
                        List<int> benchedChamps = new List<int>();
                        foreach (var id in json["benchChampionIds"])
                        {
                            benchedChamps.Add(Convert.ToInt32(id));
                        }
                        int maxPrio = 0;
                        string champId = "";
                        int currentChampionPrio = 0;
                        foreach (championPick tempPick in configData.championPickList)
                        {
                            if (tempPick.Id == selectedChampionId)
                            {
                                currentChampionPrio = Convert.ToInt32(tempPick.Aram);
                            }

                            if (!bans.Contains(tempPick.Id.ToString()) && tempPick.Aram > maxPrio && championPlayable.Contains(tempPick.Id) && benchedChamps.Contains(tempPick.Id))
                            {
                                maxPrio = tempPick.Aram;
                                champId = tempPick.Id.ToString();
                            }
                        }
                        //bench swap
                        if (champId != "" && currentChampionPrio < maxPrio)
                        {
                            var resp = LCURequest("/lol-champ-select/v1/session/bench/swap/" + champId.ToString(), "POST");
                        }
                    }
                    //auto runes/ auto summoners
                    string currentChampionName = "";
                    if (configData.autoRunes || configData.autoSummoners)
                    {
                        foreach (champion champ in leagueOfLegendsChampions)
                        {
                            if (champ.key == selectedChampionId.ToString())
                            {
                                currentChampionName = champ.name;
                            }
                        }

                    }

                    //auto runes
                    List<string> runes = new List<string>();

                    if (configData.autoRunes && selectedChampionId != 0 && selectedChampionId.ToString() != lastRunePageBuilt)
                    {
                        string html = "";
                        if (assignedPosition != "")
                        {
                            html = "https://u.gg/lol/champions/" + currentChampionName.ToLower() + "/build?rank=diamond_plus&role=" + assignedPosition;
                        }
                        else
                        {
                            html = "https://u.gg/lol/champions/" + currentChampionName.ToLower() + "/build?rank=diamond_plus";
                        }

                        lastRunePageBuilt = selectedChampionId.ToString();
                        var agg = LCURequest("/lol-perks/v1/currentpage", "GET");
                        var currentRunePage = new JObject();
                        string currentPageId = "";
                        string currentPageEditable = "";
                        if (!agg.Value.Contains("404"))
                        {
                            currentRunePage = JObject.Parse(agg.Value);
                            currentPageId = currentRunePage["id"].ToString();
                            currentPageEditable = currentRunePage["isEditable"].ToString().ToLower();
                        }
                        else
                        {
                            Random rnd = new Random();
                            currentPageId = rnd.Next(10000, 100000).ToString();
                            currentPageEditable = "true";
                        }

                        foreach (champion champ in leagueOfLegendsChampions)
                        {
                            if (champ.key == selectedChampionId.ToString() && currentPageEditable == "true")
                            {
                                string championName = champ.name;
                                if (runeSource == "U.GG")
                                {
                                    if (championName.ToLower().Contains("nunu"))
                                    {
                                        championName = "nunu";
                                    }


                                    //var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");
                                    string ClassToGet1 = "perk perk-active";
                                    string ClassToGet2 = "perk keystone perk-active";
                                    string ClassToGet3 = "shard shard-active";
                                    string splitRunes = "\\\"";
                                    string selector = "//div[contains(@class, '" + ClassToGet2 + "')]";
                                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                                    HtmlWeb web = new HtmlWeb();
                                    htmlDoc = web.Load(html);
                                    htmlDoc.LoadHtml(htmlDoc.Text);
                                    var htmlNodes = htmlDoc.DocumentNode.SelectNodes(selector);
                                    foreach (HtmlNode node in htmlNodes)
                                    {
                                        //string frormed = node.InnerHtml.Split("alt");
                                        if (node.InnerHtml.Contains(ClassToGet2))
                                        {
                                            if (!runes.Contains(Regex.Split(node.InnerHtml, splitRunes)[Regex.Split(node.InnerHtml, splitRunes).Count() - 2]))
                                            {
                                                runes.Add(Regex.Split(node.InnerHtml, splitRunes)[Regex.Split(node.InnerHtml, splitRunes).Count() - 2]);
                                            }
                                        }
                                    }
                                    foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[@class='" + ClassToGet1 + "']"))
                                    {
                                        if (!runes.Contains(Regex.Split(node.InnerHtml, splitRunes)[Regex.Split(node.InnerHtml, splitRunes).Count() - 2]))
                                        {

                                            runes.Add(Regex.Split(node.InnerHtml, splitRunes)[Regex.Split(node.InnerHtml, splitRunes).Count() - 2]);
                                        }
                                    }
                                    foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[@class='" + ClassToGet3 + "']"))
                                    {
                                        runes.Add(Regex.Split(node.InnerHtml, splitRunes)[Regex.Split(node.InnerHtml, splitRunes).Count() - 2]);
                                    }
                                    runes.RemoveAt(runes.Count() - 1);
                                    runes.RemoveAt(runes.Count() - 1);
                                    runes.RemoveAt(runes.Count() - 1);
                                    //Console.WriteLine("Node Name: " + node.Name + "\n" + node.OuterHtml);
                                    string name = "";
                                    List<Tuple<string, string>> runePage = new List<Tuple<string, string>>();
                                    foreach (string perk in runes)
                                    {
                                        foreach (var style in runesReforged)
                                        {
                                            foreach (var slot in style["slots"])
                                            {
                                                foreach (var perkPage in slot["runes"])
                                                {
                                                    name = perkPage["name"].ToString();
                                                    if (perk.Contains(name))
                                                    {
                                                        runePage.Add(new Tuple<string, string>(perkPage["id"].ToString(), perkPage["name"].ToString()));
                                                    }
                                                }
                                            }
                                        }
                                        foreach (var shard in shardsRunes)
                                        {
                                            if (perk.Contains(shard.Value))
                                            {
                                                runePage.Add(new Tuple<string, string>(shard.Key, shard.Value));
                                            }
                                        }
                                    }
                                    var resp = LCURequest("/lol-perks/v1/styles", "GET");
                                    var styles = JArray.Parse(resp.Value);
                                    //set the runes
                                    //string boddy = "{\"autoModifiedSelections\":[],\"current\":true,\"id\":" + currentPageId + ",\"isActive\":true,\"isDeletable\":true,\"isEditable\":true,\"isValid\":true,\"lastModified\":" + currentRunePage["lastModified"] + ",\"name\":\"" + currentRunePage["name"] + "\",\"order\":0,\"primaryStyleId\":8000,\"selectedPerkIds\":[" + runePage.ToList()[0].Key + "," + runePage.ToList()[1].Key + "," + runePage.ToList()[2].Key + "," + runePage.ToList()[3].Key + "," + runePage.ToList()[4].Key + "," + runePage.ToList()[5].Key + "," + runePage.ToList()[6].Key + "," + runePage.ToList()[7].Key + "," + runePage.ToList()[8].Key + "],\"subStyleId\":8400}";
                                    string style1 = runePage[1].Item1.Substring(0, 2) + "00";
                                    string style2 = runePage[4].Item1.Substring(0, 2) + "00";
                                    if (style2.Substring(0, 1).Contains("9"))
                                    {
                                        style2 = "8000";
                                    }
                                    string lastModified = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                                    string boddy = "{\"current\":true,\"id\":" + currentPageId + ",\"isActive\":true,\"isDeletable\":true,\"isEditable\":true,\"isValid\":true,\"lastModified\":" + lastModified + ",\"name\":\"" + "LOL Client TOOL" + "\",\"order\":0,\"primaryStyleId\":" + style1 + ",\"selectedPerkIds\":[" + runePage[0].Item1 + "," + runePage[1].Item1 + "," + runePage[2].Item1 + "," + runePage[3].Item1 + "," + runePage.ToList()[4].Item1 + "," + runePage.ToList()[5].Item1 + "," + runePage[6].Item1 + "," + runePage[7].Item1 + "," + runePage[8].Item1 + "],\"subStyleId\":" + style2 + "}";
                                    //string boddy = "{\"current\":true,\"id\":" + currentPageId + ",\"isActive\":true,\"isDeletable\":true,\"isEditable\":true,\"isValid\":true,\"lastModified\":" + currentRunePage["lastModified"] + ",\"name\":\"" + currentRunePage["name"] + "\",\"order\":0,\"selectedPerkIds\":[" + runePage[0].Item1 + "," + runePage[1].Item1 + "," + runePage[2].Item1 + "," + runePage[3].Item1 + "," + runePage.ToList()[4].Item1 + "," + runePage.ToList()[5].Item1 + "," + runePage[6].Item1 + "," + runePage[7].Item1 + "," + runePage[8].Item1 + "]}";
                                    //LCURequest("/lol-perks/v1/pages", "POST", boddy);
                                    LCURequest("/lol-perks/v1/pages/" + currentPageId, "DELETE");

                                    LCURequest("/lol-perks/v1/pages", "POST", boddy);
                                }
                            }
                        }

                    }
                    //auto summoners
                    if (configData.autoSummoners && selectedChampionId != 0)
                    {
                        string html = "https://u.gg/lol/champions/" + currentChampionName.ToLower() + "/build?rank=diamond_plus&role=" + assignedPosition;
                        lastSummonerBuilt = selectedChampionId.ToString();
                        HtmlWeb web = new HtmlWeb();
                        var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                        htmlDoc.LoadHtml(html);
                        string ClassToGet1 = "content-section_content summoner-spells";
                        //foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[@class='" + ClassToGet2 + "']"))
                        foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[@class='" + ClassToGet1 + "']").Nodes())
                        {
                            //string frormed = node.InnerHtml.Split("alt");
                            string data = node.InnerHtml;
                        }
                    }
                }
            }
            //auto lock end

            //reQ/play again start
            AutoQPlayAgian();
            //reQ/play again end

            //AUTO POSITION start
            if (configData.autoRole)
            {
                //{ "TOP", "JUNGLE", "MIDDLE", "BOTTOM", "UTILITY", "FILL" }
                string tempPos1 = position1;
                string tempPos2 = position2;
                if (tempPos1 == "MID") { tempPos1 = "MIDDLE"; }
                if (tempPos1 == "SUPPORT") { tempPos1 = "UTILITY"; }
                if (tempPos1 == "ADC") { tempPos1 = "BOTTOM"; }
                if (tempPos2 == "MID") { tempPos2 = "MIDDLE"; }
                if (tempPos2 == "ADC") { tempPos2 = "BOTTOM"; }
                if (tempPos2 == "SUPPORT") { tempPos2 = "UTILITY"; }
                string json = "{\"firstPreference\":\"" + tempPos1 + "\",\"secondPreference\":\"" + tempPos2 + "\"}";
                LCURequest("/lol-lobby/v2/lobby/members/localMember/position-preferences", "PUT", json);
            }
            //AUTO POSITION end

            //AUTO MESSAGE START
            if (configData.autoMessage)
            {
                JArray json = JArray.Parse(LCURequest("/lol-chat/v1/conversations", "GET").Value);
                foreach (var conversations in json.Root)
                {
                    if (conversations["type"].ToString() == "championSelect")//game type: customGame 
                    {
                        string conversationId = conversations["id"].ToString();
                        if (!conversationsMessageSent.Contains(conversationId))
                        {
                            string boddy = "{\"body\":\"" + textBoxConversationMessage.Text + "\"}";
                            LCURequest("/lol-chat/v1/conversations/" + conversationId + "/messages", "POST", boddy);
                            conversationsMessageSent.Add(conversationId);
                        }
                    }
                }
            }
            //AUTO MESSAGE END

            //AUTO REROLL START
            if (configData.autoReroll)
            {
                if (lolChampSelectV1Session.Key == "OK")
                {
                    JObject json = JObject.Parse(lolChampSelectV1Session.Value);
                    if (json["allowRerolling"].ToString().ToLower() == "true" && Convert.ToInt32(json["rerollsRemaining"].ToString()) != 0)
                    {
                        int rerolls = Convert.ToInt32(json["rerollsRemaining"].ToString());
                        int i = 0;
                        while (i != rerolls)
                        {
                            LCURequest("/lol-champ-select/v1/session/my-selection/reroll", "POST");
                            i++;
                        }

                    }
                }
            }
            //AUTO REROLL END

            //AUTO SKIN START
            if (configData.autoSkin)
            {
                //  /lol-champ-select/v1/skin-carousel-skins   GET
                //  /lol-champ-select/v1/pickable-skin-ids  GET
                //  /lol-champ-select/v1/session/my-selection     patch
                //{
                // "selectedSkinId": 0,
                // "spell1Id": 0,
                // "spell2Id": 0,
                // "wardSkinId": 0
                //}
                if (lolChampSelectV1Session.Key == "OK")
                {
                    JObject json = JObject.Parse(lolChampSelectV1Session.Value);
                    foreach (var player in json["myTeam"])
                    {
                        if (player["summonerId"].ToString() == currentSummoner.summonerId && player["selectedSkinId"].ToString() != "0")
                        {
                            List<string> ownedSkins = new List<string>();
                            var skins = JArray.Parse(LCURequest("/lol-champ-select/v1/skin-carousel-skins", "GET").Value);
                            foreach (var sk in skins)
                            {
                                string unlocked = sk["unlocked"].ToString().ToLower();
                                string isbase = sk["isBase"].ToString().ToLower();
                                if (unlocked == "true" && isbase != "true")
                                {
                                    ownedSkins.Add(sk["id"].ToString());
                                }
                            }
                            foreach (var skin in skins)
                            {
                                string selectedSkinId = player["selectedSkinId"].ToString();
                                string isBase = skin["isBase"].ToString().ToLower();
                                string skinId = skin["id"].ToString();
                                if (skinId == selectedSkinId && isBase == "true")
                                {
                                    if (ownedSkins.Count > 0)
                                    {
                                        string changeLoadout = "{\"selectedSkinId\":" + ownedSkins[ownedSkins.Count - 1] + "}";
                                        LCURequest("/lol-champ-select/v1/session/my-selection", "PATCH", changeLoadout);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //AUTO SKIN END
        }

        public static void setupLCU()//for api usage
        {
            try
            {
                System.Diagnostics.ProcessStartInfo usbDevicesInfo = new System.Diagnostics.ProcessStartInfo("wmic", "PROCESS WHERE name='LeagueClientUx.exe' GET commandline");
                usbDevicesInfo.RedirectStandardOutput = true;
                usbDevicesInfo.UseShellExecute = false;
                usbDevicesInfo.CreateNoWindow = true;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo = usbDevicesInfo;
                process.Start();
                process.WaitForExit();
                Console.WriteLine("ExitCode: " + process.ExitCode.ToString() + "\n");
                string result = process.StandardOutput.ReadToEnd();
                string[] lesArgumentsTemp = result.Split(new[] { "\" \"" }, StringSplitOptions.None);
                foreach (string argument in lesArgumentsTemp)
                {
                    string arg = argument.Replace("\"", "");
                    if (arg.Contains("="))
                    {
                        string[] kv = arg.Split(Convert.ToChar("="));
                        lesArguments.Add(kv[0], kv[1]);
                    }
                }
                byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes("riot:" + lesArguments["--remoting-auth-token"]);
                autorization = Convert.ToBase64String(data);
                //string laData = "PASSWORD: " + lesArguments["--remoting-auth-token"] + "\r\n" + "PORT: " + lesArguments["--app-port"] + "\r\n" + "AUTH: " + autorization;
                port = lesArguments["--app-port"];
                lesArguments.Clear();
                //throw new Exception();//testing
            }
            catch (Exception ex)
            {
                try
                {
                    //trying with the lock file
                    var process = Process.GetProcesses();
                    string processesPath = "";
                    foreach (var name in process)
                    {
                        if (name.ProcessName.ToLower().Contains("LeagueClient".ToLower()))
                        {
                            processesPath = name.MainModule.FileName;
                        }
                    }
                    string[] pathes = processesPath.Split(new[] { @"\" }, StringSplitOptions.None);
                    string newPath = "";
                    foreach (string str in pathes)
                    {
                        if (str.Contains("exe"))
                        {
                            newPath += "lockfile";
                        }
                        else
                        {
                            newPath += str + @"\";
                        }
                    }
                    string readText = "";
                    using (FileStream stream = File.Open(newPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            readText = reader.ReadToEnd();
                        }
                    }
                    port = readText.Split(new[] { @":" }, StringSplitOptions.None)[2];
                    string temp = readText.Split(new[] { @":" }, StringSplitOptions.None)[3];

                    autorization = Convert.ToBase64String(Encoding.ASCII.GetBytes("riot:" + temp));

                }
                catch (Exception ex2)
                {
                    System.Windows.MessageBox.Show("Make sure to start the League of Legends Client before runing the TOOL", "Information");
                    setupLCU();
                }

            }
        }

        public static async Task setupFormAsync()
        {
            //getting language
            shardsRunes.Add("5008", "The Adaptive Force Shard");
            shardsRunes.Add("5007", "The Scaling CDR Shard");
            shardsRunes.Add("5005", "The Attack Speed Shard");
            shardsRunes.Add("5003", "The Magic Resist Shard");
            shardsRunes.Add("5002", "The Armor Shard");
            shardsRunes.Add("5001", "The Scaling Bonus Health Shard");
            var regionAndLanguage = JObject.Parse(LCURequest("/riotclient/get_region_locale", "GET").Value);
            lang = regionAndLanguage["locale"].ToString();
            runesReforged = JArray.Parse(DDragonRequest("/data/" + "en_US" + "/runesReforged.json"));

            string configPath = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\LOL_Client_TOOL\\config\\";
            Directory.CreateDirectory(configPath);
            JObject configuration = new JObject();
            if (File.Exists(configPath + "config.json"))
            {
                string tempConf = File.ReadAllText(configPath + "config.json");
                configData = JsonConvert.DeserializeObject<ConfigurationData>(tempConf);
                //populate form                
                configuration = JObject.Parse(tempConf);
                JsonConvert.PopulateObject(tempConf, configData);
                checkBoxAutoAccept.IsChecked = configData.autoAccept;
                checkBoxAutoPick.IsChecked = configData.autoPick;
                checkBoxAutoBan.IsChecked = configData.autoBan;
                checkBoxAutoAramBenchSwap.IsChecked = configData.autoAramBenchSwap;
                checkBoxAutoPrePick.IsChecked = configData.autoPrePick;
                checkBoxAutoQPlayAgain.IsChecked = configData.autoPlayAgain;
                checkBoxAutoHonor.IsChecked = configData.autoHonor;
                checkBoxAutoPosition.IsChecked = configData.autoRole;
                checkBoxAutoMessage.IsChecked = configData.autoMessage;
                textBoxConversationMessage.Text = configData.autoMessageText;
                checkBoxAutoReroll.IsChecked = configData.autoReroll;
                checkBoxAutoSkin.IsChecked = configData.autoSkin;
                checkBoxAutoRunes.IsChecked = configData.autoRunes;
                checkBoxAutoSummoners.IsChecked = configData.autoSummoners;
                position1 = configData.autoRolePosition1;
                position2 = configData.autoRolePosition2;
                if (configData.championPickList == null)
                {
                    configData.championPickList = new List<championPick>();
                }
                else//JObject.Parse duplicate the picklist somehow lets fix it
                {
                    int i = 0;
                    List<int> champUniqueIds = new List<int>();
                    List<int> toRemove = new List<int>();
                    foreach (championPick championPick in configData.championPickList)
                    {
                        if (!champUniqueIds.Contains(championPick.Id))
                        {
                            champUniqueIds.Add(championPick.Id);
                        }
                        else
                        {
                            toRemove.Add(i);
                        }
                        i++;
                    }
                    toRemove.Reverse();
                    foreach (int nb in toRemove)
                    {
                        configData.championPickList.RemoveAt(nb);
                    }
                }
            }


            //getting all champions
            string tempJsonChamp = DDragonRequest("/data/" + lang + "/champion.json");
            JObject championsJson = JObject.Parse(tempJsonChamp);


            foreach (var champData in championsJson["data"].Values())
            {
                string lestring = champData.ToString();
                champion lechamp = JsonConvert.DeserializeObject<champion>(lestring);
                leagueOfLegendsChampions.Add(lechamp);
            }
            leagueOfLegendsChampions = leagueOfLegendsChampions.OrderBy(o => o.name).ToList();

            //leagueOfLegendsChampions = ;
            await getSummoner();


            if (preview)
            {
                labelSummonerDisplayName.Content = "summoner name";
            }
            else
            {
                labelSummonerDisplayName.Content = currentSummoner.displayName;
            }

            //labelSummonerDisplayName.Text = "summoner name";
            labelSummonerDisplayName.HorizontalAlignment = HorizontalAlignment.Center;
            labelSummonerDisplayName.VerticalAlignment = VerticalAlignment.Bottom;
            labelSummonerDisplayName.Width = Double.NaN;
            labelSummonerDisplayName.Height = Double.NaN;
            labelSummonerDisplayName.ToolTip = "Copy in clipboard";
            labelSummonerDisplayName.MouseLeftButtonUp += LabelSummonerDisplayName_MouseLeftButtonUp; ;

            buttonChampions.Width = Double.NaN;
            buttonChampions.Height = Double.NaN;
            Border myBorder1 = new Border();
            myBorder1.CornerRadius = new CornerRadius(15);

            Style myStyle1 = new Style { Setters = { new Setter { Property = Border.CornerRadiusProperty, Value = new CornerRadius(10) } } };
            buttonChampions.Style = myStyle1;
            buttonChampions.Content = "Champions";
            buttonChampions.Click += ButtonChampions_Click;

            formChampions.Width = 400;

            isDebug();
            gridChampions.ShowGridLines = debug;
            while (gridChampions.ColumnDefinitions.Count < 4)
            {
                if(gridChampions.ColumnDefinitions.Count == 0)
                {
                    ColumnDefinition def = new ColumnDefinition();
                    def.MaxWidth = 110;
                    def.MinWidth = 110;
                    gridChampions.ColumnDefinitions.Add(def);
                }
                else
                {
                    gridChampions.ColumnDefinitions.Add(new ColumnDefinition());
                }
            }
            formChampions.Content = gridChampions;

            summonerStatus.Add(new ComboBoxIdName { Id = 0, Name = "online".ToUpper() });
            summonerStatus.Add(new ComboBoxIdName { Id = 1, Name = "away".ToUpper() });
            summonerStatus.Add(new ComboBoxIdName { Id = 2, Name = "mobile".ToUpper() });
            summonerStatus.Add(new ComboBoxIdName { Id = 3, Name = "offline".ToUpper() });
            
            comboBoxSummonerStatus.ItemsSource = summonerStatus;
            comboBoxSummonerStatus.DisplayMemberPath = "Name";
            comboBoxSummonerStatus.SelectedValuePath = "Id";
            comboBoxSummonerStatus.SelectedIndex = 0;
            comboBoxSummonerStatus.Width = Double.NaN;
            comboBoxSummonerStatus.Height = Double.NaN;
            comboBoxSummonerStatus.HorizontalContentAlignment = HorizontalAlignment.Left;
            comboBoxSummonerStatus.VerticalContentAlignment = VerticalAlignment.Center;
            comboBoxSummonerStatus.IsEditable = false;
            comboBoxSummonerStatus.SelectionChanged += ComboBoxSummonerStatus_SelectionChanged;
            //buttonRune.Text = "RUNES";
            //buttonRune.Location = new Point(labelSummonerDisplayName.Location.X + labelSummonerDisplayName.Width, labelSummonerDisplayName.Location.Y);

            checkBoxAutoPick.Width = Double.NaN;
            checkBoxAutoPick.Height = Double.NaN;
            checkBoxAutoPick.Content = "Auto pick";
            checkBoxAutoPick.HorizontalContentAlignment = HorizontalAlignment.Left;
            checkBoxAutoPick.VerticalContentAlignment = VerticalAlignment.Center;
            checkBoxAutoPick.Click += CheckBoxAutoPick_Click;

            checkBoxAutoPrePick.Width = Double.NaN;
            checkBoxAutoPrePick.Width = Double.NaN;
            checkBoxAutoPrePick.Content = "Auto pre-pick";
            checkBoxAutoPrePick.HorizontalContentAlignment = HorizontalAlignment.Left;
            checkBoxAutoPrePick.VerticalContentAlignment = VerticalAlignment.Center;
            checkBoxAutoPrePick.Click += CheckBoxAutoPrePick_Click; ;

            checkBoxAutoBan.Width = Double.NaN;
            checkBoxAutoBan.Width = Double.NaN;
            checkBoxAutoBan.Content = "Auto ban";
            checkBoxAutoBan.HorizontalContentAlignment = HorizontalAlignment.Left;
            checkBoxAutoBan.VerticalContentAlignment = VerticalAlignment.Center;
            checkBoxAutoBan.Click += CheckBoxAutoBan_Click;

            checkBoxAutoAramBenchSwap.Width = Double.NaN;
            checkBoxAutoAramBenchSwap.Width = Double.NaN;
            checkBoxAutoAramBenchSwap.Content = "Auto ARAM bench swap";
            checkBoxAutoAramBenchSwap.HorizontalContentAlignment = HorizontalAlignment.Left;
            checkBoxAutoAramBenchSwap.VerticalContentAlignment = VerticalAlignment.Center;
            checkBoxAutoAramBenchSwap.Click += CheckBoxAutoAramBenchSwap_Click;

            checkBoxAutoAccept.Width = Double.NaN;
            checkBoxAutoAccept.Width = Double.NaN;
            checkBoxAutoAccept.Content = "Auto accept";
            checkBoxAutoAccept.HorizontalContentAlignment = HorizontalAlignment.Left;
            checkBoxAutoAccept.VerticalContentAlignment = VerticalAlignment.Center;
            checkBoxAutoAccept.Click += CheckBoxAutoAccept_Click;

            checkBoxAutoQPlayAgain.Width = Double.NaN;
            checkBoxAutoQPlayAgain.Width = Double.NaN;
            checkBoxAutoQPlayAgain.Content = "Re Q/Play";
            checkBoxAutoQPlayAgain.HorizontalContentAlignment = HorizontalAlignment.Left;
            checkBoxAutoQPlayAgain.VerticalContentAlignment = VerticalAlignment.Center;
            checkBoxAutoQPlayAgain.Click += CheckBoxAutoQPlayAgain_Click;

            checkBoxAutoHonor.Width = Double.NaN;
            checkBoxAutoHonor.Width = Double.NaN;
            checkBoxAutoHonor.Content = "Auto honor";
            checkBoxAutoHonor.HorizontalContentAlignment = HorizontalAlignment.Left;
            checkBoxAutoHonor.VerticalContentAlignment = VerticalAlignment.Center;
            checkBoxAutoHonor.Click += CheckBoxAutoHonor_Click;

            buttonDelays.Width = Double.NaN;
            buttonDelays.Width = Double.NaN;
            buttonDelays.Content = "Delays";
            buttonDelays.HorizontalContentAlignment = HorizontalAlignment.Center;
            buttonDelays.VerticalContentAlignment = VerticalAlignment.Center;
            buttonDelays.Click += ButtonDelays_Click;

            checkBoxAutoPosition.Width = Double.NaN;
            checkBoxAutoPosition.Width = Double.NaN;
            checkBoxAutoPosition.Content = "Auto role";
            checkBoxAutoPosition.HorizontalContentAlignment = HorizontalAlignment.Left;
            checkBoxAutoPosition.VerticalContentAlignment = VerticalAlignment.Center;
            checkBoxAutoPosition.Click += CheckBoxAutoPosition_Click; ;

            comboBoxAutoPosition1.Width = Double.NaN;
            comboBoxAutoPosition1.Height = Double.NaN;
            comboBoxAutoPosition1.HorizontalContentAlignment = HorizontalAlignment.Left;
            comboBoxAutoPosition1.VerticalContentAlignment = VerticalAlignment.Center;
            comboBoxAutoPosition2.VerticalContentAlignment = VerticalAlignment.Center;
            comboBoxAutoPosition2.HorizontalContentAlignment = HorizontalAlignment.Left;
            comboBoxAutoPosition2.Width = Double.NaN;
            comboBoxAutoPosition2.Height = Double.NaN;
            foreach (string str in postions)
            {
                comboBoxAutoPosition1.Items.Add(str);
            }
            foreach (string str in postions)
            {
                comboBoxAutoPosition2.Items.Add(str);
            }
            if (configData.autoRolePosition1 != null)
            {
                comboBoxAutoPosition1.SelectedItem = configData.autoRolePosition1;
            }
            else { comboBoxAutoPosition1.SelectedItem = postions[0]; }
            if (configData.autoRolePosition2 != null)
            {
                comboBoxAutoPosition2.SelectedItem = configData.autoRolePosition2;
            }
            else { comboBoxAutoPosition2.SelectedItem = postions[1]; }
            comboBoxAutoPosition1.SelectionChanged += ComboBoxAutoPosition1_SelectionChanged;
            comboBoxAutoPosition2.SelectionChanged += ComboBoxAutoPosition2_SelectionChanged;

            comboBoxSummonerStatus.ItemsSource = summonerStatus;
            comboBoxSummonerStatus.DisplayMemberPath = "Name";
            comboBoxSummonerStatus.SelectedValuePath = "Id";
            comboBoxSummonerStatus.SelectedIndex = 0;
            comboBoxSummonerStatus.Width = Double.NaN;
            comboBoxSummonerStatus.Height = Double.NaN;
            comboBoxSummonerStatus.HorizontalContentAlignment = HorizontalAlignment.Left;
            comboBoxSummonerStatus.VerticalContentAlignment = VerticalAlignment.Center;
            comboBoxSummonerStatus.IsEditable = false;
            comboBoxSummonerStatus.SelectionChanged += ComboBoxSummonerStatus_SelectionChanged;

            checkBoxAutoReroll.Width = Double.NaN;
            checkBoxAutoReroll.Width = Double.NaN;
            checkBoxAutoReroll.Content = "Auto reroll";
            checkBoxAutoReroll.HorizontalContentAlignment = HorizontalAlignment.Left;
            checkBoxAutoReroll.VerticalContentAlignment = VerticalAlignment.Center;
            checkBoxAutoReroll.Click += CheckBoxAutoReroll_Click;

            checkBoxAutoSkin.Width = Double.NaN;
            checkBoxAutoSkin.Width = Double.NaN;
            checkBoxAutoSkin.Content = "Auto skin";
            checkBoxAutoSkin.HorizontalContentAlignment = HorizontalAlignment.Left;
            checkBoxAutoSkin.VerticalContentAlignment = VerticalAlignment.Center;
            checkBoxAutoSkin.Click += CheckBoxAutoSkin_Click;

            checkBoxAutoMessage.Width = Double.NaN;
            checkBoxAutoMessage.Width = Double.NaN;
            checkBoxAutoMessage.Content = "Auto message";
            checkBoxAutoMessage.HorizontalContentAlignment = HorizontalAlignment.Left;
            checkBoxAutoMessage.VerticalContentAlignment = VerticalAlignment.Center;
            checkBoxAutoMessage.Click += CheckBoxAutoMessage_Click1;

            textBoxConversationMessage.Width = Double.NaN;
            textBoxConversationMessage.Width = Double.NaN;
            if (textBoxConversationMessage.Text == "")
            {
                textBoxConversationMessage.Text = "Hello everyone";
            }
            textBoxConversationMessage.HorizontalContentAlignment = HorizontalAlignment.Left;
            textBoxConversationMessage.VerticalContentAlignment = VerticalAlignment.Center;
            textBoxConversationMessage.TextChanged += TextBoxConversationMessage_TextChanged; ;

            //checkBoxAutoRunes.Text = "Auto runes";
            //checkBoxAutoRunes.Location = new Point(comboBoxAutoPosition2.Location.X + comboBoxAutoPosition2.Width + 5, comboBoxAutoPosition2.Location.Y);
            //checkBoxAutoRunes.Click += CheckBoxAutoRunes_Click;
            //comboBoxAutoRunesSources.Location = new Point(checkBoxAutoRunes.Location.X + checkBoxAutoRunes.Width + 5, checkBoxAutoRunes.Location.Y);
            //comboBoxAutoRunesSources.DropDownStyle = ComboBoxStyle.DropDownList;
            //comboBoxAutoRunesSources.Width = 75;
            //comboBoxAutoRunesSources.Items.Add("U.GG");
            //comboBoxAutoRunesSources.SelectedIndex = 0;
            //checkBoxAutoSummoners.Text = "Auto summoners";
            //checkBoxAutoSummoners.Location = new Point(checkBoxAutoRunes.Location.X, checkBoxAutoRunes.Location.Y + checkBoxAutoRunes.Height);
            //checkBoxAutoSummoners.Click += CheckBoxAutoSummoners_Click;
            //checkBoxAutoSummoners.TextAlign = ContentAlignment.MiddleCenter;
            //comboBoxAutoSummonersSources.Location = new Point(checkBoxAutoSummoners.Location.X + checkBoxAutoSummoners.Width + 5, checkBoxAutoSummoners.Location.Y);
            //comboBoxAutoSummonersSources.DropDownStyle = ComboBoxStyle.DropDownList;
            //comboBoxAutoSummonersSources.Width = 75;
            //comboBoxAutoSummonersSources.Items.Add("U.GG");
            //comboBoxAutoSummonersSources.SelectedIndex = 0;

            labelInfo.Width = Double.NaN;
            labelInfo.Width = Double.NaN;
            labelInfo.Content = "INFORMATION";
            labelInfo.HorizontalContentAlignment = HorizontalAlignment.Left;
            labelInfo.VerticalContentAlignment = VerticalAlignment.Center;
            labelInfo.Foreground = System.Windows.Media.Brushes.Blue;
            labelInfo.MouseUp += LabelInfo_MouseUp;

            //formInformation.FormClosing += FormInformation_FormClosing;
            //formInformation.Text = "LOL Client TOOL information";
            //formInformation.Icon = Properties.Resources.LOL_Client_TOOL;
            //formInformation.Width = 400;
            //formInformation.Height = 100;

            //Label labelInformation = new Label();
            //labelInformation.Text = "To follow the tool developement you can look at the Github or join the Discord";
            //labelInformation.AutoSize = true;
            //labelInformation.Location = new Point(5, 5);
            //LinkLabel linkLabelGithub = new LinkLabel();
            //linkLabelGithub.Text = "Github";
            //linkLabelGithub.Location = new Point(labelInformation.Location.X, labelInformation.Height + 5 + labelInformation.Location.Y);
            //LinkLabel linkLabelDiscord = new LinkLabel();
            //linkLabelDiscord.Text = "Discord";
            //linkLabelDiscord.Location = new Point(labelInformation.Location.X + linkLabelGithub.Width, linkLabelGithub.Location.Y);

            //formInformation.Controls.Add(labelInformation);
            //formInformation.Controls.Add(linkLabelGithub);
            //formInformation.Controls.Add(linkLabelDiscord);

            //linkLabelGithub.Click += LinkLabelGithub_Click;
            //linkLabelDiscord.Click += LinkLabelDiscord_Click;

            //buttonRune.Click += buttonRune_Click;
            //tooltip.AutoPopDelay = 32767;
            //tooltip.UseAnimation = false;
            //tooltip.UseFading = false;
            //tooltip.InitialDelay = 0;
            //tooltip.ReshowDelay = 0;
            //tooltip.ShowAlways = true;
            //tooltip.ToolTipIcon = ToolTipIcon.Info;
            //tooltip.SetToolTip(labelSummonerDisplayName, "Copy to clipboard");
            summonerIcon.Source = getIcon(currentSummoner.profileIconId);
            Grid.SetRow(summonerIcon, 1);
            Grid.SetColumn(summonerIcon, 0);
            Grid.SetRowSpan(summonerIcon, 4);
            //int iconSize = 90;
            //summonerIcon.Size = new Size(iconSize, iconSize);
            //summonerIcon.Location = new Point(labelSummonerDisplayName.Location.X, labelSummonerDisplayName.Location.Y + labelSummonerDisplayName.Height);
            //tooltip.SetToolTip(summonerIcon, summonerIcon.Name);
            //verifyOwnedIcons.Text = "Only show owned icons";
            //verifyOwnedIcons.Checked = true;
            //verifyOwnedIcons.AutoSize = true;
            //verifyOwnedIcons.Location = new Point(summonerIcon.Location.X, summonerIcon.Location.Y + iconSize);
            //FormSummonerIcon.SizeChanged += FormSummonerIcon_SizeChanged;
            //apiEnpoint.Location = new Point(summonerIcon.Location.X, summonerIcon.Location.Y + iconSize);
            //apiEnpoint.Width = 300;
            //apiEndpointCall.Height = apiEndpointCall.Height - 1;
            //apiEndpointCall.Location = new Point(summonerIcon.Location.X + apiEnpoint.Width, summonerIcon.Location.Y + iconSize - 1);
            //apiEndpointCall.Text = "REQUEST";
            //apiEndpointCall.Click += ApiEndpointCall_Click;
            //apiRequestType.Location = new Point(summonerIcon.Location.X + apiEnpoint.Width + apiEndpointCall.Width, summonerIcon.Location.Y + iconSize);
            //apiRequestJson.Multiline = true;
            //apiRequestJson.Location = new Point(summonerIcon.Location.X, apiEnpoint.Location.Y + apiEnpoint.Height + 10);
            //apiRequestJson.Size = new Size(apiEnpoint.Width + apiEndpointCall.Width + apiRequestType.Width, 200);
            //apiEndPointResponse.Multiline = true;
            //apiEndPointResponse.Location = new Point(summonerIcon.Location.X + apiEnpoint.Width + apiEndpointCall.Width + apiRequestType.Width + 5, summonerIcon.Location.Y + iconSize);
            //apiEndPointResponse.Size = new Size(300, 230);

            //formDelays.Text = "Delays";
            //formDelays.Icon = Properties.Resources.LOL_Client_TOOL;
            //formDelays.FormClosing += FormDelays_FormClosing;
            //formDelays.Height = 115;
            //Label labelPrePickDelay = new Label();
            //labelPrePickDelay.Text = "Pre-pick delay (ms) : ";
            //labelPrePickDelay.Location = new Point(5, 5);
            //NumericUpDown numericUpDownPrePickDelay = new NumericUpDown();
            //numericUpDownPrePickDelay.Maximum = 25000;
            //numericUpDownPrePickDelay.Minimum = 0;
            //numericUpDownPrePickDelay.Increment = 333;
            //if (configData != null)
            //{
            //    numericUpDownPrePickDelay.Value = configData.prePickDelay;
            //}
            //numericUpDownPrePickDelay.Location = new Point(labelPrePickDelay.Location.X + labelPrePickDelay.Width + 5, labelPrePickDelay.Location.Y);
            //numericUpDownPrePickDelay.ValueChanged += NumericUpDownPrePickDelay_ValueChanged;
            //Label labelPickDelay = new Label();
            //labelPickDelay.Text = "Pick delay (ms) : ";
            //labelPickDelay.Location = new Point(labelPrePickDelay.Location.X, labelPrePickDelay.Location.Y + labelPrePickDelay.Height);
            //NumericUpDown numericUpDownPickDelay = new NumericUpDown();
            //numericUpDownPickDelay.Maximum = 25000;
            //numericUpDownPickDelay.Minimum = 0;
            //numericUpDownPickDelay.Increment = 333;
            //if (configData != null)
            //{
            //    numericUpDownPickDelay.Value = configData.pickDelay;
            //}
            //numericUpDownPickDelay.Location = new Point(labelPickDelay.Location.X + labelPickDelay.Width + 5, labelPickDelay.Location.Y);
            //numericUpDownPickDelay.ValueChanged += NumericUpDownPickDelay_ValueChanged;
            //Label labelBanDelay = new Label();
            //labelBanDelay.Text = "Ban delay (ms) : ";
            //labelBanDelay.Location = new Point(labelPickDelay.Location.X, labelPickDelay.Location.Y + labelPickDelay.Height);
            //NumericUpDown numericUpDownBanDelay = new NumericUpDown();
            //numericUpDownBanDelay.Maximum = 25000;
            //numericUpDownBanDelay.Increment = 333;
            //if (configData != null)
            //{
            //    numericUpDownBanDelay.Value = configData.banDelay;
            //}
            //numericUpDownBanDelay.Location = new Point(labelBanDelay.Location.X + labelBanDelay.Width + 5, labelBanDelay.Location.Y);
            //numericUpDownBanDelay.ValueChanged += NumericUpDownBanDelay_ValueChanged;

            //formDelays.Controls.Add(labelPrePickDelay);
            //formDelays.Controls.Add(numericUpDownPrePickDelay);
            //formDelays.Controls.Add(labelPickDelay);
            //formDelays.Controls.Add(numericUpDownPickDelay);
            //formDelays.Controls.Add(labelBanDelay);
            //formDelays.Controls.Add(numericUpDownBanDelay);
        }

        private static void LabelInfo_MouseUp(object sender, MouseButtonEventArgs e)
        {
            formInformation.Show();
        }

        private static void TextBoxConversationMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            configData.autoMessageText = textBoxConversationMessage.Text;
            configSaveAsync();
        }

        private static void CheckBoxAutoMessage_Click1(object sender, RoutedEventArgs e)
        {
            configData.autoMessage = (bool)checkBoxAutoMessage.IsChecked;
            configSaveAsync();
        }

        private static void ComboBoxAutoPosition2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxAutoPosition1.SelectedItem != null && comboBoxAutoPosition2.SelectedItem != null)
            {
                if (comboBoxAutoPosition1.SelectedItem == comboBoxAutoPosition2.SelectedItem)
                {
                    comboBoxAutoPosition1.SelectedItem = position2;
                }
                position2 = comboBoxAutoPosition2.SelectedItem.ToString();
                configData.autoRolePosition2 = position2;
                configSaveAsync();
            }
        }

        private static void ComboBoxAutoPosition1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxAutoPosition1.SelectedItem != null && comboBoxAutoPosition2.SelectedItem != null)
            {
                if (comboBoxAutoPosition2.SelectedItem == comboBoxAutoPosition1.SelectedItem)
                {
                    comboBoxAutoPosition2.SelectedItem = position1;
                }
                position1 = comboBoxAutoPosition1.SelectedItem.ToString();
                configData.autoRolePosition1 = position1;
                configSaveAsync();
            }
        }

        private static void CheckBoxAutoSkin_Click(object sender, RoutedEventArgs e)
        {
            configData.autoSkin = (bool)checkBoxAutoSkin.IsChecked;
            configSaveAsync();
        }

        private static void CheckBoxAutoReroll_Click(object sender, RoutedEventArgs e)
        {
            configData.autoReroll = (bool)checkBoxAutoReroll.IsChecked;
            configSaveAsync();
        }

        private static void CheckBoxAutoMessage_Click(object sender, RoutedEventArgs e)
        {
            configData.autoMessage = (bool)checkBoxAutoMessage.IsChecked;
            configSaveAsync();
        }

        private static void CheckBoxAutoPosition_Click(object sender, RoutedEventArgs e)
        {
            configData.autoRole = (bool)checkBoxAutoPosition.IsChecked;
            configSaveAsync();
        }

        private static void ButtonDelays_Click(object sender, RoutedEventArgs e)
        {
            formDelays.Show();
        }

        private static void CheckBoxAutoHonor_Click(object sender, RoutedEventArgs e)
        {
            configData.autoHonor = (bool)checkBoxAutoHonor.IsChecked;
            configSaveAsync();
        }

        private static void CheckBoxAutoQPlayAgain_Click(object sender, RoutedEventArgs e)
        {
            configData.autoPlayAgain = (bool)checkBoxAutoQPlayAgain.IsChecked;
            configSaveAsync();
        }

        private static void CheckBoxAutoAccept_Click(object sender, RoutedEventArgs e)
        {
            configData.autoAccept = (bool)checkBoxAutoAccept.IsChecked;
            configSaveAsync();
        }

        private static void CheckBoxAutoAramBenchSwap_Click(object sender, RoutedEventArgs e)
        {
            configData.autoAramBenchSwap = (bool)checkBoxAutoAramBenchSwap.IsChecked;
            configSaveAsync();
        }

        private static void CheckBoxAutoBan_Click(object sender, RoutedEventArgs e)
        {
            configData.autoBan = (bool)checkBoxAutoBan.IsChecked;
            configSaveAsync();
        }

        private static void CheckBoxAutoPrePick_Click(object sender, RoutedEventArgs e)
        {
            configData.autoPrePick = (bool)checkBoxAutoPrePick.IsChecked;
            configSaveAsync();
        }

        private static void CheckBoxAutoPick_Click(object sender, RoutedEventArgs e)
        {
            configData.autoPick = (bool)checkBoxAutoPick.IsChecked;
            configSaveAsync();
        }

        private static void ComboBoxSummonerStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (ComboBoxIdName combo in summonerStatus)
            {
                if (comboBoxSummonerStatus.SelectedIndex == combo.Id)
                {
                    LCURequest("/lol-chat/v1/me", "PUT", "{\"availability\": \"" + combo.Name.ToLower() + "\"}");
                }
            }
        }

        public static void FormChampionsSetup()
        {
            //formChampions.Visible = false;
            formChampions.Title = "Champions Pick Ban and ARAM priorities";
            Font LargeFont = new Font(new FontFamily("Arial"), 12, System.Drawing.FontStyle.Bold);
            Font SmaleFont = new Font(new FontFamily("Arial"), 10, System.Drawing.FontStyle.Bold);

            ScrollViewer myScrollViewer = new ScrollViewer();
            myScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            myScrollViewer.Content = gridChampions;

            formChampions.Content = myScrollViewer;

            TextBox textBoxFilterChampions = new TextBox();
            Button buttonResetPicks = new Button();
            Button buttonResetBans = new Button();
            Button buttonResetAram = new Button();

            textBoxFilterChampions.Text = "Search champion";

            textBoxFilterChampions.GotKeyboardFocus += TextBoxFilterChampions_GotKeyboardFocus; ;

            while (gridChampions.RowDefinitions.Count < 2)
            {
                RowDefinition myRow = new RowDefinition();
                myRow.MaxHeight = 25;
                myRow.MinHeight = 25;
                gridChampions.RowDefinitions.Add(myRow);
            }

            buttonResetPicks.Content = "RESET PICKS";
            buttonResetBans.Content = "RESET BANS";
            buttonResetAram.Content = "RESET ARAM";
            buttonResetPicks.FontFamily = new System.Windows.Media.FontFamily("Arial");
            buttonResetPicks.FontSize = SmaleFont.Size;
            buttonResetPicks.FontWeight = System.Windows.FontWeights.Bold;
            buttonResetBans.FontFamily = new System.Windows.Media.FontFamily("Arial");
            buttonResetBans.FontSize = SmaleFont.Size;
            buttonResetBans.FontWeight = System.Windows.FontWeights.Bold;
            buttonResetAram.FontFamily = new System.Windows.Media.FontFamily("Arial");
            buttonResetAram.FontSize = SmaleFont.Size;
            buttonResetAram.FontWeight = System.Windows.FontWeights.Bold;
            buttonResetPicks.Width = Double.NaN;
            buttonResetBans.Width = Double.NaN;
            buttonResetAram.Width = Double.NaN;
            buttonResetPicks.Height = Double.NaN;
            buttonResetBans.Height = Double.NaN;
            buttonResetAram.Height = Double.NaN;

            buttonResetPicks.Click += ButtonResetPicks_Click; ;
            buttonResetBans.Click += ButtonResetBans_Click; ;
            buttonResetAram.Click += ButtonResetAram_Click; ;
            textBoxFilterChampions.TextChanged += TextBoxFilterChampions_TextChanged; ;

            gridChampions.Children.Add(textBoxFilterChampions);
            gridChampions.Children.Add(buttonResetPicks);
            gridChampions.Children.Add(buttonResetBans);
            gridChampions.Children.Add(buttonResetAram);

            Grid.SetColumn(textBoxFilterChampions, 0);
            Grid.SetRow(textBoxFilterChampions, 0);
            Grid.SetColumn(buttonResetPicks, 1);
            Grid.SetRow(buttonResetPicks, 0);
            Grid.SetColumn(buttonResetBans, 2);
            Grid.SetRow(buttonResetBans, 0);
            Grid.SetColumn(buttonResetAram, 3);
            Grid.SetRow(buttonResetAram, 0);

            List<Label> labelChampions = new List<Label>();
            List<IntegerUpDown> pick = new List<IntegerUpDown>();
            List<IntegerUpDown> ban = new List<IntegerUpDown>();
            List<IntegerUpDown> aram = new List<IntegerUpDown>();
            List<Label> colNames = new List<Label>();
            List<string> colText = new List<string>();
            colText.Add("CHAMPION");
            colText.Add("PICK");
            colText.Add("BAN");
            colText.Add("ARAM");

            int column = 0, row = 1, index = 0;

            foreach (string text in colText)
            {
                colNames.Add(new Label());
                colNames[index].Content = text;
                colNames[index].FontWeight = System.Windows.FontWeights.Bold;
                colNames[index].FontFamily = new System.Windows.Media.FontFamily("Arial");
                colNames[index].FontSize = 12;
                colNames[index].VerticalAlignment = VerticalAlignment.Center;
                colNames[index].HorizontalAlignment = HorizontalAlignment.Center;

                gridChampions.Children.Add(colNames[index]);
                Grid.SetRow(colNames[index], row);
                Grid.SetColumn(colNames[index], column);
                column++;
                colNames[index].Width = Double.NaN;
                colNames[index].Height = Double.NaN;
                index++;
            }

            row = 2;
            column = 0;
            index = 0;
            foreach (champion champ in leagueOfLegendsChampions)
            {
                RowDefinition myRow = new RowDefinition();
                myRow.MaxHeight = 25;
                myRow.MinHeight = 25;
                gridChampions.RowDefinitions.Add(myRow);

                labelChampions.Add(new Label());
                gridChampions.Children.Add(labelChampions[index]);
                labelChampions[index].Content = champ.name;
                labelChampions[index].FontWeight = System.Windows.FontWeights.Bold;
                labelChampions[index].FontFamily = new System.Windows.Media.FontFamily("Arial");
                labelChampions[index].FontSize = 12;
                Grid.SetRow(labelChampions[index], row);
                Grid.SetColumn(labelChampions[index], column);
                labelChampions[index].ToolTip = champ.key;
                labelChampions[index].Width = Double.NaN;
                labelChampions[index].Height = Double.NaN;

                column++;

                pick.Add(new IntegerUpDown());
                gridChampions.Children.Add(pick[index]);
                Grid.SetRow(pick[index], row);
                Grid.SetColumn(pick[index], column);
                pick[index].ToolTip = champ.key + "pick";
                pick[index].Value = 0;
                pick[index].Minimum = 0;
                pick[index].Maximum = 200;
                pick[index].Increment = 1;
                pick[index].Width = Double.NaN;
                pick[index].Height = Double.NaN;
                //pick[index].MouseWheel += FormChampions_MouseWheel;

                column++;

                ban.Add(new IntegerUpDown());
                gridChampions.Children.Add(ban[index]);
                Grid.SetRow(ban[index], row);
                Grid.SetColumn(ban[index], column);
                ban[index].ToolTip = champ.key + "ban";
                ban[index].Value = 0;
                ban[index].Minimum = 0;
                ban[index].Maximum = 200;
                ban[index].Increment = 1;
                ban[index].Width = Double.NaN;
                ban[index].Height = Double.NaN;
                //ban[index].MouseWheel += FormChampions_MouseWheel;

                column++;

                aram.Add(new IntegerUpDown());
                gridChampions.Children.Add(aram[index]);
                Grid.SetRow(aram[index], row);
                Grid.SetColumn(aram[index], column);
                aram[index].ToolTip = champ.key + "aram";
                aram[index].Value = 0;
                aram[index].Minimum = 0;
                aram[index].Maximum = 200;
                aram[index].Increment = 1;
                aram[index].Width = Double.NaN;
                aram[index].Height = Double.NaN;
                //aram[index].MouseWheel += FormChampions_MouseWheel;



                if (configData.championPickList != null)
                {
                    int found = 0;
                    foreach (championPick data in configData.championPickList)
                    {
                        if (data.Id.ToString() == champ.key)
                        {
                            pick[index].Value = data.Pick;
                            ban[index].Value = data.Ban;
                            aram[index].Value = data.Aram;
                            found = 1;
                        }
                    }
                    if (found == 0)
                    {
                        championPick tempPick = new championPick();
                        tempPick.Id = Convert.ToInt32(champ.key);
                        tempPick.Pick = Convert.ToInt32(pick[index].Value);
                        tempPick.Ban = Convert.ToInt32(ban[index].Value);
                        tempPick.Aram = Convert.ToInt32(aram[index].Value);
                        configData.championPickList.Add(tempPick);
                    }
                }
                else
                {
                    if (configData.championPickList == null)
                    {
                        configData.championPickList = new List<championPick>();
                    }
                    championPick tempPick = new championPick();
                    tempPick.Id = Convert.ToInt32(champ.key);
                    tempPick.Pick = Convert.ToInt32(pick[index].Value);
                    tempPick.Ban = Convert.ToInt32(ban[index].Value);
                    tempPick.Aram = Convert.ToInt32(aram[index].Value);
                    configData.championPickList.Add(tempPick);
                }

                pick[index].ValueChanged += MainWindow_ValueChanged; ;
                ban[index].ValueChanged += MainWindow_ValueChanged1; ;
                aram[index].ValueChanged += MainWindow_ValueChanged2; ;

                column = 0;
                row++;
                index++;
            }
        }
        public static void FormInformationSetup()
        {
            formInformation.Title = "Informations";
            formInformation.Closing += FormInformation_Closing;
            formInformation.MaxWidth = 450;
            formInformation.MaxHeight = 100;
            formInformation.Content = gridInformations;
            gridInformations.ShowGridLines = debug;

            Label labelInformation = new Label();
            labelInformation.Content = "To follow the tool developement you can look at the Github or join the Discord";
            labelInformation.Width = Double.NaN;
            labelInformation.Height = Double.NaN;
            labelInformation.VerticalAlignment = VerticalAlignment.Center;

            Label linkLabelGithub = new Label();
            linkLabelGithub.Content = "Github";
            linkLabelGithub.Width = Double.NaN;
            linkLabelGithub.Height = Double.NaN;
            linkLabelGithub.Foreground = System.Windows.Media.Brushes.Blue;
            linkLabelGithub.MouseLeftButtonUp += LinkLabelGithub_MouseLeftButtonUp;
            linkLabelGithub.VerticalAlignment = VerticalAlignment.Center;
            linkLabelGithub.HorizontalAlignment = HorizontalAlignment.Left;

            Label linkLabelDiscord = new Label();
            linkLabelDiscord.Content = "Discord";
            linkLabelDiscord.Width = Double.NaN;
            linkLabelDiscord.Height = Double.NaN;
            linkLabelDiscord.Foreground = System.Windows.Media.Brushes.Blue;
            linkLabelDiscord.MouseLeftButtonUp += LinkLabelDiscord_MouseLeftButtonUp;
            linkLabelDiscord.VerticalAlignment = VerticalAlignment.Center;
            linkLabelDiscord.HorizontalAlignment = HorizontalAlignment.Left;

            gridInformations.Children.Add(labelInformation);
            gridInformations.Children.Add(linkLabelGithub);
            gridInformations.Children.Add(linkLabelDiscord);

            gridInformations.ColumnDefinitions.Add(new ColumnDefinition());
            gridInformations.ColumnDefinitions.Add(new ColumnDefinition());
            gridInformations.ColumnDefinitions.Add(new ColumnDefinition());
            gridInformations.ColumnDefinitions.Add(new ColumnDefinition());
            gridInformations.ColumnDefinitions.Add(new ColumnDefinition());
            gridInformations.RowDefinitions.Add(new RowDefinition());
            gridInformations.RowDefinitions.Add(new RowDefinition());

            Grid.SetColumn(labelInformation, 0);
            Grid.SetColumnSpan(labelInformation, 5);
            Grid.SetRow(labelInformation, 0);
            Grid.SetColumn(linkLabelGithub, 0);
            Grid.SetRow(linkLabelGithub, 1);
            Grid.SetColumn(linkLabelDiscord, 1);
            Grid.SetRow(linkLabelDiscord, 1);
        }

        public static void FormDelaysSetup()
        {
            formDelays.Title = "Setup pre-pick, pick and ban delays";
            formDelays.Closing += FormDelays_Closing;
            formDelays.SizeToContent = SizeToContent.WidthAndHeight;
            formDelays.Content = gridDelays;
            gridDelays.ShowGridLines = debug;

            Label labelPrePick = new Label();
            labelPrePick.Content = "Pre-pick delay (ms) :";
            labelPrePick.Width = Double.NaN;
            labelPrePick.Height = Double.NaN;
            labelPrePick.VerticalAlignment = VerticalAlignment.Center;
            labelPrePick.HorizontalAlignment = HorizontalAlignment.Left;

            Label labelPick = new Label();
            labelPick.Content = "Pick delay (ms) :";
            labelPick.Width = Double.NaN;
            labelPick.Height = Double.NaN;
            labelPick.VerticalAlignment = VerticalAlignment.Center;
            labelPick.HorizontalAlignment = HorizontalAlignment.Left;

            Label labelBan = new Label();
            labelBan.Content = "Ban delay (ms) :";
            labelBan.Width = Double.NaN;
            labelBan.Height = Double.NaN;
            labelBan.VerticalAlignment = VerticalAlignment.Center;
            labelBan.HorizontalAlignment = HorizontalAlignment.Left;

            IntegerUpDown integerUpDownPrePickDelay = new IntegerUpDown();
            integerUpDownPrePickDelay.DefaultValue = 0;
            integerUpDownPrePickDelay.Maximum = 25000;
            integerUpDownPrePickDelay.Minimum = 0;
            integerUpDownPrePickDelay.Increment = 1000;
            integerUpDownPrePickDelay.Width = Double.NaN;
            integerUpDownPrePickDelay.Height = Double.NaN;
            integerUpDownPrePickDelay.VerticalAlignment = VerticalAlignment.Center;
            integerUpDownPrePickDelay.HorizontalAlignment = HorizontalAlignment.Left;
            integerUpDownPrePickDelay.ValueChanged += IntegerUpDownPrePickDelay_ValueChanged;

            IntegerUpDown integerUpDownPickDelay = new IntegerUpDown();
            integerUpDownPickDelay.DefaultValue = 0;
            integerUpDownPickDelay.Maximum = 25000;
            integerUpDownPickDelay.Minimum = 0;
            integerUpDownPickDelay.Increment = 1000;
            integerUpDownPickDelay.Width = Double.NaN;
            integerUpDownPickDelay.Height = Double.NaN;
            integerUpDownPickDelay.VerticalAlignment = VerticalAlignment.Center;
            integerUpDownPickDelay.HorizontalAlignment = HorizontalAlignment.Left;
            integerUpDownPickDelay.ValueChanged += IntegerUpDownPickDelay_ValueChanged;

            IntegerUpDown integerUpDownBanDelay = new IntegerUpDown();
            integerUpDownBanDelay.DefaultValue = 0;
            integerUpDownBanDelay.Maximum = 25000;
            integerUpDownBanDelay.Minimum = 0;
            integerUpDownBanDelay.Increment = 1000;
            integerUpDownBanDelay.Width = Double.NaN;
            integerUpDownBanDelay.Height = Double.NaN;
            integerUpDownBanDelay.VerticalAlignment = VerticalAlignment.Center;
            integerUpDownBanDelay.HorizontalAlignment = HorizontalAlignment.Left;
            integerUpDownBanDelay.ValueChanged += IntegerUpDownBanDelay_ValueChanged;

            if (configData.prePickDelay != null) { integerUpDownPrePickDelay.Value = configData.prePickDelay; }
            if (configData.pickDelay != null) { integerUpDownPickDelay.Value = configData.pickDelay; }
            if (configData.banDelay != null) { integerUpDownBanDelay.Value = configData.banDelay; }



            gridDelays.Children.Add(labelPrePick);
            gridDelays.Children.Add(labelPick);
            gridDelays.Children.Add(labelBan);
            gridDelays.Children.Add(integerUpDownPrePickDelay);
            gridDelays.Children.Add(integerUpDownPickDelay);
            gridDelays.Children.Add(integerUpDownBanDelay);

            gridDelays.ColumnDefinitions.Add(new ColumnDefinition());
            gridDelays.ColumnDefinitions.Add(new ColumnDefinition());
            gridDelays.RowDefinitions.Add(new RowDefinition());
            gridDelays.RowDefinitions.Add(new RowDefinition());
            gridDelays.RowDefinitions.Add(new RowDefinition());

            Grid.SetColumn(labelPrePick, 0);
            Grid.SetRow(labelPrePick, 0);
            Grid.SetColumn(labelPick, 0);
            Grid.SetRow(labelPick, 1);
            Grid.SetColumn(labelBan, 0);
            Grid.SetRow(labelBan, 2);
            Grid.SetColumn(integerUpDownPrePickDelay, 1);
            Grid.SetRow(integerUpDownPrePickDelay, 0);
            Grid.SetColumn(integerUpDownPickDelay, 1);
            Grid.SetRow(integerUpDownPickDelay, 1);
            Grid.SetColumn(integerUpDownBanDelay, 1);
            Grid.SetRow(integerUpDownBanDelay, 2);

        }

        private static void IntegerUpDownBanDelay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            IntegerUpDown numericUpDown = (IntegerUpDown)sender;
            configData.banDelay = (int)numericUpDown.Value;
            configSaveAsync();
        }

        private static void IntegerUpDownPickDelay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            IntegerUpDown numericUpDown = (IntegerUpDown)sender;
            configData.pickDelay = (int)numericUpDown.Value;
            configSaveAsync();
        }

        private static void IntegerUpDownPrePickDelay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            IntegerUpDown numericUpDown = (IntegerUpDown)sender;
            configData.prePickDelay = (int)numericUpDown.Value;
            configSaveAsync();
        }

        private static void FormDelays_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            formDelays.Hide();
        }

        private static void FormInformation_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            formInformation.Hide();
        }

        private static void LinkLabelDiscord_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var startInfo = new ProcessStartInfo("explorer.exe", "https://discord.gg/ZE7fZrFeJd");
            Process.Start(startInfo);
        }

        private static void LinkLabelGithub_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var startInfo = new ProcessStartInfo("explorer.exe", "https://github.com/Terevenen2/LOL-Client-TOOL");
            Process.Start(startInfo);
        }

        private static void TextBoxFilterChampions_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox filter = (TextBox)sender;
            if (filter.Text == "Search champion")
            {
                filter.Text = "";
            }
        }

        private static void MainWindow_ValueChanged2(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            IntegerUpDown numericUpDown = (IntegerUpDown)sender;
            for (int i = 0; i < configData.championPickList.Count; i++)
            {
                if (configData.championPickList[i].Id.ToString() == Regex.Replace(numericUpDown.ToolTip.ToString(), "[^0-9.]", ""))
                {
                    configData.championPickList[i].Aram = Convert.ToInt32(numericUpDown.Value);
                }
            }
            configSaveAsync();
        }

        private static void MainWindow_ValueChanged1(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            IntegerUpDown numericUpDown = (IntegerUpDown)sender;
            for (int i = 0; i < configData.championPickList.Count; i++)
            {
                if (configData.championPickList[i].Id.ToString() == Regex.Replace(numericUpDown.ToolTip.ToString(), "[^0-9.]", ""))
                {
                    configData.championPickList[i].Ban = Convert.ToInt32(numericUpDown.Value);
                }
            }
            configSaveAsync();
        }

        private static void MainWindow_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            IntegerUpDown numericUpDown = (IntegerUpDown)sender;
            for (int i = 0; i < configData.championPickList.Count; i++)
            {
                if (configData.championPickList[i].Id.ToString() == Regex.Replace(numericUpDown.ToolTip.ToString(), "[^0-9.]", ""))
                {
                    configData.championPickList[i].Pick = Convert.ToInt32(numericUpDown.Value);
                }
            }
            configSaveAsync();
        }

        private static void TextBoxFilterChampions_TextChanged(object sender, TextChangedEventArgs e)
        {
            var watch = new System.Diagnostics.Stopwatch();
            List<string> times = new List<string>();

            TextBox theSearch = (TextBox)sender;
            int row = 2, index = 0, count2 = 0;
            var labels = gridChampions.Children.OfType<Label>();
            var numericUpDowns = gridChampions.Children.OfType<IntegerUpDown>();
            watch.Start();
            foreach (var label in labels)
            {
                if (label.Content.ToString().ToLower().Contains(theSearch.Text.ToLower()) && label.ToolTip != null)
                {
                    Grid.SetRow(label, row);
                    label.Visibility = Visibility.Visible;

                    var temp2 = numericUpDowns.Where(n => n.ToolTip.ToString().ToLower().Equals(label.ToolTip.ToString().ToLower() + "pick"));
                    Grid.SetRow(temp2.First(), row);
                    temp2.First().Visibility = Visibility.Visible;

                    temp2 = numericUpDowns.Where(n => n.ToolTip.ToString().ToLower().Equals(label.ToolTip.ToString().ToLower() + "ban"));
                    Grid.SetRow(temp2.First(), row);
                    temp2.First().Visibility = Visibility.Visible;

                    temp2 = numericUpDowns.Where(n => n.ToolTip.ToString().ToLower().Equals(label.ToolTip.ToString().ToLower() + "aram"));
                    Grid.SetRow(temp2.First(), row);
                    temp2.First().Visibility = Visibility.Visible;

                    row++;
                }
                else
                {
                    if (label.ToolTip != null)
                    {
                        label.Visibility = Visibility.Hidden;
                        var temp2 = numericUpDowns.Where(n => n.ToolTip.ToString().ToLower().Equals(label.ToolTip.ToString().ToLower() + "pick"));
                        temp2.First().Visibility = Visibility.Hidden;

                        temp2 = numericUpDowns.Where(n => n.ToolTip.ToString().ToLower().Equals(label.ToolTip.ToString().ToLower() + "ban"));
                        temp2.First().Visibility = Visibility.Hidden;

                        temp2 = numericUpDowns.Where(n => n.ToolTip.ToString().ToLower().Equals(label.ToolTip.ToString().ToLower() + "aram"));
                        temp2.First().Visibility = Visibility.Hidden;
                    }
                }
            }
            watch.Stop();
            times.Add(watch.ElapsedMilliseconds.ToString());
        }

        private static void ButtonResetAram_Click(object sender, RoutedEventArgs e)
        {
            foreach (var temp in gridChampions.Children.OfType<IntegerUpDown>().Where(x => x.ToolTip.ToString().Contains("aram")))
            {
                temp.Value = 0;
            }
        }

        private static void ButtonResetBans_Click(object sender, RoutedEventArgs e)
        {
            foreach (var temp in gridChampions.Children.OfType<IntegerUpDown>().Where(x => x.ToolTip.ToString().Contains("ban")))
            {
                temp.Value = 0;
            }
        }

        private static void ButtonResetPicks_Click(object sender, RoutedEventArgs e)
        {
            foreach (var temp in gridChampions.Children.OfType<IntegerUpDown>().Where(x => x.ToolTip.ToString().Contains("pick")))
            {
                temp.Value = 0;
            }
        }

        public static void ButtonChampions_Click(object sender, RoutedEventArgs e)
        {
            formChampions.Show();
            formChampions.Closing += FormChampions_Closing;
            formChampions.Focus();
        }

        private static void FormChampions_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            formChampions.Hide();
        }

        private static void LabelSummonerDisplayName_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetDataObject(labelSummonerDisplayName.Content.ToString());
        }

        public MainWindow()
        {
            isDebug();
            InitializeComponent();
            SetTimer();
            getVersion();
            //this.Controls.Add(mainFormMenu);
            while (gridMain.RowDefinitions.Count < 5)
            {
                gridMain.RowDefinitions.Add(new RowDefinition());
            }
            while (gridMain.ColumnDefinitions.Count < 8)
            {
                gridMain.ColumnDefinitions.Add(new ColumnDefinition());
            }

            windowMain.MinHeight = 160;
            windowMain.Height = 160;
            //gridMain.MinHeight = 125;
            //gridMain.MaxHeight = 125;

            gridMain.ShowGridLines = debug;

            gridMain.Children.Add(labelSummonerDisplayName);
            gridMain.Children.Add(buttonChampions);
            gridMain.Children.Add(summonerIcon);
            gridMain.Children.Add(comboBoxSummonerStatus);
            gridMain.Children.Add(checkBoxAutoPick);
            gridMain.Children.Add(checkBoxAutoPrePick);
            gridMain.Children.Add(checkBoxAutoBan);
            gridMain.Children.Add(checkBoxAutoAramBenchSwap);
            gridMain.Children.Add(checkBoxAutoAccept);
            gridMain.Children.Add(checkBoxAutoQPlayAgain);
            gridMain.Children.Add(checkBoxAutoHonor);
            gridMain.Children.Add(buttonDelays);
            gridMain.Children.Add(checkBoxAutoPosition);
            gridMain.Children.Add(checkBoxAutoMessage);
            gridMain.Children.Add(checkBoxAutoReroll);
            gridMain.Children.Add(checkBoxAutoSkin);
            gridMain.Children.Add(comboBoxAutoPosition1);
            gridMain.Children.Add(textBoxConversationMessage);
            gridMain.Children.Add(comboBoxAutoPosition2);
            gridMain.Children.Add(labelInfo);
            Grid.SetColumn(buttonChampions, 1);
            Grid.SetRow(buttonChampions, 0);
            Grid.SetColumn(labelSummonerDisplayName, 0);
            Grid.SetRow(labelSummonerDisplayName, 0);
            Grid.SetColumn(comboBoxSummonerStatus, 2);
            Grid.SetRow(comboBoxSummonerStatus, 0);
            Grid.SetColumn(checkBoxAutoPick, 1);
            Grid.SetRow(checkBoxAutoPick, 1);
            Grid.SetColumn(checkBoxAutoPrePick, 1);
            Grid.SetRow(checkBoxAutoPrePick, 2);
            Grid.SetColumn(checkBoxAutoBan, 1);
            Grid.SetRow(checkBoxAutoBan, 3);
            Grid.SetColumn(checkBoxAutoAramBenchSwap, 1);
            Grid.SetColumnSpan(checkBoxAutoAramBenchSwap, 2);
            Grid.SetRow(checkBoxAutoAramBenchSwap, 4);
            Grid.SetColumn(checkBoxAutoAccept, 3);
            Grid.SetRow(checkBoxAutoAccept, 0);
            Grid.SetColumn(checkBoxAutoQPlayAgain, 3);
            Grid.SetRow(checkBoxAutoQPlayAgain, 1);
            Grid.SetColumn(checkBoxAutoHonor, 3);
            Grid.SetRow(checkBoxAutoHonor, 2);
            Grid.SetColumn(buttonDelays, 3);
            Grid.SetRow(buttonDelays, 3);
            Grid.SetColumn(checkBoxAutoPosition, 4);
            Grid.SetRow(checkBoxAutoPosition, 0);
            Grid.SetColumn(checkBoxAutoMessage, 4);
            Grid.SetRow(checkBoxAutoMessage, 1);
            Grid.SetColumn(checkBoxAutoReroll, 4);
            Grid.SetRow(checkBoxAutoReroll, 2);
            Grid.SetColumn(checkBoxAutoSkin, 4);
            Grid.SetRow(checkBoxAutoSkin, 3);
            Grid.SetColumn(comboBoxAutoPosition1, 5);
            Grid.SetRow(comboBoxAutoPosition1, 0);
            Grid.SetColumn(textBoxConversationMessage, 5);
            Grid.SetRow(textBoxConversationMessage, 1);
            Grid.SetColumn(comboBoxAutoPosition2, 6);
            Grid.SetRow(comboBoxAutoPosition2, 0);
            Grid.SetColumn(labelInfo, gridMain.ColumnDefinitions.Count - 1);
            Grid.SetRow(labelInfo, gridMain.RowDefinitions.Count - 1);
            //this.Controls.Add(verifyOwnedIcons);
            windowMain.Title = "LOL Client TOOL - game version: " + version;
            setupLCU();
            setupFormAsync();
            FormChampionsSetup();
            FormInformationSetup();
            FormDelaysSetup();
        }

        private void windowMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
