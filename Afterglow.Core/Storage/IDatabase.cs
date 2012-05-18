using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Storage
{
    public interface IDatabase
    {
        ITable AddTable(string id = null);

        void RemoveTable(string id);
    }
}
