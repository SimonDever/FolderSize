﻿using System.Linq;
using FileTree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Runtime.InteropServices;

public class DriveWatcher
{
    

    static int errorCounter = 0;

    public static void Main()
    {
        Run();
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public static void Run()
    {
        //  FileWatcher();
        errorCounter = 0;
        var timerCounter = DateTime.Now;
        string[] args = System.Environment.GetCommandLineArgs();

        // If a directory is not specified, exit program. 
        if (args.Length != 2)
        {
            // Display the proper way to call the program.
            Console.WriteLine("Usage: DriveWatcher.exe (directory)");
            return;
        }
        var path = args[1];

        ulong fileCounter, dirCounter;
        var fileTree = GetDirectoryTree(path, out fileCounter, out dirCounter);


        fileTree.DFS(o =>
        {
            if (o.Parent != null)
                o.Parent.Value.Size += o.Value.Size;
            return null;
        },
        VisitingOrder.Post);
        Console.WriteLine(">>>> Total Size = {0}", HumanReadableSize((ulong)fileTree.Root.Value.Size));
        var runningTime = DateTime.Now - timerCounter;
        Console.WriteLine("\r\n>> It took {3} seconds to add {0} files and {1} folders to the tree. {2} operations could not be completed.", fileCounter, dirCounter, errorCounter, runningTime.Duration().TotalSeconds);
    }

    private static string HumanReadableSize(ulong p)
    {
        ulong b = p % (1 << 10);
        ulong kb = (p >> 10) % (1 << 10);
        ulong mb = (p >> 20) % (1 << 10);
        ulong gb = (p >> 30) % (1 << 10);
        return String.Format("{0} GB and {1} MB and {2} KB and {3} Bytes", gb, mb, kb, b);
    }
    private static NTree<PathInfo> GetDirectoryTree(string path)
    {
        ulong a, b;
        return GetDirectoryTree(path, out a, out b);
    }
    private static NTree<PathInfo> GetDirectoryTree(string path, out ulong FileCount, out ulong DirectoryCount)
    {
        var cur_dir = path;
        var bfsQ = new Queue<NNode<PathInfo>>();
        var root = new NNode<PathInfo>(new PathInfo { Path = path });
        var cur_node = root;
        bfsQ.Enqueue(root);
        ulong dirCounter = 1, fileCounter = 0;
        while (bfsQ.Count > 0)
        {
            cur_node = bfsQ.Dequeue();
            try
            {
                var subdirs = FastFileOperations.GetDirectoriesAndFiles(cur_node.Value.Path);
                foreach (var sd in subdirs)
                {
                    var child = new NNode<PathInfo>(sd);
                    bfsQ.Enqueue(child);
                    cur_node.Children.Add(child);
                    dirCounter++;

                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
        var dirTree = new NTree<PathInfo>(root);
        FileCount = fileCounter;
        DirectoryCount = dirCounter;
        return dirTree;

    }

    private static void ShowError(Exception ex, bool supress = true)
    {
        errorCounter++;
        if (!supress)
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