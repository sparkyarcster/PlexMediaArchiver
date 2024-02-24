using System;
using System.Collections.Generic;
using System.Text;

namespace PMAData.Model
{
    public class GenericData
    {
        public Guid ID { get; set; }
        public int MediaID { get; set; }
        public string MediaType { get; set; }
        public string DataKey { get; set; }
        public string DataValue { get; set; }
    }
}
