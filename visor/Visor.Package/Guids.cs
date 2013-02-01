// Guids.cs
// MUST match guids.h
using System;

namespace Visor
{
    static class GuidList
    {
        public const string LanguagePkgString = "4FCA0205-8130-4F53-B20B-1106775A6E85";

        public static readonly Guid LanguagePkg = new Guid(LanguagePkgString);
    };
}