namespace Banking.Account.Query.Application.Models
{
    public class KafkaSettings
    {
        public string GroupId { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;    
    }
}
