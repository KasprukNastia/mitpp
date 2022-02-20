using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab2_Matrix
{
    class SquareMatrix
    {
        public int Dimension { get; }
        public int[,] Matrix { get; }

        public SquareMatrix(int dimension)
        {
            if (dimension <= 0)
                throw new ArgumentException($"Matrix with dimension {dimension} cannot be created");
            Dimension = dimension;
            Matrix = new int[Dimension, Dimension];
            var rnd = new Random();
            for(int i = 0; i< Dimension; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    Matrix[i, j] = rnd.Next(1, 100);
                }
            }
        }

        public SquareMatrix(int[,] matrix)
        {
            if (matrix.GetLength(0) != matrix.GetLength(1))
                throw new ArgumentException("Input matrix is not square matrix");
            Dimension = matrix.GetLength(0);
            Matrix = matrix;
        }

        public async Task<int[]> MultiplyTransformMergeAsync(SquareMatrix another,
            Func<int, int> transform,
            Func<int[], int> merge)
        {
            int[][] multResult = new int[Dimension][];

            List<Task> calculationTasks = new List<Task>(Dimension * Dimension);
            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    calculationTasks.Add(Task.Run(() => 
                    {
                        multResult[i][j] = 0;
                        for (int k = 0; k < Dimension; k++)
                        {
                            multResult[i][j] += Matrix[i, k] * another.Matrix[k, j];
                        }
                        multResult[i][j] = transform(multResult[i][j]);
                    }));
                }
            }
            await Task.WhenAll(calculationTasks);

            calculationTasks.Clear();
            int[] vectorResult = new int[Dimension];
            for (int i = 0; i < Dimension; i++)
            {
                calculationTasks.Add(Task.Run(() =>
                {
                    vectorResult[i] = merge(multResult[i]);
                }));
            }
            await Task.WhenAll(calculationTasks);

            return vectorResult;
        }
    }
}
