using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HViewer.Model
{
    public class Tag
    {
        public int tid;
        public string title = "";
        public string url;
        public bool selected = false;

        public Tag(int tid, string title)
        {
            this.tid = tid;
            this.title = title;
        }
        public Tag(int tid, string title, string url)
        {
            this.tid = tid;
            this.title = title;
            this.url = url;
        }

        public int getId()
        {
            return tid;
        }
    }
}

