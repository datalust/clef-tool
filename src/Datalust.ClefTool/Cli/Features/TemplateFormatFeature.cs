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

namespace Datalust.ClefTool.Cli.Features
{
    class TemplateFormatFeature : CommandFeature
    {
        public string? OutputTemplate { get; private set; }

        public override void Enable(OptionSet options)
        {
            options.Add("format-template=",
                "Specify an output template to control plain text formatting",
                v => OutputTemplate = string.IsNullOrWhiteSpace(v) ? null : v.Trim());
        }
    }
}
