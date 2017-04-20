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
using System.IO;
using System.Linq;

namespace Datalust.ClefTool.Cli
{
    public abstract class Command
    {
        private readonly IList<CommandFeature> _features = new List<CommandFeature>();

        protected Command()
        {
            Options = new OptionSet();
        }

        protected OptionSet Options { get; }

        public bool HasArgs => Options.Any();

        protected T Enable<T>()
            where T : CommandFeature, new()
        {
            var t = new T();
            return Enable(t);
        }

        protected T Enable<T>(T t)
            where T : CommandFeature
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            t.Enable(Options);
            _features.Add(t);
            return t;
        }

        public void PrintUsage()
        {
            if (Options.Any())
            {
                Console.Error.WriteLine("Arguments:");
                Options.WriteOptionDescriptions(Console.Error);
            }
        }

        public int Invoke(string[] args)
        {
            try
            {
                var unrecognised = Options.Parse(args).ToArray();

                var errs = _features.SelectMany(f => f.Errors).ToList();

                if (errs.Any())
                {
                    ShowUsageErrors(errs);
                    return -1;
                }

                return Run(unrecognised);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return -1;
            }
        }

        protected virtual int Run(string[] unrecognised)
        {
            // All commands used to accept --nologo
            var notIgnored = unrecognised.Where(o => o.IndexOf("nologo", StringComparison.OrdinalIgnoreCase) == -1);
            if (notIgnored.Any())
            {
                ShowUsageErrors(new [] { "Unrecognized options: " + string.Join(", ", notIgnored) });
                return -1;
            }

            return Run();
        }

        protected virtual int Run() { return 0; }

        protected virtual void ShowUsageErrors(IEnumerable<string> errors)
        {
            var header = "Error:";
            foreach (var error in errors)
            {
                Printing.Define(header, error, 7, Console.Out);
                header = new string(' ', header.Length);
            }
        }
    }
}
