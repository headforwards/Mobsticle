using Mobsticle.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Logic
{
    public class MobsticleTimer : IMobsticleTimer
    {
        public DateTime Now => DateTime.Now;

        public event EventHandler Tick;

        public void OnTick(object o, EventArgs e)
        {
            var handler = Tick;
            if (Tick != null)
            {
                handler(this, new EventArgs());
            }
        }
    }
}
