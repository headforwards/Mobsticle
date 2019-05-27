using Mobsticle.Logic.Mobsticle;
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

        private SoundPlayer _player = new SoundPlayer();

        public Main()
        {
            InitializeComponent();
            _icons16 = createIcons(16, _sections);
            _icons48 = createIcons(48, _sections);
            notifyIcon.Icon = _icons16[0];
            notifyIcon.Visible = true;
            Icon = _icons48[0];
            loadNotifications();

            var t = new MobsticleTimer();
            timer.Tick += t.OnTick;
            var mobsticle = new MobsticleLogic(t);
            var store = new Store(new FilePersistence());
            var _interface = new MobsticleInterface(this, mobsticle, store);
            MobsticleInterface = _interface;

            var settings = store.Load<MobsticleSettings>();
            var item = cboNotification.Items.Cast<object>().SingleOrDefault(x => ((KeyValuePair<string, string>)x).Value == settings.Notification);
            if (item != null)
            {
                cboNotification.SelectedIndex = cboNotification.Items.IndexOf(item);
            }

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
            get => cboNotification.Items.Cast<KeyValuePair<string, string>>().ToDictionary(x => x.Key, x => x.Value);
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

        public void StartNotification()
        {
            _player.PlayLooping();
        }

        public void StopNotification()
        {
            _player.Stop();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (cboNotification.SelectedItem != null)
            {
                setNotification(((KeyValuePair<string, string>)cboNotification.SelectedItem).Value);
            }
            MobsticleInterface.btnCloseClick();
        }

        private Icon[] createIcons(int size, int sections)
        {
            var icons = new List<Icon>();
            using (var br = new SolidBrush(Color.IndianRed))
            using (var bg = new SolidBrush(Color.PaleGreen))
            //using (var br = new SolidBrush(Color.FromArgb(255, 255, 100, 100)))
            //using (var bg = new SolidBrush(Color.FromArgb(200, 200, 255, 255)))
            //using (var bb = new SolidBrush(Color.DarkGray))
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
                            g.FillPie(bg, new Rectangle(0, 0, size - 1, size - 1), 270 + arc, 360 - arc);
                        }
                        if (i > 0)
                        {
                            g.FillPie(br, new Rectangle(0, 0, size - 1, size - 1), 270, arc);
                        }
                        //var p = new GraphicsPath();
                        //var fac = (int) (size * 0.3);
                        //p.AddEllipse(new Rectangle(fac, fac, size - (1 + fac * 2), size - (1 + fac * 2)));
                        //g.SetClip(p);
                        //g.Clear(Color.Transparent);
                        //g.ResetClip();
                        icons.Add(Icon.FromHandle(bitmap.GetHicon()));
                    }
                }

                using (var bitmap = new Bitmap(size, size))
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillEllipse(br, new Rectangle(0, 0, size - 1, size - 1));
                    var o1 = (int)(size * 0.2);
                    var o2 = (int)(size * 0.6);
                    g.FillRectangle(bg, new Rectangle(o1, o1, o1, o2));
                    g.FillRectangle(bg, new Rectangle(o2, o1, o1, o2));
                    //var p = new GraphicsPath();
                    icons.Add(Icon.FromHandle(bitmap.GetHicon()));
                }
            }
            return icons.ToArray();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MobsticleInterface.formClosing(sender, e);
        }

        private void loadNotifications()
        {
            var list = new List<KeyValuePair<string, string>>();
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var prefix = assembly.GetName().Name + ".Resources.";
            foreach (var wav in assembly.GetManifestResourceNames().Where(name => name.EndsWith(".wav")))
            {
                list.Add(new KeyValuePair<string, string>(wav.Substring(prefix.Length, wav.Length - (prefix.Length + 4)), wav));
            }
            list.Sort((i1, i2) => i1.Key.CompareTo(i2.Key));
            cboNotification.Items.AddRange(list.Cast<object>().ToArray());
            cboNotification.ValueMember = "Value";
            cboNotification.DisplayMember = "Key";
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
            //Show();
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