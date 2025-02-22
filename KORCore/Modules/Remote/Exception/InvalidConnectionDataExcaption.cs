using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Modules.Remote.Exception
{
    public class InvalidConnectionDataExcaption : System.Exception
    {
        public InvalidConnectionDataExcaption() : base("Invalid Connection Data") { }
    }
}
