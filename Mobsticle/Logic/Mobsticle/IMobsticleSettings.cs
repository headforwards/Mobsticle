using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Logic.Mobsticle
{
    public interface IMobsticleSettings
    {
        int Minutes { get; }

        IList<string> Participants { get; }

    }
}
