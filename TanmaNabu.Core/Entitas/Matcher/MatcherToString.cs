using System.Text;

namespace Entitas
{
    public partial class Matcher<TEntity>
    {
        private string _toStringCache;
        private StringBuilder _toStringBuilder;

        public override string ToString()
        {
            if (_toStringCache == null)
            {
                if (_toStringBuilder == null)
                {
                    _toStringBuilder = new StringBuilder();
                }

                _toStringBuilder.Length = 0;

                if (_allOfIndices != null)
                {
                    AppendIndices(_toStringBuilder, "AllOf", _allOfIndices, ComponentNames);
                }
                if (_anyOfIndices != null)
                {
                    if (_allOfIndices != null)
                    {
                        _toStringBuilder.Append(".");
                    }
                    AppendIndices(_toStringBuilder, "AnyOf", _anyOfIndices, ComponentNames);
                }
                if (_noneOfIndices != null)
                {
                    AppendIndices(_toStringBuilder, ".NoneOf", _noneOfIndices, ComponentNames);
                }
                _toStringCache = _toStringBuilder.ToString();
            }

            return _toStringCache;
        }

        private static void AppendIndices(StringBuilder sb, string prefix, int[] indexArray, string[] componentNames)
        {
            const string separator = ", ";
            sb.Append(prefix);
            sb.Append("(");
            int lastSeparator = indexArray.Length - 1;
            for (int i = 0; i < indexArray.Length; i++)
            {
                int index = indexArray[i];
                if (componentNames == null)
                {
                    sb.Append(index);
                }
                else
                {
                    sb.Append(componentNames[index]);
                }

                if (i < lastSeparator)
                {
                    sb.Append(separator);
                }
            }
            sb.Append(")");
        }
    }
}