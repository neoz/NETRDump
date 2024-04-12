using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using dnlib.DotNet;
using System.Reflection;
using System.Xml.Linq;

namespace NETRDump
{
    public static class Extension
    {
        public static byte[] Decompress(this byte[] data)
        {
            using var input = new MemoryStream(data);
            using var output = new MemoryStream();
            using var deflateStream = new DeflateStream(input, CompressionMode.Decompress);
            deflateStream.CopyTo(output);

            return output.ToArray();
        }

        public static string GetOutputPath(this Assembly module)
        {
            string name = Path.GetFileName(module.Location);
            return Path.Combine(module.Location.Remove(module.Location.Length - name.Length),
                @$"{module.ManifestModule.Name}-decompressed-resources\");
        }
    }
}
