using System;
using System.Runtime.Serialization;

namespace ZafiraIntegration.models
{
    [DataContract]
    public class TestArtifactType
    {
        [DataMember]
        public String name;
        [DataMember]
        public String link;
        [DataMember]
        public long? testId;
        //[DataMember]
        //public DateTime expiresAt;

        public TestArtifactType(String name, String link)
        {
            this.name = name;
            this.link = link;
        }
    }
}
