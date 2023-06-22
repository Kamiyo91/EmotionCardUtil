using System.Collections.Generic;

namespace EmotionCardUtil
{
    public class LorIdRootEmotionComparer : IEqualityComparer<LorIdRoot>
    {
        public bool Equals(LorIdRoot x, LorIdRoot y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id && x.PackageId == y.PackageId;
        }

        public int GetHashCode(LorIdRoot obj)
        {
            unchecked
            {
                return (obj.Id * 397) ^ (obj.PackageId != null ? obj.PackageId.GetHashCode() : 0);
            }
        }
    }
}