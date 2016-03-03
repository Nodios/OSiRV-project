using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSIRVProject
{
    public class Program
    {
        static void Main(string[] args)
        {
            //ffaaa99c4b8a4844ada44b5137dd2481
            Helper help = new Helper("<PRETPLATNICKI_BROJ>");

            string imgPath = string.Empty;
            string filePath = string.Empty;
            string dictionaryNumber = string.Empty; ;

            imgPath = help.GetValidImagePathorUrl();

            Console.WriteLine("Odaberi riječnik:");
            Console.WriteLine("1. English(default)");
            Console.WriteLine("2. Input custom  dictionary(*.txt) file path or url");
            dictionaryNumber = Console.ReadLine();
            switch (dictionaryNumber)
            {
                case "1":
                    Console.WriteLine("Odabrano 1");
                    filePath = "<PUTANJA DO ENGLESKOG RJECNIKA>";
                    break;
                case "2":
                    Console.WriteLine("Odabrano 2");
                    filePath = help.GetValidFilePathorUrl();
                    break;
                default:
                    Console.WriteLine("Default is English");
                    filePath = "<PUTANJA DO ENGLESKOG RJECNIKA>";
                    break;
            }
            string[] dictionary = help.ReadTextFromFile(filePath).ToArray();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Done!");
            Console.ResetColor();

            //returns text on a picture
            //D:/JAVNA-Dino!!!/Pictures/Pictures/programerLove.jpg
            string text = help.ReturnRetrieveText(help.RecognizeText(imgPath));
            Console.Write(text);

            string[] readWords = help.SplitWords(text);

            help.CheckDictionary(dictionary, readWords);
        }
    }
}
