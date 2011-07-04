using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilTemple.Runtime
{
    public interface IShortcuts
    {

        void Register(Keys key, Action callback);

    }
}
