using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace ClassLibraryEstudios
{
    public static class cLog
    {
        public static readonly log4net.ILog
         log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
