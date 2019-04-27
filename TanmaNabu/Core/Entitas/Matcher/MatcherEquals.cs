namespace Entitas
{
    public partial class Matcher<TEntity>
    {
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType() || obj.GetHashCode() != GetHashCode())
            {
                return false;
            }

            Matcher<TEntity> matcher = (Matcher<TEntity>)obj;

            if (!EqualIndices(matcher.AllOfIndices, _allOfIndices))
            {
                return false;
            }

            if (!EqualIndices(matcher.AnyOfIndices, _anyOfIndices))
            {
                return false;
            }

            if (!EqualIndices(matcher.NoneOfIndices, _noneOfIndices))
            {
                return false;
            }

            return true;
        }

        private static bool EqualIndices(int[] i1, int[] i2)
        {
            if ((i1 == null) != (i2 == null))
            {
                return false;
            }

            if (i1 == null)
            {
                return true;
            }

            if (i1.Length != i2.Length)
            {
                return false;
            }

            for (int i = 0; i < i1.Length; i++)
            {
                if (i1[i] != i2[i])
                {
                    return false;
                }
            }

            return true;
        }

        private int _hash;
        private bool _isHashCached;

        public override int GetHashCode()
        {
            if (!_isHashCached)
            {
                int hash = GetType().GetHashCode();
                hash = ApplyHash(hash, _allOfIndices, 3, 53);
                hash = ApplyHash(hash, _anyOfIndices, 307, 367);
                hash = ApplyHash(hash, _noneOfIndices, 647, 683);
                _hash = hash;
                _isHashCached = true;
            }

            return _hash;
        }

        private static int ApplyHash(int hash, int[] indices, int i1, int i2)
        {
            if (indices == null) return hash;

            for (int i = 0; i < indices.Length; i++)
            {
                hash ^= indices[i] * i1;
            }
            hash ^= indices.Length * i2;

            return hash;
        }
    }
}