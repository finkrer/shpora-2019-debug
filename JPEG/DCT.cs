using System;
using System.Threading.Tasks;
// ReSharper disable HeuristicUnreachableCode
#pragma warning disable 162

namespace JPEG
{
    public class DCT
    {
        private static readonly double Sqrt2 = Math.Sqrt(2);

        private static readonly Action<double[]> DCTMethod;

        private static readonly Action<double[]> IDCTMethod;

        private static readonly double[] S = new double[8];

        private static readonly double[] A = new double[6];

        static DCT()
        {
            if (Program.DCTSize == 8)
            {
                DCTMethod = DCT8_1D;
                IDCTMethod = IDCT8_1D;
            }

            else
            {
                DCTMethod = DCT1D;
                IDCTMethod = IDCT1D;
            }

            var C = new double[8];
            for (var i = 0; i < C.Length; i++)
            {
                C[i] = Math.Cos(Math.PI / 16 * i);
                S[i] = 1 / (4 * C[i]);
            }
            S[0] = 1 / (2 * Math.Sqrt(2));
            A[1] = C[4];
            A[2] = C[2] - C[6];
            A[3] = C[4];
            A[4] = C[6] + C[2];
            A[5] = C[6];
        }

        public static void DCT2D(double[,] data)
        {
            var rows = data.GetLength(0);
            var cols = data.GetLength(1);

            Parallel.For(0, rows, i => { DCT8_1DRow(data, i); });

            Parallel.For(0, cols, j => { DCT8_1DColumn(data, j); });
        }

        public static void IDCT2D(double[,] data)
        {
            var rows = data.GetLength(0);
            var cols = data.GetLength(1);

            Parallel.For(0, cols, j => { IDCT8_1DColumn(data, j); });

            Parallel.For(0, rows, i => { IDCT8_1DRow(data, i); });
        }

        public static void DCT1D(double[] data)
        {
            var result = new double[data.Length];
            var c = Math.PI / (2.0 * data.Length);
            var scale = Math.Sqrt(2.0 / data.Length);

            for (var k = 0; k < data.Length; k++)
            {
                double sum = 0;
                for (var n = 0; n < data.Length; n++)
                    sum += data[n] * Math.Cos((2.0 * n + 1.0) * k * c);
                result[k] = scale * sum;
            }

            data[0] = result[0] / Sqrt2;
            for (var i = 1; i < data.Length; i++)
                data[i] = result[i];
        }

        public static void IDCT1D(double[] data)
        {
            var result = new double[data.Length];
            var c = Math.PI / (2.0 * data.Length);
            var scale = Math.Sqrt(2.0 / data.Length);

            for (var k = 0; k < data.Length; k++)
            {
                var sum = data[0] / Sqrt2;
                for (var n = 1; n < data.Length; n++)
                    sum += data[n] * Math.Cos((2 * k + 1) * n * c);

                result[k] = scale * sum;
            }

            for (var i = 0; i < data.Length; i++)
                data[i] = result[i];
        }

        public static void DCT8_1D(double[] vector)
        {
            var v0 = vector[0] + vector[7];
            var v1 = vector[1] + vector[6];
            var v2 = vector[2] + vector[5];
            var v3 = vector[3] + vector[4];
            var v4 = vector[3] - vector[4];
            var v5 = vector[2] - vector[5];
            var v6 = vector[1] - vector[6];
            var v7 = vector[0] - vector[7];

            var v8 = v0 + v3;
            var v9 = v1 + v2;
            var v10 = v1 - v2;
            var v11 = v0 - v3;
            var v12 = -v4 - v5;
            var v13 = (v5 + v6) * A[3];
            var v14 = v6 + v7;

            var v15 = v8 + v9;
            var v16 = v8 - v9;
            var v17 = (v10 + v11) * A[1];
            var v18 = (v12 + v14) * A[5];

            var v19 = -v12 * A[2] - v18;
            var v20 = v14 * A[4] - v18;

            var v21 = v17 + v11;
            var v22 = v11 - v17;
            var v23 = v13 + v7;
            var v24 = v7 - v13;

            var v25 = v19 + v24;
            var v26 = v23 + v20;
            var v27 = v23 - v20;
            var v28 = v24 - v19;

            vector[0] = S[0] * v15;
            vector[1] = S[1] * v26;
            vector[2] = S[2] * v21;
            vector[3] = S[3] * v28;
            vector[4] = S[4] * v16;
            vector[5] = S[5] * v25;
            vector[6] = S[6] * v22;
            vector[7] = S[7] * v27;
        }

        public static void DCT8_1DRow(double[,] vector, int row)
        {
            var v0 = vector[0, row] + vector[7, row];
            var v1 = vector[1, row] + vector[6, row];
            var v2 = vector[2, row] + vector[5, row];
            var v3 = vector[3, row] + vector[4, row];
            var v4 = vector[3, row] - vector[4, row];
            var v5 = vector[2, row] - vector[5, row];
            var v6 = vector[1, row] - vector[6, row];
            var v7 = vector[0, row] - vector[7, row];

            var v8 = v0 + v3;
            var v9 = v1 + v2;
            var v10 = v1 - v2;
            var v11 = v0 - v3;
            var v12 = -v4 - v5;
            var v13 = (v5 + v6) * A[3];
            var v14 = v6 + v7;

            var v15 = v8 + v9;
            var v16 = v8 - v9;
            var v17 = (v10 + v11) * A[1];
            var v18 = (v12 + v14) * A[5];

            var v19 = -v12 * A[2] - v18;
            var v20 = v14 * A[4] - v18;

            var v21 = v17 + v11;
            var v22 = v11 - v17;
            var v23 = v13 + v7;
            var v24 = v7 - v13;

            var v25 = v19 + v24;
            var v26 = v23 + v20;
            var v27 = v23 - v20;
            var v28 = v24 - v19;

            vector[0, row] = S[0] * v15;
            vector[1, row] = S[1] * v26;
            vector[2, row] = S[2] * v21;
            vector[3, row] = S[3] * v28;
            vector[4, row] = S[4] * v16;
            vector[5, row] = S[5] * v25;
            vector[6, row] = S[6] * v22;
            vector[7, row] = S[7] * v27;
        }

        public static void DCT8_1DColumn(double[,] vector, int column)
        {
            var v0 = vector[column, 0] + vector[column, 7];
            var v1 = vector[column, 1] + vector[column, 6];
            var v2 = vector[column, 2] + vector[column, 5];
            var v3 = vector[column, 3] + vector[column, 4];
            var v4 = vector[column, 3] - vector[column, 4];
            var v5 = vector[column, 2] - vector[column, 5];
            var v6 = vector[column, 1] - vector[column, 6];
            var v7 = vector[column, 0] - vector[column, 7];

            var v8 = v0 + v3;
            var v9 = v1 + v2;
            var v10 = v1 - v2;
            var v11 = v0 - v3;
            var v12 = -v4 - v5;
            var v13 = (v5 + v6) * A[3];
            var v14 = v6 + v7;

            var v15 = v8 + v9;
            var v16 = v8 - v9;
            var v17 = (v10 + v11) * A[1];
            var v18 = (v12 + v14) * A[5];

            var v19 = -v12 * A[2] - v18;
            var v20 = v14 * A[4] - v18;

            var v21 = v17 + v11;
            var v22 = v11 - v17;
            var v23 = v13 + v7;
            var v24 = v7 - v13;

            var v25 = v19 + v24;
            var v26 = v23 + v20;
            var v27 = v23 - v20;
            var v28 = v24 - v19;

            vector[column, 0] = S[0] * v15;
            vector[column, 1] = S[1] * v26;
            vector[column, 2] = S[2] * v21;
            vector[column, 3] = S[3] * v28;
            vector[column, 4] = S[4] * v16;
            vector[column, 5] = S[5] * v25;
            vector[column, 6] = S[6] * v22;
            vector[column, 7] = S[7] * v27;
        }

        public static void IDCT8_1D(double[] vector)
        {
            var v15 = vector[0] / S[0];
            var v26 = vector[1] / S[1];
            var v21 = vector[2] / S[2];
            var v28 = vector[3] / S[3];
            var v16 = vector[4] / S[4];
            var v25 = vector[5] / S[5];
            var v22 = vector[6] / S[6];
            var v27 = vector[7] / S[7];

            var v19 = (v25 - v28) / 2;
            var v20 = (v26 - v27) / 2;
            var v23 = (v26 + v27) / 2;
            var v24 = (v25 + v28) / 2;

            var v7 = (v23 + v24) / 2;
            var v11 = (v21 + v22) / 2;
            var v13 = (v23 - v24) / 2;
            var v17 = (v21 - v22) / 2;

            var v8 = (v15 + v16) / 2;
            var v9 = (v15 - v16) / 2;

            var v18 = (v19 - v20) * A[5];
            var v12 = (v19 * A[4] - v18) / (A[2] * A[5] - A[2] * A[4] - A[4] * A[5]);
            var v14 = (v18 - v20 * A[2]) / (A[2] * A[5] - A[2] * A[4] - A[4] * A[5]);

            var v6 = v14 - v7;
            var v5 = v13 / A[3] - v6;
            var v4 = -v5 - v12;
            var v10 = v17 / A[1] - v11;

            var v0 = (v8 + v11) / 2;
            var v1 = (v9 + v10) / 2;
            var v2 = (v9 - v10) / 2;
            var v3 = (v8 - v11) / 2;

            vector[0] = (v0 + v7) / 2;
            vector[1] = (v1 + v6) / 2;
            vector[2] = (v2 + v5) / 2;
            vector[3] = (v3 + v4) / 2;
            vector[4] = (v3 - v4) / 2;
            vector[5] = (v2 - v5) / 2;
            vector[6] = (v1 - v6) / 2;
            vector[7] = (v0 - v7) / 2;
        }

        public static void IDCT8_1DRow(double[,] vector, int row)
        {
            var v15 = vector[0, row] / S[0];
            var v26 = vector[1, row] / S[1];
            var v21 = vector[2, row] / S[2];
            var v28 = vector[3, row] / S[3];
            var v16 = vector[4, row] / S[4];
            var v25 = vector[5, row] / S[5];
            var v22 = vector[6, row] / S[6];
            var v27 = vector[7, row] / S[7];

            var v19 = (v25 - v28) / 2;
            var v20 = (v26 - v27) / 2;
            var v23 = (v26 + v27) / 2;
            var v24 = (v25 + v28) / 2;

            var v7 = (v23 + v24) / 2;
            var v11 = (v21 + v22) / 2;
            var v13 = (v23 - v24) / 2;
            var v17 = (v21 - v22) / 2;

            var v8 = (v15 + v16) / 2;
            var v9 = (v15 - v16) / 2;

            var v18 = (v19 - v20) * A[5];
            var v12 = (v19 * A[4] - v18) / (A[2] * A[5] - A[2] * A[4] - A[4] * A[5]);
            var v14 = (v18 - v20 * A[2]) / (A[2] * A[5] - A[2] * A[4] - A[4] * A[5]);

            var v6 = v14 - v7;
            var v5 = v13 / A[3] - v6;
            var v4 = -v5 - v12;
            var v10 = v17 / A[1] - v11;

            var v0 = (v8 + v11) / 2;
            var v1 = (v9 + v10) / 2;
            var v2 = (v9 - v10) / 2;
            var v3 = (v8 - v11) / 2;

            vector[0, row] = (v0 + v7) / 2;
            vector[1, row] = (v1 + v6) / 2;
            vector[2, row] = (v2 + v5) / 2;
            vector[3, row] = (v3 + v4) / 2;
            vector[4, row] = (v3 - v4) / 2;
            vector[5, row] = (v2 - v5) / 2;
            vector[6, row] = (v1 - v6) / 2;
            vector[7, row] = (v0 - v7) / 2;
        }

        public static void IDCT8_1DColumn(double[,] vector, int column)
        {
            var v15 = vector[column, 0] / S[0];
            var v26 = vector[column, 1] / S[1];
            var v21 = vector[column, 2] / S[2];
            var v28 = vector[column, 3] / S[3];
            var v16 = vector[column, 4] / S[4];
            var v25 = vector[column, 5] / S[5];
            var v22 = vector[column, 6] / S[6];
            var v27 = vector[column, 7] / S[7];

            var v19 = (v25 - v28) / 2;
            var v20 = (v26 - v27) / 2;
            var v23 = (v26 + v27) / 2;
            var v24 = (v25 + v28) / 2;

            var v7 = (v23 + v24) / 2;
            var v11 = (v21 + v22) / 2;
            var v13 = (v23 - v24) / 2;
            var v17 = (v21 - v22) / 2;

            var v8 = (v15 + v16) / 2;
            var v9 = (v15 - v16) / 2;

            var v18 = (v19 - v20) * A[5];
            var v12 = (v19 * A[4] - v18) / (A[2] * A[5] - A[2] * A[4] - A[4] * A[5]);
            var v14 = (v18 - v20 * A[2]) / (A[2] * A[5] - A[2] * A[4] - A[4] * A[5]);

            var v6 = v14 - v7;
            var v5 = v13 / A[3] - v6;
            var v4 = -v5 - v12;
            var v10 = v17 / A[1] - v11;

            var v0 = (v8 + v11) / 2;
            var v1 = (v9 + v10) / 2;
            var v2 = (v9 - v10) / 2;
            var v3 = (v8 - v11) / 2;

            vector[column, 0] = (v0 + v7) / 2;
            vector[column, 1] = (v1 + v6) / 2;
            vector[column, 2] = (v2 + v5) / 2;
            vector[column, 3] = (v3 + v4) / 2;
            vector[column, 4] = (v3 - v4) / 2;
            vector[column, 5] = (v2 - v5) / 2;
            vector[column, 6] = (v1 - v6) / 2;
            vector[column, 7] = (v0 - v7) / 2;
        }
    }
}