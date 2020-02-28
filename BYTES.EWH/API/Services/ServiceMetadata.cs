//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'BYTES.NET' library
using BYTES.NET.MEF.API;

namespace BYTES.EWH.API.Services
{
    public class ServiceMetadata : Metadata
    {

        #region "public new instance method(s)"

        /// <summary>
        /// default new instance method
        /// </summary>
        public ServiceMetadata() : base(typeof(IService))
        {
        }

        #endregion

    }
}
