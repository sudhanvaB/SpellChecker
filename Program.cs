using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SpellCheckApp app = new SpellCheckApp();
            app.inputFile = "C:\\Users\\Sudhanva B\\Desktop\\SpellCheckFiles\\input.txt";
            app.outputFile = "C:\\Users\\Sudhanva B\\Desktop\\SpellCheckFiles\\output.txt";
            Stopwatch stopwatch = Stopwatch.StartNew();
            app.StartSpellCheck();
            Console.WriteLine("Time Taken = "+stopwatch.ElapsedMilliseconds);

        }
    }
    class SpellCheckApp
    {
        public string inputFile { get; set; }
        public string outputFile { get; set; }
        public void StartSpellCheck()
        {
            List<string> inputWords =  InputFileLoader.Load(this.inputFile);
            using (StreamWriter sw1 = new StreamWriter(outputFile))
            {
                sw1.Write("");
            }
            using (StreamWriter sw = new StreamWriter(outputFile,true))
            {
                //foreach (string word in inputWords)
                Parallel.ForEach(inputWords, (word) =>
                {
                    {
                        List<string> closestWords = SpellChecker.Check(word);
                        if (closestWords != null)
                        {
                            sw.WriteLine($"{word}");
                            foreach (string word1 in closestWords)
                            {
                                sw.WriteLine($"\t{word1}");
                            }
                        }
                    }
                });
            }
        }
    }
    class InputFileLoader
    {
        public static List<string> Load(string inputFile)
        {
            List<string> strings = new List<string>();
            string[] list = null;
            using(StreamReader sr = new StreamReader(inputFile))
            {
                string s = sr.ReadToEnd();
                list = s.Split(new char[] { ' ', '\n' });
            }
            Parallel.ForEach(list, x => {
                strings.Add(x);
            });
            
            return strings;
        }
    }
    class SpellChecker
    {
        public static List<string> Check(string word)
        {
            List<string> dictionary =  DictionaryFileLoader.Load("C:\\Users\\Sudhanva B\\Desktop\\SpellCheckFiles\\dictionary.txt");
            if(dictionary.Contains(word))
            {
                return null;
            }
            Dictionary<string,int> dict  = new Dictionary<string,int>();
            Parallel.ForEach(dictionary, x =>
            {
                int k = LevenstheinDistance.Distance(word, x);
                lock (dict)
                {
                    dict.Add(x, k); 
                }
            });
            List<string> sortedWords = dict.OrderBy(x => x.Value).Select(x => x.Key).Take(10).ToList();
            return sortedWords;
        }
    }
    class LevenstheinDistance
    {
        public static int Distance(string word1, string word2)
        {
            int len1 = word1.Length;
            int len2 = word2.Length;

            int[,] distances = new int[len1 + 1, len2 + 1];

            for (int i = 0; i <= len1; i++)
            {
                distances[i, 0] = i;
            }

            for (int j = 0; j <= len2; j++)
            {
                distances[0, j] = j;
            }

            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    int cost = (word2[j - 1] == word1[i - 1]) ? 0 : 1;

                    distances[i, j] = Math.Min(
                        Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                        distances[i - 1, j - 1] + cost
                    );
                }
            }

            return distances[len1, len2];
        }

    }
    class DictionaryFileLoader
    {
        public static List<string> Load(string file)
        {
            List<string> strings= new List<string>();
            using(StreamReader sr = new StreamReader(file))
            {
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    string[] list = s.Split('/');
                    strings.Add(list[0]); 
                }
            }
            return strings;
        }
    }
}
