using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;

namespace generator
{
    class CharGenerator 
    {
        private string syms = "абвгдеёжзийклмнопрстуфхцчшщьыъэюя";
        protected char[] data;
        protected int size;
        protected Random random = new Random();
        public CharGenerator() 
        {
           size = syms.Length;
           data = syms.ToCharArray(); 
        }
        public char GetSym() 
        {
           return data[random.Next(0, size)];
        }
    }

    class BigramGenerator : CharGenerator {

        private string syms = "абвгдежзийклмнопрстуфхцчшщьыэюя";

        private int[,] BigramTable;
        private int[] LineWeightSum;

        private int LastCharIndex;

        public BigramGenerator() : base()
        {
            data = syms.ToCharArray();
            size = syms.Length;

            BigramTable = new int[this.size, this.size];

            using (var parser = new TextFieldParser("../../../../data/bigram.csv")) {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                for (int i = 0; i < this.size; i++)
                {
                    //Process row
                    string[] fields = parser.ReadFields();

                    for (int j = 0; j < this.size; j++) {
                        string field = fields[j];

                        if (field != "")
                        {
                            BigramTable[i, j] = int.Parse(field);
                        }
                        else
                        {
                            BigramTable[i, j] = 0;
                        }
                    }
                }
            }

            LineWeightSum = new int[this.size];

            for (int i = 0; i < this.size; i++)
            {
                LineWeightSum[i] = 0;

                for (int j = 0; j < this.size; j++)
                {
                    LineWeightSum[i] += BigramTable[i, j];
                }
            }

            LastCharIndex = 0;
        }

        new public char GetSym()
        {
            int i = LastCharIndex;

            int sourced_random_number = random.Next(0, this.LineWeightSum[i]);

            int chosen_index = 0;

            for (int j = 0; j < this.size; j++) {

                chosen_index = j;

                if (sourced_random_number - BigramTable[i, j] < 0)
                {
                    break;
                }
                else
                {
                    sourced_random_number -= BigramTable[i, j];
                }
            }

            return data[chosen_index];
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BigramGenerator gen = new BigramGenerator();
            SortedDictionary<char, int> stat = new SortedDictionary<char, int>();
            for(int i = 0; i < 1000; i++) 
            {
               char ch = gen.GetSym(); 
               if (stat.ContainsKey(ch))
                  stat[ch]++;
               else
                  stat.Add(ch, 1); Console.Write(ch);
            }
            Console.Write('\n');
            foreach (KeyValuePair<char, int> entry in stat) 
            {
                 Console.WriteLine("{0} - {1}",entry.Key,entry.Value/1000.0); 
            }
            
        }
    }
}

