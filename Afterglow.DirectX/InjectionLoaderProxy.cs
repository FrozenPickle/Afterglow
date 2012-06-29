using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using EasyHook;

namespace Afterglow.DirectX
{
    public class InjectionLoader
    {
        #pragma warning disable 0028
        public static int Main(String InParam)
        {
            return EasyHook.InjectionLoader.Main(InParam);
        }

        public static int Test(String InParam)
        {
            Debug.WriteLine("In Afterglow.DirectX.InjectionLoader.Test");
            try
            {
                return EasyHook.InjectionLoader.Main(InParam);
            }
            catch (Exception exc)
            {
                Config.PrintError(exc.ToString());
                throw;
            }
            
            return 100;
        }
    }
}
