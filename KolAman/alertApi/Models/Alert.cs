using System.Text.Json.Serialization;

namespace alertApi.Models
{
    public class Alert
    {
      
        public string AlertId { get; set; } = string.Empty;

       
        public string Source { get; set; } = string.Empty;

      
        public string Title { get; set; } = string.Empty;

       
        public string Content { get; set; } = string.Empty;

      
        public string Priority { get; set; } = string.Empty;

        
        public string Classification { get; set; } = string.Empty;

        public double Lat { get; set; }

      
        public double Lon { get; set; }

      
        public DateTime Timestamp { get; set; }

 
        public string Status { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;

    }
}
