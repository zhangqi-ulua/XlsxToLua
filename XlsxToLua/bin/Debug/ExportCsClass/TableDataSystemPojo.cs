using System;
using System.Collections.Generic;

namespace Assets.Scripts.CsClass
{
    public class TableDataSystemPojo
    {
        public int systemId { get; set; }
        public string systemName { get; set; }
        public string help { get; set; }
        public Dictionary<string, object> openCondition { get; set; }
        public List<Dictionary<string, object>> openRewards { get; set; }
    }
}
