using System;
using System.Collections.Generic;
using EvilTemple.Rules.Messages;
using EvilTemple.Runtime;
using OpenTK;
using Rules;

namespace EvilTemple.Rules
{
    /// <summary>
    ///   A campaign describes the currently running game with all attached rule objects, like the party,
    ///   players or visited levels.
    /// </summary>
    public class Campaign
    {
        public static Campaign Current { get; set; }

        public Party Party { get; set; }

        private readonly GameTime _time;
        
        public GameTime Time
        {
            get { return _time; }
        }

        private MapRenderState _mapRenderState;

        public Map CurrentMap { get; private set; }

        public bool Running { get; private set; }

        public static GameTime StartTime = new GameTime(579, 3, 14, 15);

        private Dictionary<BaseObject, IRenderable> _activeObjects = new Dictionary<BaseObject, IRenderable>();

        private readonly GlobalLightingController _globalLightingController = new GlobalLightingController();

        public Campaign()
        {
            _time = StartTime.Copy();

            Time.OnHourChange += (oldHour, time) =>
                                    {
                                        if (!Running)
                                            return;

                                        var message = new HourChanged {PreviousHour = oldHour, Time = time};
                                        EventBus.Send(message);
                                    };

            Time.OnTimeChange += (oldTime, newTime) =>
                                    {
                                        if (!Running)
                                            return;

                                        var message = new TimeChanged {OldTime = oldTime, Time = newTime};
                                        EventBus.Send(message);
                                    };
        }

        private void TimeTick()
        {
            if (!Running)
                return;

            Time.Advance(hours: 1);
        }

        public void Start()
        {
            Current = this;

            VisualTimers.AddTimer(500, TimeTick, true);

            EventBus.Send<StartingCampaign>();

            // Set up a nice debugging party
            Party.Money.AddGold(1000); // Start with 1000 gold
            
            var maps = Services.Get<Maps>();

            Map startMap;
            string startMovie;
            
            // Vignette based on party alignment)
            switch (Party.Alignment) {
                case Alignment.LawfulGood:
                    startMap = maps["vignetteLawfulGood"];
                    startMovie = "movies/New_LG_Final_0.bik";
                    break;
                case Alignment.NeutralGood:
                    startMap = maps["vignetteGood"];
                    startMovie = "movies/New_NG_Final_0.bik";
                    break;
                case Alignment.ChaoticGood:
                    startMap = maps["vignetteChaoticGood"];
                    startMovie = "movies/New_CG_Final_0.bik";
                    break;
                case Alignment.LawfulNeutral:
                    startMap = maps["vignetteLawful"];
                    startMovie = "movies/New_LN_Final_0.bik";
                    break;
                case Alignment.TrueNeutral:
                    startMap = maps["vignetteNeutral"];
                    startMovie = "movies/New_NN_Final_0.bik";
                    break;
                case Alignment.ChaoticNeutral:
                    startMap = maps["vignetteChaotic"];
                    startMovie = "movies/New_CN_Final_0.bik";
                    break;
                case Alignment.LawfulEvil:
                    startMap = maps["vignetteLawfulEvil"];
                    startMovie = "movies/New_CE-intro.bik";
                    break;
                case Alignment.NeutralEvil:
                    startMap = maps["vignetteEvil"];
                    startMovie = "movies/New_NE_FInal0.bik";
                    break;
                case Alignment.ChaoticEvil:
                    startMap = maps["vignetteChaoticEvil"];
                    startMovie = "movies/New_LE_Final_0.bik";
                    break;
                default:
                    throw new InvalidOperationException("Invalid party alignment: " + Party.Alignment);
            }

            startMap = maps["hommlet"];

            // Services.GameView.PlayMovie(startMovie);
            GoTo(startMap, startMap.StartPosition);
        }

        public void GoTo(Map map, Vector3 startPosition)
        {

            if (CurrentMap != map)
            {
                if (_mapRenderState != null)
                    _mapRenderState.Deactivate();

                // TODO: Fade over to new map?

                _mapRenderState = new MapRenderState(this, map);
                _mapRenderState.Activate();

                CurrentMap = map;
            }

            Services.GameView.CenterOnWorld(startPosition);
        }


    }
}
