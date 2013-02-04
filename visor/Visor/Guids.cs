﻿// Guids.cs
// MUST match guids.h
using System;

namespace Visor
{
    static class GuidList
    {
        public const string VisorPackageString = "b06dd064-a1ca-4c34-8411-7ba23013c31f";
        public const string VisorCmdSetString = "5990bbe9-3b8a-449a-8135-2f2a0de3b712";

        public static readonly Guid VisorPackage = new Guid(VisorPackageString);
        public static readonly Guid VisorCmdSet = new Guid(VisorCmdSetString);
    };
}