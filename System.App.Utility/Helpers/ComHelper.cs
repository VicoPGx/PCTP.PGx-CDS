using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace System.App.Utility.Helpers
{
    [ComVisible(false)]
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000001-0000-0000-C000-000000000046")]
    internal interface IClassFactory
    {
        void CreateInstance([MarshalAs(UnmanagedType.Interface)] object pUnkOuter, ref Guid refiid, [MarshalAs(UnmanagedType.Interface)] out object ppunk);
        void LockServer(bool fLock);
    }

    /// <summary>
    /// Utility class to get a Class Factory for a certain Class ID 
    /// by loading the dll that implements that class
    /// </summary>
    public static class ComHelper
    {
        //DllGetClassObject fuction pointer signature
        private delegate int DllGetClassObject(ref Guid ClassId, ref Guid InterfaceId, [Out, MarshalAs(UnmanagedType.Interface)] out object ppunk);

        //Some win32 methods to load\unload dlls and get a function pointer
        public class Win32NativeMethods
        {
            // [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            // public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
            /// <remarks>
            /// GetProcAddress only comes in an ANSI flavor, hence we help the runtime by telling it to always use ANSI when marshalling the string parameter. We also prevent the runtime looking for a non-existent GetProcAddressA, because the default for C# is to set ExactSpelling to false.
            /// </remarks>
            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr LoadLibrary(string lpFileName);
        }

        /// <summary>
        /// Holds a list of dll handles and unloads the dlls 
        /// in the destructor
        /// </summary>
        private class DllList
        {
            private List<IntPtr> _dllList = new List<IntPtr>();
            public void AddDllHandle(IntPtr dllHandle)
            {
                lock (_dllList)
                {
                    _dllList.Add(dllHandle);
                }
            }

            ~DllList()
            {
                foreach (IntPtr dllHandle in _dllList)
                {
                    try
                    {
                        Win32NativeMethods.FreeLibrary(dllHandle);
                    }
                    catch { };
                }
            }
        }

        static DllList _dllList = new DllList();

        /// <summary>
        /// Gets a class factory for a specific COM Class ID. 
        /// </summary>
        /// <param name="dllName">The dll where the COM class is implemented</param>
        /// <param name="filterPersistClass">The requested Class ID</param>
        /// <returns>IClassFactory instance used to create instances of that class</returns>
        internal static IClassFactory GetClassFactory(string dllName, string filterPersistClass)
        {
            //Load the class factory from the dll
            IClassFactory classFactory = GetClassFactoryFromDll(dllName, filterPersistClass);
            return classFactory;
        }

        private static IClassFactory GetClassFactoryFromDll(string dllName, string filterPersistClass)
        {
            //Load the dll
            IntPtr dllHandle = Win32NativeMethods.LoadLibrary(dllName);
            if (dllHandle == IntPtr.Zero)
                return null;

            //Keep a reference to the dll until the process\AppDomain dies
            _dllList.AddDllHandle(dllHandle);

            //Get a pointer to the DllGetClassObject function
            IntPtr dllGetClassObjectPtr = Win32NativeMethods.GetProcAddress(dllHandle, "DllGetClassObject");
            if (dllGetClassObjectPtr == IntPtr.Zero)
                return null;

            //Convert the function pointer to a .net delegate
            DllGetClassObject dllGetClassObject = (DllGetClassObject)Marshal.GetDelegateForFunctionPointer(dllGetClassObjectPtr, typeof(DllGetClassObject));

            //Call the DllGetClassObject to retreive a class factory for out Filter class
            Guid filterPersistGUID = new Guid(filterPersistClass);
            Guid IClassFactoryGUID = new Guid("00000001-0000-0000-C000-000000000046"); //IClassFactory class id
            Object unk;
            if (dllGetClassObject(ref filterPersistGUID, ref IClassFactoryGUID, out unk) != 0)
                return null;

            //Yippie! cast the returned object to IClassFactory
            return (unk as IClassFactory);
        }
    }
}