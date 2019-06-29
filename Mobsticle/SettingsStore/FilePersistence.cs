using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Logic.SettingsStore
{
    public class FilePersistence : IPersistence
    {
        private const string _filename = "MobsticleSettings.json";

        public string Read()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _filename);
            if (File.Exists(path))
                return File.ReadAllText(path);
            return "";
        }

        public void Write(string content)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _filename);
            File.WriteAllText(path, content);
        }
    }
}
