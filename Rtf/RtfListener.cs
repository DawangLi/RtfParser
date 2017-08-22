namespace Rtf
{
    using System;
    using System.Linq;
    using System.Text;

    public class RtfListener : IRtfParserListener
    {
        private const string _newLine = "\n";
        private string[] _ignoreGroups = { "*", "fonttbl", "colortbl", "info", "stylesheet", "pict" };
        private StringBuilder _builder = new StringBuilder();
        private int _level = 0;
        private int? _ignoreLevel = null;

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
            _level++;
        }

        public void GroupEnd()
        {
            _level--;
            if (_ignoreLevel != null)
            {
                if (_level < _ignoreLevel)
                {
                    _ignoreLevel = null;
                }
            }
        }

        public void TagFound(string keyword, string value)
        {
            if (keyword == "pict")
            {
                _builder.Append(" ");
            }
            if (_ignoreLevel == null)
            {
                if (_ignoreGroups.Contains(keyword))
                {
                    _ignoreLevel = _level;
                }
            }
            if (_ignoreLevel == null)
            {
                switch (keyword)
                {
                    case "line":
                    case "par":
                    case "row":
                        _builder.Append(_newLine);
                        break;
                    case "tab":
                    case "cell":
                        _builder.Append("\t");
                        break;
                    case "rquote":
                        _builder.Append("’");
                        break;
                }
            }
        }

        public void TextFound(StringBuilder text)
        {
            if (_ignoreLevel == null)
            {
                _builder.Append(text.ToString());
            }
        }
    }
}
