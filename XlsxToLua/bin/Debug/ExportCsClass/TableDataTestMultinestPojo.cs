using System;
using System.Collections.Generic;

namespace Assets.Scripts.CsClass
{
    public class TableDataTestMultinestPojo
    {
        public int Id { get; set; }
        public Dictionary<string, object> TestDict { get; set; }
        public List<Dictionary<string, object>> TestArray1 { get; set; }
        public List<List<Dictionary<string, object>>> TestArray2 { get; set; }
    }
}
