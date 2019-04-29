using Mobsticle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Logic
{
    public class MobsticleLogic : IMobsticle
    {
        public MobsticleStatus Status { get; private set; } = MobsticleStatus.Running;

        public int ElapsedTime => throw new NotImplementedException();

        public int TotalTime => TimeSpan.FromMinutes(Settings.Minutes).Seconds;

        public IMobsticleSettings Settings { get; set; }

        public event EventHandler StatusChanged;
        public event EventHandler TimeChanged;
        public event EventHandler ParticipantsChanged;
        public event EventHandler<MobsticleNotification> Notify;        

        public void Pause()
        {
            if (Status != MobsticleStatus.Paused)
            {
                Status = MobsticleStatus.Paused;
                OnStatusChanged(this, new EventArgs());
            }            
        }

        public void Rotate()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            if (Status != MobsticleStatus.Running)
            {
                Status = MobsticleStatus.Running;
                OnStatusChanged(this, new EventArgs());
            }
        }

        private void OnStatusChanged(object o, EventArgs e)
        {
            var evt = StatusChanged;
            if (evt != null)
            {
                evt.Invoke(o, e);
            }
        }
    }
}
