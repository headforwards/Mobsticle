using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.UserInterface.Notification
{
    public class SoundNotifier : ISoundNotifier
    {
        public string Notification { get; set; }

        public IDictionary<string, string> Notifications { get; private set; }

        private INotificationSettings _settings;

        public INotificationSettings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                setNotification(_settings.Notification);
            }
        }

        private SoundPlayer _player = new SoundPlayer();

        public SoundNotifier()
        {
            loadNotifications();
        }

        public void StartNotification()
        {
            _player.PlayLooping();
        }

        public void StopNotification()
        {
            _player.Stop();
        }

        private void loadNotifications()
        {
            var dictionary = new Dictionary<string, string>();
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var prefix = assembly.GetName().Name + ".Resources.";
            foreach (var wav in assembly.GetManifestResourceNames().Where(name => name.EndsWith(".wav")))
            {
                dictionary.Add(wav.Substring(prefix.Length, wav.Length - (prefix.Length + 4)), wav);
            }
            Notifications = dictionary;
        }

        private void setNotification(string name)
        {
            if (name != null)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                _player.Stream = assembly.GetManifestResourceStream(name);
            }
        }
    }
}