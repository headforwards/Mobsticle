using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mobsticle.Logic.Mobsticle;
using Mobsticle.Logic.Timer;
using NSubstitute;

namespace Mobsticle.Tests
{
    [TestClass]
    public class MobsticleTests
    {
        private IMobsticle _mobsticle;
        private IMobsticleSettings _settings;
        private IMobsticleTimer _timer;
        private DateTime _start = new DateTime(2001, 2, 14, 15, 35, 00);

        [TestInitialize]
        public void Setup()
        {
            _settings = Substitute.For<IMobsticleSettings>();
            _timer = Substitute.For<IMobsticleTimer>();
            _timer.Now.Returns(_start);

            _mobsticle = new MobsticleLogic(_timer);
            _mobsticle.Settings = _settings;
        }

        [TestMethod]
        public void It_StartsWithStatusRunning()
        {
            Assert.AreEqual(MobsticleStatus.Running, _mobsticle.Status);
        }

        [TestMethod]
        public void Start_SetsStatusToRunning()
        {
            var called = false;
            _mobsticle.Pause();
            _mobsticle.StatusChanged += (o, e) => called = true;
            _mobsticle.Start();
            Assert.AreEqual(MobsticleStatus.Running, _mobsticle.Status);
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void Start_DoesNothingIfAlreadyStarted()
        {
            _mobsticle.Start();
            var called = false;
            _mobsticle.StatusChanged += (o, e) => called = true;
            _mobsticle.Start();
            Assert.IsFalse(called);
        }

        [TestMethod]
        public void Pause_DoesNothingIfAlreadyPaused()
        {
            _mobsticle.Pause();
            var called = false;
            _mobsticle.StatusChanged += (o, e) => called = true;
            _mobsticle.Pause();           
            Assert.IsFalse(called);
        }

        [TestMethod]
        public void ParticipantsGet_ReturnsEmptyList()
        {
            CollectionAssert.AreEqual(new List<IParticipant>(), _mobsticle.Participants.ToList());
        }

        [TestMethod]
        public void SettingsSet_SetsParticipantsAndDriver()
        {
            var called = false;
            _settings.Participants.Returns(new List<string> { "A", "B", "C" });
            _mobsticle.ParticipantsChanged += (o, e) => called = true;
            _mobsticle.Settings = _settings;
            Assert.IsTrue(called);
            Assert.AreEqual("A", _mobsticle.Participants.Single(p => p.IsDriving).Name);
            CollectionAssert.AreEqual(new[] { "B", "C" }, _mobsticle.Participants.Where(p => !p.IsDriving).Select(p => p.Name).ToList());
        }

        [TestMethod]
        public void SettingsSet_DoesntFireParticipantEventIfTheSame()
        {
            _settings.Participants.Returns(new List<string> { "A", "B", "C" });
            _mobsticle.Settings = _settings;
            var called = false;
            _mobsticle.ParticipantsChanged += (o, e) => called = true;
            _mobsticle.Settings = _settings;
            Assert.IsFalse(called);
        }

        [TestMethod]
        public void SettingsSet_FiresIfOrderChanges()
        {
            _settings.Participants.Returns(new List<string> { "A", "B", "C" });
            _mobsticle.Settings = _settings;
            var called = false;
            _mobsticle.ParticipantsChanged += (o, e) => called = true;
            _settings.Participants.Returns(new List<string> { "B", "A", "C" });
            _mobsticle.Settings = _settings;
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void SettingsSet_ChangesDriverIfExistingDriverRemoved()
        {
            _settings.Participants.Returns(new List<string> { "A", "B", "C" });
            _mobsticle.Settings = _settings;
            _settings.Participants.Returns(new List<string> { "B", "C" });
            _mobsticle.Settings = _settings;
            Assert.IsTrue(_mobsticle.Participants[0].Name == "B" && _mobsticle.Participants[0].IsDriving == true);
        }

        [TestMethod]
        public void SettingsSet_SetsNextDriver()
        {
            _settings.Participants.Returns(new List<string> { "A", "B", "C" });
            _mobsticle.Settings = _settings;
            Assert.IsTrue(_mobsticle.Participants[1].Name == "B" && _mobsticle.Participants[1].IsDrivingNext == true);
        }

        [TestMethod]
        public void SettingsSet_CanHandleEmptyParticipantsList()
        {
            _settings.Participants.Returns(new List<string> ());
            _mobsticle.Settings = _settings;
        }

        [TestMethod]
        public void Rotate_MovesToNextDriver()
        {
            _settings.Participants.Returns(new List<string> { "A", "B", "C" });
            _mobsticle.Settings = _settings;
            _mobsticle.Rotate();
            Assert.IsTrue(_mobsticle.Participants[1].IsDriving == true);
            Assert.IsTrue(_mobsticle.Participants[2].IsDrivingNext == true);
            _mobsticle.Rotate();
            Assert.IsTrue(_mobsticle.Participants[2].IsDriving == true);
            Assert.IsTrue(_mobsticle.Participants[0].IsDrivingNext == true);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(50)]
        [DataRow(100)]
        [DataRow(75)]
        public void FractionElapsedTime_ReturnsExpectedTime(int percent)
        {
            _settings.Minutes.Returns(2);
            _mobsticle.Settings = _settings;
            _timer.Now.Returns(_start.AddSeconds((120d / 100 ) * percent));
            Assert.AreEqual(percent / 100m, _mobsticle.FractionElapsedTime);
        }

        [TestMethod]
        public void Status_ChangesToExpiredWhenTimerExpires()
        {
            _settings.Minutes.Returns(4);
            _mobsticle.Settings = _settings;
            var called = false;
            _mobsticle.StatusChanged += (o, e) => called = true;
            _timer.Now.Returns(_start.AddMinutes(4.01));
            _timer.Tick += Raise.Event();
            Assert.AreEqual(MobsticleStatus.Expired, _mobsticle.Status);
            Assert.IsTrue(called);            
        }

        [TestMethod]
        public void Pause_ExtendsTimer()
        {
            _settings.Minutes.Returns(4);
            _mobsticle.Settings = _settings;
            _timer.Now.Returns(_start.AddMinutes(1));
            _timer.Tick += Raise.Event();
            Assert.AreEqual(MobsticleStatus.Running, _mobsticle.Status);
            _mobsticle.Pause();
            _timer.Now.Returns(_start.AddMinutes(21));
            _timer.Tick += Raise.Event();
            Assert.AreEqual(MobsticleStatus.Paused, _mobsticle.Status);
            _mobsticle.Start();
            Assert.AreEqual(MobsticleStatus.Running, _mobsticle.Status);
            _timer.Now.Returns(_start.AddMinutes(23.99));
            _timer.Tick += Raise.Event();
            Assert.AreEqual(MobsticleStatus.Running, _mobsticle.Status);
            _timer.Now.Returns(_start.AddMinutes(24.01));
            _timer.Tick += Raise.Event();
            Assert.AreEqual(MobsticleStatus.Expired, _mobsticle.Status);
        }

        [TestMethod]
        public void Rotate_WorksWithNoParticipants()
        {
            _settings.Minutes.Returns(5);
            _mobsticle.Settings = _settings;
            _mobsticle.Rotate();
        }

        [TestMethod]
        public void Rotate_StartsTimer()
        {
            _settings.Minutes.Returns(5);
            _mobsticle.Settings = _settings;
            _mobsticle.Pause();
            _mobsticle.Rotate();
            Assert.AreEqual(MobsticleStatus.Running, _mobsticle.Status);
        }

        [TestMethod]
        public void Rotate_ResetsTheTimer()
        {
            _settings.Minutes.Returns(5);
            _mobsticle.Settings = _settings;
            _timer.Now.Returns(_start.AddMinutes(2.5));
            _timer.Tick += Raise.Event();
            _mobsticle.Rotate();
            Assert.AreEqual(0, _mobsticle.FractionElapsedTime);
        }

        [TestMethod]
        public void TimeChanged_FiresWhenPercentChanges()
        {
            _settings.Minutes.Returns(200);
            _mobsticle.Settings = _settings;
            var called = 0;
            _mobsticle.TimeChanged += (o, e) => called++;
            for (int x = 1; x <= 250; x++)
            {
                _timer.Now.Returns(_start.AddMinutes(x));
                _timer.Tick += Raise.Event();
            }
            Assert.AreEqual(100, called);
        }

        [TestMethod]
        public void TimeChanged_ContinuesToWorkOnceRotated()
        {
            _settings.Minutes.Returns(10);
            _mobsticle.Settings = _settings;
            _timer.Now.Returns(_start.AddMinutes(9));
            _timer.Tick += Raise.Event();
            _timer.Now.Returns(_start.AddMinutes(11));
            _timer.Tick += Raise.Event();
            _mobsticle.Rotate();
            var called = false;
            _mobsticle.TimeChanged += (o, e) => called = true;
            _timer.Now.Returns(_start.AddMinutes(12));
            _timer.Tick += Raise.Event();
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void FractionTimeElapsed_NeverReturnsMoreThan1()
        {
            _settings.Minutes.Returns(10);
            _mobsticle.Settings = _settings;
            _timer.Now.Returns(_start.AddMinutes(15));
            Assert.AreEqual(1, _mobsticle.FractionElapsedTime);
        }

        [TestMethod]
        public void FractionTimeElapsed_ResetsWhenRotated()
        {
            _settings.Minutes.Returns(5);
            _mobsticle.Settings = _settings;
            _timer.Now.Returns(_start.AddMinutes(6));
            _timer.Tick += Raise.Event();
            _mobsticle.Rotate();
            _timer.Now.Returns(_start.AddMinutes(7));
            Assert.IsTrue(_mobsticle.FractionElapsedTime < 1);
        }

        [TestMethod]
        public void Rotate_TriggersTimeChangedEventImmediately()
        {
            _settings.Minutes.Returns(10);
            _mobsticle.Settings = _settings;            
            var called = false;
            _mobsticle.TimeChanged += (o, e) => called = true;
            _mobsticle.Rotate();
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void Rotate_TriggersParticipantsChanged()
        {
            _settings.Minutes.Returns(10);
            _settings.Participants.Returns(new List<string> { "Test1", "Test2", "Test3" });
            _mobsticle.Settings = _settings;
            var called = false;
            _mobsticle.ParticipantsChanged += (o, e) => called = true;
            _mobsticle.Rotate();
            Assert.IsTrue(called);
        }
    }
}
