using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace HBD.Mef.Modularity
{
    [Serializable]
    [DebuggerDisplay("ModuleType = {ModuleType}")]
    public class PluginInfo
    {
        public string ModuleName { get; internal protected set; }

        public Type ModuleType { get; internal protected set; }

        public Collection<string> DependsOn { get;internal protected set; }

        public PluginState State { get;internal protected set; }

        public PluginInfo()
          : this(null, null, new string[0])
        {
        }

        public PluginInfo(string name, Type type, params string[] dependsOn)
        {
            if (dependsOn == null)
                throw new ArgumentNullException(nameof(dependsOn));
            ModuleName = name;
            ModuleType = type;
            DependsOn = new Collection<string>();
            foreach (string str in dependsOn)
                DependsOn.Add(str);
        }

        public PluginInfo(string name, Type type)
          : this(name, type, new string[0])
        {
        }
    }
}
