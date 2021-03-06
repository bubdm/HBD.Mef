#region using

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using HBD.Framework;
using HBD.Framework.Exceptions;
using HBD.Mef.Console;
using HBD.Mef.Console.Core;
using HBD.Mef.Logging;
using HBD.Mef.Modularity;
using HBD.Mef.Shell.Configuration;
using HBD.Mef.Catalogs;

#endregion

namespace HBD.App.Shell
{
    internal class Bootstrapper : MefConsoleAppBootstrapper
    {
        private IShellConfigManager ShellConfigManager { get; }

        public Bootstrapper()
        {
            ShellConfigManager = new ShellConfigManager();
        }

        protected override void RegisterExternalObjects()
        {
            base.RegisterExternalObjects();
            Container.ComposeExportedValue(ShellConfigManager);
        }

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            Logger.Info("Add Bootstrapper Assembly");
            AggregateCatalog.Catalogs.Add(
                new MultiDirectoriesCatalog(new[] { ShellConfigManager.ShellConfig.ModulePath }, System.IO.SearchOption.AllDirectories, CreateReflectionContext()));
        }

        public override void Run(params string[] args)
        {
            try
            {
                base.Run(args);

                Console.WriteLine($"Running on Environment:{ShellConfigManager.ShellConfig.Environment}" +
                                  Environment.NewLine);
                Logger.Info($"Running on Environment:{ShellConfigManager.ShellConfig.Environment}");

                //Process the Arguments
                if (args == null || args.Length <= 0)
                {
                    Logger.Info("Parameter is empty using -module:[ModuleName] to execute the module.");
                    return;
                }
                if (args[0].StartsWithIgnoreCase("-help"))
                {
                    Console.WriteLine(@"HELP:");
                    Console.WriteLine(@"	- Parameter to execute the module: -[ModuleName] that [ModuleName].");
                    Console.WriteLine(
                        @"	- The module class must be inherited from HBD.Mef.Modularity.ConsoleModuleBase.");
                    Console.WriteLine(
                        @"	- Using [Export] attribute to export the class with a name instead of module type.");
                    Console.WriteLine(@"	- Ex: -Module1 param1 param2 -Module2 param1.");
                    Console.Read();
                    return;
                }

                foreach (var m in ParameterParser.Parse(args))
                {
                    var module = GetModuleByName(m.Name);

                    if (module == null)
                        throw new NotFoundException($"Module {m.Name} ");

                    Logger.Info($"Run module {m.Name}");
                    module.Run(m.Parameters.ToArray());
                }

                if (ShellConfigManager.ShellConfig.Environment.EqualsIgnoreCase("debug"))
                {
                    Console.WriteLine();
                    Console.WriteLine(
                        $"Running on {ShellConfigManager.ShellConfig.Environment} Environment. Please press any key to close...");
                    Console.Read();
                }
            }
            catch (Exception ex)
            {
                if (ShellConfigManager.ShellConfig.Environment.EqualsIgnoreCase("debug"))
                    throw;
                Logger?.Exception(ex);
            }
        }

        private ConsolePluginBase GetModuleByName(string moduleName)
        {
            Logger.Info($"Try to fine the module with name {moduleName}");
            var moduleInfo =
                Container.GetExports<IPlugin, IPluginExport>()
                    .FirstOrDefault(l => l.Metadata.ModuleName.EqualsIgnoreCase(moduleName))?.Value;

            if (moduleInfo == null)
                moduleInfo = Container.GetExportedValues<IPlugin>().FirstOrDefault(a => a.GetType().Name.EqualsIgnoreCase(moduleName));

            if (moduleInfo == null)
                throw new NotFoundException($"Module {moduleName}");

            Logger.Info($"Try to create module {moduleName}");
            return moduleInfo as ConsolePluginBase;
        }
    }
}