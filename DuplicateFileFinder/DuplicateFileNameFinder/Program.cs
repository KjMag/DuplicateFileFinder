// See https://aka.ms/new-console-template for more information
namespace DuplicateFileFinder
{
    public class DuplicateFileNameFinder
    {
        string rootPath;
        public Dictionary<string, SortedSet<string>> FileOccuranceDictionary  { get; private set; }
        public Dictionary<string, SortedSet<string>> DuplicateFilesDictionary { get; private set; }
        public string RootPath
        {
            get { return rootPath; }
            private set
            {
                rootPath = value;
                RefreshFileDictionaries();
            }
        }

        public DuplicateFileNameFinder(string rootpath)
        {
            RootPath = rootpath;
        }

        public void GenerateFileDictionaries()
        {
            GenerateFileOccuranceDictionary();
            GenerateDuplicateFilesDictionary();
        }

        // convenience method useful when you want to check whether directory contents has changed.
        // In the future it may be more sophisticated ;)
        public void RefreshFileDictionaries()
        {
            GenerateFileDictionaries();
        }

        private void GenerateDuplicateFilesDictionary()
        {
            var findDuplicates = from file in FileOccuranceDictionary where file.Value.Count > 1 select file;
            DuplicateFilesDictionary = new Dictionary<string, SortedSet<string>>(findDuplicates);
        }

        private Dictionary<string, SortedSet<string>> GenerateFileOccuranceDictionary()
        {
            FileOccuranceDictionary = new Dictionary<string, SortedSet<string>>();

            // Data structure to hold names of subfolders to be
            // examined for files.
            Stack<string> dirs = new Stack<string>(20);

            if (!System.IO.Directory.Exists(RootPath))
            {
                throw new ArgumentException();
            }
            dirs.Push(RootPath);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs;
                try
                {
                    subDirs = System.IO.Directory.GetDirectories(currentDir);
                }
                // An UnauthorizedAccessException exception will be thrown if we do not have
                // discovery permission on a folder or file. It may or may not be acceptable
                // to ignore the exception and continue enumerating the remaining files and
                // folders. It is also possible (but unlikely) that a DirectoryNotFound exception
                // will be raised. This will happen if currentDir has been deleted by
                // another application or thread after our call to Directory.Exists. The
                // choice of which exceptions to catch depends entirely on the specific task
                // you are intending to perform and also on how much you know with certainty
                // about the systems on which this code will run.
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (System.IO.DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                string[] files = null;
                try
                {
                    files = System.IO.Directory.GetFiles(currentDir);
                }

                catch (UnauthorizedAccessException e)
                {

                    Console.WriteLine(e.Message);
                    continue;
                }

                catch (System.IO.DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                // Perform the required action on each file here.
                foreach (string file in files)
                {
                    try
                    {
                        // Perform whatever action is required in your scenario.
                        AddToFileOccuranceDictionary(file);
                    }
                    catch (System.IO.FileNotFoundException e)
                    {
                        // If file was deleted by a separate application
                        //  or thread since the call to TraverseTree()
                        // then just continue.
                        Console.WriteLine(e.Message);
                        continue;
                    }
                }

                // Push the subdirectories onto the stack for traversal.
                // This could also be done before handing the files.
                foreach (string str in subDirs)
                    dirs.Push(str);
            }

            return FileOccuranceDictionary;
        }

        private void AddToFileOccuranceDictionary(string file)
        {
            string fname = Path.GetFileName(file);
            if (!FileOccuranceDictionary.ContainsKey(fname))
                FileOccuranceDictionary.Add(fname, new SortedSet<string> { file });
            else
                FileOccuranceDictionary[fname].Add(file);
        }
    }

    public class Program
    {
        public void Main()
        {

        }
    }
}