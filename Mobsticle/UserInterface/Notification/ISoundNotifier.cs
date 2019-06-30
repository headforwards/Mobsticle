using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.UserInterface.Notification
{
    public interface ISoundNotifier
    {
        string Notification { get; set; }

        IDictionary<string, string> Notifications { get; }

        void StartNotification();

        void StopNotification();

        INotificationSettings Settings { get; set; }
    }
}