// Guids.cs
// MUST match guids.h
using System;

namespace Visor
{
    static class GuidList
    {
        public const string VisorPackageString = "f4fa20ca-0475-4d4a-a5ce-e643fdbbd2e4";

        public static readonly Guid VisorPackage = new Guid(VisorPackageString);
    };
}