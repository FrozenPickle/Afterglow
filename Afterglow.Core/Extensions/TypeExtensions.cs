//
// TypeExtensions.cs:
//
// Author:
//   Robert Kozak (rkozak@gmail.com)
//
// Copyright 2011, Nowcom Corporation
//
// Code licensed under the MIT X11 license
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Afterglow.Core.Extensions
{
    internal static class TypeExtensions
    {
        public static T GetCustomAttribute<T>(this MemberInfo member) where T : Attribute
        {
            return GetCustomAttribute<T>(member, false);
        }

        public static T GetCustomAttribute<T>(this MemberInfo member, bool inherited) where T : Attribute
        {
            var attributes = Attribute.GetCustomAttributes(member, typeof(T), inherited);
            if (attributes.Length > 0)
                return attributes[0] as T;

            return default(T);
        }

        public static PropertyInfo GetNestedProperty(this Type sourceType, ref object obj, string path, bool allowPrivateProperties)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;

            if (allowPrivateProperties)
                bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            Type type = sourceType;

            if (path.Contains("."))
            {
                PropertyInfo info = null;
                var properties = path.Split('.');
                for (int index = 0; index < properties.Length; index++)
                {
                    var property = properties[index];
                    info = type.GetProperty(property, bindingFlags);
                    if (info != null)
                    {
                        type = info.PropertyType;

                        if (obj != null && index < properties.Length - 1)
                        {
                            try
                            {
                                obj = info.GetValue(obj, null);
                            }
                            catch (TargetInvocationException)
                            {
                            }
                        }
                    }
                }

                return info;
            }
            else
                return type.GetProperty(path, bindingFlags);
        }
    }
}
