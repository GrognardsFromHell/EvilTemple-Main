using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using EvilTemple.Runtime;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace EvilTemple.Support
{
    public class Scripting : IScripting
    {

        private readonly ScriptEngine _engine;

        public Scripting()
        {
            var setup = Python.CreateRuntimeSetup(null);
            // setup.HostArguments = new List<object> { new ResourceAwarePlatformAdaptationLayer("Conversion.Scripts.", typeof(PythonEngine).Assembly) };
            // setup.HostType = typeof(HostingProvider);
            var runtime = new ScriptRuntime(setup);
            _engine = runtime.GetEngineByTypeName(typeof(PythonContext).AssemblyQualifiedName);
        }

        public ScriptEngine Engine
        {
            get { return _engine; }
        }
    }

}
