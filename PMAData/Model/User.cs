using System;
using System.Collections.Generic;
using System.Text;

namespace PMAData.Model
{
    public class User
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastActivity { get; set; }
        public List<GenericData> GenericData { get; set; }

        public User()
        {
            GenericData = new List<GenericData>();
        }
    }
}
