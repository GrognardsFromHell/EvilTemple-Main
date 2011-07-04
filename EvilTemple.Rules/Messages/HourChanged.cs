using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvilTemple.Runtime;

namespace EvilTemple.Rules.Messages
{
    /// <summary>
    /// This message is sent when the current hour in game time changes.
    /// </summary>
    public class HourChanged : IMessage
    {

        /// <summary>
        /// The hour before the hour changed.
        /// </summary>
        public int PreviousHour { get; set; }

        /// <summary>
        /// The current game time.
        /// </summary>
        public GameTime Time { get; set; }

    }
}
