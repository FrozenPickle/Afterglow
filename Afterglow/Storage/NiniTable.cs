using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Storage
{
    public class NiniTable : Afterglow.Core.Storage.ITable
    {
        private NiniDatabase _nini;
        private string _id;

        public NiniTable(NiniDatabase nini, string id)
        {
            this._nini = nini;
            this._id = id;
        }

        public Core.Storage.ITable[] GetTables(string key)
        {
            List<NiniTable> tables = new List<NiniTable>();
            foreach (string id in GetTableIDs(key))
            {
                tables.Add(this._nini.GetStorageGroup(id));
            }
            return tables.ToArray();
        }

        public string Id
        {
            get 
            { 
                return _id;
            }
        }

        public bool Contains(string key)
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(string key)
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(string key, bool defaultValue)
        {
            throw new NotImplementedException();
        }

        public double? GetDouble(string key)
        {
            try
            {
                return _nini.Configs[_id].GetDouble(key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public double? GetDouble(string key, double defaultValue)
        {
            try
            {
                return _nini.Configs[this._id].GetDouble(key, defaultValue);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetExpanded(string key)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(string key)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(string key, float defaultValue)
        {
            throw new NotImplementedException();
        }

        public int? GetInt(string key)
        {
            try
            {
                return _nini.Configs[this._id].GetInt(key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int? GetInt(string key, bool fromAlias)
        {
            throw new NotImplementedException();
        }

        public int? GetInt(string key, int defaultValue)
        {
            try
            {
                return _nini.Configs[this._id].GetInt(key, defaultValue);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public int GetInt(string key, int defaultValue, bool fromAlias)
        {
            throw new NotImplementedException();
        }

        public string[] GetKeys()
        {
            throw new NotImplementedException();
        }

        public long GetLong(string key)
        {
            throw new NotImplementedException();
        }

        public long GetLong(string key, long defaultValue)
        {
            throw new NotImplementedException();
        }

        public string GetString(string key)
        {
            return _nini.Configs[this._id].GetString(key);
        }

        public string GetString(string key, string defaultValue)
        {
            return _nini.Configs[this._id].GetString(key, defaultValue);
        }

        public string[] GetValues()
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, object value)
        {
            _nini.Configs[this._id].Set(key, value);
        }

        public string[] GetTableIDs(string key)
        {
            string result = _nini.Configs[this._id].GetString(key);
            if (result != null)
            {
                return result.Split(',');
            }
            else
            {
                return new string[0];
            }
        }

        public string Type
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public Core.Storage.IDatabase Database
        {
            get 
            { 
                return _nini; 
            }
        }
    }
}
