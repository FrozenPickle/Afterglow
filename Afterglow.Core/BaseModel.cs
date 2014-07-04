using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Afterglow.Core.Extensions;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Afterglow.Core
{
    /// <summary>
    /// All objects that are saved in Afterglow descend from this, provides reference to the AfterglowRuntime and simple property creation
    /// </summary>
    [DataContract]
    public abstract class BaseModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        /// <summary>
        /// An Identifier
        /// </summary>
        [DataMember]
        [Display(Name = "Identifier")]
        [Key]
        public int Id
        {
            get { return Get(() => Id); }
            set { Set(() => Id, value); }
        }

        private IDictionary<string, object> _propertyMap;
        /// <summary>
        /// Stores the property name and it's value
        /// </summary>
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

        /// <summary>
        /// Creates a generic get property
        /// </summary>
        /// <example>get { return Get(() => Runtime); }</example>
        /// <param name="property">A delegate to the property</param>
        /// <returns>The properties value</returns>
        public T Get<T>(Expression<Func<T>> property)
        {
            return Get(property, default(T));
        }
        /// <summary>
        /// Creates a generic get property, with a static default value
        /// </summary>
        /// <example>get { return Get(() => Runtime, new AfterglowRuntime()); }</example>
        /// <param name="property">A delegate to the property</param>
        /// <param name="defaultValue">A default value</param>
        /// <returns>The properties value</returns>
        public T Get<T>(Expression<Func<T>> property, T defaultValue)
        {
            return Get(property, () => defaultValue);
        }
        /// <summary>
        /// Creates a generic get for a property
        /// </summary>
        /// <example>get { return Get(() => DisplayName, () => this.Name); }</example>
        /// <param name="property">A delegate to the property</param>
        /// <param name="defaultValue">A delegate to the default value</param>
        /// <returns>The properties value</returns>
        public T Get<T>(Expression<Func<T>> property, Func<T> defaultValue)
        {
            T result = default(T);

            if (property == null)
            {
                throw new ArgumentException("cannot be null", "property");
            }

            var name = property.PropertyName();

            if (PropertyMap.ContainsKey(name))
            {
                result = (T)PropertyMap[name];
            }
            else
            {
                //get default value
                if (defaultValue != null)
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
                    Set(property, result);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a generic set for a property
        /// </summary>
        /// <example>set { Set(() => Runtime, value); }</example>
        /// <param name="property">A delegate to the property</param>
        /// <param name="value">Always use 'value'</param>
        /// <returns>Always returns true</returns>
        public bool Set<T>(Expression<Func<T>> property, T value)
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

                //ensure value has changed if not return so notify property changed is not called
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

            NotifyPropertyChanged(name);
            return true;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, e) => { };

        /// <summary>
        /// Notify Property Changed is used for responsive UI
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary>
        /// Notify Property Changed is used for responsive UI
        /// </summary>
        /// <param name="property">A delegate to the property</param>
        public void NotifyPropertyChanged<T>(Expression<Func<T>> property)
        {
            NotifyPropertyChanged(property.PropertyName());
        }
    }
}
