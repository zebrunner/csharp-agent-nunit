using System;
using System.Runtime.Serialization;

namespace ZafiraIntegration.models
{
    [DataContract]
    public class RefreshTokenType : AbstractType
    {
        [DataMember]
        public String refreshToken { get; set; }

        public RefreshTokenType() { }

        public RefreshTokenType(String refreshToken)
        {
            this.refreshToken = refreshToken;
        }

    }
}
