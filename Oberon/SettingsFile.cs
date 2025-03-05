using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Oberon
{
    public class SettingsFile
    {
        public List<PairedRemote> PairedRemotes;

        static async Task<SettingsFile> DefaultFile()
        {
            return new SettingsFile() { PairedRemotes = new List<PairedRemote>() };
        }

        public static async Task<StorageFile> GetFileHandle()
        {
            return await ApplicationData.Current.LocalFolder.CreateFileAsync("settings.json", CreationCollisionOption.OpenIfExists);
        }

        public static async Task Write(SettingsFile newSettings)
        {
            var settingsFile = await GetFileHandle();
            await FileIO.WriteTextAsync(settingsFile, JsonConvert.SerializeObject(newSettings));
        }

        public static async Task<SettingsFile> Get()
        {
            var settingsFile = await GetFileHandle();
            var fileContent = await FileIO.ReadTextAsync(settingsFile);

            // Create default file if it doesn't exist
            if (string.IsNullOrEmpty(fileContent)) {
                var defaultFile = await DefaultFile();
                await Write(defaultFile);
                return defaultFile;
            }

            return JsonConvert.DeserializeObject<SettingsFile>(fileContent);
        }
    }
}
