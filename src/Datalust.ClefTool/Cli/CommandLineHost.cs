// Copyright 2016-2017 Datalust Pty Ltd
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
using Autofac.Features.Metadata;

namespace Datalust.ClefTool.Cli
{
    public class CommandLineHost
    {
        readonly List<Meta<Lazy<Command>, CommandMetadata>> _availableCommands;

        public CommandLineHost(IEnumerable<Meta<Lazy<Command>, CommandMetadata>> availableCommands)
        {
            _availableCommands = availableCommands.ToList();
        }
        
        public int Run(string[] args)
        {
            if (args.Length > 0)
            {
                var norm = args[0].ToLowerInvariant();
                var cmd = _availableCommands.SingleOrDefault(c => c.Metadata.Name == norm);
                if (cmd != null)
                {
                    return cmd.Value.Value.Invoke(args.Skip(1).ToArray());
                }
            }

            var pipeCommand = _availableCommands.Single(c => c.Metadata.Name == "pipe");
            return pipeCommand.Value.Value.Invoke(args.ToArray());
        }
    }
}
