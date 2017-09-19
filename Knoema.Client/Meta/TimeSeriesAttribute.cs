using System.Collections.Generic;

namespace Knoema.Meta
{
    public class TimeSeriesAttribute
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public IEnumerable<string> AvailableValues { get; set; }
    }
}
