using System;

namespace Entitas
{
    public class MatcherException(int indices) : Exception($"matcher.indices.Length must be 1 but was {indices}");
}