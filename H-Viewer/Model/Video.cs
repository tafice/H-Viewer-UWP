using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HViewer.Model
{
    public class Video
    {
        public int vid;
        public String thumbnail, content;
        public String vlink;
        public int status = DownloadItemStatus.STATUS_WAITING;
        public int percent = 0;
        public int retries;

        public Video(int vid, String thumbnail, String content)
        {
            this.vid = vid;
            this.thumbnail = thumbnail;
            this.content = content;
        }

        public int getId()
        {
            return vid;
        }
    }
}
