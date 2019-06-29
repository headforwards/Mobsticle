using Mobsticle.Logic.Mobsticle;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Logic.SettingsStore
{
    public class Store : ISettingsStore
    {
        IPersistence _persistence;

        public Store(IPersistence persistence)
        {
            _persistence = persistence;
        }

        public T Load<T>()
        {
            var text = _persistence.Read();
            text = string.IsNullOrWhiteSpace(text) ? "{}" : text;
            return JsonConvert.DeserializeObject<T>(text);
        }

        public void Save<T>(T settings)
        {
            _persistence.Write(JsonConvert.SerializeObject(settings));
        }
    }
}
