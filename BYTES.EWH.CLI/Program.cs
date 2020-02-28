//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//import namespace(s) required from 'BYTES.NET' library
using BYTES.NET.Logging;

using BYTES.NET.MEF;

using BYTES.NET.IO;

//import namespace(s) required from 'BYTES.EWH' library
using BYTES.EWH.Typing.Configuration;

namespace BYTES.EWH.CLI
{
    class Program
    {

        #region private variable(s)

        private static Host _host = null;

        #endregion

        #region the main method

        /// <summary>
        /// the main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            //parse the command line argument(s) and setup the host instance
            HostSettings settings = AssembleHostSettings(new HostSettings(), ParseArgs(args));
            _host = new Host(settings);

            //add an event handler for the log
            _host.Log.Logged += new Log.LoggedEventHandler(HandleLogUpdated);
            _host.Log.InformationThreshold = LogEntry.InformationLevel.Debug; //set the information level threshold to 'Debug'

            //start the host
            _host.Start();

            //prevent console from closing
            Console.ReadLine();

        }

        #endregion

        #region private method(s)

        /// <summary>
        /// method for parsing the command line argument(s)
        /// </summary>
        private static Dictionary<string, string> ParseArgs(string[] args)
        {

            Dictionary<string, string> output = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (string arg in args)
            {

                //check for the 'settings' arguments
                if (arg.ToLower().StartsWith("-settings:"))
                {

                    output.Add("settings", Helper.ExpandPath(arg.Substring(10)));

                }

                //check for the (create) 'examplesettings' argument
                if (arg.ToLower().StartsWith("-createexample"))
                {

                    output.Add("createexample", true.ToString());

                }

            }

            return output;

        }

        /// <summary>
        /// method assebmling the host settings based on the command line arguments
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static HostSettings AssembleHostSettings(HostSettings settings, Dictionary<string, string> args)
        {

            //try to read settings from disk file
            if(args.ContainsKey("settings"))
            {

                if(args.ContainsKey("createexample")) //create a set of sample settings, written to the file specified by 'settings'
                {

                    //setup the extensions
                    List<ExtensionsSettings> extensions = new List<ExtensionsSettings>();

                    extensions.Add(new ExtensionsSettings() { Source = "%InstallationDir%\\*.dll" });
                    extensions.Add(new ExtensionsSettings() { Source = "%InstallationDir%\\..\\..\\..\\Extensions\\BYTES.EWH.Demo\\bin\\Debug\\*.dll" });

                    settings.Extensions = extensions.ToArray();

                    //create a variable storing the configurations
                    List<ConfigurationSettings> configs = new List<ConfigurationSettings>();

                    //add a rolling file log appender
                    ConfigurationParameters logParams = new ConfigurationParameters();
                    logParams.Add("Extension", "BYTES_LogAppender_RollingFile");
                    logParams.Add("FilePath", "D:\\Sample.LOG");

                    configs.Add(new ConfigurationSettings() { Name = "MyLogFile", Type = ConfigurationSettings.Types.LogAppender, Parameters = logParams });

                    //add some properties
                    ConfigurationParameters propParams = new ConfigurationParameters();
                    propParams.Add("Default", "42");
                    propParams.Add("NamedItem", "Hello World!");

                    configs.Add(new ConfigurationSettings() { Name = "MyProperties", Type = ConfigurationSettings.Types.Properties, Parameters = propParams });

                    //add a static content directory
                    ConfigurationParameters staticDirParams = new ConfigurationParameters();
                    staticDirParams.Add("Path", "%InstallationDir%\\Content");
                    staticDirParams.Add("Route", "htdocs");

                    configs.Add(new ConfigurationSettings() { Name = "MyHtDocsDir", Type = ConfigurationSettings.Types.StaticContent, Parameters = staticDirParams });

                    //add a static content file
                    ConfigurationParameters staticFileParams = new ConfigurationParameters();
                    staticFileParams.Add("Path", "%InstallationDir%\\Content\\index.html");
                    staticFileParams.Add("Route", "/index.html");

                    configs.Add(new ConfigurationSettings() { Name = "MyIndexFile", Type = ConfigurationSettings.Types.StaticContent, Parameters = staticFileParams });

                    //add a service configuration
                    ConfigurationParameters serviceParams = new ConfigurationParameters();
                    serviceParams.Add("Extension", "APIDemo_AnswerService");
                    serviceParams.Add("TheAnswer", "42");

                    configs.Add(new ConfigurationSettings() { Name = "MyAnswerService", Type = ConfigurationSettings.Types.Service, Parameters = serviceParams });

                    //set the configurations
                    settings.Configurations = configs.ToArray();

                    //write the settings file
                    try
                    {

                        settings.Write(args["settings"]);
                        Prompt("Settings written to '" + args["settings"] + "'");

                    }
                    catch (Exception ex)
                    {

                        Prompt("Failed to write example settings to '" + args["settings"] + "': " + ex.Message, LogEntry.InformationLevel.Exception);

                    }

                } else //try to load settings from file
                {

                    if (File.Exists(args["settings"]))
                    {

                        settings.Read(args["settings"]);
                        Prompt("Settings loaded from '" + args["settings"] + "'", LogEntry.InformationLevel.Debug);

                    }
                    else //no settings file was found
                    {

                        Prompt("No settings file found at '" + args["settings"] + "'; default settings will be applied", LogEntry.InformationLevel.Warning);

                    }

                }

            } else
            {

                Prompt("No settings (file path) specified; default settings will be applied", LogEntry.InformationLevel.Debug);

            }

            //return the output value
            return settings;

        }

        /// <summary>
        /// handler for host log updates
        /// </summary>
        /// <param name="entry"></param>
        private static void HandleLogUpdated(ref LogEntry entry)
        {

            string message = entry.TimeStamp.ToString("yyyy-MM-dd HH.mm.ss") + " | " + ResizeString(entry.Level.ToString(), 10) + " | " + entry.Message;

            if (!(entry.Details == null))
            {

                if (entry.Details.GetType().Equals(typeof(Exception)))
                {

                    Exception ex = (Exception)entry.Details;

                    message += Environment.NewLine + ex.StackTrace.ToString();

                }

            }

            Console.WriteLine(message);

        }

        /// <summary>
        /// method for prompting a message to the CLI
        /// </summary>
        /// <param name="message"></param>
        /// <param name="msgType"></param>
        private static void Prompt(string message, LogEntry.InformationLevel level = LogEntry.InformationLevel.Info)
        {

            if(_host != null)
            {

                _host.Log.Write(message, level);

            } else
            {

                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss") + " | " + ResizeString(level.ToString(), 10) + " | " + message);

            }

        }

        /// <summary>
        /// method filling up a string with whitespaces to opitimize presentation style
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string ResizeString(string value, int length)
        {

            string output = value;

            while (output.Length < length)
            {

                output += " ";

            }

            return output;

        }

        #endregion

    }

}
