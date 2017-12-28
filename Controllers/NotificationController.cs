using System;
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
                    Title = model.title , Body = model.body , IsReflectedToDevice = false , recivedAt = DateTime.Now , Package = model.package , User = User.Identity.Name , 
                    Id = Guid.NewGuid()
                    };
                await _db.Notifications.AddAsync(notification);
                await _db.SaveChangesAsync();
                await _websocketHandler.SendMessageToAllSocketsWithTagAsync(User.Identity.Name , JsonConvert.SerializeObject(model));
                return Ok();
        }


    }
}