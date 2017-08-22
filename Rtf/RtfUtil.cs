namespace Rtf
{
    using System.IO;
    using System.Text;

    public static class RtfUtil
    {
        public static string GetPlainText(string rtf)
        {
            var listener = new RtfListener();
            var parser = new RtfParser(new StringReader(rtf), listener);
            parser.Parse();
            RemoveLastCarriageReturn(listener.Builder);
            return listener.Builder.ToString();
        }

        public static string GetPlainText(TextReader rtf)
        {
            var listener = new RtfListener();
            var parser = new RtfParser(rtf, listener);
            parser.Parse();
            RemoveLastCarriageReturn(listener.Builder);
            return listener.Builder.ToString();
        }

        private static void RemoveLastCarriageReturn(StringBuilder builder)
        {
            if (builder.Length > 0)
            {
                if (builder[builder.Length - 1] == '\n')
                {
                    builder.Remove(builder.Length - 1, 1);
                }
            }
        }
    }
}
