using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetVersion
{
    class Program
    {
        static readonly Regex ValidRegex = new Regex(@"\d+\.\d+\.\d+");

        static readonly Regex VersionRegex = new Regex(@"\n    \<Version\>\d+\.\d+\.\d+\</Version\>");
        //static readonly Regex FileVersionRegex = new Regex(@"\n    \<FileVersion\>\d+\.\d+\.\d+\</FileVersion\>");
        //static readonly Regex AssemblyVersion = new Regex(@"\n    \<AssemblyVersion\>\d+\.\d+\.\d+\</AssemblyVersion\>");

        static void Main(string[] args)
        {
            var version = "1.0.0";
            if (args == null || args.Length == 0)
            {
                Console.Write("请输入版本号(主版本号.次版本号.修订号)：");
                version = Console.ReadLine();
            }
            else
            {
                version = args[0];
            }

            if (!ValidRegex.IsMatch(version))
            {
                Console.WriteLine("版本号格式(主版本号.次版本号.修订号)不正确。正确示例：1.0.0");
                return;
            }

            var root = new DirectoryInfo(Environment.CurrentDirectory);

            var files = root.GetFiles("*csproj", SearchOption.AllDirectories);

            foreach (var item in files)
            {
                var content = File.ReadAllText(item.FullName);

                content = VersionRegex.Replace(content, $"\n    <Version>{version}</Version>");
                //content = FileVersionRegex.Replace(content, $"\n    <FileVersion>{version}</FileVersion>");
                //content = AssemblyVersion.Replace(content, $"\n    <AssemblyVersion>{version}</AssemblyVersion>");

                File.WriteAllText(item.FullName, content, Encoding.UTF8);
            }

            Console.WriteLine("success.");
        }

        static void Help()
        {
            Console.WriteLine("example: netversion 1.0.1");
        }
    }
}
