using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Load
{
    //Used to load Objects in other project/namespaces
    public interface ILoader
    {
        Type GetObjectType(string type);

        Type[] GetPlugins(Type pluginType);
    }
}
