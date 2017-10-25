using System;
using System.Collections.Generic;

namespace Assets.Scripts.CsClass
{
    public class TableDataSystemPojo
    {
        public int SystemId { get; set; }
        public string SystemName { get; set; }
        public string Help { get; set; }
        public Dictionary<string, object> OpenCondition { get; set; }
        public List<Dictionary<string, object>> OpenRewards { get; set; }
    }
}
