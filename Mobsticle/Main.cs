using Mobsticle.Interface;
using Mobsticle.Logic;
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
    public partial class Main : Form
    {
        private const int _sections = 24;
        const string CURRENT = " (Current)";
        const string NEXT = " (Next)";
        const string PAUSE = "Pause";
        const string ROTATE = "Rotate";
        const string START = "Start";
        private Icon[] _icons16;
        private Icon[] _icons48;
        private bool _okClose = false;
        private SoundPlayer _player = new SoundPlayer();
        private IMobsticle _mobsticle;

        public Main()
        {
            InitializeComponent();
            _icons16 = createPieIcons(16, _sections);
            _icons48 = createPieIcons(48, _sections);
            notifyIcon.Icon = _icons16[0];
            notifyIcon.Visible = true;
            Icon = _icons48[0];
            loadNotifications();

            var t = new MobsticleTimer();
            timer.Tick += t.OnTick;
            _mobsticle = new MobsticleLogic(t);
            _mobsticle.Settings = new MobsticleSettings { Minutes = 10, Participants = new List<string>() };
            _mobsticle.StatusChanged += (o,e) => statusChanged();
            _mobsticle.ParticipantsChanged += (o, e) => participantsChanged();
            _mobsticle.TimeChanged += (o, e) => timeChanged();
            timer.Start();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
            var settings = new MobsticleSettings();
            settings.Participants = split(txtParticipants.Text);
            settings.Minutes = (int) numMinutes.Value;
            _mobsticle.Settings = settings;

            
        }
        
        private void cboNotification_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboNotification.SelectedItem != null)
            {
                setNotification(((Tuple<string, string>)cboNotification.SelectedItem).Item1);
            }
        }

        private Icon[] createPieIcons(int size, int sections)
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
                        var p = new GraphicsPath();
                        //var fac = (int) (size * 0.3);
                        //p.AddEllipse(new Rectangle(fac, fac, size - (1 + fac * 2), size - (1 + fac * 2)));
                        //g.SetClip(p);
                        //g.Clear(Color.Transparent);
                        //g.ResetClip();
                        icons.Add(Icon.FromHandle(bitmap.GetHicon()));
                    }
                }
            }
            return icons.ToArray();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_okClose)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            txtParticipants.Text = string.Join(Environment.NewLine, _mobsticle.Participants.Select(p => p.Name));
            numMinutes.Value = _mobsticle.Settings.Minutes;
        }

        private void loadNotifications()
        {
            var list = new List<Tuple<string, string>>();
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var prefix = assembly.GetName().Name + ".Resources.";
            foreach (var wav in assembly.GetManifestResourceNames().Where(name => name.EndsWith(".wav")))
            {
                list.Add(Tuple.Create(wav, wav.Substring(prefix.Length, wav.Length - (prefix.Length + 4))));
            }
            list.Sort((i1, i2) => i1.Item1.CompareTo(i2.Item1));
            cboNotification.Items.AddRange(list.ToArray());
            cboNotification.ValueMember = "Item1";
            cboNotification.DisplayMember = "Item2";
        }

        private void mniExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Mobsticle", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _okClose = true;
                Application.Exit();
            }
        }

        private void mniPause_Click(object sender, EventArgs e)
        {
            switch (mniPause.Text)
            {
                case PAUSE:
                    _mobsticle.Pause();
                    break;
                case START:
                    _mobsticle.Start();
                    break;
                case ROTATE:
                    _mobsticle.Rotate();
                    break;
            }
        }

        private void statusChanged()
        {
            switch (_mobsticle.Status)
            {
                case MobsticleStatus.Expired:
                    _player.PlayLooping();
                    mniPause.Text = ROTATE;
                    break;
                case MobsticleStatus.Paused:
                    mniPause.Text = START;
                    break;
                case MobsticleStatus.Running:
                    _player.Stop();
                    mniPause.Text = PAUSE;
                    break;
            }
        }

        private void participantsChanged()
        {
            for (int i = contextMenuStrip.Items.Count; i > 3; i--)
                contextMenuStrip.Items.RemoveAt(i - 1);
            if (_mobsticle.Participants.Count > 0)
                contextMenuStrip.Items.Add(new ToolStripSeparator());
            foreach (var participant in _mobsticle.Participants)
            {
                var item = new ToolStripMenuItem(textFor(participant));
                contextMenuStrip.Items.Add(item);
            }
        }

        private void timeChanged()
        {
            var section = (int)(_sections * _mobsticle.FractionElapsedTime);
            notifyIcon.Icon = _icons16[section];
            Icon = _icons48[section];
        }

        private void mniSettings_Click(object sender, EventArgs e)
        {
            Show();
        }

        private void setNotification(string name)
        {
            if (name != null)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                _player.Stream = assembly.GetManifestResourceStream(name);
            }
        }
        private List<string> split(string input)
        {
            return Regex.Split(input, Environment.NewLine).Select(s => s.Trim()).Where(x => !string.IsNullOrWhiteSpace(x) && Regex.IsMatch(x, "[\\w\\s\\d]{1,30}")).Distinct().ToList();
        }

        private string textFor(IParticipant participant)
        {
            if (participant.IsDriving)
                return participant.Name + CURRENT;
            if (participant.IsDrivingNext)
                return participant.Name + NEXT;
            return participant.Name;
        }
    }
}
