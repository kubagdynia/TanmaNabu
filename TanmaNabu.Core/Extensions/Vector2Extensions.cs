using System;
using SFML.System;

namespace TanmaNabu.Core.Extensions
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// Compare two vectors and checks if they are equal
        /// </summary>
        /// <param name="other">Vector to check</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>Vectors are equal</returns>
        public static bool Equals(this Vector2f vector2f, Vector2f other, float tolerance) =>
            Math.Abs(vector2f.X - other.X) < tolerance && Math.Abs(vector2f.Y - other.Y) < tolerance;
    }
}