#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using EvilTemple.Runtime;

#endregion

namespace Rules
{
    internal class LegacyLiteralAttribute : Attribute
    {
        public string Name { get; private set; }

        public LegacyLiteralAttribute(string name)
        {
            Name = name;
        }

        public static string GetLegacyName(MemberInfo memberInfo)
        {
            var attribute = (LegacyLiteralAttribute) GetCustomAttribute(memberInfo,
                                                                        typeof (LegacyLiteralAttribute));

            return attribute == null ? null : attribute.Name;
        }
    }

    public enum Gender
    {
        [LegacyLiteral("other")] Other,
        [LegacyLiteral("female")] Female,
        [LegacyLiteral("male")] Male
    }

    public enum ConversionChoice
    {
        None,
        Positive,
        Negative
    }

    public enum Alignment
    {
        [LegacyLiteral("lawful_good")] LawfulGood,
        [LegacyLiteral("neutral_good")] NeutralGood,
        [LegacyLiteral("chaotic_good")] ChaoticGood,
        [LegacyLiteral("lawful_neutral")] LawfulNeutral,
        [LegacyLiteral("true_neutral")] TrueNeutral,
        [LegacyLiteral("chaotic_neutral")] ChaoticNeutral,
        [LegacyLiteral("lawful_evil")] LawfulEvil,
        [LegacyLiteral("neutral_evil")] NeutralEvil,
        [LegacyLiteral("chaotic_evil")] ChaoticEvil
    }
    
    public enum MagicType
    {
        Divine,
        Arcane
    }
    
    public static class Alignments
    {
        private static readonly string[] LegacyNames = EnumHelper.GetLegacyLiterals<Alignment>();

        public static readonly IDictionary<Alignment, Alignment[]> CompatibilityTable;

        static Alignments()
        {
            CompatibilityTable = new Dictionary<Alignment, Alignment[]>();
            CompatibilityTable[ Alignment.LawfulGood ] = new [] {Alignment.LawfulGood, Alignment.NeutralGood, Alignment.LawfulNeutral};
            CompatibilityTable[ Alignment.NeutralGood ] = new [] {Alignment.LawfulGood, Alignment.NeutralGood, Alignment.ChaoticGood, Alignment.TrueNeutral};
            CompatibilityTable[ Alignment.ChaoticGood ] = new [] {Alignment.NeutralGood, Alignment.ChaoticGood, Alignment.ChaoticNeutral};
            CompatibilityTable[ Alignment.LawfulNeutral ] = new [] {Alignment.LawfulGood, Alignment.LawfulNeutral, Alignment.TrueNeutral, Alignment.LawfulEvil};
            CompatibilityTable[ Alignment.TrueNeutral ] = new [] {Alignment.NeutralGood, Alignment.LawfulNeutral, Alignment.TrueNeutral, Alignment.ChaoticNeutral, Alignment.NeutralEvil};
            CompatibilityTable[ Alignment.ChaoticNeutral ] = new [] {Alignment.ChaoticGood, Alignment.TrueNeutral, Alignment.ChaoticGood, Alignment.ChaoticEvil};
            CompatibilityTable[ Alignment.LawfulEvil ] = new [] {Alignment.LawfulNeutral, Alignment.LawfulEvil, Alignment.NeutralEvil};
            CompatibilityTable[ Alignment.NeutralEvil ] = new [] {Alignment.TrueNeutral, Alignment.LawfulEvil, Alignment.NeutralEvil, Alignment.ChaoticEvil};
            CompatibilityTable[ Alignment.ChaoticEvil ] = new [] {Alignment.ChaoticNeutral, Alignment.NeutralEvil, Alignment.ChaoticEvil};
        }

        public static string Translate(this Alignment value)
        {
            switch (value)
            {
                case Alignment.LawfulGood:
                    return "#stat/6005".Translate();
                case Alignment.NeutralGood:
                    return "#stat/6004".Translate();
                case Alignment.ChaoticGood:
                    return "#stat/6006".Translate();
                case Alignment.LawfulNeutral:
                    return "#stat/6001".Translate();
                case Alignment.TrueNeutral:
                    return "#stat/6000".Translate();
                case Alignment.ChaoticNeutral:
                    return "#stat/6002".Translate();
                case Alignment.LawfulEvil:
                    return "#stat/6009".Translate();
                case Alignment.NeutralEvil:
                    return "#stat/6008".Translate();
                case Alignment.ChaoticEvil:
                    return "#stat/6010".Translate();
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        public static string ToLegacyString(this Alignment value)
        {
            return LegacyNames[(int) value];
        }

        public static Alignment ParseLegacyString(string alignment)
        {
            for (var i = 0; i < LegacyNames.Length; ++i)
            {
                if (LegacyNames[i] == alignment)
                    return (Alignment) i;
            }

            throw new InvalidOperationException("Given legacy alignment doesn't exist: " + alignment);
        }

        public static bool IsCompatibleWith(this Alignment a, Alignment b)
        {
            return CompatibilityTable[a].Contains(b);
        }
    }


    public static class Genders
    {
        private static readonly string[] LegacyNames = EnumHelper.GetLegacyLiterals<Gender>();

        public static string ToLegacyString(this Gender value)
        {
            return LegacyNames[(int) value];
        }

        public static string Translate(this Gender value)
        {
            switch (value)
            {
                case Gender.Other:
                    return "Other";
                case Gender.Female:
                    return "#stat/4000".Translate();
                case Gender.Male:
                    return "#stat/4001".Translate();
                default:
                    throw new ArgumentOutOfRangeException("value");
            }
        }

        public static Gender ParseLegacyString(string gender)
        {
            for (var i = 0; i < LegacyNames.Length; ++i)
            {
                if (LegacyNames[i] == gender)
                    return (Gender) i;
            }

            throw new InvalidOperationException("Given legacy gender doesn't exist: " + gender);
        }
    }


    internal static class EnumHelper
    {
        private static readonly string[] LegacyNames = GetLegacyLiterals<Alignment>();

        public static string[] GetLegacyLiterals<T>()
        {
            var enumType = typeof (T);

            if (!enumType.IsEnum)
                throw new InvalidOperationException("This method can only be used for enumeration types.");

            var enumNames = enumType.GetEnumNames();
            var legacyNames = new string[enumNames.Length];

            var i = 0;
            foreach (var enumName in enumNames)
            {
                var memberInfos = enumType.GetMember(enumName);

                if (memberInfos.Length != 1)
                    throw new InvalidOperationException("Enumeration literal " + enumName + " is not unique!");

                var memberInfo = memberInfos[0];

                var legacyName = LegacyLiteralAttribute.GetLegacyName(memberInfo);

                if (legacyName == null)
                    throw new InvalidOperationException("Enumeration literal " + enumName +
                                                        " has no legacy type attribute.");

                legacyNames[i++] = legacyName;
            }

            return legacyNames;
        }
    }
}