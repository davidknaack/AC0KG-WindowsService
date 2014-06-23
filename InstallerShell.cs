namespace AC0KG.WindowsService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Reflection;

    [RunInstaller(false)]
    public class InstallerShell : System.Configuration.Install.Installer
    {
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller serviceInstaller;

        public InstallerShell()
        {
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;

            System.Reflection.MemberInfo info = GetType();
            var attribs = ((ServiceNameAttribute)info.GetCustomAttributes(typeof(ServiceNameAttribute), true).First());

            this.serviceInstaller.Description = attribs.Description;
            this.serviceInstaller.DisplayName = attribs.DisplayName;
            this.serviceInstaller.ServiceName = attribs.ServiceName;
            this.serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.serviceInstaller});
        }
    }
}
