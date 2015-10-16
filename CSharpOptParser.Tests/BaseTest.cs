using System;
using System.Collections.Generic;
using CSharpOptParser;
using NUnit.Framework;

namespace CSharpOptParser.Tests
{
    [TestFixture]
    public class BaseTest
    {
        /// <summary>
        /// Test creating Parser with fluent interface
        /// </summary
        [Test]
        public void testValidCountOfRequiredParameters()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program")
                                        .addOption('h', "help", OptParser.OPTIONAL, "false", "Show help")
                                        .addOption('p', "parameter", OptParser.OPTIONAL, "false", "Test parameter")
                                        .addPathOrExpression("path", OptParser.REQUIRED, "", "Params");
            Assert.AreEqual(1, parser.RequiredParameters.Count);
        }


        /// <summary>
        /// Test exception when brackets overlaps.
        /// </summary>
        [Test]
        public void testOverleapingBrackets()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program").addOption('h', "help", OptParser.OPTIONAL, "", "Show this help").addOption('p', "parameter", OptParser.OPTIONAL, "", "Test parameter").addOption('f', "parameter2", OptParser.OPTIONAL, "", "Test parameter 2");

            // Assert exception
            string cmdLine = "--parameter \"test'test2\" -f pepa'";
            Assert.Throws<OverlapingBracketsException>(delegate { parser.parseArguments(cmdLine); });
        }

        /// <summary>
        /// Test exception when brackets overlaps.
        /// </summary>
        [Test]
        public void testOverleapingSingleOpen()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program").addOption('h', "help", OptParser.OPTIONAL, "", "Show this help").addOption('p', "parameter", OptParser.OPTIONAL, "", "Test parameter").addOption('f', "parameter2", OptParser.OPTIONAL, "", "Test parameter 2");

            // Assert exception
            string cmdLine = "--parameter \"test2\" -f 'pepa\"";
            Assert.Throws<OverlapingBracketsException>(delegate { parser.parseArguments(cmdLine); });
        }

        /// <summary>
        /// Test exception when brackets overlaps.
        /// </summary>
        [Test]
        public void testOverleapingDoubleNotClosed()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program").addOption('h', "help", OptParser.OPTIONAL, "", "Show this help").addOption('p', "parameter", OptParser.OPTIONAL, "", "Test parameter").addOption('f', "parameter2", OptParser.OPTIONAL, "", "Test parameter 2");

            // Assert exception
            string cmdLine = "--parameter \"test2\" -f \"pepa";
            Assert.Throws<OverlapingBracketsException>(delegate { parser.parseArguments(cmdLine); });
        }


        /// <summary>
        /// Invalid count of exception (simple is not ended) </summary>
        /// <exception cref="Exception"> </exception>
        [Test]
        public void testInvalidBracketCount()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program").addOption('h', "help", OptParser.OPTIONAL, "", "Show this help").addOption('p', "parameter", OptParser.OPTIONAL, "", "Test parameter").addOption('f', "parameter2", OptParser.OPTIONAL, "", "Test parameter 2");

            // Assert invalid count of brackets
            string cmdLine = "--parameter \"test\" -f 'pepa";
            Assert.Throws<OverlapingBracketsException>(delegate { parser.parseArguments(cmdLine); });
        }

        /// <summary>
        /// Test of assigning required POE before optional POE
        /// </summary>
        [Test]
        public void testOptionalPOE()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program")
                                        .addOption('h', "help", OptParser.OPTIONAL, "", "Show this help")
                                        .addPathOrExpression("testpoe2", OptParser.OPTIONAL, "test", "POE 2 for optional test")
                                        .addPathOrExpression("testpoe", OptParser.REQUIRED, "", "Path for test");

            // Assert invalid count of brackets
            string cmdLine = "testpoe1";
            parser.parseArguments(new List<string>() { cmdLine });

            // Assert Required expression
            Assert.AreEqual("testpoe1", parser.getOption("testpoe").Value);
            Assert.IsNull(parser.getOption("testpoe2"), "Option poe filled");

            // But I can get default value from testpoe2
            Assert.AreEqual("test", parser.getOptionValue("testpoe2"), "Invalid assigning default string to Optional POE");
        }

        /// <summary>
        /// Test missing otpion value
        /// </summary>
        [Test]
        public void testMissingOptionValue()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program").addOptionRequiredValue('c', "help", OptParser.OPTIONAL, null, "Show this help").addOption('p', "parameter", OptParser.OPTIONAL, null, "Test parameter").addOption('f', "parameter2", OptParser.OPTIONAL, null, "Test parameter 2");

            // Assert invalid count of brackets
            string cmdLine = "-c -p";
            Assert.Throws<MissingOptionValue>(delegate { parser.parseArguments(cmdLine); });
        }

        /// <summary>
        /// Test missing otpion value
        /// </summary>
        [Test]
        public void testMissingOptionValueNothing()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program").addOptionRequiredValue('c', "help", OptParser.OPTIONAL, "", "Show this help").addOption('p', "parameter", OptParser.OPTIONAL, "", "Test parameter").addOption('f', "parameter2", OptParser.OPTIONAL, "", "Test parameter 2");

            // Assert invalid count of brackets
            string cmdLine = "-c";
            Assert.Throws<MissingOptionValue>(delegate { parser.parseArguments(cmdLine); });
        }

        /// <summary>
        /// Unexcepted option from command line! </summary>
        /// <exception cref="Exception"> </exception>
        [Test]
        public void testUnexpectedOption()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program").addOption('h', "help", OptParser.OPTIONAL, "", "Show this help").addOption('p', "parameter", OptParser.OPTIONAL, "", "Test parameter").addOption('f', "parameter2", OptParser.OPTIONAL, "", "Test parameter 2");

            // Assert invalid count of brackets
            string cmdLine = "--invalidOption \"test1\" --parameter \"test\" -f 'pepa'";
            Assert.Throws<UnexpectedOption>(delegate { parser.parseArguments(cmdLine); });
        }

        /// <summary>
        /// Missing required option </summary>
        /// <exception cref="Exception">  </exception>
        [Test]
        public void testMissingRequiredOption()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program").addOptionRequiredValue('c', "help", OptParser.OPTIONAL, "", "Show this help").addOption('p', "parameter", OptParser.REQUIRED, "", "Test parameter").addOption('f', "parameter2", OptParser.OPTIONAL, "", "Test parameter 2", OptParser.OPTION_NO_VALUE);

            // Assert invalid count of brackets
            string cmdLine = "-f";
            Assert.Throws<MissingOptions>(delegate { parser.parseArguments(cmdLine); });
        }

        /// <summary>
        /// Missing required path or expression
        /// </summary>
        [Test]
        public void testMissingRequiredPOE()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program").addOption('h', "help", OptParser.OPTIONAL, "", "Show this help").addOptionRequiredValue('p', "parameter", OptParser.OPTIONAL, "", "Test parameter").addOption('f', "parameter2", OptParser.OPTIONAL, "", "Test parameter 2").addPathOrExpression("testpoe", OptParser.REQUIRED, "", "Path for test");

            // Assert invalid count of brackets
            string cmdLine = "-f";
            Assert.Throws<MissingOptions>(delegate { parser.parseArguments(cmdLine); });
        }

        /// <summary>
        /// Missing required path or expression
        /// </summary>
        [Test]
        public void testMissingRequiredPOEAfterOptionWithValue()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program").addOption('h', "help", OptParser.OPTIONAL, "", "Show this help").addOptionRequiredValue('p', "parameter", OptParser.OPTIONAL, "", "Test parameter").addOption('f', "parameter2", OptParser.OPTIONAL, "", "Test parameter 2").addPathOrExpression("testpoe", OptParser.REQUIRED, "", "Path for test");

            // Assert invalid count of brackets
            string cmdLine = "-p \"test\"";
            Assert.Throws<MissingOptions>(delegate { parser.parseArguments(cmdLine); });
        }

        /// <summary>
        /// Test get help.
        /// </summary>
        [Test]
        public void testGetHelp()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program").addOption('h', "help", OptParser.OPTIONAL, "", "Show this help").addOptionRequiredValue('p', "parameter", OptParser.OPTIONAL, "", "Test parameter").addOption('f', "parameter2", OptParser.OPTIONAL, "", "Test parameter 2").addPathOrExpression("testpoe", OptParser.REQUIRED, "", "Path for test");

            // Assert invalid count of brackets
            string cmdLine = "-h";
            Assert.Throws<MissingOptionsHelp>(delegate { parser.parseArguments(cmdLine); });
        }


        /// <summary>
        /// Test correct processing CMD line with optional parameters
        /// </summary>
        [Test]
        public void testProcessingOptional()
        {
            OptParser parser = OptParser.createOptionParser("test", "Test program").addOptionRequiredValue('h', "help", OptParser.OPTIONAL, "", "Show this help").addOptionRequiredValue('p', "parameter", OptParser.OPTIONAL, "", "Test parameter").addOption('f', "parameter2", OptParser.OPTIONAL, "", "Test parameter 2");

            string cmdLine = "--parameter test2 -h test1";

            // Parse
            parser.parseArguments(cmdLine);

            // Try value
            Assert.AreEqual("test1", parser.getOption("help").value(), "Invalid parsing help parameter");
            Assert.AreEqual("test2", parser.getOption("parameter").value(), "Invalid parsing help parameter");
            Assert.IsNull(parser.getOption("parameter2"), "Invalid parsing help parameter");

            // Test is filled?
            Assert.IsTrue(parser.isOptionFilled("help"), "Invalid help filled");
            Assert.IsFalse(parser.isOptionFilled("parameter2"), "Invalid non-filled");
        }
    }
}
