using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CS.WebUI.Models.SR
{
    public class ChartModel
    {
        public List<string> legend { get; set; }
        public List<string> xAxis { get; set; }
        public List<Serie> series { get; set; }
    }

    public class Serie
    {
        public string name { get; set; }
        public string type { get; set; }
        public string stack { get; set; }
        public List<int> data { get; set; }
    }
}