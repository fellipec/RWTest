using System;
using System.Diagnostics;
using System.IO;
using CommandLine;
using CommandLine.Text;

namespace RWTest
{


    class Options
    {
        [Option('p', "path", Required = false,
          HelpText = "Path to write/read files.")]
        public string Path { get; set; }

        [Option('s', "size", Required = false, DefaultValue = 1024,
            HelpText = "Size of test files in megabytes. Default 1024")]
        public int Size { get; set; }

        [Option('f', "files", Required = false, DefaultValue = 1,
            HelpText = "Number of files to read/write. Default 1")]
        public int Files { get; set; }

        [Option('r',"read", Required = false, DefaultValue = false,
            HelpText = "Just read test files. You must create the test files before by running -w option")]
        public bool ReadOnly { get; set; }

        [Option('w', "write", Required = false, DefaultValue = false,
            HelpText = "Only create teste files.")]
        public bool WriteOnly { get; set; }



        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }








    class Program
    {
        const string TT = "Test";
        const int megabyte = 1048576;
        const long gigabyte = 1024 * megabyte;

        static string fname(int f)
        {
            return ("RWTEST" + f.ToString("000") + ".TXT");
        }


        static int WriteFiles(int maxF, long fsz = gigabyte)
        {
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
                            Console.Write("\rWriting file {0}/{1} {2}%     ", f, maxF, k);
                            j = fsz / 100 / TT.Length;
                        }                                          
                    }
                    sw.Close();
                    stpw.Stop();
                    Console.Write("                Mean write speed: {0}MB/s", ((fsz / megabyte) / (stpw.ElapsedMilliseconds / 1000)));

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
                    FileInfo fi = new FileInfo(fname(f));
                    long fsz = fi.Length;
                    long i = 0;
                    
                    StreamReader sw = new StreamReader(fname(f));
                    Stopwatch stpw = new Stopwatch();
                    stpw.Start();
                    long j = fsz / 100 / TT.Length;                    
                    while (!sw.EndOfStream)
                    {
                        sw.ReadBlock(Buff, 0, 4);
                        
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
                            Console.Write("\rReading file {0}/{1} {2}% {3} errors          ", f, maxF, i, errors);
                            j = fsz / 100 / TT.Length;
                        }
                    
                    }

                    sw.Close();
                    stpw.Stop();
                    Console.Write("  Mean read speed:  {0}MB/s", ((fsz / megabyte) / (stpw.ElapsedMilliseconds / 1000))); 


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


            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                
                if (options.Path != null)
                {
                    try
                    {
                        Directory.SetCurrentDirectory(options.Path);
                    }
                    catch (DirectoryNotFoundException e)
                    {
                        Console.WriteLine("Path {0} not found.", options.Path);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("Cannot open path {0}. IO Error {1}", options.Path, e);
                    }
                }


                maxF = options.Files;
                sizeF = options.Size * megabyte;
                ro = options.ReadOnly;
                wo = options.WriteOnly;
            }



            Console.WriteLine("Read/Write Test");


            // Write the test files

            if (ro)
            {
                Console.WriteLine("Read Only mode. Skipping file creation...");
            }
            else
            {                
                werrors = WriteFiles(maxF,sizeF);
            }

            Console.Write("\n\r");

            if (wo)
            {
                Console.WriteLine("Skipping reading test files.");
            }
            else
            {
                rerrors = ReadFiles(maxF);
            }
                                    
            
            Console.Write("\n\r");


            Console.WriteLine("Done. {0} write errors, {1} read errors.", werrors, rerrors);

            Console.Read();

        }
    }
}
