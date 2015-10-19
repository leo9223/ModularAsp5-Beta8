using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ModularAsp5_Beta8.Infrastructure
{
    public class ModuleAssemblyLocator
    {
        public ModuleAssemblyLocator(IEnumerable<Assembly> moduleAssemblies)
        {
            ModuleAssemblies = moduleAssemblies;
        }

        public IEnumerable<Assembly> ModuleAssemblies { get; private set; }
    }
}
