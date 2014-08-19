using System.Linq;
using FileTree;
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

public class DriveWatcher
{


    static int errorCounter = 0;


    public static void Main()
    {
        #region Get args
        string[] args = System.Environment.GetCommandLineArgs();

        // If a directory is not specified, exit program. 
        if (args.Length != 2)
        {
            // Display the proper way to call the program.
            Console.WriteLine("Usage: DriveWatcher.exe (directory)");
            return ;
        }
        var path = args[1];
        #endregion

        var t = new System.Timers.Timer(5000);
        var fileTree = InitializeTrees(path);
        ulong rootSize = 0;
        t.Elapsed += (o, e) =>
        {
            fileTree.Refresh();
            if (rootSize != fileTree.Root.Value.Size)
            {
                rootSize = fileTree.Root.Value.Size;
                Console.WriteLine("Folder {0} size is {1}", path, FastFileOperations.HumanReadableSize(rootSize));
            }
        };

        t.Start();
        while (Console.Read() != 'q') ;
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public static NTree<PathInfo> InitializeTrees(string path)
    {
        ulong fileCounter, dirCounter;
        Stopwatch sw = new Stopwatch();
        sw.Restart();
        var fileTree = FastFileOperations.GetDirectoryTree(path, out fileCounter, out dirCounter);
        sw.Stop();

        Console.WriteLine("\r\n>> It took {0} seconds to add {1} files and folders to the tree. {2} operations could not be completed.",
                                sw.Elapsed.TotalSeconds, dirCounter, errorCounter);
        sw.Restart();
        fileTree.Refresh();
        sw.Stop();
        // 
        Console.WriteLine("\r\n>> It took {0} seconds to run DFS.\r\nTotal Size = {1}\r\nPress any key to start watching the directory...",
            sw.Elapsed.TotalSeconds, FastFileOperations.HumanReadableSize((ulong)fileTree.Root.Value.Size));

        Console.ReadLine();

        sw.Restart();
        fileTree.DFS(o =>
        {
            if (!o.Value.IsFile)
                WatchFile(o.Value.Path, o, fileTree);
            return null;
        },
        VisitingOrder.Post);
        sw.Stop();

        Console.WriteLine("Watch tree built in {0} seconds.", sw.Elapsed.TotalSeconds);
        return fileTree;
    }

    private static void ShowError(Exception ex, bool supress = true)
    {
        errorCounter++;
        if (!supress)
            Console.Error.WriteLine(ex.Message);
        if (System.Diagnostics.Debugger.IsAttached)
            Console.WriteLine(ex.StackTrace);
    }

    private static void WatchFile(string path, NNode<PathInfo> node, NTree<PathInfo> tree)
    {
        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = path;

        /* Watch for changes in LastAccess and LastWrite times, and
           the renaming of files or directories. */
        watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
           | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size | NotifyFilters.CreationTime;
        // Watch all files.
        watcher.Filter = "";
        watcher.IncludeSubdirectories = false;
        // Add event handlers.
        watcher.Changed += (o, e) =>
        {
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        };
        ///TODO: Invalidate the size with a tag
        watcher.Created += (o, e) =>
        {
            node.AddChild(new NNode<PathInfo> { Value = new PathInfo { Path = e.FullPath, IsFile = FastFileOperations.IsFile(e.FullPath), Size = 0 } });
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        };
        watcher.Deleted += (o, e) =>
        {
            var effectedNode = node.Children.FirstOrDefault(p => p.Value.Path == e.FullPath);
            if (effectedNode == null)
                Console.Error.WriteLine("Unexpected error: Cannot find {0} in file tree.", e.FullPath);
            else
            {
                Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
                tree.RemoveNode(effectedNode);
            }
        };

        watcher.Renamed += (o, e) =>
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            var effectedNode = node.Children.FirstOrDefault(p => p.Value.Path == e.OldFullPath);
            if (effectedNode == null)
                Console.Error.WriteLine("Unexpected error: Cannot find {0} in file tree.", e.FullPath);
            else
                effectedNode.Value.Path = e.FullPath;
        };

        watcher.Error += watcher_Error;

        // Begin watching.
        watcher.EnableRaisingEvents = true;
    }

    static void watcher_Error(object sender, ErrorEventArgs e)
    {
        ShowError(e.GetException(), false);
    }

}