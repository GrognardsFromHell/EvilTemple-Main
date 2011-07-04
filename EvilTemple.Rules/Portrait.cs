#region

using EvilTemple.Rules;
using EvilTemple.Rules.Utilities;

#endregion

namespace Rules
{
    public class Portrait : IIdentifiable
    {
        public static readonly Portrait Default = new Portrait();

        public string Id { get; set; }

        public string LargeImage { get; set; }

        public string MediumImage { get; set; }

        public string SmallImage { get; set; }

        public string MediumGrayImage { get; set; }

        public string SmallGrayImage { get; set; }

        public string Race { get; set; }

        public Gender Gender { get; set; }

        public Portrait()
        {
            Gender = Gender.Other;
        }
    }

    public class Portraits : Registry<Portrait>
    {
    }
}