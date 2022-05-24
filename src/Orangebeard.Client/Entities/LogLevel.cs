using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orangebeard.Client.Entities
{
    public enum LogLevel
    {
        //TODO?~ Determine which ordering to use...

        // This is the ordering in the old .NET Client:
        trace, debug, info, warn, error, fatal, unknown
        // Ordering in Java client:
        //    error, warn, info, debug, trace, fatal, unknown
    }
}
