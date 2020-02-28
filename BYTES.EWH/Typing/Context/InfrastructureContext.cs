//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'BYTES.NET' library
using BYTES.NET.MEF;

using BYTES.NET.IO;

using BYTES.NET.Logging.API;

//import namespace(s) required from 'Nancy' framework
using Nancy;

//import internal namespace(s) required
using BYTES.EWH.API.Modules;
using BYTES.EWH.API.Services;

using BYTES.EWH.Typing.Configuration;

namespace BYTES.EWH.Typing.Context
{
    public class InfrastructureContext
    {

        #region private variable(s)

        private HostSettings _settings = null;
        private Dictionary<string, string> _envVars = new Dictionary<string, string>();

        private ExtendedExtensionsManager<IModule, IModuleMetadata> _modules = null;
        private INancyModule[] _moduleInstances = { };

        private ExtendedExtensionsManager<IService, IServiceMetadata> _services = null;
        private Dictionary<string, IService> _serviceInstances = new Dictionary<string, IService>(StringComparer.OrdinalIgnoreCase);

        private ExtendedExtensionsManager<ILogAppender, ILogAppenderMetadata> _logAppenders = null;

        #endregion

        #region public properties

        public HostSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public string RootPath
        {
            get
            {
                return Helper.GetAppDirPath();
            }
        }

        public string ApplicationPath
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().Location;
            }
        }

        public Lazy<IModule, IModuleMetadata>[] ModuleExtensions
        {
            get
            {
                return _modules.Extensions;
            }
        }

        public Lazy<IService, IServiceMetadata>[] ServiceExtensions
        {
            get
            {
                return _services.Extensions;
            }
        }

        public Lazy<ILogAppender,ILogAppenderMetadata>[] LogAppenderExtensions
        {
            get
            {
                return _logAppenders.Extensions;
            }
        }

        public INancyModule[] ModulesLoaded
        {
            get
            {
                return _moduleInstances;
            }
            set
            {
                _moduleInstances = value;
            }
        }

        #endregion

        #region public new instance method(s)

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <param name="settings"></param>
        public InfrastructureContext(HostSettings settings)
        {

            //set the variable(s)
            _settings = settings;

            //initialize the data model
            Initialize();

        }

        #endregion

        #region public method(s)

        /// <summary>
        /// method parsing a path given by expanding the "evironment variables" used
        /// </summary>
        /// <param name="path"></param>
        /// <param name="variables"></param>
        /// <returns></returns>
        public string ParsePath(string path, Dictionary<string, string> variables = null)
        {

            //parse the arguments given
            if(variables == null)
            {
                variables = _envVars;
            }
            else
            {
                foreach(KeyValuePair<string,string> pair in _envVars)
                {
                    variables.Add(pair.Key, pair.Value);
                }
            }

            //return the output value
            return Helper.ExpandPath(path, _envVars);

        }

        public bool TryGetExtension<t>(string ID, ref t instance)
        {

            //check for a module
            if(typeof(t).Equals(typeof(Lazy<IModule, IModuleMetadata>)))
            {

                foreach (Lazy<IModule,IModuleMetadata> extension in this.ModuleExtensions)
                {

                    if (extension.Metadata.ID.Equals(ID, StringComparison.InvariantCultureIgnoreCase))
                    {
                        instance = (t)Convert.ChangeType(extension, typeof(t));
                        return true;
                    }

                }

            }

            //check for a service
            if (typeof(t).Equals(typeof(Lazy<IService, IServiceMetadata>)))
            {

                foreach (Lazy<IService, IServiceMetadata> extension in this.ServiceExtensions)
                {

                    if (extension.Metadata.ID.Equals(ID,StringComparison.InvariantCultureIgnoreCase))
                    {
                        instance = (t)Convert.ChangeType(extension, typeof(t));
                        return true;
                    }

                }

            }

            //check for a log appender
            if (typeof(t).Equals(typeof(Lazy<ILogAppender, ILogAppenderMetadata>)))
            {

                foreach (Lazy<ILogAppender, ILogAppenderMetadata> extension in this.LogAppenderExtensions)
                {

                    if (extension.Metadata.ID.Equals(ID, StringComparison.InvariantCultureIgnoreCase))
                    {

                        instance = (t)Convert.ChangeType(extension, typeof(t));
                        return true;
                    }

                }

            }

            //return the default output value
            return false;

        }

        /// <summary>
        /// method trying to get a service instance by ID
        /// </summary>
        /// <param name="name"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public bool TryGetService(string name, ref IService service)
        {

            //return the service instance (if found)
            if (_serviceInstances.ContainsKey(name))
            {

                service = _serviceInstances[name];
                return true;

            }

            //return the default output value
            return false;

        }

        /// <summary>
        /// overloaded method trying to get a typed service instance by ID
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="ID"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public bool TryGetService<t>(string name, ref t service)
        {

            IService result = null;

            if (TryGetService(name, ref result))
            {

                if (result.GetType().Equals(typeof(t)))
                {

                    service = (t)Convert.ChangeType(result, typeof(t));

                    return true;

                }

            }

            //return the default output value
            return false;

        }

        #endregion

        #region private method(s)

        /// <summary>
        /// method initializing the model
        /// </summary>
        private void Initialize()
        {

            //update the list of local "environment variable(s)"
            _envVars.Add("%BYTES.EWH%", this.ApplicationPath);

            //update the extensions
            UpdateExtensions();

            //update the service instance(s)
            UpdateServices();

        }

        /// <summary>
        /// method updating the MEF-based extensions lists
        /// </summary>
        private void UpdateExtensions()
        {

            //initialize the extensions manager(s)
            _modules = new ExtendedExtensionsManager<IModule, IModuleMetadata>();
            _services = new ExtendedExtensionsManager<IService, IServiceMetadata>();
            _logAppenders = new ExtendedExtensionsManager<ILogAppender, ILogAppenderMetadata>();

            //update the extensions list(s)
            _modules.Update(_settings.Extensions, _envVars);
            _services.Update(_settings.Extensions, _envVars);
            _logAppenders.Update(_settings.Extensions, _envVars);

        }

        /// <summary>
        /// method instanciating the services configured
        /// </summary>
        private void UpdateServices()
        {

            //clear the instances list
            _serviceInstances = new Dictionary<string, IService>(StringComparer.OrdinalIgnoreCase);

            foreach(ConfigurationSettings config in _settings.GetConfiguration(ConfigurationSettings.Types.Service))
            {

                //check for the 'Extension' parameter
                string extID = null;

                if (!config.TryGetParameter("Extension", ref extID))
                {

                    Instance.Host.Warn("Failed to instanciate '" + config.Name + "' service: 'Extension' parameter missing");
                    break;

                }

                //check for the extension instance
                Lazy<IService, IServiceMetadata> extInstance = null;

                if (!TryGetExtension<Lazy<IService, IServiceMetadata>>(extID, ref extInstance))
                {

                    Instance.Host.Warn("Failed to instanciate '" + config.Name + "' service: Extension '" + extID + "' unknown");
                    break;


                }

                //try to initialize the service
                try
                {

                    extInstance.Value.Initialize(config);
                    _serviceInstances.Add(config.Name, extInstance.Value);

                    Instance.Host.Inform("Service '" + config.Name + "' instanciated", true);

                }catch(Exception ex)
                {

                    Instance.Host.Warn("Failed to instanciate '" + config.Name + "' service: " + ex.Message, true,ex);

                }

            }

        }

        #endregion

    }

}
