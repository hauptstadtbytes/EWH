//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import internal namespace(s) required
using BYTES.EWH.Typing.Configuration;

namespace BYTES.EWH.API.Services
{

    public abstract class Service : IService
    {


        #region method(s) implementing the 'IService' interface

        public abstract void Initialize(ConfigurationSettings config);

        #endregion

    }

}
