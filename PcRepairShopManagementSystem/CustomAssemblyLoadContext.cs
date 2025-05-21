using System.Reflection;
using System.Runtime.Loader;
using System;
using System.IO;

namespace PcRepairShopManagementSystem
{
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllPath)
        {
            // This actually invokes LoadUnmanagedDllFromPath under the hood
            return LoadUnmanagedDllFromPath(unmanagedDllPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName) => null;
    }
}
