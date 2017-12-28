using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NotificationBackend.Infrastrucuture.Firebase
{
    public class FireBaseNotificationSender
    {
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
        string postDataContentType = "application/json";
        string apiKey = "AAAA4r_9Gro:APA91bHVPKC1wnACoBTh6Nc4m_AQyhG2H-RX2XA2WL8Tok8y-J6lT7e6a2_Bp-fhwuGhHmdIHlbD7S8y0iDCAm2eklVypBM3QQfoILujPunhFFvEh74DX0x7Z22yKvy-K9KCgGrk-l2T"; // hardcorded

        

        public void SendNotification(string deviceId ,  string title, string message , string tickerText)
        {
            string postData =
    "{ \"registration_ids\": [ \"" + deviceId + "\" ], " +
      "\"data\": {\"tickerText\":\"" + tickerText + "\", " +
                 "\"contentTitle\":\"" + title + "\", " +
                 "\"message\": \"" + message + "\"}}";

            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(FireBaseNotificationSender.ValidateServerCertificate);
            //
            //  MESSAGE CONTENT
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //
            //  CREATE REQUEST
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://android.googleapis.com/gcm/send");
            Request.Method = "POST";
            Request.KeepAlive = false;
            Request.ContentType = postDataContentType;
            Request.Headers.Add(string.Format("Authorization: key={0}", apiKey));
            Request.ContentLength = byteArray.Length;

            Stream dataStream = Request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            //
            //  SEND MESSAGE
            try
            {
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
            }




        }








        
    }



}