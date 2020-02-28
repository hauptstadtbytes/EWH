//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'BYTES.NET' library
using BYTES.NET.Logging;

//import internal namespace(s) required
using BYTES.EWH.Typing.Configuration;

namespace BYTES.EWH.Typing.Context
{
    public class HostContext
    {

        #region private variable(s)

        private Host _host = null;

        #endregion

        #region public properties

        public Log Log
        {
            get
            {
                return _host.Log;
            }
        }

        #endregion

        #region public new instance method(s)

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <param name="host"></param>
        public HostContext(ref Host host)
        {

            //set the variable(s)
            _host = host;

        }

        #endregion

        #region public method(s)

        /// <summary>
        /// method for adding an 'Info' level log message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isDebugging"></param>
        /// <param name="details"></param>
        public void Inform(string message, bool isDebugging, object details)
        {

            if (isDebugging)
            {

                    _host.Log.Write(message, LogEntry.InformationLevel.Debug, ref details);

            }
            else
            {

                _host.Log.Write(message, LogEntry.InformationLevel.Info, ref details);

            }


        }

        public void Inform(string message, bool isDebugging = false)
        {

            if (isDebugging)
            {

                _host.Log.Write(message, LogEntry.InformationLevel.Debug);

            }
            else
            {

                _host.Log.Write(message, LogEntry.InformationLevel.Info);

            }

        }

        /// <summary>
        /// method for adding an 'Exception' level log message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isException"></param>
        /// <param name="details"></param>
        public void Warn(string message, bool isException, object details)
        {

            if (isException)
            {

                _host.Log.Write(message, LogEntry.InformationLevel.Exception,ref details);

            }
            else
            {

                _host.Log.Write(message, LogEntry.InformationLevel.Warning, ref details);

            }

        }

        public void Warn(string message, bool isException = false)
        {

            if (isException)
            {

                _host.Log.Write(message, LogEntry.InformationLevel.Exception);

            }
            else
            {

                _host.Log.Write(message, LogEntry.InformationLevel.Warning);

            }

        }

        /// <summary>
        /// wrapper method returning the configuration setting(s)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ConfigurationSettings[] GetConfiguration(ConfigurationSettings.Types type)
        {

            return _host.Settings.GetConfiguration(type);

        }

        public ConfigurationSettings GetConfiguration(ConfigurationSettings.Types type, string name)
        {

            return _host.Settings.GetConfiguration(type,name);

        }

        #endregion

    }

}
