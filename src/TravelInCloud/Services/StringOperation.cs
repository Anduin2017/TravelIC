using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TravelInCloud.Services
{
    public static class StringOperation
    {
        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            //for (int i = 0; i < data.Length; i++)
            foreach (var c in data)
            {
                sBuilder.Append(c.ToString("x2"));
            }
            return sBuilder.ToString();
        }
        public static bool IsImage(this string str)
        {
            bool isimage = false;
            string thestr = str.ToLower();
            string[] allowExtension = { ".jpg", ".gif", ".bmp", ".png", ".jpeg" };
            for (int i = 0; i < allowExtension.Length; i++)
            {
                if (thestr == allowExtension[i])
                {
                    isimage = true;
                    break;
                }
            }
            return isimage;
        }
        public static string GetMD5(this string SourceString)
        {
            string hash = GetMd5Hash(MD5.Create(), SourceString);
            return hash;
        }
        public static string OTake(this string source, int Count)
        {
            if (source.Length <= Count)
            {
                return source;
            }
            else
            {
                return source.Substring(0, Count - 3) + "...";
            }
        }
        public static string ORemoveHTML(this string Content)
        {
            string s = string.Empty;
            Content = WebUtility.HtmlDecode(Content);
            if (!Content.Contains(">"))
            {
                return Content;
            }
            while (Content.Contains(">"))
            {
                s = s + Content.Substring(0, Content.IndexOf("<"));
                Content = Content.Substring(Content.IndexOf(">") + 1);
            }
            return s;
        }
        public static string RandomString(int count)
        {
            int number;
            string checkCode = string.Empty;
            var random = new Random();
            for (int i = 0; i < count; i++)
            {
                number = random.Next();
                number = number % 36;
                if (number < 10)
                {
                    number += 48;
                }
                else
                {
                    number += 55;
                }
                checkCode += ((char)number).ToString();
            }
            return checkCode;
        }
    }

}
