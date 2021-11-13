namespace Collard
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    
    public class Utils
    {

        public static IEnumerable<String> GetStringIterable(string text)
        {
            string line;
            using StringReader reader = new StringReader(text);
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }

            reader.Close();
        }

        public static IEnumerable<(int, T)> GetIndexEnumerable<T>(T[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                yield return (i, arr[i]);
            }
        }

        public static IEnumerable<(int, int, T)> Get2DEnumerable<T>(T[,] arr)
        {
            for (int r = 0; r < arr.GetLength(0); r++)
            {
                for (int c = 0; c < arr.GetLength(1); c++)
                {
                    yield return (r, c, arr[r, c]);
                }
            }
        }
    }
}