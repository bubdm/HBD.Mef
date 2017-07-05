﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace HBD.Mef.Logging.Test
{
    [TestClass]
    public class Log4NetTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            HBD.Framework.IO.DirectoryEx.DeleteDirectories("Logs");
        }

        [TestMethod]
        public void Log4Net_Default_File_LoggerTest()
        {
            var file = string.Empty;
            using (var log = new Log4NetLogger())
            {
                log.Info("AA");

                file = log.DefaultOutFileName;

                Assert.IsTrue(File.Exists(log.DefaultOutFileName));
            }

            Assert.IsTrue(File.ReadAllText(file).Contains("AA"));
        }

        [TestMethod]
        public void Log4NetLogger_Custom_File_Test()
        {
            var file = "Logs\\Log_Log4NetLogger_Log4NetLoggerTest.log";
            using (var log = new Log4NetLogger(file))
            {
                log.Info("BB");
            }

            Assert.IsTrue(File.ReadAllText(file).Contains("BB"));
        }

        [TestMethod]
        //[ExpectedException(typeof(NullReferenceException))]
        public void DisposeTest()
        {
            var file = "Logs\\Log_Log4NetLogger_DisposeTest.log";
            var log = new Log4NetLogger(file);
            log.Info("Duy");
            log.Dispose();
           // Assert.IsTrue(File.ReadAllText(file).Contains("Duy"));
            //log.Dispose();
            //log.Info("AA");
        }
    }
}