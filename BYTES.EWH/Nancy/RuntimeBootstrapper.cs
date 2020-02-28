//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//import namespace(s) required from 'Nancy' framework
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using Nancy.Conventions;

//import namespace(s) required from 'BYTES.NET' library
using BYTES.NET.Logging.API;

//import internal namespace(s) required
using BYTES.EWH.Nancy.StaticContent;

using BYTES.EWH.API.Modules;

using BYTES.EWH.Typing;
using BYTES.EWH.Typing.Configuration;
using BYTES.EWH.Typing.Context;

namespace BYTES.EWH.Nancy
{
    public class RuntimeBootstrapper : DefaultNancyBootstrapper
    {

        #region private variable(s)

        private Host _host = null;

        private bool _modulesLogged = false; //variable used for preventing from multi-logging module(s) found

        #endregion

        #region public new instance method(s)

        /// <summary>
        /// default new instance method
        /// </summary>
        /// <param name="host"></param>
        public RuntimeBootstrapper(ref Host host)
        {

            //set the (private variable(s)
            _host = host;

            //update the (global) 'Instance' variable(s)
            Instance.Host = new HostContext(ref host);
            Instance.Infrastructure = new InfrastructureContext(host.Settings);

            //add the log appenders
            AddLogAppenders();

        }

        #endregion

        #region proptected method(s) overriding the base-class method(s)

        /// <summary>
        /// override the module loading
        /// </summary>
        protected override IEnumerable<ModuleRegistration> Modules
        {
            get
            {

                //create the output value
                List<ModuleRegistration> modules = new List<ModuleRegistration>();

                //add the module(s)
                foreach (Lazy<IModule, IModuleMetadata> module in Instance.Infrastructure.ModuleExtensions)
                {

                    //add the module extension found
                    modules.Add(new ModuleRegistration(module.Value.GetType()));

                    //log the extension found
                    if(_modulesLogged != true)
                    {

                        Instance.Host.Inform("Module named '" + module.Metadata.Name + "' ('" + module.Metadata.ID + "') found", true);

                    }


                }

                //set the (internal) switch
                _modulesLogged = true;

                //return the output value
                return modules;

            }

        }

        /// <summary>
        /// override the application startup method
        /// </summary>
        /// <param name="container"></param>
        /// <param name="pipelines"></param>
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {

            base.ApplicationStartup(container, pipelines);

            //enable logging for non-static (URL) requests
            //see 'https://github.com/NancyFx/Nancy/wiki/The-Application-Before,-After-and-OnError-pipelines' for more details
            pipelines.BeforeRequest += (ctx) => {

                Instance.Host.Inform("Route '" + ctx.Request.Url + "' requested");

                return null;

            };

            //enable error logging
            pipelines.OnError += (ctx, ex) => {

                Instance.Host.Warn("An error occured while requesting route '" + ctx.Request.Url + "': " + ex.Message,true,ex);

                return null;

            };

            //update the global 'Instance' variable(s)
            List<INancyModule> modules = new List<INancyModule>();

            foreach (ModuleRegistration registration in this.Modules)
            {

                //add the module to the list
                modules.Add(GetModule(container, registration.ModuleType));

            }

            Instance.Infrastructure.ModulesLoaded = modules.ToArray();

        }

        /// <summary>
        /// override the method for configuring the conventions (e.g. for static content)
        /// </summary>
        /// <param name="nancyConventions"></param>
        /// <remarks>based on the articles found at 'https://github.com/NancyFx/Nancy/wiki/Managing-static-content' and 'https://stackoverflow.com/questions/13270163/how-to-serve-static-content-in-nancy'</remarks>
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {

            foreach(ConfigurationSettings config in Instance.Infrastructure.Settings.GetConfiguration(ConfigurationSettings.Types.StaticContent))
            {

                //check for the 'Path' property
                string path = null;

                if(!config.TryGetParameter("Path",ref path))
                {

                    Instance.Host.Warn("Failed to add static content convention: 'Path' parameter missing");
                    break;

                }

                //check for the 'Route' property
                string route = null;

                if (!config.TryGetParameter("Route", ref route)) {

                    Instance.Host.Warn("Failed to add static content convention: 'Route' parameter missing");
                    break;

                }

                //add the static content convention
                if(ConventionBuilder.IsFile(path)) //add a static content file
                {

                    nancyConventions.StaticContentsConventions.Add(ConventionBuilder.AddFile(route, Instance.Infrastructure.ParsePath(path)));
                    Instance.Host.Inform("Static content file route '" + route + "' added for path '" + path + "'", true);

                } else //add a static content directory
                {

                    nancyConventions.StaticContentsConventions.Add(ConventionBuilder.AddDirectory(route, Instance.Infrastructure.ParsePath(path)));
                    Instance.Host.Inform("Static content directory route '" + route + "' added for path '" + path + "'", true);

                }

            }

            //initialize the conventions
            base.ConfigureConventions(nancyConventions);

        }

        #endregion

        #region private method(s)

        /// <summary>
        /// method adding the appenders to the host log
        /// </summary>
        private void AddLogAppenders()
        {

            foreach (ConfigurationSettings config in _host.Settings.GetConfiguration(ConfigurationSettings.Types.LogAppender)) //loop for each log appender configured
            {

                //check for the 'Extension' parameter
                string extID = null;

                if(!config.TryGetParameter("Extension",ref extID))
                {

                    Instance.Host.Warn("Failed to add log appender: 'Extension' parameter missing");
                    break;

                }

                //check for the extension instance
                Lazy<ILogAppender, ILogAppenderMetadata> extInstance = null;

                if (!Instance.Infrastructure.TryGetExtension<Lazy<ILogAppender, ILogAppenderMetadata>>(extID, ref extInstance))
                {

                    Instance.Host.Warn("Failed to add log appender: Extension '" + extID + "' unknown");
                    break;


                }

                //try to add the appender
                try
                {

                    extInstance.Value.Initialize(config.Parameters); //initialize a new appender instance (using the parameters set)
                    _host.Log.Appenders.Add(extInstance.Value); //add the appender instance

                    Instance.Host.Inform("Log appender '" + config.Name + "' added for '" + extID + "'", true);

                }catch(Exception ex)
                {

                    Instance.Host.Warn("Failed to add log appender: " + ex.Message, true, ex);

                }

            }

        }

        #endregion

    }

}