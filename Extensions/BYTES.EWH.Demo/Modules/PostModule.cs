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

    [ModuleMetadata(ID = "APIDemo_Post", Name = "API Demo: Post Module")]
    public class PostModule : Module
    {

        #region public new instance method(s)

        public PostModule()
        {

            //return a dictionary containing the post data on requesting '/apidemo/post'
            Post("/apidemo/post", args =>
            {

                //get the POST data
                Dictionary<string, string> postData = null;

                if (TryGetPostData<Dictionary<string, string>>(ref postData))
                {

                    //parse the POST data
                    Dictionary<string, string> returnData = new Dictionary<string, string>();

                    foreach (KeyValuePair<string, string> pair in postData)
                    {

                        returnData.Add(pair.Key, HtmlDecode(pair.Value)); //add the items, parsing HTML masks

                    }

                    //return the output value
                    return JSON(returnData);

                }

                //return the default output value
                return JSON(null,"Failed to get POST data");

            });

        }

        #endregion

    }

}
