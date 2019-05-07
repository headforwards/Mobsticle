using Mobsticle.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Logic
{
    public class Participant : IParticipant
    {
        public string Name { get; set; }

        public bool IsDriving { get; set; }

        public bool IsDrivingNext { get; set; }
    }
}
