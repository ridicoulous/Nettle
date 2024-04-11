using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nettle
{
    public class CsvRecord
    {
        public string Name { get; set; }
        public string Time { get; set; }
        public string Description { get; set; }
        public string Coords { get; set; }
        public double[] LatLon { get; set; }
    }

    public class CsvRecordMap : CsvHelper.Configuration.ClassMap<CsvRecord>
    {
        [RequiresUnreferencedCode("CsvHelper is not trim compatible")]
        public CsvRecordMap()
        {
            Map(m => m.Name).Name("name");
            Map(m => m.Time).Name("time");
            Map(m => m.Description).Name("description");
            Map(m => m.Coords).Name("coords");
        }
    }
}
