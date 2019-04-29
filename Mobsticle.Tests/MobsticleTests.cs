using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mobsticle.Interfaces;
using Mobsticle.Logic;
using NSubstitute;

namespace Mobsticle.Tests
{
    [TestClass]
    public class MobsticleTests
    {
        private IMobsticle _mobsticle;
        private IMobsticleSettings _settings;

        [TestInitialize]
        public void Setup()
        {
            _settings = Substitute.For<IMobsticleSettings>();
            _mobsticle = new MobsticleLogic();
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
        public void SettingsSet_UpdatesTotalTime()
        {
            _settings.Minutes.Returns(11);
            _mobsticle.Settings = _settings;
            Assert.AreEqual(TimeSpan.FromMinutes(11).Seconds, _mobsticle.TotalTime);
        }
    }
}
