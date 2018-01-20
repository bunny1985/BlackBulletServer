using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationBackend.Infrastrucuture.Database;
using NotificationBackend.Infrastrucuture.Firebase;
using NotificationBackend.Models;

namespace NotificationBackend.Controllers
{
    [Route("/api/firebase")]
    [Authorize]
    public class FireBaseController: Controller
    {
        private readonly MyDbContext _db;

        public FireBaseController(MyDbContext db){
            _db = db;
        }

        [HttpPost]
        [Route("MyToken")]
        public async Task<IActionResult> UpdateDeviceIdToken([FromBody]TokenViewModel newToken){
            var userName = User.Identity.Name;
            //FInd and remove previous tokens for user 
            var entities =  _db.FireBaseTokens.Where( e => e.IdentittyName == userName);
            _db.FireBaseTokens.RemoveRange(entities);
            await _db.SaveChangesAsync();
            //inset new Token
            var dbModel = new TokenDbModel(){IdentittyName = userName , Token = newToken.token};
            await _db.AddAsync(dbModel);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("Notifications")]
        public async Task<IActionResult> SendNotificationToPhone([FromBody]  FirebaseNotificationViewModel model){
            var userName = User.Identity.Name;
            var token = _db.FireBaseTokens.Find(userName).Token;
            var service = new FireBaseNotificationSender();
            service.SendNotification(token, model.title , model.body );
            return Ok();
        }
        [HttpPost]
        [Route("Sms")]
        public async Task<IActionResult> SendSmsmRemotely([FromBody]  SmsmViewModel model){
            var userName = User.Identity.Name;
            var token = _db.FireBaseTokens.Find(userName).Token;
            var service = new FireBaseNotificationSender();
            service.SendSms(token, model.to , model.msg);
            return Ok();
        }
        [HttpPost]
        [Route("ringtone")]
        public async Task<IActionResult> PlayRingtonERemotely(){
            var userName = User.Identity.Name;
            var token = _db.FireBaseTokens.Find(userName).Token;
            var service = new FireBaseNotificationSender();
            service.SendRingtone(token);
            return Ok();
        }
    }
    
}