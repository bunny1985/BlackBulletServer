using System;
using System.ComponentModel.DataAnnotations;

namespace NotificationBackend.Models
{
    public class NotificationDbModel
    {
        [Key]
        public Guid Id { get; set; }
        public string User { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Package { get; set; }
        public bool IsReflectedToDevice { get; set; }
        public DateTime recivedAt{ get;set;}
    }
}