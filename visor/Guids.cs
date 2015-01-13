// Guids.cs
// MUST match guids.h

using System;

namespace Visor
{
    internal static class GuidList
    {
        public const string VisorPackageString = "b06dd064-a1ca-4c34-8411-7ba23013c31f";
        public const string VisorCmdSetString = "5990bbe9-3b8a-449a-8135-2f2a0de3b712";
        public const string VisorToolbarImagesString = "b3f4913f-bcb5-4c42-bc37-71df6ab370f5";
        public const string VisorOptionsPageString = "e8f10dd4-f947-46a1-a653-29304c9fe554";
        public const string VisorProjectFactoryString = "46abf810-899a-485a-bb1a-f079d80a5395";
        public const string VisorReportRunnerWindowString = "cd201cbb-2fda-41a2-8b02-159a0e532fcb";
        public const string VisorReportPromptWindowString = "2118e1b3-621f-47db-8638-5c4414212ae0";

        public static readonly Guid VisorPackage = new Guid(VisorPackageString);
        public static readonly Guid VisorCmdSet = new Guid(VisorCmdSetString);
        public static readonly Guid VisorToolbarImages = new Guid(VisorToolbarImagesString);
        public static readonly Guid VisorOptions = new Guid(VisorOptionsPageString);
        public static readonly Guid VisorProjectFactory = new Guid(VisorProjectFactoryString);
        public static readonly Guid VisorReportRunnerWindow = new Guid(VisorReportRunnerWindowString);
        public static readonly Guid VisorReportPromptWindow = new Guid(VisorReportPromptWindowString);
    };
}