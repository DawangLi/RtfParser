namespace Rtf
{
    using System;
    using System.Text;

    public interface IRtfParserListener
    {
        void GroupBegin();
        void GroupEnd();
        void TagFound(string keyword, string value);
        void TextFound(StringBuilder text);
    }
}
