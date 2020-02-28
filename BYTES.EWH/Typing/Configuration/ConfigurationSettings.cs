//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;

namespace BYTES.EWH.Typing.Configuration
{

    [Serializable]
    [XmlRoot("ConfigurationSettings")]
    public class ConfigurationSettings
    {

        #region public variable(s)

        public enum Types
        {
            Service,
            Properties,
            LogAppender,
            StaticContent
        }

        #endregion

        #region private variable(s)

        private ConfigurationParameters _params = new ConfigurationParameters();

        #endregion

        #region public properties

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Type")]
        public ConfigurationSettings.Types Type { get; set; }

        public ConfigurationParameters Parameters
        {
            get
            {
                return _params;
            }
            set
            {
                _params = value;
            }
        }

        #endregion

        #region public new instance method(s)

        /// <summary>
        /// default new instance method
        /// </summary>
        public ConfigurationSettings()
        {
        }

        #endregion

        #region public method(s)

        public bool TryGetParameter(string key, ref string val)
        {

            //check for the item by key
            if(this.Parameters.ContainsKey(key))
            {

                val = this.Parameters[key];
                return true;

            }

            //return the default output
            return false;

        }

        #endregion

    }

}
