using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HViewer.Model
{
    public class Collection
    {
        public int cid;
        public string idCode = "";
        public string title = "", uploader = "", cover = "", category = "", datetime = "";
        public string description;
        public float rating;
        public string referer;
        public List<Tag> tags;
        public List<Picture> pictures;
        public List<Video> videos;
        public List<Comment> comments;
        public bool preloaded = false;

        public Collection(int cid)
        {
            this.cid = cid;
        }

        public Collection(int cid, string idCode, string title, string uploader, string cover, string category,
                          string datetime, string description, float rating, string referer, List<Tag> tags,
                          List<Picture> pictures, List<Video> videos, List<Comment> comments, bool preloaded)
        {
            this.cid = cid;
            this.idCode = idCode;
            this.title = title;
            this.uploader = uploader;
            this.cover = cover;
            this.category = category;
            this.datetime = datetime;
            this.description = description;
            this.rating = rating;
            this.referer = referer;
            this.tags = tags;
            this.pictures = pictures;
            this.comments = comments;
            this.preloaded = preloaded;
        }

    public int getId()
        {
            return cid;
        }

public override bool Equals(object obj)
{
    if (obj is Picture)
    {
        Collection item = (Collection)obj;

        if (string.Equals(this.idCode, item.idCode) &&
            string.Equals(this.title, item.title) &&
            string.Equals(this.uploader, item.uploader) &&
            string.Equals(this.cover, item.cover) &&
            string.Equals(this.category, item.category) &&
            string.Equals(this.datetime, item.datetime) &&
            object.Equals(this.rating, item.rating))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    else
    {
        return false;
    }
}

public new bool Equals(object obj1, object obj2)
{
    if (obj1 == obj2)
    {
        return true;
    }
    if (obj1 == null || obj2 == null)
    {
        return false;
    }
    return obj1.Equals(obj2);
}

public long getChildId()
{
    return cid;
}

    public string getText()
{
    return title;
}
    }
}
