using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NotificationBackend.Infrastrucuture.Database;
using NotificationBackend.Infrastrucuture.Firebase;

namespace NotificationBackend.Infrastrucuture.WebSockets
{
    public class NotificationSocketHandler: WebSocketHandler
    {
        private FireBaseNotificationSender _fireBaseNotificationSender;
        private MyDbContext _db;
        private WebSocketConnectionManager _webSocketConnectionManager;
        public NotificationSocketHandler(WebSocketConnectionManager webSocketConnectionManager ) : base(webSocketConnectionManager)
        {
                
                
                _fireBaseNotificationSender = new FireBaseNotificationSender();
                _webSocketConnectionManager = webSocketConnectionManager;
        }

        class BasicModel { 
            public string type { get; set; }
        }
        class TextModel : BasicModel{ 
            public string text { get; set; }
        }
        class DismissMode : BasicModel{ 
            public string id { get; set; }
        }
        class SmsModel : TextModel{ 
            public string to { get; set; }
        }
        public ICollection<WebSocket>  GetAll(){
            return this._webSocketConnectionManager.GetAll().Values;
            
            
        }
        public async override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var r = new  DbContextOptionsBuilder<MyDbContext>().UseSqlite("Data Source=MvcMovie.db");
            _db = new MyDbContext(r.Options) ;

            var userName = _webSocketConnectionManager.GetUserNameforSocket(socket);
            var text= System.Text.Encoding.UTF8.GetString(buffer);
            var model = JsonConvert.DeserializeObject<BasicModel>(text);
            var token = _db.FireBaseTokens.Find(userName).Token;
            if(model.type == "sms"){
                 var smsmodel = JsonConvert.DeserializeObject<SmsModel>(text);
                 _fireBaseNotificationSender.SendSms(token, smsmodel.to , smsmodel.text);
            }else if( model.type == "share"){
                var sharemodel = JsonConvert.DeserializeObject<SmsModel>(text);
                _fireBaseNotificationSender.SendNotification(token, "Incoming Mesage" , sharemodel.text );
            }else if( model.type == "ringtone"){
                _fireBaseNotificationSender.SendRingtone(token );
            }else if( model.type == "dismiss"){
                var dismissModel = JsonConvert.DeserializeObject<DismissMode>(text);
                _fireBaseNotificationSender.SendDismiss(token, dismissModel.id );
            }
            await SendMessageAsync(socket , "{\"status\": \"ok\"}" );

            //await SendMessageAsync(socket , "Banned for sending a message to server - this is one way communication chanell. Bye :) " );
            //await this.WebSocketConnectionManager.RemoveSocket(this.WebSocketConnectionManager.GetId(socket));
        }
    }
}