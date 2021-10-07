using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIR1
{
    class Program
    {
        static object locker = new object();

        static Random random = new Random();

        static void Main(string[] args)
        {
            try
            {
                string alphabet = "";
                string sample = "";

                Console.Write("Минимальная длина строки: ");
                int n = int.Parse(Console.ReadLine());//От
                Console.Write("Максимальная длина строки: ");
                int m = int.Parse(Console.ReadLine());//До

                int[] numbers = { 50, 100, 500, 1000, 1500 };

                #region Проверка на пустоту
                //===================================================================
                Console.ForegroundColor = ConsoleColor.Blue;
                if (string.IsNullOrEmpty(alphabet))
                {
                    Console.Write("Введите алфавит: ");
                    alphabet = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine($"Алфавит: {alphabet}");
                }
                Console.ForegroundColor = ConsoleColor.Red;
                if (string.IsNullOrEmpty(sample))
                {
                    Console.Write("Введите подстроку: ");
                    sample = Console.ReadLine();
                }
                else
                {
                    Console.WriteLine($"Подстрока: {sample}");
                }
                Console.ResetColor();
                //===================================================================
                #endregion

                foreach (var item in numbers)
                {
                    string[] arrStr = new string[item];
                    arrStr = arrStr.ToList().Select(x => new string(GenerationString(n, m, null))).ToArray();

                    //arrStr.ToList().ForEach(x => Console.WriteLine(x));

                   

                    if (string.IsNullOrEmpty(sample))
                    {
                        Console.WriteLine("А искать то что?");
                    }
                    else
                    {
                        Console.WriteLine($"Совпадения: " + ((SearchNativeLogic(arrStr, sample)) ? "есть" : "нет"));
                        Console.WriteLine($"Совпадения: " + ((SearchKMPLogic(arrStr, sample)) ? "есть" : "нет"));
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static char[] GenerationString(int n, int m, char[] text = default, string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz ")
        {
            int length = random.Next(n, m + 1);

            if (text == null)
            {
                text = new char[length];
                if (Parallel.For(0, length, x =>
                {
                    int index;
                    lock (locker)
                    {
                        index = random.Next(0, alphabet.Length);
                    }
                    text[x] = alphabet[index];
                }).IsCompleted)
                {
                    return text;
                }
            }
            return text;
        }

        static bool SearchNativeLogic(string[] text, string sample)
        {
            bool IsSearch = false;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Наивный алгоритм:");
            Console.ResetColor();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<List<int>> search = new List<List<int>>();
            for (int i = 0; i < text.Length; i++)
            {
                var searchInString = SearchNative(text[i], sample);
                search.Add(searchInString);

                if (searchInString.Count > 0)
                {
                    IsSearch = true;
                }
            }

            stopwatch.Stop();
            //for (int i = 0; i < search.Count; i++)
            //{
            //    PrintOutput(search[i], sample);
            //}

            Console.WriteLine(stopwatch?.Elapsed);

            return IsSearch;
        }

        static bool SearchKMPLogic(string[] text, string sample)
        {
            bool IsSearch = false;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("KMP алгоритм:");
            Console.ResetColor();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<List<int>> search = new List<List<int>>();
            for (int i = 0; i < text.Length; i++)
            {
                var searchInString = SearchKMP(text[i], sample);
                search.Add(searchInString);

                if (searchInString.Count > 0)
                {
                    IsSearch = true;
                }
            }

            stopwatch.Stop();
            //for (int i = 0; i < search.Count; i++)
            //{
            //    PrintOutput(search[i], sample);
            //}
            Console.WriteLine(stopwatch?.Elapsed);

            return IsSearch;
        }

        static void PrefixFunction(int[] values, string sample, int sampleLength)
        {
            for (int i = 1; i < sampleLength; i++)
            {
                for (int j = 0; i + j < sampleLength && sample[j] == sample[i + j]; j++)
                {
                    values[i + j] = Math.Max(values[i + j], j + 1);
                }
            }

            //foreach (var item in values)
            //{
            //    Console.Write(item+" ");
            //}
            //Console.WriteLine();
        }

        static List<int> SearchKMP(string text, string sample)
        {
            int textLength = text.Length;
            int sampleLength = sample.Length;

            List<int> foundPosition = new List<int>();
            int[] prefixFunc = new int[sampleLength];

            PrefixFunction(prefixFunc, sample, sampleLength);

            for (int i = 0, j = 0; i < textLength;)
            {
                if (sample[j] == text[i])
                {
                    i++;
                    j++;
                }
                if (j == sampleLength)
                {
                    foundPosition.Add(i - j);
                    j = prefixFunc[j - 1];
                }

                else if (i < textLength && sample[j] != text[i])
                {
                    if (j != 0)
                    {
                        j = prefixFunc[j - 1];
                    }
                    else i++;
                }
            }
            return foundPosition;
        }

        static List<int> SearchNative(string text, string sample)
        {
            List<int> foundPosition = new List<int>();

            for (int i = 0; i < text.Length; i++)
            {
                int position;
                for (position = 0; position < sample.Length && i + position < text.Length && text[i + position] == sample[position]; position++)
                {
                    if (position == sample.Length - 1)
                    {
                        foundPosition.Add(i);
                    }
                }
            }
            return foundPosition;
        }

        static void PrintOutput(List<int> searchText, string sample, Stopwatch stopwatch = default)
        {
            if (searchText.Count == 0)
            {
                Console.WriteLine("Совпадений не найдено!");
            }
            else
            {
                foreach (var item in searchText)
                {
                    if (sample.Length == 1)
                        Console.Write($"[{item}] ");
                    else
                        Console.Write($"[{item}..{item + sample.Length - 1}] ");
                }
            }
        }
    }
}
