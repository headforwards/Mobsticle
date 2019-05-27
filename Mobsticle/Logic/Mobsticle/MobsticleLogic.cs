using Mobsticle.Logic.Timer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Logic.Mobsticle
{
    public class MobsticleLogic : IMobsticle
    {
        private decimal _lastPercentNotified;
        private List<Participant> _participants = new List<Participant>();
        private DateTime _pausedTime;
        private IMobsticleSettings _settings;
        private DateTime _startTime;
        private IMobsticleTimer _timer;

        public MobsticleLogic(IMobsticleTimer timer)
        {
            _timer = timer;
            _timer.Tick += TimerTick;
            Start();
        }

        public event EventHandler ParticipantsChanged;

        public event EventHandler StatusChanged;

        public event EventHandler TimeChanged;

        public decimal FractionElapsedTime
        {
            get
            {
                var p = (decimal)((_timer.Now - _startTime).TotalMinutes / _settings.Minutes);
                return p > 1 ? 1m : p;
            }
        }

        public IList<IParticipant> Participants
        {
            get
            {
                return _participants?.Cast<IParticipant>().ToList() ?? new List<IParticipant>();
            }
        }

        public IMobsticleSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                UpdateParticipantsList();
            }
        }

        public MobsticleStatus Status { get; private set; } = MobsticleStatus.Paused;

        public void Pause()
        {
            if (Status != MobsticleStatus.Paused)
            {
                Status = MobsticleStatus.Paused;
                _pausedTime = _timer.Now;
                OnStatusChanged(this, new EventArgs());
            }
        }

        public void Rotate()
        {
            RotateImpl();
        }

        public void Rotate(int newIndex)
        {
            RotateImpl(newIndex);
        }

        private void RotateImpl(int? newIndex = null)
        {
            if (_participants.Count > 0)
            {
                var i = _participants.IndexOf(_participants.Single(x => x.IsDriving));
                var n = _participants.IndexOf(_participants.Single(x => x.IsDrivingNext));
                var ni = newIndex ?? (i < _participants.Count - 1 ? i + 1 : 0);
                var nni = ni < _participants.Count - 1 ? ni + 1 : 0;
                _participants[i].IsDriving = false;
                _participants[n].IsDrivingNext = false;
                _participants[ni].IsDriving = true;
                _participants[nni].IsDrivingNext = true;
                OnParticipantsChanged(this, new EventArgs());
            }
            if (Status != MobsticleStatus.Running)
            {
                Status = MobsticleStatus.Running;
                OnStatusChanged(this, new EventArgs());
            }
            _startTime = _timer.Now;
            _lastPercentNotified = 0;
            OnTimeChanged(this, new EventArgs());
        }

        public void Start()
        {
            if (Status != MobsticleStatus.Running)
            {
                Status = MobsticleStatus.Running;
                if (Status == MobsticleStatus.Expired)
                    _startTime = _timer.Now;
                else
                    _startTime = _startTime + (_timer.Now - _pausedTime);
                OnStatusChanged(this, new EventArgs());
            }
        }

        private void OnParticipantsChanged(object o, EventArgs e)
        {
            var evt = ParticipantsChanged;
            if (evt != null)
            {
                evt.Invoke(o, e);
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

        private void OnTimeChanged(object o, EventArgs e)
        {
            var evt = TimeChanged;
            if (evt != null)
            {
                evt.Invoke(o, e);
            }
        }

        private void TimerTick(object source, EventArgs evt)
        {
            var percent = Math.Floor(FractionElapsedTime * 100) / 100;
            if (Status == MobsticleStatus.Running && percent > _lastPercentNotified)
            {
                _lastPercentNotified = percent;
                OnTimeChanged(this, new EventArgs());
            }
            if (Status == MobsticleStatus.Running && percent >= 1)
            {
                Status = MobsticleStatus.Expired;
                OnStatusChanged(this, new EventArgs());
            }
        }

        private void UpdateParticipantsList()
        {
            bool changed = false;
            foreach (var participant in _participants.ToList())
            {
                if (!Settings.Participants.Contains(participant.Name))
                {
                    _participants.Remove(participant);
                    changed = true;
                }
            }
            if (Settings.Participants != null)
            {
                foreach (var participant in Settings.Participants)
                {
                    var existing = _participants.SingleOrDefault(x => x.Name == participant);
                    if (existing == null)
                    {
                        _participants.Add(new Participant { Name = participant });
                        changed = true;
                    }
                    else
                    {
                        var index = Settings.Participants.IndexOf(participant);
                        if (index != _participants.IndexOf(existing))
                        {
                            _participants.Remove(existing);
                            _participants.Insert(index, existing);
                            changed = true;
                        }
                    }
                }
            }
            if (!_participants.Any(x => x.IsDriving))
            {
                if (_participants.Count > 0)
                    _participants[0].IsDriving = true;
                if (_participants.Count > 1)
                    _participants[1].IsDrivingNext = true;
            }
            if (changed)
                OnParticipantsChanged(this, new EventArgs());
        }
    }
}