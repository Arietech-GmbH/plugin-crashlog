// ------------------------------------------------------------------------
//   Copyright (c) ArieTech GmbH.  All rights reserved.
// ------------------------------------------------------------------------

using System;

namespace ArieTech.Plugin.CrashLog
{
    /// <summary>
    /// Cross CrashLog
    /// </summary>
    public static class CrossCrashLog
    {

        static Lazy<ICrashLog> implementation = new Lazy<ICrashLog>(() => CreateCrashLog(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets if the plugin is supported on the current platform.
        /// </summary>
        public static bool IsSupported => implementation.Value == null ? false : true;

        /// <summary>
        /// Current plugin implementation to use
        /// </summary>
        public static ICrashLog Current
        {
            get
            {
                ICrashLog ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static ICrashLog CreateCrashLog()
        {
#if NETSTANDARD1_0 || NETSTANDARD2_0
            return null;
#else
#pragma warning disable IDE0022 // Use expression body for methods
            return new CrashLogImplementation();
#pragma warning restore IDE0022 // Use expression body for methods
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");

    }
}
