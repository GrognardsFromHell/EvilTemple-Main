using System;
using System.Collections.Generic;
using OpenTK;

namespace EvilTemple.Rules
{
    public class GlobalLightingSettings
    {
        public Color3 Color { get; set; }

        public Vector3 Direction { get; set; }

        public List<LightingKeyframe> Night2dKeyframes { get; set; }

        public List<LightingKeyframe> Day2dKeyframes { get; set; }

        public List<LightingKeyframe> Night3dKeyframes { get; set; }

        public List<LightingKeyframe> Day3dKeyframes { get; set; }

    }
}