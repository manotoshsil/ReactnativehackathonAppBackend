using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace DeviceHubListner
{
    internal static class ConfigureService
    {
        internal static void Configure()
        {
            HostFactory.Run(configure =>
            {
                configure.Service<ListnerService>(service =>
                {
                    
                    service.ConstructUsing(s => new ListnerService());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                //Setup Account that window service use to run.  
                
                configure.RunAsLocalSystem();
                configure.SetServiceName("DeviceListner1");
                configure.SetDisplayName("DeviceListner1");
                configure.SetDescription("My .Net windows service with Topshelf");
            });
        }
    }
}
