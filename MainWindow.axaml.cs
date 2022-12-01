using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Threading.Tasks;
using System.Timers;
using Point = System.Drawing.Point;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using Avalonia.Themes.Fluent;
using System.Threading;
using System.Net.Http;
using System.Net.Http.Headers;
using Avalonia.Threading;

namespace LOL_Client_TOOL
{

    public partial class MainWindow : Window
    {
        public static bool debug = false;//set true to display inputs for testing lcu requests
        public static bool preview = false;
        public static bool alreadyRunning = false;
        public static bool showFormLoginOnce = true;
        public static ConfigurationData configData = new ConfigurationData();

        public static Avalonia.Controls.WindowIcon icco = new WindowIcon(getBitmap("https://user-images.githubusercontent.com/21199858/166489461-f28fbae9-b620-474e-9fc6-7a58566e584b.png"));

        public static bool isClientApiReady = false;

        public static Dictionary<string, string> lesArguments = new Dictionary<string, string>();
        public static Dictionary<string, string> riotCredntials = new Dictionary<string, string>();
        public static Dictionary<string, string> shardsRunes = new Dictionary<string, string>();
        public static Dictionary<int, string> currentSummonerTestedIcon = new Dictionary<int, string>();
        public static Dictionary<int, System.Drawing.Point> posSubMainRune = new Dictionary<int, Point>();
        public static SortedDictionary<string, string> champs = new SortedDictionary<string, string>();
        public static List<string> summonerIcons = new List<string>();
        public static List<string> currentSummonerOwnedIcons = new List<string>();
        public static List<string> conversationsMessageSent = new List<string>();
        public static List<string> usernames = new List<string>();
        public static List<TextBlock> TextBlocksSummonerStatus = new List<TextBlock>();
        public static List<championPick> pickPriorities = new List<championPick>();
        public static string LCUport = "";
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
        public static int autoPositionOnce = 0;

        public static JArray runesReforged = new JArray();

        public static bool formSummonerIconLoadComplete = false;
        public static bool processingAllIcon = false;
        public static bool formSummonerRuneIsSetup = false;
        public static bool instalockIsEnabled = false;

        public HttpClient httpClient;

        public string Token { get; set; }
        public ushort RiotPort { get; set; }

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
            public bool LightOn { get; set; }
            public List<championPick> championPickList { get; set; }
            public int prePickDelay { get; set; }
            public int pickDelay { get; set; }
            public int banDelay { get; set; }
            public string leagueOfLegendsClientExe { get; set; }
            public string lang { get; set; }
            public List<leagueOfLegendsAccount> leagueOfLegendsAccounts { get; set; }
        }

        public class leagueOfLegendsAccount
        {
            public string username { get; set; }
            public string password { get; set; }
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
        public static Grid gridMain = new Grid();
        public static Grid FormSummonerIcon = new Grid();
        public static Grid FormSummonerRunes = new Grid();
        public static Grid FormChampSelect = new Grid();
        public static Window formChampions = new Window();
        public static Grid gridChampions = new Grid();
        public static Window formInformation = new Window();
        public static Grid gridInformations = new Grid();
        public static Window formDelays = new Window();
        public static Grid gridDelays = new Grid();
        public static Window formLogin = new Window();

        public static ComboBox comboBoxRunesPages = new ComboBox();
        public static ComboBox comboBoxSummonerStatus = new ComboBox();
        public static ComboBox comboBoxAutoPosition1 = new ComboBox();
        public static ComboBox comboBoxAutoPosition2 = new ComboBox();
        public static ComboBox comboBoxAutoRunesSources = new ComboBox();
        public static ComboBox comboBoxAutoSummonersSources = new ComboBox();
        public static ComboBox comboBoxLightOnOff = new ComboBox();
        public static ComboBox comboBoxSavedAccounts = new ComboBox();
        public static ComboBox comboBoxLanguage = new ComboBox();

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
        public static TextBox textBoxConversationMessage = new TextBox();
        public static TextBox textBoxUsername = new TextBox();
        public static TextBox textBoxPassword = new TextBox();

        public static Button apiEndpointCall = new Button();
        public static Button buttonRune = new Button();
        public static Button buttonSetupFrom = new Button();
        public static Button buttonChampions = new Button();
        public static Button buttonDelays = new Button();
        public static Button buttonLogin = new Button();
        public static Button buttonAccount = new Button();
        public static Button buttonRemoveAccount = new Button();
        public static Button buttonDisconnect = new Button();

        public static Avalonia.Controls.Image summonerIcon = new Avalonia.Controls.Image();
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
            //System.Timers.Timer aTimer = new System.Timers.Timer();
            //aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            //aTimer.Interval = 1000;
            //aTimer.Enabled = true;
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
                if (url.Contains("languages"))
                {
                    url = "https://ddragon.leagueoflegends.com/cdn/" + url;
                }
                else
                {
                    url = "https://ddragon.leagueoflegends.com/cdn/" + version + url;
                }
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
                var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager
                    .GetMessageBoxStandardWindow("DDragonRequest", e.Message);
                messageBoxStandardWindow.Show();
                return e.Message;
            }
        }

        public static KeyValuePair<string, string> LCURequest(string url = "", string method = "", string json = "")
        {
            method = method.ToUpper();
            url = "https://127.0.0.1:" + LCUport + url.Trim();
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

        public static Bitmap getIcon(string iconId)
        {
            string url = "http://ddragon.leagueoflegends.com/cdn/" + version + "/img/profileicon/" + iconId + ".png";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            System.Drawing.Bitmap bitmap2 = new System.Drawing.Bitmap(responseStream);
            //If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool DeleteObject([In] IntPtr hObject);

            System.Drawing.Bitmap bitmapTmp = new System.Drawing.Bitmap(bitmap2);
            var bitmapdata = bitmapTmp.LockBits(new Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Bitmap bitmap1 = new Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
                bitmapdata.Scan0,
                new Avalonia.PixelSize(bitmapdata.Width, bitmapdata.Height),
                new Avalonia.Vector(96, 96),
                bitmapdata.Stride);
            bitmapTmp.UnlockBits(bitmapdata);
            bitmapTmp.Dispose();
            return bitmap1;
        }

        public static Bitmap getBitmap(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            System.Drawing.Bitmap bitmap2 = new System.Drawing.Bitmap(responseStream);
            //If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool DeleteObject([In] IntPtr hObject);

            System.Drawing.Bitmap bitmapTmp = new System.Drawing.Bitmap(bitmap2);
            var bitmapdata = bitmapTmp.LockBits(new Rectangle(0, 0, bitmapTmp.Width, bitmapTmp.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Bitmap bitmap1 = new Bitmap(Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul,
                bitmapdata.Scan0,
                new Avalonia.PixelSize(bitmapdata.Width, bitmapdata.Height),
                new Avalonia.Vector(96, 96),
                bitmapdata.Stride);
            bitmapTmp.UnlockBits(bitmapdata);
            bitmapTmp.Dispose();
            return bitmap1;
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
        public static async Task getAllChampions()
        {
            string tempJsonChamp = DDragonRequest("/data/" + lang + "/champion.json");
            JObject championsJson = JObject.Parse(tempJsonChamp);

            leagueOfLegendsChampions.Clear();
            foreach (var champData in championsJson["data"].Values())
            {
                string lestring = champData.ToString();
                champion lechamp = JsonConvert.DeserializeObject<champion>(lestring);
                leagueOfLegendsChampions.Add(lechamp);
            }
            leagueOfLegendsChampions = leagueOfLegendsChampions.OrderBy(o => o.name).ToList();
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
        public async void HearthBeatEvent(Object source, ElapsedEventArgs e)
        {
            var status = LCURequest("/lol-service-status/v1/lcu-status", "GET");
            if(status.Key != "OK")
            {
                bool clientStarted = false;
                isClientApiReady = false;
                var processes = Process.GetProcesses();
                foreach(var proc in processes)
                {
                    if (proc.ProcessName.ToLower().Contains("leagueclient"))
                    {
                        setupLCU();
                        clientStarted = true;
                    }
                }
                if (!clientStarted && showFormLoginOnce)
                {
                    Dispatcher.UIThread.InvokeAsync(formLogin_Show);
                }
            }
            else
            {
                isClientApiReady = true;
                Dispatcher.UIThread.InvokeAsync(setupFormAsync);
            }
        }
        public void setSummonerName()
        {
            labelSummonerDisplayName.Content = "caca";
        }
        public void formLogin_Show()
        {
            formLogin.Show();
        }
        private static async Task OnTimedEvent()
        {
            var currentState = LCURequest("/lol-gameflow/v1/session", "GET");
            int aaaaaaaled = 1000;
            while (currentState.Key == "" || currentState.Key == null)
            {
                currentState = LCURequest("/lol-gameflow/v1/session", "GET");
                Thread.Sleep(aaaaaaaled);
                aaaaaaaled += 1000;
            }
            if (currentState.Key.Contains("OK") && alreadyRunning != true)
            {
                alreadyRunning = true;
                JObject gameFlow = JObject.Parse(currentState.Value);
                //Keep json for the loop START
                var lolChampSelectV1Session = new KeyValuePair<string, string>();
                int timer = 0;
                int delayMax = 0;
                if (configData.autoReroll || configData.autoPick || configData.autoBan || configData.autoMessage || configData.autoSkin || configData.autoReroll)
                {
                    if(gameFlow["phase"].ToString().ToLower() == "ChampSelect".ToLower())
                    {
                        lolChampSelectV1Session = LCURequest("/lol-champ-select/v1/session", "GET");
                        if (lolChampSelectV1Session.Key == "OK")
                        {
                            JObject json = JObject.Parse(lolChampSelectV1Session.Value);
                            timer = Convert.ToInt32(json["timer"]["adjustedTimeLeftInPhase"]);
                            delayMax = Convert.ToInt32(json["timer"]["totalTimeInPhase"]);
                        }
                    }
                }
                //Keep json for the loop END
                //auto accept start
                if (configData.autoAccept && gameFlow["phase"].ToString().ToLower() == "readycheck")
                {
                    LCURequest("/lol-matchmaking/v1/ready-check/accept", "POST", "");
                }
                //auto accept end

                //Honor start
                if (configData.autoHonor && gameFlow["phase"].ToString().ToLower() == "PreEndOfGame".ToLower())
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
                if ((configData.autoPick || configData.autoBan || configData.autoPrePick) && currentSummoner.summonerId != null && gameFlow["phase"].ToString().ToLower() == "ChampSelect".ToLower())
                {
                    List<int> championPlayable = new List<int>();
                    if (lolChampSelectV1Session.Key == "OK")
                    {
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
                                    KeyValuePair<string, string> jsonChampionsMinimal = LCURequest("/lol-champions/v1/owned-champions-minimal", "GET", "");
                                    if (jsonChampionsMinimal.Key == "OK")
                                    {
                                        var ownedChampionsMinimal = JArray.Parse(jsonChampionsMinimal.Value);
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
                                    }
                                    string isInProgress = team["isInProgress"].ToString();
                                    //PICK
                                    if (isInProgress == "True" && team["type"].ToString().Contains("pick") && configData.autoPick)
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
                                    if (configData.autoPrePick)
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
                                        //string jsonDataForLock = "{  \"actorCellId\": " + actorCellId + ",  \"championId\": " + champId + ",  \"completed\": true,  \"id\": " + team["id"].ToString() + ",  \"isAllyAction\": true,  \"type\": \"string\"}";
                                        string jsonDataForLock = "{  \"championId\": " + champId + "}";
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
                        string benchEnabled = json["benchEnabled"].ToString().ToLower();
                        //ARAM swap bench
                        if (configData.autoAramBenchSwap && benchEnabled != "false")
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
                        //List<string> runes = new List<string>();

                        //if (configData.autoRunes && selectedChampionId != 0 && selectedChampionId.ToString() != lastRunePageBuilt)
                        //{
                        //    string html = "";
                        //    if (assignedPosition != "")
                        //    {
                        //        html = "https://u.gg/lol/champions/" + currentChampionName.ToLower() + "/build?rank=diamond_plus&role=" + assignedPosition;
                        //    }
                        //    else
                        //    {
                        //        html = "https://u.gg/lol/champions/" + currentChampionName.ToLower() + "/build?rank=diamond_plus";
                        //    }

                        //    lastRunePageBuilt = selectedChampionId.ToString();
                        //    var agg = LCURequest("/lol-perks/v1/currentpage", "GET");
                        //    var currentRunePage = new JObject();
                        //    string currentPageId = "";
                        //    string currentPageEditable = "";
                        //    if (!agg.Value.Contains("404"))
                        //    {
                        //        currentRunePage = JObject.Parse(agg.Value);
                        //        currentPageId = currentRunePage["id"].ToString();
                        //        currentPageEditable = currentRunePage["isEditable"].ToString().ToLower();
                        //    }
                        //    else
                        //    {
                        //        Random rnd = new Random();
                        //        currentPageId = rnd.Next(10000, 100000).ToString();
                        //        currentPageEditable = "true";
                        //    }

                        //    foreach (champion champ in leagueOfLegendsChampions)
                        //    {
                        //        if (champ.key == selectedChampionId.ToString() && currentPageEditable == "true")
                        //        {
                        //            string championName = champ.name;
                        //            if (runeSource == "U.GG")
                        //            {
                        //                if (championName.ToLower().Contains("nunu"))
                        //                {
                        //                    championName = "nunu";
                        //                }


                        //                //var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");
                        //                string ClassToGet1 = "perk perk-active";
                        //                string ClassToGet2 = "perk keystone perk-active";
                        //                string ClassToGet3 = "shard shard-active";
                        //                string splitRunes = "\\\"";
                        //                string selector = "//div[contains(@class, '" + ClassToGet2 + "')]";
                        //                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                        //                HtmlWeb web = new HtmlWeb();
                        //                htmlDoc = web.Load(html);
                        //                htmlDoc.LoadHtml(htmlDoc.Text);
                        //                var htmlNodes = htmlDoc.DocumentNode.SelectNodes(selector);
                        //                foreach (HtmlNode node in htmlNodes)
                        //                {
                        //                    //string frormed = node.InnerHtml.Split("alt");
                        //                    if (node.InnerHtml.Contains(ClassToGet2))
                        //                    {
                        //                        if (!runes.Contains(Regex.Split(node.InnerHtml, splitRunes)[Regex.Split(node.InnerHtml, splitRunes).Count() - 2]))
                        //                        {
                        //                            runes.Add(Regex.Split(node.InnerHtml, splitRunes)[Regex.Split(node.InnerHtml, splitRunes).Count() - 2]);
                        //                        }
                        //                    }
                        //                }
                        //                foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[@class='" + ClassToGet1 + "']"))
                        //                {
                        //                    if (!runes.Contains(Regex.Split(node.InnerHtml, splitRunes)[Regex.Split(node.InnerHtml, splitRunes).Count() - 2]))
                        //                    {

                        //                        runes.Add(Regex.Split(node.InnerHtml, splitRunes)[Regex.Split(node.InnerHtml, splitRunes).Count() - 2]);
                        //                    }
                        //                }
                        //                foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[@class='" + ClassToGet3 + "']"))
                        //                {
                        //                    runes.Add(Regex.Split(node.InnerHtml, splitRunes)[Regex.Split(node.InnerHtml, splitRunes).Count() - 2]);
                        //                }
                        //                runes.RemoveAt(runes.Count() - 1);
                        //                runes.RemoveAt(runes.Count() - 1);
                        //                runes.RemoveAt(runes.Count() - 1);
                        //                //Console.WriteLine("Node Name: " + node.Name + "\n" + node.OuterHtml);
                        //                string name = "";
                        //                List<Tuple<string, string>> runePage = new List<Tuple<string, string>>();
                        //                foreach (string perk in runes)
                        //                {
                        //                    foreach (var style in runesReforged)
                        //                    {
                        //                        foreach (var slot in style["slots"])
                        //                        {
                        //                            foreach (var perkPage in slot["runes"])
                        //                            {
                        //                                name = perkPage["name"].ToString();
                        //                                if (perk.Contains(name))
                        //                                {
                        //                                    runePage.Add(new Tuple<string, string>(perkPage["id"].ToString(), perkPage["name"].ToString()));
                        //                                }
                        //                            }
                        //                        }
                        //                    }
                        //                    foreach (var shard in shardsRunes)
                        //                    {
                        //                        if (perk.Contains(shard.Value))
                        //                        {
                        //                            runePage.Add(new Tuple<string, string>(shard.Key, shard.Value));
                        //                        }
                        //                    }
                        //                }
                        //                var resp = LCURequest("/lol-perks/v1/styles", "GET");
                        //                var styles = JArray.Parse(resp.Value);
                        //                //set the runes
                        //                //string boddy = "{\"autoModifiedSelections\":[],\"current\":true,\"id\":" + currentPageId + ",\"isActive\":true,\"isDeletable\":true,\"isEditable\":true,\"isValid\":true,\"lastModified\":" + currentRunePage["lastModified"] + ",\"name\":\"" + currentRunePage["name"] + "\",\"order\":0,\"primaryStyleId\":8000,\"selectedPerkIds\":[" + runePage.ToList()[0].Key + "," + runePage.ToList()[1].Key + "," + runePage.ToList()[2].Key + "," + runePage.ToList()[3].Key + "," + runePage.ToList()[4].Key + "," + runePage.ToList()[5].Key + "," + runePage.ToList()[6].Key + "," + runePage.ToList()[7].Key + "," + runePage.ToList()[8].Key + "],\"subStyleId\":8400}";
                        //                string style1 = runePage[1].Item1.Substring(0, 2) + "00";
                        //                string style2 = runePage[4].Item1.Substring(0, 2) + "00";
                        //                if (style2.Substring(0, 1).Contains("9"))
                        //                {
                        //                    style2 = "8000";
                        //                }
                        //                string lastModified = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        //                string boddy = "{\"current\":true,\"id\":" + currentPageId + ",\"isActive\":true,\"isDeletable\":true,\"isEditable\":true,\"isValid\":true,\"lastModified\":" + lastModified + ",\"name\":\"" + "LOL Client TOOL" + "\",\"order\":0,\"primaryStyleId\":" + style1 + ",\"selectedPerkIds\":[" + runePage[0].Item1 + "," + runePage[1].Item1 + "," + runePage[2].Item1 + "," + runePage[3].Item1 + "," + runePage.ToList()[4].Item1 + "," + runePage.ToList()[5].Item1 + "," + runePage[6].Item1 + "," + runePage[7].Item1 + "," + runePage[8].Item1 + "],\"subStyleId\":" + style2 + "}";
                        //                //string boddy = "{\"current\":true,\"id\":" + currentPageId + ",\"isActive\":true,\"isDeletable\":true,\"isEditable\":true,\"isValid\":true,\"lastModified\":" + currentRunePage["lastModified"] + ",\"name\":\"" + currentRunePage["name"] + "\",\"order\":0,\"selectedPerkIds\":[" + runePage[0].Item1 + "," + runePage[1].Item1 + "," + runePage[2].Item1 + "," + runePage[3].Item1 + "," + runePage.ToList()[4].Item1 + "," + runePage.ToList()[5].Item1 + "," + runePage[6].Item1 + "," + runePage[7].Item1 + "," + runePage[8].Item1 + "]}";
                        //                //LCURequest("/lol-perks/v1/pages", "POST", boddy);
                        //                LCURequest("/lol-perks/v1/pages/" + currentPageId, "DELETE");

                        //                LCURequest("/lol-perks/v1/pages", "POST", boddy);
                        //            }
                        //        }
                        //    }

                        //}
                        ////auto summoners
                        //if (configData.autoSummoners && selectedChampionId != 0)
                        //{
                        //    string html = "https://u.gg/lol/champions/" + currentChampionName.ToLower() + "/build?rank=diamond_plus&role=" + assignedPosition;
                        //    lastSummonerBuilt = selectedChampionId.ToString();
                        //    HtmlWeb web = new HtmlWeb();
                        //    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                        //    htmlDoc.LoadHtml(html);
                        //    string ClassToGet1 = "content-section_content summoner-spells";
                        //    //foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[@class='" + ClassToGet2 + "']"))
                        //    foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[@class='" + ClassToGet1 + "']").Nodes())
                        //    {
                        //        //string frormed = node.InnerHtml.Split("alt");
                        //        string data = node.InnerHtml;
                        //    }
                        //}
                    }
                }
                //auto lock end

                //reQ/play again start
                if (configData.autoPlayAgain)
                {
                    if (gameFlow["phase"].ToString().ToLower() == "Lobby".ToLower())
                    {
                        LCURequest("/lol-lobby/v2/lobby/matchmaking/search", "POST");
                    }
                    if (gameFlow["phase"].ToString().ToLower() == "EndOfGame".ToLower())
                    {
                        LCURequest("/lol-player-level-up/v1/level-up-notifications", "POST");
                        LCURequest("/lol-lobby/v2/play-again", "POST");
                    }
                }
                //reQ/play again end

                //AUTO POSITION start
                if (configData.autoRole && gameFlow["phase"].ToString().ToLower() == "lobby")
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
                if (configData.autoMessage && lolChampSelectV1Session.Key == "OK" && gameFlow["phase"].ToString().ToLower() == "ChampSelect".ToLower())
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
                if (configData.autoReroll && lolChampSelectV1Session.Key == "OK" && gameFlow["gameData"]["queue"]["gameTypeConfig"]["reroll"].ToString().ToLower() == "true")
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
                if (configData.autoSkin && lolChampSelectV1Session.Key == "OK" && gameFlow["phase"].ToString().ToLower() == "ChampSelect".ToLower())
                {
                    JObject json = JObject.Parse(lolChampSelectV1Session.Value);
                    foreach (var player in json["myTeam"])
                    {
                        if (player["summonerId"].ToString() == currentSummoner.summonerId && player["selectedSkinId"].ToString() != "0" && selectedChampionId != 0)
                        {
                            List<string> ownedSkins = new List<string>();
                            KeyValuePair<string, string> skinCarousel = LCURequest("/lol-champ-select/v1/skin-carousel-skins", "GET");
                            if (skinCarousel.Key == "OK")
                            {
                                var skins = JArray.Parse(skinCarousel.Value);
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
                alreadyRunning = false;
            }
        }

        public static void setupLCU()//for api usage
        {
            //WMIC stopped working as of 30/11/2022
            //try
            //{
            //    System.Diagnostics.ProcessStartInfo usbDevicesInfo = new System.Diagnostics.ProcessStartInfo("wmic", "PROCESS WHERE name='LeagueClientUx.exe' GET commandline");
            //    usbDevicesInfo.RedirectStandardOutput = true;
            //    usbDevicesInfo.UseShellExecute = false;
            //    usbDevicesInfo.CreateNoWindow = true;
            //    System.Diagnostics.Process process = new System.Diagnostics.Process();
            //    process.StartInfo = usbDevicesInfo;
            //    process.Start();
            //    process.WaitForExit();
            //    Console.WriteLine("ExitCode: " + process.ExitCode.ToString() + "\n");
            //    string result = process.StandardOutput.ReadToEnd();
            //    string[] lesArgumentsTemp = result.Split(new[] { "\" \"" }, StringSplitOptions.None);
            //    foreach (string argument in lesArgumentsTemp)
            //    {
            //        string arg = argument.Replace("\"", "");
            //        if (arg.Contains("="))
            //        {
            //            string[] kv = arg.Split(Convert.ToChar("="));
            //            lesArguments.Add(kv[0], kv[1]);
            //        }
            //    }
            //    byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes("riot:" + lesArguments["--remoting-auth-token"]);
            //    autorization = Convert.ToBase64String(data);
            //    //string laData = "PASSWORD: " + lesArguments["--remoting-auth-token"] + "\r\n" + "PORT: " + lesArguments["--app-port"] + "\r\n" + "AUTH: " + autorization;
            //    LCUport = lesArguments["--app-port"];
            //    lesArguments.Clear();
            //    //throw new Exception();//testing
            //}
            //catch (Exception ex)
            //{
            //    try
            //    {
            //        //trying with the lock file
            //        var process = Process.GetProcesses();
            //        string processesPath = "";
            //        foreach (var name in process)
            //        {
            //            if (name.ProcessName.ToLower().Contains("LeagueClient".ToLower()))
            //            {
            //                processesPath = name.MainModule.FileName;
            //            }
            //        }
            //        string[] pathes = processesPath.Split(new[] { @"\" }, StringSplitOptions.None);
            //        string newPath = "";
            //        foreach (string str in pathes)
            //        {
            //            if (str.Contains("exe"))
            //            {
            //                newPath += "lockfile";
            //            }
            //            else
            //            {
            //                newPath += str + @"\";
            //            }
            //        }
            //        string readText = "";
            //        using (FileStream stream = File.Open(newPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //        {
            //            using (StreamReader reader = new StreamReader(stream))
            //            {
            //                readText = reader.ReadToEnd();
            //            }
            //        }
            //        LCUport = readText.Split(new[] { @":" }, StringSplitOptions.None)[2];
            //        string temp = readText.Split(new[] { @":" }, StringSplitOptions.None)[3];

            //        autorization = Convert.ToBase64String(Encoding.ASCII.GetBytes("riot:" + temp));
            //    }
            //    catch (Exception ex2)
            //    {
            //    }
            //}
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
                LCUport = readText.Split(new[] { @":" }, StringSplitOptions.None)[2];
                string temp = readText.Split(new[] { @":" }, StringSplitOptions.None)[3];

                autorization = Convert.ToBase64String(Encoding.ASCII.GetBytes("riot:" + temp));
            }
            catch (Exception ex2)
            {
            }
        }


        private string GetRiotCommandLineArgs()
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c wmic PROCESS WHERE name='RiotClientUx.exe' GET commandline";
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            //p.WaitForExit();

            return output;
        }
        private string[] GetPortAndAuthKey()
        {
            string[] retvalues = new string[3];

            string wholeCommandLine = GetRiotCommandLineArgs();

            string[] splitSpaces = wholeCommandLine.Split(' ');

            foreach (string s in splitSpaces)
            {
                if (s.Contains("="))
                {

                    string key = s.Split('=')[0];
                    string value = s.Split('=')[1];

                    if (key == "--app-port")
                    {
                        retvalues[0] = value;
                    }
                    else if (key == "--remoting-auth-token")
                    {
                        string Token = value;
                        retvalues[1] = Convert.ToBase64String(Encoding.ASCII.GetBytes($"riot:{Token}"));
                    }
                    else if (key == "--app-pid")
                    {
                        retvalues[2] = value;
                    }
                }
            }

            return retvalues;
        }
        public bool RiotClientConnect()
        {
            string[] items = GetPortAndAuthKey();

            foreach (string s in items)
            {
                if (s == null || s == "")
                {
                    return false;
                }
            }

            Token = items[1];
            RiotPort = ushort.Parse(items[0]);
            string ApiUri = "https://127.0.0.1:" + RiotPort.ToString() + "/";
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Token);
            httpClient.BaseAddress = new Uri(ApiUri);

            return true;
        }
        public string MakeHttpRequest(string method, string endpoint, string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            if (method == "put")
            {
                var result = httpClient.PutAsync(endpoint, content);
                string resultContent = result.Result.Content.ReadAsStream().ToString();

                return resultContent;
            }

            return "";
        }
        public static void loadConfig()
        {
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
                if (configData.leagueOfLegendsAccounts != null)
                {
                    //JObject.Parse duplicate the picklist somehow lets fix it
                    int i = 0;
                    List<string> uniqueUsername = new List<string>();
                    List<leagueOfLegendsAccount> accToSave = new List<leagueOfLegendsAccount>();
                    foreach (leagueOfLegendsAccount acc in configData.leagueOfLegendsAccounts)
                    {
                        if (!uniqueUsername.Contains(acc.username))
                        {
                            uniqueUsername.Add(acc.username);
                            accToSave.Add(acc);
                        }
                    }
                    configData.leagueOfLegendsAccounts = accToSave;


                    foreach (leagueOfLegendsAccount acc in configData.leagueOfLegendsAccounts)
                    {
                        usernames.Add(acc.username);
                    }
                    comboBoxSavedAccounts.Items = usernames;
                }
                else
                {
                    configData.leagueOfLegendsAccounts = new List<leagueOfLegendsAccount>();
                }
                if (configData.leagueOfLegendsClientExe == null || configData.leagueOfLegendsClientExe == "")
                {
                    var installDir = LCURequest("/data-store/v1/install-dir", "GET");
                    if (installDir.Key == "OK")
                    {
                        configData.leagueOfLegendsClientExe = installDir.Value.ToString() + @"\LeagueClient.exe";
                    }
                }

                if (!configData.LightOn)
                {
                    Application.Current.Styles.Clear();
                    var i = new FluentTheme(new Uri(@"avares://Avalonia.Themes.Fluent/FluentTheme.xaml")) { Mode = FluentThemeMode.Dark };
                    Application.Current.Styles.Add(i);
                }
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
            if(configData.lang is null)
            {
                lang = regionAndLanguage["locale"].ToString();
                configData.lang = regionAndLanguage["locale"].ToString();
            }
            else
            {
                lang = configData.lang;
            }
            runesReforged = JArray.Parse(DDragonRequest("/data/" + "en_US" + "/runesReforged.json"));

            //start getting all champions
            getAllChampions();
            //end getting all champions

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
            labelSummonerDisplayName.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            labelSummonerDisplayName.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom;
            labelSummonerDisplayName.Width = Double.NaN;
            labelSummonerDisplayName.Height = Double.NaN;
            //labelSummonerDisplayName.ToolTip = "Copy in clipboard";
            labelSummonerDisplayName.AddHandler(Label.PointerPressedEvent, LabelSummonerDisplayName_MouseLeftButtonUpAsync);

            Border myBorder1 = new Border();
            myBorder1.CornerRadius = new CornerRadius(15);
            buttonChampions.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            buttonChampions.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            buttonChampions.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            buttonChampions.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            buttonChampions.Content = "Champions";
            buttonChampions.AddHandler(Button.ClickEvent, ButtonChampions_Click);

            formChampions.SizeToContent = SizeToContent.WidthAndHeight;

            isDebug();
            gridChampions.ShowGridLines = debug;
            while (gridChampions.ColumnDefinitions.Count < 4)
            {
                if (gridChampions.ColumnDefinitions.Count == 0)
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

            comboBoxSummonerStatus.Items = new String[] { "ONLINE", "AWAY", "MOBILE", "OFFLINE" };
            comboBoxSummonerStatus.SelectedIndex = 0;
            comboBoxSummonerStatus.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            comboBoxSummonerStatus.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            comboBoxSummonerStatus.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            comboBoxSummonerStatus.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            comboBoxSummonerStatus.SelectionChanged += ComboBoxSummonerStatus_SelectionChanged;

            //buttonRune.Text = "RUNES";
            //buttonRune.Location = new Point(labelSummonerDisplayName.Location.X + labelSummonerDisplayName.Width, labelSummonerDisplayName.Location.Y);

            checkBoxAutoPick.Content = "Auto pick";
            checkBoxAutoPick.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            checkBoxAutoPick.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            checkBoxAutoPick.AddHandler(CheckBox.ClickEvent, CheckBoxAutoPick_Click);

            checkBoxAutoPrePick.Content = "Auto pre-pick";
            checkBoxAutoPrePick.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            checkBoxAutoPrePick.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            checkBoxAutoPrePick.AddHandler(CheckBox.ClickEvent, CheckBoxAutoPrePick_Click);

            checkBoxAutoBan.Content = "Auto ban";
            checkBoxAutoBan.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            checkBoxAutoBan.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            checkBoxAutoBan.AddHandler(CheckBox.ClickEvent, CheckBoxAutoBan_Click);

            checkBoxAutoAramBenchSwap.Content = "Auto ARAM bench swap";
            checkBoxAutoAramBenchSwap.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            checkBoxAutoAramBenchSwap.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            checkBoxAutoAramBenchSwap.AddHandler(CheckBox.ClickEvent, CheckBoxAutoAramBenchSwap_Click);

            checkBoxAutoAccept.Content = "Auto accept";
            checkBoxAutoAccept.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            checkBoxAutoAccept.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            checkBoxAutoAccept.AddHandler(CheckBox.ClickEvent, CheckBoxAutoAccept_Click);

            checkBoxAutoQPlayAgain.Content = "Re Q/Play";
            checkBoxAutoQPlayAgain.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            checkBoxAutoQPlayAgain.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            checkBoxAutoQPlayAgain.AddHandler(CheckBox.ClickEvent, CheckBoxAutoQPlayAgain_Click);

            checkBoxAutoHonor.Content = "Auto honor";
            checkBoxAutoHonor.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            checkBoxAutoHonor.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            checkBoxAutoHonor.AddHandler(CheckBox.ClickEvent, CheckBoxAutoHonor_Click);

            buttonDelays.Content = "Delays";
            buttonDelays.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            buttonDelays.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            buttonDelays.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            buttonDelays.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            buttonDelays.AddHandler(Button.ClickEvent, ButtonDelays_Click);

            buttonAccount.Content = "Account";
            buttonAccount.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            buttonAccount.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            buttonAccount.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            buttonAccount.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            buttonAccount.AddHandler(Button.ClickEvent, buttonAccount_Click);

            checkBoxAutoPosition.Content = "Auto role";
            checkBoxAutoPosition.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            checkBoxAutoPosition.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            checkBoxAutoPosition.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            checkBoxAutoPosition.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            checkBoxAutoPosition.AddHandler(CheckBox.ClickEvent, CheckBoxAutoPosition_Click);

            comboBoxAutoPosition2.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            comboBoxAutoPosition2.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            comboBoxAutoPosition2.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            comboBoxAutoPosition2.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;

            comboBoxAutoPosition1.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            comboBoxAutoPosition1.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            comboBoxAutoPosition1.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            comboBoxAutoPosition1.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;

            comboBoxAutoPosition1.Items = postions;
            comboBoxAutoPosition2.Items = postions;
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
            //comboBoxAutoPosition1.SelectionChanged += ComboBoxAutoPosition1_SelectionChanged;
            //comboBoxAutoPosition2.SelectionChanged += ComboBoxAutoPosition2_SelectionChanged;
            comboBoxAutoPosition1.AddHandler(ComboBox.SelectionChangedEvent, ComboBoxAutoPosition1_SelectionChanged);
            comboBoxAutoPosition2.AddHandler(ComboBox.SelectionChangedEvent, ComboBoxAutoPosition2_SelectionChanged);
            checkBoxAutoReroll.Content = "Auto reroll";
            checkBoxAutoReroll.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            checkBoxAutoReroll.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            checkBoxAutoReroll.AddHandler(CheckBox.ClickEvent, CheckBoxAutoReroll_Click);

            checkBoxAutoSkin.Content = "Auto skin";
            checkBoxAutoSkin.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            checkBoxAutoSkin.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            checkBoxAutoSkin.AddHandler(CheckBox.ClickEvent, CheckBoxAutoSkin_Click);

            checkBoxAutoMessage.Content = "Auto message";
            checkBoxAutoMessage.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            checkBoxAutoMessage.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            checkBoxAutoMessage.AddHandler(CheckBox.ClickEvent, CheckBoxAutoMessage_Click1);

            textBoxConversationMessage.Watermark = "Your message";
            textBoxConversationMessage.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            textBoxConversationMessage.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            textBoxConversationMessage.AddHandler(TextBox.KeyUpEvent, TextBoxConversationMessage_TextChanged);

            comboBoxLightOnOff.Items = new String[] { "Light", "Dark" };
            comboBoxLightOnOff.SelectedIndex = 0;
            comboBoxLightOnOff.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            comboBoxLightOnOff.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            comboBoxLightOnOff.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            comboBoxLightOnOff.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            if (!configData.LightOn)
            {
                comboBoxLightOnOff.SelectedIndex = 1;
            }
            comboBoxLightOnOff.SelectionChanged += ComboBoxLifghtOnOff_SelectionChanged;


            string languages = DDragonRequest("languages.json");
            var json = JArray.Parse(languages);
            List<string> list = new List<string>();
            foreach(string lang in json)
            {
                list.Add(lang);
            }
            list.Sort();
            comboBoxLanguage.Items = list;
            if (configData.lang != null)
            {
                comboBoxLanguage.SelectedItem = configData.lang;
            }
            else
            {
                comboBoxLanguage.SelectedItem = lang;
            }
            comboBoxLanguage.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            comboBoxLanguage.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            comboBoxLanguage.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            comboBoxLanguage.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            comboBoxLanguage.AddHandler(ComboBox.SelectionChangedEvent, ComboBoxLanguageChanged);

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

            labelInfo.Content = "INFORMATION";
            labelInfo.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            labelInfo.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            labelInfo.Foreground = Avalonia.Media.Brushes.Blue;
            labelInfo.AddHandler(Label.PointerPressedEvent, LabelInfo_MouseUp);

            summonerIcon.Source = getIcon(currentSummoner.profileIconId);
            summonerIcon.MaxHeight = 128;
            summonerIcon.MaxHeight = 128;
        }

        private static void buttonAccount_Click(object sender, RoutedEventArgs e)
        {
            formLogin.Show();
        }
        private static void LabelInfo_MouseUp(object sender, RoutedEventArgs e)
        {
            formInformation.Show();
        }

        private static void TextBoxConversationMessage_TextChanged(object sender, RoutedEventArgs e)
        {
            configData.autoMessageText = textBoxConversationMessage.Text;
            configSaveAsync();
        }

        private static void CheckBoxAutoMessage_Click1(object sender, RoutedEventArgs e)
        {
            configData.autoMessage = (bool)checkBoxAutoMessage.IsChecked;
            configSaveAsync();
        }

        private static void ComboBoxAutoPosition2_SelectionChanged(object sender, RoutedEventArgs e)
        {
            autoPositionOnce = 0;
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

        private static void ComboBoxAutoPosition1_SelectionChanged(object sender, RoutedEventArgs e)
        {
            autoPositionOnce = 0;
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
            ComboBox combobox = (ComboBox)sender;
            if (combobox.SelectedItem != null)
            {
                LCURequest("/lol-chat/v1/me", "PUT", "{\"availability\": \"" + combobox.SelectedItem.ToString().ToLower() + "\"}");
            }
        }

        private static void ComboBoxLifghtOnOff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;
            if (combobox.SelectedItem != null)
            {
                if (combobox.SelectedItem.ToString().ToLower() == "Light".ToLower())
                {
                    Application.Current.Styles.Clear();
                    var i = new FluentTheme(new Uri(@"avares://Avalonia.Themes.Fluent/FluentTheme.xaml")) { Mode = FluentThemeMode.Light };
                    Application.Current.Styles.Add(i);
                    configData.LightOn = true;
                }
                else if (combobox.SelectedItem.ToString().ToLower() == "dark".ToLower())
                {
                    Application.Current.Styles.Clear();
                    var i = new FluentTheme(new Uri(@"avares://Avalonia.Themes.Fluent/FluentTheme.xaml")) { Mode = FluentThemeMode.Dark };
                    Application.Current.Styles.Add(i);
                    configData.LightOn = false;
                }
                configSaveAsync();
            }
        }        
        
        private static void ComboBoxLanguageChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;
            if (combobox.SelectedItem != null)
            {
                lang = combobox.SelectedItem.ToString();
                configData.lang = lang;
                getAllChampions();
                configSaveAsync();
            }
        }

        public static void FormChampionsSetup()
        {
            formChampions.Title = "Champions Pick Ban and ARAM priorities";
            //formChampions.Visible = false;
            Font LargeFont = new Font(new FontFamily("Arial"), 12, System.Drawing.FontStyle.Bold);
            Font SmaleFont = new Font(new FontFamily("Arial"), 10, System.Drawing.FontStyle.Bold);

            ScrollViewer myScrollViewer = new ScrollViewer();
            myScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            formChampions.Content = myScrollViewer;
            myScrollViewer.Content = gridChampions;



            TextBox textBoxFilterChampions = new TextBox();
            Button buttonResetPicks = new Button();
            Button buttonResetBans = new Button();
            Button buttonResetAram = new Button();

            textBoxFilterChampions.Watermark = "Search champions";
            textBoxFilterChampions.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            textBoxFilterChampions.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            textBoxFilterChampions.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            textBoxFilterChampions.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            textBoxFilterChampions.AddHandler(TextBox.KeyUpEvent, TextBoxFilterChampions_TextChanged);

            while (gridChampions.RowDefinitions.Count < 2)
            {
                RowDefinition myRow = new RowDefinition();
                //myRow.MaxHeight = 25;
                //myRow.MinHeight = 25;
                gridChampions.RowDefinitions.Add(myRow);
            }




            buttonResetPicks.Content = "RESET PICKS";
            buttonResetPicks.FontFamily = new Avalonia.Media.FontFamily("Arial");
            buttonResetPicks.FontSize = SmaleFont.Size;
            buttonResetPicks.FontWeight = Avalonia.Media.FontWeight.Bold;
            buttonResetPicks.Click += ButtonResetPicks_Click;
            buttonResetPicks.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            buttonResetPicks.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            buttonResetPicks.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            buttonResetPicks.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            buttonResetBans.Content = "RESET BANS";
            buttonResetBans.FontFamily = new Avalonia.Media.FontFamily("Arial");
            buttonResetBans.FontSize = SmaleFont.Size;
            buttonResetBans.FontWeight = Avalonia.Media.FontWeight.Bold;
            buttonResetBans.Click += ButtonResetBans_Click;
            buttonResetBans.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            buttonResetBans.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            buttonResetBans.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            buttonResetBans.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            buttonResetAram.Content = "RESET ARAM";
            buttonResetAram.FontFamily = new Avalonia.Media.FontFamily("Arial");
            buttonResetAram.FontSize = SmaleFont.Size;
            buttonResetAram.FontWeight = Avalonia.Media.FontWeight.Bold;
            buttonResetAram.Click += ButtonResetAram_Click;
            buttonResetAram.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            buttonResetAram.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            buttonResetAram.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            buttonResetAram.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;

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
            List<NumericUpDown> pick = new List<NumericUpDown>();
            List<NumericUpDown> ban = new List<NumericUpDown>();
            List<NumericUpDown> aram = new List<NumericUpDown>();
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
                colNames[index].FontWeight = Avalonia.Media.FontWeight.Bold;
                colNames[index].FontFamily = new Avalonia.Media.FontFamily("Arial");
                colNames[index].FontSize = 12;
                colNames[index].VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                colNames[index].HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                colNames[index].VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
                colNames[index].HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;

                gridChampions.Children.Add(colNames[index]);
                Grid.SetRow(colNames[index], row);
                Grid.SetColumn(colNames[index], column);
                column++;
                index++;
            }

            row = 2;
            column = 0;
            index = 0;
            foreach (champion champ in leagueOfLegendsChampions)
            {
                RowDefinition myRow = new RowDefinition();
                //myRow.MaxHeight = 25;
                //myRow.MinHeight = 25;
                gridChampions.RowDefinitions.Add(myRow);

                Label temp = new Label();
                temp.Name = champ.key;
                temp.Content = champ.name;
                temp.FontWeight = Avalonia.Media.FontWeight.Bold;
                temp.FontFamily = new Avalonia.Media.FontFamily("Arial");
                temp.FontSize = 12;
                temp.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
                temp.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
                temp.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
                temp.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;

                labelChampions.Add(temp);
                gridChampions.Children.Add(temp);

                Grid.SetRow(temp, row);
                Grid.SetColumn(temp, column);

                column++;

                NumericUpDown tempUDPick = new Avalonia.Controls.NumericUpDown();

                tempUDPick.Name = champ.key + "pick";
                tempUDPick.Value = 0;
                tempUDPick.Minimum = 0;
                tempUDPick.Maximum = 200;
                tempUDPick.Increment = 1;
                tempUDPick.MinWidth = 130;
                tempUDPick.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
                tempUDPick.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
                tempUDPick.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
                tempUDPick.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                tempUDPick.AddHandler(Avalonia.Controls.NumericUpDown.ValueChangedEvent, formChampionsPickValueChanged);
                gridChampions.Children.Add(tempUDPick);
                Grid.SetRow(tempUDPick, row);
                Grid.SetColumn(tempUDPick, column);
                pick.Add(tempUDPick);

                column++;

                NumericUpDown tempUDBan = new Avalonia.Controls.NumericUpDown();
                tempUDBan = new NumericUpDown();
                tempUDBan.Name = champ.key + "ban";
                tempUDBan.Value = 0;
                tempUDBan.Minimum = 0;
                tempUDBan.Maximum = 200;
                tempUDBan.Increment = 1;
                tempUDBan.MinWidth = 130;
                tempUDBan.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
                tempUDBan.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
                tempUDBan.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
                tempUDBan.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                tempUDBan.AddHandler(Avalonia.Controls.NumericUpDown.ValueChangedEvent, formChampionsBanValueChanged);
                gridChampions.Children.Add(tempUDBan);
                Grid.SetRow(tempUDBan, row);
                Grid.SetColumn(tempUDBan, column);
                ban.Add(tempUDBan);

                column++;

                NumericUpDown tempUDAram = new Avalonia.Controls.NumericUpDown();
                tempUDAram = new NumericUpDown();
                tempUDAram.Name = champ.key + "aram";
                tempUDAram.Value = 0;
                tempUDAram.Minimum = 0;
                tempUDAram.Maximum = 200;
                tempUDAram.Increment = 1;
                tempUDAram.MinWidth = 130;
                tempUDAram.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
                tempUDAram.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
                tempUDAram.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
                tempUDAram.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                tempUDAram.AddHandler(Avalonia.Controls.NumericUpDown.ValueChangedEvent, formChampionsAramValueChanged);
                gridChampions.Children.Add(tempUDAram);
                Grid.SetRow(tempUDAram, row);
                Grid.SetColumn(tempUDAram, column);
                aram.Add(tempUDAram);


                //tempNUD.MouseWheel += FormChampions_MouseWheel;



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

                //pick[index].ValueChanged += formChampionsPickValueChanged;

                //ban[index].ValueChanged += formChampionsBanValueChanged; ;

                //aram[index].ValueChanged += formChampionsAramValueChanged; ;


                column = 0;
                row++;
                index++;
            }
        }
        public static void FormInformationSetup()
        {
            formInformation.Title = "Informations";
            formInformation.Closing += FormInformation_Closing;
            formInformation.Content = gridInformations;
            formInformation.SizeToContent = SizeToContent.WidthAndHeight;
            gridInformations.ShowGridLines = debug;

            Label labelInformation = new Label();
            labelInformation.Content = "To follow the tool developement you can look at the Github or join the Discord";
            labelInformation.Width = Double.NaN;
            labelInformation.Height = Double.NaN;
            labelInformation.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

            Label linkLabelGithub = new Label();
            linkLabelGithub.Content = "Github";
            linkLabelGithub.Width = Double.NaN;
            linkLabelGithub.Height = Double.NaN;
            linkLabelGithub.Foreground = Avalonia.Media.Brushes.Blue;
            linkLabelGithub.AddHandler(Label.PointerPressedEvent, LinkLabelGithub_MouseLeftButtonUp);
            linkLabelGithub.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            linkLabelGithub.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;

            Label linkLabelDiscord = new Label();
            linkLabelDiscord.Content = "Discord";
            linkLabelDiscord.Width = Double.NaN;
            linkLabelDiscord.Height = Double.NaN;
            linkLabelDiscord.Foreground = Avalonia.Media.Brushes.Blue;
            linkLabelDiscord.AddHandler(Label.PointerPressedEvent, LinkLabelDiscord_MouseLeftButtonUp);
            linkLabelDiscord.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            linkLabelDiscord.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;

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
            labelPrePick.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            labelPrePick.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            labelPrePick.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            labelPrePick.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;

            Label labelPick = new Label();
            labelPick.Content = "Pick delay (ms) :";
            labelPick.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            labelPick.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            labelPick.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            labelPick.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;

            Label labelBan = new Label();
            labelBan.Content = "Ban delay (ms) :";
            labelBan.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            labelBan.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            labelBan.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            labelBan.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;

            NumericUpDown numericUpDownPrePickDelay = new NumericUpDown();
            numericUpDownPrePickDelay.Value = 0;
            numericUpDownPrePickDelay.Maximum = 25000;
            numericUpDownPrePickDelay.Minimum = 0;
            numericUpDownPrePickDelay.Increment = 1000;
            numericUpDownPrePickDelay.Width = Double.NaN;
            numericUpDownPrePickDelay.Height = Double.NaN;
            numericUpDownPrePickDelay.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            numericUpDownPrePickDelay.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            numericUpDownPrePickDelay.AddHandler(Avalonia.Controls.NumericUpDown.ValueChangedEvent, PrePickDelay_ValueChanged);// += NumericUpDown PrePickDelay_ValueChanged;

            NumericUpDown numericUpDownPickDelay = new NumericUpDown();
            numericUpDownPickDelay.Value = 0;
            numericUpDownPickDelay.Maximum = 25000;
            numericUpDownPickDelay.Minimum = 0;
            numericUpDownPickDelay.Increment = 1000;
            numericUpDownPickDelay.Width = Double.NaN;
            numericUpDownPickDelay.Height = Double.NaN;
            numericUpDownPickDelay.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            numericUpDownPickDelay.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            numericUpDownPickDelay.AddHandler(NumericUpDown.ValueChangedEvent, PickDelay_ValueChanged);// += NumericUpDown PickDelay_ValueChanged;

            NumericUpDown numericUpDownBanDelay = new NumericUpDown();
            numericUpDownBanDelay.Value = 0;
            numericUpDownBanDelay.Maximum = 25000;
            numericUpDownBanDelay.Minimum = 0;
            numericUpDownBanDelay.Increment = 1000;
            numericUpDownBanDelay.Width = Double.NaN;
            numericUpDownBanDelay.Height = Double.NaN;
            numericUpDownBanDelay.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            numericUpDownBanDelay.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            numericUpDownBanDelay.AddHandler(NumericUpDown.ValueChangedEvent, BanDelay_ValueChanged);  // += NumericUpDown BanDelay_ValueChanged;

            if (configData.prePickDelay != null) { numericUpDownPrePickDelay.Value = configData.prePickDelay; }
            if (configData.pickDelay != null) { numericUpDownPickDelay.Value = configData.pickDelay; }
            if (configData.banDelay != null) { numericUpDownBanDelay.Value = configData.banDelay; }



            gridDelays.Children.Add(labelPrePick);
            gridDelays.Children.Add(labelPick);
            gridDelays.Children.Add(labelBan);
            gridDelays.Children.Add(numericUpDownPrePickDelay);
            gridDelays.Children.Add(numericUpDownPickDelay);
            gridDelays.Children.Add(numericUpDownBanDelay);

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
            Grid.SetColumn(numericUpDownPrePickDelay, 1);
            Grid.SetRow(numericUpDownPrePickDelay, 0);
            Grid.SetColumn(numericUpDownPickDelay, 1);
            Grid.SetRow(numericUpDownPickDelay, 1);
            Grid.SetColumn(numericUpDownBanDelay, 1);
            Grid.SetRow(numericUpDownBanDelay, 2);

        }

        public void setupFormLogin ()
        {
            formLogin.Title = "LOL Client TOOL - game version: " + version + " - Login needed";
            formLogin.Icon = icco;

            Grid gridLogin = new Grid();
            gridLogin.RowDefinitions.Add(new RowDefinition());
            gridLogin.RowDefinitions.Add(new RowDefinition());
            gridLogin.RowDefinitions.Add(new RowDefinition());
            gridLogin.RowDefinitions.Add(new RowDefinition());
            gridLogin.ColumnDefinitions.Add(new ColumnDefinition());
            gridLogin.ColumnDefinitions.Add(new ColumnDefinition());

            formLogin.Closing += FormLogin_Closing;

            formLogin.Content = gridLogin;

            textBoxUsername.Watermark = "user name";
            textBoxUsername.MinWidth = 150;
            textBoxUsername.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            textBoxUsername.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            textBoxUsername.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            textBoxUsername.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            gridLogin.Children.Add(textBoxUsername);


            textBoxPassword.Watermark = "password";
            textBoxPassword.MinWidth = 150;
            textBoxPassword.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            textBoxPassword.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            textBoxPassword.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            textBoxPassword.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            textBoxPassword.PasswordChar = '*';
            gridLogin.Children.Add(textBoxPassword);

            buttonLogin.Content = "Login";
            buttonLogin.MinWidth = 150;
            buttonLogin.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            buttonLogin.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            buttonLogin.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            buttonLogin.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            buttonLogin.AddHandler(Button.ClickEvent, buttonLogin_Clicked);
            gridLogin.Children.Add(buttonLogin);

            Button buttonSaveCredentials = new Button();
            buttonSaveCredentials.Content = "Save account";
            buttonSaveCredentials.MinWidth = 150;
            buttonSaveCredentials.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            buttonSaveCredentials.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            buttonSaveCredentials.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            buttonSaveCredentials.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            buttonSaveCredentials.AddHandler(Button.ClickEvent, buttonSaveCredentials_Clicked);
            gridLogin.Children.Add(buttonSaveCredentials);

            buttonDisconnect.Content = "Disconnect";
            buttonDisconnect.MinWidth = 150;
            buttonDisconnect.IsEnabled = false;
            buttonDisconnect.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            buttonDisconnect.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            buttonDisconnect.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            buttonDisconnect.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            buttonDisconnect.AddHandler(Button.ClickEvent, buttonDisconnect_Clicked);
            gridLogin.Children.Add(buttonDisconnect);

            buttonRemoveAccount.Content = "Remove account";
            buttonRemoveAccount.MinWidth = 150;
            buttonRemoveAccount.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            buttonRemoveAccount.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            buttonRemoveAccount.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            buttonRemoveAccount.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            buttonRemoveAccount.AddHandler(Button.ClickEvent, buttonRemoveAccount_Clicked);
            gridLogin.Children.Add(buttonRemoveAccount);

            CheckBox checkBoxPersitentLogin = new CheckBox();
            checkBoxPersitentLogin.Content = "Stay logged";
            checkBoxPersitentLogin.MinWidth = 150;
            checkBoxPersitentLogin.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            checkBoxPersitentLogin.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            checkBoxPersitentLogin.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            checkBoxPersitentLogin.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            gridLogin.Children.Add(checkBoxPersitentLogin);

            comboBoxSavedAccounts.MinWidth = 150;
            comboBoxSavedAccounts.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            comboBoxSavedAccounts.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            comboBoxSavedAccounts.HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left;
            comboBoxSavedAccounts.VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center;
            comboBoxSavedAccounts.AddHandler(ComboBox.SelectionChangedEvent, comboBoxSavedAccounts_SelectionChanged);
            comboBoxSavedAccounts.SelectedIndex = 0;
            gridLogin.Children.Add(comboBoxSavedAccounts);

            Grid.SetRow(buttonSaveCredentials, 0);
            Grid.SetColumn(buttonSaveCredentials, 0);
            Grid.SetRow(textBoxUsername, 0);
            Grid.SetColumn(textBoxUsername, 1);
            Grid.SetRow(textBoxPassword, 1);
            Grid.SetColumn(textBoxPassword, 1);
            Grid.SetRow(buttonLogin, 2);
            Grid.SetColumn(buttonLogin, 1);
            Grid.SetRow(checkBoxPersitentLogin, 2);
            Grid.SetColumn(checkBoxPersitentLogin, 0);
            Grid.SetRow(comboBoxSavedAccounts, 1);
            Grid.SetColumn(comboBoxSavedAccounts, 0);
            Grid.SetRow(buttonRemoveAccount, 3);
            Grid.SetColumn(buttonRemoveAccount, 0);
            Grid.SetRow(buttonDisconnect, 3);
            Grid.SetColumn(buttonDisconnect, 1);


            formLogin.SizeToContent = SizeToContent.WidthAndHeight;

            textBoxUsername.Text = "";
            textBoxPassword.Text = "";

            formLogin.MinHeight = formLogin.Height;
            formLogin.MinWidth = formLogin.Width;
        }

        private void FormLogin_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            formLogin.Hide();
        }

        private static void BanDelay_ValueChanged(object sender, RoutedEventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            configData.banDelay = (int)numericUpDown.Value;
            configSaveAsync();
        }        
        private static void comboBoxSavedAccounts_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox savedUsername = (ComboBox)sender;
            textBoxUsername.Text = (string)savedUsername.SelectedItem;
            foreach(leagueOfLegendsAccount account in configData.leagueOfLegendsAccounts)
            {
                if(account.username == textBoxUsername.Text)
                {
                    textBoxPassword.Text = DecodeFrom64(account.password).Replace(Environment.UserName, "");
                }
            }
        }
                
        private static void buttonSaveCredentials_Clicked(object sender, RoutedEventArgs e)
        {
            if(textBoxUsername.Text != "" && textBoxPassword.Text != "")
            {

                leagueOfLegendsAccount leagueOfLegendsAccount = new leagueOfLegendsAccount();
                leagueOfLegendsAccount.username = textBoxUsername.Text;
                leagueOfLegendsAccount.password = EncodePasswordToBase64(textBoxPassword.Text + Environment.UserName + Environment.UserName);
                var accToCheck = configData.leagueOfLegendsAccounts.Find(x => x.username == leagueOfLegendsAccount.username);
                if (configData.leagueOfLegendsAccounts == null)
                {
                    configData.leagueOfLegendsAccounts = new List<leagueOfLegendsAccount>();
                    configData.leagueOfLegendsAccounts.Add(leagueOfLegendsAccount);
                    comboBoxSavedAccounts.Items = new String[] { leagueOfLegendsAccount.username };
                    configSaveAsync();
                }
                else if (accToCheck == null)
                {
                    configData.leagueOfLegendsAccounts.Add(leagueOfLegendsAccount);
                    usernames.Add(leagueOfLegendsAccount.username);
                    //comboBoxSavedAccounts.ItemCount = list.Count();
                    comboBoxSavedAccounts.Items = new List<string>();
                    comboBoxSavedAccounts.Items = usernames;
                    comboBoxSavedAccounts.SelectedIndex = usernames.Count - 1;
                    configSaveAsync();
                }
            }
        }
        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }
        //this function Convert to Decord your Password
        public static string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }

        private static void PickDelay_ValueChanged(object sender, RoutedEventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            configData.pickDelay = (int)numericUpDown.Value;
            configSaveAsync();
        }

        private static void PrePickDelay_ValueChanged(object sender, RoutedEventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
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

        private static void LinkLabelDiscord_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            var startInfo = new ProcessStartInfo("explorer.exe", "https://discord.gg/ZE7fZrFeJd");
            Process.Start(startInfo);
        }

        private static void LinkLabelGithub_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            var startInfo = new ProcessStartInfo("explorer.exe", "https://github.com/Terevenen2/LOL-Client-TOOL");
            Process.Start(startInfo);
        }


        private static void formChampionsAramValueChanged(object sender, RoutedEventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            string name = numericUpDown.Name.ToString().ToLower().Replace("aram", "");
            int index = configData.championPickList.FindIndex(n => n.Id.ToString().Equals(name));
            configData.championPickList[index].Aram = Convert.ToInt32(numericUpDown.Value);
            configSaveAsync();
        }

        private static void formChampionsBanValueChanged(object sender, RoutedEventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            string name = numericUpDown.Name.ToString().ToLower().Replace("ban", "");
            int index = configData.championPickList.FindIndex(n => n.Id.ToString().Equals(name));
            configData.championPickList[index].Ban = Convert.ToInt32(numericUpDown.Value);
            configSaveAsync();
        }

        private static void formChampionsPickValueChanged(object sender, RoutedEventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            string name = numericUpDown.Name.ToString().ToLower().Replace("pick", "");
            int index = configData.championPickList.FindIndex(n => n.Id.ToString().Equals(name));
            configData.championPickList[index].Pick = Convert.ToInt32(numericUpDown.Value);
            configSaveAsync();
        }

        private static void TextBoxFilterChampions_TextChanged(object sender, RoutedEventArgs e)
        {
            var watch = new System.Diagnostics.Stopwatch();
            List<string> times = new List<string>();

            TextBox theSearch = (TextBox)sender;
            int row = 2, index = 0, count2 = 0;
            var labels = gridChampions.Children.OfType<Label>();
            var numericUpDowns = gridChampions.Children.OfType<NumericUpDown>();
            watch.Start();
            foreach (var label in labels)
            {
                if (label.Content.ToString().ToLower().Contains(theSearch.Text.ToLower()) && label.Name != null)
                {
                    Grid.SetRow(label, row);
                    label.IsVisible = true;

                    var temp2 = numericUpDowns.Where(n => n.Name.ToString().ToLower().Equals(label.Name.ToString().ToLower() + "pick"));
                    Grid.SetRow(temp2.First(), row);
                    temp2.First().IsVisible = true;

                    temp2 = numericUpDowns.Where(n => n.Name.ToString().ToLower().Equals(label.Name.ToString().ToLower() + "ban"));
                    Grid.SetRow(temp2.First(), row);
                    temp2.First().IsVisible = true;

                    temp2 = numericUpDowns.Where(n => n.Name.ToString().ToLower().Equals(label.Name.ToString().ToLower() + "aram"));
                    Grid.SetRow(temp2.First(), row);
                    temp2.First().IsVisible = true;

                    row++;
                }
                else
                {
                    if (label.Name != null)
                    {
                        label.IsVisible = false;
                        var temp2 = numericUpDowns.Where(n => n.Name.ToString().ToLower().Equals(label.Name.ToString().ToLower() + "pick"));
                        temp2.First().IsVisible = false;

                        temp2 = numericUpDowns.Where(n => n.Name.ToString().ToLower().Equals(label.Name.ToString().ToLower() + "ban"));
                        temp2.First().IsVisible = false;

                        temp2 = numericUpDowns.Where(n => n.Name.ToString().ToLower().Equals(label.Name.ToString().ToLower() + "aram"));
                        temp2.First().IsVisible = false;
                    }
                }
            }
            watch.Stop();
            times.Add(watch.ElapsedMilliseconds.ToString());
        }

        private static void ButtonResetAram_Click(object sender, RoutedEventArgs e)
        {
            foreach (var temp in gridChampions.Children.OfType<NumericUpDown>().Where(x => x.Name.ToString().Contains("aram")))
            {
                temp.Value = 0;
            }
        }

        private static void ButtonResetBans_Click(object sender, RoutedEventArgs e)
        {
            foreach (var temp in gridChampions.Children.OfType<NumericUpDown>().Where(x => x.Name.ToString().Contains("ban")))
            {
                temp.Value = 0;
            }
        }

        private static void ButtonResetPicks_Click(object sender, RoutedEventArgs e)
        {
            foreach (var temp in gridChampions.Children.OfType<NumericUpDown>().Where(x => x.Name.ToString().Contains("pick")))
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

        private static async Task LabelSummonerDisplayName_MouseLeftButtonUpAsync(object sender, RoutedEventArgs e)
        {
            await Application.Current.Clipboard.SetTextAsync(labelSummonerDisplayName.Content.ToString());
        }

        public MainWindow()
        {
            isDebug();
            InitializeComponent();
            SetTimer();
            getVersion();
            windowMain.Title = "LOL Client TOOL - game version: " + version;
            // set source name for the new theme

            //hearth beat client alive
            //System.Timers.Timer hearthBeat = new System.Timers.Timer();
            //hearthBeat.Elapsed += new ElapsedEventHandler(HearthBeatEvent);
            //hearthBeat.Interval = 5000;
            //hearthBeat.Enabled = true;

            windowMain.Content = gridMain;
            windowMain.SizeToContent = SizeToContent.WidthAndHeight;

            
            windowMain.Icon = icco;
            formChampions.Icon = icco;
            formDelays.Icon = icco;
            formInformation.Icon = icco;
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
            windowMain.MinWidth = 1000;
            windowMain.Width = 1000;

            gridMain.ShowGridLines = debug; ;

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
            gridMain.Children.Add(buttonAccount);
            gridMain.Children.Add(checkBoxAutoPosition);
            gridMain.Children.Add(checkBoxAutoMessage);
            gridMain.Children.Add(checkBoxAutoReroll);
            gridMain.Children.Add(checkBoxAutoSkin);
            gridMain.Children.Add(comboBoxAutoPosition1);
            gridMain.Children.Add(textBoxConversationMessage);
            gridMain.Children.Add(comboBoxAutoPosition2);
            gridMain.Children.Add(comboBoxLightOnOff);
            gridMain.Children.Add(labelInfo);
            gridMain.Children.Add(comboBoxLanguage);
            Grid.SetColumn(labelSummonerDisplayName, 0);
            Grid.SetRow(labelSummonerDisplayName, 0);
            Grid.SetColumn(summonerIcon, 0);
            Grid.SetRow(summonerIcon, 1);
            Grid.SetRowSpan(summonerIcon, 4);
            Grid.SetColumn(buttonChampions, 1);
            Grid.SetRow(buttonChampions, 0);
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
            Grid.SetColumn(buttonAccount, 3);
            Grid.SetRow(buttonAccount, 4);
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
            Grid.SetColumn(comboBoxLightOnOff, 7);
            Grid.SetRow(comboBoxLightOnOff, 0);
            Grid.SetColumn(labelInfo, gridMain.ColumnDefinitions.Count - 1);
            Grid.SetRow(labelInfo, gridMain.RowDefinitions.Count - 1);
            Grid.SetColumn(comboBoxLanguage, 7);
            Grid.SetRow(comboBoxLanguage, 1);

            //this.Controls.Add(verifyOwnedIcons);
            setupLCU();
            loadConfig();
            setupFormAsync();
            FormChampionsSetup();
            FormInformationSetup();
            FormDelaysSetup();
            setupFormLogin();

            var task2 = Task.Run(async () => {
                while (true)
                {
                    try
                    {
                        await OnTimedEvent();
                    }
                    catch
                    {
                        //super easy error handling
                    }
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
        }


        public void buttonDisconnect_Clicked(object sender, RoutedEventArgs e)
        {
            //MakeHttpRequest("DELETE", "​lol-login​/v1​/session",  "");
            MakeHttpRequest("DELETE", "​lol-rso-auth​/v1​/session",  "");
        }        
        private void buttonRemoveAccount_Clicked(object sender, RoutedEventArgs e)
        {
            if(comboBoxSavedAccounts.SelectedItem != null)
            {
                string usnername = (string)comboBoxSavedAccounts.SelectedItem;
                configData.leagueOfLegendsAccounts.Remove(configData.leagueOfLegendsAccounts.Find(x => x.username == usnername));
                usernames.Remove(usnername);
                //comboBoxSavedAccounts.SelectedIndex = 0;
                //comboBoxSavedAccounts.Items = new List<string>();
                var names = usernames;
                comboBoxSavedAccounts.Items = names;
                comboBoxSavedAccounts_Update();
                configSaveAsync();
            }
        }
        public void comboBoxSavedAccounts_Update()
        {
            var items = comboBoxSavedAccounts.Items;
            comboBoxSavedAccounts.Items = null;
            comboBoxSavedAccounts.Items = items;
            comboBoxSavedAccounts.SelectedIndex = 0;
        }
        private void buttonLogin_Clicked(object sender, RoutedEventArgs e)
        {
            if (RiotClientConnect())
            {
                string rchText = "{\"username\":\"xxx\",\"password\":\"qqq\",\"persistLogin\":false}";

                rchText = rchText.Replace("xxx", textBoxUsername.Text);
                rchText = rchText.Replace("qqq", textBoxPassword.Text);

                string resp = MakeHttpRequest("put", "rso-auth/v1/session/credentials", rchText);

                if (resp.Contains("auth_failure"))
                {
                    //MessageBox.Show("Wrong username or password!", "Error!"
                    //    , MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                //MessageBox.Show("Please open League Of Legends Client!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            System.Diagnostics.Process.Start(configData.leagueOfLegendsClientExe.Replace("\"", "").Replace("\\\\", "\\").Replace("\\\\", "\\"));
        }

        private void windowMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Application.Current.Shutdown();
            //formChampions.Close();
            //formDelays.Close();
            //formInformation.Close();
            Close();
        }
    }
}