using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobsticle.Logic.SettingsStore
{
    public interface IPersistence
    {
        string Read();

        void Write(string content);
    }
}
