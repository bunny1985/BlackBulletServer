using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace NotificationBackend.Infrastrucuture.Firebase
{
    public class FireBaseNotificationSender
    {
        public class FireBaseData<T> where T: BasicData
            {
            public string to { get; set; }

            public FireBaseData(){
                
            }
            public FireBaseData(String id){
                this.to = id;
            }
            public FireBaseData(String id , T data){
                this.to = id;
                this.data = data;
            }

            public List<string> registrationIds { get; set; }
            public T data { get; set; }
        }
        public  class  BasicData {
            public string type { get; set; }
        }
        class NotificationData : BasicData{
                public NotificationData(){
                    this.type = "command.notification";
                }
                public string contentTitle { get; set; }
                public string message { get; set; }
        }
        class SmsData : BasicData{
                public SmsData(){
                    this.type = "command.sms";
                }
                public string to { get; set; }
                public string message { get; set; }
        }
        
        class DismissData : BasicData{
                public DismissData(){
                    this.type = "command.dismiss";
                }
                public string id { get; set; }
                
        }
        public FireBaseNotificationSender(){
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            var config = builder.Build();
            this.apiKey = config.GetValue<string>("FireBase:ApiKey");

        }



        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
        string postDataContentType = "application/json";
        string apiKey ="" ; // hardcorded

        
        public void SendRingtone(string deviceId  ){
            var basicData = new BasicData();
            basicData.type = "command.ringtone";
            var model  = new FireBaseData<BasicData>(deviceId , basicData);
            SendFireBaseRequest(model);
        }
        public void SendBatteryStatusReques(string deviceId  ){
            var basicData = new BasicData();
            basicData.type = "command.battery";
            var model  = new FireBaseData<BasicData>(deviceId , basicData);
            SendFireBaseRequest(model);
        }

        public void SendSms(string deviceId ,  string to, string message ){
            var sms = new SmsData();
            sms.to = to;
            sms.message = message;
            sms.type = "command.sms";
            var model  = new FireBaseData<SmsData>(deviceId , sms);
            
            SendFireBaseRequest(model);
        }
        public void SendNotification(string deviceId ,  string title, string message )
        {
            var model  = new FireBaseData<NotificationData>(deviceId);
            model.data = new NotificationData(){contentTitle= title,message = message};
            model.data.type = "command.notification";
            SendFireBaseRequest(model);
        }
        public void SendDismiss(string deviceId ,  string id)
        {
            
            var model  = new FireBaseData<DismissData>(deviceId);
            model.data = new DismissData(){id = id};
            model.data.type = "command.dismiss";
            SendFireBaseRequest(model);
        }

        public void SendFireBaseRequest<T>(FireBaseData<T> requestData ) where T: BasicData{

            string postData =JsonConvert.SerializeObject(requestData);
            
            //
            //  MESSAGE CONTENT
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //
            //  CREATE REQUEST
            

            //
            //  SEND MESSAGE
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(FireBaseNotificationSender.ValidateServerCertificate);
                HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://android.googleapis.com/gcm/send");
            Request.Method = "POST";
            Request.KeepAlive = false;
            Request.ContentType = postDataContentType;
            Request.Headers.Add(string.Format("Authorization: key={0}", apiKey));
            Request.ContentLength = byteArray.Length;
            Log.Logger.Information("Firebase Request:" + postData);
            Stream dataStream = Request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
                WebResponse Response = Request.GetResponse();
                HttpStatusCode ResponseCode = ((HttpWebResponse)Response).StatusCode;
                if (ResponseCode.Equals(HttpStatusCode.Unauthorized) || ResponseCode.Equals(HttpStatusCode.Forbidden))
                {
                    var text = "Unauthorized - need new token";
                }
                else if (!ResponseCode.Equals(HttpStatusCode.OK))
                {
                    var text = "Response from web service isn't OK";
                }

                StreamReader Reader = new StreamReader(Response.GetResponseStream());
                string responseLine = Reader.ReadToEnd();
                Reader.Close();


            }
            catch (Exception )
            {
                Log.Logger.Error("Error While Sending firebase data ");
            }
        }



        








        
    }



}