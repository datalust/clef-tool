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

namespace Datalust.ClefTool.Cli.Features
{
    class SeqOutputFeature : CommandFeature
    {
        public string SeqUrl { get; private set; }
        public string SeqApiKey { get; private set; }

        public override void Enable(OptionSet options)
        {
            options.Add("out-seq=",
                "Send output to Seq at the specified URL",
                v => SeqUrl = string.IsNullOrWhiteSpace(v) ? null : v.Trim());

            options.Add("out-seq-apikey=",
                "Specify the API key to use when writing to Seq, if required",
                v => SeqApiKey = string.IsNullOrWhiteSpace(v) ? null : v.Trim());
        }
    }
}
