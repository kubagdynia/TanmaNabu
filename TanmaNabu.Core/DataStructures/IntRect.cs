using System;
using System.Runtime.InteropServices;

namespace TanmaNabu.Core.DataStructures;

[StructLayout(LayoutKind.Sequential)]
public struct IntRect : IEquatable<IntRect>
{
    public static readonly IntRect Zero = new IntRect(0, 0, 0, 0);

    public int Left;

    public int Top;

    public int Right;

    public int Bottom;

    public int Width => Right - Left;

    public int Height => Bottom - Top;

    /// <summary>
    /// Construct the rectangle from its coordinates
    /// </summary>
    public IntRect(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    /// <summary>
    /// Construct the rectangle from its coordinates
    /// </summary>
    public IntRect(float left, float top, float right, float bottom)
    {
        Left = (int)left;
        Top = (int)top;
        Right = (int)right;
        Bottom = (int)bottom;
    }

    /// <summary>
    /// Construct the rectangle from its coordinates
    /// </summary>
    public IntRect(double left, double top, double right, double bottom)
    {
        Left = (int)left;
        Top = (int)top;
        Right = (int)right;
        Bottom = (int)bottom;
    }

    /// <summary>
    /// Construct the rectangle from its coordinates points
    /// </summary>
    public IntRect(Point2 leftUpCorner, Point2 rightBottomCorner)
        : this(leftUpCorner.X, leftUpCorner.Y, rightBottomCorner.X, rightBottomCorner.Y)
    {

    }

    /// <summary>
    /// Check if a point is inside the rectangle's area
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Contains(int x, int y)
    {
        int minX = Math.Min(Left, Right);
        int maxX = Math.Max(Left, Right);
        int minY = Math.Min(Top, Bottom);
        int maxY = Math.Max(Top, Bottom);

        return x >= minX && x < maxX && y >= minY && y < maxY;
    }

    /// <summary>
    /// Check if a point is inside the rectangle's area
    /// </summary>
    public bool Contains(Point2 point)
    {
        return Contains(point.X, point.Y);
    }

    /// <summary>
    /// Check intersection between two rectangles
    /// </summary>
    /// <param name="rect">Rectangle to test</param>
    /// <returns>True if rectangles overlap</returns>
    public bool Intersects(IntRect rect)
    {
        // Rectangles with negative dimensions are allowed, so we must handle them correctly

        // Compute the min and max of the first rectangle on both axes
        int r1MinX = Math.Min(Left, Right);
        int r1MaxX = Math.Max(Left, Right);
        int r1MinY = Math.Min(Top, Bottom);
        int r1MaxY = Math.Max(Top, Bottom);

        // Compute the min and max of the second rectangle on both axes
        int r2MinX = Math.Min(rect.Left, rect.Right);
        int r2MaxX = Math.Max(rect.Left, rect.Right);
        int r2MinY = Math.Min(rect.Top, rect.Bottom);
        int r2MaxY = Math.Max(rect.Top, rect.Bottom);

        // Compute the intersection boundaries
        int interLeft = Math.Max(r1MinX, r2MinX);
        int interTop = Math.Max(r1MinY, r2MinY);
        int interRight = Math.Min(r1MaxX, r2MaxX);
        int interBottom = Math.Min(r1MaxY, r2MaxY);

        // If the intersection is valid (positive non zero area), then there is an intersection
        if (interLeft < interRight && interTop < interBottom)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check intersection between two rectangles
    /// </summary>
    /// <param name="rect">Rectangle to test</param>
    /// <param name="overlap">Rectangle to be filled with overlapping rect</param>
    /// <returns>True if rectangles overlap</returns>
    public bool Intersects(IntRect rect, out IntRect overlap)
    {
        // Rectangles with negative dimensions are allowed, so we must handle them correctly

        // Compute the min and max of the first rectangle on both axes
        int r1MinX = Math.Min(Left, Right);
        int r1MaxX = Math.Max(Left, Right);
        int r1MinY = Math.Min(Top, Bottom);
        int r1MaxY = Math.Max(Top, Bottom);

        // Compute the min and max of the second rectangle on both axes
        int r2MinX = Math.Min(rect.Left, rect.Right);
        int r2MaxX = Math.Max(rect.Left, rect.Right);
        int r2MinY = Math.Min(rect.Top, rect.Bottom);
        int r2MaxY = Math.Max(rect.Top, rect.Bottom);

        // Compute the intersection boundaries
        int interLeft = Math.Max(r1MinX, r2MinX);
        int interTop = Math.Max(r1MinY, r2MinY);
        int interRight = Math.Min(r1MaxX, r2MaxX);
        int interBottom = Math.Min(r1MaxY, r2MaxY);

        // If the intersection is valid (positive non zero area), then there is an intersection
        if (interLeft < interRight && interTop < interBottom)
        {
            overlap.Left = interLeft;
            overlap.Top = interTop;
            overlap.Right = interRight;
            overlap.Bottom = interBottom;
            return true;
        }

        overlap.Left = 0;
        overlap.Top = 0;
        overlap.Right = 0;
        overlap.Bottom = 0;
        return false;
    }

    /// <summary>
    /// Provide a string describing the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"IntRect: Left({Left}), Top({Top}), Right({Right}), Bottom({Bottom})";
    }

    /// <summary>
    /// Compare rectangle and object and checks if they are equal
    /// </summary>
    public override bool Equals(object obj)
    {
        return obj is IntRect rect && Equals(rect);
    }

    /// <summary>
    /// Compare two rectangles and checks if they are equal
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IntRect other)
    {
        return Left == other.Left &&
               Top == other.Top &&
               Right == other.Right &&
               Bottom == other.Bottom;
    }

    /// <summary>
    /// Provide a integer describing the object
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return unchecked((int)((uint)Left ^
                               (((uint)Top << 13) | ((uint)Top >> 19)) ^
                               (((uint)Right << 26) | ((uint)Right >> 6)) ^
                               (((uint)Bottom << 7) | ((uint)Bottom >> 25))));
    }

    /// <summary>
    /// Compares the specified instances for equality
    /// </summary>
    public static bool operator ==(IntRect r1, IntRect r2)
    {
        return r1.Equals(r2);
    }

    /// <summary>
    /// Compares the specified instances for inequality
    /// </summary>
    /// <param name="r1"></param>
    /// <param name="r2"></param>
    /// <returns></returns>
    public static bool operator !=(IntRect r1, IntRect r2)
    {
        return !r1.Equals(r2);
    }

    /// <summary>
    /// Multiplies the specified IntRect with the specified factor
    /// </summary>
    public static IntRect operator *(IntRect left, int right)
    {
        left.Left *= right;
        left.Top *= right;
        left.Right *= right;
        left.Bottom *= right;
        return left;
    }

    /// <summary>
    /// Multiplies the specified IntRect
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static IntRect operator *(IntRect left, IntRect right)
    {
        left.Left *= right.Left;
        left.Top *= right.Top;
        left.Right *= right.Right;
        left.Bottom *= right.Bottom;
        return left;
    }
}