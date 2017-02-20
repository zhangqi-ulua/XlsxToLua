using System;
using System.Collections.Generic;

namespace Assets.Scripts.CsvClass
{
    public class SystemData
    {
        public int systemId { get; set; }
        public string systemName { get; set; }
        public string help { get; set; }
        public Dictionary<string, object> openCondition { get; set; }
        public List<Dictionary<string, object>> openRewards { get; set; }
    }
}
