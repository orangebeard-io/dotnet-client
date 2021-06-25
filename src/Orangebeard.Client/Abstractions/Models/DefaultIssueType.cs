using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orangebeard.Client.Abstractions.Models
{
    public struct DefaultIssueType
    {
        public const string ProductBug = "PB001";
        public const string AutomationBug = "AB001";
        public const string SystemIssue = "SI001";
        public const string ToInvestigate = "TI001";
        public const string NotDefect = "ND001";
    }
}
