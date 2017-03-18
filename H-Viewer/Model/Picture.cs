using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HViewer.Model
{
    public class Picture
    {
        public int pid;
        public string thumbnail, url, pic, highRes;
        public int retries;
        public int status = DownloadItemStatus.STATUS_WAITING;
        public string referer;
        public bool loadedHighRes;

        public Picture(int pid, string url, string thumbnail, string highRes, string referer)
        {
            this.pid = pid;
            this.url = url;
            this.thumbnail = thumbnail;
            this.highRes = highRes;
            this.referer = referer;
        }

        public int getId()
        {
            return pid;
        }

        public override bool Equals(object obj)
        {
            if (obj is Picture)
            {
                Picture item = (Picture)obj;

                if (string.Equals(this.thumbnail, item.thumbnail) &&
                    string.Equals(this.url, item.url)
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

        public static bool hasPicPosfix(String url)
        {
            return url != null && (url.EndsWith(".jpg") || url.EndsWith(".png") || url.EndsWith(".bmp") || url.EndsWith(".gif") || url.EndsWith(".webp"));
        }
    }
}
