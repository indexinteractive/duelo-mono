namespace Duelo.Build
{
    using UnityEditor;
    using UnityEngine;
    using System.Reflection;
    using Microsoft.Extensions.Configuration;

    public class BuildScript
    {
        public static void BuildServer()
        {
            IConfiguration args = new ConfigurationBuilder()
                .AddCommandLine(System.Environment.GetCommandLineArgs())
                .Build();

            var buildFolder = args["buildFolder"] ?? "Builds";
            var appName = args["appName"] ?? "duelo-server";
            var appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            string outputPath = $"{buildFolder}/{appName}-{appVersion}";

            Debug.Log("[BuildScript] Building server to: " + outputPath);

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] {
                    "Assets/_duelo/01_scenes/ServerMain.unity"
                },
                locationPathName = outputPath,
                target = BuildTarget.StandaloneLinux64,
                options = BuildOptions.None,
                subtarget = (int)StandaloneBuildSubtarget.Server
            };

            BuildPipeline.BuildPlayer(buildPlayerOptions);
        }
    }
}