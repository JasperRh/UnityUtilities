using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crimsilk.Utilities.Collections
{
    [Serializable]
    public class Matrix<T> : IMatrix<T>
    {
        public int Count => collection.Count();
        public bool IsReadOnly => false;

        public int Width => width;
        public int Height => height;
        
        protected List<T> collection;

        [SerializeField]
        protected int width, height;
        
        public Matrix(int width, int height)
        {
            this.width = width;
            this.height = height;

            collection = new List<T>(new T[width * height]);
        }

        public Matrix(int width, int height, T[] array)
        {
            this.width = width;
            this.height = height;

            if (array.Length != width * height)
            {
                throw new ArgumentException($"The array is not the same length as the Matrix dimensions. \n" +
                                            $"Length: {array.Length}\n" +
                                            $"Dimensions: {width}x{height}\n" +
                                            $"Expected length: {width * height}");
            }
            collection = new List<T>(array);
        }

        public T this[int index]
        {
            get => collection[index];
            set => collection[index] = value;
        }
        
        public T this[T item]
        {
            get => item;
            set => collection[collection.IndexOf(item)] = value;
        }
        
        public T this[int column, int row]
        {
            get => collection[(row * Width) + column];
            set => collection[(row * Width) + column] = value;
        }
        
        public (int column, int row) PointOf(T item)
        {
            int index = collection.IndexOf(item);
            if (index < 0)
            {
                return (-1, -1);
            }

            return (index % Width, index / Width);
        }

        public (int column, int row) PointOf(int index)
        {
            if (index < 0)
            {
                return (-1, -1);
            }

            return (index % Width, index / Width);
        }

        public IMatrix<T> GetRange(int startColumn, int startRow, int endColumn, int endRow)
        {
            if (startColumn < 0 || startRow < 0 || endColumn >= width || endRow >= height)
            {
                throw new ArgumentOutOfRangeException(
                    $"Matrix dimensions are out of range. Matrix dimensions are '{Width}:{Height}'");
            }

            if (endColumn < startColumn || endRow < startRow)
            {
                throw new ArgumentException($"'{nameof(endColumn)}' or '{nameof(endRow)}' cannot be smaller than start parameters.");
            }

            IEnumerable<T> range = collection.Where((_, i) =>
            {
                (int column, int row) = PointOf(i);
                return column >= startColumn && column <= endColumn && row >= startRow && row <= endRow;
            });
            IMatrix<T> subMatrix = new Matrix<T>((endColumn - startColumn) + 1, (endRow - startRow) + 1, range.ToArray());
            
            return subMatrix;
        }
        
        public IMatrix<T> GetRangeShifted(int startColumn, int startRow, int endColumn, int endRow)
        {
            if (endColumn < startColumn || endRow < startRow)
            {
                throw new ArgumentException($"'{nameof(endColumn)}' or '{nameof(endRow)}' cannot be smaller than start parameters.");
            }
            
            // Check if the given boundingbox is not bigger than the actual matrix.
            if(endColumn - startColumn >= width|| endRow - startRow >= height)
            {
                throw new ArgumentOutOfRangeException(
                    $"Cannot get shifted range. The range exceeds the boundaries of the matrix. \n" +
                    $"Range width: {startColumn + endColumn}\n" +
                    $"Matrix width: {width}\n" +
                    $"Range height: {startRow + endRow}\n" +
                    $"Matrix height: {height}");
            }
            
            if (startColumn < 0)
            {
                // Shift the whole range to the right.
                endColumn += Math.Abs(startColumn);
                startColumn = 0;
            }
            if (startRow < 0)
            {
                // Shift the whole range up.
                endRow += Math.Abs(startRow);
                startRow = 0;
            }
            if (endColumn >= width)
            {
                // Shift the whole to the left.
                startColumn -= endColumn - (width - 1);
                endColumn = width - 1;
            }
            if (endRow >= height)
            {
                // Shift the whole down.
                startRow -= endRow - (height - 1);
                endRow = height - 1;
            }

            return GetRange(startColumn, startRow, endColumn, endRow);
        }

        public IMatrix<T> GetRangeClamped(int startColumn, int startRow, int endColumn, int endRow)
        {
            if (endColumn < startColumn || endRow < startRow)
            {
                throw new ArgumentException($"'{nameof(endColumn)}' or '{nameof(endRow)}' cannot be smaller than start parameters.");
            }
            
            startColumn = startColumn < 0 ? 0 : startColumn;
            startRow = startRow < 0 ? 0 : startRow;
            endColumn = endColumn >= width ? width - 1 : endColumn;
            endRow = endRow >= height ? height - 1 : endRow;
            
            return GetRange(startColumn, startRow, endColumn, endRow);
        }
        
        public (int column, int row) Clamp(int column, int row)
        {
            int x = Math.Clamp(column, 0, width - 1);
            int y = Math.Clamp(row, 0, height - 1);

            return (x, y);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MatrixEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear()
        {
            collection.Clear();
        }

        public bool Contains(T item)
        {
            return collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            int index = collection.IndexOf(item);
            if (index >= 0)
            {
                collection[index] = default(T);
                return true;
            }

            return false;
        }
        
        private class MatrixEnumerator<T> : IEnumerator<T> 
        {
            public T Current { get; private set; }
            object IEnumerator.Current => Current;
            
            private int position = -1;
            private Matrix<T> matrix;
            private T current;

            public MatrixEnumerator(Matrix<T> matrix)
            {
                this.matrix = matrix ?? throw new ArgumentNullException(nameof(matrix));
            }
            
            public bool MoveNext()
            {
                position++;

                if (position < matrix.Count)
                {
                    Current = matrix[position];
                    return true;
                }
                
                return false;
            }

            public void Reset()
            {
                position = -1;
                Current = default(T);
            }

            public void Dispose()
            {
                
            }
        }
    }
}