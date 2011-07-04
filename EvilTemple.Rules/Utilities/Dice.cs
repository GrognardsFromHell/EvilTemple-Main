using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace EvilTemple.Rules.Utilities
{
    /// <summary>
    /// Definition for a dice pattern.
    /// </summary>
    public class Dice
    {

        /// <summary>
        /// The number of dice.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The sides of each dice.
        /// </summary>
        public int Sides { get; set; }

        /// <summary>
        /// The bonus applied to the result.
        /// </summary>
        public int Bonus { get; set; }

        /// <summary>
        /// The maximum value this dice roll can ever achieve.
        /// </summary>
        public int Maximum {
            get { return Count*Sides + Bonus; }
        }

        /// <summary>
        /// The minimum value this dice roll will always achieve.
        /// </summary>
        public int Minimum
        {
            get { return Count + Bonus; }   
        }

        private static readonly Regex DicePattern = new Regex(@"^(\d*)d(\d+)((?:\+|-)(\d+))?$", 
            RegexOptions.Compiled|RegexOptions.CultureInvariant|RegexOptions.IgnoreCase);

        public Dice(int count, int sides, int bonus)
        {
            Count = count;
            Sides = sides;
            Bonus = bonus;
        }

        public static Dice FromString(string dicePattern)
        {
            var match = DicePattern.Match(dicePattern);

            if (!match.Success)
                throw new ArgumentException(dicePattern + " is not a valid dice pattern.");

            var count = 1;
            if (match.Groups[1].Length > 0)
                count = Convert.ToInt32(match.Groups[1].Value);

            var sides = Convert.ToInt32(match.Groups[2].Value);

            var bonus = 0;
            if (match.Groups[3].Length > 0)
                bonus = Convert.ToInt32(match.Groups[3].Value);

            return new Dice(count, sides, bonus);
        }

        public override string ToString()
        {
            var result = "";

            if (Count != 1)
                result += Count;

            result += "d" + Sides;

            if (Bonus > 0)
                result += "+" + Bonus;
            else if (Bonus < 0)
                result += Bonus;

            return result;
        }

        public int Roll()
        {
            // TODO: Implement
            return 42;
        }
    }

    public class DiceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Trace.Assert(typeof (string) == reader.ValueType);

            var dicePattern = (string) reader.Value;
            return Dice.FromString(dicePattern);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (Dice).IsAssignableFrom(objectType);
        }
    }

}
