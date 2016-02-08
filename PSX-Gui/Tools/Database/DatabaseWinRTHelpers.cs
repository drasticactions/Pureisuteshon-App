using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace PlayStation_Gui.Tools.Database
{
    public class DatabaseWinRTHelpers
    {
        public static string GetWinRTDatabasePath(string filename)
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, filename);
        }
    }
}
