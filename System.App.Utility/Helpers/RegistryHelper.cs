using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.App.Utility.Helpers
{
    /// <summary>
    /// A registry helper that works for both 32-bit and 64-bit OS
    /// </summary>
    /// <remarks>
    /// http://stackoverflow.com/questions/1268715/registry-localmachine-opensubkey-returns-null
    /// </remarks>
    public class RegistryHelper
    {
        public static RegistryKey GetRegistryKey(RegistryHive hive)
        {
            return GetRegistryKey(hive, null);
        }

        public static RegistryKey GetRegistryKey(RegistryHive hive, string keyPath)
        {
            RegistryKey localMachineRegistry
                = RegistryKey.OpenBaseKey(hive,
                                          Environment.Is64BitOperatingSystem
                                              ? RegistryView.Registry64
                                              : RegistryView.Registry32);

            return string.IsNullOrEmpty(keyPath)
                ? localMachineRegistry
                : localMachineRegistry.OpenSubKey(keyPath);
        }

        public static object GetRegistryValue(RegistryHive hive, string keyPath, string keyName)
        {
            RegistryKey registry = GetRegistryKey(hive, keyPath);
            return registry.GetValue(keyName);
        }
    }
}
