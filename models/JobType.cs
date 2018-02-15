using System;
using System.Runtime.Serialization;

namespace ZafiraIntegration.models
{
    [DataContract]
    public class JobType : AbstractType
    {
        [DataMember]
        public String name { get; set; }

        [DataMember]
        public String jobURL { get; set; }

        [DataMember]
        public String jenkinsHost { get; set; }

        [DataMember]
        public long userId { get; set; }

        public JobType() { }

        public JobType(String name, String jobURL, String jenkinsHost, long userId)
        {
            this.name = name;
            this.jobURL = jobURL;
            this.jenkinsHost = jenkinsHost;
            this.userId = userId;
        }
    }
}
