using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HViewer.Model
{
    public class Category
    {

        public int cid { get; set; }
        public string title { get; set; }
        public string url { get; set; }

    public Category(int cid, string title, string url)
        {
            this.cid = cid;
            this.title = title;
            this.url = url;
        }

        public int getId()
        {
            return cid;
        }
    }
}
