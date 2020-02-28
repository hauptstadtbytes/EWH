//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

//import namespace(s) from 'Nancy' library required
using Nancy;
using Nancy.Extensions;
using Nancy.Helpers;

//import internal namespace(s) required
using BYTES.EWH.Nancy;

namespace BYTES.EWH.API.Modules
{
    public abstract class Module : NancyModule, IModule
    {

        /// <summary>
        /// method enabling CORS
        /// </summary>
        /// <remarks>to be called at the very beginning of the constructor method</remarks>
        protected void EnableCORS()
        {

            this.After.AddItemToEndOfPipeline(ctx =>
            {
                ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                    .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                    .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");

            });

        }

        #region protected method(s)

        /// <summary>
        /// method trying to get the post data
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <returns></returns>
        protected bool TryGetPostData<t>(ref t value)
        {

            try
            {

                //get the body string
                string data = this.Request.Body.AsString();

                //return a 'string' type value
                if (typeof(string).Equals(typeof(t)))
                {

                    value = (t)Convert.ChangeType(data, typeof(t));
                    return true;

                }

                //try to create a dictionary
                if (typeof(Dictionary<string, string>).Equals(typeof(t)))
                {

                    //try to parse a 'x-www-form-urlencoded' body
                    Regex rx1 = new Regex("([\\d\\D]+=[\\d\\D]+)&?");

                    foreach (Match myMatch in rx1.Matches(data))
                    {
                        if (myMatch.Success)
                        {

                            Dictionary<string, string> output = new Dictionary<string, string>();

                            foreach (string statement in data.Split('&'))
                            {


                                string[] tmp = statement.Split('=');


                                switch (tmp.Length)
                                {

                                    case 1:
                                        output.Add(tmp[0].ToLower(), String.Empty);
                                        break;

                                    case 2:
                                        output.Add(tmp[0].ToLower(), tmp[1]);
                                        break;

                                }

                            }

                            value = (t)Convert.ChangeType(output, typeof(t));
                            return true;

                        }

                    }

                }

                //try to return "any" type
                value = (t)Convert.ChangeType(data, typeof(t));
                return true;

            }
            catch
            {

                return false;

            }

        }

        /// <summary>
        /// method decoding a string from HTML masks
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected string HtmlDecode(string text)
        {

            return HttpUtility.UrlDecode(text);

        }

        /// <summary>
        /// method assemblying a file response
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        protected Response File(string path, string dataType = "text/html")
        {

            return Response.AsFile(path, dataType);

        }

        /// <summary>
        /// method assemblying a JSON string response
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected Response JSON(object data, string error = null)
        {

            GenericResponse response = new GenericResponse(data, error);

            return Response.AsJson(response);

        }

        #endregion

    }

}
