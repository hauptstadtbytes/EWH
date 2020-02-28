//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import internal namespace(s) required
using BYTES.EWH.Typing.Context;

namespace BYTES.EWH.Typing
{
    public static class Instance
    {

        #region public properties(s)

        public static HostContext Host { get; set; }

        public static InfrastructureContext Infrastructure { get; set; }

        #endregion



    }
}
