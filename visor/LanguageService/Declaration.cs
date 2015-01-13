using System;

namespace Visor.LanguageService
{
    public struct Declaration : IComparable<Declaration>
    {
        public string Description;
        public string DisplayText;
        public int Glyph;
        public string Name;

        #region IComparable<Declaration> Members

        public int CompareTo(Declaration other)
        {
            return DisplayText.CompareTo(other.DisplayText);
        }

        #endregion

        public Declaration(string description, string displayText, int glyph, string name)
        {
            Description = description;
            DisplayText = displayText;
            Glyph = glyph;
            Name = name;
        }
    }
}