# RWTest
Read/Write test

Usage: rwtest [options]

Options:
  -p|--path <path>    Path to read/write test files.
  -f|--files <files>  Number of files to read/write. Default 1.
  -s|--size <size>    Size of test file (MB). Default 1024.
  -r|--read           Only read test files. Remember to create files with -w
  -w|--write          Only write test files.
  -?|-h|--help        Show help information

RWTest will create files in the current directory following this pattnern: RWTEST000.TXT where 000 is a sequetial number, 
starting in 1 and going up to the requested number of files with -f option. 
The default file size is 1GB (1024MB). You can alter the file size with -s option.

After creating the test files, RWTest will attemp to read they, and will check if the contents match the Test pattern. 
Any inconsistency will be reported. IO Errors maybe treated by the underlying operational system.

