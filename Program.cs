using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DuplicatedFileFinder
{
    class Program
    {
        static int directoryCount;
        static String[] directories;
        static List<FileInfo> files = new List<FileInfo>();
        static List<FileInfo> fileDump = new List<FileInfo>();
        static List<FileInfo> filesToDelete = new List<FileInfo>();
        static bool showInfo = true;
        static bool recursive = false;
        static long countSize =0;
        

        static void Main(string[] args)
        {
            Console.WriteLine("********\nThis application will print a list of files that have the same name or size, it does no actual modifications.\n********");
            Console.WriteLine("How many folders would you like to compare files between?");
            try { directoryCount = int.Parse(Console.ReadLine()); }
            catch(Exception e)
            {
                Console.WriteLine("Please enter a valid integer between 0 and 10 (noninclusive).");
                directoryCount = int.Parse(Console.ReadLine());
            }
            directories = new String[directoryCount];
            for(int i = 0; i<directoryCount; i++)
            {
                Console.WriteLine("Enter the name of the " + (i + 1) +"/"+directoryCount+" directory.");
                directories[i] = Console.ReadLine();
            }
            Console.Clear();
            Console.WriteLine("You have entered the following directories...");
            foreach(String a in directories){
                Console.WriteLine(a);
            }
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Is this a recursive directory search? This may increase search/compare time considerably. (y/n)");
            try { if (Console.ReadLine().ToLower().Equals("y"))
                {
                    recursive = true;
                    Console.WriteLine("Recrusive mode enabled.");
                    Console.WriteLine("");
                }
                else
                {
                    recursive = false;
                }
                }
            catch (Exception e)
            {
                Console.WriteLine("please respond with (y/n).");
                
            }
            

            Console.WriteLine("Press Enter to start your search...");
            Console.ReadLine();
            Console.Clear();
            Console.WriteLine("Searching...this may take a while depending on the contents of the folders...");

            for (int i = 0; i < directories.Length; i++)
            {
                try { Directory.SetCurrentDirectory(directories[i]); }
                catch (Exception)
                {
                   Console.WriteLine("Can't find "+ directories[i]+ " folder. Try checking folder path.");
              
                    return;
                }
                try
                {
                    if (!recursive) { 
                        foreach (string file in Directory.GetFiles(Environment.CurrentDirectory))
                         {

                        Console.WriteLine("Adding: " + file);
                        files.Add(new FileInfo(file));
                        }
                    }
                 else
                    {
                        foreach (string file in Directory.GetFiles(Environment.CurrentDirectory))
                        {

                            Console.WriteLine("Adding: " + file);
                            files.Add(new FileInfo(file));
                        }
                        foreach( string directory in Directory.GetDirectories(Environment.CurrentDirectory))
                        {

                            try
                            {
                                Directory.SetCurrentDirectory(directory);

                                foreach (string file in Directory.GetFiles(Environment.CurrentDirectory))
                                {

                                    Console.WriteLine("Adding: " + file);
                                    files.Add(new FileInfo(file));
                                }
                                foreach (string directory2 in Directory.GetDirectories(Environment.CurrentDirectory))
                                {

                                    try
                                    {
                                        Directory.SetCurrentDirectory(directory2);
                                        foreach (string file in Directory.GetFiles(Environment.CurrentDirectory))
                                        {

                                            Console.WriteLine("Adding: " + file);
                                            files.Add(new FileInfo(file));
                                        }
                                        foreach (string directory3 in Directory.GetDirectories(Environment.CurrentDirectory))
                                        {
                                            try
                                            {
                                                Directory.SetCurrentDirectory(directory3);
                                                foreach (string file in Directory.GetFiles(Environment.CurrentDirectory))
                                                {

                                                    Console.WriteLine("Adding: " + file);
                                                    files.Add(new FileInfo(file));
                                                }

                                            }
                                            catch (Exception e) { }
                                        }
                                     }catch(Exception e) { }

                                }
                            }
                            catch(Exception e) { }


                        }

                    }
                }

                catch (Exception)
                {
                    return;

                }

            }


            countSize = files.Count; //just used for user information
            while (files.Count > 1)
            {
                Boolean needsToRemove = true;

                String name = files[0].Name;
                long size = files[0].Length;
             
              
                Console.WriteLine("Finding duplicates of: " + files[0].Name);
                for(int i = 1; i<files.Count; i++)
                {
                    long sizeDif = Math.Abs(files[i].Length - size);
                    if (files[i].Name == name || sizeDif <= 10) 
                    {
                        if(files[i].FullName == files[0].FullName)
                        {
                            continue;
                        }
                        fileDump.Add(files[i]);
                        files.Remove(files[i]);
                        fileDump.Add(files[0]);
                        files.Remove(files[0]);
                        needsToRemove = false;
                        break;
                    }
                }
                if (files.Count > 0 && needsToRemove)
                {
                    files.Remove(files[0]);
                }

            }
            if(fileDump.Count == 0)
            {
                Console.WriteLine("No duplicates found by name or size!");

            }
            else
            {
                Console.WriteLine((fileDump.Count / 2) + " files found!\n************************");
                for(int i = 0; i<fileDump.Count; i++)
                {
                    Console.WriteLine(fileDump[i].FullName);
                    if (i % 2 == 1)
                    {
                        Console.WriteLine("");
                    }
                }
                Console.WriteLine("");
                Console.WriteLine((fileDump.Count / 2) + " files found! from " + countSize+ " files iterated\n************************");
                Console.WriteLine("");
                Console.WriteLine("Would you like to iterate through the duplicates to delete some files? (y/n)");
                string response = Console.ReadLine();
                if(response.ToLower() == "n")
                {
                  
                }
               else if(response.ToLower() == "y")
                {
                    Console.Clear();
                   
                    for(int i = 0; i<fileDump.Count; i++)
                    {
                        
                        if (showInfo)
                        {
                            Console.WriteLine(fileDump[i].FullName + " - " + fileDump[i].Length/1000);
                        }
                       
                        if(i%2 == 1)
                        {
                            if (showInfo)
                            {
                                Console.WriteLine("\nWould you like to delete one of these? (y/n)\nYou can also press (a/b) to open the files.");
                            }
                                response = Console.ReadLine();
                            if(response.ToLower() == "a")
                            {
                                showInfo = false;
                          new Process { StartInfo = new ProcessStartInfo(fileDump[i-1].FullName) 
                             { UseShellExecute = true } }.Start();
                          
                                 
                                i = i - 2;
                                
                            }
                            else if (response.ToLower() == "b")
                            {
                                showInfo = false;
                                new Process
                                {
                                    StartInfo = new ProcessStartInfo(fileDump[i].FullName)
                                    { UseShellExecute = true }
                                }.Start();
                                i = i - 2;
                            }
                            else if (response.ToLower() == "n")
                            {
                                Console.Clear();
                                showInfo = true;
                            }
                            else if (response.ToLower() == "y")
                            {
                                
                                showInfo = true;
                                Console.Write("\nWhich would you like to delete? (a/b)");
                                response = Console.ReadLine();
                                if (response.ToLower() == "a")
                                {
                                    filesToDelete.Add(fileDump[i - 1]);
                                }
                                else if (response.ToLower() == "b")
                                {
                                    filesToDelete.Add(fileDump[i]);

                                }
                                Console.Clear();
                            }
                        }
                    }
                    Console.WriteLine("This are the files you have indicated you would like to delete:\n");
                    foreach(FileInfo file in filesToDelete)
                    {
                        Console.WriteLine(file.FullName);
                    }
                    
                    Console.WriteLine("Delete these files? (y/n)\n\n");
                    response = Console.ReadLine();
                    if (response.ToLower() == "n")
                    {

                    }
                    else if (response.ToLower() == "y")
                    {
                        foreach(FileInfo file in filesToDelete)
                        {
                            File.Delete(file.FullName);
                        }
                    }
                }
            }
            

            Console.WriteLine("Thank you for using this tool! Press Enter to close this prompt!");
            Console.ReadLine();
        }
    }
}
