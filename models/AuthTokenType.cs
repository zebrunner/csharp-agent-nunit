using System;
using System.Runtime.Serialization;

namespace ZafiraIntegration.models
{
    [DataContract]
    public class AuthTokenType : AbstractType
    {
        [DataMember]
        public String authTokenType { get; set; }
        [DataMember]
        public String authToken { get; set; }
        [DataMember]
        public String refreshToken { get; set; }
        [DataMember]
        public String authTokenExpirationInSecs { get; set; }

        public AuthTokenType()
        { }
                
    }
}
