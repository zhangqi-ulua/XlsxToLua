using System;
using System.Collections.Generic;

namespace Assets.Scripts.CsvClass
{
    public class TestExportTimeTypeToMySQLData
    {
        public int id { get; set; }
        public DateTime testExportDateToDatetime { get; set; }
        public DateTime testExportDateToReferenceSec { get; set; }
        public DateTime testExportDateToReferenceMsec { get; set; }
        public DateTime testExportDateToDate { get; set; }
        public DateTime testExportDateToVarchar { get; set; }
        public DateTime testExportTimeToTime { get; set; }
        public DateTime testExportTimeToReferenceSec { get; set; }
        public DateTime testExportTimeToVarchar { get; set; }
    }
}
