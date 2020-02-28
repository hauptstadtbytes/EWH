//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import internal namespae(s) required
using BYTES.EWH.Typing.Configuration;

namespace BYTES.EWH.API.Services
{
    public interface IService
    {

        /// <summary>
        /// method returning a new instance of the implementing class
        /// </summary>
        /// <param name="context"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        void Initialize(ConfigurationSettings config);

    }
}
