using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NotificationBackend.Infrastrucuture.Database;
using NotificationBackend.Infrastrucuture.WebSockets;
using NotificationBackend.Models;

namespace NotificationBackend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController: Controller
    {
        private readonly MyDbContext _db;
        private readonly NotificationSocketHandler _websocketHandler;

        public NotificationController(MyDbContext db , NotificationSocketHandler websocketHandler)
        {
            _db = db;
            _websocketHandler = websocketHandler;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> AddNotification([FromBody]NotificationViewModel model)
        {
                var notification = new NotificationDbModel(){ 
                    Title = model.title , Body = model.body , IsReflectedToDevice = false , recivedAt = DateTime.Now , Package = model.package , User = User.Identity.Name, 
                    Id = Guid.NewGuid()
                    };
                
                model.notificationType = "notification";
                await _websocketHandler.SendMessageToAllSocketsWithTagAsync(User.Identity.Name , JsonConvert.SerializeObject(model));
                return Ok();
        }
        [HttpPost]
        [Route("SetClipboard")]
        public async Task<IActionResult> SetClipBoard([FromBody]ClipboardViewModel model)
        {
                model.notificationType = "clipboard";
                await _websocketHandler.SendMessageToAllSocketsWithTagAsync(User.Identity.Name , JsonConvert.SerializeObject(model));
                return Ok();
        }

        [HttpPost]
        [Route("IsMySocketOk")]
        public async Task<IActionResult> GetOpenSockets()
        {       
            var model = new{notificationType= "ping"};
                await _websocketHandler.SendMessageToAllSocketsWithTagAsync(User.Identity.Name , JsonConvert.SerializeObject(model));
                var count = _websocketHandler.GetAll().Select(w => w.ToString()).ToArray().Count();
                if(count>0)return Ok(new{ Ok= true});
                return Ok(new{ Ok= false});
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetLatestNotifications()
        {
                var dateSince = DateTime.Now.Add( - new TimeSpan(0 , 5 , 0));
                var notifications = _db.Notifications.Where(r => r.recivedAt >=  dateSince);
                
                
                return Ok(notifications);
        }


    }
}