namespace Rtf
{
    using System;
    using System.IO;
    using System.Text;

    public class RtfParser
    {
        private IRtfParserListener _listener;
        private TextReader _reader;
        private StringBuilder _textAcc;
        private int _unicodeSkip = 1;

        public RtfParser(TextReader reader, IRtfParserListener listener)
        {
            _reader = reader;
            _listener = listener;
        }

        public void Parse()
        {
            _textAcc = new StringBuilder();
            while (true)
            {
                var i = _reader.Read();
                if (i == -1)
                {
                    break;
                }
                if (i == '\\')
                {
                    var p = _reader.Peek();
                    if (p == '\\')
                    {
                        _reader.Read();
                        _textAcc.Append('\\');
                    }
                    else if (p == '{')
                    {
                        _reader.Read();
                        _textAcc.Append('{');
                    }
                    else if (p == '}')
                    {
                        _reader.Read();
                        _textAcc.Append('}');
                    }
                    else if (p == '\'')
                    {
                        _reader.Read();
                        ReadHex();
                    }
                    else if (p == '*')
                    {
                        FlushText();
                        _reader.Read();
                        NotifyTagFound("*", null);
                    }
                    else
                    {
                        FlushText();
                        ReadTag();
                    }
                }
                else if (i == '{')
                {
                    FlushText();
                    NotifyGroupBegin();
                }
                else if (i == '}')
                {
                    FlushText();
                    NotifyGroupEnd();
                }
                else if (i == '\r' || i == '\n')
                {
                    // nothing
                }
                else
                {
                    var c = (char)i;
                    _textAcc.Append(c);
                }
            }
            FlushText();
        }

        private void ReadHex()
        {
            var h1 = (char)_reader.Read();
            var h2 = (char)_reader.Read();
            int code = Convert.ToInt32(h1.ToString() + h2, 16);
            _textAcc.Append((char)code);
        }

        private void ReadTag()
        {
            var keyword = new StringBuilder();
            var value = new StringBuilder();
            while (true)
            {
                var i = _reader.Peek();
                if (i == -1)
                {
                    break;
                }
                var c = (char)i;
                if (char.IsLetter(c))
                {
                    if (value.Length == 0)
                    {
                        _reader.Read();
                        keyword.Append(c);
                    }
                    else
                    {
                        NotifyTagFound(keyword.ToString(), value.ToString());
                        break;
                    }
                }
                else if (i == '-')
                {
                    if (value.Length == 0)
                    {
                        _reader.Read();
                        value.Append('-');
                    }
                    else
                    {
                        NotifyTagFound(keyword.ToString(), value.ToString());
                        break;
                    }
                }
                else if (char.IsDigit(c))
                {
                    _reader.Read();
                    value.Append(c);
                }
                else
                {
                    if (i == ' ')
                    {
                        _reader.Read();
                    }
                    NotifyTagFound(keyword.ToString(), value.ToString());
                    break;
                }
            }
        }

        private void FlushText()
        {
            if (_textAcc.Length > 0)
            {
                NotifyTextFound(_textAcc);
                _textAcc.Clear();
            }
        }

        private void NotifyGroupBegin()
        {
            _listener.GroupBegin();
        }

        private void NotifyGroupEnd()
        {
            _listener.GroupEnd();
        }

        private void NotifyTextFound(StringBuilder builder)
        {
            _listener.TextFound(builder);
        }

        private void NotifyTagFound(string keyword, string value)
        {
            if (keyword == "uc")
            {
                _unicodeSkip = Convert.ToInt32(value);
                return;
            }
            if (keyword == "u")
            {
                _textAcc.Append((char)Convert.ToInt32(value));
                SkipAfterUnicode(_unicodeSkip);
                return;
            }
            _listener.TagFound(keyword, value);
        }

        private void SkipAfterUnicode(int charCount)
        {
            for (int i = 0; i < charCount; i++)
            {
                _reader.Read();
            }
        }
    }
}
