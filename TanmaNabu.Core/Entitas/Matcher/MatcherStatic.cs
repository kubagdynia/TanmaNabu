using System;
using System.Collections.Generic;

namespace Entitas
{
    public partial class Matcher<TEntity>
    {
        [ThreadStatic] static List<int> _indexBufferThreadStatic;
        static readonly List<int> _indexBuffer = _indexBufferThreadStatic ??= new List<int>();
        
        [ThreadStatic] static HashSet<int> _indexSetBufferThreadStatic;
        static readonly HashSet<int> _indexSetBuffer = _indexSetBufferThreadStatic ??= new HashSet<int>();

        public static IAllOfMatcher<TEntity> AllOf(params int[] indices)
        {
            var matcher = new Matcher<TEntity>
            {
                _allOfIndexes = DistinctIndices(indices)
            };
            return matcher;
        }

        public static IAllOfMatcher<TEntity> AllOf(params IMatcher<TEntity>[] matchers)
        {
            var allOfMatcher = (Matcher<TEntity>)AllOf(MergeIndices(matchers));
            SetComponentNames(allOfMatcher, matchers);
            return allOfMatcher;
        }

        public static IAnyOfMatcher<TEntity> AnyOf(params int[] indices)
        {
            var matcher = new Matcher<TEntity>
            {
                _anyOfIndexes = DistinctIndices(indices)
            };
            return matcher;
        }

        public static IAnyOfMatcher<TEntity> AnyOf(params IMatcher<TEntity>[] matchers)
        {
            var anyOfMatcher = (Matcher<TEntity>)AnyOf(MergeIndices(matchers));
            SetComponentNames(anyOfMatcher, matchers);
            return anyOfMatcher;
        }

        private static int[] MergeIndices(int[] allOfIndices, int[] anyOfIndices, int[] noneOfIndices)
        {
            if (allOfIndices != null)
            {
                _indexBuffer.AddRange(allOfIndices);
            }
            if (anyOfIndices != null)
            {
                _indexBuffer.AddRange(anyOfIndices);
            }
            if (noneOfIndices != null)
            {
                _indexBuffer.AddRange(noneOfIndices);
            }

            var mergedIndices = DistinctIndices(_indexBuffer);
            _indexBuffer.Clear();
            return mergedIndices;
        }

        private static int[] MergeIndices(IMatcher<TEntity>[] matchers)
        {
            var indices = new int[matchers.Length];
            for (var i = 0; i < matchers.Length; i++)
            {
                var matcher = matchers[i];
                if (matcher.Indices.Length != 1)
                {
                    throw new MatcherException(matcher.Indices.Length);
                }
                indices[i] = matcher.Indices[0];
            }

            return indices;
        }

        private static string[] GetComponentNames(IMatcher<TEntity>[] matchers)
        {
            foreach (var t in matchers)
            {
                var matcher = t as Matcher<TEntity>;
                if (matcher?.ComponentNames != null)
                {
                    return matcher.ComponentNames;
                }
            }

            return null;
        }

        private static void SetComponentNames(Matcher<TEntity> matcher, IMatcher<TEntity>[] matchers)
        {
            var componentNames = GetComponentNames(matchers);
            if (componentNames != null)
            {
                matcher.ComponentNames = componentNames;
            }
        }

        private static int[] DistinctIndices(IList<int> indices)
        {
            foreach (var index in indices)
            {
                _indexSetBuffer.Add(index);
            }

            var uniqueIndices = new int[_indexSetBuffer.Count];
            _indexSetBuffer.CopyTo(uniqueIndices);
            Array.Sort(uniqueIndices);

            _indexSetBuffer.Clear();

            return uniqueIndices;
        }
    }
}