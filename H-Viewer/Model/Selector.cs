using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HViewer.Model
{
    public class Selector
    {
        public String selector, path, fun, param, regex, replacement;

        public Selector()
        {
        }

        public Selector(String selector, String fun, String param, String regex, String replacement)
        {
            this.selector = selector;
            this.fun = fun;
            this.param = param;
            this.regex = regex;
            this.replacement = replacement;
        }

        public Selector(String path, String fun, String param, String regex, String replacement, bool isJson)
        {
            if (isJson)
                this.path = path;
            else
                this.selector = path;
            this.fun = fun;
            this.param = param;
            this.regex = regex;
            this.replacement = replacement;
        }
    }
}
