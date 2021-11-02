// ------------------------------------------------------------------------
//   Copyright (c) ArieTech GmbH.  All rights reserved.
// ------------------------------------------------------------------------

namespace ArieTech.Plugin.CrashLog
{

    public interface ICrashLog
    {
        /// <summary>
        /// Context only needs to be set within the Android OnCreate method.
        /// </summary>
        void Init(object context = null);

        /// <summary>
        /// If not set the default is set to 'Fatal'.
        /// </summary>
        string Filename { get; set; }
    }
}
