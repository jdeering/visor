// Guids.cs
// MUST match guids.h
using System;

namespace Visor
{
    static class GuidList
    {
        public const string VisorPackageString = "45F0973E-50AB-49DD-B3AA-7E5245ECC705";
        
        public static readonly Guid VisorPackage = new Guid(VisorPackageString);
    };
}