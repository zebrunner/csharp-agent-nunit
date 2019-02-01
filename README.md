# Zafira::NUnit

ZafiraIntegration library that implements integration Zafira reporting into existing NUnit test structure.

## Usage

### Installation

To enable library just add reference to your project:

```
References -> Add Reference ...  bin/Realese/ZafiraIntegration.dll 
```

### Configuration


There are two possible ways to use library:
1. Add class attributes for class level reporting:
```
[ZafiraClass] - for creation test suite in zafira with the name of the class
[ZafiraTest] - for creation tests in zafira
```
2. Add assembly and class attributes for assembly level reporting:
```
[ZafiraAssembly] - for creation test suite in zafira with the name of Jenkins job it was triggered from
[ZafiraAssemblyTest] - for creation tests in zafira
```
### Examples

- Use attributes for base testing class or just for classes that need to be reported:
```csharp
    [TestFixture, ZafiraClass, ZafiraTest]
    public class BaseTest 
    {
     
        [OneTimeSetUp]
        public void startDriver()
        {
            ...
        }
    
    }
```
- Add assembly attribute for base testing class at assembly level and add test attribute for base testing class or just for classes that need to be reported

```csharp
[assembly: ZafiraAssembly]

namespace Automation.tests
{
    [TestFixture, ZafiraAssemblyTest]
    public class BaseTest
    {
       ...
    }
}
```
### Troubleshooting
To be done ...