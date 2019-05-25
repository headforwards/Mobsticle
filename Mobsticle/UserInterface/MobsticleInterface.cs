using Mobsticle.Logic.Mobsticle;
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
        private IMainWindow _mainWindow;
        private IMobsticle _mobsticle;
        private bool _okClose;
        private ISettingsStore _store;

        public MobsticleInterface(IMainWindow mainWindow, IMobsticle mobsticle, ISettingsStore store)
        {
            _mainWindow = mainWindow;
            _mobsticle = mobsticle;
            _store = store;
        }

        public void btnCloseClick()
        {
            _mainWindow.Hide();
            var settings = new MobsticleSettings();
            settings.Participants = Split(_mainWindow.ParticipantsList);
            settings.Minutes = (int)_mainWindow.Minutes;
            _mobsticle.Settings = settings;
            _store.Save(settings);
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
    }
}