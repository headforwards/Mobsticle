using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Interface
{
    public interface IMobsticleTimer
    {
        event EventHandler Tick;

        DateTime Now { get; }
    }
}
