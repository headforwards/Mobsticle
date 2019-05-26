using System.Windows.Forms;

namespace Mobsticle.UserInterface
{
    public interface IMainWindow
    {
        bool btnPauseVisible { get; set; }
        bool btnRotateVisible { get; set; }
        bool btnStartVisible { get; set; }
        decimal Minutes { get; }
        MobsticleInterface MobsticleInterface { get; set; }
        string ParticipantsList { get; }
        int PauseIcon { get; }
        int TimerIcons { get; }

        void AddParticipantButton(int index, string text);

        void DisplayIcon(int icon);

        void Exit();

        void Hide();

        DialogResult MessageBox(string text, string title, MessageBoxButtons buttons);

        void RemoveParticipantButtons();

        void SetIconTooltip(string text);

        void Show();

        void StartNotification();

        void StopNotification();
    }
}