// ------------------------------------------------------------------------
//   Copyright (c) ArieTech GmbH.  All rights reserved.
// ------------------------------------------------------------------------

using System;
using System.Linq;

namespace ArieTech.Plugin.CrashLog.Samples
{
    public class DebugItem
    {
        public string Path { get; private set; }

        public string File { get; private set; }

        public string Description { get; private set; }

        public DebugItem(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

            Path = path;
            Description = path.EndsWith(".log") && path.Contains("Fatal") ? "CrashReport" : "Unknown";
            File = path.Split('/').Last();
        }
    }
}
