using OpenTK;

namespace EvilTemple.Runtime
{
    public interface IGameView
    {

        IScene Scene { get; }
        
        /// <summary>
        /// Plays a movie as an overlay over the game.
        /// </summary>
        /// <param name="filename">The filename of the movie, relative to the data root.</param>
        /// <returns>True if the movie started playing successfully, false otherwise.</returns>
        bool PlayMovie(string filename);

        void CenterOnWorld(Vector3 worldCoordinates);

    }

}
