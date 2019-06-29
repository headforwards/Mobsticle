using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mobsticle.Logic.Mobsticle;
using Mobsticle.Logic.Notification;
using Mobsticle.Logic.SettingsStore;
using Mobsticle.UserInterface;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Mobsticle.Tests.UserInterface
{
    [TestClass]
    public class UserInterfaceTests
    {
        private MobsticleInterface _interface;
        private IMainWindow _mainWindow;
        private ISoundNotifier _soundNotifier;
        private IMobsticle _mobsticleLogic;
        private ISettingsStore _store;

        [TestMethod]
        public void btnCloseClick_HidesWindowAndSavesSettings()
        {
            _mainWindow.ParticipantsList.Returns("A\r\nB");
            _mainWindow.Minutes.Returns(5);
            _mainWindow.Notification.Returns("A.wav");

            _interface.btnCloseClick();

            _mainWindow.Received().Hide();
            Expression<Predicate<MobsticleSettings>> checkSettings = s => s.Minutes == 5 && s.Participants[0] == "A" && s.Participants[1] == "B" && s.Participants.Count == 2
                && s.Notification == "A.wav";
            _mobsticleLogic.Received().Settings = Arg.Is(checkSettings);
            _soundNotifier.Received().Settings = Arg.Is(checkSettings);
            _store.Received().Save(Arg.Is(checkSettings));
        }

        [TestMethod]
        public void btnCloseClick_TrimsInvalidLines()
        {
            _mainWindow.ParticipantsList.Returns("Test1\r\nTest2\r\n@BAd\r\n\r\n" + new string('X', 31) + "\r\n;;;;;\r\n  \r\n");

            _interface.btnCloseClick();

            _mobsticleLogic.Received().Settings = Arg.Is<IMobsticleSettings>(s => s.Participants[0] == "Test1" && s.Participants[1] == "Test2" && s.Participants.Count == 2);
        }

        [TestMethod]
        public void btnParticipant_RotatesParticipant()
        {
            _interface.btnParticipantClick(4);
            _mobsticleLogic.Received().Rotate(4);
        }

        [TestMethod]
        public void btnPause_PausesTimer()
        {
            _interface.btnPauseClick();
            _mobsticleLogic.Received().Pause();
        }

        [TestMethod]
        public void btnRotate_RotatesTimer()
        {
            _interface.btnRotateClick();
            _mobsticleLogic.Received().Rotate();
        }

        [TestMethod]
        public void btnSettingsClick_ShowsWindow()
        {
            _interface.btnSettingsClick();
            _mainWindow.Received().Show();
        }

        [TestMethod]
        public void btnStart_StartsTimer()
        {
            _interface.btnStartClick();
            _mobsticleLogic.Received().Start();
        }

        [TestMethod]
        [DataRow(DialogResult.Yes)]
        [DataRow(DialogResult.No)]
        public void formClosing_DoesNotCancelIfClosed(DialogResult result)
        {
            object sender = null;
            var e = new FormClosingEventArgs(CloseReason.UserClosing, false);

            _mainWindow.MessageBox(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<MessageBoxButtons>()).Returns(result);

            _interface.menuClose();
            _interface.formClosing(sender, e);

            _mainWindow.Received(result != DialogResult.Yes ? 1 : 0).Hide();
            _mainWindow.Received(result != DialogResult.Yes ? 0 : 1).Exit();
            Assert.AreEqual(result != DialogResult.Yes, e.Cancel);
            _mainWindow.Received().MessageBox("Are you sure you want to exit?", "Mobsticle", MessageBoxButtons.YesNo);
        }

        [TestMethod]
        public void formClosing_HidesAndCancels()
        {
            object sender = null;
            var e = new FormClosingEventArgs(CloseReason.UserClosing, false);

            _interface.formClosing(sender, e);

            _mainWindow.Received().Hide();
            Assert.IsTrue(e.Cancel);
        }

        [TestMethod]
        public void LoadsSettingsOnConstruction()
        {
            _store.Load<MobsticleSettings>().Returns(new MobsticleSettings { Minutes = 7, Participants = new[] { "A", "B" }, Notification = "A.wav" });
            var ndict = new Dictionary<string, string>();
            _soundNotifier.Notifications.Returns(ndict);

            var iface = new MobsticleInterface(_mainWindow, _mobsticleLogic, _store, _soundNotifier);

            _mainWindow.Received().ParticipantsList = "A\r\nB";
            _mainWindow.Received().Minutes = 7;
            _soundNotifier.Received().Notification = "A.wav";
            _mobsticleLogic.Received().Settings = Arg.Is<IMobsticleSettings>(s => s.Minutes == 7 && s.Participants[0] == "A" && s.Participants[1] == "B" && s.Participants.Count == 2);
            _mainWindow.Received().Notifications = ndict;
        }

        [TestMethod]
        public void ParticipantsChanged_SetsCorrectParticipants()
        {
            _mobsticleLogic.Participants.Returns(new[] {
                new Participant {Name = "A", IsDriving = true},
                new Participant {Name = "B", IsDrivingNext = true},
                new Participant {Name = "C"}
            });

            _mobsticleLogic.ParticipantsChanged += Raise.Event();

            _mainWindow.Received().RemoveParticipantButtons();
            _mainWindow.Received().AddParticipantButton(0, "A (Current)");
            _mainWindow.Received().AddParticipantButton(1, "B (Next)");
            _mainWindow.Received().AddParticipantButton(2, "C");
            _mainWindow.Received().SetIconTooltip("A (Current) / B (Next)");
        }

        [TestMethod]
        public void ParticipantsChanged_SetsCorrectParticipantsWithOne()
        {
            _mobsticleLogic.Participants.Returns(new[] {
                new Participant {Name = "A", IsDriving = true, IsDrivingNext = true},
            });

            _mobsticleLogic.ParticipantsChanged += Raise.Event();

            _mainWindow.Received().AddParticipantButton(0, "A");
            _mainWindow.Received().SetIconTooltip("Mobsticle");
        }

        [TestMethod]
        public void ParticipantsChanged_SetsCorrectParticipantsWithoutAny()
        {
            _mobsticleLogic.Participants.Returns(new Participant[] { });

            _mobsticleLogic.ParticipantsChanged += Raise.Event();

            _mainWindow.Received().SetIconTooltip("Mobsticle");
        }

        [TestMethod]
        public void SetsUpOnConstruction()
        {
            new MobsticleInterface(_mainWindow, _mobsticleLogic, _store, _soundNotifier);

            _mainWindow.Received().btnRotateVisible = false;
            _mainWindow.Received().btnStartVisible = false;
            _mainWindow.Received().btnPauseVisible = true;
        }

        [TestInitialize]
        public void Setup()
        {
            _mainWindow = Substitute.For<IMainWindow>();
            _soundNotifier = Substitute.For<ISoundNotifier>();
            _mobsticleLogic = Substitute.For<IMobsticle>();
            _store = Substitute.For<ISettingsStore>();
            _store.Load<MobsticleSettings>().Returns(new MobsticleSettings());
            _interface = new MobsticleInterface(_mainWindow, _mobsticleLogic, _store, _soundNotifier);
            _mainWindow.MobsticleInterface = _interface;
        }

        [TestMethod]
        public void StatusChanged_Expired_StartsNotification()
        {
            _mobsticleLogic.Status.Returns(MobsticleStatus.Expired);

            _mobsticleLogic.StatusChanged += Raise.Event();

            _soundNotifier.Received().StartNotification();
        }

        [TestMethod]
        public void StatusChanged_Paused_StartsPausedNotification()
        {
            _mainWindow.PauseIcon.Returns(50);
            _mobsticleLogic.Status.Returns(MobsticleStatus.Paused);

            _mobsticleLogic.StatusChanged += Raise.Event();

            _mainWindow.Received().DisplayIcon(50);
        }

        [TestMethod]
        public void StatusChanged_Running_StopsNotification()
        {
            _mainWindow.TimerIcons.Returns(24);
            _mobsticleLogic.FractionElapsedTime.Returns(0.5m);
            _mobsticleLogic.Status.Returns(MobsticleStatus.Running);

            _mobsticleLogic.StatusChanged += Raise.Event();

            _soundNotifier.Received().StopNotification();
            _mainWindow.Received().DisplayIcon((int)(24 * 0.5));
        }

        [TestMethod]
        [DataRow(MobsticleStatus.Expired)]
        [DataRow(MobsticleStatus.Running)]
        [DataRow(MobsticleStatus.Paused)]
        public void StatusChanged_MakesButtonsVisible(MobsticleStatus status)
        {
            _mobsticleLogic.Status.Returns(status);

            _mobsticleLogic.StatusChanged += Raise.Event();

            _mainWindow.Received().btnRotateVisible = status == MobsticleStatus.Expired;
            _mainWindow.Received().btnPauseVisible = status == MobsticleStatus.Running;
            _mainWindow.Received().btnStartVisible = status == MobsticleStatus.Paused;
        }

        [TestMethod]
        [DataRow(0d)]
        [DataRow(0.5)]
        [DataRow(0.75)]
        [DataRow(1d)]
        public void TimeChanged_DisplaysTimerIcon(double fraction)
        {
            var fr = (decimal)fraction;

            _mainWindow.TimerIcons.Returns(24);

            _mobsticleLogic.FractionElapsedTime.Returns(fr);

            _mobsticleLogic.TimeChanged += Raise.Event();

            _mainWindow.Received().DisplayIcon((int)(24 * fraction));
        }
    }
}