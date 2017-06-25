This is a example for https://medium.com/zdjohn/making-resilient-service-using-akka-net-5434e28f87c8

The main prupose is to showcase using kensis + akka.net remote to build a self healing service that can replay events all by itself from the time of critical failure.


in side exapmle there is two types of processor:
* dummy processor
* kcl processor
you can pick the one you would like to try by updating autofac code here:
https://github.com/zdjohn/akka-remote-kcl-exapmle/blob/master/IngestionService/Config/Dependencies.cs
~~~
//builder.RegisterType<RemoteProcessInvoker>().As<IRemoteProcessInvoker>().SingleInstance();
builder.RegisterType<KclProcessInvoker>().As<IRemoteProcessInvoker>().SingleInstance();
~~~

*setup your kinesis:*
you will need to have a aws account and kinesis stream available in order to have kenisis processor working:
see more detailed documentation here: http://docs.aws.amazon.com/streams/latest/dev/before-you-begin.html

see required setting in app settings:
https://github.com/zdjohn/akka-remote-kcl-exapmle/blob/master/IngestionService/App.config
~~~
 <appSettings>
    <add key="access_key" value="your_aws_access_key" />
    <add key="secret" value="your_aws_access_secret" />
    <add key="jars_folder" value="jars" />
    <add key="javaExcutable" value="C:\ProgramData\Oracle\Java\javapath\java.exe" />
    <add key="processorName" value="KCL.Processor"/>
  </appSettings>
~~~



**By intentionally design a fragile component piece inside your application achitecture, and make it fail fast. 
This protects the system as a whole.**


![i live, i die , i live again](http://i.makeagif.com/media/6-02-2016/X0PGo3.gif)


Happy coding!
