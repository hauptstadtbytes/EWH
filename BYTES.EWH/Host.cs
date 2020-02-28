//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'Nancy' library
using Nancy.Hosting.Self;
using Nancy.Bootstrapper;

//import namespace(s) required from 'BYTES.NET' library
using BYTES.NET.Logging;

//import internal namespace(s) required
using BYTES.EWH.Nancy;

namespace BYTES.EWH
{

    /// <summary>
    /// hosting main class
    /// </summary>
    /// <remarks>based on the article found at 'http://www.codeguru.com/columns/dotnet/creating-microservices-using-nancyfx.html'</remarks>
    public class Host
    {

        #region private variable(s)

        private HostSettings _settings = null;
        private NancyHost _host = null;

        private Log _log = new Log();

        #endregion

        #region public properties

        public HostSettings Settings
        {
            get
            {
                return _settings;
            }

        }

        public Log Log
        {
            get
            {
                return _log;
            }
            set
            {
                _log = value;
            }
        }

        #endregion

        #region public new instance method(s)

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <param name="settings"></param>
        public Host(HostSettings settings = null)
        {

            //set the variable(s)
            if (settings == null)
            {

                _settings = new HostSettings();

            }
            else
            {

                _settings = settings;

            }

        }

        #endregion

        #region public method(s)

        /// <summary>
        /// method for starting the server host/ port listensing
        /// </summary>
        /// <param name="port"></param>
        /// <param name="address"></param>
        public void Start()
        {

            //create a new ('Nancy') bootstrapper
            Host me = this;
            INancyBootstrapper bootstrapper = new RuntimeBootstrapper(ref me);

            //create a new host and start
            Uri listenTo = new Uri(_settings.Address + ":" + _settings.Port.ToString());

            _host = new NancyHost(bootstrapper, listenTo);
            _host.Start();

            //prompt a message
            this.Log.Write("Host started listening on '" + listenTo.ToString() + "'");

        }

        /// <summary>
        /// method for stopping the server host/ port listening
        /// </summary>
        public void Stop()
        {

            //stop the host
            if (_host != null)
            {

                //stop the host
                _host.Stop();
                _host = null;

                //prompt a message
                this.Log.Write("Host stopped listening");

            }
            else
            {

                //prompt a message
                this.Log.Write("No host STOP required: No active host was listening", LogEntry.InformationLevel.Debug);

            }

        }

        #endregion

    }

}
