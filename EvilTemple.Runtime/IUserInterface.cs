using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilTemple.Runtime
{
    public interface IUserInterface
    {

        /// <summary>
        /// The root interface item.
        /// </summary>
        dynamic RootInterfaceItem { get; }

        dynamic AddWidget(string url);

    }
}
