using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HViewer.Model;
using HViewer.Utils;
using NSoup_UWP.Nodes;
using NSoup_UWP;
using NSoup_UWP.Select;
using NSoup_UWP.Helper;

namespace HViewer.Core
{
    public class RuleParser
    {
        public static Dictionary<string, string> ParseUrl(string url)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(url))
                return map;

            Regex r = new Regex(@"\{([^\{\}]*?):([^\{\}]*?)\}", RegexOptions.Singleline);
            var matches = r.Matches(url);
            foreach (Match match in matches)
            {
                map.Add(match.Groups[1].Value, match.Groups[2].Value);
            }

            Regex r2 = new Regex(@"\{([^{}]*?):([^{}]*?\{[^{}]*?\}[^{}]*?)\}", RegexOptions.Singleline);
            var matches2 = r2.Matches(url);
            foreach (Match match in matches2)
            {
                map.Add(match.Groups[1].Value, match.Groups[2].Value);
            }

            return map;
        }

        public static string ParseUrl(string url, int page, string idCode, string keyword, Object[] objs)
        {
            Dictionary<string, string> matchResult = RuleParser.ParseUrl(url);
            string pageStr = matchResult["page"];
            int startPage = 0;
            int pageStep = 1;

            try
            {
                if ("minid".Equals(pageStr) && objs != null)
                {
                    int min = int.MinValue;
                    foreach (object obj in objs)
                    {
                        if (obj is Collection)
                            min = Math.Min(min, int.Parse(((Collection)obj).idCode.Replace("[^0-9]", "")));
                        else if (obj is Picture)
                            min = Math.Min(min, ((Picture)obj).pid);
                    }
                    page = min;
                }
                else if ("maxid".Equals(pageStr) && objs != null)
                {
                    int max = int.MaxValue;
                    foreach (object obj in objs)
                    {
                        if (obj is Collection)
                            max = Math.Min(max, int.Parse(((Collection)obj).idCode.Replace("[^0-9]", "")));
                        else if (obj is Picture)
                            max = Math.Min(max, ((Picture)obj).pid);
                    }
                    page = max;
                }
                else if (pageStr != null)
                {
                    string[] pageStrs = pageStr.Split(';');
                    if (pageStrs.Length > 1)
                    {
                        pageStep = int.Parse(pageStrs[1]);
                        startPage = int.Parse(pageStrs[0]);
                    }
                    else
                    {
                        pageStep = 1;
                        startPage = int.Parse(pageStr);
                    }
                }
            }
            catch (FormatException) { }

            int realPage = page + (page - startPage) * (pageStep - 1);
            url = Regex.Replace(url, "\\{page:.*?\\}", "" + realPage);
            url = Regex.Replace(url, "\\{keyword:.*?\\}", keyword);
            url = Regex.Replace(url, "\\{idCode:\\}", idCode);
            if (matchResult.ContainsKey("pageStr"))
            {
                url = Regex.Replace(url, "\\{pageStr:(.*?\\{.*?\\}.*?)\\}", (realPage == startPage) ? "" : matchResult["pageStr"]);
            }
            /* TODO
            if (matchResult.ContainsKey("date")
            {
                string dateStr = matchResult["date"];
                int index = dateStr.LastIndexOf(':');
                string firstParam = dateStr.Substring(0, index);
                string lastParam = dateStr.Substring(index + 1);
                DateTime calendar = new DateTime();
                DateTime dateFormat;
                try
                {
                    int offset = int.Parse(lastParam);
                    dateFormat = DateTime.ParseExact(firstParam, "yyyy-MM-dd HH:mm:ss",null);
                    calendar.AddDays(calendar.Day + offset);

                }catch(FormatException e)
                {
                    dateFormat = DateTime.ParseExact(dateStr, "yyyy-MM-dd HH:mm:ss", null);
                }
                string currDate = calendar.ToString();
            }
            if (matchResult.ContainsKey("time")
            {
                string dateStr = matchResult["time"];
                int index = dateStr.LastIndexOf(':');
                string firstParam = dateStr.Substring(0, index);
                string lastParam = dateStr.Substring(index + 1);
                DateTime calendar = new DateTime();
                DateTime dateFormat;
                try
                {
                    int offset = int.Parse(lastParam);
                    dateFormat = DateTime.ParseExact(firstParam, "yyyy-MM-dd HH:mm:ss", null);
                    calendar.AddDays(calendar.Second + offset);

                }
                catch (FormatException e)
                {
                    dateFormat = DateTime.ParseExact(dateStr, "yyyy-MM-dd HH:mm:ss", null);
                }
                string currDate = calendar.ToString();
            }
            */
            return url;
        }

        public static bool IsJson(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            str = str.Trim();
            return str.StartsWith("{") || str.StartsWith("[");
        }

        public static List<Collection> GetCollection(List<Collection> collections, string text, Rule rule, string sourceUrl)
        {
            return GetCollections(collections, text, rule, sourceUrl, false);
        }

        public static List<Collection> GetCollections(List<Collection> collections, string text, Rule rule, string sourceUrl, bool noRegex)
        {
            Document doc = NSoupClient.Parse(text);

            IEnumerable items = doc.Select(rule.item.selector);

            if(!IsJson(text))
            {
                foreach (var item in items)
                {
                    string itemStr;
                    if (item is Element)
                    {
                        if ("attr".Equals(rule.item.fun))
                        {
                            itemStr = ((Element)item).Attr(rule.title.param);
                        }
                        else if ("html".Equals(rule.item.fun))
                        {
                            itemStr = ((Element)item).Html();
                        }
                        else if ("text".Equals(rule.item.fun))
                        {
                            itemStr = ((Element)item).Text();
                        }
                        else
                        {
                            itemStr = ((Element)item).ToString();
                        }
                    }
                    else
                        continue;

                    if (!noRegex && rule.item.regex != null)
                    {
                        var regex = new Regex(rule.item.regex);
                        Match m = regex.Match(itemStr);
                        if (!regex.IsMatch(itemStr))
                        {
                            continue;
                        }
                        else if (m.Groups.Count >= 1)
                        {

                            if (rule.item.replacement != null)
                            {
                                itemStr = rule.item.replacement;
                                for (int i = 1; i < m.Groups.Count; i++)
                                {
                                    string replace = m.Groups[i].Value.ToString();
                                    itemStr = itemStr.Replace(@"\$" + i, (replace != null) ? replace : "");
                                }
                            }
                            else
                            {
                                itemStr = m.Groups[1].Value.ToString();
                            }
                        }
                    }

                    if (rule.item.path != null && IsJson(itemStr))
                    {
                        collections = GetCollections(collections, itemStr, rule, sourceUrl, true);
                    }
                    else
                    {
                        Collection collection = new Collection(collections.Count + 1);
                        collection = GetCollectionDetail(collection, item, rule, sourceUrl);
                        collections.Add(collection);
                    }
                }
            }
            else
            {
                JObject o = JObject.Parse(text);
                var jt = o.SelectTokens(rule.item.path);
                items = jt;
                foreach(object item in items)
                {
                    string itemStr = item.ToString();
                    if(!noRegex && rule.item.regex != null)
                    {
                        var regex = new Regex(rule.item.regex);
                        var m = regex.Match(itemStr);
                        if(!regex.IsMatch(itemStr))
                        {
                            continue;
                        }
                        else if (m.Groups.Count >=1)
                        {
                            if (rule.item.replacement != null)
                            {
                                itemStr = rule.item.replacement;
                                for (int i = 1; i < m.Groups.Count; i++)
                                {
                                    string replace = m.Groups[i].Value.ToString();
                                    itemStr = itemStr.Replace(@"\$" + i, (replace != null) ? replace : "");
                                }
                            }
                            else
                            {
                                itemStr = m.Groups[1].Value.ToString();
                            }
                        }
                    }
                    if (rule.item.path != null && IsJson(itemStr))
                    {
                        collections = GetCollections(collections, itemStr, rule, sourceUrl, true);
                    }
                    else
                    {
                        Collection collection = new Collection(collections.Count + 1);
                        collection = GetCollectionDetail(collection, item, rule, sourceUrl);
                        collections.Add(collection);
                    }
                }
            }
            

            return collections;
        }

        public static Collection GetCollectionDetail(Collection collection,string text, Rule rule, string sourceUrl)
        {
            if (rule == null)
                return collection;
            try
            {
                if (rule.item != null && rule.pictureRule != null && rule.pictureRule.item != null)
                {
                    List<Collection> collections = new List<Collection>();
                    collections.Add(collection);
                    collection = GetCollection(collections, text, rule, sourceUrl)[0];

                }
                else
                {
                    if (!IsJson(text))
                    {
                        var element = NSoupClient.Parse(text);
                        collection = GetCollectionDetail(collection, element, rule, sourceUrl);
                    }
                    else
                    {
                        var elemet = JToken.Parse(text);
                        collection = GetCollectionDetail(collection, elemet, rule, sourceUrl);
                    }
                }
            }
            catch(Exception)
            {

            }

            return collection;
        }

        public static Collection GetCollectionDetail(Collection collection, Object source, Rule rule, string sourceUrl)
        {
            string idCode = ParseSingleProperty(source, rule.idCode, sourceUrl, false);

            string title = ParseSingleProperty(source, rule.title, sourceUrl, false);

            string uploader = ParseSingleProperty(source, rule.uploader, sourceUrl, false);

            string cover = ParseSingleProperty(source, rule.cover, sourceUrl, true);

            string category = ParseSingleProperty(source, rule.category, sourceUrl, false);

            string datetime = ParseSingleProperty(source, rule.datetime, sourceUrl, false);

            string description = ParseSingleProperty(source, rule.rating, sourceUrl, false);

            if (source is Element)
            {
                try
                {
                    Element element = NSoupClient.Parse(description);
                    element.Select("iframe").Remove();
                    element.Select("script").Remove();
                    description = element.Select("body").Html();
                }
                catch (Exception) { }
            }

            string ratingStr = ParseSingleProperty(source, rule.rating, sourceUrl, false);

            float rating;

            if (Regex.IsMatch(ratingStr, "\\d+(.\\d+)?") && ratingStr.IndexOf(".") > 0)
            {
                rating = float.Parse(ratingStr);
            }
            else if (StringUtil.IsNumeric(ratingStr))
            {
                rating = float.Parse(ratingStr);
            }
            else
            {
                string result = new MathUtil().RunExpress(ratingStr);
                try
                {
                    rating = result.Contains("NaN") ? 0 : float.Parse(result);
                }
                catch (FormatException)
                {
                    rating = Math.Min(ratingStr.Replace(" ", "").Length, 5);
                }
            }

            IEnumerable temp = null;

            List<Tag> tags = new List<Tag>();
            if (rule.tagRule != null && rule.tagRule.item != null)
            {
                if (source is Element)
                {
                    temp = ((Element)source).Select(rule.tagRule.item.selector);
                }
                else if (source is JToken)
                {
                    JObject o = JObject.Parse(source.ToString());
                    var jt = o.SelectTokens(rule.tagRule.item.path);
                    if (jt is JArray)
                    {
                        temp = jt.ToList();
                    }
                    else
                    {
                        temp = jt;
                    }
                }
                else
                    return collection;

                foreach (object element in temp)
                {
                    if (rule.tagRule.item.regex != null)
                    {
                        var r = new Regex(rule.tagRule.item.regex);
                        var mc = r.Matches(element.ToString());
                        if (mc.Count == 0)
                            continue;
                    }
                    string tagTitle = ParseSingleProperty(element, rule.tagRule.title, sourceUrl, false);
                    string tagUrl = ParseSingleProperty(element, rule.tagRule.url, sourceUrl, true);

                    if (string.IsNullOrEmpty(tagUrl))
                    {
                        tagUrl = null;
                    }
                    tags.Add(new Tag(tags.Count + 1, tagTitle, tagUrl));
                }
            }
            else if (rule.tags != null)
            {
                List<string> tagStrs = ParseSinglePropertyMatchAll(source, rule.tags, sourceUrl, false);
                foreach (string tagStr in tagStrs)
                {
                    if (!string.IsNullOrEmpty(tagStr))
                    {
                        tags.Add(new Tag(tags.Count + 1, tagStr));
                    }
                }
            }

            List<Picture> pictures = new List<Picture>();

            Selector pictureId = null, pictureItem = null, pictureThumbnail = null, pictureUrl = null, pictureHighRes = null;
            if (rule.pictureRule != null && rule.pictureRule.url != null && rule.pictureRule.thumbnail != null)
            {
                pictureId = rule.pictureRule.id;
                pictureItem = rule.pictureRule.item;
                pictureThumbnail = rule.pictureRule.thumbnail;
                pictureUrl = rule.pictureRule.url;
                pictureHighRes = rule.pictureRule.highRes;
            }
            else if (rule.pictureUrl != null && rule.pictureThumbnail != null)
            {
                pictureId = rule.pictureId;
                pictureItem = rule.item;
                pictureThumbnail = rule.pictureThumbnail;
                pictureUrl = rule.pictureUrl;
                pictureHighRes = rule.pictureHighRes;
            }

            if (pictureUrl != null && pictureThumbnail != null)
            {
                if (pictureItem != null)
                {
                    if (source is Element)
                    {
                        temp = ((Element)source).Select(pictureItem.selector);
                    }
                }
                else if (source is JToken)
                {
                    JObject o = JObject.Parse(source.ToString());
                    var jt = o.SelectTokens(pictureItem.path);

                    if (jt is JArray)
                    {
                        temp = jt.ToList();
                    }
                    else
                    {
                        temp = jt;
                    }
                }
                else
                {
                    return collection;
                }

                foreach (object element in temp)
                {
                    if (pictureItem.regex != null)
                    {
                        var r = new Regex(pictureItem.regex);
                        var mc = r.Matches(element.ToString());
                        if (mc.Count == 0)
                        {
                            continue;
                        }
                    }
                    string pId = ParseSingleProperty(element, pictureId, sourceUrl, false);
                    int pid;
                    try
                    {
                        pid = int.Parse(pId);
                    }
                    catch (Exception)
                    {
                        pid = 0;
                    }
                    pid = (pid != 0) ? pid : (pictures.Count > 0) ? pictures[pictures.Count - 1].pid + 1 : pictures.Count + 1;
                    string pUrl = ParseSingleProperty(element, pictureUrl, sourceUrl, true);
                    string PHighRes = ParseSingleProperty(element, pictureThumbnail, sourceUrl, true);
                    string pThumbnail = ParseSingleProperty(element, pictureThumbnail, sourceUrl, true);
                    pictures.Add(new Picture(pid, pUrl, pThumbnail, PHighRes, sourceUrl));
                }
            }
            else
            {
                List<string> pids = ParseSinglePropertyMatchAll(source, pictureId, sourceUrl, false);
                List<string> urls = ParseSinglePropertyMatchAll(source, pictureUrl, sourceUrl, true);
                List<string> thumbnails = ParseSinglePropertyMatchAll(source, pictureThumbnail, sourceUrl, true);
                List<string> highReses = ParseSinglePropertyMatchAll(source, pictureHighRes, sourceUrl, true);
                for (int i = 0; i < urls.Count; i++)
                {
                    string pId = (i < pids.Count) ? pids[i] : "";
                    int pid;
                    try
                    {
                        pid = int.Parse(pId);
                    }
                    catch (Exception)
                    {
                        pid = 0;
                    }
                    pid = (pid != 0) ? pid : (pictures.Count > 0) ? pictures[pictures.Count - 1].pid + 1 : pictures.Count + 1;
                    string url = urls[i];
                    string thumbnail = (i < thumbnails.Count) ? thumbnails[i] : "";
                    string highRes = (i < highReses.Count) ? highReses[i] : "";
                    pictures.Add(new Picture(pid, url, thumbnail, highRes, sourceUrl));
                }
            }

            List<Video> videos = new List<Video>();

            if (rule.videoRule != null && rule.videoRule.item != null)
            {
                if (source is Element)
                {
                    temp = ((Element)source).Select(rule.videoRule.item.selector);
                }
                else if (source is JToken)
                {
                    JObject o = JObject.Parse(source.ToString());
                    var jt = o.SelectTokens(pictureItem.path);

                    if (jt is JArray)
                    {
                        temp = jt.ToList();
                    }
                    else
                    {
                        temp = jt;
                    }
                }
                else
                {
                    return collection;
                }

                foreach (object element in temp)
                {
                    if (rule.videoRule.item.regex != null)
                    {
                        var r = new Regex(rule.videoRule.item.regex);
                        var mc = r.Matches(element.ToString());
                        if (mc.Count == 0)
                        {
                            continue;
                        }
                    }
                    string vId = ParseSingleProperty(element, rule.videoRule.id, sourceUrl, false);
                    int vid;
                    try
                    {
                        vid = int.Parse(vId);
                    }
                    catch (Exception)
                    {
                        vid = 0;
                    }
                    vid = (vid != 0) ? vid : (videos.Count > 0) ? videos[videos.Count - 1].vid + 1 : videos.Count + 1;
                    string vThumbnail = ParseSingleProperty(element, rule.videoRule.thumbnail, sourceUrl, true);
                    if (string.IsNullOrEmpty(vThumbnail))
                    {
                        vThumbnail = (string.IsNullOrEmpty(cover)) ? collection.cover : cover;
                    }
                    string vContent = ParseSingleProperty(element, rule.videoRule.content, sourceUrl, true);
                    videos.Add(new Video(vid, vThumbnail, vContent));
                }
            }

            Selector commentItem = null, commentAvatar = null, commentAuthor = null, commentDatetime = null, commentContent = null;
            List<Model.Comment> comments = new List<Model.Comment>();
            if (rule.commentRule != null && rule.commentRule.item != null && rule.commentRule.content != null)
            {
                commentItem = rule.commentRule.item;
                commentAvatar = rule.commentRule.avatar;
                commentAuthor = rule.commentRule.author;
                commentDatetime = rule.commentRule.datetime;
                commentContent = rule.commentRule.content;
            }
            else if (rule.commentItem != null && rule.commentContent != null)
            {
                commentItem = rule.commentItem;
                commentAvatar = rule.commentAvatar;
                commentAuthor = rule.commentAuthor;
                commentDatetime = rule.commentDatetime;
                commentContent = rule.commentContent;
            }
            if (pictureUrl != null && pictureThumbnail != null)
            {
                if (pictureItem != null)
                {
                    if (source is Element)
                    {
                        temp = ((Element)source).Select(pictureItem.selector);
                    }
                }
                else if (source is JToken)
                {
                    JObject o = JObject.Parse(source.ToString());
                    var jt = o.SelectTokens(pictureItem.path);

                    if (jt is JArray)
                    {
                        temp = jt.ToList();
                    }
                    else
                    {
                        temp = jt;
                    }
                }
                else
                {
                    return collection;
                }

                foreach (object element in temp)
                {
                    if (pictureItem.regex != null)
                    {
                        var r = new Regex(pictureItem.regex);
                        var mc = r.Matches(element.ToString());
                        if (mc.Count == 0)
                        {
                            continue;
                        }
                    }
                    string cAvatar = ParseSingleProperty(element, commentAvatar, sourceUrl, false);
                    string cAuthor = ParseSingleProperty(element, commentAuthor, sourceUrl, false);
                    string cDatetime = ParseSingleProperty(element, commentDatetime, sourceUrl, false);
                    string cContent = ParseSingleProperty(element, commentContent, sourceUrl, false);
                    comments.Add(new Model.Comment(comments.Count + 1, cAvatar, cAuthor, cDatetime, cContent, sourceUrl));
                }
            }

            if (!string.IsNullOrEmpty(idCode))
                collection.idCode = idCode;
            if (!string.IsNullOrEmpty(title))
                collection.title = title;
            if (!string.IsNullOrEmpty(uploader))
                collection.uploader = uploader;
            if (!string.IsNullOrEmpty(cover))
                collection.cover = cover;
            if (!string.IsNullOrEmpty(category))
                collection.category = category;
            if (!string.IsNullOrEmpty(datetime))
                collection.datetime = datetime;
            if (!string.IsNullOrEmpty(description))
                collection.description = description;
            if (rating > 0)
                collection.rating = rating;
            if (!string.IsNullOrEmpty(sourceUrl))
                collection.referer = sourceUrl;
            if (tags != null && tags.Count > 0)
                collection.tags = tags;
            if (pictures != null && pictures.Count > 0)
                collection.pictures = pictures;
            if (videos != null && videos.Count > 0)
                collection.videos = videos;
            if (comments != null && comments.Count > 0)
                collection.comments = comments;
            return collection;

        }

        public static string ParseSingleProperty(object source,Selector selector,string sourceUrl,bool isUrl)
        {
            List<string> props = ParseSinglePropertyMatchAll(source, selector, sourceUrl, isUrl);
            return (props.Count > 0) ? props[0] : "";
        }

        public static List<string> ParseSinglePropertyMatchAll(object source,Selector selector,string sourceUrl,bool isUrl)
        {
            List<string> props = new List<string>();

            if(selector != null)
            {
                string prop;
                if(source is Element)
                {
                    var temp = ("this".Equals(selector.selector)) ? new Elements((Element)source) : ((Element)source).Select(selector.selector);
                    if(temp != null)
                    {
                        bool doJsonParse = !string.IsNullOrEmpty(selector.path);
                        foreach(var elem in temp)
                        {
                            if ("attr".Equals(selector.fun))
                            {
                                prop = elem.Attr(selector.param);
                            }
                            else if ("html".Equals(selector.fun))
                            {
                                prop = elem.Html();
                            }
                            else if("text".Equals(selector.fun))
                            {
                                prop = elem.Text();
                            }
                            else
                            {
                                prop = elem.ToString();
                            }


                            if (doJsonParse)
                            {
                                props = GetPropertyAfterRegex(props, prop, selector, sourceUrl, false);
                            }
                            else
                            {
                                props = GetPropertyAfterRegex(props, prop, selector, sourceUrl, isUrl);
                            }
                        }

                        if(doJsonParse)
                        {
                            try
                            {
                                for(int i =0; i< props.Count;i++)
                                {
                                    prop = props[i];
                                    object tempItem = JToken.Parse(prop).SelectToken(selector.path);
                                    if (tempItem is JValue)
                                        prop = ((JValue)tempItem).ToString();
                                    else
                                        prop = tempItem.ToString();
                                    if(!string.IsNullOrEmpty(prop))
                                    {
                                        if(isUrl)
                                            prop = RegexValidateUtil.getAbsoluteUrlFromRelative(prop, sourceUrl);

                                        props[i] = prop;
                                    }
                                }
                            }
                            catch(Exception)
                            {

                            }
                        }
                    }
                }
                else if(source is JToken)
                {
                    List<JToken> temp = new List<JToken>();

                    try
                    {
                        var elem = ((JToken)source).SelectTokens(selector.path);
                        temp = elem.ToList();
                        
                    }
                    catch(Exception)
                    {

                    }

                    if (temp != null)
                    {
                        foreach (JToken item in temp)
                        {
                            prop = item.ToString();

                            if (!string.IsNullOrEmpty(selector.selector))
                            {
                                try
                                {
                                    string newProp;
                                    var element = ("this".Equals(selector.selector)) ? new Elements(NSoupClient.Parse(prop)) : NSoupClient.Parse(prop).Select(selector.selector);
                                    if (element != null)
                                    {
                                        foreach (var elem in element)
                                        {
                                            if ("attr".Equals(selector.fun))
                                            {
                                                newProp = elem.Attr(selector.param);
                                            }
                                            else if ("html".Equals(selector.fun))
                                            {
                                                newProp = elem.Html();
                                            }
                                            else if ("text".Equals(selector.fun))
                                            {
                                                newProp = elem.Text();
                                            }
                                            else
                                            {
                                                newProp = elem.ToString();
                                            }
                                            if (!string.IsNullOrEmpty(newProp))
                                                prop = newProp;
                                        }
                                    }
                                }
                                catch (Exception)
                                {

                                }

                            }
                            if (!string.IsNullOrEmpty(prop) && !"null".Equals(prop.Trim()))
                                props = GetPropertyAfterRegex(props, prop, selector, sourceUrl, isUrl);
                        }
                    }
                }
            }
            if (props.Count == 0)
                props.Add("");
            return props;
        }

        public static List<string> GetPropertyAfterRegex(List<string> props, string prop, Selector selector, string sourceUrl, bool isUrl)
        {
            if (selector.regex != null)
            {
                var r = new Regex(selector.regex);
                

                if(r.IsMatch(prop))
                {
                    var m = r.Match(prop);

                    do
                    {
                        if (selector.replacement != null)
                        {
                            prop = selector.replacement;
                            for (int i = 1; i <= m.Groups.Count; i++)
                            {
                                string replace = m.Groups[i].Value;
                                prop = prop.Replace("$" + i, (replace != null) ? replace : "");
                            }
                        }
                        else
                        {
                            prop = m.Groups[1].Value;
                        }
                        if (isUrl)
                        {
                            if (string.IsNullOrEmpty(prop))
                                break;
                            prop = RegexValidateUtil.getAbsoluteUrlFromRelative(prop, sourceUrl);
                        }
                        props.Add(System.Net.WebUtility.HtmlEncode(prop.Trim()));

                        m = m.NextMatch();
                    } while ((m.Success && m.Groups.Count >= 1));
                    
                }


            }
            else
            {
                if(isUrl && !string.IsNullOrEmpty(prop))
                {
                    prop = RegexValidateUtil.getAbsoluteUrlFromRelative(prop, sourceUrl);
                }
                props.Add(System.Net.WebUtility.HtmlEncode(prop.Trim()));
            }

            return props;

        }

        public static string GetPictureUrl(string text, Selector selector, string sourceUrl)
        {
            try
            {
                if(!IsJson(text))
                {
                    Document doc = NSoupClient.Parse(text);
                    return ParseSingleProperty(doc, selector, sourceUrl, true);
                }
                else
                {
                    JToken jt = JToken.Parse(text);
                    return ParseSingleProperty(jt, selector, sourceUrl, true);
                }
            }
            catch (Exception)
            {
                return "";
            }
        }

        
        public static List<string> GetVideoUrl(string html,string sourceUrl)
        {
            List<string> videoUrls = new List<string>();
            try
            {
                Regex r = new Regex("https?[^\"'<>]*?[^\"' <>]+?\\.(?: mp4 | flv)[^\"'<>]*");
                
                
                while(r.IsMatch(html))
                {
                    string videoUrl = r.Match(html).Value;
                    if (string.IsNullOrEmpty(videoUrl))
                        continue;
                    videoUrl = RegexValidateUtil.getAbsoluteUrlFromRelative(videoUrl, sourceUrl);
                    videoUrls.Add(videoUrl);
                }
            }
            catch (Exception)
            {
                
            }
            return videoUrls;
        }
        

    }
}
