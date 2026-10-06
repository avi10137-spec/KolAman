using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace consumerrabbit.Models
{
    using System;
    using System.Text.Json.Serialization;

    namespace consumerrabbit.Models
    {
        public class Alert
        {
            [JsonPropertyName("alert_id")]
            public string AlertId { get; set; } = string.Empty;

            [JsonPropertyName("source")]
            public string Source { get; set; } = string.Empty;

            [JsonPropertyName("title")]
            public string Title { get; set; } = string.Empty;

            [JsonPropertyName("content")]
            public string Content { get; set; } = string.Empty;

            [JsonPropertyName("priority")]
            public string Priority { get; set; } = string.Empty;

            [JsonPropertyName("classification")]
            public string Classification { get; set; } = string.Empty;

            [JsonPropertyName("lat")]
            public double Lat { get; set; }

            [JsonPropertyName("lon")]
            public double Lon { get; set; }

            [JsonPropertyName("timestamp")]
            public DateTime Timestamp { get; set; }

            [JsonPropertyName("status")]
            public string Status { get; set; } = string.Empty;
            public string Command { get; set; } = string.Empty;
            
        }
    }
}
