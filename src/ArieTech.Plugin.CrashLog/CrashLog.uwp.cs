// ------------------------------------------------------------------------
//   Copyright (c) ArieTech GmbH.  All rights reserved.
// ------------------------------------------------------------------------

using System;

namespace ArieTech.Plugin.CrashLog
{
    /// <summary>
    /// Interface for CrashLog
    /// </summary>
    public class CrashLogImplementation : ICrashLog
    {
        public string Filename { get; set; } = "Fatal";

        public void Init(object context = null)
        {
            throw new NotImplementedException();
        }
    }
}
