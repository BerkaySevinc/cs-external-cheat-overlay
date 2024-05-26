using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Serialization;


// check number pads
// check betweenasarray

// Json Elementte, string "null" sa onu null a çevir

namespace BekoS
{


    #region String Extensions

    public static class StringExtensions
    {

        #region Format

        public static string Format(this string text, params object[] Args)
        {
            return string.Format(text, Args);
        }

        #endregion
        #region Split

        public static string[] Split(this string text)
        {
            return text.Split(new string[] { " " }, StringSplitOptions.None);
        }

        // Overload 1
        public static string[] Split(this string text, string Separator)
        {
            return text.Split(new string[] { Separator }, StringSplitOptions.None);
        }

        #endregion
        #region Index Of Any

        public static int IndexOfAny(this string text, params object[] Values)
        {
            int first = -1;
            foreach (object item in Values)
            {
                string target = item.ToString()!;

                int i = text.IndexOf(target);
                if (i < 0) continue;

                if (first < 0)
                {
                    first = i;
                    continue;
                }

                if (i < first) first = i;
            }

            return first;
        }

        public static int IndexOfAny(this string text, int StartIndex, params object[] Values)
        {
            return text[StartIndex..].IndexOfAny(Values) + StartIndex;
        }

        #endregion

        #region Delete

        public static string Delete(this string text, params string[] Args)
        {
            foreach (string arg in Args)
                text = text.Replace(arg, "");

            return text;
        }

        #endregion
        #region Repeat

        public static string Repeat(this string Text, int Count)
        {
            return new StringBuilder(Text.Length * Count).Insert(0, Text, Count).ToString();
        }

        #endregion
        #region Pad Left

        public static string PadLeft(this string value, int TotalLength, int PaddingString)
        {
            return PaddingString.Repeat(TotalLength - value.Length) + value;
        }


        #endregion
        #region Pad Right

        public static string PadRight(this string value, int TotalLength, int PaddingString)
        {
            return value + PaddingString.Repeat(TotalLength - value.Length);
        }


        #endregion

        #region To Capitalized

        public static string ToCapitalized(this string text)
        {
            return char.ToUpper(text[0]) + text[1..];
        }

        #endregion
        #region To Title

        public static string ToTitle(this string text)
        {
            char[] a = text.ToLower().ToCharArray();

            for (int i = 0; i < a.Length; i++)
                a[i] = i == 0 || a[i - 1] == ' ' ? char.ToUpper(a[i]) : a[i];

            return new string(a);
        }

        #endregion
        #region To Random Case

        public static string ToRandomCase(this string text)
        {
            var result = new StringBuilder();
            var random = new Random();

            for (int i = 0; i < text.Length; i++)
                result.Append(random.Next(2) == 0 ? char.ToUpper(text[i]) : char.ToLower(text[i]));

            return result.ToString();
        }

        #endregion
        #region To Swapped Case

        public static string ToSwappedCase(this string text)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                var letter = text[i];
                bool isUpper = letter == char.ToUpper(letter);
                result.Append(isUpper ? char.ToLower(letter) : char.ToUpper(letter));
            }

            return result.ToString();
        }

        #endregion


        #region Json Element

        public static T? JsonElement<T>(this string Json, string Element)
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
                    if (Result == "") break;
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

                string ArrayXml = "";
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
        #region Between

        public static string Between(this string text, string First, string Last)
        {
            int resBas = text.IndexOf(First) + First.Length;

            int resSon = text.IndexOf(Last, resBas);

            return text[resBas..resSon];

        }

        // Overload 1
        public static string Between(this string text, string First)
        {
            int resBas = text.IndexOf(First) + First.Length;

            int resSon = text.Length;

            return text[resBas..resSon];

        }

        // Overload 2
        public static string Between(this string text, string First, string Last, string StartText)
        {
            int resBas = text.IndexOf(First, text.IndexOf(StartText) + StartText.Length) + First.Length;

            int resSon = text.IndexOf(Last, resBas);

            return text[resBas..resSon];

        }

        // Overload 3
        public static string Between(this string text, string First, string Last, int StartIndex)
        {
            int resBas = text.IndexOf(First, StartIndex) + First.Length;

            int resSon = text.IndexOf(Last, resBas);

            return text[resBas..resSon];

        }

        #endregion
        #region Between As Array

        public static string[] BetweenAsArray(this string text, string First, string Last)
        {
            List<string> list = new List<string>();

            int count = text.Count(First);

            for (int i = 0; i < count; i++)
            {
                int resBas = text.IndexOf(First) + First.Length;
                int resSon = text.IndexOf(Last, resBas);

                if (resSon == -1) break;

                list.Add(text[resBas..resSon]);
            }

            return list.ToArray();
        }

        // Overload 2
        public static string[] BetweenAsArray(this string text, string First, string Last, string StartText)
        {
            List<string> list = new List<string>();

            int count = text.Count(First);
            int searchIndex = text.IndexOf(StartText) + StartText.Length;

            for (int i = 0; i < count; i++)
            {
                int resBas = text.IndexOf(First, searchIndex) + First.Length;
                int resSon = text.IndexOf(Last, resBas);

                if (resSon == -1) break;
                searchIndex = resSon;

                list.Add(text[resBas..resSon]);
            }

            return list.ToArray();
        }

        // Overload 2
        public static string[] BetweenAsArray(this string text, string First, string Last, int StartIndex)
        {
            List<string> list = new List<string>();

            int count = text.Count(First);
            int searchIndex = StartIndex;

            for (int i = 0; i < count; i++)
            {
                int resBas = text.IndexOf(First, searchIndex) + First.Length;
                int resSon = text.IndexOf(Last, resBas);

                if (resSon == -1) break;
                searchIndex = resSon;

                list.Add(text[resBas..resSon]);
            }

            return list.ToArray();
        }

        #endregion


        #region Count

        public static int Count(this string text)
        {
            return text.Split(' ').Length - 1;
        }

        // Overload 1
        public static int Count(this string text, string Word)
        {
            return text.Split(new string[] { Word }, StringSplitOptions.None).Length - 1;
        }

        #endregion


        #region Is Numeric

        public static bool IsNumeric(this string Text)
        {
            return Regex.IsMatch(Text, @"^\d+$");
        }

        #endregion


        #region Is Short
        public static bool IsShort(this string Text)
        {
            return short.TryParse(Text, out _);
        }

        #endregion
        #region Is Int
        public static bool IsInt(this string Text)
        {
            return int.TryParse(Text, out _);
        }

        #endregion
        #region Is Long
        public static bool IsLong(this string Text)
        {
            return long.TryParse(Text, out _);
        }

        #endregion

        #region Is Float
        public static bool IsFloat(this string Text)
        {
            return float.TryParse(Text.Replace(".", ","), out _);
        }

        #endregion
        #region Is Double
        public static bool IsDouble(this string Text)
        {
            return double.TryParse(Text.Replace(".", ","), out _);
        }

        #endregion
        #region Is Decimal
        public static bool IsDecimal(this string Text)
        {
            return decimal.TryParse(Text.Replace(".", ","), out _);
        }

        #endregion

        #region Is Bool

        public static bool IsBool(this string Text)
        {
            return bool.TryParse(Text, out _);
        }

        #endregion


        #region To Int From Roman String

        public static int ToIntFromRomanString(this string value)
        {
            Dictionary<string, int> romanNumerals =
                new Dictionary<string, int>(13)
                {
                { "M", 1000 },
                { "CM", 900 },
                { "D", 500 },
                { "CD", 400 },
                { "C", 100 },
                { "XC", 90 },
                { "L", 50 },
                { "XL", 40 },
                { "X", 10 },
                { "IX", 9 },
                { "V", 5 },
                { "IV", 4 },
                { "I", 1 }
                };

            Regex validRomanNumeral = new Regex(
                "^(?i:(?=[MDCLXVI])((M{0,3})((C[DM])|(D?C{0,3}))"
                + "?((X[LC])|(L?XX{0,2})|L)?((I[VX])|(V?(II{0,2}))|V)?))$",
                RegexOptions.Compiled);

            value = value.ToUpperInvariant().Trim();
            var length = value.Length;

            if ((length == 0) || !validRomanNumeral.IsMatch(value)) return 0;

            var total = 0;
            var i = length;

            while (i > 0)
            {
                var digit = romanNumerals[value[--i].ToString()];
                if (i > 0)
                {
                    var previousDigit = romanNumerals[value[i - 1].ToString()];
                    if (previousDigit < digit)
                    {
                        digit -= previousDigit;
                        i--;
                    }
                }
                total += digit;
            }
            return total;
        }

        #endregion

        #region To Short

        public static short ToShort(this string text)
        {
            return short.TryParse(text, out short result) ? result : default;
        }

        #endregion
        #region To Int

        public static int ToInt(this string text)
        {
            return int.TryParse(text, out int result) ? result : default;
        }

        #endregion
        #region To Long

        public static long ToLong(this string text)
        {
            return long.TryParse(text, out long result) ? result : default;
        }

        #endregion

        #region To Float

        public static float ToFloat(this string text)
        {
            return float.TryParse(text.Replace(".", ","), out float result) ? result : default;
        }
        #endregion
        #region To Double

        public static double ToDouble(this string text)
        {
            return double.TryParse(text.Replace(".", ","), out _) ? double.Parse(text) : default;
        }

        #endregion
        #region To Decimal

        public static decimal ToDecimal(this string text)
        {
            return decimal.TryParse(text.Replace(".", ","), out decimal result) ? result : default;
        }

        #endregion

        #region To Bool

        public static bool ToBool(this string text)
        {
            return bool.TryParse(text, out bool result) ? result : default;
        }

        #endregion

    }

    #endregion


    #region Number Extensions

    public static class NumberExtensions
    {

        #region Floor

        public static int Floor(this float value)
        {
            return (int)Math.Floor(value);
        }

        public static int Floor(this double value)
        {
            return (int)Math.Floor(value);
        }

        public static int Floor(this decimal value)
        {
            return (int)Math.Floor(value);
        }

        #endregion
        #region Ceiling

        public static int Ceiling(this float value)
        {
            return (int)Math.Ceiling(value);
        }

        public static int Ceiling(this double value)
        {
            return (int)Math.Ceiling(value);
        }

        public static int Ceiling(this decimal value)
        {
            return (int)Math.Ceiling(value);
        }

        #endregion
        #region Round

        public static int Round(this float value)
        {
            return (int)Math.Round(value, 0, MidpointRounding.AwayFromZero);
        }
        public static int Round(this double value)
        {
            return (int)Math.Round(value, 0, MidpointRounding.AwayFromZero);
        }
        public static int Round(this decimal value)
        {
            return (int)Math.Round(value, 0, MidpointRounding.AwayFromZero);
        }

        // Overloads
        public static double Round(this float value, int decimalPlaces)
        {
            return Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero);
        }
        public static double Round(this double value, int decimalPlaces)
        {
            return Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero);
        }
        public static double Round(this decimal value, int decimalPlaces)
        {
            return Convert.ToInt32(Math.Round(value, decimalPlaces, MidpointRounding.AwayFromZero));
        }

        #endregion
        #region Sqrt

        public static double Sqrt(this short value)
        {
            return Math.Sqrt(value);
        }
        public static double Sqrt(this int value)
        {
            return Math.Sqrt(value);
        }
        public static double Sqrt(this long value)
        {
            return Math.Sqrt(value);
        }
        public static double Sqrt(this float value)
        {
            return Math.Sqrt(value);
        }
        public static double Sqrt(this double value)
        {
            return Math.Sqrt(value);
        }
        public static double Sqrt(this decimal value)
        {
            return Math.Sqrt(Convert.ToDouble(value));
        }

        #endregion

        #region Max Value

        public static short MaxValue(this short value, short MaxValue)
        {
            return value > MaxValue ? MaxValue : value;
        }

        public static int MaxValue(this int value, int MaxValue)
        {
            return value > MaxValue ? MaxValue : value;
        }

        public static long MaxValue(this long value, long MaxValue)
        {
            return value > MaxValue ? MaxValue : value;
        }

        #endregion
        #region Min Value

        public static short MinValue(this short value, short MinValue)
        {
            return value < MinValue ? MinValue : value;
        }

        public static int MinValue(this int value, int MinValue)
        {
            return value < MinValue ? MinValue : value;
        }

        public static long MinValue(this long value, long MinValue)
        {
            return value < MinValue ? MinValue : value;
        }

        #endregion

        #region Repeat

        public static int Repeat(this int value, int Count)
        {
            return new StringBuilder(value.ToString().Length * Count).Insert(0, value.ToString(), Count).ToString().ToInt();
        }


        #endregion
        #region Pad Left

        public static int PadLeft(this int value, int TotalLength)
        {
            int x = TotalLength - value.ToString().Length;
            return x != 0 ? 0.Repeat(x) + value : value;
        }

        // Overload 1
        public static int PadLeft(this int value, int TotalLength, int PaddingNumber)
        {
            int x = TotalLength - value.ToString().Length;
            return x != 0 ? PaddingNumber.Repeat(x) + value : value;
        }

        #endregion
        #region Pad Right

        public static int PadRight(this int value, int TotalLength)
        {
            int x = TotalLength - value.ToString().Length;
            return x != 0 ? value + 0.Repeat(x) : value;
        }

        // Overload 1
        public static int PadRight(this int value, int TotalLength, int PaddingNumber)
        {
            int x = TotalLength - value.ToString().Length;
            return x != 0 ? value + PaddingNumber.Repeat(x) : value;
        }

        #endregion

        #region KB

        public static int KB(this int value)
        {
            return value * 1024;
        }

        #endregion
        #region MB

        public static int MB(this int value)
        {
            return value * 1024 * 1024;
        }

        #endregion
        #region Size Suffix

        public static string SizeSuffix(this int value, int decimalPlaces)
        {
            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
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

        public static string SizeSuffix(this long value, int decimalPlaces)
        {
            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
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


        #region Is Between

        public static bool IsBetween<T>(this T Number, T Lower, T Upper) where T : IComparable<T>
        {
            return Number.CompareTo(Lower) >= 0 && Number.CompareTo(Upper) < 0;
        }

        #endregion
        #region Is Prime

        public static bool IsPrime(this int number)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }

        #endregion


        #region To Roman String

        public static string ToRomanString(this int value)
        {
            Dictionary<string, int> romanNumerals =
                new Dictionary<string, int>(13)
                {
                { "M", 1000 },
                { "CM", 900 },
                { "D", 500 },
                { "CD", 400 },
                { "C", 100 },
                { "XC", 90 },
                { "L", 50 },
                { "XL", 40 },
                { "X", 10 },
                { "IX", 9 },
                { "V", 5 },
                { "IV", 4 },
                { "I", 1 }
                };

            const int MinValue = 1;
            const int MaxValue = 3999;

            if ((value < MinValue) || (value > MaxValue))
            {
                throw new ArgumentOutOfRangeException("value", value, "Argument out of Roman numeral range.");
            }

            const int MaxRomanNumeralLength = 15;
            var sb = new StringBuilder(MaxRomanNumeralLength);

            foreach (var pair in romanNumerals)
            {
                while (value / pair.Value > 0)
                {
                    sb.Append(pair.Key);
                    value -= pair.Value;
                }
            }
            return sb.ToString();
        }

        #endregion

        #region To Double

        public static double ToDouble(this short value)
        {
            return Convert.ToDouble(value);
        }
        public static double ToDouble(this int value)
        {
            return Convert.ToDouble(value);
        }
        public static double ToDouble(this long value)
        {
            return Convert.ToDouble(value);
        }

        #endregion
        #region To Decimal

        public static decimal ToDecimal(this short value)
        {
            return Convert.ToDecimal(value);
        }
        public static decimal ToDecimal(this int value)
        {
            return Convert.ToDecimal(value);
        }
        public static decimal ToDecimal(this long value)
        {
            return Convert.ToDecimal(value);
        }

        #endregion

    }

    #endregion


    #region IEnumerable Extensions

    public static class IEnumerableExtensions
    {

        #region Join

        public static string Join<T>(this IEnumerable<T> array)
        {
            return string.Join("", array);
        }

        // Overload 1
        public static string Join<T>(this IEnumerable<T> array, string Separator)
        {
            return string.Join(Separator, array);
        }

        #endregion
        #region Concat 

        public static T[] Concat<T>(this T[] array, params T[][] arrays)
        {
            T[] result = new T[arrays.Sum(a => a.Length) + array.Length];

            array.CopyTo(result, 0);
            int offset = array.Length;
            foreach (T[] item in arrays)
            {
                item.CopyTo(result, offset);
                offset += item.Length;
            }

            return result;
        }

        #endregion


        #region Shuffle 

        public static IList<T> Shuffle<T>(this IList<T> array)
        {
            Random rng = new Random();
            int n = array.ToArray().Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }

            return array;
        }

        #endregion
        #region Random One

        public static T RandomOne<T>(this IEnumerable<T> array)
        {
            Random random = new Random();
            return array.ToArray()[random.Next(array.ToArray().Length)];
        }

        #endregion

    }

    #endregion


    #region Object Extensions

    public static class ObjectExtensions
    {

        #region Equals Any Of

        public static bool EqualsAnyOf(this object source, params object[] List)
        {
            string? value = source.ToString();
            if (string.IsNullOrEmpty(value)) return false;

            foreach (object obj in List)
                if (value == obj.ToString()) return true;

            return false;
        }

        #endregion
        #region Contains Any Of

        public static bool ContainsAnyOf(this object source, params object[] List)
        {
            string? value = source.ToString();
            if (string.IsNullOrEmpty(value)) return false;

            foreach (object obj in List)
            {
                string? target = obj.ToString();
                if (string.IsNullOrEmpty(target)) continue;

                if (value.Contains(target)) return true;
            }

            return false;
        }

        #endregion
        #region Contains All Of

        public static bool ContainsAllOf(this object source, params object[] List)
        {
            string? value = source.ToString();
            if (string.IsNullOrEmpty(value)) return false;

            foreach (object obj in List)
            {
                string? target = obj.ToString();
                if (string.IsNullOrEmpty(target)) continue;

                if (!value.Contains(target)) return false;
            }

            return true;
        }

        #endregion

    }

    #endregion


    #region Random Extensions

    public static class RandomExtensions
    {

        #region Bool

        public static bool Bool(this Random random)
        {
            return random.Next(2) == 0 ? true : false;
        }

        #endregion

        #region Number

        public static int Number(this Random random, int MaxValue)
        {
            return random.Next(MaxValue);
        }

        // Overload 1
        public static int Number(this Random random, int MinValue, int MaxValue)
        {
            return random.Next(MinValue, MaxValue);
        }

        #endregion

        #region String

        public static string String(this Random random, int Length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, Length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #endregion

    }

    #endregion


    #region Control Extensions

    public static class ControlExtensions
    {

        #region Shorten

        public static void Shorten(this Control Control, bool Vertical, int MillisecondTime, bool Center)
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            int Length = Vertical ? Control.Height : Control.Width;
            double _LengthPerRound = Length / (MillisecondTime / 14.0);
            int LengthPerRound = Convert.ToInt32(Math.Ceiling(_LengthPerRound));
            int CenterLoc = (Vertical ? Control.Location.Y : Control.Location.X) + (Length / 2);

            System.Timers.Timer Timer = new System.Timers.Timer();
            Timer.Interval = LengthPerRound / _LengthPerRound * 10;
            Timer.Elapsed += (object? sender, ElapsedEventArgs a) =>
            {
                bool Last = false;
                if ((Vertical ? Control.Height : Control.Width) < LengthPerRound)
                {
                    if (Vertical) Control.Height = 1;
                    else Control.Width = 1;

                    Last = true;
                }
                // Is Last Round

                if (Center)
                {
                    if (Vertical) Control.Location = new Point(Control.Location.X, CenterLoc - (Control.Height / 2));
                    else Control.Location = new Point(CenterLoc - (Control.Width / 2), Control.Location.Y);
                }
                // Center

                if (Last) { Timer.Dispose(); return; }
                // Return If Last Round

                if (Vertical) Control.Height -= LengthPerRound;
                else Control.Width -= LengthPerRound;
            };
            Timer.Start();
        }

        public static void Shorten(this Control Control, bool Vertical, int MillisecondTime)
        { Shorten(Control, Vertical, MillisecondTime, false); }
        // Overload

        #endregion

        #region Stretch

        public static void Stretch(this Control Control, int Length, bool Vertical, int MillisecondTime, bool Center)
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            int InitialLength = Vertical ? Control.Height : Control.Width;
            double _LengthPerRound = (Length - InitialLength) / (MillisecondTime / 14.0);
            int LengthPerRound = Convert.ToInt32(Math.Ceiling(_LengthPerRound));
            int CenterLoc = (Vertical ? Control.Location.Y : Control.Location.X) + (InitialLength / 2);

            System.Timers.Timer Timer = new System.Timers.Timer();
            Timer.Interval = LengthPerRound / _LengthPerRound * 10;
            Timer.Elapsed += (object? sender, ElapsedEventArgs a) =>
            {
                bool Last = false;
                if ((Vertical ? Control.Height : Control.Width) > Length - LengthPerRound)
                {
                    if (Vertical) Control.Height = Length;
                    else Control.Width = Length;

                    Last = true;
                }
                // Is Last Round

                if (Center)
                {
                    if (Vertical) Control.Location = new Point(Control.Location.X, CenterLoc - (Control.Height / 2));
                    else Control.Location = new Point(CenterLoc - (Control.Width / 2), Control.Location.Y);
                }
                // Center

                if (Last) { Timer.Dispose(); return; }
                // Return If Last Round

                if (Vertical) Control.Height += LengthPerRound;
                else Control.Width += LengthPerRound;
            };
            Timer.Start();
        }

        public static void Stretch(this Control Control, int Length, bool Vertical, int MillisecondTime)
        { Stretch(Control, Length, Vertical, MillisecondTime, false); }
        // Overload

        #endregion

    }

    #endregion


}
