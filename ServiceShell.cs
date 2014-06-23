namespace AC0KG.WindowsService
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Configuration.Install;
    using System.Reflection;
    using System.ServiceProcess;

    public class ServiceShell : System.ServiceProcess.ServiceBase
    {
        private Action _start;
        private Action _stop;

        public ServiceShell()
        {
            Trace.WriteLine("Get service name");
            System.Reflection.MemberInfo info = GetType();
            //Trace.WriteLine("got type " + info.Name);
            var attribs = info.GetCustomAttributes(typeof(ServiceNameAttribute), true);
            Trace.WriteLine("attribs: " + attribs.Length);
            ServiceName = ((ServiceNameAttribute)attribs.First()).ServiceName;
            Trace.WriteLine(ServiceName);

            CanStop = true;
            CanPauseAndContinue = false;
            CanShutdown = true;
            AutoLog = true;
            Trace.WriteLine("constructor done");
        }

        /// <summary>
        /// Get an instance of the service for use with ServiceBase.Run()
        /// </summary>
        /// <param name="start">Startup action</param>
        /// <param name="stop">Shutdown action</param>
        /// <returns></returns>
        public static void StartService<T>(Action start=null, Action stop=null, bool asConsole = false) where T : ServiceShell, new()
        {
            if (asConsole)
            {
                if (start != null)
                    start();

                Trace.WriteLine("Press [Enter] to close the service.");
                Console.ReadLine();

                if (stop != null)
                    stop();
            }
            else
                ServiceBase.Run(new T() { _start = start, _stop = stop });
        }

        protected override void OnStart(string[] args)
        {
            Trace.WriteLine("ServiceShell start");
            if (_start != null)
                _start();
            Trace.WriteLine("ServiceShell done");
        }

        protected override void OnStop()
        {
            Trace.WriteLine("ServiceShell stop");
            if (_stop != null)
                _stop();
            Trace.WriteLine("ServiceShell stop done");
        }

        /// <summary>
        /// Check args for install or uninstall parameters.
        /// </summary>
        /// <param name="args">Arguments array, such as from Main()</param>
        /// <returns>True if parameters included -i/-install or -u/-uninstall options.</returns>
        public static bool ProcessInstallOptions(string[] args)
        {
            var result = false;

            if ((args != null)
                 && (args.Length == 1)
                 && (args[0].Length > 1)
                 && (args[0][0] == '-' || args[0][0] == '/'))
            {
                switch (args[0].Substring(1).ToLower())
                {
                    default:
                        break;

                    case "install":
                    case "i":
                        Install();
                        result = true;
                        break;

                    case "uninstall":
                    case "u":
                        Uninstall();
                        result = true;
                        break;
                }
            }

            return result;
        }

        private static bool Install()
        {
            try
            {
                Trace.WriteLine("Service Install");
                ManagedInstallerClass.InstallHelper(new string[] { "/LogToConsole=true", Assembly.GetEntryAssembly().Location });
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return false;
            }
            return true;
        }

        private static bool Uninstall()
        {
            try
            {
                Trace.WriteLine("Service Uninstall");
                ManagedInstallerClass.InstallHelper(new string[] { "/LogToConsole=true", "/u", Assembly.GetEntryAssembly().Location });
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return false;
            }
            return true;
        }

    }

}
