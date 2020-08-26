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

        static readonly Regex VersionRegex = new Regex(@"\n    \<Version\>((\d+)\.(\d+)\.(\d+))\</Version\>");

        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.Write("请输入版本号(主版本号.次版本号.修订号)：");
                var version = Console.ReadLine();

                if (!ValidRegex.IsMatch(version))
                {
                    Console.WriteLine("版本号格式(主版本号.次版本号.修订号)不正确。正确示例：1.0.0");
                    return;
                }

                SetVersion(version);
            }
            else if (args != null && args.Length == 1 && ValidRegex.IsMatch(args[10]))
            {
                var maxVersion = FindMaxVersion();
                SetVersion(maxVersion);
            }
            else if (args != null && args.Length == 1 && args[0].Equals("update"))
            {
                var maxVersion = FindMaxVersion();
                SetVersion(maxVersion);
            }
            else
            {
                ShowMenu();
            }
        }

        private static void SetVersion(string version)
        {
            var root = new DirectoryInfo(Environment.CurrentDirectory);

            var files = root.GetFiles("*csproj", SearchOption.AllDirectories);

            foreach (var item in files)
            {
                var content = File.ReadAllText(item.FullName);

                content = VersionRegex.Replace(content, $"\n    <Version>{version}</Version>");

                File.WriteAllText(item.FullName, content, Encoding.UTF8);
            }
        }

        private static string FindMaxVersion()
        {
            var root = new DirectoryInfo(Environment.CurrentDirectory);

            var files = root.GetFiles("*csproj", SearchOption.AllDirectories);

            var maxVersionValue = 0;

            foreach (var item in files)
            {
                var content = File.ReadAllText(item.FullName);

                var versionRegex = VersionRegex.Match(content);
                if (versionRegex.Success)
                {
                    var tempVersion = versionRegex.Groups[1].Value;
                    Console.WriteLine(item.FullName);
                    Console.WriteLine(tempVersion);
                    var tempMajorVersion = int.Parse(versionRegex.Groups[2].Value);
                    var tempMinorVersion = int.Parse(versionRegex.Groups[3].Value);
                    var tempBuildVersion = int.Parse(versionRegex.Groups[4].Value);
                    var tempVersionValue = tempMajorVersion * 10000 * 10000 + tempMinorVersion * 10000 + tempBuildVersion;
                    maxVersionValue = Math.Max(maxVersionValue, tempVersionValue);
                }
            }

            var buildVersion = maxVersionValue % 10000;
            maxVersionValue /= 10000;
            var minorVersion = maxVersionValue % 10000;
            maxVersionValue /= 10000;
            var majorVersion = maxVersionValue;

            Console.WriteLine($"oldMaxVersion = {majorVersion}.{minorVersion}.{buildVersion}");
            Console.WriteLine($"newMaxVersion = {majorVersion}.{minorVersion}.{buildVersion + 1}");

            return $"{majorVersion}.{minorVersion}.{buildVersion + 1}";
        }

        static void ShowMenu()
        {
            Console.WriteLine("1.设置版本号 netversion 1.0.1");
            Console.WriteLine("2.升级版本号 netversion update");
        }
    }
}
