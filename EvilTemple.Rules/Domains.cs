#region

using System.Collections.Generic;
using EvilTemple.Rules.Utilities;

#endregion

namespace EvilTemple.Rules
{
    public class Domains : Registry<Domain>
    {
    }

    public class DomainNameComparer : IComparer<Domain>
    {
        public static readonly IComparer<Domain> Instance = new DomainNameComparer();

        public int Compare(Domain x, Domain y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}