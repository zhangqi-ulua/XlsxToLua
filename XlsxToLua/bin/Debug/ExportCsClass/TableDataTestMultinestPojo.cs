using System;
using System.Collections.Generic;

namespace Assets.Scripts.CsClass
{
    public class TableDataTestMultinestPojo
    {
        public int id { get; set; }
        public Dictionary<string, object> testDict { get; set; }
        public List<Dictionary<string, object>> testArray1 { get; set; }
        public List<List<Dictionary<string, object>>> testArray2 { get; set; }
    }
}
