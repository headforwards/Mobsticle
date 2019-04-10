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
    public partial class Form1 : Form
    {
        private const int _sections = 24;
        const string CURRENT = " (Current)";
        const string NEXT = " (Next)";
        const string PAUSE = "Pause";
        const string ROTATE = "Rotate";
        const string START = "Start";
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private Icon[] _icons16;
        private Icon[] _icons48;
        private bool _isPaused = false;
        private bool _okClose = false;
        private string _participant;
        private List<string> _participants = new List<string>();
        private SoundPlayer _player = new SoundPlayer();

        private DateTime _startFrom;
        private TimeSpan _totalTime = TimeSpan.FromMinutes(10);
        public Form1()
        {
            InitializeComponent();
            _icons16 = createPieIcons(16, _sections);
            _icons48 = createPieIcons(48, _sections);
            notifyIcon.Icon = _icons16[0];
            notifyIcon.Visible = true;
            Icon = _icons48[0];
            loadNotifications();
            Start();
        }




        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();

            _participants = split(txtParticipants.Text);
            _totalTime = TimeSpan.FromMinutes((int)numMinutes.Value);

            for (int i = contextMenuStrip.Items.Count; i > 3; i--)
                contextMenuStrip.Items.RemoveAt(i - 1);
            if (_participants.Count > 0)
                contextMenuStrip.Items.Add(new ToolStripSeparator());
            if (!_participants.Contains(_participant))
                _participant = _participants.FirstOrDefault();
            foreach (var participant in _participants)
            {
                var item = new ToolStripMenuItem(textFor(participant));
                item.Click += participantClick;
                item.Tag = participant;
                contextMenuStrip.Items.Add(item);
            }
        }

        private void Buzz()
        {
            timer.Stop();
            mniPause.Text = ROTATE;
            //SystemSounds.Exclamation.Play();            
            _player.PlayLooping();
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
            txtParticipants.Text = string.Join(Environment.NewLine, _participants);
            numMinutes.Value = (int)_totalTime.TotalMinutes;
        }

        private void loadNotifications()
        {
            var list = new List<Tuple<string, string>>();
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var prefix = assembly.GetName().Name + ".Resources.";
            foreach (var wav in assembly.GetManifestResourceNames().Where(name => name.EndsWith(".wav")))
            {
                list.Add(Tuple.Create(wav, Text = wav.Substring(prefix.Length, wav.Length - (prefix.Length + 4))));
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
                    Pause();
                    break;
                case START:
                    Start();
                    break;
                case ROTATE:
                    Rotate();
                    break;
            }
        }

        private void mniSettings_Click(object sender, EventArgs e)
        {
            Show();
        }
        private string nextParticipant()
        {
            return _participants[(_participants.IndexOf(_participant) + 1) % _participants.Count];
        }

        private void participantClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;

        }

        private void Pause()
        {
            var now = DateTime.Now;
            timer.Stop();
            mniPause.Text = START;
            _elapsedTime += now - _startFrom;
        }

        private void Rotate()
        {
            _player.Stop();
            var now = DateTime.Now;
            mniPause.Text = PAUSE;
            _elapsedTime = TimeSpan.Zero;
            _startFrom = now;
            timer.Start();

            if (_participants.Count > 0)
            {
                contextMenuStrip.Items.Cast<ToolStripItem>().SingleOrDefault(i => (string)i.Tag == _participant).Text = _participant;
                _participant = nextParticipant();
                contextMenuStrip.Items.Cast<ToolStripItem>().SingleOrDefault(i => (string)i.Tag == _participant).Text = _participant + CURRENT;
                contextMenuStrip.Items.Cast<ToolStripItem>().SingleOrDefault(i => (string)i.Tag == nextParticipant()).Text = nextParticipant() + NEXT;
            }
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

        private void Start()
        {
            var now = DateTime.Now;
            timer.Start();
            mniPause.Text = PAUSE;
            _startFrom = now;
        }

        private string textFor(string participant)
        {
            if (_participant == participant)
                return participant + CURRENT;
            if (participant == nextParticipant())
                return participant + NEXT;
            return participant;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            _elapsedTime += now - _startFrom;
            _startFrom = now;
            if (_elapsedTime > _totalTime)
                _elapsedTime = _totalTime;

            var percentage = _elapsedTime.TotalMilliseconds / _totalTime.TotalMilliseconds;
            var section = (int)(_sections * percentage);

            notifyIcon.Icon = _icons16[section];
            Icon = _icons48[section];

            if (_elapsedTime >= _totalTime)
                Buzz();
        }
    }
}
