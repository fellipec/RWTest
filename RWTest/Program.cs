using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Diagnostics;
using System.IO;


namespace RWTest
{

    class Program
    {
        
        const int megabyte = 1048576;
        const long gigabyte = 1024 * megabyte;

        static string fname(int f)
        {
            return ("RWTEST" + f.ToString("000") + ".TXT");
        }

        static string LongString()
        {
            string T = "Test";
            for (int i = 0; i < (10*megabyte/T.Length); i++)
            {
                T += T;
            }

            return T;
        }

        static void PrintTimeRemaining (int maxF,int f, long fsz, long milsec)
        {

            double speed = ((fsz / megabyte) / (milsec / 1000));
            Console.Write("                Average speed: {0}MB/s", speed);

            if ((maxF - f) > 1)
            {
                TimeSpan timeremaing = new TimeSpan(0, 0, Convert.ToInt32(((fsz * (maxF - f)) / megabyte) / speed));
                Console.Write(" Time remaning: {0}", timeremaing.ToString("g"));
            }
            else
            {
                Console.Write("                                                ");
            }
        }

        
        static int WriteFiles(int maxF, long fsz = gigabyte)
        {
            string TT = LongString();
            int errors = 0;
            for (int f = 1; f <= maxF; f++)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(fname(f));
                    Stopwatch stpw = new Stopwatch();
                    stpw.Start();
                    long j = fsz / 100 / TT.Length;
                    int k = 0;
                    for (long i = 1; i <= fsz; i += TT.Length)
                    {
                        sw.Write(TT);
                        j--;
                       
                        if (j == 0)
                        {
                            sw.Flush();
                            k++;
                            Console.Write("\rWriting file {0}/{1} {2}%            ", f, maxF, k);
                            j = fsz / 100 / TT.Length;
                        }                                          
                    }
                    sw.Close();
                    stpw.Stop();
                    PrintTimeRemaining(maxF, f, fsz, stpw.ElapsedMilliseconds);

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
            string TT = LongString();
            char[] Buff = new char[TT.Length];
            int errors = 0;

            for (int f = 1; f <= maxF; f++)
            {
                try
                {
                    FileInfo fi = new FileInfo(fname(f));
                    long fsz = fi.Length;
                    long i = 0;
                    
                    StreamReader sw = new StreamReader(fname(f));
                    Stopwatch stpw = new Stopwatch();
                    stpw.Start();
                    long j = fsz / 100 / TT.Length;                    
                    while (!sw.EndOfStream)
                    {
                        sw.ReadBlock(Buff, 0, Buff.Length);
                        
                        j--;
                        for (int z = 0; z < Buff.Length; z++)
                        {
                            if (Buff[z] != TT[z])
                            {
                                errors++;
                            }
                        }


                        if (j == 0)
                        {
                            i++;
                            Console.Write("\rReading file {0}/{1} {2}% {3} errors   ", f, maxF, i, errors);
                            j = fsz / 100 / TT.Length;
                        }
                    
                    }

                    sw.Close();
                    stpw.Stop();
                    PrintTimeRemaining(maxF, f, fsz, stpw.ElapsedMilliseconds);


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
            int maxF = 1;
            long sizeF = gigabyte;
            int werrors = 0;
            int rerrors = 0;
            bool ro = false;
            bool wo = false;


            //Parse command line arguments

            //rwtest.exe [-?|-h|--help] [-p|path] <path>

            var app = new CommandLineApplication();
            app.Name = "RWTest";
            app.Description = "Read/Write test";
            app.FullName = "Read/Write test";
            var pathOption = app.Option("-p|--path <path>", "Path to read/write test files.",CommandOptionType.SingleValue);
            var filesOption = app.Option("-f|--files <files>","Number of files to read/write. Default 1.", CommandOptionType.SingleValue);
            var sizeOption = app.Option("-s|--size <size>", "Size of test file (MB). Default 1024.", CommandOptionType.SingleValue);
            var roOption = app.Option("-r|--read","Only read test files. Remember to create files with -w",CommandOptionType.NoValue);
            var woOption = app.Option("-w|--write", "Only write test files.", CommandOptionType.NoValue);



            app.HelpOption("-?|-h|--help");

            
            // Main function
            app.OnExecute(() =>
            {

                //Initialize
                Stopwatch runningtime = new Stopwatch();
                runningtime.Start();
                Console.WriteLine("Read/Write Test");


                // Set path
                if (pathOption.HasValue())
                {
                    try
                    {
                        Directory.SetCurrentDirectory(pathOption.Value());
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        Console.WriteLine("Path {0} not found.", pathOption.Value());
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("Cannot open path {0}. IO Error {1}", pathOption.Value(), e);
                    }

                }

                //Set number of files
                if (filesOption.HasValue())
                {
                    try
                    {
                        maxF = Convert.ToInt32(filesOption.Value());
                        if (maxF > 999) maxF = 999;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid argument {0}", filesOption.Value());
                        return (1);
                    }
                }


                //Set file size
                if (sizeOption.HasValue())
                {
                    try
                    {
                        sizeF = Convert.ToInt32(sizeOption.Value()) * megabyte;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid argument {0}", sizeOption.Value());
                        return (1);
                    }
                }


                //Set options for read or write only
                ro = roOption.HasValue();
                wo = woOption.HasValue();
                


                // Write test files
                if (ro)
                {
                    Console.WriteLine("Read Only mode. Skipping file creation...");
                }
                else
                {
                    werrors = WriteFiles(maxF, sizeF);
                }

                Console.Write("\n\r");

                // Read test files
                if (wo)
                {
                    Console.WriteLine("Skipping reading test files.");
                }
                else
                {
                    rerrors = ReadFiles(maxF);
                }


                //Say goodbye ;)
                runningtime.Stop();
                TimeSpan totaltime = new TimeSpan(runningtime.ElapsedTicks);
                Console.Write("\n\r");
                Console.WriteLine("Done. {0} write errors, {1} read errors. Total time elapsed: {2}", werrors, rerrors, totaltime.ToString("g"));

                return(0);

            });


            
            app.Execute(args);
            
            
        }
    }
}
