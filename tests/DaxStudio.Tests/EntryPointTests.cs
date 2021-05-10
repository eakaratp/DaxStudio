﻿using DaxStudio.Standalone;
using Fclp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaxStudio.Tests.Helpers;

namespace DaxStudio.Tests
{
    [TestClass]
    public class EntryPointTests
    {


        [TestMethod]
        public void HelpFormatter()
        {
            var p = new FluentCommandLineParser();
            p.Setup<int>('p', "port");

            p.Setup<bool>('l', "log")
                .WithDescription("Enable Debug Logging")
                .SetDefault(false);

            p.Setup<string>('f', "file")
                .WithDescription("Name of file to open");

            p.Setup<bool>('c', "crashtest")
                .SetDefault(false);

            p.Setup<string>('s', "server")
                .WithDescription("Server to connect to");

            p.Setup<string>('d', "database")
                .WithDescription("Database to connect to");

            var formattedHelp = DaxStudio.Standalone.HelpFormatter.Format(p.Options).NormalizeNewline();
            var expectedHelp = @"  -l --log                Enable Debug Logging
  -f --file <string>      Name of file to open
  -s --server <string>    Server to connect to
  -d --database <string>  Database to connect to
".NormalizeNewline();
            Assert.AreEqual(expectedHelp, formattedHelp);
        }
    }
}
