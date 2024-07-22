using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FintachartsApi.Models
{
    public class Provider
    {
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [JsonIgnore]
        public Instrument Instrument { get; set; }
        public string Name { get; set; }
        public string symbol { get; set; }
        public string exchange { get; set; }
        public int? defaultOrderSize { get; set; }
    }
}
