using System;
using System.Collections.Generic;

namespace Assets.Scripts.CsClass
{
    public class TableDataTestAllDataTypePojo
    {
        public int Id { get; set; }
        public bool TestBool { get; set; }
        public string TestLang { get; set; }
        public long TestLong { get; set; }
        public Dictionary<string, object> TestSimpleDict { get; set; }
        public Dictionary<string, object> TestDictIncludeArray { get; set; }
        public float TestFloat { get; set; }
        public string TestString { get; set; }
        public List<string> TestSimpleArray { get; set; }
        public List<string> TestSimpleArray2 { get; set; }
        public Dictionary<int ,bool> TestTableString1 { get; set; }
        public List<List<string>> TestArrayIncludeArray { get; set; }
        public List<int> TestTableString2 { get; set; }
        public List<Dictionary<string, object>> TestArrayIncludeDict { get; set; }
        public Dictionary<string, object> TestDictIncludeDict { get; set; }
        public List<Dictionary<string, object>> TestTableString3 { get; set; }
        public DateTime TestDate1 { get; set; }
        public DateTime TestDate2 { get; set; }
        public DateTime TestDate3 { get; set; }
        public DateTime TestDate4 { get; set; }
        public DateTime TestTime1 { get; set; }
        public DateTime TestTime2 { get; set; }
        public DateTime TestTime3 { get; set; }
        public Dictionary<string, object> TestExportJsonSpecialString { get; set; }
        public LitJson.JsonData TestJson { get; set; }
    }
}
