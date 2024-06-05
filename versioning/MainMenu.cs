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

        private readonly Option<string> repoOption;
        private readonly Option<string> envFileOption;
        private readonly Option<string> projectOption;

        public MainMenu()
        {
            this.shell = new Shell();

            this.versionArgument = new Argument<string>("version", () => "1.0.0.0", $"Build version.");

            this.repoOption = new Option<string>(new[] { "-r", "--repo" }, () => Directory.GetCurrentDirectory(), $"Build source directory.");
            this.envFileOption = new Option<string>(new[] { "-e", "--env" }, () => "vervs.cmd", $"Build environment variable file name. e.g vervs.cmd or vervs.ps1");

            this.projectOption = new Option<string>(new[] { "-p", "--project" }, () => Directory.GetCurrentDirectory(), $"Name of project.");

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

            var cmd = new Command("update-project", "Update version in a single project.")
            {
                versionArgument,
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
