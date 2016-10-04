using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
}
