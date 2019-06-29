using Mobsticle.Logic.Mobsticle;
using Mobsticle.Logic.Notification;
using Mobsticle.Logic.SettingsStore;
using Mobsticle.Logic.Timer;
using Mobsticle.UserInterface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Mobsticle
{
    public partial class Main : Form, IMainWindow
    {
        private const int _pausedIcon = _sections + 1;
        private const int _sections = 24;

        private Icon[] _icons16;

        private Icon[] _icons48;

        private List<ToolStripItem> _participantMenuItems = new List<ToolStripItem>();

        public Main()
        {
            InitializeComponent();
            _icons16 = createIcons(16, _sections);
            _icons48 = createIcons(48, _sections);
            notifyIcon.Icon = _icons16[0];
            notifyIcon.Visible = true;
            Icon = _icons48[0];

            var t = new MobsticleTimer();
            timer.Tick += t.OnTick;
            var mobsticle = new MobsticleLogic(t);
            var store = new Store(new FilePersistence());
            var soundNotifier = new SoundNotifier();
            var _interface = new MobsticleInterface(this, mobsticle, store, soundNotifier);
            MobsticleInterface = _interface;

            timer.Start();
        }

        public bool btnPauseVisible { get => mniPause.Visible; set => mniPause.Visible = value; }
        public bool btnRotateVisible { get => mniRotate.Visible; set => mniRotate.Visible = value; }
        public bool btnStartVisible { get => mniStart.Visible; set => mniStart.Visible = value; }

        public decimal Minutes { get => numMinutes.Value; set => numMinutes.Value = value; }

        public MobsticleInterface MobsticleInterface { get; set; }

        public string Notification
        {
            get => ((KeyValuePair<string, string>)cboNotification.SelectedItem).Value;
            set => cboNotification.SelectedItem = cboNotification.Items.Cast<KeyValuePair<string, string>>().SingleOrDefault(k => k.Value == value);
        }

        public IDictionary<string, string> Notifications
        {
            set
            {
                cboNotification.Items.Clear(); foreach (var kvp in value)
                {
                    cboNotification.Items.Add(kvp);
                }
            }
        }

        public string ParticipantsList { get => txtParticipants.Text; set => txtParticipants.Text = value; }

        public int PauseIcon => _pausedIcon;

        public int TimerIcons => _sections;

        public void AddParticipantButton(int index, string text)
        {
            if (_participantMenuItems.Count == 0)
            {
                var sep = new ToolStripSeparator();
                _participantMenuItems.Add(sep);
                contextMenuStrip.Items.Insert(index, sep);
            }
            var menuItem = new ToolStripMenuItem(text);
            _participantMenuItems.Add(menuItem);
            contextMenuStrip.Items.Insert(index, menuItem);
            menuItem.Click += (o, e) => MobsticleInterface.btnParticipantClick(index);
        }

        public void DisplayIcon(int icon)
        {
            setIcon(icon);
        }

        public void Exit()
        {
            Application.Exit();
        }

        public DialogResult MessageBox(string text, string title, MessageBoxButtons buttons)
        {
            return System.Windows.Forms.MessageBox.Show(text, title, buttons);
        }

        public void RemoveParticipantButtons()
        {
            foreach (var mni in _participantMenuItems)
            {
                contextMenuStrip.Items.Remove(mni);
            }
            _participantMenuItems.Clear();
        }

        public void SetIconTooltip(string text)
        {
            notifyIcon.Text = text;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            MobsticleInterface.btnCloseClick();
        }

        private Icon[] createIcons(int size, int sections)
        {
            var icons = new List<Icon>();
            using (var red = new SolidBrush(Color.IndianRed))
            using (var green = new SolidBrush(Color.PaleGreen))
            using (var white = new SolidBrush(Color.White))
            {
                for (int i = 0; i <= _sections; i++)
                {
                    using (var bitmap = new Bitmap(size, size))
                    using (var g = Graphics.FromImage(bitmap))
                    {
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        float arc = (360f / sections) * i;
                        if (i < _sections)
                        {
                            g.FillPie(green, new Rectangle(0, 0, size - 1, size - 1), 270 + arc, 360 - arc);
                        }
                        if (i > 0)
                        {
                            g.FillPie(red, new Rectangle(0, 0, size - 1, size - 1), 270, arc);
                        }
                        icons.Add(Icon.FromHandle(bitmap.GetHicon()));
                    }
                }

                using (var bitmap = new Bitmap(size, size))
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillEllipse(red, new Rectangle(0, 0, size - 1, size - 1));
                    var o1 = (int)(size * 0.2);
                    var o2 = (int)(size * 0.6);
                    g.FillRectangle(white, new Rectangle(o1, o1, o1, o2));
                    g.FillRectangle(white, new Rectangle(o2, o1, o1, o2));
                    icons.Add(Icon.FromHandle(bitmap.GetHicon()));
                }
            }
            return icons.ToArray();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MobsticleInterface.formClosing(sender, e);
        }

        private void mniExit_Click(object sender, EventArgs e)
        {
            MobsticleInterface.menuClose();
        }

        private void mniPause_Click(object sender, EventArgs e)
        {
            MobsticleInterface.btnPauseClick();
        }

        private void MniRotate_Click(object sender, EventArgs e)
        {
            MobsticleInterface.btnRotateClick();
        }

        private void mniSettings_Click(object sender, EventArgs e)
        {
            MobsticleInterface.btnSettingsClick();
        }

        private void MniStart_Click(object sender, EventArgs e)
        {
            MobsticleInterface.btnStartClick();
        }

        private void setIcon(int icon)
        {
            notifyIcon.Icon = _icons16[icon];
            Icon = _icons48[icon];
        }

        private void NotifyIcon_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                MobsticleInterface.btnIconClick();
        }

        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                MobsticleInterface.btnIconDoubleClick();
        }
    }
}