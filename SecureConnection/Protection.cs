using System;
using System.Configuration;
using System.IO;

namespace SecureConnection
{
    public class Protection
    {
        public string FileName { get; set; }
        public Protection(string executableFileName)
        {
            if (!(File.Exists(string.Concat(executableFileName, ".config"))))
            {
                throw new FileNotFoundException(string.Concat(executableFileName, ".config"));
            }
            FileName = executableFileName;
        }
        private bool EncryptConnectionString(bool encrypt, string fileName)
        {
            bool success = true;
            Configuration configuration = null;

            try
            {
                configuration = ConfigurationManager.OpenExeConfiguration(fileName);
                var configSection = configuration.GetSection("connectionStrings") as ConnectionStringsSection;

                if ((!configSection.ElementInformation.IsLocked) && (!configSection.SectionInformation.IsLocked))
                {
                    if (encrypt && (!configSection.SectionInformation.IsProtected))
                    {
                        // encrypt the file
                        configSection.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    }

                    if ((!encrypt) && configSection.SectionInformation.IsProtected) //encrypt is true so encrypt
                    {
                        // decrypt the file. 
                        configSection.SectionInformation.UnprotectSection();
                    }

                    configSection.SectionInformation.ForceSave = true;
                    configuration.Save();

                    success = true;

                }
            }
            catch (Exception)
            {
                success = false;
            }

            return success;

        }
        public bool IsProtected()
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(FileName);
            var configSection = configuration.GetSection("connectionStrings") as ConnectionStringsSection;
            return configSection.SectionInformation.IsProtected;
        }
        public bool EncryptFile() => File.Exists(FileName) && EncryptConnectionString(true, FileName);

        public bool DecryptFile() => File.Exists(FileName) && EncryptConnectionString(false, FileName);
    }
}
