using System.ComponentModel.DataAnnotations;

namespace NotificationBackend.Models
{
    public class TokenDbModel
    {   [Key]
        public string IdentittyName { get; set; }
        public string Token {get;set;}
    }
}