using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Load
{
    public interface ILoader
    {
        Type GetObjectType(string type);
    }
}
