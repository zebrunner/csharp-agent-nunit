using System;
using System.Runtime.Serialization;

namespace ZafiraIntegration.models
{
    [DataContract]
    public class TestCaseType : AbstractType
    {
        [DataMember]
        public String testClass;
        [DataMember]
        public String testMethod;
        [DataMember]
        public String info;
        [DataMember]
        public long testSuiteId;
        [DataMember]
        public long primaryOwnerId;
        [DataMember]
        public long secondaryOwnerId;
        [DataMember]
        public ProjectType project;

        public TestCaseType() { }

        public TestCaseType(String testClass, String testMethod, String info, long testSuiteId, long primaryOwnerId)
        {
            this.testClass = testClass;
            this.testMethod = testMethod;
            this.info = info;
            this.testSuiteId = testSuiteId;
            this.primaryOwnerId = primaryOwnerId;
            this.secondaryOwnerId = primaryOwnerId;
        }
    }
}
