using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MedabotsLib
{
    class Encoding
    {
        static char[] encoding = new char[]
        {
            ' ', 'A', 'B', 'C', 'D', 'E', 'F', 'G',
            'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
            'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W',
            'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e',
            'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u',
            'v', 'w', 'x', 'y', 'z', '0', '1', '2',
            '3', '4', '5', '6', '7', '8', '9', '·',
            '.', ',', '\'', '-', '/', ':', '?', '!',
            '"', '(', ')', '♥', '£', '&', '%'
        };

        public static string Decode(byte[] data)
        {
            string result = "";
            for (int i = 0; i < data.Length; i++)
            {
                byte chr = data[i];
                if (chr < 0x4f)
                {
                    result += encoding[data[i]];
                }
                else if (chr == 0xf7)
                {
                    result += "<SPEED:" + data[i + 1] + ">";
                    i++;
                }
                else if (chr == 0xf8)
                {
                    result += "<I>";
                }
                else if (chr == 0xf9)
                {
                    result += "<MEM:" + data[i + 1] + ">";
                    i++;
                }
                else if (chr == 0xfa)
                {
                    result += "<?>";
                }
                else if (chr == 0xfb)
                {
                    result += "<PORTRAIT:" + data[i + 1] + ", " + data[i + 2] + ", " + data[i + 3] + ">";
                    i += 3;
                }
                else if (chr == 0xfc)
                {
                    result += "<NB>";
                }
                else if (chr == 0xfD)
                {
                    result += "<NL>";
                }
                else if (chr == 0xfe)
                {
                    result += "<ENDLST>";
                }
                else if (chr == 0xff)
                {
                    result += "<END:" + data[i + 1] + ">";
                }
            }
            return result;
        }

        private static string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        public static byte[] Encode(string data)
        {
            List<byte> result = new List<byte>();
            for (int i = 0; i < data.Length; i++)
            {
                char chr = data[i];
                if (encoding.Contains(chr))
                {
                    result.Add((byte)Array.IndexOf(encoding, chr));
                }
                else if (chr == '<')
                {
                    string command = "";
                    chr = data[++i];
                    while (chr != '>')
                    {
                        command += chr;
                        chr = data[++i];
                    }
                    if (command.StartsWith("SPEED"))
                    {
                        result.Add(0xf7);
                        result.Add(byte.Parse(GetNumbers(command)));
                    }
                    else if (command.StartsWith("I"))
                    {
                        result.Add(0xf8);
                    }
                    else if (command.StartsWith("MEM"))
                    {
                        result.Add(0xf9);
                        result.Add(byte.Parse(GetNumbers(command)));
                    }
                    else if (command.StartsWith("?"))
                    {
                        result.Add(0xfa);
                    }
                    else if (command.StartsWith("PORTRAIT"))
                    {
                        result.Add(0xfb);
                        string[] args = command.Split(',');
                        foreach (string num in args)
                            result.Add(byte.Parse(GetNumbers(num)));
                    }
                    else if (command.StartsWith("NB"))
                    {
                        result.Add(0xfc);
                    }
                    else if (command.StartsWith("NL"))
                    {
                        result.Add(0xfd);
                    }
                    else if (command.StartsWith("ENDLST"))
                    {
                        result.Add(0xfe);
                    }
                    else if (command.StartsWith("END"))
                    {
                        result.Add(0xff);
                        result.Add(byte.Parse(GetNumbers(command)));
                    }
                }
            }
            return result.ToArray();
        }
    }
}
