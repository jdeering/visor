using System;
using System.IO;

namespace Visor
{
    public static class Constants
    {
        public static readonly string AppDataPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Visor");

        public static readonly string KeyFilePath =
            Path.Combine(AppDataPath, "Key.pkf");

        public static readonly string IVFilePath =
            Path.Combine(AppDataPath, "IV.pkf");
        
    }
}