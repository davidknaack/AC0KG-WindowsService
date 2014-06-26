AC0KG.WindowsService
==============

Simple .NET Windows Service Shell
--------------
AC0KG.WindowsService simplifies the creation of Windows Services.
It is designed to provide a nearly painless setup for service applications
that do not need support from the Service Control Manager. Apps using it may 
run on the desktop for debugging, or may be started through the Windows
service control functions.
The shell supports self-installation, start and stop actions, and
allows the application to be run in console mode.

For a practical example of the library in action, see the Firmata Web API 
project, a self-hosted NancyFX ( http://nancyfx.org ) based web service
which runs as a Windows Service with AC0KG.WindowsService:

	https://github.com/davidknaack/FirmataWebApi

Quickstart
--------------
Create a new console application and install AC0KG.WindowsService with Nuget:

    PM> Install-Package AC0KG-WindowsService

Above the default "class Program" in Program.cs, add two classes:

    // This attribute tells Visual Studio to not use the designer for this file.
    [System.ComponentModel.DesignerCategory("")]
    [ServiceName("SampleService")]
    class Service : ServiceShell { }
	
    [RunInstaller(true)]
    [ServiceName("SampleService", DisplayName = "Sample Service", Description = "Sample Service Description")]
    public class Installer : InstallerShell { }

The value of the "ServiceName" attribute should match on both classes, it 
will be the short name of the service that can be used on the command line
for starting and stopping the service. The values of the DisplayName and 
Description properties will appear in the Service Control Manager.

The work is done by a long-running process. It can be contained by a Task, 
or some other long-running process, for example:

    public static void Run()
    {
		do
		{
			Console.WriteLine("working");
		} while (!tokenSrc.Token.WaitHandle.WaitOne(1000));
    }

The variable tokenSrc is a CancellationTokenSource used in the Stop() action 
to signal the worker to exit.

To start the service:

    Service.StartService<Service>(
    	() => { Task.Factory.StartNew(Run, tokenSrc.Token); },
    	() => { tokenSrc.Cancel(); }, 
    	Environment.UserInteractive);

The type parameter should be the descendent class of ServiceShell. StartService
will inspect that type to retrieve the ServiceName attribute.

The first parameter is the start up action. You can pass an anonymous function,
as in the example, or for more complex services you might want to pass a member
function of a class that implements the behavior of the application:

    using( var appCore = new AppCore())
		Service.StartService<Service>(
			appCore.Start,
			appCore.Stop, 
			Environment.UserInteractive);

Installation of the service is handled by the ServiceShell:

    if (ServiceShell.ProcessInstallOptions(args))
	    return;
		
This will check for -i, -install, -u, -uninstall parameters and perform the
installation or uninstallation. These should be run as Administrator.

Here is the complete service application:

    using System;
    using AC0KG.WindowsService;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    
    namespace ConsoleApplication1
    {
        // This attribute tells Visual Studio to not use the designer for this file.
        [System.ComponentModel.DesignerCategory("")]
	[ServiceName("SampleService")]
        class Service : ServiceShell { }
    
        [RunInstaller(true)]
        [ServiceName("SampleService", DisplayName = "Sample Service", Description = "Sample Service")]
        public class Installer : InstallerShell { }
    
        class Program
        {
            static CancellationTokenSource tokenSrc = new CancellationTokenSource();
    
            static void Main(string[] args)
            {
                if (ServiceShell.ProcessInstallOptions(args))
                    return;
    
                Service.StartService<Service>(
                    () => { Task.Factory.StartNew(Run, tokenSrc.Token); },
                    () => { tokenSrc.Cancel(); }, 
                    Environment.UserInteractive);
            }
    
            public static void Run()
            {
                do
                {
                    Console.WriteLine("working");
                } while (!tokenSrc.Token.WaitHandle.WaitOne(1000));
            }
        }
    }
