using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentenceCalculatorPlugin.Model
{
    public class MarqueeSettingEntity
    {

        public bool marqueeVisible { get; set; }
        public string marqueeText { get; set; }
        public string marqueeLinkText { get; set; }
        public string marqueeLink { get; set; }



    }
}
