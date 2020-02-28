//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'BYTES.EWH' library
using BYTES.EWH.API.Services;
using BYTES.EWH.Typing.Configuration;

namespace BYTES.EWH.Demo.Services
{

    [ServiceMetadata(ID = "APIDemo_AnswerService")]
    public class AnswerService : Service
    {

        #region private variable(s)

        private ConfigurationSettings _settings = null;

        #endregion

        #region public method(s) implementing 'IService'

        /// <summary>
        /// method for initializing the service
        /// </summary>
        /// <param name="config"></param>
        public override void Initialize(ConfigurationSettings config)
        {

            //set the variable(s)
            _settings = config;

        }

        #endregion

        #region public method(s)

        /// <summary>
        /// demo method returning the answer
        /// </summary>
        /// <returns></returns>
        public string GetAnswer()
        {

            string output = null;

            if(!_settings.TryGetParameter("TheAnswer",ref output))
            {

                return "Unable to get the answer. I have to think again once more.";

            }

            return output;

        }

        #endregion

    }
}
