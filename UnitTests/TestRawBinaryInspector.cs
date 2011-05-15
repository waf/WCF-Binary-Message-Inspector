using BinaryMessageFiddlerExtension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;

namespace UnitTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class TestRawBinaryInspector
    {
        readonly RawBinaryInspector rawBinInspector;

        public TestRawBinaryInspector()
        {
            rawBinInspector = new RawBinaryInspector { _myControl = new TextBox() };
            rawBinInspector._myControl.Text = "<s:Envelope xmlns:s=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:a=\"http://www.w3.org/2005/08/addressing\"><s:Header><a:Action s:mustUnderstand=\"1\">http://tempuri.org/IEncounterWCFService/GetEncounter</a:Action><a:MessageID>urn:uuid:fa9d4b23-eae2-4487-9bbc-19041fcc6318</a:MessageID><a:SequenceAcknowledgement><a:ReplyTo>http://www.w3.org/2005/08/addressing/anonymous</a:ReplyTo></a:SequenceAcknowledgement><a:To s:mustUnderstand=\"1\">http://emr.local.wellmed.net/service/EncounterWCFService.svc</a:To></s:Header><s:Body><GetEncounter xmlns=\"http://tempuri.org/\"><EncounterId>a87b775c-1377-4142-8899-bda093e7795d</EncounterId></GetEncounter></s:Body></s:Envelope>";
        }

        private TestContext testContextInstance;

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void testTextChanged()
        {
            rawBinInspector.TextChanged(new object(), new System.EventArgs());
        }
    }
}
