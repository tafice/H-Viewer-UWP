using HViewer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HViewer.Model
{
    public class Site
    {
        public const string FLAG_NO_COVER = "noCover";
        public const string FLAG_NO_TITLE = "noTitle";
        public const string FLAG_NO_RATING = "noRating";
        public const string FLAG_NO_TAG = "noTag";
        public const string FLAG_WATERFALL_AS_LIST = "waterfallAsList";
        public const string FLAG_WATERFALL_AS_GRID = "waterfallAsGrid";
        public const string FLAG_SECOND_LEVEL_GALLERY = "secondLevelGallery";
        public const string FLAG_REPEATED_THUMBNAIL = "repeatedThumbnail";
        public const string FLAG_SINGLE_PAGE_BIG_PICTURE = "singlePageBigPicture";
        public const string FLAG_PRELOAD_GALLERY = "preloadGallery";
        public const string FLAG_ONE_PIC_GALLERY = "onePicGallery";
        public const string FLAG_EXTRA_INDEX_INFO = "extraIndexInfo";
        public const string FLAG_JS_NEEDED_ALL = "jsNeededAll";
        public const string FLAG_JS_NEEDED_INDEX = "jsNeededIndex";
        public const string FLAG_JS_NEEDED_GALLERY = "jsNeededGallery";
        public const string FLAG_JS_NEEDED_PICTURE = "jsNeededPicture";
        public const string FLAG_JS_SCROLL = "jsScroll";
        public const string FLAG_IFRAME_GALLERY = "iframeGallery";
        public const string FLAG_POST_ALL = "postAll";
        public const string FLAG_POST_INDEX = "postIndex";
        public const string FLAG_POST_GALLERY = "postGallery";
        public const string FLAG_POST_PICTURE = "postPicture";

        public int sid, gid;
        public string title = "";
        public string indexUrl = "", galleryUrl = "", searchUrl = "", loginUrl = "";
        public List<Category> categories;
        public Rule indexRule, galleryRule, searchRule, extraRule;
        public int versionCode;

        public Selector picUrlSelector;

        public string cookie = "";
        public string header = "";
        public string flag = "";
        public int index;
        public bool isGrid = false;
        public bool disableHProxy = false;

        public Site()
        {
        }

        public Site(int sid, string title, string indexUrl, string galleryUrl, string searchUrl, string loginUrl,
                    Rule indexRule, Rule galleryRule, Rule searchRule, Rule extraRule, string flag)
        {
            this.sid = sid;
            this.title = title;
            this.indexUrl = indexUrl;
            this.galleryUrl = galleryUrl;
            this.searchUrl = searchUrl;
            this.loginUrl = loginUrl;
            this.indexRule = indexRule;
            this.galleryRule = galleryRule;
            this.searchRule = searchRule;
            this.extraRule = extraRule;
            this.flag = flag;
        }

        public void setCategories(List<Category> categories)
        {
            this.categories = categories;
        }

        public void setGroupId(int gid)
        {
            this.gid = gid;
        }


    public int getId()
        {
            return sid;
        }

    public long getChildId()
        {
            return sid;
        }

    public string getText()
        {
            return title;
        }


        public List<KeyValuePair<string, string>> getHeaders()
        {
            List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string,string>>();
            if (!string.IsNullOrEmpty(cookie))
                headers.Add(new KeyValuePair<string,string>("cookie", cookie));
            if (!string.IsNullOrEmpty(header))
            {
                var r = new Regex("([^\\r\\n]*?):([^\\r\\n]*)");
                var m = r.Match(header);
                while (r.IsMatch(header) && m.Groups.Count == 2)
                {
                    headers.Add(new KeyValuePair<string,string>(m.Groups[1].Value, m.Groups[2].Value));
                }
            }
            return headers;
        }

        public bool hasFlag(string flag)
        {
            if (this.flag == null)
                return false;
            else
                return this.flag.Contains(flag);
        }

        public string getListUrl(string url, int page, string keyword, List<Collection> collections)
        {
            Object[] array = (collections != null) ? collections.ToArray() : null;
            return RuleParser.ParseUrl(url, page, "", keyword, array);
        }

        public string getGalleryUrl(string idCode, int page, List<Picture> pictures)
        {
            Object[] array = (pictures != null) ? pictures.ToArray() : null;
            return RuleParser.ParseUrl(galleryUrl, page, idCode, "", array);
        }

        
    }
}
