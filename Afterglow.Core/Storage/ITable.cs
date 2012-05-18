using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Storage
{
    /// <summary>
    /// Storage Interface
    /// </summary>
    public interface ITable
    {


        /// <summary>
        /// An internal identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The Object type of the record
        /// </summary>
        string Type { get; set; }

        bool Contains(string key);
        bool GetBoolean(string key);
        bool GetBoolean(string key, bool defaultValue);
        double? GetDouble(string key);
        double? GetDouble(string key, double defaultValue);
        string GetExpanded(string key);
        float GetFloat(string key);
        float GetFloat(string key, float defaultValue);
        int? GetInt(string key);
        int? GetInt(string key, int defaultValue);
        string[] GetKeys();
        long GetLong(string key);
        long GetLong(string key, long defaultValue);
        string GetString(string key);
        string GetString(string key, string defaultValue);
        string[] GetValues();
        void Remove(string key);

        ITable[] GetTables(string key);
        string[] GetTableIDs(string key);

        IDatabase Database { get; }

        void Set(string key, object value);
    }
}
