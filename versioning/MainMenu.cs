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

        public MainMenu()
        {
            this.shell = new Shell();

            this.versionArgument = new Argument<string>("version", () => "1.0.0.0", $"Build version.");
        }

        public int Run(string[] args, string title)
        {
            RootCommand rootCommand = new RootCommand(title)
            {
                SolutionCommand(),
                ProjectCommand(),
            };

            int exit = rootCommand.InvokeAsync(args).Result;
            return exit;
        }

        private Command SolutionCommand()
        {
            Option<string> repoOption = new Option<string>(new[] { "-r", "--repo" }, () => Directory.GetCurrentDirectory(), $"Build source directory.");
            Option<string> envFileOption = new Option<string>(new[] { "-e", "--env" }, () => "vervs.cmd", $"Build environment variable file name. e.g vervs.cmd or vervs.ps1");

            var cmd = new Command("update-repo", "Update version for all projects in repository.")
            {
                versionArgument,
                repoOption,
                envFileOption,
            };

            cmd.SetHandler(shell.UpdateRepo,
               versionArgument,
               repoOption,
               envFileOption
            );

            return cmd;
        }

        private Command ProjectCommand()
        {
            string directory = Directory.GetCurrentDirectory();
            string repo = Path.GetFullPath(Path.Combine(directory, ".."));
            string projectName = Path.GetFileName(directory);

            Option<string> repoOption = new Option<string>(new[] { "-r", "--repo" }, () => repo, $"Build source directory.");
            Option<string> projectOption = new Option<string>(new[] { "-p", "--project" }, () => projectName, $"Name of project.");

            var cmd = new Command("update-project", "Update version in a single project.")
            {
                versionArgument,
                repoOption,
                projectOption,
            };

            cmd.SetHandler(shell.UpdateProject,
               versionArgument,
               repoOption,
               projectOption
               );

            return cmd;
        }
    }
}
