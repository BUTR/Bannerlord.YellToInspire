using Bannerlord.ButterLib.Common.Extensions;

using HarmonyLib;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System.Linq;

using YellToInspire.Loader.SubModuleWrappers;
using YellToInspire.Loader.SubModuleWrappers.Patches;

namespace YellToInspire.Loader
{
    public sealed class LoaderSubModule : MBSubModuleBaseListWrapper
    {
        private readonly Harmony _harmonyWrappers = new("YellToInspire.Loader.SubModuleWrappers");

        private bool ServiceRegistrationWasCalled { get; set; }

        public override void OnServiceRegistration()
        {
            ServiceRegistrationWasCalled = true;

            _ = MBSubModuleBasePatch.Enable(_harmonyWrappers);

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
