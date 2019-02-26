using System;
using System.Runtime.Serialization;

namespace ZafiraIntegration.models
{
    [DataContract]
    public class TestRunType : AbstractType
    {
        [DataMember]
        public String ciRunId { get; set; }
        [DataMember]
        public long? testSuiteId { get; set; }
        //[DataMember]
        //public Status status { get; set; }
        [DataMember]
        public String scmURL { get; set; }
        [DataMember]
        public String scmBranch { get; set; }
        [DataMember]
        public String scmCommit { get; set; }
        [DataMember]
        public String configXML { get; set; }
        [DataMember]
        public long? jobId { get; set; }
        [DataMember]
        public long? upstreamJobId { get; set; }
        [DataMember]
        public int? upstreamJobBuildNumber { get; set; }
        [DataMember]
        public int? buildNumber { get; set; }
        [DataMember]
        public String startedBy;
        [DataMember]
        public long? userId { get; set; }
        [DataMember]
        public String workItem { get; set; }
        [DataMember]
        public ProjectType project { get; set; }
        [DataMember]
        public Boolean knownIssue { get; set; }
        [DataMember]
        public Boolean blocker { get; set; }
        [DataMember]
        public String driverMode;
        [DataMember]
        public Boolean reviewed { get; set; }

        public TestRunType() { }

        public TestRunType(String ciRunId, long? testSuiteId, long? userId, String scmURL, String scmBranch, String scmCommit,
            String configXML, long? jobId, long? upstreamJobId, int? buildNumber, String startedBy, String workItem)
        {
            this.ciRunId = ciRunId;
            this.testSuiteId = testSuiteId;
            this.userId = userId;
            this.scmURL = scmURL;
            this.scmBranch = scmBranch;
            this.scmCommit = scmCommit;
            this.configXML = configXML;
            this.jobId = jobId;
            this.buildNumber = buildNumber;
            this.startedBy = startedBy;
            this.workItem = workItem;
            this.driverMode = DriverMode.SUITE_MODE.ToString();
            this.upstreamJobId = upstreamJobId;
        }
    }
}