﻿// Copyright 2016-2017 Datalust Pty Ltd
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac.Features.Metadata;

namespace Datalust.ClefTool.Cli.Commands
{
    [Command("--help", "Show information about available commands")]
    public class HelpCommand : Command
    {
        readonly List<Meta<Lazy<Command>, CommandMetadata>> _availableCommands;

        public HelpCommand(IEnumerable<Meta<Lazy<Command>, CommandMetadata>> availableCommands)
        {
            _availableCommands = availableCommands.OrderBy(c => c.Metadata.Name).ToList();
        }

        protected override int Run(string[] unrecognised)
        {
            // This is a little hacky; we always show the help for the (anonymous) "pipe" command.

            var ea = Assembly.GetEntryAssembly();
            var name = ea.GetName().Name;

            var cmd = _availableCommands.SingleOrDefault(c => c.Metadata.Name == "pipe");
            if (cmd != null)
            {
                var argHelp = cmd.Value.Value.HasArgs ? " [<args>]" : "";
                Console.Error.WriteLine(name + argHelp);
                Console.Error.WriteLine();
                Console.Error.WriteLine(cmd.Metadata.HelpText);
                Console.Error.WriteLine();

                cmd.Value.Value.PrintUsage();
                return 0;
            }

            return base.Run(unrecognised);
        }
    }
}
