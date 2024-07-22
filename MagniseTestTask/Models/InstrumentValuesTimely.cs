using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace FintachartsApi.Models
{
    public class Root
    {
        public List<InstrumentValuesTimely> data { get; set; }
    }

    public class InstrumentValuesTimely
    {
        [JsonIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime t { get; set; }
        public float o { get; set; }
        public float h { get; set; }
        public float l { get; set; }
        public float c { get; set; }
        public int v { get; set; }
        [JsonIgnore]
        public Instrument? instrument { get; set; }
    }
}
