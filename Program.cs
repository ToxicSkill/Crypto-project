using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Snail_Krypto_Projekt
{
    class Program
    {
        static void Main(string[] args)
        {
            string path1 = "etest.TXT";
            string path2 = "detest.TXT";

            SnailFile newFile = new SnailFile();
            snail snail = new snail();

            bool exit = default(bool),
                mode = default(bool);
            string text = default(string);
            byte[] newByteArray = default(byte[]);

            while (!exit)
            {
                Console.Clear();

                Console.WriteLine("1 - Encrypt file\n" +
                    "2 - Decrypt file\n" +
                    "3 - Encrypt text\n" +
                    "4 - Decrypt text (be ensure whether provided data has a size of power from natural number eg. 9,16,25 and contains only ASCII)\n" +
                    "5 - Exit");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("Enter file name or path (default test.txt):\n");
                        mode = true;
                        newFile = new SnailFile(Console.ReadLine());
                        snail.Encrypt(newFile,mode);
                        newFile.ByteArrayToFile(path1);
                        Console.Clear();
                        Console.WriteLine(@"Encrypted file saved in: " + path1 + "\n"); 
                        Console.ReadKey();
                        break;
                    case "2":
                        Console.Clear();
                        Console.WriteLine("Enter file name or path:\n");
                        mode = true;
                        newFile = new SnailFile(Console.ReadLine());
                        snail.Decrypt(newFile,mode);
                        newFile.ByteArrayToFile(path2);
                        Console.Clear();
                        Console.WriteLine(@"Decrypted file saved in: " + path2 + "\n"); 
                        Console.ReadKey();
                        break;
                    case "3":
                        Console.Clear();
                        Console.WriteLine("Enter text to encrypt:\n");
                        mode = false;
                        text = Console.ReadLine();
                        newByteArray = Encoding.ASCII.GetBytes(text);
                        newFile.Data = newByteArray;
                        newFile.size = (uint)newFile.Data.Length;
                        snail.Encrypt(newFile,mode);
                        Console.Clear();
                        var str = System.Text.Encoding.Default.GetString(newFile.Data);
                        Console.WriteLine("Result:\n");
                        Console.WriteLine(str);

                        Console.ReadKey();
                        break;
                    case "4":
                        Console.Clear();
                        Console.WriteLine("Enter text to decrypt:\n");
                        mode = false;
                        text = Console.ReadLine();
                        newByteArray = Encoding.ASCII.GetBytes(text);
                        newFile.Data = newByteArray;
                        newFile.size = (uint)newFile.Data.Length;
                        snail.Decrypt(newFile,mode);
                        Console.Clear();
                        var str2 = System.Text.Encoding.Default.GetString(newFile.Data);
                        Console.WriteLine("Result:\n");
                        Console.WriteLine(str2);

                        Console.ReadKey();
                        break;
                    case "5":
                        exit = true;
                        break;
                    default:
                        break;
                }
            }
            Console.Clear();
            Console.WriteLine("Bye Bye!"); Console.ReadKey();
        }
        public class SnailFile
        {
            public byte[] Data { get; set; }
            public uint size = 0;
            public SnailFile(string path)
            {
                Data = FileToByteArray(path);
                size = (uint)Data.Length;
            }
            public SnailFile()
            {
                size = default(uint);
                Data = default(byte[]);
            }

            public byte[] FileToByteArray(string fileName)
            {
                return File.ReadAllBytes(fileName);
            }
            public bool ByteArrayToFile(string fileName)
            {
                File.WriteAllBytes(fileName, Data);
                return true;
            }
        }
        public class snail
        {
            public byte[] AddByteToEndArray(byte[] bArray, byte newByte)
            {
                byte[] newArray = new byte[bArray.Length + 1];
                bArray.CopyTo(newArray, 0);
                newArray[newArray.Length - 1] = newByte;
                return newArray;
            }
            public byte[] AddByteToBegArray(byte[] bArray, byte newByte)
            {
                byte[] newArray = new byte[bArray.Length + 1];
                bArray.CopyTo(newArray, 1);
                newArray[0] = newByte;
                return newArray;
            }

            public byte[] RemoveByteFromBegArray(byte[] bArray, int length)
            {
                byte[] newArray = new byte[bArray.Length - length];
                Array.Copy(bArray, length, newArray, 0, newArray.Length);
                return newArray;
            }
            public byte[] RemoveByteFromEndArray(byte[] bArray, int length)
            {
                byte[] newArray = new byte[bArray.Length - length];
                Array.Copy(bArray, newArray, bArray.Length - length);
                return newArray;
            }
            public bool Encrypt(SnailFile file, bool mode)
            {
                if (file.Data.Length <= 5)
                    return false;

                SnailEncryptrion(file,mode);

                return true;
            }
            public bool Decrypt(SnailFile file, bool mode)
            {
                if (file.Data.Length <= 5)
                    return false;

                SnailDecryptrion(file,mode);

                return true;
            }

            public List<char> encrypted_snail = new List<char>();
            public List<char> temp = new List<char>();
            public List<List<char>> encrypted_columns = new List<List<char>>();

            public int GetOversize(SnailFile file)
            {
                string oversize = default(string);
                int i = 0;
                while (file.Data[i] != '\n')
                {
                    oversize = oversize + (char)file.Data[i];
                    ++i;
                }

                if (file.Data[i + 2] == 13)
                    ++i;

                file.Data = RemoveByteFromBegArray(file.Data, i + 1);

                Int32.TryParse(oversize, out i);

                return i;
            }
            private bool SnailEncryptrion(SnailFile file,bool mode)
            {
                if (file.size < 5)
                    return false;

                Clear();

                var size = file.size;

                uint a = 0,
                    overflow = 0;
                byte oversize = 0;

                long sqrt = 0,
                    s = 0,
                    limit = 0;

                do
                {
                    sqrt = (long)Math.Round(Math.Pow(size, 0.5));
                    a = (uint)Math.Pow(sqrt, 2);
                    size++;
                } while (a < file.size);

                s = sqrt;
                overflow = a - file.size;
                oversize = (byte)(overflow);

                char[,] Matrix = new char[sqrt, sqrt];

                for (var i = 0; i < overflow; ++i)
                    file.Data = AddByteToEndArray(file.Data, oversize);

                var it = 0;
                for (var i = 0; i < sqrt; ++i)
                {
                    for (var j = 0; j < sqrt; ++j)
                    {
                        Matrix[i, j] = (char)file.Data[it];
                        ++it;
                    }
                }

                if ((sqrt % 2) == 1)
                    limit = sqrt - 1;
                else
                    limit = sqrt - 2;

                for (var cntr = 0; cntr < limit; ++cntr)
                {
                    for (var i = cntr; i < sqrt - cntr; ++i)
                        encrypted_snail.Add(Matrix[cntr, i]);

                    for (var i = cntr + 1; i < sqrt - cntr - 1; ++i)
                        encrypted_snail.Add(Matrix[i, s - cntr - 1]);

                    for (var i = sqrt - 1 - cntr; i >= cntr; --i)
                        encrypted_snail.Add(Matrix[s - cntr - 1, i]);

                    for (var i = sqrt - 1 - cntr; i > cntr + 1; --i)
                        encrypted_snail.Add(Matrix[i - 1, cntr]);
                }

                if ((sqrt % 2) == 1)
                    encrypted_snail.Remove((char)(encrypted_snail.Count - 1));

                for (var i = 0; i < encrypted_snail.Count; ++i)
                {
                    temp.Add(encrypted_snail[i]);
                    encrypted_snail[i] = default(char);
                    if (temp.Count == sqrt)
                    {
                        encrypted_columns.Add(new List<char>(temp));
                        temp.Clear();
                    }
                }
                if (encrypted_columns.Count > 0)
                {
                    byte[] newData = new byte[sqrt * sqrt];

                    Array.Clear(file.Data, 0, file.Data.Length);
                    file.Data.Append(oversize);
                    file.Data.Append((byte)('\n'));

                    it = 0;
                    for (var i = 0; i < sqrt; ++i)
                        for (var j = 0; j < sqrt; ++j)
                        {
                            newData[it] = (byte)encrypted_columns[j][i];
                            ++it;
                        }

                    file.Data = newData;

                    encrypted_columns.Clear();
                }

                Matrix = default(char[,]);

                if (mode)
                {
                    file.Data = AddByteToBegArray(file.Data, (byte)(10));
                    file.Data = AddByteToBegArray(file.Data, (byte)(oversize + 48));
                }
                Clear();
                return true;
            }
            private bool SnailDecryptrion(SnailFile file, bool mode)
            {
                if (file.size < 5)
                    return false;

                Clear();

                int overflow = 0;

                if (mode)
                    overflow = GetOversize(file);

                var size = file.Data.Length;

                int iterator = 0;

                long sqrt = (long)Math.Pow(size, 0.5),
                    s = sqrt,
                    limit = sqrt;

                char[,] Matrix = new char[sqrt, sqrt];

                for (var i = 0; i < size; ++i)
                {
                    temp.Add((char)file.Data[i]);
                    file.Data[i] = default(byte);
                    if (temp.Count == sqrt)
                    {
                        encrypted_columns.Add(new List<char>(temp));
                        temp.Clear();
                    }
                }

                for (var i = 0; i < sqrt; ++i)
                    for (var j = 0; j < sqrt; ++j)
                        encrypted_snail.Add(encrypted_columns[j][i]);

                encrypted_columns.Clear();

                if ((sqrt % 2) == 1)
                    limit = (sqrt - 1) / 2;
                else
                    limit = sqrt / 2;

                for (var cntr = 0; cntr < limit; ++cntr)
                {
                    for (var i = cntr; i < sqrt - cntr; ++i)
                    {
                        Matrix[cntr, i] = encrypted_snail[iterator];
                        ++iterator;
                    }

                    for (var i = cntr + 1; i < sqrt - cntr - 1; ++i)
                    {
                        Matrix[i, s - cntr - 1] = encrypted_snail[iterator];
                        ++iterator;
                    }

                    for (var i = sqrt - 1 - cntr; i >= cntr; --i)
                    {
                        Matrix[s - cntr - 1, i] = encrypted_snail[iterator];
                        ++iterator;
                    }

                    for (var i = sqrt - 1 - cntr; i > cntr + 1; --i)
                    {
                        Matrix[i - 1, cntr] = encrypted_snail[iterator];
                        ++iterator;
                    }
                }
                if ((sqrt % 2) == 1)
                    Matrix[(sqrt - 1) / 2, (sqrt - 1) / 2] = encrypted_snail[iterator];

                encrypted_snail.Clear();

                s = 0;

                for (var i = 0; i < sqrt; ++i)
                    for (var j = 0; j < sqrt; ++j)
                    {
                        file.Data[s] = (byte)Matrix[i, j];
                        ++s;
                    }

                Matrix = default(char[,]);

                file.Data = RemoveByteFromEndArray(file.Data, overflow);

                Clear();
                return true;
            }
            public void Clear()
            {
                encrypted_snail.Clear();
                encrypted_columns.Clear();
                temp.Clear();
            }
        }
    }
}
