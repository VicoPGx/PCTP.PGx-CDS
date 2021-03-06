using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace FileHelpers.Tests
{
    /// <summary>
    /// This class only adds the relative path to the sample files. 
    /// </summary>
    public static class TestCommon
    {
        /// <summary>
        /// Create a path to a directory or file given a list of directories
        /// to get there.  Goes from project directory
        /// </summary>
        /// <remarks>.. takes it back from bin, Data is forward to data area</remarks>
        /// <param name="pathElements">list of directories to navigate with optional filename</param>
        /// <returns>Path to Data area</returns>
        public static string GetPath(params string[] pathElements)
        {
            var dataDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "FileHelpers.Tests", "Data");

            foreach (var element in pathElements)
                dataDirectory = Path.Combine(dataDirectory, element);

            return dataDirectory;
        }

        /// <summary>
        /// Create a temporary filename for work
        /// </summary>
        /// <param name="fileName">Filename for test</param>
        /// <returns>Path to a working file</returns>
        public static string GetTempFile(string fileName)
        {
            return Path.Combine(Path.GetTempPath(), fileName);
        }

        /// <summary>
        /// Use an engine to read an array of objects
        /// </summary>
        /// <param name="engine">Engine to read file</param>
        /// <param name="pathElements">List of directories and a filename in Data area</param>
        /// <returns>objects from file</returns>
        public static T[] ReadTest<T>(FileHelperEngine<T> engine, params string[] pathElements) where T : class
        {
            return engine.ReadFile(GetPath(pathElements));
        }

        /// <summary>
        /// Read the file using the async engine
        /// </summary>
        /// <param name="engine">Engine to read the data</param>
        /// <param name="pathElements">List of directories and a filename in Data area</param>
        /// <returns>objects from file</returns>
        public static object[] ReadAllAsync(FileHelperAsyncEngine engine, params string[] pathElements)
        {
            var arr = new ArrayList();

            using (engine.BeginReadFile(GetPath(pathElements))) {
                while (engine.ReadNext() != null)
                    arr.Add(engine.LastRecord);
            }

            return arr.ToArray();
        }

        /// <summary>
        /// Read the file using the async engine
        /// </summary>
        /// <param name="engine">Engine to read the data</param>
        /// <param name="pathElements">List of directories and a filename in Data area</param>
        /// <returns>objects from file</returns>
        public static List<T> ReadAllAsync<T>(FileHelperAsyncEngine<T> engine, params string[] pathElements)
            where T : class
        {
            var arr = new List<T>();

            using (engine.BeginReadFile(GetPath(pathElements))) {
                while (engine.ReadNext() != null)
                    arr.Add(engine.LastRecord);
            }

            return arr;
        }

        /// <summary>
        /// Use an engine to read the first part of the file
        /// </summary>
        /// <param name="engine">Engine to read the data</param>
        /// <param name="pathElements">List of directories and a filename in Data area</param>
        public static void BeginReadTest(FileHelperAsyncEngine engine, params string[] pathElements)
        {
            engine.BeginReadFile(GetPath(pathElements));
        }

        /// <summary>
        /// Use an engine to read the first part of the file
        /// </summary>
        /// <param name="engine">Engine to read the data</param>
        /// <param name="pathElements">List of directories and a filename in Data area</param>
        public static void BeginReadTest<T>(FileHelperAsyncEngine<T> engine, params string[] pathElements)
            where T : class
        {
            engine.BeginReadFile(GetPath(pathElements));
        }
    }
}