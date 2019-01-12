using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SadRogue
{
    public class NameGenerator
    {
        // empty constructor

        public string GenerateName()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var namesOneSyllable = assembly.GetManifestResourceNames().Single(str => str.EndsWith("one_syllable.txt"));
            var namesTwoSyllable = assembly.GetManifestResourceNames().Single(str => str.EndsWith("two_syllable.txt"));
            var namesThreeSyllable = assembly.GetManifestResourceNames().Single(str => str.EndsWith("three_syllable.txt"));
            var namesMultipleSyllable = assembly.GetManifestResourceNames().Single(str => str.EndsWith("multiple_syllable.txt"));


            var randomName = new StringBuilder();

            try
            {   
                using (var stream = assembly.GetManifestResourceStream(namesOneSyllable))
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            // Read the stream to a string, and write the string to the console.
                            var words = reader.ReadToEnd().Split(',');
                            var randNum = new Random();
                            var nameNum = randNum.Next(0, words.Length);
                            randomName.Append(words[nameNum] + ' ');
                        }
            }
            catch
            {
            }

            try
            {
                using (var stream = assembly.GetManifestResourceStream(namesMultipleSyllable))
                    if (stream != null)
                        using (var reader = new StreamReader(stream))
                        {
                            // Read the stream to a string, and write the string to the console.
                            var words = reader.ReadToEnd().Split(',');
                            var randNum = new Random();
                            var nameNum = randNum.Next(0, words.Length);
                            randomName.Append(words[nameNum]);
                        }
            }
            catch
            {
            }

            return randomName.ToString();
        }
    }
}
