#nullable enable
using System;
using System.IO;
using Newtonsoft.Json;

namespace Rutschig
{
    public class AppConfig
    {
        private readonly Config? _configValues;

        public AppConfig()
        {
            var stream = new MemoryStream();
            var confFile = File.ReadAllBytes(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "appconfig.json"));
            stream.Write(confFile, 0, confFile.Length);

            var streamReader = new StreamReader(stream);
            streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
            _configValues = JsonConvert.DeserializeObject<Config?>(streamReader.ReadToEnd());
        }

        public T GetValue<T>(string key)
        {
            return (T) _configValues.GetType().GetProperty(key).GetValue(_configValues, null);
        }
    }
}