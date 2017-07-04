﻿using System;
using System.IO;
using System.Linq;

namespace RWTest
{
    class Program
    {
        const string TT = "Test";

        static string fname(int f)
        {
            return ("RWTEST" + f.ToString("000") + ".TXT");
        }


        static int WriteFiles(int maxF)
        {
            int errors = 0;
            for (int f = 1; f <= maxF; f++)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(fname(f));

                    for (int i = 1; i <= 256; i++)
                    {
                        for (int j = 1; j <= 1024; j++)
                        {
                            for (int k = 1; k <= 1024; k++)
                            {
                                sw.Write(TT);
                            }
                        }
                        sw.Flush();
                        Console.Write("\rFile {0}/{1} {2}%     ", f, maxF, (i * 100 / 256));
                    }
                    sw.Close();

                }
                catch (IOException e)
                {
                    Console.WriteLine("Error writing to file" + fname(f));
                    Console.Write(e.ToString());
                    errors++;
                }

            }
            return (errors);
        }


        static int ReadFiles(int maxF)
        {
            char[] Buff = new char[4];
            int errors = 0;

            for (int f = 1; f <= maxF; f++)
            {
                try
                {
                    StreamReader sw = new StreamReader(fname(f));

                    for (int i = 1; i <= 256; i++)
                    {
                        for (int j = 1; j <= 1024; j++)
                        {
                            for (int k = 1; k <= 1024; k++)
                            {
                                sw.ReadBlock(Buff, 0, 4);
                                for (int z = 0; z < Buff.Length; z++)
                                {
                                    /*if (Buff[z] != TT[z])
                                    {
                                        errors++;
                                    }*/
                                    if (!Buff.SequenceEqual(TT))
                                    {
                                        errors++;
                                    }

                                }
                            }
                        }

                        Console.Write("\rReading file {0}/{1} {2}% {3} errors          ", f, maxF, (i * 100 / 256), errors);
                    }
                    sw.Close();
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error reading file" + fname(f));
                    Console.Write(e.ToString());
                    errors++;
                }

            }
            return (errors);


        }





        static void Main(string[] args)
        {
            int maxF = 0;
            int werrors = 0;
            int rerrors = 0;

            if (args.Length > 0) 
            { 
                string fpath = args[0];
            }


            try
            {
                Directory.SetCurrentDirectory(fpath);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("Path {0} not found.", fpath);
            }
            catch (IOException e)
            {
                Console.WriteLine("Cannot open path {0}. IO Error {1}", fpath, e);
            }

            Console.WriteLine("Read/Write Test");


            while (maxF == 0)
            {
                try
                {
                    Console.Write("Number of files?:");
                    maxF = Convert.ToInt16(Console.ReadLine());
                }
                catch (FormatException)
                {
                    maxF = 0;
                    Console.WriteLine("Invalid number. ");
                }
            }


            // Write the test files

            Console.Write("\n\r Skip writing files? [y/N]:", maxF);

            if ("y" == Console.ReadKey().KeyChar.ToString().ToLower())
            {
                Console.Write("\n\r");
            }
            else
            {
                werrors = WriteFiles(maxF);
                Console.Write("\n\r");
            }


            Console.Write("\n\r{0} files written. Do you want to read they? [y/N]:", maxF);

            if ("y" == Console.ReadKey().KeyChar.ToString().ToLower())
            {
                Console.Write("\n\r");
                rerrors = ReadFiles(maxF);
                Console.Write("\n\r");
            }
            else
            {
                Console.Write("\n\r");
            }


            Console.WriteLine("Done. {0} write errors, {1} read errors.", werrors, rerrors);

            Console.Read();

        }
    }
}
