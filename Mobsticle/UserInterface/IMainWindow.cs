using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mobsticle.UserInterface
{
    public interface IMainWindow
    {
        MobsticleInterface MobsticleInterface { get; set; }

        void Hide();

        string ParticipantsList { get; }

        decimal Minutes { get; }

        DialogResult MessageBox(string text, string title, MessageBoxButtons buttons);

        void Exit();
    }
}