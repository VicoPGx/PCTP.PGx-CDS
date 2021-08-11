using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Web;
using System.Runtime.Serialization.Json;

namespace System.App.Utility.Helpers
{
    [DataContract]
    public class AdmAccessToken
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }

    /// <summary>
    /// Authorization for MarketPlace services, such as Bing Search or Bing Translate.
    /// <see cref="!:https://datamarket.azure.com/account"/>
    /// </summary>
    public class AdmAuthentication
    {
        public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        private string clientId;
        private string cientSecret;
        private string request;

        public AdmAuthentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.cientSecret = clientSecret;
            //If clientid or client secret has special characters, encode before sending request
            this.request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", HttpUtility.UrlEncode(clientId), HttpUtility.UrlEncode(clientSecret));
        }

        public AdmAccessToken GetAccessToken()
        {
            return HttpPost(DatamarketAccessUri, this.request);
        }

        private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
        {
            //Prepare OAuth request 
            WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                //Get deserialized object from JSON stream
                AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }
    }
    
    public class TermTranslationHelper
    {
        public const string AFRIKAANS = "af";
        public const string ALBANIAN = "sq";
        public const string AMHARIC = "am";
        public const string ARABIC = "ar";
        public const string ARMENIAN = "hy";
        public const string AZERBAIJANI = "az";
        public const string BASQUE = "eu";
        public const string BELARUSIAN = "be";
        public const string BENGALI = "bn";
        public const string BIHARI = "bh";
        public const string BULGARIAN = "bg";
        public const string BURMESE = "my";
        public const string CATALAN = "ca";
        public const string CHEROKEE = "chr";
        public const string CHINESE = "zh";
        public const string CHINESE_SIMPLIFIED = "zh-CN";
        public const string CHINESE_TRADITIONAL = "zh-TW";
        public const string CROATIAN = "hr";
        public const string CZECH = "cs";
        public const string DANISH = "da";
        public const string DHIVEHI = "dv";
        public const string DUTCH = "nl";
        public const string ENGLISH = "en";
        public const string ESPERANTO = "eo";
        public const string ESTONIAN = "et";
        public const string FILIPINO = "tl";
        public const string FINNISH = "fi";
        public const string FRENCH = "fr";
        public const string GALICIAN = "gl";
        public const string GEORGIAN = "ka";
        public const string GERMAN = "de";
        public const string GREEK = "el";
        public const string GUARANI = "gn";
        public const string GUJARATI = "gu";
        public const string HEBREW = "iw";
        public const string HINDI = "hi";
        public const string HUNGARIAN = "hu";
        public const string ICELANDIC = "is";
        public const string INDONESIAN = "id";
        public const string INUKTITUT = "iu";
        public const string ITALIAN = "it";
        public const string JAPANESE = "ja";
        public const string KANNADA = "kn";
        public const string KAZAKH = "kk";
        public const string KHMER = "km";
        public const string KOREAN = "ko";
        public const string KURDISH = "ku";
        public const string KYRGYZ = "ky";
        public const string LAOTHIAN = "lo";
        public const string LATVIAN = "lv";
        public const string LITHUANIAN = "lt";
        public const string MACEDONIAN = "mk";
        public const string MALAY = "ms";
        public const string MALAYALAM = "ml";
        public const string MALTESE = "mt";
        public const string MARATHI = "mr";
        public const string MONGOLIAN = "mn";
        public const string NEPALI = "ne";
        public const string NORWEGIAN = "no";
        public const string ORIYA = "or";
        public const string PASHTO = "ps";
        public const string PERSIAN = "fa";
        public const string POLISH = "pl";
        public const string PORTUGUESE = "pt-PT";
        public const string PUNJABI = "pa";
        public const string ROMANIAN = "ro";
        public const string RUSSIAN = "ru";
        public const string SANSKRIT = "sa";
        public const string SERBIAN = "sr";
        public const string SINDHI = "sd";
        public const string SINHALESE = "si";
        public const string SLOVAK = "sk";
        public const string SLOVENIAN = "sl";
        public const string SPANISH = "es";
        public const string SWAHILI = "sw";
        public const string SWEDISH = "sv";
        public const string TAJIK = "tg";
        public const string TAMIL = "ta";
        public const string TAGALOG = "tl";
        public const string TELUGU = "te";
        public const string THAI = "th";
        public const string TIBETAN = "bo";
        public const string TURKISH = "tr";
        public const string UKRAINIAN = "uk";
        public const string URDU = "ur";
        public const string UZBEK = "uz";
        public const string UIGHUR = "ug";
        public const string VIETNAMESE = "vi";
        public const string UNKNOWN = "";

        private static string headerValue = null;
        /// <summary>
        /// Bing Translate API requires a MarketPlace App id。
        /// Don't over use it, otherwise service will be blocked.
        /// </summary>
        public static string BingTranslate(string stringToTranslate, string fromLanguage, string toLanguage)
        {
            if (string.IsNullOrEmpty(headerValue))
            {
                AdmAccessToken admToken;
                //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
                //Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
                AdmAuthentication admAuth = new AdmAuthentication("7c48fdef-f87d-4824-b6ac-8feafcdbcf05", "IhQWGF8ov46nE9L14GrzsLgqmoUyvvBHAhl7ebhVe1k");

                admToken = admAuth.GetAccessToken();
                // Create a header with the access_token property of the returned token
                headerValue = "Bearer " + admToken.access_token;
            }

            string text = stringToTranslate;
            string from = fromLanguage;
            string to = toLanguage;
            string translation = string.Empty;

            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + System.Web.HttpUtility.UrlEncode(text) + "&from=" + from + "&to=" + to;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Headers.Add("Authorization", headerValue);
            WebResponse response = null;

            response = httpWebRequest.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                translation = (string)dcs.ReadObject(stream);
            }

            if (response != null)
            {
                response.Close();
                response = null;
            }
            return translation;
        }


        /// <summary>
        /// Translate by Google service.
        /// Currently, the Google Translate Service is blocked. 
        /// <see cref="!:https://developers.google.com/errors/?hl=zh-CN"/>
        /// </summary>
        public static string GoogleTranslate(string stringToTranslate, string fromLanguage, string toLanguage)
        {
            // make sure that the passed string is not empty or null
            if (!String.IsNullOrEmpty(stringToTranslate))
            {
                // per Google's terms of use, we can only translate
                // a string of up to 5000 characters long
                if (stringToTranslate.Length <= 5000)
                {
                    const int bufSizeMax = 65536;
                    const int bufSizeMin = 8192;

                    try
                    {
                        // by default format? is text.  
                        // so we don't need to send a format? key
                        string requestUri = "http://ajax.googleapis.com/ajax/services/language/translate?v=1.0&q=" + stringToTranslate + "&langpair=" + fromLanguage + "%7C" + toLanguage;

                        // execute the request and get the response stream
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Stream responseStream = response.GetResponseStream();

                        // get the length of the content returned by the request
                        int length = (int)response.ContentLength;
                        int bufSize = bufSizeMin;

                        if (length > bufSize)
                            bufSize = length > bufSizeMax ? bufSizeMax : length;

                        // allocate buffer and StringBuilder for reading response
                        byte[] buf = new byte[bufSize];
                        StringBuilder sb = new StringBuilder(bufSize);

                        // read the whole response
                        while ((length = responseStream.Read(buf, 0, buf.Length)) != 0)
                        {
                            sb.Append(Encoding.UTF8.GetString(buf, 0, length));
                        }

                        // the format of the response is like this
                        // {"responseData": {"translatedText":"¿Cómo estás?"},             "responseDetails": null, "responseStatus": 200}
                        // so now let's clean up the response by manipulating the string
                        string translatedText = sb.Remove(0, 36).ToString();
                        translatedText = translatedText.Substring(0,
                        translatedText.IndexOf("\"},"));

                        return translatedText;
                    }
                    catch
                    {
                        return "Cannot get the translation.  Please try again later.";
                    }
                }
                else
                {
                    return "String to translate must be less than 5000 characters long.";
                }
            }
            else
            {
                return "String to translate is empty.";
            }
        }
        

        private static bool isInitialized = false;

        /// <summary>
        /// This is an English to Chinese dictionary
        /// </summary>
        private static Dictionary<String, String> dictionary = new Dictionary<String, String>();

        public static String TranslateTerm(String term)
        {
            if (isInitialized == false)
            {
                PopulateDictionary(ref dictionary);
                isInitialized = true;
            }

            if (dictionary.ContainsKey(term))
                return dictionary[term];
            else
                return term;
        }

        // [TODO] In real application, retrive from database or config xml
        public static void PopulateDictionary(ref Dictionary<String, String> dict)
        {
            #region English2Chinese

            dict.Add("Anesthesia","麻醉");
            dict.Add("Examination","检查");
            dict.Add("Medication","药疗");
            dict.Add("Nurse_CheckList","护理工作");
            dict.Add("Nurse CheckList","护理工作");
            dict.Add("Nursing","护理");
            dict.Add("Diet","膳食");
            dict.Add("Physician_CheckList","诊疗工作");
            dict.Add("Physician CheckList","诊疗工作");
            dict.Add("Surgery","手术");
            dict.Add("Test","检验");
            dict.Add("Treatment","处置");
            dict.Add("Other","其它");

            dict.Add("Longterm", "长期");
            dict.Add("Temporary", "临时");

            #endregion


            #region Chinese2English

            dict.Add("麻醉","Anesthesia");
            dict.Add("检查", "Examination");
            dict.Add("药疗", "Medication");
            dict.Add("护理工作", "Nurse_CheckList");
            dict.Add("护理", "Nursing");
            dict.Add("膳食", "Diet");
            dict.Add("诊疗工作","Physician_CheckList");
            dict.Add("手术", "Surgery");
            dict.Add("检验","Test");
            dict.Add("处置", "Treatment");
            dict.Add("其它", "Other");

            dict.Add("长期", "Longterm");
            dict.Add("临时", "Temporary");

            #endregion
        }
    }
}
