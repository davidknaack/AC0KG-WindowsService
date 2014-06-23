namespace AC0KG.WindowsService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceNameAttribute : Attribute
    {
        public readonly string ServiceName;

        public string DisplayName { get; set; }
        public string Description { get; set; }

        public ServiceNameAttribute(string serviceName)
        {
            ServiceName = serviceName;
        }
    }
}
