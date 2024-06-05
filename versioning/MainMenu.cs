using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace versioning
{
    internal class MainMenu
    {
        private readonly Shell shell;

        private readonly Argument<string> versionArgument;
        private readonly Option<bool> canGenerateCmdOption;

        public MainMenu()
        {
            this.shell = new Shell();

            this.versionArgument = new Argument<string>("version", () => "1.0.0.0", $"Build version.");
            this.canGenerateCmdOption = new Option<bool>(new[] { "-c", "--cmd" }, () => false, $"Generate nuget command files.");
        }

        public int Run(string[] args, string title)
        {
            RootCommand rootCommand = new RootCommand(title)
            {
                SolutionCommand(),
                ProjectCommand(),
                UpdatePackageCommand(),
            };

            int exit = rootCommand.InvokeAsync(args).Result;
            return exit;
        }

        private Command SolutionCommand()
        {
            Option<string> repoOption = new Option<string>(new[] { "-r", "--repo" }, () => Directory.GetCurrentDirectory(), $"Build repository directory.");
            Option<string> envFileOption = new Option<string>(new[] { "-e", "--env" }, () => "vervs.cmd", $"Build environment variable file name. e.g vervs.cmd or vervs.ps1");

            var cmd = new Command("update-repo", "Update version for all projects in repository.")
            {
                versionArgument,
                repoOption,
                envFileOption,
                canGenerateCmdOption,
            };

            cmd.SetHandler(shell.UpdateRepo,
               versionArgument,
               repoOption,
               envFileOption,
               canGenerateCmdOption
            );

            return cmd;
        }

        private Command ProjectCommand()
        {
            string directory = Directory.GetCurrentDirectory();

            string repo;
            string projectName;

            if (Shell.IsRepoDirectory(directory))
            {
                repo = directory;
                projectName = Shell.UNDEFINED;
            }
            else
            {
                repo = Path.GetFullPath(Path.Combine(directory, ".."));
                if (Shell.IsRepoDirectory(repo))
                {
                    projectName = Path.GetFileName(directory);
                }
                else
                {
                    repo = Shell.UNDEFINED;
                    projectName = Shell.UNDEFINED;
                }
            }

            Option<string> repoOption = new Option<string>(new[] { "-r", "--repo" }, () => repo, $"Build repository directory.");
            Option<string> projectOption = new Option<string>(new[] { "-p", "--project" }, () => projectName, $"Name of project.");

            var cmd = new Command("update-project", "Update version in a single project.")
            {
                versionArgument,
                repoOption,
                projectOption,
                canGenerateCmdOption,
            };

            cmd.SetHandler(shell.UpdateProject,
               versionArgument,
               repoOption,
               projectOption,
               canGenerateCmdOption
               );

            return cmd;
        }

        private Command UpdatePackageCommand()
        {
            string directory = Directory.GetCurrentDirectory();

            Argument<string> packageArgument = new Argument<string>("package-id", $"Package Id.");
            Option<string> repoOption = new Option<string>(new[] { "-r", "--repo" }, () => directory, $"Build repository directory.");

            var cmd = new Command("update-package", "Update package version in .nuspec files.")
            {
                packageArgument,
                versionArgument,
                repoOption,
            };

            cmd.SetHandler(shell.UpdatePackage,
               packageArgument,
               versionArgument,
               repoOption
               );

            return cmd;
        }
    }
}
