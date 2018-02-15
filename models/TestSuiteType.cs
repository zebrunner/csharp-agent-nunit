using System;
using System.Runtime.Serialization;

namespace ZafiraIntegration.models
{
    [DataContract]
    public class TestSuiteType : AbstractType
    {
        [DataMember]
        public String name { get; set; }
        [DataMember]
        public String fileName { get; set; }
        [DataMember]
        public String description { get; set; }
        [DataMember]
        public long userId { get; set; }

        public TestSuiteType() { }

        public TestSuiteType(String name, String fileName, long userId)
        {
            this.name = name;
            this.userId = userId;
            this.fileName = fileName;
        }
    }
}
