namespace NotificationBackend.Models
{
    public class BatteryStatusViewModel
    {
        public BatteryStatusViewModel(){
            notificationType = "battery";
        }
        public string notificationType { get; set; }
        public string percent { get; set; }
        public string isCharging { get; set; }
    }
}