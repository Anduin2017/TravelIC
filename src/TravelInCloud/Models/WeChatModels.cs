using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace TravelInCloud.Models
{
    public class Source
    {
        public List<Button> button { get; set; }
    }
    public class Button
    {
        public string name { get; set; }
        public List<SubButton> sub_button { get; set; }
    }
    public class SubButton
    {
        public string type { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }
    public class AuthAccessToken
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string openid { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
    }
    public class xml
    {
        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
        public int CreateTime { get; set; }
        public string MsgType { get; set; }
        public string Content { get; set; }
        public long MsgId { get; set; }
    }
    public class WeChatUser
    {
        public virtual int subscribe { get; set; }
        public virtual string openid { get; set; }
        public virtual string nickname { get; set; }
        public virtual int sex { get; set; }
        public virtual string language { get; set; }
        public virtual string city { get; set; }
        public virtual string province { get; set; }
        public virtual string country { get; set; }
        public virtual string headimgurl { get; set; }
        public virtual int subscribe_time { get; set; }
        public virtual string remark { get; set; }
        public virtual int groupid { get; set; }
        public virtual List<dynamic> tagid_list { get; set; }
    }
    public class JsApiPay
    {
        public JsApiPay(string OpenId,int Amount,string IP)
        {
            this.openid = OpenId;
            this.total_fee = Amount;
            this.IP = IP;
        }
        //private Controller Controller { get; set; }
        public string openid { get; set; }
        public string access_token { get; set; }
        public int total_fee { get; set; }
        public string IP { get; set; }
        public WxPayData unifiedOrderResult { get; set; }

        public async Task<WxPayData> GetUnifiedOrderResult()
        {
            //统一下单
            WxPayData data = new WxPayData();
            data.SetValue("body", "test");
            data.SetValue("attach", "test");
            data.SetValue("out_trade_no", WxPayApi.GenerateOutTradeNo());
            data.SetValue("total_fee", total_fee);
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            data.SetValue("goods_tag", "test");
            data.SetValue("trade_type", "JSAPI");
            data.SetValue("openid", openid);

            WxPayData result = await WxPayApi.UnifiedOrder(data, IP: IP);
            if (!result.IsSet("appid") || !result.IsSet("prepay_id") || result.GetValue("prepay_id").ToString() == "")
            {
                throw new Exception(result.ToJson());
            }

            unifiedOrderResult = result;
            return result;
        }
        public string GetJsApiParameters()
        {
            WxPayData jsApiParam = new WxPayData();
            jsApiParam.SetValue("appId", unifiedOrderResult.GetValue("appid"));
            jsApiParam.SetValue("timeStamp", WxPayApi.GenerateTimeStamp());
            jsApiParam.SetValue("nonceStr", WxPayApi.GenerateNonceStr());
            jsApiParam.SetValue("package", "prepay_id=" + unifiedOrderResult.GetValue("prepay_id"));
            jsApiParam.SetValue("signType", "MD5");
            jsApiParam.SetValue("paySign", jsApiParam.MakeSign());
            string parameters = jsApiParam.ToJson();
            return parameters;
        }
    }
    public class WxPayData
    {
        public WxPayData()
        {

        }
        private SortedDictionary<string, object> m_values = new SortedDictionary<string, object>();
        public void SetValue(string key, object value)
        {
            m_values[key] = value;
        }
        public object GetValue(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return o;
        }
        public bool IsSet(string key)
        {
            object o = null;
            m_values.TryGetValue(key, out o);
            return o != null;
        }
        public string MakeSign()
        {
            //转url格式
            string str = ToUrl();
            //在string后加入API KEY
            str += "&key=" + Secrets.KEY;
            //MD5加密
            var md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }

        public SortedDictionary<string, object> FromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new Exception("将空的xml串转换为WxPayData不合法!");
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode xmlNode = xmlDoc.FirstChild;//获取到根节点<xml>
            XmlNodeList nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                XmlElement xe = (XmlElement)xn;
                m_values[xe.Name] = xe.InnerText;//获取xml的键值对到WxPayData内部的数据中
            }
            //2015-06-29 错误是没有签名
            if (m_values["return_code"] as string != "SUCCESS")
            {
                return m_values;
            }
            CheckSign();
            return m_values;
        }
        public bool CheckSign()
        {
            if (!IsSet("sign"))
            {
                throw new Exception("WxPayData签名存在但不合法!");
            }
            else if (GetValue("sign") == null || GetValue("sign").ToString() == "")
            {
                throw new Exception("WxPayData签名存在但不合法!");
            }
            string return_sign = GetValue("sign").ToString();
            string cal_sign = MakeSign();
            if (cal_sign == return_sign)
            {
                return true;
            }
            throw new Exception("WxPayData签名验证错误!");
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(m_values);
        }
        public string ToPrintStr()
        {
            string str = "";
            foreach (var pair in m_values)
            {
                if (pair.Value == null)
                {
                    throw new Exception("WxPayData内部含有值为null的字段!");
                }
                str += string.Format($"{pair.Key}={pair.Value.ToString()}<br>");
            }
            return str;
        }
        public string ToUrl()
        {
            string buff = string.Empty;
            foreach (var pair in m_values)
            {
                if (pair.Value == null)
                {
                    throw new Exception("WxPayData内部含有值为null的字段!");
                }
                if (pair.Key != "sign" && pair.Value.ToString() != "")
                {
                    buff += $"{pair.Key}={pair.Value}&";
                }
            }
            return buff.Trim('&');
        }
        public string ToXml()
        {
            if (0 == m_values.Count)
            {
                throw new Exception("WxPayData数据为空!");
            }
            string xml = "<xml>";
            foreach (var pair in m_values)
            {
                if (pair.Value == null)
                {
                    throw new Exception("WxPayData内部含有值为null的字段!");
                }
                if (pair.Value is int)
                {
                    xml += $"<{pair.Key}>{pair.Value}</{pair.Key}>";
                }
                else if (pair.Value is string)
                {
                    xml += $"<{pair.Key}><![CDATA[{pair.Value}]]></{pair.Key}>";
                }
                else
                {
                    throw new Exception("WxPayData字段数据类型错误!");
                }
            }
            xml += "</xml>";
            return xml;
        }
    }
    public class WxPayApi
    {
        public static string GenerateOutTradeNo()
        {
            var ran = new Random();
            return string.Format("{0}{1}{2}", Secrets.mch_id, DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }
        public static async Task<WxPayData> UnifiedOrder(WxPayData inputObj, int timeOut = 6, string IP = "")
        {
            string url = "https://api.mch.weixin.qq.com/pay/unifiedorder";
            //检测必填参数
            if (!inputObj.IsSet("out_trade_no"))
            {
                throw new Exception("缺少统一支付接口必填参数out_trade_no！");
            }
            else if (!inputObj.IsSet("body"))
            {
                throw new Exception("缺少统一支付接口必填参数body！");
            }
            else if (!inputObj.IsSet("total_fee"))
            {
                throw new Exception("缺少统一支付接口必填参数total_fee！");
            }
            else if (!inputObj.IsSet("trade_type"))
            {
                throw new Exception("缺少统一支付接口必填参数trade_type！");
            }
            if (inputObj.GetValue("trade_type").ToString() == "JSAPI" && !inputObj.IsSet("openid"))
            {
                throw new Exception("统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
            }
            if (inputObj.GetValue("trade_type").ToString() == "NATIVE" && !inputObj.IsSet("product_id"))
            {
                throw new Exception("统一支付接口中，缺少必填参数product_id！trade_type为JSAPI时，product_id为必填参数！");
            }

            //异步通知url未设置，则使用配置文件中的url
            if (!inputObj.IsSet("notify_url"))
            {
                inputObj.SetValue("notify_url", Secrets.NOTIFY_URL);//异步通知url
            }

            inputObj.SetValue("appid", Secrets.AppID);//公众账号ID
            inputObj.SetValue("mch_id", Secrets.mch_id);//商户号
            inputObj.SetValue("spbill_create_ip", IP);//终端ip	  	    
            inputObj.SetValue("nonce_str", GenerateNonceStr());//随机字符串

            //签名
            inputObj.SetValue("sign", inputObj.MakeSign());
            string xml = inputObj.ToXml();

            var Service = new Services.HTTPService();
            string response = await Service.Post(url, xml);


            WxPayData result = new WxPayData();
            result.FromXml(response);

            return result;
        }
        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        public static string GenerateNonceStr()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
