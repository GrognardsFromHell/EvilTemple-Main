using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Hosting;

namespace EvilTemple.Runtime
{
    public interface IScripting
    {

        ScriptEngine Engine { get; }

    }
}
