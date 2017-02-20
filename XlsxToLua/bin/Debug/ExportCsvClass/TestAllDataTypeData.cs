using System;
using System.Collections.Generic;

namespace Assets.Scripts.CsvClass
{
    public class TestAllDataTypeData
    {
        public int id { get; set; }
        public bool testBool { get; set; }
        public string testLang { get; set; }
        public long testLong { get; set; }
        public Dictionary<string, object> testSimpleDict { get; set; }
        public Dictionary<string, object> testDictIncludeArray { get; set; }
        public float testFloat { get; set; }
        public string testString { get; set; }
        public List<string> testSimpleArray { get; set; }
        public List<string> testSimpleArray2 { get; set; }
        public Dictionary<int ,bool> testTableString1 { get; set; }
        public List<List<string>> testArrayIncludeArray { get; set; }
        public List<int> testTableString2 { get; set; }
        public List<Dictionary<string, object>> testArrayIncludeDict { get; set; }
        public Dictionary<string, object> testDictIncludeDict { get; set; }
        public List<Dictionary<string, object>> testTableString3 { get; set; }
        public DateTime testDate1 { get; set; }
        public DateTime testDate2 { get; set; }
        public DateTime testDate3 { get; set; }
        public DateTime testDate4 { get; set; }
        public DateTime testTime1 { get; set; }
        public DateTime testTime2 { get; set; }
        public DateTime testTime3 { get; set; }
        public Dictionary<string, object> testExportJsonSpecialString { get; set; }
    }
}
