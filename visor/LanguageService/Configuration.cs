using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Visor.LanguageService
{
    public static class Configuration
    {
        public static Grammar Grammar = new Grammar();

        public static string Name = "RepGen";
        public static string FormatList = "All Files (*.*)\n*.*\nRepGen Files (*.rg)\n*.rg\n";

        private static readonly List<IVsColorableItem> colorableItems = new List<IVsColorableItem>();

        private static readonly TokenDefinition defaultDefinition = new TokenDefinition(TokenType.Text, TokenColor.Text,
                                                                                        TokenTriggers.None);

        private static readonly Dictionary<string, TokenDefinition> definitions =
            new Dictionary<string, TokenDefinition>();

        public static IList<IVsColorableItem> ColorableItems
        {
            get { return colorableItems; }
        }

        public static TokenColor CreateColor(string name, COLORINDEX foreground, COLORINDEX background)
        {
            return CreateColor(name, foreground, background, false, false);
        }

        public static TokenColor CreateColor(string name, COLORINDEX foreground, COLORINDEX background, bool bold,
                                             bool strikethrough)
        {
            colorableItems.Add(new ColorableItem(name, foreground, background, bold, strikethrough));
            return (TokenColor) colorableItems.Count;
        }

        public static void ColorToken(string tokenName, TokenType type, TokenColor color, TokenTriggers trigger)
        {
            definitions[tokenName] = new TokenDefinition(type, color, trigger);
        }

        public static TokenDefinition GetDefinition(string tokenName)
        {
            TokenDefinition result;
            return definitions.TryGetValue(tokenName, out result) ? result : defaultDefinition;
        }

        public struct TokenDefinition
        {
            public TokenColor TokenColor;
            public TokenTriggers TokenTriggers;
            public TokenType TokenType;

            public TokenDefinition(TokenType type, TokenColor color, TokenTriggers triggers)
            {
                TokenType = type;
                TokenColor = color;
                TokenTriggers = triggers;
            }
        }
    }

    public class ColorableItem : IVsColorableItem
    {
        private readonly COLORINDEX background;
        private readonly string displayName;
        private readonly uint fontFlags = (uint) FONTFLAGS.FF_DEFAULT;
        private readonly COLORINDEX foreground;

        public ColorableItem(string displayName, COLORINDEX foreground, COLORINDEX background, bool bold,
                             bool strikethrough)
        {
            this.displayName = displayName;
            this.background = background;
            this.foreground = foreground;

            if (bold)
                fontFlags = fontFlags | (uint) FONTFLAGS.FF_BOLD;
            if (strikethrough)
                fontFlags = fontFlags | (uint) FONTFLAGS.FF_STRIKETHROUGH;
        }

        #region IVsColorableItem Members

        public int GetDefaultColors(COLORINDEX[] piForeground, COLORINDEX[] piBackground)
        {
            if (null == piForeground)
            {
                throw new ArgumentNullException("piForeground");
            }
            if (0 == piForeground.Length)
            {
                throw new ArgumentOutOfRangeException("piForeground");
            }
            piForeground[0] = foreground;

            if (null == piBackground)
            {
                throw new ArgumentNullException("piBackground");
            }
            if (0 == piBackground.Length)
            {
                throw new ArgumentOutOfRangeException("piBackground");
            }
            piBackground[0] = background;

            return VSConstants.S_OK;
        }

        public int GetDefaultFontFlags(out uint pdwFontFlags)
        {
            pdwFontFlags = fontFlags;
            return VSConstants.S_OK;
        }

        public int GetDisplayName(out string pbstrName)
        {
            pbstrName = displayName;
            return VSConstants.S_OK;
        }

        #endregion
    }
}