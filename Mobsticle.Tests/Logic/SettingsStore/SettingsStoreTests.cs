using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mobsticle.Logic.Mobsticle;
using Mobsticle.Logic.SettingsStore;
using NSubstitute;
using System.Linq;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Mobsticle.Tests.Logic.SettingsStore
{
    [TestClass]
    public class SettingsStoreTests
    {
        private ISettingsStore _store;
        private IPersistence _persistence;

        [TestInitialize()]
        public void Setup()
        {
            _persistence = Substitute.For<IPersistence>();
            _store = new Store(_persistence);
        }

        [TestMethod]
        public void Load_ReadsSettingsFromDefaultLocationWhenEmpty()
        {
            var result = _store.Load<MobsticleSettings>();
            Assert.IsInstanceOfType(result, typeof(MobsticleSettings));
        }

        [TestMethod]
        public void Load_ReadsSettingsFromDefaultLocation()
        {
            _persistence.Read().Returns("{Minutes: \"1\", Participants: [\"Test1\", \"Test2\"]}");
            var result = _store.Load<MobsticleSettings>();
            Assert.AreEqual(1, result.Minutes);
            CollectionAssert.AreEqual(new [] { "Test1", "Test2" }, result.Participants.ToList());
        }

        [TestMethod]
        public void Save_WritesSettingsToDefaultLocation()
        {
            var settings = new MobsticleSettings { Minutes = 1, Participants = new List<string> { "Test1", "Test2" } };
            _store.Save(settings);
            Func<string, bool> expr = s =>
            {
                var ob = JsonConvert.DeserializeObject<MobsticleSettings>(s);
                Assert.AreEqual(1, ob.Minutes);
                CollectionAssert.AreEqual(new[] { "Test1", "Test2" }, ob.Participants.ToList());
                return true;
            };
            _persistence.Received(1).Write(Arg.Is<string>(s => expr(s)));
        }
    }
}
