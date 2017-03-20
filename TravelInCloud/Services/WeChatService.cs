using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TravelInCloud.Models;
using System.Text;

namespace TravelInCloud.Services
{
    public class WeChatService
    {
        public static bool Verify(string signature, string timestamp, string nonce)
        {
            var Token = Secrets.Token;
            string[] ArrTmp = { Token, timestamp, nonce };
            Array.Sort(ArrTmp);
            var tmpStr = string.Join(string.Empty, ArrTmp);
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var hash = sha1.ComputeHash(Encoding.Unicode.GetBytes(tmpStr));
            return true;
            //return Encoding.Unicode.GetString(hash).ToLower().Trim() == signature.ToLower().Trim();
        }
        private static DateTime _AccessTokenRefreshTime { get; set; }
        private static string _WeChatServerAddress = @"https://api.weixin.qq.com/cgi-bin/";
        private class _AccessTokenResult
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
        }
        private static _AccessTokenResult _AccessToken { get; set; }
        public async static Task<string> AccessTokenAsync()
        {
            if (_AccessToken == null || DateTime.Now > _AccessTokenRefreshTime + new TimeSpan(0, 0, _AccessToken.expires_in - 200))
            {
                var HTTPContainer = new HTTPService();
                var URL = _WeChatServerAddress + $@"token?grant_type=client_credential&appid={
                    Secrets.AppID}&secret={
                    Secrets.AppSecret}";
                var JResult = await HTTPContainer.Get(URL);
                var Result = JsonConvert.DeserializeObject<_AccessTokenResult>(JResult);
                _AccessTokenRefreshTime = DateTime.Now;
                _AccessToken = Result;
            }
            return _AccessToken.access_token;
        }

        public async static Task<string> GenerateAuthUrlAsync(string Destination, string Argument = "null")
        {
            return await Task.Factory.StartNew(() =>
            {
                string result = @"https://open.weixin.qq.com" + $@"/connect/oauth2/authorize?appid={
                 Secrets.AppID}&redirect_uri={
                 WebUtility.UrlEncode(Destination)}&response_type=code&scope=snsapi_base&state={Argument}#wechat_redirect";
                return result;
            });
        }

        public async static Task<AuthAccessToken> AuthCodeToAccessTokenAsync(string code)
        {
            var HTTPContainer = new HTTPService();
            var URL = $@"https://api.weixin.qq.com/sns/oauth2/access_token?appid={
                Secrets.AppID}&secret={
                Secrets.AppSecret}&code={code}&grant_type=authorization_code ";
            var result = await HTTPContainer.Get(URL);
            var JResult = JsonConvert.DeserializeObject<AuthAccessToken>(result);
            return JResult;
        }

        public async static Task<WeChatUser> UserInfomationAsync(string openId, string AuthAccessToken)
        {
            var HTTPContainer = new HTTPService();
            var URL = _WeChatServerAddress + $@"user/info?access_token={AuthAccessToken}&openid={openId}&lang=zh_CN";
            var Jresult = await HTTPContainer.Get(URL);
            var result = JsonConvert.DeserializeObject<WeChatUser>(Jresult);
            return result;
        }
        public async static Task<string> Reply(string text)
        {
            return await Task.Factory.StartNew(() =>
            {
                return $"强哥是傻屌";// + GenerateAddress($"[{text}]", "https://www.obisoft.com.cn/");
            });
        }
        public async static Task<T> XMLDeserializeObjectAsync<T>(string inputXML) where T : class
        {
            return await Task.Factory.StartNew(() =>
            {
                StringReader sr = new StringReader(inputXML);
                XmlSerializer xmldes = new XmlSerializer(typeof(T));
                var Result = xmldes.Deserialize(sr) as T;
                return Result;
            });
        }
        public async static Task<T> XMLDeserializeObjectAsync<T>(Stream inputXML) where T : class
        {
            return await Task.Factory.StartNew(() =>
            {
                XmlSerializer xmldes = new XmlSerializer(typeof(T));
                var Result = xmldes.Deserialize(inputXML) as T;
                return Result;
            });
        }
        public async static Task<string> XMLSerializeObjectAsync<T>(T obj)
        {
            var Stream = new MemoryStream();
            var xml = new XmlSerializer(typeof(T));
            xml.Serialize(Stream, obj);
            Stream.Position = 0;
            var sr = new StreamReader(Stream);
            string str = await sr.ReadToEndAsync();
            sr.Dispose();
            Stream.Dispose();
            return str.Replace("<?xml version=\"1.0\"?>", "")
            .Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
        }
        public static string GenerateAddress(string DisplayText, string URL)
        {
            return $"<a href=\"{URL}\">{DisplayText}</a>";
        }
        public static DateTime UnixTimeToTime(string timeStamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1);
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        public static int ConvertDateTimeInt(DateTime time)
        {
            DateTime startTime = new DateTime(1970, 1, 1);
            return (int)(time - startTime).TotalSeconds;
        }
    }

}
