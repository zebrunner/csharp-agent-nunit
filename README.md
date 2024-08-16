# Zebrunner NUnit agent

> **Incubation warning**
>
> Please note, that agent is currently in an **incubating state**, meaning that Zebrunner team can not guarantee its stable work since it was not properly tested.

The Zebrunner NUnit agent provides reporting functionality and allows analyzing tests execution results from Zebrunner.

Feel free to support the development with a [**donation**](https://www.paypal.com/donate/?hosted_button_id=MNHYYCYHAKUVA) for the next improvements.

<p align="center">
  <a href="https://zebrunner.com/"><img alt="Zebrunner" src="https://github.com/zebrunner/zebrunner/raw/master/docs/img/zebrunner_intro.png"></a>
</p>

## Inclusion into your project

The agent have not been published into NuGet since it is in incubating state. To include the agent into your project you have the following options:
1. Clone the repository and manually build the .dll file with the agent. Then, add the .dll file into your test project.
2. Contact Zebrunner team, and we will share the .dll file with you.

## Tracking of test results

### Configuration

Once the agent is added to test project, it is not automatically enabled. The valid configuration must be provided first. It is currently possible to provide the configuration only via environment variables of OS.

The following environment variables are recognized by the agent:
- `REPORTING_ENABLED` - enables or disables reporting. The default value is `false`. If disabled, the agent will use no op component implementations that will not submit execution results to Zebrunner;
- `REPORTING_SERVER_HOSTNAME` - mandatory if reporting is enabled. It is Zebrunner server hostname. It can be obtained in Zebrunner on the 'Account and Profile' page under the 'Zebrunner API access' section;
- `REPORTING_SERVER_ACCESS_TOKEN` - mandatory if reporting is enabled. Access token must be used to perform API calls. It can be obtained in Zebrunner on the 'Account and Profile' page under the 'Zebrunner API access' section;
- `REPORTING_PROJECT_KEY` - optional value. It is the project that the test run belongs to. The default value is `DEF`. You can manage projects in Zebrunner in the appropriate section;
- `REPORTING_RUN_ENVIRONMENT` - optional value. It is the environment where the tests will run. The value does not influence the actual tests execution and used only to display in Zebrunner;
- `REPORTING_RUN_BUILD` - optional value. It is the build number that is associated with the test run. It can depict either the test build number or the application build number. The value does not influence the actual tests execution and used only to display in Zebrunner;

### Attributes

There are two possible ways to use the agent:

1. Add class attributes for class level reporting:
```csharp
[ZebrunnerClass] - for creation of a test run in Zebrunner with the name of the class
[ZebrunnerTest] - for reporting test results in Zebrunner
```

2. Add assembly and class attributes for assembly level reporting:
```csharp
[ZebrunnerAssembly] - for creation of a test run in Zebrunner with the name of Jenkins job it was triggered from
[ZebrunnerAssemblyTest] - for creation tests in Zebrunner
```

#### Examples

Use attributes for base testing class or just for classes that need to be reported:
```csharp
    [TestFixture, ZebrunnerClass, ZebrunnerTest]
    public class BaseTest 
    {

        [OneTimeSetUp]
        public void startDriver()
        {
            // some code here...
        }

    }
```

Add assembly attribute for base testing class at assembly level and add test attribute for base testing class or just for classes that need to be reported
```csharp
[assembly: ZebrunnerAssembly]

namespace Automation.tests
{
    [TestFixture, ZebrunnerAssemblyTest]
    public class BaseTest
    {
        // some code here...
    }
}
```

### Collecting test logs
It is also possible to enable the log collection for your tests. Currently, the Agent supports only NUnit logging framework out of the box. 

All you have to do to enable stream NLog logs to Zebrunner is to register the log target with type `Zebrunner` and corresponding rule in `App.config` file. Example of the configuration:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="nlog" type="NLog.Config.ConfigSectionHandler, NLog"/>
  </configSections>

  <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

    <targets>
      <target name="console" xsi:type="Console"/>
      <target name="Zebrunner" xsi:type="Zebrunner" layout="${logger} ${level}: ${message}"/>
    </targets>

    <rules>
      <logger name="*" minlevel="Info" writeTo="console"/>
      <logger name="*" minlevel="Trace" writeTo="Zebrunner"/>
    </rules>
  </nlog>
</configuration>
```

### Collecting captured screenshots
In case you are using NUnit as a UI testing framework, it may be useful to have an ability to track captured screenshots in scope of Zebrunner reporting. The agent comes with a API allowing you to send your screenshots to Zebrunner, so they will be attached to the test.

Below is a sample code of test sending a screenshot to Zebrunner:
```csharp
using NUnit.Framework;
using ZebrunnerAgent;
using ZebrunnerAgent.Registrar;

namespace NUnit.Samples.Money
{
    [TestFixture]
    [ZebrunnerClass, ZebrunnerTest]
    public class MoneyTest 
    {
        [Test]
        public void test()
        {
            // some code here...

            Screenshot.Upload(byteArray);
            Screenshot.Upload(byteArray, dateTimeOffset);

            // some code here...
        }
    }
}
```
A screenshot should be provided as a byte array along with a timestamp corresponding to the moment when the screenshot was captured. If timestamp is not provided, current timestamp will be used. However, it is recommended to use an accurate timestamp in order to get accurate tracking.
The uploaded screenshot will appear among test logs. The actual position depends on the provided (or generated) timestamp.

### Collecting additional artifacts

In case your tests or entire test run produce some artifacts, it may be useful to track them in Zebrunner. The agent comes with a few convenient methods for uploading artifacts in Zebrunner and linking them to the currently running test or the test run.

Artifacts can be uploaded using the Artifact class. This class has a bunch of static methods to either attach an arbitrary artifact reference or upload artifacts represented by `Stream`, byte array, or path to file.

The #attachToTestRun(name, file) and #attachToTest(name, file) methods can be used to upload and attach an artifact file to test run and test respectively.

The #attachReferenceToTestRun(name, reference) and #attachReferenceToTest(name, reference) methods can be used to attach an arbitrary artifact reference to test run and test respectively.

Together with an artifact or artifact reference, you must provide the display name. For the file, this name must contain the file extension that reflects the actual content of the file. If the file extension does not match the file content, this file will not be saved in Zebrunner. Artifact reference can have an arbitrary name.

Below is a sample code of test attaching a few artifacts and references to the currently running test and the whole test run:
```csharp
using NUnit.Framework;
using ZebrunnerAgent;
using ZebrunnerAgent.Registrar;

namespace NUnit.Samples.Money
{
    [TestFixture]
    [ZebrunnerClass, ZebrunnerTest]
    public class MoneyTest 
    {
        [Test]
        public void test()
        {
            // some code here...
           
            Artifact.AttachToTestRun("file-1.txt", stream);
            Artifact.AttachToTestRun("file-2.txt", pathToFile);
            Artifact.AttachReferenceToTestRun("Zebrunner", "https://zebrunner.com/");

            Artifact.AttachToTest("file.txt", byteArray);
            Artifact.AttachReferenceToTest("Zebrunner Documentation", "https://zebrunner.com/documentation");

            // some code here...
        }
    }
}
```

### Tracking test maintainer
You may want to add transparency to the process of automation maintenance by having an engineer responsible for evolution of specific tests or test classes. Zebrunner comes with a concept of a maintainer - a person that can be assigned to maintain tests. In order to keep track of those, the agent comes with the `[Maintainer]` attribute.

This attribute can be placed on both test class and method. It is also possible to override a class-level maintainer on a method-level. If a base test class is marked with this annotation, all child classes will inherit the annotation unless they have an explicitly specified one.

See a sample test class below:
```csharp
using NUnit.Framework;
using ZebrunnerAgent;
using ZebrunnerAgent.Registrar;

namespace NUnit.Samples.Money
{
    [TestFixture]
    [Maintainer("Deve")]
    [ZebrunnerClass, ZebrunnerTest]
    public class MoneyTest 
    {
        [Test]
        [Maintainer("Loper")]
        public void test1()
        {
            // some code here...
        }
        
        [Test]
        public void test2()
        {
            // some code here...
        }
    }
}
```
In the example above, Deve will be reported as a maintainer of test2 (class-level value taken into account), while Loper will be reported as a maintainer of test1.

The maintainer username should be a valid Zebrunner username, otherwise it will be set to anonymous.

### Attaching test labels
In some cases, it may be useful to attach meta information related to a test - its feature, its priority, or any other useful data.

The agent comes with a concept of a label. Label is a key-value pair associated with a test. The key is represented by a `string`, the label value accepts a `params` of `string`.

There is also the `TestLabel` attribute that can be used to attach static labels to test. The attribute can be used on both class and method levels. It is also possible to override a class-level label on a method-level.

There is also an API to attach labels during test execution. The `Label` class has a static method that can be used to attach a label.

Here is a sample:
```csharp
using NUnit.Framework;
using ZebrunnerAgent;
using ZebrunnerAgent.Registrar;

namespace NUnit.Samples.Money
{
    [TestFixture]
    [ZebrunnerClass, ZebrunnerTest]
    public class MoneyTest 
    {
        [Test]
        public void test()
        {
            // some code here...

            Label.AttachToTestRun("backend-service", "reporting", "iam");
            Label.AttachToTest("feature", "Labels");

            // some code here...
        }
    }
}
```
The values of attached labels will be displayed in Zebrunner under the name of a corresponding test.

## Troubleshooting
In case of any difficulties, please contact the Zebrunner team in Telegram https://t.me/zebrunner
