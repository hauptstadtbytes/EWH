//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'BYTES.EWH' library
using BYTES.EWH.API.Modules;

using BYTES.EWH.Typing.Configuration;

namespace BYTES.EWH.Demo.Modules
{

    [ModuleMetadata(ID = "APIDemo_Get", Name = "API Demo: Get Module")]
    public class GetModule : API.Modules.Module
    {

        #region public new instance method(s)

        public GetModule()
        {

            //return a simple message response upon requesting '/apidemo/{message?}'
            Get("/apidemo/{message?}", args =>
            {

                //parse the message
                string msg = "Hello World!";

                if (args.message != null)
                {

                    msg = args.message;

                }

                //return the output value
                return msg;

            });

            Get("/apidemo/log/{message?}", args =>
            {

                //parse the message
                string msg = "A sample log message via API demo";

                if (args.message != null)
                {

                    msg = HtmlDecode(args.message);

                }

                //log the message
                string output = "Message logged successfully";

                try
                {

                    //log the message
                    Typing.Instance.Host.Inform(msg);

                }
                catch(Exception ex)
                {

                    //log the issue
                    Typing.Instance.Host.Warn("Failed to log message '" + msg + "': " + ex.Message,false,ex);

                    //set the output
                    output = ex.Message;

                }

                //return the output value
                return JSON(output);

            });

            //return a named property from 'APIDemo_Properties' properties group in the application configuration 
            Get("/apidemo/properties/{name?}", args =>
            {

                //parse the property name
                string name = "Default";

                if (args.name != null)
                {

                    name = HtmlDecode(args.name);

                }

                //get the properties from configuration
                ConfigurationSettings config = Typing.Instance.Host.GetConfiguration(ConfigurationSettings.Types.Properties, "MyProperties");

                if(config == null)
                {

                    return JSON(null, "Failed to retrieve value for '" + name + "': Unable to access 'MyProperties'");

                }

                //return the output value
                string output = null;

                if (!config.TryGetParameter(name,ref output))
                {

                    return JSON(null, "Failed to retrieve value for '" + name + "': Property unknown");

                }

                return JSON(output);

            });

            //return the the answer by respective service
            Get("/apidemo/answer", args =>
            {

                Services.AnswerService srv = null;

                if (!Typing.Instance.Infrastructure.TryGetService<Services.AnswerService>("MyAnswerService",ref srv)){


                    return JSON(null,"Failed to get the service instance");
                }

                return JSON(srv.GetAnswer());

            });

        }

        #endregion

    }

}
