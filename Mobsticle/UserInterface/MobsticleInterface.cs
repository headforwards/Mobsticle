using Mobsticle.Logic.Mobsticle;
using Mobsticle.Logic.Notification;
using Mobsticle.Logic.SettingsStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Mobsticle.UserInterface
{
    public class MobsticleInterface
    {
        private const string CURRENT = " (Current)";
        private const string NEXT = " (Next)";

        private IMainWindow _mainWindow;
        private ISoundNotifier _soundNotifier;
        private IMobsticle _mobsticle;
        private bool _okClose;
        private ISettingsStore _store;

        public MobsticleInterface(IMainWindow mainWindow, IMobsticle mobsticle, ISettingsStore store, ISoundNotifier soundNotifier)
        {
            _mainWindow = mainWindow;
            _soundNotifier = soundNotifier;
            _mobsticle = mobsticle;
            _store = store;

            _mobsticle.StatusChanged += StatusChanged;
            _mobsticle.TimeChanged += TimeChanged;
            _mobsticle.ParticipantsChanged += ParticipantsChanged;

            var settings = _store.Load<MobsticleSettings>();
            _mobsticle.Settings = settings;
            _mainWindow.ParticipantsList = string.Join(Environment.NewLine, settings.Participants ?? new string[] { });
            _mainWindow.Minutes = settings.Minutes;
            _mainWindow.Notifications = _soundNotifier.Notifications;
            _soundNotifier.Notification = settings.Notification;

            _mainWindow.btnPauseVisible = true;
            _mainWindow.btnRotateVisible = false;
            _mainWindow.btnStartVisible = false;
        }

        public void btnCloseClick()
        {
            _mainWindow.Hide();
            var settings = new MobsticleSettings();
            settings.Participants = Split(_mainWindow.ParticipantsList);
            settings.Minutes = (int)_mainWindow.Minutes;
            settings.Notification = _mainWindow.Notification;
            _mobsticle.Settings = settings;
            _soundNotifier.Settings = settings;
            _store.Save(settings);
        }

        public void btnParticipantClick(int index)
        {
            _mobsticle.Rotate(index);
        }

        public void btnPauseClick()
        {
            _mobsticle.Pause();
        }

        public void btnRotateClick()
        {
            _mobsticle.Rotate();
        }

        public void btnSettingsClick()
        {
            _mainWindow.Show();
        }

        public void btnStartClick()
        {
            _mobsticle.Start();
        }

        public void formClosing(object sender, FormClosingEventArgs e)
        {
            if (!_okClose)
            {
                _mainWindow.Hide();
                e.Cancel = true;
            }
        }

        public void menuClose()
        {
            if (_mainWindow.MessageBox("Are you sure you want to exit?", "Mobsticle", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _okClose = true;
                _mainWindow.Exit();
            }
        }

        private static List<string> Split(string input)
        {
            return Regex.Split(input, Environment.NewLine).Select(s => s.Trim()).Where(x => !string.IsNullOrWhiteSpace(x) && Regex.IsMatch(x, "^[\\w\\s\\d]{1,30}$")).Distinct().ToList();
        }

        private void ParticipantsChanged(object sender, EventArgs e)
        {
            _mainWindow.RemoveParticipantButtons();
            int i = 0;
            foreach (var partipant in _mobsticle.Participants)
            {
                var text = partipant.Name + (_mobsticle.Participants.Count > 1 ? partipant.IsDriving ? CURRENT : partipant.IsDrivingNext ? NEXT : null : null);
                _mainWindow.AddParticipantButton(i, text);
                i++;
            }
            _mainWindow.SetIconTooltip(_mobsticle.Participants.Count > 1 ? _mobsticle.Participants.SingleOrDefault(m => m.IsDriving)?.Name + " (Current) / " +
                _mobsticle.Participants.SingleOrDefault(m => m.IsDrivingNext)?.Name + " (Next)" : "Mobsticle");
        }

        private void StatusChanged(object sender, EventArgs e)
        {
            switch (_mobsticle.Status)
            {
                case MobsticleStatus.Expired:
                    _soundNotifier.StartNotification();
                    _mainWindow.btnRotateVisible = true;
                    _mainWindow.btnPauseVisible = false;
                    _mainWindow.btnStartVisible = false;
                    break;

                case MobsticleStatus.Paused:
                    _mainWindow.DisplayIcon(_mainWindow.PauseIcon);
                    _mainWindow.btnRotateVisible = false;
                    _mainWindow.btnPauseVisible = false;
                    _mainWindow.btnStartVisible = true;
                    break;

                case MobsticleStatus.Running:
                    _soundNotifier.StopNotification();
                    _mainWindow.DisplayIcon((int)(_mainWindow.TimerIcons * _mobsticle.FractionElapsedTime));
                    _mainWindow.btnRotateVisible = false;
                    _mainWindow.btnPauseVisible = true;
                    _mainWindow.btnStartVisible = false;
                    break;
            }
        }

        private void TimeChanged(object sender, EventArgs e)
        {
            _mainWindow.DisplayIcon((int)(_mainWindow.TimerIcons * _mobsticle.FractionElapsedTime));
        }
    }
}