using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mobsticle.Logic.Mobsticle;
using Mobsticle.Logic.SettingsStore;
using Mobsticle.UserInterface;
using NSubstitute;
using System;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Mobsticle.Tests.UserInterface
{
    [TestClass]
    public class UserInterfaceTests
    {
        private IMainWindow _mainWindow;
        private IMobsticle _mobsticleLogic;
        private ISettingsStore _store;
        private MobsticleInterface _interface;

        [TestInitialize]
        public void Setup()
        {
            _mainWindow = Substitute.For<IMainWindow>();
            _mobsticleLogic = Substitute.For<IMobsticle>();
            _store = Substitute.For<ISettingsStore>();
            _interface = new MobsticleInterface(_mainWindow, _mobsticleLogic, _store);
            _mainWindow.MobsticleInterface = _interface;
        }

        [TestMethod]
        public void btnCloseClick_HidesWindowAndSavesSettings()
        {
            _mainWindow.ParticipantsList.Returns("A\r\nB");
            _mainWindow.Minutes.Returns(5);

            _interface.btnCloseClick();

            _mainWindow.Received().Hide();
            Expression<Predicate<IMobsticleSettings>> checkSettings = s => s.Minutes == 5 && s.Participants[0] == "A" && s.Participants[1] == "B" && s.Participants.Count == 2;
            _mobsticleLogic.Received().Settings = Arg.Is(checkSettings);
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
        public void formClosing_HidesAndCancels()
        {
            object sender = null;
            var e = new FormClosingEventArgs(CloseReason.UserClosing, false);

            _interface.formClosing(sender, e);

            _mainWindow.Received().Hide();
            Assert.IsTrue(e.Cancel);
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
    }
}