using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using FirebaseNet.Messaging;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;

static class AzureIoTHub
{
    //
    // Note: this connection string is specific to the device "1001". To configure other devices,
    // see information on iothub-explorer at http://aka.ms/iothubgetstartedVSCS
    //
    const string deviceConnectionString = "HostName=HackHub.azure-devices.net;DeviceId=1001;SharedAccessKey=NKWjMXLef5UPuiha4JDHSc9T+SIFlWaW+Hrl7ijJXz8=";

    //
    // To monitor messages sent to device "1001" use iothub-explorer as follows:
    //    iothub-explorer HostName=HackHub.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=6Z6GIX/riytfl4VQjUUzpokp4UGqUUe9hILEyor1yBk= monitor-events "1001"
    //

    // Refer to http://aka.ms/azure-iot-hub-vs-cs-wiki for more information on Connected Service for Azure IoT Hub

    public static async Task SendDeviceToCloudMessageAsync()
    {
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, Microsoft.Azure.Devices.Client.TransportType.Amqp);

        Dictionary<string, string> dctDevice = new Dictionary<string, string>();
        dctDevice.Add("Id", "1010");
        dctDevice.Add("Type", "gas");
        dctDevice.Add("latitude", "12.9716");
        dctDevice.Add("Event", "Installed");
        dctDevice.Add("longitude", "77.5946");
        //dctDevice.Add("Id", "1010");
        //var message = new Message(Encoding.ASCII.GetBytes(str));
        try
        {

            var msg = new Microsoft.Azure.Devices.Client.Message(Encoding.UTF8.GetBytes(dctDevice.ToString().ToCharArray()));

        //    var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(dctDevice)));


           // message.Properties.Add("severity", "high");


            await deviceClient.SendEventAsync(msg);
        }catch(Exception oEx)
        {

        }
    }

    public static async Task<string> ReceiveCloudToDeviceMessageAsync()
    {
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, Microsoft.Azure.Devices.Client.TransportType.Amqp);

        while (true)
        {
            
            var receivedMessage = await deviceClient.ReceiveAsync();

            if (receivedMessage != null)
            {
                var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                // await deviceClient.CompleteAsync(receivedMessage);
                //SendNotificationFromFirebaseCloud(receivedMessage);
                bool isEvent = false;
                string URI = string.Empty;
                System.Collections.Specialized.NameValueCollection formData = new System.Collections.Specialized.NameValueCollection();
                foreach (var item in receivedMessage.Properties)
                {
                    formData.Add(item.Key, item.Value);
                    if (item.Key == "eventName")
                        isEvent = true;
                    // formData[item.Key] = item.Value;
                    // formData["Password"] = "mypassword";
                }
                await deviceClient.CompleteAsync(receivedMessage);
                // var json = JsonConvert.SerializeObject(dctDevice);
                //// var json = new JavaScriptSerializer().Serialize(dctDevice);
                // client.DefaultRequestHeaders.Accept.Add(
                //    new MediaTypeWithQualityHeaderValue("application/json"));
                if (isEvent)
                {
                    URI = "http://172.23.245.131:3000/Meter/Event";
                }else
                {
                    URI = "http://172.23.245.131:3000/Meter/Install";
                }
                // var response = client.PostAsJsonAsync("Meter/Event", json).Result;
                using (WebClient wc = new WebClient())
                {


                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                    //formData["Username"] = "myusername";
                    //formData["Password"] = "mypassword";
                    byte[] responseBytes = (new WebClient()).UploadValues(URI, "POST", formData);
                    string Result = Encoding.UTF8.GetString(responseBytes);
                    

                }


                  
                
               // Console.Write(receivedMessage);
                return messageData;
            }

            await Task.Delay(TimeSpan.FromSeconds(1000));
        }
    }

    //private static async void SendNotificationFromFirebaseCloud(Microsoft.Azure.Devices.Client.Message receivedMessage)
    //{
    //    FCMClient client = new FCMClient("AAAAkPbPGI0:APA91bFGiQB5wIRoupQUtThWvz-5duJV4pXUjseWbr_Z2eTwtFRjLXjqhb0h8waQ55SCJJ1ZG22_zJFZrNUoAJY85aEsXyt0eUItEJmZD5IeqHSOO__pAx8B9fpzOPI4uUYaeEWHmNFc");
    //    Dictionary<string, string> dctDevice = new Dictionary<string, string>();
    //    foreach(var item in receivedMessage.Properties)
    //    {
    //        dctDevice.Add(item.Key, item.Value);
    //    }
    //    var json = new JavaScriptSerializer().Serialize(dctDevice);
    //    var message = new FirebaseNet.Messaging.Message()
    //    {
    //        To = "dzqavrHY9L8:APA91bHNtTe4nBf8Pn8yRxEf9WLervWH8pm2uDMsM5Wxq_VIXPp0jcALV01uc2pbXRjk09HdvnPwn2_CpIvyqlU1eWqKHF3C2BMJmpHHIkY8W-EczgHKquCbQZ_M2OcJTxY6bxbv9Noo",
    //        Notification = new AndroidNotification()
    //        {
    //            Body = json,
    //            Title = "Portugal vs. Denmark",
    //            Icon = "myIcon"
    //        }
    //    };

    //    var result = await client.SendMessageAsync(message);
    //  //  SendMessageUsingApi(json);
    //}

    //private static bool SendMessageUsingApi(Microsoft.Azure.Devices.Client.Message receivedMessage)
    //{
    //    System.Collections.Specialized.NameValueCollection formData = new System.Collections.Specialized.NameValueCollection();
    //    foreach (var item in receivedMessage.Properties)
    //    {
    //        formData.Add(item.Key, item.Value);
    //       // formData[item.Key] = item.Value;
    //       // formData["Password"] = "mypassword";
    //    }
    //   // var json = JsonConvert.SerializeObject(dctDevice);
    //   //// var json = new JavaScriptSerializer().Serialize(dctDevice);
    //   // client.DefaultRequestHeaders.Accept.Add(
    //   //    new MediaTypeWithQualityHeaderValue("application/json"));
    //    string URI = "http://172.23.245.131:3000/Meter/Event";
    //    // var response = client.PostAsJsonAsync("Meter/Event", json).Result;
    //    using (WebClient wc = new WebClient())
    //    {


    //        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
           
    //        //formData["Username"] = "myusername";
    //        //formData["Password"] = "mypassword";
    //        byte[] responseBytes = (new WebClient()).UploadValues(URI, "POST",formData);
    //        string Result = Encoding.UTF8.GetString(responseBytes);
    //        if (String.IsNullOrEmpty(Result)) { return true; }
    //        else
    //         return   false;
    //    }
    //}
}
