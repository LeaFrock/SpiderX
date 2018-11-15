using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using SpiderX.DataClient;

namespace SpiderX.Launcher
{
    public sealed class AppSettingManager
    {
        private static AppSettingManager _instance;

        public static AppSettingManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    CreateInstance(null, null);
                }
                return _instance;
            }
        }

        public static AppSettingManager CreateInstance(string name, string[] paramStrings)
        {
            if (Interlocked.CompareExchange(ref _instance, new AppSettingManager(), null) == null)
            {
                _instance.Initialize(name, paramStrings);
            }
            return _instance;
        }

        private static string CorrectCaseName(string name)
        {
            string trimName = name.Trim();
            if (!trimName.EndsWith("Bll", StringComparison.CurrentCultureIgnoreCase))
            {
                trimName += "Bll";
            }
            return trimName;
        }

        private AppSettingManager()
        {
        }

        public string CaseName { get; private set; }

        public string[] CaseParams { get; private set; }

        public string BusinessModuleName { get; private set; }

        public string BusinessModulePath { get; private set; }

        private BusinessModuleCopyModeEnum _businessModuleCopyMode;

        public BusinessModuleCopyModeEnum BusinessModuleCopyMode => _businessModuleCopyMode;

        public bool AutoClose { get; private set; } = true;

        public string[] GetBusinessModuleSourceFiles()
        {
            if (!Directory.Exists(BusinessModulePath))
            {
                throw new DirectoryNotFoundException("Modules Load Fail: " + BusinessModulePath);
            }
            string[] sourceFilePaths = Directory.GetFiles(BusinessModulePath, "*.dll");
            if (sourceFilePaths.Length < 1)
            {
                throw new FileNotFoundException("Modules Load Fail: No dlls Matched.");
            }
            return sourceFilePaths;
        }

        public void CopyModuleTo(string destDirectoryRoot)
        {
            if (string.IsNullOrWhiteSpace(BusinessModulePath))
            {
                //throw new ArgumentException("Modules Load Fail: Invalid BusinessModulePath.");
                return;
            }
            string destDirectoryPath = Path.Combine(destDirectoryRoot, CaseName);
            if (!Directory.Exists(destDirectoryPath))
            {
                Directory.CreateDirectory(destDirectoryPath);
            }
            string[] sourceFiles = GetBusinessModuleSourceFiles();
            switch (_businessModuleCopyMode)
            {
                case BusinessModuleCopyModeEnum.AlwaysCopy:
                    foreach (string sourceFilePath in sourceFiles)
                    {
                        string sourceFileName = Path.GetFileName(sourceFilePath);
                        string destFilePath = Path.Combine(destDirectoryPath, sourceFileName);
                        File.Copy(sourceFilePath, destFilePath, true);
                    }
                    break;

                case BusinessModuleCopyModeEnum.CopyOnce:
                    foreach (string sourceFile in sourceFiles)
                    {
                        string sourceFileName = Path.GetFileName(sourceFile);
                        string destFileName = Path.Combine(destDirectoryPath, sourceFileName);
                        File.Copy(sourceFile, destFileName);
                    }
                    break;

                default: break;
            }
        }

        private void Initialize(string caseName, string[] caseParams)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "AppSettings");
            var conf = new ConfigurationBuilder()
                .SetBasePath(filePath)
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            //Load Module Name
            string nameSpace = conf.GetSection(nameof(BusinessModuleName)).Value;
            if (string.IsNullOrWhiteSpace(nameSpace))
            {
                throw new ArgumentNullException("Invalid BusinessModuleName.");
            }
            BusinessModuleName = nameSpace;
            BusinessModulePath = conf.GetSection(nameof(BusinessModulePath)).Value;
            string copyMode = conf.GetSection(nameof(BusinessModuleCopyMode)).Value;
            Enum.TryParse(copyMode, true, out _businessModuleCopyMode);
            //Load Case Name&Params
            if (caseName == null)
            {
                caseName = conf.GetSection(nameof(CaseName)).Value;
            }
            if (string.IsNullOrWhiteSpace(caseName))
            {
                throw new ArgumentNullException("CaseName is Null or WhiteSpace");
            }
            CaseName = CorrectCaseName(caseName);
            CaseParams = caseParams ?? conf.GetSection(nameof(CaseParams)).GetChildren().Select(p => p.Value).ToArray();
            //Load DbConfigs
            var dbConfigs = LoadDbConfigs(conf);
            DbClientSetting.Instance.InitializeConfigs(dbConfigs);
            //Load Other
            var autoCloseStr = conf.GetSection(nameof(AutoClose)).Value;
            if (bool.TryParse(autoCloseStr, out bool autoClose))
            {
                AutoClose = autoClose;
            }
        }

        private List<DbConfig> LoadDbConfigs(IConfigurationRoot root)
        {
            var dbSections = root.GetSection("DbConfigs").GetChildren();
            var result = new List<DbConfig>();
            foreach (var dbSection in dbSections)
            {
                DbConfig item = DbConfig.CreateInstance(dbSection);
                if (item != null)
                {
                    result.Add(item);
                }
            }
            if (result.Count < 1)
            {
                throw new ArgumentException("No DbConfigs Valid.");
            }
            return result;
        }
    }

    public enum BusinessModuleCopyModeEnum : sbyte
    {
        CopyOnce = 0,//Copy only when the module doesn't exist
        AlwaysCopy = 1,//Copy or Overwrite the module
    }
}