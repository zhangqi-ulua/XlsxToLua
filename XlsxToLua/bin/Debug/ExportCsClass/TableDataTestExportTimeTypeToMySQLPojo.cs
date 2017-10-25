using System;
using System.Collections.Generic;

namespace Assets.Scripts.CsClass
{
    public class TableDataTestExportTimeTypeToMySQLPojo
    {
        public int Id { get; set; }
        public DateTime TestExportDateToDatetime { get; set; }
        public DateTime TestExportDateToReferenceSec { get; set; }
        public DateTime TestExportDateToReferenceMsec { get; set; }
        public DateTime TestExportDateToDate { get; set; }
        public DateTime TestExportDateToVarchar { get; set; }
        public DateTime TestExportTimeToTime { get; set; }
        public DateTime TestExportTimeToReferenceSec { get; set; }
        public DateTime TestExportTimeToVarchar { get; set; }
    }
}
