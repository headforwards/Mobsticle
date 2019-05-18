using Mobsticle.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Logic
{
    public class MobsticleSettings : IMobsticleSettings
    {
        public int Minutes { get; set; }

        public IList<string> Participants { get; set; }
    }
}
