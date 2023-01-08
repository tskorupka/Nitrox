﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using NitroxModel.Discovery;
using NitroxModel.Helper;
using System.Runtime.Versioning;

namespace Nitrox.BuildTool
{
    /// <summary>
    ///     Entry point of the build automation project.
    ///     1. Search for Subnautica install.
    ///     2. Publicize the .NET dlls and persist for subsequent Nitrox builds.
    /// </summary>
    public static class Program
    {
        private static readonly Lazy<string> processDir =
            new(() => Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location ?? Directory.GetCurrentDirectory()));

        public static string ProcessDir => processDir.Value;

        public static string GeneratedOutputDir => Path.Combine(ProcessDir, "generated_files");

        public static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(eventArgs.ExceptionObject);
                Console.ResetColor();

                Exit((eventArgs.ExceptionObject as Exception)?.HResult ?? 1);
            };

            GameInstallData game = await Task.Factory.StartNew(EnsureGame).ConfigureAwait(false);
            Console.WriteLine($"Found game at {game.InstallDir}");
            await EnsurePublicizedAssembliesAsync(game).ConfigureAwait(false);

            Exit();
        }

        private static void Exit(int exitCode = 0)
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue . . .");
            Console.ReadKey(true);
            Environment.Exit(exitCode);
        }

        private static GameInstallData EnsureGame()
        {
            string cacheFile = Path.Combine(GeneratedOutputDir, "game.props");
            if (GameInstallData.TryFrom(cacheFile, out GameInstallData game))
            {
                // Retry if the saved path is invalid
                if (false == Directory.Exists(game.InstallDir))
                {
                    game = new GameInstallData(NitroxUser.GamePath);
                }

                if (string.IsNullOrWhiteSpace(game.InstallDir))
                {
                    throw new UnableToValidateUnityGameException($"Path to game is not found: '{game.InstallDir}'");
                }

                if (false == File.Exists(Path.Combine(game.InstallDir, GetUnityPlayerFileName())))
                {
                    throw new UnableToValidateUnityGameException($"Game at: '{game.InstallDir}' is not a Unity game, did not find {GetUnityPlayerFileName()} in it.");
                }

                if (false == Directory.Exists(game.ManagedDllsDir))
                {
                    throw new UnableToValidateUnityGameException($"Invalid Unity managed DLLs directory: {game.ManagedDllsDir}");
                }
            }

            game ??= new GameInstallData(NitroxUser.GamePath);
            game.TrySave(cacheFile);
            return game;
        }

        private static async Task EnsurePublicizedAssembliesAsync(GameInstallData game)
        {
            static void LogReceived(object sender, string message) => Console.WriteLine(message);

            if (Directory.Exists(Path.Combine(GeneratedOutputDir, "publicized_assemblies")))
            {
                Console.WriteLine("Assemblies are already publicized.");
                return;
            }

            string[] dllsToPublicize = Directory.GetFiles(game.ManagedDllsDir, "Assembly-*.dll");
            Publicizer.LogReceived += LogReceived;
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                await Publicizer.PublicizeAsync(dllsToPublicize, "", Path.Combine(GeneratedOutputDir, "publicized_assemblies"));
            }
            catch (Exception)
            {
                sw.Stop();
                Publicizer.LogReceived -= LogReceived;
                throw;
            }
            Console.WriteLine($"Publicized {dllsToPublicize.Length} DLL(s) in {Math.Round(sw.Elapsed.TotalSeconds, 2)}s");
        }

        private static String GetUnityPlayerFileName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                return "UnityPlayer.dylib";
            }

            return "UnityPlayer.dll";
        }
    }
}
