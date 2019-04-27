using System;
using System.Collections.Generic;

namespace Entitas
{
    public partial class Matcher<TEntity>
    {
        static readonly List<int> _indexBuffer = new List<int>();
        static readonly HashSet<int> _indexSetBuffer = new HashSet<int>();

        public static IAllOfMatcher<TEntity> AllOf(params int[] indices)
        {
            Matcher<TEntity> matcher = new Matcher<TEntity>
            {
                _allOfIndices = DistinctIndices(indices)
            };
            return matcher;
        }

        public static IAllOfMatcher<TEntity> AllOf(params IMatcher<TEntity>[] matchers)
        {
            Matcher<TEntity> allOfMatcher = (Matcher<TEntity>)AllOf(MergeIndices(matchers));
            SetComponentNames(allOfMatcher, matchers);
            return allOfMatcher;
        }

        public static IAnyOfMatcher<TEntity> AnyOf(params int[] indices)
        {
            Matcher<TEntity> matcher = new Matcher<TEntity>
            {
                _anyOfIndices = DistinctIndices(indices)
            };
            return matcher;
        }

        public static IAnyOfMatcher<TEntity> AnyOf(params IMatcher<TEntity>[] matchers)
        {
            Matcher<TEntity> anyOfMatcher = (Matcher<TEntity>)AnyOf(MergeIndices(matchers));
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

            int[] mergedIndices = DistinctIndices(_indexBuffer);

            _indexBuffer.Clear();

            return mergedIndices;
        }

        private static int[] MergeIndices(IMatcher<TEntity>[] matchers)
        {
            int[] indices = new int[matchers.Length];
            for (int i = 0; i < matchers.Length; i++)
            {
                IMatcher<TEntity> matcher = matchers[i];
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
            for (int i = 0; i < matchers.Length; i++)
            {
                Matcher<TEntity> matcher = matchers[i] as Matcher<TEntity>;
                if (matcher?.ComponentNames != null)
                {
                    return matcher.ComponentNames;
                }
            }

            return null;
        }

        private static void SetComponentNames(Matcher<TEntity> matcher, IMatcher<TEntity>[] matchers)
        {
            string[] componentNames = GetComponentNames(matchers);
            if (componentNames != null)
            {
                matcher.ComponentNames = componentNames;
            }
        }

        private static int[] DistinctIndices(IList<int> indices)
        {
            foreach (int index in indices)
            {
                _indexSetBuffer.Add(index);
            }

            int[] uniqueIndices = new int[_indexSetBuffer.Count];
            _indexSetBuffer.CopyTo(uniqueIndices);
            Array.Sort(uniqueIndices);

            _indexSetBuffer.Clear();

            return uniqueIndices;
        }
    }
}