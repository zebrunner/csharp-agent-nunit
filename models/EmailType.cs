using System;
using System.Runtime.Serialization;

namespace ZafiraIntegration.models
{
    [DataContract]
    public class EmailType : AbstractType
    {
        [DataMember]
        public String recipients { get; set; }

        [DataMember]
        public String subject { get; set; }

        [DataMember]
        public String text { get; set; }


        public EmailType(String recipients)
        {
            this.recipients = recipients;
        }
    }
}
