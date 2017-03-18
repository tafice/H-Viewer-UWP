using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HViewer.Model
{
    public class Category
    {

        public int cid;
        public string title, url;

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
