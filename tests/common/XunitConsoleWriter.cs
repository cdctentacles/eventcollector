using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using Xunit.Abstractions;

namespace eventcollector.tests
{
    public class XunitConsoleWriter : TextWriter
    {
        ITestOutputHelper _output;
        public XunitConsoleWriter(ITestOutputHelper output)
        {
            _output = output;
        }
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        public override void Write(string message)
        {
            WriteLineXunit(message, null);
        }
        public override void WriteLine(string message)
        {
            WriteLineXunit(message, null);
        }
        public override void WriteLine(string format, params object[] args)
        {
            WriteLineXunit(format, args);
        }

        void WriteLineXunit(string format, params object[] args)
        {
            try
            {
                if (args == null)
                {
                    _output.WriteLine(format);
                }
                else
                {
                    _output.WriteLine(format, args);
                }
            }
            catch (InvalidOperationException ex)
            {
                if (!ex.Message.Contains("There is no currently active test"))
                {
                    // eat this exception..
                    throw;
                }
            }
        }
    }
}