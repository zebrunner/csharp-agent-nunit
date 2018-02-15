using System;
using System.Runtime.Serialization;

namespace ZafiraIntegration.models
{
    [DataContract]
    public class ProjectType : AbstractType
    {
        [DataMember]
        public String name { get; set; }
        [DataMember]
        public String description { get; set; }
    }
}
