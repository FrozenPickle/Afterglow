using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Afterglow.Core.Extensions;
using Afterglow.Core.Configuration;
using System.Reflection;
using Afterglow.Core.Plugins;
using Afterglow.Core.Storage;
using Afterglow.Core.Log;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections;
namespace Afterglow.Core.Storage
{
    public abstract class BaseRecord : INotifyPropertyChanged, INotifyCollectionChanged
    {
        public BaseRecord()
        {

        }

        public BaseRecord(ITable storage, ILogger logger, AfterglowRuntime runtime)
        {
            this.Table = storage;
            this.Id = storage.Id;
            this.Runtime = runtime;
            this.Logger = logger;
            SaveToStorage(() => this.Type, this.Type);

        }

        [ConfigString(DisplayName = "Id", Description = "An internal value used to differentiate between plugins.", IsHidden = true)]
        public string Id
        {
            get { return Get(() => Id, () => Guid.NewGuid().ToString()); }
            set { Set(() => Id, value); }
        }

        [ConfigString(DisplayName = "Type", IsHidden = true)]
        public string Type
        {
            get { return Get(() => this.Type, () => GetType().ToString()); }
            set { Set(() => this.Type, value); }
        }

        /// <summary>
        /// The logger for this plugin.
        /// </summary>
        public Afterglow.Core.Log.ILogger Logger { get; set; }

        public Afterglow.Core.AfterglowRuntime Runtime { get; set; }

        [ConfigLookup(DisplayName = "Log Level", SortIndex = -50)]
        public Afterglow.Core.Log.Level LogLevel
        {
            get { return Get(() => LogLevel, () => Afterglow.Core.Log.Level.Error); }
            set { Set(() => LogLevel, value); }
        }

        /// <summary>
        /// The storage
        /// </summary>
        public Afterglow.Core.Storage.ITable Table { get; set; }

        #region Storage Methods
        public void SaveToStorage<T>(Expression<Func<T>> property, object value)
        {
            SaveToStorage(property.PropertyName(), value);
        }

        private void SaveToStorage(string propertyName, object value)
        {
            if (Table == null)
            {
                //TODOLogger.Error("Storage has not be setup for " + this.Name);
                return;
            }

            Type t = this.GetType();

            PropertyInfo prop = t.GetProperty(propertyName);

            if (Attribute.IsDefined(prop, typeof(ConfigTableAttribute)))
            {
                ConfigTableAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigTableAttribute)) as ConfigTableAttribute;

                string ids = string.Empty;
                Type propertyType = prop.PropertyType;
                if (typeof(IEnumerable<object>).IsAssignableFrom(propertyType))
                {
                    var propValue = value as IEnumerable;
                    if (propValue != null)
                    {
                        foreach (BaseRecord item in propValue)
                        {

                            if (!string.IsNullOrEmpty(ids))
                            {
                                ids += ",";
                            }
                            ids += item.Id.ToString();
                        }
                    }
                }
                else
                {
                    ids = (value as BaseRecord).Id.ToString();
                }

                //Find removed ids then delete them from the settings file
                string[] oldValues = Table.GetTableIDs(propertyName);
                string[] newValues = ids.Split(',');
                string[] removedIds = (from o in oldValues
                                       where !(from n in newValues
                                               where n == o
                                               select n).Any()
                                       select o).ToArray();
                //remove records now they have been found
                foreach (string id in removedIds)
                {
                    Table.Database.RemoveTable(id);
                }

                //Update/Add records
                Table.Set(propertyName, ids);

            }
            else if (Attribute.IsDefined(prop, typeof(ConfigAttribute)) && !Attribute.IsDefined(prop, typeof(ConfigReadOnlyAttribute)))
            {
                Table.Set(propertyName, value);
            }

        }

        /// <summary>
        /// Loads settings from storage
        /// </summary>
        public T LoadFromStorage<T>(string propertyName)
        {
            T result = default(T);
            if (Table == null)
            {
                //TODO Logger.Error("Storage has not be setup for " + this.Name);
                return result;
            }

            Type t = this.GetType();

            PropertyInfo prop = t.GetProperty(propertyName);
            
            if (prop.CanWrite)
            {
                if (Attribute.IsDefined(prop, typeof(ConfigLookupAttribute)))
                {
                    ConfigLookupAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigLookupAttribute)) as ConfigLookupAttribute;
                    Type propertyType = prop.PropertyType;
                    if (propertyType.IsEnum)
                    {
                        string value = Table.GetString(prop.Name);
                        if (!string.IsNullOrEmpty(value))
                        {
                            result = (T)(Enum.Parse(propertyType, value) as Object);
                        }
                    }
                    else if (prop.PropertyType == typeof(int?))
                    {
                        result = (T)(Table.GetInt(prop.Name) as object);
                    }
                    else if (prop.PropertyType == typeof(string))
                    {
                        result = (T)(Table.GetString(prop.Name) as object);
                    }
                    else
                    {
                        //TODO change to log error
                        throw new Exception(prop.PropertyType.ToString() + " is not supported update BasePlugin.LoadFromStorage() to implement support");
                    }
                }
                else if (Attribute.IsDefined(prop, typeof(ConfigNumberAttribute)))
                {
                    ConfigNumberAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigNumberAttribute)) as ConfigNumberAttribute;

                    if (prop.PropertyType == typeof(int?))
                    {
                        result = (T)(Table.GetInt(prop.Name) as object);
                    }
                    else if (prop.PropertyType == typeof(double?))
                    {
                        result = (T)(Table.GetDouble(prop.Name) as object);
                    }
                    else
                    {
                        throw new Exception(prop.PropertyType.ToString() + " is not supported update BasePlugin.LoadFromStorage() to implement support");
                    }
                }
                else if (Attribute.IsDefined(prop, typeof(ConfigStringAttribute)))
                {
                    ConfigStringAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigStringAttribute)) as ConfigStringAttribute;

                    result = (T)(Table.GetString(prop.Name) as object);
                }
                else if (Attribute.IsDefined(prop, typeof(ConfigTableAttribute)))
                {
                    ConfigTableAttribute configAttribute = Attribute.GetCustomAttribute(prop, typeof(ConfigTableAttribute)) as ConfigTableAttribute;

                    //Single Item
                    if (typeof(T).IsInterface)
                    {
                        ITable table = Table.GetTables(prop.Name).FirstOrDefault();

                        if (table != null)
                        {
                            string tableTypeString = table.GetString("Type");
                            if (!string.IsNullOrEmpty(tableTypeString))
                            {
                                //Get type from local project if possible, if not use other loader
                                Type tableType = System.Type.GetType(tableTypeString) ?? Runtime.Loader.GetObjectType(tableTypeString);
                                string id = table.GetString("Id");
                                BaseRecord createdTable = Activator.CreateInstance(tableType, Table.Database.AddTable(id), Logger, this.Runtime) as BaseRecord;

                                result = (T)(createdTable as object);
                            }
                        }
                    }
                    //List of items
                    else
                    {
                        result = (T)Activator.CreateInstance(typeof(T));
                        foreach (ITable table in Table.GetTables(prop.Name))
                        {
                            string tableTypeString = table.GetString("Type");
                            if (!string.IsNullOrEmpty(tableTypeString))
                            {
                                //Get type from local project if possible, if not use other loader
                                Type tableType = System.Type.GetType(tableTypeString) ?? Runtime.Loader.GetObjectType(tableTypeString);
                                string id = table.GetString("Id");
                                BaseRecord createdTable = Activator.CreateInstance(tableType, Table.Database.AddTable(id), Logger, this.Runtime) as BaseRecord;

                                result.GetType().GetMethod("Add").Invoke(result, new object[] { createdTable });
                            }
                        }
                    }
                }
            }
            return result;
        }
        #endregion

        #region INotifyPropertyChanged

        private IDictionary<string, object> _propertyMap;

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, e) => { };

        protected IDictionary<string, object> PropertyMap
        {
            get
            {
                return _propertyMap ?? (_propertyMap = new Dictionary<string, object>());
            }
            set
            {
                _propertyMap = value;
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void NotifyPropertyChanged<T>(Expression<Func<T>> property)
        {
            NotifyPropertyChanged(property.PropertyName());
        }

        public T Get<T>(Expression<Func<T>> property)
        {
            return Get(property, default(T));
        }

        public T Get<T>(Expression<Func<T>> property, T defaultValue)
        {
            return Get(property, () => defaultValue);
        }

        public T Get<T>(Expression<Func<T>> property, Func<T> defaultValue)
        {
            bool loaded = false;

            if (property == null)
            {
                throw new ArgumentException("cannot be null", "property");
            }

            var name = property.PropertyName();
            T result = default(T);

            if (PropertyMap.ContainsKey(name))
            {
                result = (T)PropertyMap[name];
            }
            else
            {

                //load from disk
                result = LoadFromStorage<T>(name);
                if (result != null)
                {
                    loaded = true;
                }

                //get default value
                if (!loaded && defaultValue != null)
                {
                    try
                    {
                        result = defaultValue.Invoke();
                    }
                    catch (Exception)
                    {
                        //TODO log
                        result = default(T);
                    }
                }

                if (result != null)
                {
                    Set(property, result, !loaded);
                }
            }

            return result;
        }

        public bool Set<T>(Expression<Func<T>> property, T value, bool save = true)
        {
            if (property == null)
            {
                throw new ArgumentException("cannot be null", "property");
            }

            var name = property.PropertyName();

            object tryValue;

            if (PropertyMap.TryGetValue(name, out tryValue))
            {
                T oldValue = (T)tryValue;

                if (Equals(oldValue, default(T)) && Equals(value, default(T)))
                    return false;

                if (!Equals(oldValue, default(T)) && oldValue.Equals(value))
                    return false;

                PropertyMap[name] = value;
            }
            else
            {
                PropertyMap.Add(name, value);
            }

            if (save)
            {
                SaveToStorage(name, value);
            }
            NotifyPropertyChanged(name);
            return true;
        }

        #endregion
    }
}
