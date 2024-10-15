
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagniseTestTaskFintacharts.Models
{
    public class WebSocketMessage
    {
        public string type { get; set; }
        public string id { get; set; }
        public string instrumentalId { get; set; }
        public string provider { get; set; }
        public bool subscribe { get; set; }
        [NotMapped]
        public List<string> kinds { get; set; }
        [JsonIgnore]
        public Instrument instrument { get; set; }
    }
}
