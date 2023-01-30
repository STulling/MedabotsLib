using System;
using System.Collections.Generic;
using System.Linq;
using MedabotsLib.Utils;

namespace MedabotsLib.GameData
{
    /// <summary>
    /// A class that represents a string of text or conversation in the ROM
    /// This message can contain multiple lines and textboxes
    /// It can be encoded between the game's encoding and a human readable format
    /// </summary>
    public class Text : IByteable, ICanGetDirty
    {

        string decoded;
        string decodedCopy;
        byte[] encoded;
        byte[] encodedCopy;
        private bool isDirty = false;

        /// <summary>
        /// A Text object created from a string in the 'human readable' format
        /// </summary>
        /// <param name="text">The string to be encoded</param>
        public Text(string text)
        {
            this.decoded = text;
            this.encoded = Encode(text);
            this.isDirty = false;
            initCopies();
        }

        /// <summary>
        /// A Text object created from a byte array in the game's encoding
        /// </summary>
        /// <param name="data">The byte array to be decoded</param>
        public Text(byte[] data)
        {
            this.encoded = data;
            this.decoded = Decode(data);
            this.isDirty = false;
            initCopies();
        }

        /// <summary>
        /// Here we create some copies to help us track changes
        /// </summary>
        private void initCopies()
        {
            this.decodedCopy = new string(decoded);
            this.encodedCopy = new byte[encoded.Length];
            this.encoded.CopyTo(encodedCopy, 0);
        }

        public string Str { get => decoded; set => Set(value); }
        public byte[] Enc { get => encoded; set => Set(value); }

        public bool IsDirty
        {
            get
            {
                this.isDirty = !(decoded.Equals(decodedCopy) && encoded.SequenceEqual(encodedCopy));
                return this.isDirty;
            }
        }

        /// <summary>
        /// Sets the text to the given byte array
        /// </summary>
        /// <param name="text">The byte array to be encoded in the game's format</param>
        public void Set(byte[] encodedData)
        {
            this.encoded = encodedData;
            this.decoded = Decode(encodedData);
            this.isDirty = true;
        }

        /// <summary>
        /// Sets the text to the given string
        /// </summary>
        /// <param name="text">The string to be encoded in the human readable format</param>
        public void Set(string decodedData)
        {
            this.decoded = decodedData;
            this.encoded = Encode(decodedData);
            this.isDirty = true;
        }

        private static char[] encoding = new char[]
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

        /// <summary>
        /// Encodes a string to the human readable format from the game's encoding
        /// </summary>
        /// <param name="text">The game's encoding</param>
        private static string Decode(byte[] data)
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
                    result += "#";
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

        /// <summary>
        /// Encodes a string to the game's encoding from the human readable format
        /// </summary>
        /// <param name="text">The human readable format</param>
        private static byte[] Encode(string data)
        {
            List<byte> result = new List<byte>();
            for (int i = 0; i < data.Length; i++)
            {
                char chr = data[i];
                if (encoding.Contains(chr))
                {
                    result.Add((byte)Array.IndexOf(encoding, chr));
                }
                else if (chr == '#')
                {
                    result.Add((byte)0xFE);
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

        public override bool Equals(object other)
        {
            if (other.GetType() == typeof(string))
            {
                string otherString = other as string;
                return otherString.Equals(this.Str);
            }
            else
            {
                return base.Equals(other);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public byte[] ToBytes()
        {
            List<byte> bs = encoded.ToList();
            bs.Add(0xFE);
            return bs.ToArray();
        }
    }
}
