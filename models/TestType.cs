using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ZafiraIntegration.models
{
    [DataContract]
    public class TestType : AbstractType
    {
        [DataMember]
        public String name { get; set; }
        [DataMember]
        public String status { get; set; }
        [DataMember]
        public String testArgs { get; set; }
        [DataMember]
        public long testRunId { get; set; }
        [DataMember]
        public long testCaseId { get; set; }
        [DataMember]
        public String testGroup { get; set; }
        [DataMember]
        public String message { get; set; }
        [DataMember]
        public int messageHashCode { get; set; }
        [DataMember]
        public long startTime { get; set; }
        [DataMember]
        public long finishTime { get; set; }
        [DataMember]
        public List<String> workItems { get; set; }
        [DataMember]
        public int retry { get; set; }
        [DataMember]
        public String configXML { get; set; }
        //[DataMember]
        //private Map<String, Long> testMetrics { get; set; }
        [DataMember]
        public Boolean knownIssue { get; set; }
        [DataMember]
        public Boolean blocker { get; set; }
        [DataMember]
        public Boolean needRerun { get; set; }
        [DataMember]
        public String dependsOnMethods { get; set; }
        [DataMember]
        public String testClass { get; set; }
        [DataMember]
        public HashSet<TestArtifactType> artifacts = new HashSet<TestArtifactType>();

        public TestType() { }

        public TestType(String name, String status, String testArgs, long testRunId, long testCaseId, long startTime, List<String> workItems, int retry, String configXML)
        {
            this.name = name;
            this.status = status;
            this.testArgs = testArgs;
            this.testRunId = testRunId;
            this.testCaseId = testCaseId;
            this.startTime = startTime;
            this.workItems = workItems;
            this.retry = retry;
            this.configXML = configXML;
        }
    }
}
