//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml.Serialization;

//import namespace(s) required from 'BYTES.NET' library
using BYTES.NET.MEF;

//import internal namespae(s) required
using BYTES.EWH.Typing.Configuration;

namespace BYTES.EWH
{
    [Serializable]
    public class HostSettings
    {

        #region "private variable(s)"

        private string _address = "http://localhost";
        private int _port = 11833;

        private ExtensionsSettings[] _extensions = {};
        private ConfigurationSettings[] _configurations = { };

        #endregion

        #region public properties

        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                _address = value;
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
            }
        }

        public ExtensionsSettings[] Extensions
        {
            get
            {
                return _extensions;
            }
            set
            {
                _extensions = value;
            }
        }

        public ConfigurationSettings[] Configurations
        {
            get
            {
                return _configurations;
            }
            set
            {
                _configurations = value;
            }
        }

        #endregion

        #region public new instace method(s)

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <remarks>required for XML serialization</remarks>
        public HostSettings()
        {

            //add the default settings
            List<ExtensionsSettings> extensions = new List<ExtensionsSettings>();
            extensions.Add(new ExtensionsSettings() { Source = "%InstallationDir%\\*.dll" });

            this.Extensions = extensions.ToArray();

        }

        #endregion

        #region public method(s)

        /// <summary>
        /// method writing the data to (XML-based) disk file
        /// </summary>
        /// <param name="path"></param>
        public void Write(string path)
        {

            // check for the output directory (and create if required)
            string dirPath = path.Substring(path.LastIndexOf(@"\") + 1);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            // serialize the data to XML file
            XmlSerializer serializer = new XmlSerializer(typeof(HostSettings));
            StreamWriter writer = new StreamWriter(path);

            serializer.Serialize(writer, this);

            writer.Close();

        }

        /// <summary>
        /// method reading the data from (XML-based) disk file
        /// </summary>
        /// <param name="path"></param>
        public void Read(string path)
        {

            // read the file
            if (File.Exists(path))
            {

                // create an intermediate class instance
                HostSettings tmp;

                // de-serialize the data
                XmlSerializer serializer = new XmlSerializer(typeof(HostSettings));
                FileStream reader = new FileStream(path, FileMode.Open);

                tmp = (HostSettings)serializer.Deserialize(reader);

                // set current class' properties
                this.Address = tmp.Address;
                this.Port = tmp.Port;
                this.Extensions = tmp.Extensions;
                this.Configurations = tmp.Configurations;

                System.Diagnostics.Debug.WriteLine("Count Configurations: " + tmp.Configurations.Length.ToString());

                reader.Close();

            }

        }

        /// <summary>
        /// method returning all configuration items of a specific type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ConfigurationSettings[] GetConfiguration(ConfigurationSettings.Types type)
        {

            List<ConfigurationSettings> output = new List<ConfigurationSettings>();

            foreach(ConfigurationSettings config in this.Configurations)
            {

                if(config.Type == type)
                {

                    output.Add(config);

                }

            }

            return output.ToArray();

        }

        public ConfigurationSettings GetConfiguration(ConfigurationSettings.Types type, string name)
        {

            foreach (ConfigurationSettings config in this.Configurations)
            {

                if (config.Type == type)
                {

                    if(config.Name.Equals(name,StringComparison.InvariantCultureIgnoreCase))
                    {

                        return config;

                    }

                }

            }

            return null;

        }

        #endregion

    }

}
