//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

//import namespace(s) required from 'BYTES.NET' library
using BYTES.NET.Collections;

namespace BYTES.EWH.Typing.Configuration
{
    [Serializable]
    [XmlRoot("Parameters")]
    public class ConfigurationParameters : XMLSerializableDictionary<string, string>
    {

        #region public new instance method(s)

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <remarks>required to make dictionary case-insensitive</remarks>
        public ConfigurationParameters() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        #endregion

        #region public method(s)

        /// <summary>
        /// method returning the value for a named item (incl. type casting)
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>based on the article found at 'https://stackoverflow.com/questions/972636/casting-a-variable-using-a-type-variable'</remarks>
        public t Get<t>(string name)
        {

            if (this.ContainsKey(name))
            {

                try
                {

                    return (t)Convert.ChangeType(this[name], typeof(t));

                }
                catch
                {

                    if (this[name.ToLower()].GetType().Equals(typeof(string)) && typeof(t).Equals(typeof(char))) //check for a string > char conversion
                    {

                        return (t)Convert.ChangeType(this[name.ToLower()].ToString()[0], typeof(t));

                    }

                }

            }

            //return the default output
            return default(t);

        }

        #endregion

    }

}
