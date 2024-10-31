namespace Entitas;

public partial class Matcher<TEntity>
{
    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != GetType() || obj.GetHashCode() != GetHashCode())
        {
            return false;
        }

        var matcher = (Matcher<TEntity>)obj;

        if (!EqualIndices(matcher.AllOfIndexes, _allOfIndexes))
        {
            return false;
        }

        if (!EqualIndices(matcher.AnyOfIndexes, _anyOfIndexes))
        {
            return false;
        }

        if (!EqualIndices(matcher.NoneOfIndexes, _noneOfIndexes))
        {
            return false;
        }

        return true;
    }

    private static bool EqualIndices(int[] i1, int[] i2)
    {
        if (i1 == null != (i2 == null))
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

        for (var i = 0; i < i1.Length; i++)
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
            hash = ApplyHash(hash, _allOfIndexes, 3, 53);
            hash = ApplyHash(hash, _anyOfIndexes, 307, 367);
            hash = ApplyHash(hash, _noneOfIndexes, 647, 683);
            _hash = hash;
            _isHashCached = true;
        }

        return _hash;
    }

    private static int ApplyHash(int hash, int[] indices, int i1, int i2)
    {
        if (indices == null) return hash;

        for (var i = 0; i < indices.Length; i++)
        {
            hash ^= indices[i] * i1;
        }
        hash ^= indices.Length * i2;

        return hash;
    }
}