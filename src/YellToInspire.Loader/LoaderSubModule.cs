using Bannerlord.ButterLib.Common.Extensions;
using Bannerlord.ButterLib.SubModuleWrappers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System.Linq;

namespace YellToInspire.Loader
{
    public sealed class LoaderSubModule : MBSubModuleBaseListWrapper
    {
        private bool ServiceRegistrationWasCalled { get; set; }

        public override void OnServiceRegistration()
        {
            ServiceRegistrationWasCalled = true;

            var logger = this.GetTempServiceProvider()?.GetRequiredService<ILogger<LoaderSubModule>>() ?? NullLogger<LoaderSubModule>.Instance;
            SubModules.AddRange(LoaderHelper.LoadAllImplementations(logger, "YellToInspire.*.dll").Select(x => new MBSubModuleBaseWrapper(x)).ToList());

            base.OnServiceRegistration();
        }

        protected override void OnSubModuleLoad()
        {
            if (!ServiceRegistrationWasCalled)
            {
                var logger = this.GetTempServiceProvider()?.GetRequiredService<ILogger<LoaderSubModule>>() ?? NullLogger<LoaderSubModule>.Instance;
                SubModules.AddRange(LoaderHelper.LoadAllImplementations(logger, "YellToInspire.*.dll").Select(x => new MBSubModuleBaseWrapper(x)).ToList());
            }

            base.OnSubModuleLoad();
        }
    }
}
