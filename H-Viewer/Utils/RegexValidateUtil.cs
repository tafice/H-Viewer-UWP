using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HViewer.Utils
{
    public class RegexValidateUtil
    {
        public static bool checkMobileNumber(String mobileNumber)
        {
            bool flag = false;
            try
            {
                Regex r = new Regex("^(((13[0-9])|(14[0-9])|(15([0-3]|[5-9]))|(17[0-9])|(18[0-9]))\\d{8})|(0\\d{2}-\\d{8})|(0\\d{3}-\\d{7})$");
                Match m = r.Match(mobileNumber);
                flag = m.Success;
            }
            catch (Exception e)
            {
                flag = false;
            }
            return flag;
        }

        public static String getHostFromUrl(String url)
        {
            Regex r = new Regex("https?://[^/]*", RegexOptions.IgnoreCase);
            Match m = r.Match(url);
            if (m.Success)
                return m.Groups[0].ToString();
            else
                return "";
        }

        public static String getDominFromUrl(String url)
        {
            Regex r = new Regex("https?://([^/]*)", RegexOptions.IgnoreCase);
            Match m = r.Match(url);
            if (m.Success)
                return m.Groups[1].Value;
            else
                return "";
        }

        public static String geCurrDirFromUrl(String url)
        {
            Regex r = new Regex("https?://[\\w./]*/", RegexOptions.IgnoreCase);
            Match m = r.Match(url);
            if (m.Success)
                return m.Groups.ToString();
            else
                return "";
        }

        public static string getAbsoluteUrlFromRelative(string url, string host)
        {
            if (url.StartsWith("/"))
                return getHostFromUrl(host) + url;
            else if (url.StartsWith("./"))
                return geCurrDirFromUrl(host) + url;
            else if (url.StartsWith("../"))
            {
                Regex r = new Regex("(https?://[\\w./]*/).*/", RegexOptions.IgnoreCase);
                Match m = r.Match(url);
                string prefix = m.Success ? m.Groups[1].Value : "";
                return prefix + url.Substring(3);
            }
            else if (url.StartsWith("../../"))
            {
                Regex r = new Regex("(https?://[\\w./]*/).*/.*/", RegexOptions.IgnoreCase);
                Match m = r.Match(url);
                string prefix = m.Success ? m.Groups[1].Value : "";
                return prefix + url.Substring(6);
            }
            else
                return url;
        }
    }
}
