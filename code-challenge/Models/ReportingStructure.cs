using System;

namespace challenge.Models
{
    public class ReportingStructure
    {
        public String ReportingStructureId { get; set; }

        public Employee Employee { get; set; }

        public int NumberOfReports { get; set; }
    }
}
