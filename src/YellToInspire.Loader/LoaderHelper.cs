using Bannerlord.BUTR.Shared.Helpers;
using Bannerlord.ModuleManager;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

using AccessTools2 = HarmonyLib.BUTR.Extensions.AccessTools2;

namespace YellToInspire.Loader
{
    internal record ImplementationFile(FileInfo Implementation, ApplicationVersion Version);

    internal static class LoaderHelper
    {
        private delegate MBSubModuleBase ConstructorDelegate();

        private static ApplicationVersion? GameVersion() => ApplicationVersion.TryParse(ApplicationVersionHelper.GameVersionStr(), out var v) ? v : null;

        public static IEnumerable<MBSubModuleBase> LoadAllImplementations(ILogger? logger, string filterWildcard)
        {
            logger?.LogInformation("Loading implementations...");

            var implementationAssemblies = new List<Assembly>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).ToList();

            var thisAssembly = typeof(LoaderSubModule).Assembly;

            var assemblyFile = new FileInfo(thisAssembly.Location);
            if (!assemblyFile.Exists)
            {
                logger?.LogError("Assembly file does not exists!");
                yield break;
            }

            var assemblyDirectory = assemblyFile.Directory;
            if (assemblyDirectory?.Exists != true)
            {
                logger?.LogError("Assembly directory does not exists!");
                yield break;
            }

            var implementations = assemblyDirectory.GetFiles(filterWildcard);
            if (implementations.Length == 0)
            {
                logger?.LogError("No implementations found.");
                yield break;
            }

            var gameVersion = GameVersion();
            if (gameVersion is null)
            {
                logger?.LogError("Failed to get Game version!");
                yield break;
            }


            var implementationsFiles = implementations.Where(x => assemblies.All(a => Path.GetFileNameWithoutExtension(a.Location) != Path.GetFileNameWithoutExtension(x.Name)));
            var implementationsWithVersions = GetImplementations(implementationsFiles, logger).ToList();
            if (implementationsWithVersions.Count == 0)
            {
                logger?.LogError("No compatible implementations were found!");
                yield break;
            }

            var implementationsForGameVersion = ImplementationForGameVersion(gameVersion, implementationsWithVersions).ToList();
            switch (implementationsForGameVersion.Count)
            {
                case > 1:
                {
                    logger?.LogInformation("Found multiple matching implementations:");
                    foreach (var (implementation1, version1) in implementationsForGameVersion)
                        logger?.LogInformation("Implementation {name} for game {gameVersion}.", implementation1.Name, version1);


                    logger?.LogInformation("Loading the latest available.");

                    var (implementation, version) = ImplementationLatest(implementationsForGameVersion);
                    logger?.LogInformation("Implementation {name} for game {gameVersion} is loaded.", implementation.Name, version);
                    implementationAssemblies.Add(Assembly.LoadFrom(implementation.FullName));
                    break;
                }

                case 1:
                {
                    logger?.LogInformation("Found matching implementation. Loading it.");

                    var (implementation, version) = implementationsForGameVersion[0];
                    logger?.LogInformation("Implementation {name} for game {gameVersion} is loaded.", implementation.Name, version);
                    implementationAssemblies.Add(Assembly.LoadFrom(implementation.FullName));
                    break;
                }

                case 0:
                {
                    logger?.LogInformation("Found no matching implementations. Loading the latest available.");

                    var (implementation, version) = ImplementationLatest(implementationsWithVersions);
                    logger?.LogInformation("Implementation {name} for game {gameVersion} is loaded.", implementation.Name, version);
                    implementationAssemblies.Add(Assembly.LoadFrom(implementation.FullName));
                    break;
                }
            }

            var subModules = implementationAssemblies.SelectMany(a =>
            {
                try
                {
                    return AccessTools2.GetTypesFromAssembly(a).Where(t => typeof(MBSubModuleBase).IsAssignableFrom(t));
                }
                catch (ReflectionTypeLoadException e)
                {
                    logger?.LogError(e, "Implementation {name} is not compatible with the current game!", Path.GetFileName(a.Location));
                    return e.Types.Where(t => typeof(MBSubModuleBase).IsAssignableFrom(t));
                }

            }).ToList();

            if (subModules.Count == 0)
                logger?.LogError("No implementation was initialized!");

            foreach (var subModuleType in subModules)
            {
                var constructor = AccessTools2.Constructor(subModuleType, Type.EmptyTypes);
                if (constructor is null)
                {
                    logger?.LogError("SubModule {subModuleType} is missing a default constructor!", subModuleType);
                    continue;
                }

                var constructorFunc = AccessTools2.GetDelegate<ConstructorDelegate>(constructor);
                if (constructorFunc is null)
                {
                    logger?.LogError("SubModule {subModuleType}'s default constructor could not be converted to a delegate!", subModuleType);
                    continue;
                }

                yield return constructorFunc();
            }

            logger?.LogInformation("Finished loading implementations.");
        }

        private static IEnumerable<ImplementationFile> GetImplementations(IEnumerable<FileInfo> implementations, ILogger? logger = null)
        {
            foreach (var implementation in implementations)
            {
                bool found = false;
                logger?.LogInformation("Found implementation {name}.", implementation.Name);

                using var fs = File.OpenRead(implementation.FullName);
                using var peReader = new PEReader(fs);
                var mdReader = peReader.GetMetadataReader(MetadataReaderOptions.None);
                foreach (var attr in mdReader.GetAssemblyDefinition().GetCustomAttributes().Select(ah => mdReader.GetCustomAttribute(ah)))
                {
                    var ctorHandle = attr.Constructor;
                    if (ctorHandle.Kind != HandleKind.MemberReference) continue;

                    var container = mdReader.GetMemberReference((MemberReferenceHandle) ctorHandle).Parent;
                    var name = mdReader.GetTypeReference((TypeReferenceHandle) container).Name;
                    if (!string.Equals(mdReader.GetString(name), "AssemblyMetadataAttribute", StringComparison.Ordinal)) continue;

                    var attributeReader = mdReader.GetBlobReader(attr.Value);
                    attributeReader.ReadByte();
                    attributeReader.ReadByte();
                    var key = attributeReader.ReadSerializedString();
                    var value = attributeReader.ReadSerializedString();
                    if (string.Equals(key, "GameVersion", StringComparison.Ordinal))
                    {
                        if (!ApplicationVersion.TryParse(value, out var implementationGameVersion))
                        {
                            logger?.LogError("Implementation {name} has invalid GameVersion AssemblyMetadataAttribute!", implementation.Name);
                            continue;
                        }

                        found = true;
                        yield return new(implementation, implementationGameVersion);
                        break;
                    }
                }

                if (!found)
                    logger?.LogError("Implementation {name} is missing GameVersion AssemblyMetadataAttribute!", implementation.Name);
            }
        }

        private static IEnumerable<ImplementationFile> ImplementationForGameVersion(ApplicationVersion gameVersion, IEnumerable<ImplementationFile> implementations)
        {
            foreach (var (implementation, version) in implementations)
            {
                if (gameVersion.IsSame(version))
                {
                    yield return new(implementation, version);
                }
            }
        }
        private static ImplementationFile ImplementationLatest(IEnumerable<ImplementationFile> implementations)
        {
            return implementations.MaxBy(x => x.Version, new ApplicationVersionComparer(), out _);
        }
    }
}