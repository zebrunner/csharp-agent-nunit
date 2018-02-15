# zafira-nunit
To use it add reference to your project:

```
References -> Add Reference ...  bin/Realese/ZafiraIntegration.dll 

```
### Ways to use:
```
1. Class attributes(should be added to all classes that need to be report): 
[ReportSuite] - for creation test suit in zafira
[ReportTest] - for creation tests 
* possible to use only one [ReportSuite]; 
** single [ReportTest] will not work

2. Extent your BaseTest class from ZafiraListener.cs(will automaticaly work with all existing tests)

```
