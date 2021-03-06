using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Timers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace LOL_CLient_TOOL
{
    public partial class Form1 : Form
    {
        public static bool debug = false;//set true to display inputs for testing lcu requests
        public static bool preview = false;
        public static ConfigurationData configData = new ConfigurationData();

        public static Dictionary<string, string> lesArguments = new Dictionary<string, string>();
        public static Dictionary<string, string> riotCredntials = new Dictionary<string, string>();
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
        public static string[] postions = new string[] { "TOP", "JUNGLE", "MIDDLE", "BOTTOM", "UTILITY", "FILL" };//  TOP JUNGLE MIDDLE BOTTOM UTILITY FILL
        public static int champPickId = 0;
        public static int instalockCount = 0;
        public static int downloadingRunes = 0;
        public static int downloadedRunes = 0;
        public static int AutoQPlayAgianIntervalle = 0;

        public static bool formSummonerIconLoadComplete = false;
        public static bool processingAllIcon = false;
        public static bool formSummonerRuneIsSetup = false;
        public static bool instalockIsEnabled = false;
        public static bool formSetupFaild = true;

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
            public List<championPick> championPickList { get; set; }
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
        public static Form FormSummonerIcon = new Form();
        public static Form FormSummonerRunes = new Form();
        public static Form FormChampSelect = new Form();
        public static Form formLogin = new Form();
        public static Form formChampions = new Form();

        public static ComboBox comboBoxRunesPages = new ComboBox();
        public static ComboBox comboBoxSummonerStatus = new ComboBox();
        public static ComboBox comboBoxAutoPosition1 = new ComboBox();
        public static ComboBox comboBoxAutoPosition2 = new ComboBox();

        public static List<ComboBoxIdName> summonerStatus = new List<ComboBoxIdName>();

        public static Label labelSummonerDisplayName = new Label();

        public static ToolTip tooltip = new ToolTip();

        public static MenuStrip mainFormMenu = new MenuStrip();

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


        public static List<PictureBox> allSummonerIcons = new List<PictureBox>();
        public static PictureBox summonerIcon = new PictureBox();
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
        public void summonerIcon_LoadCompleted(object sender, EventArgs e)
        {
            PictureBox uneImage = (PictureBox)sender;
            string path = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\LOL_Client_TOOL\\icon\\";
            //MessageBox.Show(uneImage.ImageLocation);
            string[] name = uneImage.ImageLocation.Split(Convert.ToChar("/"));
            string leNom = name[7];
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/png");
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            DirectoryInfo di = Directory.CreateDirectory(path);
            myEncoderParameters.Param[0] = myEncoderParameter;
            uneImage.Image.Save(path + leNom, myImageCodecInfo, myEncoderParameters);

        }

        public async void summonerIcon_Click(object sender, EventArgs e)
        {
            PictureBox saveSummonerIcon = summonerIcon;
            FormSummonerIcon.Text = "Loading/downloading icons : can take long time closing the app wont reset loading/downloading";
            FormSummonerIcon.Show();
            FormSummonerIcon.AutoScroll = true;
            fillFormWithIcons2Async();
            //await Task.Run(() => fillFormWithIcons2Async());
            //fillFormWithIconsAsync();
            summonerIcon.Image = saveSummonerIcon.Image;
            summonerIcon.Refresh();
            int i = 0;
            int j = 0;
            foreach (PictureBox icon in allSummonerIcons)
            {
                int count = 0;
                foreach (PictureBox ico in allSummonerIcons)
                {
                    if (icon.Name == ico.Name)
                    {
                        count++;
                        if (count == 2)
                        {
                            allSummonerIcons.Remove(ico);
                            break;
                        }
                    }
                }
            }
            foreach (PictureBox icon in allSummonerIcons)
            {
                if (i >= 27)
                {
                    i = 0;
                    j = j + 70;
                }
                icon.Location = new System.Drawing.Point(i * 70, j);
                FormSummonerIcon.Controls.Add(icon);
                i++;
            }
            //FormSummonerIcon.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            FormSummonerIcon.Text = "Choose an icon";
        }
        private async Task fillFormWithIcons2Async()
        {
            string fileName1 = "ownedIcons" + currentSummoner.summonerId + ".txt";
            string path = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\LOL_Client_TOOL\\icon\\";
            string lesIconJson = DDragonRequest("/data/en_US/profileicon.json");
            var icons = JObject.Parse(lesIconJson);
            if (!File.Exists(path + fileName1))
            {
                using (StreamWriter sw = File.CreateText(path + fileName1)) { }
            }
            string[] ownedIcons1 = File.ReadLines(path + fileName1).ToArray();
            string last = "";
            string lastId = "";
            ownedIcons1 = ownedIcons1.Distinct().ToArray();
            if (ownedIcons1.Length > 0)
            {
                last = ownedIcons1.Last();
            }
            Dictionary<int, string> lesIcon = new Dictionary<int, string>();
            foreach (var icon in icons["data"])
            {
                string[] uneIcon = icon.ToString().Split(new[] { "id" }, StringSplitOptions.None);
                string icco = Regex.Replace(uneIcon[0], @"[^\d]", "");
                lesIcon.Add(lesIcon.Count, icco);
                if (last == icco)
                {
                    lastId = lesIcon.Count.ToString();
                }
            }
            if (lastId == "") { lastId = "-1"; }
            foreach (KeyValuePair<int, string> ico in lesIcon)
            {
                if (ico.Key > Convert.ToInt32(lastId))
                {
                    string resp = LCURequest("/lol-summoner/v1/current-summoner/icon", "PUT", "{ \"profileIconId\" : \"" + ico.Value + "\"}").Value;
                    if (resp.Contains(currentSummoner.displayName) || resp.Contains("(429) Bad Request"))
                    {
                        while (resp.Contains("(429) Bad Request"))
                        {
                            resp = LCURequest("/lol-summoner/v1/current-summoner/icon", "PUT", "{ \"profileIconId\" : \"" + ico.Value + "\"}").Value;
                        }
                        if (resp.Contains(currentSummoner.displayName))
                        {
                            File.AppendAllText(path + fileName1, ico.Value + Environment.NewLine);
                            PictureBox unIcon = new PictureBox();
                            if (File.Exists(path + ico.Value + ".png"))
                            {
                                unIcon.Image = Image.FromFile(path + ico.Value + ".png");
                            }
                            else
                            {
                                unIcon.LoadAsync("http://ddragon.leagueoflegends.com/cdn/" + version + "/img/profileicon/" + ico.Value + ".png");
                                unIcon.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(summonerIcon_LoadCompleted);
                            }
                            unIcon.SizeMode = PictureBoxSizeMode.StretchImage;
                            allSummonerIcons.Add(unIcon);
                            unIcon.Name = ico.Value;
                            unIcon.Click += UnIcon_Click;
                            summonerIcons.Add(ico.Value);
                        }
                    }
                }
            }
        }

        private async Task fillFormWithIconsAsync()
        {
            if (formSummonerIconLoadComplete == false)
            {
                formSummonerIconLoadComplete = true;
                string lesIconJson = DDragonRequest("/data/en_US/profileicon.json");
                var icons = JObject.Parse(lesIconJson);
                Dictionary<int, string> lesIcon = new Dictionary<int, string>();
                foreach (var icon in icons["data"])
                {
                    string[] uneIcon = icon.ToString().Split(new[] { "id" }, StringSplitOptions.None);
                    string icco = Regex.Replace(uneIcon[0], @"[^\d]", "");
                    lesIcon.Add(lesIcon.Count, icco);
                }


                string fileName1 = "ownedIcons" + currentSummoner.summonerId + ".txt";

                string path = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\LOL_Client_TOOL\\icon\\";

                //create txt file to store ownedIcons
                if (!File.Exists(path + fileName1))
                {
                    using (StreamWriter sw = File.CreateText(path + fileName1)) { }
                }
                string[] ownedIcons1 = File.ReadLines(path + fileName1).ToArray();
                ownedIcons1 = ownedIcons1.Distinct().ToArray();
                if (ownedIcons1.Length > 0)
                {
                    string lastIcon1 = ownedIcons1.Last();
                    if (lesIcon.Count >= 1)
                    {
                        foreach (KeyValuePair<int, string> ico in lesIcon)
                        {
                            if (ico.Value == lastIcon1)
                            {
                                break;
                            }
                            currentSummonerTestedIcon.Add(ico.Key, ico.Value);
                        }
                    }
                }

                if (verifyOwnedIcons.Checked)
                {
                    allSummonerIcons.Clear();
                    bool oneTime = false;
                    foreach (var icon in icons["data"])
                    {
                        string fileName = "ownedIcons" + currentSummoner.summonerId + ".txt";
                        string[] uneIcon = icon.ToString().Split(new[] { "id" }, StringSplitOptions.None);
                        string lastIcon = "";
                        //create txt file to store ownedIcons
                        if (!File.Exists(path + fileName))
                        {
                            using (StreamWriter sw = File.CreateText(path + fileName)) { }
                        }
                        string[] ownedIcons = File.ReadLines(path + fileName).ToArray();
                        if (ownedIcons.Length > 0)
                        {
                            ownedIcons = ownedIcons.Distinct().ToArray();
                            lastIcon = ownedIcons.Last();
                        }


                        if (oneTime == false)
                        {
                            foreach (string tempIcon in ownedIcons)
                            {
                                PictureBox unIcon = new PictureBox();
                                unIcon.Size = new System.Drawing.Size(70, 70);
                                if (File.Exists(path + tempIcon + ".png"))
                                {
                                    unIcon.Image = Image.FromFile(path + tempIcon + ".png");
                                }
                                else
                                {
                                    string url = "http://ddragon.leagueoflegends.com/cdn/" + version + "/img/profileicon/" + tempIcon + ".png";
                                    unIcon.LoadAsync(url);
                                    unIcon.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(summonerIcon_LoadCompleted);
                                }

                                unIcon.SizeMode = PictureBoxSizeMode.StretchImage;
                                unIcon.Name = tempIcon;
                                unIcon.Click += UnIcon_Click;
                                allSummonerIcons.Add(unIcon);
                                summonerIcons.Add(Regex.Replace(tempIcon, @"[^\d]", ""));
                            }
                            oneTime = true;
                        }
                        string oldIcon = currentSummoner.profileIconId;
                        if (!ownedIcons.Contains(Regex.Replace(uneIcon[0], @"[^\d]", "")))
                        {
                            if (!currentSummonerTestedIcon.ContainsValue(Regex.Replace(uneIcon[0], @"[^\d]", "")))
                            {
                                currentSummonerTestedIcon.Add(currentSummonerTestedIcon.Count, Regex.Replace(uneIcon[0], @"[^\d]", ""));
                                PictureBox unIcon = new PictureBox();
                                unIcon.Size = new System.Drawing.Size(70, 70);
                                int loop = 1;
                                string response = LCURequest("/lol-summoner/v1/current-summoner/icon", "PUT", "{ \"profileIconId\" : \"" + Regex.Replace(uneIcon[0], @"[^\d]", "") + "\"}").Value;
                                while (response.Contains("(429) Bad Request"))//401 don't have icon and 429 too fast
                                {
                                    Thread.Sleep(50 * loop);
                                    response = LCURequest("/lol-summoner/v1/current-summoner/icon", "PUT", "{ \"profileIconId\" : \"" + Regex.Replace(uneIcon[0], @"[^\d]", "") + "\"}").Value;
                                    Debug.WriteLine(loop.ToString() + " " + response + " icon : " + Regex.Replace(uneIcon[0], @"[^\d]", ""));
                                    loop++;
                                    //MessageBox.Show(response + "\n" + "{ \"profileIconId\" : \"" + Regex.Replace(uneIcon[0], @"[^\d]", "") + "\"}");
                                    getSummoner();
                                    if (oldIcon != currentSummoner.profileIconId)
                                    {
                                        if (!ownedIcons.Contains(currentSummoner.profileIconId))
                                        {
                                            File.AppendAllText(path + fileName, currentSummoner.profileIconId + Environment.NewLine);
                                        }
                                        if (File.Exists(path + Regex.Replace(uneIcon[0], @"[^\d]", "") + ".png"))
                                        {
                                            unIcon.Image = Image.FromFile(path + Regex.Replace(uneIcon[0], @"[^\d]", "") + ".png");
                                        }
                                        else
                                        {
                                            string url = "http://ddragon.leagueoflegends.com/cdn/" + version + "/img/profileicon/" + Regex.Replace(uneIcon[0], @"[^\d]", "") + ".png";
                                            unIcon.LoadAsync(url);
                                            unIcon.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(summonerIcon_LoadCompleted);
                                        }

                                        unIcon.SizeMode = PictureBoxSizeMode.StretchImage;
                                        allSummonerIcons.Add(unIcon);
                                        unIcon.Name = Regex.Replace(uneIcon[0], @"[^\d]", "");
                                        unIcon.Click += UnIcon_Click;

                                        summonerIcons.Add(Regex.Replace(uneIcon[0], @"[^\d]", ""));
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    processingAllIcon = true;
                    foreach (var icon in icons["data"])
                    {
                        string[] uneIcon = icon.ToString().Split(new[] { "id" }, StringSplitOptions.None);
                        PictureBox unIcon = new PictureBox();
                        unIcon.Size = new System.Drawing.Size(70, 70);
                        if (File.Exists(path + Regex.Replace(uneIcon[0], @"[^\d]", "") + ".png"))
                        {
                            unIcon.Image = Image.FromFile(path + Regex.Replace(uneIcon[0], @"[^\d]", "") + ".png");
                        }
                        else
                        {
                            string url = "http://ddragon.leagueoflegends.com/cdn/" + version + "/img/profileicon/" + Regex.Replace(uneIcon[0], @"[^\d]", "") + ".png";
                            unIcon.LoadAsync(url);
                            unIcon.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(summonerIcon_LoadCompleted);
                        }

                        unIcon.SizeMode = PictureBoxSizeMode.StretchImage;
                        allSummonerIcons.Add(unIcon);
                        unIcon.Name = Regex.Replace(uneIcon[0], @"[^\d]", "");
                        unIcon.Click += UnIcon_Click;

                        summonerIcons.Add(Regex.Replace(uneIcon[0], @"[^\d]", ""));
                    }
                }
            }
            processingAllIcon = false;
        }

        private void UnIcon_Click(object sender, EventArgs e)
        {
            PictureBox uneImage = (PictureBox)sender;
            LCURequest("/lol-summoner/v1/current-summoner/icon", "PUT", "{ \"profileIconId\" : \"" + uneImage.Name + "\"}");
            summonerIcon.Image = getIcon(uneImage.Name).Image;
            summonerIcon.Refresh();
        }

        public static async void runeSetup()
        {
            buttonRune.Enabled = false;
            int elseTest = 0;
            FormSummonerRunes.BackColor = Color.FromArgb(24, 26, 27);
            string fileName = version + "data" + lang + "runesReforged.json";
            string path = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\LOL_Client_TOOL\\runes\\";
            string json = "";
            if (!File.Exists(path + fileName))
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string str = DDragonRequest("/data/" + lang + "/runesReforged.json");
                using (StreamWriter sw = File.CreateText(path + fileName)) { }

                File.AppendAllText(path + fileName, str);
                json = File.ReadAllText(path + fileName);
            }
            else
            {
                json = File.ReadAllText(path + fileName);
            }
            var runesData = JArray.Parse(json);
            foreach (var rune in runesData)
            {
                leagueRune tempRune = new leagueRune();
                tempRune.id = (int)rune["id"];
                tempRune.key = (string)rune["key"];
                tempRune.icon = (string)rune["icon"];
                if (!File.Exists(path + tempRune.icon.Split(chare, StringSplitOptions.None)[tempRune.icon.Split(chare, StringSplitOptions.None).Length - 1]))
                {
                    comboBoxRunesPages.Enabled = false;
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFileAsync(new Uri(DDragonImg + tempRune.icon), path + tempRune.icon.Split(chare, StringSplitOptions.None)[tempRune.icon.Split(chare, StringSplitOptions.None).Length - 1]);
                    }
                }
                tempRune.name = (string)rune["name"];
                Dictionary<int, Dictionary<int, Rune>> slots2 = new Dictionary<int, Dictionary<int, Rune>>();
                foreach (var slot in rune["slots"])
                {
                    Dictionary<int, Rune> slots = new Dictionary<int, Rune>();
                    foreach (var bip in slot["runes"])
                    {
                        Rune tempSlot = new Rune();
                        tempSlot.id = (int)bip["id"];
                        tempSlot.key = (string)bip["key"];
                        tempSlot.icon = (string)bip["icon"];
                        if (!File.Exists(path + tempSlot.icon.Split(chare, StringSplitOptions.None)[tempSlot.icon.Split(chare, StringSplitOptions.None).Length - 1]))
                        {
                            using (WebClient client = new WebClient())
                            {
                                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                                client.DownloadFileAsync(new Uri(DDragonImg + tempSlot.icon), path + tempSlot.icon.Split(chare, StringSplitOptions.None)[tempSlot.icon.Split(chare, StringSplitOptions.None).Length - 1]);
                                downloadingRunes += 1;
                            }
                        }
                        else
                        {
                            elseTest++;
                        }
                        tempSlot.name = (string)bip["name"];
                        tempSlot.shortDesc = (string)bip["shortDesc"];
                        tempSlot.longDesc = (string)bip["longDesc"];
                        slots.Add(slots.Count, tempSlot);
                    }
                    slots2.Add(slots2.Count, slots);
                }
                tempRune.slots = slots2;
                leagueOfLegendsRunes.Add(tempRune);
            }
            comboBoxRunesPages.Enabled = true;
            if (elseTest != 0)
            {
                buttonRune.Enabled = true;
            }
        }

        private static async void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            downloadedRunes++;
            buttonRune.Text = "LOAD: " + downloadedRunes + "/" + downloadingRunes;
            if (downloadedRunes == downloadingRunes)
            {
                buttonRune.Enabled = true;
                buttonRune.Text = "RUNES";
            }
        }

        public Form1()
        {
            InitializeComponent();
            SetTimer();
            getVersion();
            //this.Controls.Add(mainFormMenu);
            this.Controls.Add(labelSummonerDisplayName);
            //this.Controls.Add(verifyOwnedIcons);
            this.Text = "LOL Client TOOL - game version: " + version;
        }

        private static void setupForm()
        {
            //getting language
            var regionAndLanguage = JObject.Parse(LCURequest("/riotclient/get_region_locale", "GET").Value);
            lang = regionAndLanguage["locale"].ToString();

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
                checkBoxAutoAccept.Checked = configData.autoAccept;
                checkBoxAutoPick.Checked = configData.autoPick;
                checkBoxAutoBan.Checked = configData.autoBan;
                checkBoxAutoAramBenchSwap.Checked = configData.autoAramBenchSwap;
                checkBoxAutoPrePick.Checked = configData.autoPrePick;
                checkBoxAutoQPlayAgain.Checked = configData.autoPlayAgain;
                checkBoxAutoHonor.Checked = configData.autoHonor;
                checkBoxAutoPosition.Checked = configData.autoRole;
                checkBoxAutoMessage.Checked = configData.autoMessage;
                textBoxConversationMessage.Text = configData.autoMessageText;
                checkBoxAutoReroll.Checked = configData.autoReroll;
                checkBoxAutoSkin.Checked = configData.autoSkin;
                position1 = configData.autoRolePosition1;
                position2 = configData.autoRolePosition2;
                if(configData.championPickList == null)
                {
                    configData.championPickList = new List<championPick>();
                }
                else//JObject.Parse duplicate the picklist somehow lets fix it
                {
                    int i = 0;
                    List<int> champUniqueIds = new List<int>();
                    List<int> toRemove = new List<int>();
                    foreach(championPick championPick in configData.championPickList)
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
                    foreach(int nb in toRemove)
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
            getSummoner();
            runeSetup();
            mainFormMenu.BackColor = Color.Gray;
            string[] menuOption = new string[] { "File", "View" };
            string[] menuFileText = new string[] { "Open LOL Client TOOL data Folder" };
            foreach (string str in menuOption)
            {
                mainFormMenu.Items.Add(str);
            }

            FormSummonerIcon.FormClosing += FormSummonerIcon_FormClosing;
            FormSummonerIcon.Size = new Size(800, 300);

            FormSummonerRunes.FormClosing += FormSummonerRunes_FormClosing;
            FormSummonerRunes.Size = new Size(800, 350);
            FormSummonerRunes.Text = "Your rune pages";

            if (preview)
            {
                labelSummonerDisplayName.Text = "summoner name";
            }
            else
            {
                labelSummonerDisplayName.Text = currentSummoner.displayName;
            }

            //labelSummonerDisplayName.Text = "summoner name";
            labelSummonerDisplayName.Location = new Point(5, 25);
            labelSummonerDisplayName.Click += LabelSummonerDisplayName_Click;

            buttonChampions.Location = new Point(labelSummonerDisplayName.Location.X + labelSummonerDisplayName.Width + 5, labelSummonerDisplayName.Location.Y);
            buttonChampions.Text = "champions";
            buttonChampions.Click += ButtonChampions_Click;

            formChampions.FormClosing += FormChampions_FormClosing;
            formChampions.Load += FormChampions_Load;
            formChampions.Text = "Champions Pick Ban and Aram bench priorities";
            formChampions.Icon = Properties.Resources.LOL_Client_TOOL;


            summonerStatus.Add(new ComboBoxIdName { Id = 0, Name = "online".ToUpper() });
            summonerStatus.Add(new ComboBoxIdName { Id = 1, Name = "away".ToUpper() });
            summonerStatus.Add(new ComboBoxIdName { Id = 2, Name = "mobile".ToUpper() });
            summonerStatus.Add(new ComboBoxIdName { Id = 3, Name = "offline".ToUpper() });
            int len = 0;
            foreach (ComboBoxIdName cb in summonerStatus)
            {
                if (cb.Name.Length > len)
                {
                    len = cb.Name.Length;
                }
            }
            comboBoxSummonerStatus.Width = len * 10;
            comboBoxSummonerStatus.DataSource = summonerStatus;
            comboBoxSummonerStatus.DisplayMember = "Name";
            comboBoxSummonerStatus.ValueMember = "Id";
            comboBoxSummonerStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxSummonerStatus.SelectedIndexChanged += ComboBoxSummonerStatus_SelectedIndexChanged;
            buttonRune.Text = "RUNES";
            buttonRune.Location = new Point(labelSummonerDisplayName.Location.X + labelSummonerDisplayName.Width, labelSummonerDisplayName.Location.Y);
            comboBoxSummonerStatus.Location = new Point(buttonRune.Location.X + buttonRune.Width + 5, labelSummonerDisplayName.Location.Y + 1);

            
            checkBoxAutoAccept.Text = "Auto accept";
            checkBoxAutoAccept.TextAlign = ContentAlignment.MiddleLeft;
            checkBoxAutoAccept.Location = new Point(comboBoxSummonerStatus.Location.X + comboBoxSummonerStatus.Width + 5, comboBoxSummonerStatus.Location.Y);
            checkBoxAutoAccept.Click += CheckBoxAutoAccept_Click;
            checkBoxAutoPick.Text = "Auto pick";
            checkBoxAutoPick.TextAlign = ContentAlignment.MiddleLeft;
            checkBoxAutoPick.Location = new Point(buttonChampions.Location.X, buttonChampions.Location.Y + buttonChampions.Height);
            checkBoxAutoPick.Click += checkBoxAutoPick_Click;
            checkBoxAutoPrePick.Text = "Auto pre-pick";
            checkBoxAutoPrePick.TextAlign = ContentAlignment.MiddleLeft;
            checkBoxAutoPrePick.Location = new Point(buttonChampions.Location.X, checkBoxAutoPick.Location.Y + checkBoxAutoPick.Height);
            checkBoxAutoPrePick.Click += CheckBoxAutoPrePick_Click;
            checkBoxAutoBan.Text = "Auto ban";
            checkBoxAutoBan.TextAlign = ContentAlignment.MiddleLeft;
            checkBoxAutoBan.Location = new Point(buttonChampions.Location.X, checkBoxAutoPrePick.Location.Y + checkBoxAutoPrePick.Height);
            checkBoxAutoBan.Click += checkBoxAutoBan_Click;
            checkBoxAutoAramBenchSwap.Text = "Auto ARAM bench swap";
            checkBoxAutoAramBenchSwap.TextAlign = ContentAlignment.MiddleLeft;
            checkBoxAutoAramBenchSwap.Location = new Point(buttonChampions.Location.X, checkBoxAutoBan.Location.Y + checkBoxAutoBan.Height);
            checkBoxAutoAramBenchSwap.Click += CheckBoxAutoAramBenchSwap_Click;
            checkBoxAutoAramBenchSwap.AutoSize = true;
            checkBoxAutoQPlayAgain.Text = "Re Q/Play";
            checkBoxAutoQPlayAgain.TextAlign = ContentAlignment.MiddleLeft;
            checkBoxAutoQPlayAgain.Location = new Point(comboBoxSummonerStatus.Location.X + comboBoxSummonerStatus.Width + 5, comboBoxSummonerStatus.Location.Y + checkBoxAutoAccept.Height);
            checkBoxAutoQPlayAgain.Click += CheckBoxAutoQPlayAgain_Click;
            checkBoxAutoHonor.Text = "Auto honor";
            checkBoxAutoHonor.TextAlign = ContentAlignment.MiddleLeft;
            checkBoxAutoHonor.Location = new Point(comboBoxSummonerStatus.Location.X + comboBoxSummonerStatus.Width + 5, checkBoxAutoQPlayAgain.Location.Y + checkBoxAutoQPlayAgain.Height);
            checkBoxAutoHonor.Click += CheckBoxAutoHonor_Click;
            checkBoxAutoPosition.Text = "Auto role";
            checkBoxAutoPosition.TextAlign = ContentAlignment.MiddleLeft;
            checkBoxAutoPosition.Location = new Point(checkBoxAutoAccept.Location.X + checkBoxAutoAccept.Width + 5, comboBoxSummonerStatus.Location.Y);
            checkBoxAutoPosition.Width = 68;
            checkBoxAutoPosition.Click += CheckBoxAutoPosition_Click1;
            comboBoxAutoPosition1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxAutoPosition2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxAutoPosition1.Width = 70;
            comboBoxAutoPosition2.Width = 70;
            foreach (string str in postions) { comboBoxAutoPosition1.Items.Add(str); }
            foreach (string str in postions) { comboBoxAutoPosition2.Items.Add(str); }
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
            comboBoxAutoPosition1.SelectedIndexChanged += ComboBoxAutoPosition1_SelectedIndexChanged;
            comboBoxAutoPosition2.SelectedIndexChanged += ComboBoxAutoPosition2_SelectedIndexChanged;
            comboBoxAutoPosition1.Location = new Point(checkBoxAutoPosition.Location.X + checkBoxAutoPosition.Width + 5, checkBoxAutoPosition.Location.Y);
            comboBoxAutoPosition2.Location = new Point(comboBoxAutoPosition1.Location.X + comboBoxAutoPosition1.Width + 5, checkBoxAutoPosition.Location.Y);
            checkBoxAutoMessage.Text = "Auto message";
            checkBoxAutoMessage.Location = new Point(checkBoxAutoPosition.Location.X, checkBoxAutoPosition.Location.Y + checkBoxAutoPosition.Height);
            checkBoxAutoMessage.Click += CheckBoxAutoMessage_Click;
            checkBoxAutoReroll.Text = "Auto reroll";
            checkBoxAutoReroll.Location = new Point(checkBoxAutoMessage.Location.X, checkBoxAutoMessage.Location.Y + checkBoxAutoMessage.Height);
            checkBoxAutoReroll.Click += CheckBoxAutoReroll_Click;
            checkBoxAutoSkin.Text = "Auto skin";
            checkBoxAutoSkin.Location = new Point(checkBoxAutoReroll.Location.X, checkBoxAutoReroll.Location.Y + checkBoxAutoReroll.Height);
            checkBoxAutoSkin.Click += CheckBoxAutoSkin_Click;
            if (textBoxConversationMessage.Text == "")
            {
                textBoxConversationMessage.Text = "Hello everyone";
            }
            textBoxConversationMessage.Location = new Point(checkBoxAutoMessage.Location.X + checkBoxAutoMessage.Width, checkBoxAutoMessage.Location.Y);
            textBoxConversationMessage.TextChanged += TextBoxConversationMessage_TextChanged;
            buttonRune.Click += buttonRune_Click;
            tooltip.AutoPopDelay = 32767;
            tooltip.UseAnimation = false;
            tooltip.UseFading = false;
            tooltip.InitialDelay = 0;
            tooltip.ReshowDelay = 0;
            tooltip.ShowAlways = true;
            tooltip.ToolTipIcon = ToolTipIcon.Info;
            tooltip.SetToolTip(labelSummonerDisplayName, "Copy to clipboard");
            summonerIcon = getIcon(currentSummoner.profileIconId);
            summonerIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            int iconSize = 90;
            summonerIcon.Size = new Size(iconSize, iconSize);
            summonerIcon.Location = new Point(labelSummonerDisplayName.Location.X, labelSummonerDisplayName.Location.Y + labelSummonerDisplayName.Height);
            tooltip.SetToolTip(summonerIcon, summonerIcon.Name);
            verifyOwnedIcons.Text = "Only show owned icons";
            verifyOwnedIcons.Checked = true;
            verifyOwnedIcons.AutoSize = true;
            verifyOwnedIcons.Location = new Point(summonerIcon.Location.X, summonerIcon.Location.Y + iconSize);
            FormSummonerIcon.SizeChanged += FormSummonerIcon_SizeChanged;
            apiEnpoint.Location = new Point(summonerIcon.Location.X, summonerIcon.Location.Y + iconSize);
            apiEnpoint.Width = 300;
            apiEndpointCall.Height = apiEndpointCall.Height - 1;
            apiEndpointCall.Location = new Point(summonerIcon.Location.X + apiEnpoint.Width, summonerIcon.Location.Y + iconSize - 1);
            apiEndpointCall.Text = "REQUEST";
            apiEndpointCall.Click += ApiEndpointCall_Click;
            apiRequestType.Location = new Point(summonerIcon.Location.X + apiEnpoint.Width + apiEndpointCall.Width, summonerIcon.Location.Y + iconSize);
            apiRequestJson.Multiline = true;
            apiRequestJson.Location = new Point(summonerIcon.Location.X, apiEnpoint.Location.Y + apiEnpoint.Height + 10);
            apiRequestJson.Size = new Size(apiEnpoint.Width + apiEndpointCall.Width + apiRequestType.Width, 200);
            apiEndPointResponse.Multiline = true;
            apiEndPointResponse.Location = new Point(summonerIcon.Location.X + apiEnpoint.Width + apiEndpointCall.Width + apiRequestType.Width + 5, summonerIcon.Location.Y + iconSize);
            apiEndPointResponse.Size = new Size(300, 230);
            if (formSetupFaild)
            {
                apiEndpointCall.Enabled = false;
                buttonRune.Enabled = false;
                comboBoxSummonerStatus.Enabled = false;
            }
            else
            {
                apiEndpointCall.Enabled = true;
                buttonRune.Enabled = true;
                comboBoxSummonerStatus.Enabled = true;
            }
        }

        private static void CheckBoxAutoAramBenchSwap_Click(object sender, EventArgs e)
        {
            configData.autoAramBenchSwap = checkBoxAutoAramBenchSwap.Checked;
            configSaveAsync();
        }

        private static void CheckBoxAutoPrePick_Click(object sender, EventArgs e)
        {
            configData.autoPrePick = checkBoxAutoPrePick.Checked;
            configSaveAsync();
        }

        public async static void FormChampions_Load(object sender, EventArgs e)
        {
            formChampions.Visible = false;
            Font LargeFont = new Font("Arial", 12, FontStyle.Bold);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            formChampions.Width = 450;
            formChampions.Height = resolution.Height / 2;
            formChampions.AutoScroll = true;
            int x = 20, y = 5, loop = 1, index = 0;
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
            foreach(string text in colText)
            {
                colNames.Add(new Label());
                colNames[index].Text = text;
                colNames[index].Font = LargeFont;
                if (text == "CHAMPION")
                {
                    x = 10;
                    colNames[index].Location = new Point(x, y);
                    x = 20;
                }
                else
                {
                    colNames[index].Location = new Point(x, y);
                }
                colNames[index].AutoSize = true;
                formChampions.Controls.Add(colNames[index]);
                formChampions.Controls.Add(colNames[index]);
                x = x + colNames[index].Width + 12;
                index++;
            }

            y = 30;
            x = 10;
            index = 0;
            foreach (champion champ in leagueOfLegendsChampions)
            {
                labelChampions.Add(new Label());
                labelChampions[index].Text = champ.name;
                labelChampions[index].Font = LargeFont;
                labelChampions[index].Location = new Point(x, y * loop);
                labelChampions[index].Name = champ.key;
                labelChampions[index].Width = 110;
                formChampions.Controls.Add(labelChampions[index]);

                pick.Add(new NumericUpDown());
                pick[index].Location = new Point(labelChampions[index].Location.X + labelChampions[index].Width + 10, labelChampions[index].Location.Y);
                pick[index].Name = champ.key;
                pick[index].Value = 0;
                pick[index].Minimum = 0;
                pick[index].Maximum = 200;
                pick[index].Increment = 1;
                pick[index].Width = 50;
                pick[index].MouseWheel += FormChampions_MouseWheel;
                
                formChampions.Controls.Add(pick[index]);

                ban.Add(new NumericUpDown());
                ban[index].Location = new Point(pick[index].Location.X + pick[index].Width + 10, pick[index].Location.Y);
                ban[index].Name = champ.key;
                ban[index].Value = 0;
                ban[index].Minimum = 0;
                ban[index].Maximum = 200;
                ban[index].Increment = 1;
                ban[index].Width = 50;
                ban[index].MouseWheel += FormChampions_MouseWheel;

                formChampions.Controls.Add(ban[index]);
                
                aram.Add(new NumericUpDown());
                aram[index].Location = new Point(ban[index].Location.X + ban[index].Width + 10, ban[index].Location.Y);
                aram[index].Name = champ.key;
                aram[index].Value = 0;
                aram[index].Minimum = 0;
                aram[index].Maximum = 200;
                aram[index].Increment = 1;
                aram[index].Width = 50;
                aram[index].MouseWheel += FormChampions_MouseWheel;

                formChampions.Controls.Add(aram[index]);

                if(configData.championPickList != null)
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
                    if(found == 0)
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
                    if(configData.championPickList == null)
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

                pick[index].ValueChanged += Form1_PickValueChanged;
                ban[index].ValueChanged += Form1_BanValueChanged;
                aram[index].ValueChanged += Form1_AramValueChanged;

                loop++;
                index++;
            }
            formChampions.Visible = true;
        }

        private static void FormChampions_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }

        private static void Form1_PickValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            for (int i = 0; i < configData.championPickList.Count; i++)
            {
                if (configData.championPickList[i].Id.ToString() == numericUpDown.Name)
                {
                    configData.championPickList[i].Pick = Convert.ToInt32(numericUpDown.Value);
                }
            }
            configSaveAsync();
        }

        private static void Form1_BanValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            for (int i = 0; i < configData.championPickList.Count; i++)
            {
                if (configData.championPickList[i].Id.ToString() == numericUpDown.Name)
                {
                    configData.championPickList[i].Ban = Convert.ToInt32(numericUpDown.Value);
                }
            }
            configSaveAsync();
        }

        private static void Form1_AramValueChanged(object sender, EventArgs e)
        {
            NumericUpDown numericUpDown = (NumericUpDown)sender;
            for (int i = 0; i < configData.championPickList.Count; i++)
            {
                if (configData.championPickList[i].Id.ToString() == numericUpDown.Name)
                {
                    configData.championPickList[i].Aram = Convert.ToInt32(numericUpDown.Value);
                }
            }
            configSaveAsync();
        }

        private static void TextBoxConversationMessage_TextChanged(object sender, EventArgs e)
        {
            configData.autoMessageText = textBoxConversationMessage.Text;
            configSaveAsync();
        }

        private static void CheckBoxAutoSkin_Click(object sender, EventArgs e)
        {
            configData.autoSkin = checkBoxAutoSkin.Checked;
            configSaveAsync();
        }

        private static void CheckBoxAutoReroll_Click(object sender, EventArgs e)
        {
            configData.autoReroll = checkBoxAutoReroll.Checked;
            configSaveAsync();
        }

        private static void CheckBoxAutoMessage_Click(object sender, EventArgs e)
        {
            configData.autoMessage = checkBoxAutoMessage.Checked;
            configSaveAsync();
        }

        private static void CheckBoxAutoPosition_Click1(object sender, EventArgs e)
        {
            configData.autoRole = checkBoxAutoPosition.Checked;
            configSaveAsync();
        }

        private static void CheckBoxAutoHonor_Click(object sender, EventArgs e)
        {
            configData.autoHonor = checkBoxAutoHonor.Checked;
            configSaveAsync();
        }

        private static void CheckBoxAutoQPlayAgain_Click(object sender, EventArgs e)
        {
            configData.autoPlayAgain = checkBoxAutoQPlayAgain.Checked;
            configSaveAsync();
        }

        private static void checkBoxAutoPick_Click(object sender, EventArgs e)
        {
            configData.autoPick = checkBoxAutoPick.Checked;
            configSaveAsync();
        }

        private static void checkBoxAutoBan_Click(object sender, EventArgs e)
        {
            configData.autoBan = checkBoxAutoBan.Checked;
            configSaveAsync();
        }

        private static void CheckBoxAutoAccept_Click(object sender, EventArgs e)
        {
            configData.autoAccept = checkBoxAutoAccept.Checked;
            configSaveAsync();
        }

        private static void ComboBoxAutoPosition1_SelectedIndexChanged(object sender, EventArgs e)
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

        private static void ComboBoxAutoPosition2_SelectedIndexChanged(object sender, EventArgs e)
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

        private static void CheckBoxAutoPosition_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }


        private static void ComboBoxSummonerStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ComboBoxIdName combo in summonerStatus)
            {
                if (comboBoxSummonerStatus.SelectedIndex == combo.Id)
                {
                    LCURequest("/lol-chat/v1/me", "PUT", "{\"availability\": \"" + combo.Name.ToLower() + "\"}");
                }
            }
        }

        private static void FormSummonerRunes_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                FormSummonerRunes.Hide();
            }
        }

        private static void FormChampions_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                formChampions.Hide();
            }        
        }

        private static void FormSummonerIcon_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                FormSummonerIcon.Hide();
            }
        }

        private static async void ButtonChampions_Click(object sender, EventArgs e)
        {
            formChampions.Show();
            formChampions.Focus();
        }

        private static async void buttonRune_Click(object sender, EventArgs e)
        {
            if (formSummonerRuneIsSetup == false)
            {
                var summonerTempRunes = JArray.Parse(LCURequest("/lol-perks/v1/pages", "GET").Value);
                currentSummonerRunes.RunePages = new Dictionary<int, RunePage>();
                foreach (var page in summonerTempRunes)
                {
                    RunePage tempRunePage = new RunePage();
                    tempRunePage.current = Convert.ToBoolean(page["current"]);
                    tempRunePage.id = Convert.ToInt32(page["id"]);
                    tempRunePage.isActive = Convert.ToBoolean(page["isActive"]);
                    tempRunePage.isDeletable = Convert.ToBoolean(page["isDeletable"]);
                    tempRunePage.isEditable = Convert.ToBoolean(page["isEditable"]);
                    tempRunePage.isValid = Convert.ToBoolean(page["isValide"]);
                    tempRunePage.lastModified = Convert.ToDouble(page["lastModified"]);
                    tempRunePage.name = (string)page["name"];
                    tempRunePage.order = (int)page["order"];
                    tempRunePage.primaryStyleId = (int)page["primaryStyleId"];
                    tempRunePage.subStyleId = (int)page["subStyleId"];
                    tempRunePage.selectedPerkIds = new Dictionary<int, int>();
                    int i = 0;
                    foreach (var truc in page["selectedPerkIds"])
                    {
                        tempRunePage.selectedPerkIds.Add(i, (int)truc);
                        i++;
                    }
                    currentSummonerRunes.RunePages.Add(Convert.ToInt32(page["id"]), tempRunePage);
                }

                List<ComboBoxIdName> pageNames = new List<ComboBoxIdName>();
                foreach (RunePage page in currentSummonerRunes.RunePages.Values)
                {
                    ComboBoxIdName temp = new ComboBoxIdName();
                    temp.Id = page.id;
                    temp.Name = page.name;
                    pageNames.Add(temp);
                }
                comboBoxRunesPages.DataSource = pageNames;
                comboBoxRunesPages.DisplayMember = "Name";
                comboBoxRunesPages.ValueMember = "Id";
                int width = 0;
                foreach (ComboBoxIdName cb in pageNames)
                {
                    if (cb.Name.Length > width)
                    {
                        width = cb.Name.Length;
                    }
                }
                comboBoxRunesPages.Width = width * 7;
                comboBoxRunesPages.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBoxRunesPages.SelectedIndexChanged += ComboBoxRunesPages_SelectedIndexChanged;
                FormSummonerRunes.Controls.Add(comboBoxRunesPages);
                FormSummonerRunes.Show();//must be before ComboBoxRunesPages_SelectedIndexChanged(comboBoxRunesPages, null);
                while (comboBoxRunesPages.Enabled != true)
                {
                    await Task.Delay(25);
                }
                ComboBoxRunesPages_SelectedIndexChanged(comboBoxRunesPages, null);
            }
            else
            {
                FormSummonerRunes.Show();
            }
            formSummonerRuneIsSetup = true;
        }

        private static void ComboBoxRunesPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (PictureBox pic in FormSummonerRunes.Controls.OfType<PictureBox>())
            {
                FormSummonerRunes.Controls.Remove(pic);
            }
            string path = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\LOL_Client_TOOL\\runes\\";
            List<PictureBox> runesSubImage = new List<PictureBox>();
            List<PictureBox> runeMainImage = new List<PictureBox>();
            foreach (var jsp in leagueOfLegendsRunes)
            {
                PictureBox temp1 = new PictureBox();
                temp1.Image = Image.FromFile(path + jsp.icon.Split(chare, StringSplitOptions.None)[jsp.icon.Split(chare, StringSplitOptions.None).Length - 1]);
                temp1.Name = jsp.id.ToString();

                temp1.BackColor = Color.Transparent;
                temp1.SizeMode = PictureBoxSizeMode.StretchImage;
                temp1.Size = new Size(45, 45);
                runeMainImage.Add(temp1);
                foreach (var fiou in jsp.slots.Values)
                {
                    foreach (var pouf in fiou.Values)
                    {
                        PictureBox temp2 = new PictureBox();
                        temp2.Image = Image.FromFile(path + pouf.icon.Split(chare, StringSplitOptions.None)[pouf.icon.Split(chare, StringSplitOptions.None).Length - 1]);
                        temp2.Name = pouf.id.ToString();
                        temp2.BackColor = Color.Transparent;
                        temp2.SizeMode = PictureBoxSizeMode.StretchImage;
                        temp2.Size = new Size(45, 45);
                        runesSubImage.Add(temp2);
                    }
                }
            }
            int x = comboBoxRunesPages.Location.X + 20, y = 0;
            runeMainImage = runeMainImage.OrderBy(o => o.Name).ToList();
            foreach (PictureBox img in runeMainImage)//Display Main runes img
            {
                y = img.Height;
                img.Location = new Point(x, y);
                tooltip.SetToolTip(img, "X" + img.Location.X.ToString() + "Y" + img.Location.Y.ToString());
                FormSummonerRunes.Controls.Add(img);
                x += img.Width + 10;
                img.Click += ImgMainRune_Click;
            }
            string primaryStyleId = "";
            foreach (RunePage dd in currentSummonerRunes.RunePages.Values)
            {
                if (dd.id.ToString() == comboBoxRunesPages.SelectedValue.ToString())
                {
                    foreach (PictureBox pic in FormSummonerRunes.Controls.OfType<PictureBox>())
                    {
                        if (pic.Name == dd.primaryStyleId.ToString())
                        {
                            primaryStyleId = dd.primaryStyleId.ToString();
                            ImgMainRune_Click(pic, null);
                        }
                    }
                }
            }
            x += 100;
            int count = 0;
            List<string> dede = new List<string> { "8000", "8100", "8200", "8300", "8400" };
            List<string> dada = new List<string> { "8000", "8100", "8200", "8300", "8400" };
            int threshold = x - 50;
            //MessageBox.Show(threshold.ToString());
            foreach (PictureBox img in FormSummonerRunes.Controls.OfType<PictureBox>())
            {
                if (img.Location.X > threshold)
                {
                    img.Visible = false;
                    img.Refresh();
                }
            }
            foreach (PictureBox img in runeMainImage)//Display Sub runes img
            {
                if (img.Name != primaryStyleId && dede.Contains(img.Name))
                {
                    dede.Remove(img.Name);
                    PictureBox subImg = new PictureBox();
                    subImg.Image = img.Image;
                    subImg.Name = img.Name;
                    subImg.Size = new Size(45, 45);
                    y = subImg.Height;
                    tooltip.SetToolTip(subImg, subImg.Name);
                    subImg.SizeMode = PictureBoxSizeMode.StretchImage;
                    subImg.Location = new Point(x, y);
                    subImg.Click += ImgSubRune_Click;
                    subImg.Refresh();
                    FormSummonerRunes.Controls.Add(subImg);
                    x += subImg.Width + 10;
                }
                else
                {
                    PictureBox subImg = new PictureBox();
                    subImg.Image = img.Image;
                    subImg.Name = img.Name;
                    subImg.Size = new Size(45, 45);
                    y = subImg.Height;
                    tooltip.SetToolTip(subImg, subImg.Name);
                    subImg.SizeMode = PictureBoxSizeMode.StretchImage;
                    subImg.Location = new Point(x, y);
                    subImg.Click += ImgSubRune_Click;
                    subImg.Visible = false;
                    subImg.Refresh();
                    FormSummonerRunes.Controls.Add(subImg);
                    //x += subImg.Width + 10;
                }
            }
            foreach (RunePage dd in currentSummonerRunes.RunePages.Values)
            {
                if (dd.id.ToString() == comboBoxRunesPages.SelectedValue.ToString())
                {
                    foreach (PictureBox pic in FormSummonerRunes.Controls.OfType<PictureBox>())
                    {
                        if (pic.Name == dd.subStyleId.ToString())
                        {
                            ImgSubRune_Click(pic, null);
                        }
                    }
                }
            }
        }

        private static void ImgSubRune_Click(object sender, EventArgs e)
        {
            Dictionary<string, string[]> runesSetups = new Dictionary<string, string[]>();
            runesSetups.Add("8000", new string[] { "8005 8008 8021 8010", "9101 9111 8009", "9104 9105 9103", "8014 8017 8299" });
            runesSetups.Add("8100", new string[] { "8112 8124 8128 9923", "8126 8139 8143", "8136 8120 8138", "8135 8134 8105 8106" });
            runesSetups.Add("8200", new string[] { "8214 8229 8230", "8224 8226 8275", "8210 8234 8233", "8237 8232 8236" });
            runesSetups.Add("8300", new string[] { "8351 8360 8358", "8306 8304 8313", "8321 8316 8345", "8347 8410 8352" });
            runesSetups.Add("8400", new string[] { "8437 8439 8465", "8446 8463 8401", "8429 8444 8473", "8451 8453 8242" });

        }

        private static void ImgMainRune_Click(object sender, EventArgs e)
        {
            Dictionary<string, string[]> runesSetups = new Dictionary<string, string[]>();
            runesSetups.Add("8000", new string[] { "8005 8008 8021 8010", "9101 9111 8009", "9104 9105 9103", "8014 8017 8299" });
            runesSetups.Add("8100", new string[] { "8112 8124 8128 9923", "8126 8139 8143", "8136 8120 8138", "8135 8134 8105 8106" });
            runesSetups.Add("8200", new string[] { "8214 8229 8230", "8224 8226 8275", "8210 8234 8233", "8237 8232 8236" });
            runesSetups.Add("8300", new string[] { "8351 8360 8358", "8306 8304 8313", "8321 8316 8345", "8347 8410 8352" });
            runesSetups.Add("8400", new string[] { "8437 8439 8465", "8446 8463 8401", "8429 8444 8473", "8451 8453 8242" });
            PictureBox uneImage = (PictureBox)sender;
            List<string> dede = new List<string> { "8000", "8100", "8200", "8300", "8400" };
            List<PictureBox> subRune = new List<PictureBox>();
            foreach (leagueRune rune in leagueOfLegendsRunes)
            {
                if (rune.id.ToString() == uneImage.Name)
                {
                    foreach (var jj in rune.slots.Values)
                    {
                        foreach (var ii in jj.Values)
                        {
                            PictureBox toto = new PictureBox();
                            toto.Image = Image.FromFile(pathRunes + ii.icon.Split(chare, StringSplitOptions.None)[ii.icon.Split(chare, StringSplitOptions.None).Length - 1]);
                            toto.Name = ii.name;
                            dede.Add(ii.name);
                            string tt = ii.longDesc.Replace(".", "\n").Replace("<br>", "\n").Replace("\n\n", "\n");
                            tooltip.SetToolTip(toto, Regex.Replace(tt, "<.*?>", ""));
                            toto.SizeMode = PictureBoxSizeMode.StretchImage;
                            toto.Size = new Size(40, 40);
                            subRune.Add(toto);
                        }
                    }
                }
            }
            foreach (PictureBox img in FormSummonerRunes.Controls.OfType<PictureBox>())
            {
                if (!dede.Contains(img.Name))
                {
                    img.Visible = false;
                    img.Refresh();
                }
            }
            Dictionary<string, int[]> popo = new Dictionary<string, int[]>();
            popo.Add("8000", new int[] { 4, 7, 10 });
            popo.Add("8100", new int[] { 4, 7, 10 });
            popo.Add("8200", new int[] { 3, 6, 9 });
            popo.Add("8300", new int[] { 3, 6, 9 });
            popo.Add("8400", new int[] { 3, 6, 9 });
            foreach (var roo in popo)//Display primaryStyleId image of runes
            {
                if (roo.Key == uneImage.Name)
                {
                    int i = 1;
                    int y = 0;
                    int j = 1;
                    foreach (PictureBox pic in subRune)
                    {
                        if (roo.Value.Contains(y))
                        {
                            i++;
                            j = 1;
                        }
                        pic.Location = new Point(50 * j, 50 + 50 * i);
                        FormSummonerRunes.Controls.Add(pic);
                        j++;
                        y++;
                    }
                }
            }
            int picnb = 0;
            List<string> didi = new List<string> { "8000", "8100", "8200", "8300", "8400" };
            foreach (PictureBox pic in FormSummonerRunes.Controls.OfType<PictureBox>())
            {
                if (pic.Location.X > 240 && pic.Location.Y <= 50)
                {
                    if (pic.Name != uneImage.Name)
                    {
                        if (posSubMainRune.Count == 5 && didi.Contains(pic.Name))
                        {
                            pic.Visible = true;
                            didi.Remove(pic.Name);
                            pic.Location = posSubMainRune[picnb];
                            picnb++;
                        }
                    }
                    else
                    {
                        pic.Visible = false;
                    }
                }
            }
            while (posSubMainRune.Count != 5)
            {
                posSubMainRune.Add(posSubMainRune.Count, new Point(395 + 55 * posSubMainRune.Count, 45));
            }
            foreach (PictureBox pic in FormSummonerRunes.Controls.OfType<PictureBox>())
            {
                if (pic.Location.X <= 240 && pic.Location.Y <= 45)
                {
                    PictureBox sub = new PictureBox();
                }
            }

        }

        private static void ApiEndpointCall_Click(object sender, EventArgs e)
        {
            if (apiEnpoint.Text.Contains("{summonerId}"))
            {
                apiEnpoint.Text = apiEnpoint.Text.Replace("{summonerId}", currentSummoner.summonerId);
            }
            if (apiEnpoint.Text.Contains("{puuid}"))
            {
                apiEnpoint.Text = apiEnpoint.Text.Replace("{puuid}", currentSummoner.puuid);
            }
            if (apiEnpoint.Text.Contains("{summonerIconId}"))
            {
                apiEnpoint.Text = apiEnpoint.Text.Replace("{summonerIconId}", currentSummoner.profileIconId);
            }
            KeyValuePair<string, string> text = LCURequest(apiEnpoint.Text, apiRequestType.Text, apiRequestJson.Text);
            try
            {
                apiEndPointResponse.Text = text.Key + "\n" + text.Value;
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee);
            }
        }

        private static void LabelSummonerDisplayName_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(labelSummonerDisplayName.Text);
        }

        private static void FormSummonerIcon_SizeChanged(object sender, EventArgs e)
        {
            if (verifyOwnedIcons.Checked == true)
            {
                int width = FormSummonerIcon.Width;
                int x = 0, y = 0;
                if (allSummonerIcons.Count >= 1)
                {
                    List<PictureBox> atResizeBOx = allSummonerIcons.ToList<PictureBox>();
                    foreach (PictureBox icon in atResizeBOx)
                    {
                        icon.Location = new System.Drawing.Point(x, y);
                        x += icon.Width;
                        if (x + icon.Width >= width)
                        {
                            x = 0;
                            y += icon.Height;
                        }
                        FormSummonerIcon.Controls.Add(icon);
                    }
                }
            }
            else if (processingAllIcon == false)
            {
                int width = FormSummonerIcon.Width;
                int x = 0, y = 0;
                List<PictureBox> atResizeBOx = allSummonerIcons;
                foreach (PictureBox icon in atResizeBOx)
                {
                    icon.Location = new System.Drawing.Point(x, y);
                    x += icon.Width;
                    if (x + icon.Width >= width)
                    {
                        x = 0;
                        y += icon.Height;
                    }
                    FormSummonerIcon.Controls.Add(icon);
                }
            }
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

        private async void setupLCU()//for api usage
        {
            bool setupFaild = true;
            bool success = false;
            while (setupFaild)
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
                    string laData = "PASSWORD: " + lesArguments["--remoting-auth-token"] + "\r\n" + "PORT: " + lesArguments["--app-port"] + "\r\n" + "AUTH: " + autorization;
                    port = lesArguments["--app-port"];
                    lesArguments.Clear();
                    setupFaild = false;
                }
                catch (Exception ex)
                {
                    //try connection to riot client
                    //MessageBox.Show(ex.Message);
                    //ProcessStartInfo usbDevicesInfo = new ProcessStartInfo("wmic", "PROCESS WHERE name='RiotClientUx.exe' GET commandline");
                    //usbDevicesInfo.RedirectStandardOutput = true;
                    //usbDevicesInfo.UseShellExecute = false;
                    //usbDevicesInfo.CreateNoWindow = true;
                    //Process process = new Process();
                    //process.StartInfo = usbDevicesInfo;
                    //process.Start();
                    //process.WaitForExit();

                    //string result = process.StandardOutput.ReadToEnd();
                    //string[] lesArgumentsTemp = result.Split(' ');
                    //foreach (string argument in lesArgumentsTemp)
                    //{
                    //    string arg = argument.Replace("\"", "");
                    //    if (arg.Contains("="))
                    //    {
                    //        string[] kv = arg.Split(Convert.ToChar("="));
                    //        lesArguments.Add(kv[0], kv[1]);
                    //    }
                    //}
                    //byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes("riot:" + lesArguments["--remoting-auth-token"]);
                    //autorizationRiotClient = Convert.ToBase64String(data);
                    //string laData = "PASSWORD: " + lesArguments["--remoting-auth-token"] + "\r\n" + "PORT: " + lesArguments["--app-port"] + "\r\n" + "AUTH: " + autorizationRiotClient;
                    //portRiotClient = lesArguments["--app-port"];
                    //lesArguments.Clear();


                    //string json = "{'username': " +  + "}";
                    //RiotLCURequest("https://127.0.0.1:" + portRiotClient + "/rso-auth/v1/session/credentials", "PUT", );
                }
            }

        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            setupLCU();
            //string msg = (LCURequest("/lol-summoner/v1/current-summoner", "GET", "").Value);
            //Process[] localByName = Process.GetProcessesByName("LeagueClientUx");
            //if (localByName.Count() == 0 || msg.Contains("(404)"))
            //{
            //    var formLogin = new Form();
            //    TextBox TextBoxId = new TextBox();
            //    Label LabelId = new Label();
            //    LabelId.Text = "Login id :";
            //    Label LabelLogin = new Label();
            //    LabelLogin.Text = "Password :";
            //    Button ButtonLogin = new Button();
            //    ButtonLogin.Text = "LOGIN";
            //    LabelId.Location = new Point(5, 5);
            //    formLogin.Controls.Add(LabelLogin);
            //    formLogin.Show();
            //}


            setupForm();
            //if (formSetupFaild)
            //{
            //    //ID
            //    Label labelId = new Label();
            //    labelId.Text = "Login id :";
            //    labelId.Location = new Point(5, 5);
            //    textBoxId.Location = new Point(labelId.Location.X + labelId.Width, labelId.Location.Y);
            //    //PASSWORD
            //    Label labelPassword = new Label();
            //    labelPassword.Text = "Password :";
            //    labelPassword.Location = new Point(5, 5 + labelId.Height);
            //    textBoxPassword.Location = new Point(labelPassword.Location.X + labelPassword.Width, labelPassword.Location.Y);
            //    //LOGIN BUTTON
            //    Button buttonLogin = new Button();
            //    buttonLogin.Text = "LOGIN";
            //    buttonLogin.Width = labelId.Width + textBoxId.Width;
            //    buttonLogin.Location = new Point(5, 5 + labelPassword.Height + buttonLogin.Height);
            //    buttonLogin.Click += ButtonLogin_Click;
            //    //POPULATING FORM
            //    formLogin.Controls.Add(textBoxId);
            //    formLogin.Controls.Add(labelId);
            //    formLogin.Controls.Add(labelPassword);
            //    formLogin.Controls.Add(textBoxPassword);
            //    formLogin.Controls.Add(buttonLogin);
            //    formLogin.Show();
            //    buttonSetupFrom.Click += ButtonSetupFrom_Click;
            //    this.Controls.Add(buttonSetupFrom);
            //}
            //disabled until there is a good way to get summonner owned icons
            //summonerIcon.Click += new EventHandler(summonerIcon_Click);
            this.Controls.Add(buttonChampions);
            this.Controls.Add(summonerIcon);
            //this.Controls.Add(buttonRune);
            this.Controls.Add(comboBoxSummonerStatus);
            this.Controls.Add(checkBoxAutoAccept);
            this.Controls.Add(checkBoxAutoPick);
            this.Controls.Add(checkBoxAutoPrePick);
            this.Controls.Add(checkBoxAutoAramBenchSwap);
            this.Controls.Add(checkBoxAutoBan);
            this.Controls.Add(checkBoxAutoQPlayAgain);
            this.Controls.Add(checkBoxAutoHonor);
            this.Controls.Add(checkBoxAutoPosition);
            this.Controls.Add(comboBoxAutoPosition1);
            this.Controls.Add(comboBoxAutoPosition2);
            this.Controls.Add(checkBoxAutoMessage);
            this.Controls.Add(textBoxConversationMessage);
            this.Controls.Add(checkBoxAutoReroll);
            this.Controls.Add(checkBoxAutoSkin);
            if (debug)
            {
                this.Controls.Add(apiEnpoint);
                this.Controls.Add(apiEndpointCall);
                this.Controls.Add(apiRequestType);
                this.Controls.Add(apiRequestJson);
                this.Controls.Add(apiEndPointResponse);
            }
            else
            {
                this.Height = 180;
            }
            //this.Icon = Properties.Resources.LOL_Client_TOOL;
            //this.ShowInTaskbar = true;
        }

        private void ButtonLogin_Click(object sender, EventArgs e)
        {
            riotCredntials.Add(textBoxId.Text, textBoxPassword.Text);
            string rchText = "{\"username\":\"" + textBoxId.Text + "\",\"password\":\"" + textBoxPassword.Text + "\",\"persistLogin\":false}";
            var resp = RiotLCURequest("/rso-auth/v1/session/credentials", "PUT", rchText);
            if (resp.Value.Contains("auth_failure"))
            {

            }
        }

        private void ButtonSetupFrom_Click(object sender, EventArgs e)
        {
            setupForm();
        }

        private async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            //Keep json for the loop START
            var lolChampSelectV1Session = new KeyValuePair<string, string>();
            if (checkBoxAutoReroll.Checked || checkBoxAutoPick.Checked || checkBoxAutoSkin.Checked || checkBoxAutoBan.Checked)
            {
                lolChampSelectV1Session = LCURequest("/lol-champ-select/v1/session", "GET");
            }
            //Keep json for the loop END
            //auto accept start
            if (checkBoxAutoAccept.Checked)
            {
                LCURequest("/lol-matchmaking/v1/ready-check/accept", "POST", "");
            }
            //auto accept end

            //Honor start
            if (checkBoxAutoHonor.Checked)
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
            if (checkBoxAutoPick.Checked || checkBoxAutoBan.Checked)
            {


                if (lolChampSelectV1Session.Key == "OK")
                {
                    List<int> championPlayable = new List<int>();
                    var ownedChampionsMinimal = JArray.Parse(LCURequest("/lol-champions/v1/owned-champions-minimal", "GET", "").Value);
                    foreach (var champ in ownedChampionsMinimal)
                    {
                        //MessageBox.Show(champ["ownership"]["owned"].ToString());
                        if (champ["active"].ToString() == "True")
                        {
                            if (champ["ownership"]["owned"].ToString() == "True" || champ["ownership"]["freeToPlayReward"].ToString() == "True" || champ["freeToPlay"].ToString() == "True")
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
                                if (isInProgress == "True" && team["type"].ToString().Contains("pick") && checkBoxAutoPick.Checked)
                                {
                                    //get the champ based on prioritie and bans
                                    int maxPrio = 0;
                                    string champId = "";
                                    foreach(championPick tempPick in configData.championPickList)
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
                                if (isInProgress == "True" && team["type"].ToString().Contains("ban") && checkBoxAutoBan.Checked)
                                {
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
                                if (checkBoxAutoPrePick.Checked && !team["type"].ToString().Contains("ban"))
                                {
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
                    if (checkBoxAutoAramBenchSwap.Checked)
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
                }
            }
            //auto lock end

            //reQ/play again start
            AutoQPlayAgian();
            //reQ/play again end

            //AUTO POSITION start
            if (checkBoxAutoPosition.Checked)
            {
                string json = "{\"firstPreference\":\"" + position1 + "\",\"secondPreference\":\"" + position2 + "\"}";
                LCURequest("/lol-lobby/v2/lobby/members/localMember/position-preferences", "PUT", json);
            }
            //AUTO POSITION end

            //AUTO MESSAGE START
            if (checkBoxAutoMessage.Checked)
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
            if (checkBoxAutoReroll.Checked)
            {
                if (lolChampSelectV1Session.Key == "OK")
                {
                    JObject json = JObject.Parse(lolChampSelectV1Session.Value);
                    if (json["allowRerolling"].ToString().ToLower() == "true" && Convert.ToInt32(json["rerollsRemaining"].ToString()) != 0)
                    {
                        int rerolls = Convert.ToInt32(json["rerollsRemaining"].ToString());
                        int i = 0;
                        while(i != rerolls)
                        {
                            LCURequest("/lol-champ-select/v1/session/my-selection/reroll", "POST");
                            i++;
                        }
                        
                    }
                }
            }
            //AUTO REROLL END

            //AUTO SKIN START
            if (checkBoxAutoSkin.Checked)
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

        public static void AutoQPlayAgian()
        {
            if (AutoQPlayAgianIntervalle == 0)
            {
                if (checkBoxAutoQPlayAgain.Checked)
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

        private static KeyValuePair<string, string> RiotLCURequest(string url = "", string method = "", string json = "")
        {
            method = method.ToUpper();
            url = "https://127.0.0.1:" + portRiotClient + url.Trim();
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = method;
            httpWebRequest.Headers.Add("Authorization", "Basic " + autorizationRiotClient);
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
                httpWebRequest.Headers.Add("Authorization", "Basic " + autorizationRiotClient);
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
                httpWebRequest.Headers.Add("Authorization", "Basic " + autorizationRiotClient);
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
                return new KeyValuePair<string, string>(httpResponse.StatusCode.ToString(), readStream.ReadToEnd());
            }
            catch (Exception e)
            {
                return new KeyValuePair<string, string>("", e.Message);
            }
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
                MessageBox.Show(e.Message, "DDragonRequest");
                return e.Message;
            }
        }

        private void SetTimer()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
            //aTimer.AutoReset = true;
        }

        public static void getSummoner()
        {
            while (formSetupFaild)
            {
                try
                {
                    JObject summoner = JObject.Parse(LCURequest("/lol-summoner/v1/current-summoner", "GET", "").Value);
                    currentSummoner.accountId = (string)summoner["accountId"];
                    currentSummoner.displayName = (string)summoner["displayName"];
                    currentSummoner.internalName = (string)summoner["internalName"];
                    currentSummoner.nameChangeFlag = (string)summoner["nameChangeFlag"];
                    currentSummoner.percentCompleteForNextLevel = (string)summoner["percentCompleteForNextLevel"];
                    if (currentSummoner.profileIconId != (string)summoner["profileIconId"])
                    {
                        summonerIcon.Image = getIcon((string)summoner["profileIconId"]).Image;
                        summonerIcon.Update();
                        summonerIcon.Refresh();
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
                    formSetupFaild = false;
                }
                catch (Exception ex)
                {
                    formSetupFaild = true;
                }
            }
        }

        public static void getVersion()
        {
            string data = DDragonRequest("https://ddragon.leagueoflegends.com/api/versions.json");
            var LeagueVersion = JArray.Parse(data);
            version = LeagueVersion.First.ToString();
        }

        public static PictureBox getIcon(string iconId)
        {
            PictureBox icon = new PictureBox();
            string path = "C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\LOL_Client_TOOL\\icon\\";
            if (File.Exists(path + iconId + ".png"))
            {
                icon.Image = Image.FromFile(path + iconId + ".png");
            }
            else
            {
                string url = "http://ddragon.leagueoflegends.com/cdn/" + version + "/img/profileicon/" + iconId + ".png";
                icon.LoadAsync(url);
            }
            icon.Name = iconId;
            return icon;
        }

        static async Task<bool> autoAccept()
        {
            while (checkBoxAutoAccept.Checked)
            {
                var result = LCURequest("/lol-matchmaking/v1/ready-check", "GET");
                MessageBox.Show(result.Value, result.Key);
            }
            return true;
        }
    }
}
//TO DO
//GET GAME VERSION https://ddragon.leagueoflegends.com/api/versions.json
//GET ICON http://ddragon.leagueoflegends.com/cdn/11.1.1/img/profileicon/588.png
//MULTI LANGUAGE HANDELING https://ddragon.leagueoflegends.com/cdn/languages.json