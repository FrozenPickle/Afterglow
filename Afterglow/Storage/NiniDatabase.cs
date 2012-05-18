using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nini.Config;
using System.IO;
using Afterglow.Core.Storage;
using Afterglow.Core;

namespace Afterglow.Storage
{
    public class NiniDatabase : IDatabase
    {
        private readonly IConfigSource _storage;

        public NiniDatabase(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException("fileName");

            if (!File.Exists(fileName))
            {
                //TODO Log error
                FileStream fileStream = File.Create(fileName);
                fileStream.Close();
            }

            IniConfigSource iniConfigSoruce = new Nini.Config.IniConfigSource();
            iniConfigSoruce.AutoSave = true;
            iniConfigSoruce.Load(fileName);

            _storage = iniConfigSoruce;

        }

        public NiniTable GetStorageGroup(string name)
        {

            if (_storage.Configs[name] == null)
            {
                _storage.AddConfig(name);
            }

            return new NiniTable(this, name);
        }

        public ConfigCollection Configs
        {
            get
            {
                return _storage.Configs;
            }
        }

        public ITable AddTable(string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                return GetStorageGroup(Guid.NewGuid().ToString());
            }
            else
            {
                return GetStorageGroup(id);
            }
        }

        public void RemoveTable(string id)
        {
            _storage.Configs.Remove(_storage.Configs[id]);
        }
    }
}
