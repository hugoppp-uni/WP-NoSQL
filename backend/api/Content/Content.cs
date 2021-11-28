using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace backend
{
    public static class Content
    {
        public static JsonElement[] PlzData = File.ReadAllLines(ContentPath.PlzData)
            .Select(line => JsonSerializer.Deserialize<JsonElement>(line)).ToArray();
    }

    public static class ContentPath
    {
        private static readonly string ExeLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                                                     throw new Exception("Executable location could not be determined");

        public static string PlzData { get; } = GetAbsoluteContent("plz.data");

        public static string SinnDesLebens { get; } = GetAbsoluteContent("sinndeslebens.txt");

        private static string GetAbsoluteContent(string location)
        {
            return Path.Combine(ExeLocation, "Content", location);
        }
    }

}
