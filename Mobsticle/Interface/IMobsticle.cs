using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Interface
{
    public interface IMobsticle
    {
        MobsticleStatus Status { get; }
        
        decimal FractionElapsedTime { get; }

        IList<IParticipant> Participants { get; }

        void Rotate();

        void Pause();

        void Start();

        IMobsticleSettings Settings { get; set; }

        event EventHandler StatusChanged;

        event EventHandler ParticipantsChanged;

        event EventHandler TimeChanged;
    }
}