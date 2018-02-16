# ZafiraIntegration with NUnit

ZafiraIntegration library that implements integration Zafira reporting into existing NUnit test structure.

## Usage

### Installation

To enable library just add reference to your project:

```
References -> Add Reference ...  bin/Realese/ZafiraIntegration.dll 
```

### Configuration


There are two possible ways to use library:
1. Add class attributes:
```
[ZafiraSuite] - for creation test suite in zafira
[ZafiraTest] - for creation tests in zafira
```
2. Extent your BaseTest class from ZafiraListener.cs

### Examples

- Use attributes for base tesing class or just for classes that need to be reported:
```csharp
    [ZafiraSuite, ZafiraTest]
    public class BaseTest 
    {
     
        [OneTimeSetUp]
        public void startDriver()
        {
            ...
        }
    
    }
```
- Extent base testing class 

```csharp
    public class BaseTest : ZafiraListener
    {
        
        [OneTimeSetUp]
        public void startDriver()
        {
            ...
        }
    }
```
### Troubleshooting
To be done ...
