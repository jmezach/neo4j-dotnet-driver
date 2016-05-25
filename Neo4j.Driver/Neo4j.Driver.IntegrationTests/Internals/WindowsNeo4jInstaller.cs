// Copyright (c) 2002-2016 "Neo Technology,"
// Network Engine for Objects in Lund AB [http://neotechnology.com]
// 
// This file is part of Neo4j.
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver.V1;

namespace Neo4j.Driver.IntegrationTests.Internals
{
    public class WindowsNeo4jInstaller : INeo4jInstaller
    {
        public DirectoryInfo Neo4jHome { get; private set; }

        public void DownloadNeo4j()
        {
            Neo4jHome = Neo4jServerFilesDownloadHelper.DownloadNeo4j(
                Neo4jServerEdition.Community,
                Neo4jServerPlatform.Windows,
                new ZipNeo4jServerFileExtractor()).Result;
        }

        public void UpdateSettings(IDictionary<string, string> keyValuePair)
        {
            Neo4jSettingsHelper.UpdateSettings(Neo4jHome.FullName, keyValuePair);
        }

        public void InstallServer()
        {
            RunCommand("install-service");
        }

        public void UninstallServer()
        {
            RunCommand("uninstall-service");
        }

        public void StartServer()
        {
            RunCommand("start");
#if BUILDSERVER
            Task.Delay(100000).Wait();
#else
            Task.Delay(10000).Wait();
#endif
        }

        public void StopServer()
        {
            RunCommand("stop");
#if BUILDSERVER
            Task.Delay(100000).Wait();
#endif
        }

        private void RunCommand(string command)
        {
            var batfile = Path.Combine(Neo4jHome.FullName, "bin/Neo4j.bat");
            var processStartInfo = new ProcessStartInfo(batfile, command);
            var process = Process.Start(processStartInfo);
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Neo4jException("Integration", "Error running command");
            }
        }
    }
}