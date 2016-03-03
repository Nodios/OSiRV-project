using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.IO;
using System.Text.RegularExpressions;

namespace OSIRVProject
{
    public class Helper
    {
        private readonly IVisionServiceClient Service;

        public Helper(string subKey)
        {
            Service = new VisionServiceClient(subKey);
        }

        /// <summary>
        /// Reads text from image.
        /// </summary>
        /// <param name="imagePathOrUrl">Input image path</param>
        /// <param name="detectOrientation">True, if you want to detect orientation for up to 30°</param>
        /// <param name="languageCode">Language to detect. Autodetect by default.</param>
        /// <returns>Returns</returns>
        public OcrResults RecognizeText(string imagePathOrUrl, bool detectOrientation = true, string languageCode = LanguageCodes.AutoDetect)
        {
            this.ShowInfo("Učitavanje");
            OcrResults ocrResult = null;
            string resultStr = string.Empty;

            try
            {
                if (File.Exists(imagePathOrUrl))
                {
                    using (FileStream stream = File.Open(imagePathOrUrl, FileMode.Open))
                    {
                        ocrResult = Service.RecognizeTextAsync(stream, languageCode, detectOrientation).Result;
                    }
                }
                else if (Uri.IsWellFormedUriString(imagePathOrUrl, UriKind.Absolute))
                {
                    ocrResult = Service.RecognizeTextAsync(imagePathOrUrl, languageCode, detectOrientation).Result;
                }
                else
                {
                    this.ShowError("Invalid image path or Url");
                }
            }
            catch (ClientException ex)
            {
                throw ex;
            }
            catch (Exception e)
            {
                throw e;
            }

            return ocrResult;
        }

        /// <summary>
        /// Reads text from a file, line by line.
        /// </summary>
        /// <param name="filePathOrUrl">Input file path</param>
        /// <returns>Array of strings. Text from file.</returns>
        public string[] ReadTextFromFile(string filePathOrUrl)
        {
            this.ShowInfo("Učitavanje riječnika");
            string[] fileText = File.ReadAllLines(filePathOrUrl);

            return fileText;
        }

        #region Helper methods
        /// <summary>
        /// Checks if provided path/url is valid or not.
        /// </summary>
        /// <returns>Image path</returns>
        public string GetValidImagePathorUrl()
        {
            string imagePathorUrl = string.Empty;

            Console.Write("Unesi putanju do slike:");
            imagePathorUrl = Console.ReadLine();

            while ((!File.Exists(imagePathorUrl) && !Uri.IsWellFormedUriString(imagePathorUrl, UriKind.Absolute))
                || (Uri.IsWellFormedUriString(imagePathorUrl, UriKind.Absolute) && !imagePathorUrl.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Putanja nije valjana, provjeri je li dobro upisana.");
                Console.ResetColor();
                Console.Write("Unesi putanju do slike:");
                imagePathorUrl = Console.ReadLine();
            }

            return imagePathorUrl;
        }

        /// <summary>
        /// Checks if provided path/url is valid or not
        /// </summary>
        /// <returns>File path</returns>
        public string GetValidFilePathorUrl()
        {
            string filePathorUrl = string.Empty;

            Console.Write("Unesi putanju do rijecnika:");
            filePathorUrl = Console.ReadLine();

            while ((!File.Exists(filePathorUrl) && !Uri.IsWellFormedUriString(filePathorUrl, UriKind.Absolute))
                || (Uri.IsWellFormedUriString(filePathorUrl, UriKind.Absolute) && !filePathorUrl.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Putanja nije valjana, provjeri je li dobro upisana.");
                Console.ResetColor();
                Console.Write("Unesi putanju do rijecnika:");
                filePathorUrl = Console.ReadLine();
            }

            return filePathorUrl;
        }

        /// <summary>
        /// Creates single string of read text from input
        /// </summary>
        /// <param name="results">Input image text from <see cref="RecognizeText(string, bool, string)"/> function.</param>
        public string ReturnRetrieveText(OcrResults results)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (results != null && results.Regions != null)
            {
                //stringBuilder.Append("Tekst: ");
                //stringBuilder.AppendLine();
                foreach (var item in results.Regions)
                {
                    foreach (var line in item.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            stringBuilder.Append(word.Text);
                            stringBuilder.Append(" ");
                        }

                        stringBuilder.AppendLine();
                    }

                    stringBuilder.AppendLine();
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Spearates all the words in the input string
        /// </summary>
        public string[] SplitWords(string s)
        {
            return Regex.Split(s, @"\W+");
        }

        /// <summary>
        /// Checks if strings from inputArray are in dictionary. Prints dark magenta text if inputArray element is in dictionary, otherwise prints dark red. />
        /// </summary>
        /// <param name="dictionary">Input dictionary</param>
        /// <param name="inputArray">Input array</param>
        public void CheckDictionary(string[] dictionary, string[] inputArray)
        {
            for (int i = 0; i < inputArray.Length; i++)
            {
                if (Array.Exists(dictionary, element => element.Equals(inputArray[i].ToLower())))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(inputArray[i] + ' ');
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(inputArray[i] + ' ');
                    Console.ResetColor();
                }
            }
        }

        /// <summary>
        /// Displays text from input
        /// </summary>
        /// <param name="results">Input image text from <see cref="RecognizeText(string, bool, string)"/> function.</param>
        private void ShowRetrieveText(OcrResults results)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (results != null && results.Regions != null)
            {
                stringBuilder.Append("Tekst: ");
                stringBuilder.AppendLine();
                foreach (var item in results.Regions)
                {
                    foreach (var line in item.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            stringBuilder.Append(word.Text);
                            stringBuilder.Append(" ");
                        }

                        stringBuilder.AppendLine();
                    }

                    stringBuilder.AppendLine();
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(stringBuilder.ToString());
            Console.ResetColor();
        }

        /// <summary>
        /// Helper for displaying error.
        /// </summary>
        /// <param name="errorMessage"></param>
        private void ShowError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
        }

        /// <summary>
        /// Helper for displaying informations.
        /// </summary>
        /// <param name="workStr"></param>
        private void ShowInfo(string workStr)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Format("{0}......", workStr));
            Console.ResetColor();
        }
        #endregion
    }
}