using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Interfaces
{
    public interface IMobsticle
    {
        MobsticleStatus Status { get; }

        int ElapsedTime { get; }

        int TotalTime { get; }

        void Rotate();

        void Pause();

        void Start();

        IMobsticleSettings Settings { get; set; }

        event EventHandler StatusChanged;

        event EventHandler TimeChanged;

        event EventHandler ParticipantsChanged;

        event EventHandler<MobsticleNotification> Notify;
    }

    public class MobsticleNotification : EventArgs
    {
        public string FileName { get; set; }
    }
}
