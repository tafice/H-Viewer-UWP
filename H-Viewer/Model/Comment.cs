using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HViewer.Model
{
    public class Comment
    {
        public int cid { get; set; }
        public string avatar { get; set; }
        public string author { get; set; }
        public string datetime { get; set; }
        public string content { get; set; }
        public string referer { get; set; }

        public Comment(int cid, string avatar, string author, string datetime, string content, string referer)
        {
            this.cid = cid;
            this.avatar = avatar;
            this.author = author;
            this.datetime = datetime;
            this.content = content;
            this.referer = referer;
        }

        public int getId()
        {
            return cid;
        }

        public override bool Equals(object obj)
        {
            if (obj is Comment)
            {
                Comment item = (Comment)obj;

                if (string.Equals(this.author, item.author) &&
                    string.Equals(this.datetime, item.datetime)
                    )
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

        public bool equals(Object obj1, Object obj2)
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
    }
}
