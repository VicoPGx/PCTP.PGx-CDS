using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.App.Utility.Helpers
{
    public class DllHelper
    {
        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymInitialize(IntPtr hProcess, string UserSearchPath, [MarshalAs(UnmanagedType.Bool)]bool fInvadeProcess);

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymCleanup(IntPtr hProcess);

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern ulong SymLoadModuleEx(IntPtr hProcess, IntPtr hFile,
             string ImageName, string ModuleName, long BaseOfDll, int DllSize, IntPtr Data, int Flags);

        [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SymEnumerateSymbols64(IntPtr hProcess,
           ulong BaseOfDll, SymEnumerateSymbolsProc64 EnumSymbolsCallback, IntPtr UserContext);

        public delegate bool SymEnumerateSymbolsProc64(string SymbolName,
              ulong SymbolAddress, uint SymbolSize, IntPtr UserContext);

        public static bool EnumSyms(string name, ulong address, uint size, IntPtr context)
        {
            System.Console.WriteLine(name);
            list.Add(name);
            return true;
        }


        private static List<string> list = new List<string>();

        /// <summary>
        /// Export function names of a native C style API dll
        /// </summary>
        /// <param name="filepath">File path of dll</param>
        /// <returns>List of function names</returns>
        /// <remarks>
        /// Use dbghelp.dl (part of the Debugging Tools for the Windows platform) to enumerate dll functions. The dbghelp DLL provides a function called SymEnumerateSymbols64 which allows you to enumerate all exported symbols of a dynamic link library. There is also a newer function called SymEnumSymbols which also allows to enumerate exported symbols.
        /// </remarks>
        public static List<string> ExportFunctions(string filepath)
        {
            list.Clear();

            if (IO.File.Exists(filepath) == false)
                return new List<string>();

            IntPtr hCurrentProcess = Process.GetCurrentProcess().Handle;

            ulong baseOfDll;
            bool status;

            // Initialize sym.
            // Please read the remarks on MSDN for the hProcess
            // parameter.
            status = SymInitialize(hCurrentProcess, null, false);

            if (status == false)
            {
                System.Console.WriteLine("Failed to initialize sym.");
                return new List<string>();
            }

            // Load dll.
            baseOfDll = SymLoadModuleEx(hCurrentProcess,
                                        IntPtr.Zero,
                                        filepath,
                                        null,
                                        0,
                                        0,
                                        IntPtr.Zero,
                                        0);

            if (baseOfDll == 0)
            {
                System.Console.WriteLine("Failed to load module.");
                SymCleanup(hCurrentProcess);
                return list;
            }

            // Enumerate symbols. For every symbol the 
            // callback method EnumSyms is called.
            if (SymEnumerateSymbols64(hCurrentProcess,
                baseOfDll, EnumSyms, IntPtr.Zero) == false)
            {
                System.Console.WriteLine("Failed to enum symbols.");
                return list;
            }

            // Cleanup.
            SymCleanup(hCurrentProcess);

            return new List<string>(list);
        }        
    }
}
