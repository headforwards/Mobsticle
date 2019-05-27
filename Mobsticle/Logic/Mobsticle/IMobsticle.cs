﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Logic.Mobsticle
{
    public interface IMobsticle
    {
        MobsticleStatus Status { get; }

        decimal FractionElapsedTime { get; }

        IList<IParticipant> Participants { get; }

        void Rotate();

        void Rotate(int driver);

        void Pause();

        void Start();

        IMobsticleSettings Settings { get; set; }

        event EventHandler StatusChanged;

        event EventHandler ParticipantsChanged;

        event EventHandler TimeChanged;
    }
}