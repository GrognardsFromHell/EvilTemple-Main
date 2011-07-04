using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvilTemple.Runtime;

namespace EvilTemple.Rules.Messages
{
    /// <summary>
    /// Indicates that the campaign time has changed.
    /// </summary>
    public class TimeChanged : IMessage
    {

        /// <summary>
        /// A reference to the game time.
        /// </summary>
        public GameTime Time { get; set; }

        /// <summary>
        /// A copy of the time before it was changed.
        /// </summary>
        public GameTime OldTime { get; set; }

    }
}
