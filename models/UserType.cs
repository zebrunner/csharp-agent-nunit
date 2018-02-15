using System;
using System.Runtime.Serialization;

namespace ZafiraIntegration.models
{
    [DataContract]
    public class UserType : AbstractType
    {
        [DataMember]
        public String username { get; set; }
        [DataMember]
        public String email { get; set; }
        [DataMember]
        public String firstName { get; set; }
        [DataMember]
        public String lastName { get; set; }
        [DataMember]
        public String password { get; set; }

        public UserType()
        {

        }

        public UserType(String username, String email, String firstName, String lastName)
        {
            this.username = username;
            this.email = email;
            this.firstName = firstName;
            this.lastName = lastName;
        }
    }


}
