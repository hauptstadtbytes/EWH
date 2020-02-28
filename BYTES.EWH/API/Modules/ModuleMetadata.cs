//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'BYTES.NET' library
using BYTES.NET.MEF.API;

namespace BYTES.EWH.API.Modules
{
    public class ModuleMetadata : Metadata
    {

        #region "private variable(s)"

        private string _name = String.Empty;

        #endregion

        #region "public properties"

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        #endregion

        #region "public new instance method(s)"

        /// <summary>
        /// default new instance method
        /// </summary>
        public ModuleMetadata() : base(typeof(IModule))
        {
        }

        #endregion

    }
}
