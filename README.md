This is a example for https://medium.com/zdjohn/making-resilient-service-using-akka-net-5434e28f87c8

The main prupose is to showcase using kensis + akka.net remote to build a self healing service that can replay events all by itself from the time of critical failure.


in side example there is two types of processor:
* dummy processor
* kcl processor

you can pick the one you would like to try by updating autofac code here:
https://github.com/zdjohn/akka-remote-kcl-exapmle/blob/master/IngestionService/Config/Dependencies.cs
~~~
//builder.RegisterType<RemoteProcessInvoker>().As<IRemoteProcessInvoker>().SingleInstance();
builder.RegisterType<KclProcessInvoker>().As<IRemoteProcessInvoker>().SingleInstance();
~~~

***setup your kinesis:***

you will need to have a aws account and kinesis stream available in order to have kenisis processor working:
see more detailed documentation here: http://docs.aws.amazon.com/streams/latest/dev/before-you-begin.html

***see required setting in app settings:***

`https://github.com/zdjohn/akka-remote-kcl-exapmle/blob/master/IngestionService/App.config`
~~~
 <appSettings>
    <add key="access_key" value="your_aws_access_key" />
    <add key="secret" value="your_aws_access_secret" />
    <add key="jars_folder" value="jars" />
    <add key="javaExcutable" value="C:\ProgramData\Oracle\Java\javapath\java.exe" />
    <add key="processorName" value="KCL.Processor"/>
  </appSettings>
~~~

***How to run***
~~~
F5
~~~
you will need to use visual studio 2017

***how to simulate failure:***

inside solution there is a healthy.txt file, which sample health checker checking against.

by: `rm ./your_project_path/bin/debug/health.txt` 

it will trigger health checker fail, resulting exteral processor shutdown.

by: `New-Item ./your_project_path/bin/debug/health.txt` 

will help you resotre health status, then restore whole processor solution


***dependency***
The nuget package of AWS_KCL_DOTNET is compiled from the branch:

https://github.com/zdjohn/amazon-kinesis-client-net/tree/cake

you can run `build.ps1` to get the same nuget i am using in this example.

I rased issue: https://github.com/awslabs/amazon-kinesis-client-net/issues/14 
if you like it, or you have any thought, please comment along. kinesis need more dotnet love. :-)


**By intentionally design a fragile component piece inside your application achitecture, and make it fail fast. 
This protects the system as a whole.**


![nux](X0PGo3.gif "i live, i die, i live again")


Happy coding!
