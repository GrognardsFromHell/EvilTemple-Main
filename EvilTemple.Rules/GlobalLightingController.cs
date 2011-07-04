#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EvilTemple.Runtime;
using OpenTK;

#endregion

namespace EvilTemple.Rules
{
    public class GlobalLightingController
    {
        public IDynamicLight DynamicLight { get; set; }

        public IBackgroundMap BackgroundMap { get; set; }

        public GlobalLightingSettings Settings { get; set; }

        /// <summary>
        ///   Interpolates a color value from key-frames color information.
        /// </summary>
        /// <param name = "hour">The current hour, as a decimal.</param>
        /// <param name = "keyframedColorTable">Table The lighting map.</param>
        /// <returns></returns>
        public static Vector3 InterpolateColor(float hour, List<LightingKeyframe> keyframedColorTable)
        {
            Trace.Assert(keyframedColorTable.Count > 1);

            var firstFrame = keyframedColorTable.First();
            if (hour <= firstFrame.Hour)
                return firstFrame.Color;

            var lastFrame = keyframedColorTable.Last();
            if (hour >= lastFrame.Hour)
                return lastFrame.Color;

            var prevTime = firstFrame.Hour;
            var nextTime = lastFrame.Hour;
            var prevColor = firstFrame.Color;
            var nextColor = lastFrame.Color;

            // Find the previous/next index
            for (var i = 0; i < keyframedColorTable.Count - 1; ++i)
            {
                if (keyframedColorTable[i + 1].Hour < hour)
                    continue;

                prevTime = keyframedColorTable[i].Hour;
                prevColor = keyframedColorTable[i].Color;
                nextTime = keyframedColorTable[i + 1].Hour;
                nextColor = keyframedColorTable[i + 1].Color;
                break;
            }

            var factor = (hour - prevTime)/(nextTime - prevTime);
            var invFactor = 1 - factor;

            return invFactor*prevColor + factor*nextColor;
        }

        public void ChangeTime(GameTime time)
        {
            if (Settings == null)
                return;

            if (Settings.Day3dKeyframes == null)
                return;

            var hour = time.HourOfDay + time.MinuteOfHour/(float)GameTime.MinutesPerHour;

            var keyframes2D = time.IsDaytime ? Settings.Day2dKeyframes : Settings.Night2dKeyframes;
            var keyframes3D = time.IsDaytime ? Settings.Day3dKeyframes : Settings.Night3dKeyframes;

            if (BackgroundMap != null)
                BackgroundMap.Color = new Vector4(InterpolateColor(hour, keyframes2D), 1);

            if (DynamicLight != null)
                DynamicLight.Color = new Vector4(InterpolateColor(hour, keyframes3D), 1);
        }

    }
}