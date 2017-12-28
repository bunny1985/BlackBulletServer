using System.ComponentModel.DataAnnotations;

namespace NotificationBackend.Models
{
    public class SettingsDbModel
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}