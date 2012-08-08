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

namespace Afterglow.Core
{
    public abstract class BaseModel : INotifyPropertyChanged, INotifyCollectionChanged
    {
        [Display(Name = "Index")]
        [Key]
        public int Id
        {
            get { return Get(() => Id); }
            set { Set(() => Id, value); }
        }

        //A reference to the object that created this one
        [XmlIgnore]
        public AfterglowRuntime Runtime
        {
            get { return Get(() => Runtime); }
            set { Set(() => Runtime, value); }
        }

        private IDictionary<string, object> _propertyMap;
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

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, e) => { };

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
    }
}
