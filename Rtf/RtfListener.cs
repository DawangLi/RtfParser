namespace Rtf
{
    using System;
    using System.Text;

    public class RtfListener : IRtfParserListener
    {
        private StringBuilder _builder = new StringBuilder();

        public StringBuilder Builder
        {
            get
            {
                return _builder;
            }
            set
            {
                _builder = value;
            }
        }

        public void GroupBegin()
        {
        }

        public void GroupEnd()
        {
        }

        public void TagFound(string keyword, string value)
        {
        }

        public void TextFound(StringBuilder text)
        {
            _builder.Append(text.ToString());
        }
    }
}
