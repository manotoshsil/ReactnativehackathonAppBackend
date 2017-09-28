using FirebaseNet.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DeviceHubListner
{
    public class ListnerService
    {
        Timer _timer = null;
        public void Start()
        {
            
            _timer = new Timer(10 * 1000); // every 10 minutes
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            _timer.Start();
            Task.Run(async()=> { var message = await AzureIoTHub.ReceiveCloudToDeviceMessageAsync();
        } );
            // write code here that runs when the Windows Service starts up.  
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(async () =>
            {
                var message = await AzureIoTHub.ReceiveCloudToDeviceMessageAsync();
            });
        }

        public void Stop()
        {
            // write code here that runs when the Windows Service stops.  
        }
    }
}
