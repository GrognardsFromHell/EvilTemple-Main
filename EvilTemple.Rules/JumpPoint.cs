#region

using System;
using EvilTemple.Rules.Utilities;
using Newtonsoft.Json;
using OpenTK;

#endregion

namespace EvilTemple.Rules
{

    public class JumpPoints : Registry<JumpPoint>
    {
        protected override JsonSerializer CreateSerializer()
        {
            var serializer = base.CreateSerializer();
            serializer.Converters.Add(new Vector3Converter());
            return serializer;
        }
    }

    /// <summary>
    ///   Defines a point of interest. Used for teleportation and map changer targets.
    /// </summary>
    public class JumpPoint : IIdentifiable
    {
        public static readonly JumpPoint Default = new JumpPoint();

        public string Id { get; set; }

        public string Name { get; set; }

        public string MapId { get; set; }

        public Vector3 Position { get; set; }
    }
}