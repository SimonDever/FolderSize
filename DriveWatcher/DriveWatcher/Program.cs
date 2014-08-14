using System.Linq;
using FileTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;

public class DriveWatcher
{

    public static void Main()
    {
        Run();
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public static void Run()
    {
        //  FileWatcher();
        string[] args = System.Environment.GetCommandLineArgs();

        // If a directory is not specified, exit program. 
        if (args.Length != 2)
        {
            // Display the proper way to call the program.
            Console.WriteLine("Usage: Watcher.exe (directory)");
            return;
        }
        var path = args[1];

        GetDirectoryTree(path);
    }

    private static void GetDirectoryTree(string path)
    {
        var cur_dir = path;
        string[] subdirs;
        var bfsQ = new Queue<NNode<string>>();
        var root = new NNode<string>(path);
        var cur_node = root;
        bfsQ.Enqueue(root);
        while (bfsQ.Count > 0)
        {
            cur_node = bfsQ.Dequeue();
            try
            {
                subdirs = Directory.GetDirectories(cur_node.Value);
                foreach (var sd in subdirs)
                {
                    var child = new NNode<string>(sd);
                    bfsQ.Enqueue(child);
                    cur_node.Children.Add(child);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
        var dirTree = new NTree<string>(root);

    }

    private static void ShowError(Exception ex)
    {
        Console.Error.WriteLine(ex.Message);
        if (System.Diagnostics.Debugger.IsAttached)
            Console.WriteLine(ex.StackTrace);
    }

    private static void FileWatcher()
    {
        string[] args = System.Environment.GetCommandLineArgs();

        // If a directory is not specified, exit program. 
        if (args.Length != 2)
        {
            // Display the proper way to call the program.
            Console.WriteLine("Usage: Watcher.exe (directory)");
            return;
        }

        // Create a new FileSystemWatcher and set its properties.
        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = args[1];
        /* Watch for changes in LastAccess and LastWrite times, and
           the renaming of files or directories. */
        watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        // Only watch text files.

        watcher.IncludeSubdirectories = true;
        // Add event handlers.
        watcher.Changed += new FileSystemEventHandler(OnChanged);
        watcher.Created += new FileSystemEventHandler(OnChanged);
        watcher.Deleted += new FileSystemEventHandler(OnChanged);
        watcher.Renamed += new RenamedEventHandler(OnRenamed);

        // Begin watching.
        watcher.EnableRaisingEvents = true;

        // Wait for the user to quit the program.
        Console.WriteLine("Press \'q\' to quit the sample.");
        while (Console.Read() != 'q') ;
    }

    // Define the event handlers. 
    private static void OnChanged(object source, FileSystemEventArgs e)
    {
        // Specify what is done when a file is changed, created, or deleted.
        Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
    }

    private static void OnRenamed(object source, RenamedEventArgs e)
    {
        // Specify what is done when a file is renamed.
        Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
    }
}