using System;
using System.IO;
using System.Reflection;

namespace backend.Content
{
    public static class ContentPath
    {
        private static readonly string ExeLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                                                     throw new Exception("Executable location could not be determined");

        public static string PlzData { get; } = GetAbsoluteContent("plz.data");
        public static string SinnDesLebens { get; } = GetAbsoluteContent("sinndeslebens.txt");

        private static string GetAbsoluteContent(string location)
        {
            return Path.Combine(ExeLocation, "content", location);
        }

    }
}
