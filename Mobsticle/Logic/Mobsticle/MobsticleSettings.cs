using Mobsticle.Logic.Mobsticle;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Logic.Mobsticle
{
    public class MobsticleSettings : IMobsticleSettings
    {
        public int Minutes { get; set; } = 10;

        public IList<string> Participants { get; set; }

        public string Notification { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}