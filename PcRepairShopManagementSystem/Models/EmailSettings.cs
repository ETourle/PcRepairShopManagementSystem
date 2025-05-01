namespace PcRepairShopManagementSystem.Models
{
    public class EmailSettings
    {
        public string Host { get; set; }      
        public int Port { get; set; }      
        public bool EnableSSL { get; set; }// true
        public string UserName { get; set; }  
        public string Password { get; set; }
        public int Timeout { get; set; } = 10000; // Default timeout of 10 seconds
    }
}
