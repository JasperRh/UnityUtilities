using System;
using System.Collections.Generic;

namespace Crimsilk.Utilities.Collections
{
    public interface IMatrix<T> : IEnumerable<T>
    {
        int Count { get; }
        int Width { get; }
        int Height { get; }
        T this[int index] { get; set; }
        T this[T item] { get; set; }

        /// <summary>
        /// The columns and rows are index based and starting at 0
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        T this[int column, int row] { get; set; }

        /// <summary>
        /// Returns the position of the item in the matrix. The position starts at 0.
        /// </summary>
        /// <param name="item">The item to get the position of.</param>
        /// <returns>Returns the column and row of the item in the matrix. Returns (-1, -1) if item has not been found.</returns>
        (int column, int row) PointOf(T item);

        /// <summary>
        /// Returns the position of the index in the matrix. The position starts at 0.
        /// </summary>
        /// <param name="index">The index to get the position of.</param>
        /// <returns>Returns the column and row of the index in the matrix. Returns (-1, -1) if index has not been found.</returns>
        (int column, int row) PointOf(int index);

        /// <summary>
        /// Returns all elements within the given range (bounding box fashion) starting from top left to bottom right.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Throws exception when the the given range is out of bounds.</exception>
        /// <returns>Matrix with the given range.</returns>
        IMatrix<T> GetRange(int startColumn, int startRow, int endColumn, int endRow);

        /// <summary>
        /// Returns all elements within the given range (bounding box fashion) starting from top left to bottom right.
        /// It will clamp them within the dimensions of the matrix. This allows the given parameters to fall outside of the matrix without throwing an exception.
        /// It will try to clamp the range to the closest possible position in the matrix by shifting the whole range in order to fit within the matrix.
        /// Note: The returned range might be in a different position than the requested range because of shifting the range in order to fit.
        /// If you want to maintain the original position, use <see cref="GetRangeClamped"/> instead.
        /// </summary>
        /// <returns>Matrix that fall within the given range or the shifted alternative if it falls outside.</returns>
        IMatrix<T> GetRangeShifted(int startColumn, int startRow, int endColumn, int endRow);

        /// <summary>
        /// Returns all elements within the given range (bounding box fashion) starting from top left to bottom right.
        /// It will clamp them within the dimensions of the matrix by cutting the range in order to fit within the matrix.
        /// Note: The returned range might be smaller than the requested range because of cutting the range in order to fit.
        /// If you want to maintain the original size, use <see cref="GetRangeShifted"/> instead.
        /// </summary>
        /// <returns>Matrix that fall within the given range or the clamped alternative it it falls outside.</returns>
        IMatrix<T> GetRangeClamped(int startColumn, int startRow, int endColumn, int endRow);

        /// <summary>
        /// Clamps the given column and row so it fits within the matrix.
        /// </summary>
        /// <returns></returns>
        (int column, int row) Clamp(int column, int row);

        void Clear();
        bool Contains(T item);
        bool Remove(T item);
    }
}