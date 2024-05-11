using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CirBIMGame
{
    public class matrix
    {
        public double[,] Matrix { get; set; }
        public int Row => Matrix.GetLength(0);
        public int Column => Matrix.GetLength(1);
        public int GetLength(int i) => Matrix.GetLength(i);
        public int Length => Matrix.Length;
        public double this[int row, int column]
        {
            get
            {
                if (column < this.Column &&
                row < this.Row)
                    return Matrix[row, column];
                else
                    return 0;
            }
            set { Matrix[row, column] = value; }
        }
        public override string ToString()
        {
            var Parse = new string[Row];
            for (int i = 0; i < this.Row; i++)
            {
                var Layer = new string[this.Column + 1];
                for (int j = 0; j < this.Column; j++)
                {
                    Layer[j] = this[i, j].ToString();
                }
                Layer[Column] = "";
                var Str = string.Join(" ", Layer);
                Parse[i] = Str;
            }
            return string.Join(";\n", Parse);
        }
        public matrix(int row, int column)
        {
            this.Matrix = new double[row, column];
            for (int i = 0; i < row; i++)
                for (int j = 0; j < column; j++)
                    this.Matrix[i, j] = 0;
        }
        public matrix(string parse)
        {
            var MtStr = parse.Split(';').Select(x => CleanSequence(x)).ToArray();
            var Length = MtStr[0].Split(' ').ToArray().Length;
            this.Matrix = new double[MtStr.Length, Length];
            for (int i = 0; i < MtStr.Length; i++)
            {
                var Element = MtStr[i].Split(' ').ToArray();
                for (int j = 0; j < Element.Length; j++)
                {
                    if (Element[j] == " ") continue;
                    try
                    {
                        this.Matrix[i, j] = double.Parse(Element[j]);
                    }
                    catch
                    {
                        throw new Exception("Parsing failed");
                    }
                }
            }
        }
        public IEnumerable<matrix> CreateMatrixList(params string[] MatrixParse)
        {
            foreach (var item in MatrixParse)
            {
                yield return new matrix(item);
            }
        }

        public static matrix operator *(matrix Mt1, matrix Mt2)
            => Mt1.Multiply(Mt2);
        public static matrix Pow(matrix Mt, int exponent)
        {
            for (int i = 0; i < exponent - 1; i++)
            {
                Mt *= Mt;
            }
            return Mt;
        }
        public matrix(double[,] m)
        {
            this.Matrix = m;
        }
        public matrix(matrix m)
        {
            this.Matrix = new double[m.Length / m.GetLength(0), m.GetLength(0)];
            for (int i = 0; i < m.Length / m.GetLength(0); i++)
                for (int j = 0; j < m.GetLength(0); j++)
                    this.Matrix[i, j] = m[i, j];
        }
        public static matrix Multiply(params matrix[] Mts)
        {
            var M = new matrix(Mts[0]);
            for (int i = 1; i < Mts.Length; i++)
            {
                M *= Mts[i];
            }
            return M;
        }
        public matrix Multiply(matrix other)
        {
            if (this.Column != other.Row)
                throw new ArgumentException("Invalid dimensions for matrix multiplication.");
            // 1 x 6 * 6 x 6 = 1 x 6
            // R x C * r x c = R x c
            int resultRows = this.Row;
            int resultCols = other.Column;
            int commonDim = this.Column;



            double[,] result = new double[resultRows, resultCols];

            for (int i = 0; i < resultRows; i++)
            {
                for (int j = 0; j < resultCols; j++)
                {
                    double sum = 0;
                    for (int k = 0; k < commonDim; k++)
                    {
                        sum += this.Matrix[i, k] * other.Matrix[k, j];
                    }
                    result[i, j] = sum;
                }
            }

            return new matrix(result);
        }
        private string CleanSequence(string Str)
        {
            while (Str[0] == ' ' || Str[Str.Length - 1] == ' ')
            {
                if (Str[0] == ' ')
                {
                    Str = Str.Remove(0);
                }
                if (Str[Str.Length - 1] == ' ')
                    Str = Str.Remove(Str.Length - 1);
            }
            return Str;
        }
        public static int DurationEva(matrix pi, matrix P)
        {
            int Count = 0;
            for (int i = 0; i < 20; i++)
            {
                var Result = pi * Pow(P, Count);
                if (Result[0, 5] > 0.8) break;
                Count++;
            }
            return Count;
        }
    }
}
