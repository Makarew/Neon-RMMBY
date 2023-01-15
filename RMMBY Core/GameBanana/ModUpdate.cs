using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMMBY.GameBanana
{
    public class ModUpdate
    {
        [JsonProperty("_sVersion")]
        public string Version { get; set; }
    }
}
