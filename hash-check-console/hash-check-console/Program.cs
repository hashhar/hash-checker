using System;
using System.IO;
using System.Security.Cryptography;

/* linux-args branch:
- Removed the option to explicitly specify the output file from within the program. Use I/O redirection instead.
*/

/* To-do:
- Add multi-letter argument support
- Add support for using either of '/' or '-' as the argument specifier
- Add support for verifying hashes by specifying an input file containing hash and file pairs
- Decide between using I/O redirection or continuing the native option to dump the output to a file
*/

/* Expected usages:
- hash-check
	Show the proper usage
- hash-check (No input file specified)
	Message telling an input file has to be specified
- hash-check (Incorrect arguments)
	- Any argument not defined in the documentation (multi-letter arguments are planned to be supported so don't use argument length as a criteria)
*/

namespace hash_check_console
{
	class Program
	{
		private static void CalculateHash(string inputFile, bool writeToFile, bool md5, bool sha1, bool sha256)
		{
			// Check if the input file exists
			if (File.Exists(inputFile) == false)
				Console.WriteLine("The input file {0} does not exist", inputFile);
			else
			{
				using (var stream = new BufferedStream(File.OpenRead(inputFile), 120000000))
				// Create instances of CryptoProviders to expose the ComputeHash method
				using (var md5CryptoProvider = new MD5CryptoServiceProvider())
				using (var sha1CryptoProvider = new SHA1CryptoServiceProvider())
				using (var sha256CryptoProvider = new SHA256CryptoServiceProvider())
				{
					// Set all the hash strings to null so that later when we write them, we can know if the user actually requested that hash without looking at the command line arguments
					string md5Hash = null, sha1Hash = null, sha256Hash = null;
					// Calculate only the requested hashes
					if (md5 == true)
						md5Hash = BitConverter.ToString(md5CryptoProvider.ComputeHash(stream));
					if (sha1 == true)
						sha1Hash = BitConverter.ToString(sha1CryptoProvider.ComputeHash(stream));
					if (sha256 == true)
						sha256Hash = BitConverter.ToString(sha256CryptoProvider.ComputeHash(stream));
					// If all three of the arguments were skipped generate all the hashes
					if (md5 == false && sha1 == false && sha256 == false)
					{
						md5Hash = BitConverter.ToString(md5CryptoProvider.ComputeHash(stream));
						sha1Hash = BitConverter.ToString(sha1CryptoProvider.ComputeHash(stream));
						sha256Hash = BitConverter.ToString(sha256CryptoProvider.ComputeHash(stream));
					}
					if (writeToFile == false)
						WriteHashConsole(inputFile, md5Hash, sha1Hash, sha256Hash);
					else
						WriteHashFile(inputFile, md5Hash, sha1Hash, sha256Hash);
				}
			}
		}

		private static void WriteHashFile(string inputFile, string md5Hash, string sha1Hash, string sha256Hash)
		{
			return;
		}

		static void WriteHashConsole(string inputFile, string md5Hash, string sha1Hash, string sha256Hash)
		{
			// Check the hash strings for null values. Null means that the user had not requested that hash and hence the hash not been generated.
			if (md5Hash != null)
			{
				Console.Write("MD5:  ");
				Console.Write(md5Hash);
				Console.Write("  ");
				Console.WriteLine(inputFile);
				Console.WriteLine();
			}
			if (sha1Hash != null)
			{
				Console.Write("SHA1:  ");
				Console.Write(sha1Hash);
				Console.Write("  ");
				Console.WriteLine(inputFile);
				Console.WriteLine();
			}
			if (sha256Hash != null)
			{
				Console.Write("SHA256:  ");
				Console.Write(sha256Hash);
				Console.Write("  ");
				Console.WriteLine(inputFile);
				Console.WriteLine();
			}
		}

		static void Main(string[] args)
		{
			// Values of these variables set to 1 mean that those hash formats have been requested by the user
			bool md5 = false, sha1 = false, sha256 = false, write = false;
			if (args.Length == 0)
			{
				ShowUsage();
				return;
			}
			// Check for all the arguments and set the values of the options as required
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == "/m" || args[i] == "/md5" || args[i] == "-m" || args[i] == "--md5")
					md5 = true;
				else if (args[i] == "/s" || args[i] == "/sha1" || args[i] == "-s" || args[i] == "--sha1")
					sha1 = true;
				else if (args[i] == "/S" || args[i] == "/sha256" || args[i] == "-S" || args[i] == "--sha256")
					sha256 = true;
				else if (args[i] == "/w" || args[i] == "/write-to-file" || args[i] == "-w" || args[i] == "--write-to-file")
					write = true;
				else if (!args[i].StartsWith("/") || !args[i].StartsWith("-") || !args[i].StartsWith("--"))
				{
					CalculateHash(args[i], write, md5, sha1, sha256);
					Console.WriteLine("--------------------------------------------------------------------------------");
				}
			}
			Console.WriteLine("Done! Press any key to terminate.");
			Console.ReadLine();
		}

		private static void ShowUsage()
		{
			//var releaseNotes = File.ReadAllText(@"D:\GitHub\hash-checker\hash-check-console\hash-check-console\ReleaseNotes.md");
			Console.WriteLine("Incorrect usage");
			//Console.Write(releaseNotes);
			Console.WriteLine("hash-checker [/write-to-file] [/md5] [/sha1] [/sha256] file1 [file2] [file3]...");
			Console.WriteLine("hash-checker [/w] [/m] [/s] [/S] file1 [file2] [file3]...");
			Console.WriteLine("hash-checker [-w] [-m] [-s] [-S] file1 [file2] [file3]...");
			Console.WriteLine("hash-checker [--write-to-file] [--md5] [--sha1] [--sha256] file1 [file2] [file3]...");
			Console.WriteLine();
			Console.WriteLine("w or write-to-file - Specifies that the output has to be dumped to a file rather than console");
			Console.WriteLine("m or md5 - Generates the MD5 hash");
			Console.WriteLine("s or sha1 - Generates the SHA1 hash");
			Console.WriteLine("S or sha256 - Generates the SHA256 hash");
			Console.WriteLine("file - Specifies the input files whose hashes have to be calculated(at least one filename must be specified)");
			Console.WriteLine("The arguments can be used in any order and in any combination");
			Console.WriteLine("Press any key to terminate the program and try again...");
			Console.ReadLine();
		}
	}
}

