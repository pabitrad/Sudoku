using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuWPF
{
    class Utility
    {
        /// <summary>
        /// Load matrix data from text file.
        /// </summary>
        /// <param name="fileName"></param>
        public int[,] loadData(string fileName)
        {
            StreamReader fileReader = File.OpenText(fileName);
            int[,] points = null;
            if (fileReader != null)
            {
                string header = fileReader.ReadLine();
                char[] delimiterChars = { ' ' };
                string[] headerEntries = header.Split(delimiterChars);
                int rows = Convert.ToInt32(headerEntries[0]);
                int columns = Convert.ToInt32(headerEntries[1]);
                int maxValue = Convert.ToInt32(headerEntries[2]);
                int minValue = Convert.ToInt32(headerEntries[3]);

                points = new int[rows, columns];

                string line = fileReader.ReadLine();
                int rowIndex = 0;

                while (string.IsNullOrWhiteSpace(line) == false && line.Length > 0)
                {
                    string[] lineEntries = line.Split(delimiterChars);
                    for (int i = 0; i < columns; i++)
                    {
                        points[rowIndex, i] = Convert.ToInt32(lineEntries[i]);
                    }
                    rowIndex++;
                    line = fileReader.ReadLine();
                }
            }

            return points;
        }

    }
}
