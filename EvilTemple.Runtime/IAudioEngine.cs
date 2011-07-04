using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvilTemple.Runtime
{
    public interface IAudioEngine
    {

        ISoundHandle PlaySoundOnce(string path, SoundCategory category = SoundCategory.Other);

    }

    public interface ISoundHandle : IDisposable
    {
        
    }

    public enum SoundCategory
    {
        Music = 0,
        Effect, // Generic sound effect
        Interface, // Interface sound effects like hovering/clicking a button
        Ambience, // Ambient sound effects like the chirping of birds
        Movie, // Movie soundtrack
        Other
    }

}
