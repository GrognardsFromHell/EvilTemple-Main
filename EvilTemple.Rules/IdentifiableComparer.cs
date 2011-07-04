#region

using System.Collections.Generic;
using EvilTemple.Rules;

#endregion

namespace Rules
{
    public sealed class IdentifiableComparer : IComparer<IIdentifiable>
    {
        public int Compare(IIdentifiable x, IIdentifiable y)
        {
            return x.Id.CompareTo(y.Id);
        }
    }
}