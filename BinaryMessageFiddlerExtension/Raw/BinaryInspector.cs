using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Fiddler;

namespace BinaryMessageFiddlerExtension
{
    public class RawBinaryInspector : Inspector2
    {
        public TextBox _myControl;
        private byte[] _entityBody;
        private bool _isDirty;

        private static readonly XmlDictionary _wcfBinaryDictionary = WcfBinaryDictionary.CreateWcfBinaryDictionary();
        private static Logger log = new Logger(true);

        public override void AddToTab(TabPage o)
        {
            _myControl = new TextBox();
            _myControl.ReadOnly = true;
            _myControl.Multiline = true;
            _myControl.TextChanged += new EventHandler(TextChanged);
            o.Text = "WCF Binary Raw";
            o.Controls.Add(_myControl);
            o.Controls[0].Dock = DockStyle.Fill;
        }

        #region IBaseInspector2 Properties

        public override int GetOrder()
        {
            return 0;
        }

        public bool bDirty
        {
            get { return _isDirty; }
        }

        public bool bReadOnly
        {
            get { return _myControl.ReadOnly; }
            set { _myControl.ReadOnly = value; }
        }

        public void Clear()
        {
            _isDirty = false;
            _myControl.Clear();
        }

        public byte[] body
        {
            get { return _entityBody; }
            set { UpdateView(value); }
        }

        #endregion

        private void UpdateView(byte[] bytes)
        {
            _entityBody = bytes;

            try
            {
                string document = GetWcfBinaryMessageAsText(_entityBody);

                if (String.IsNullOrEmpty(document))
                    this.Clear();
                else
                    _myControl.Text = document;
            }
            catch (XmlException ex)
            {
                log.LogString(ex.ToString());
                _myControl.Text = ex.Message;
            }
        }

        private string GetWcfBinaryMessageAsText(byte[] encodedMessage)
        {
            XmlDocument document = new XmlDocument();
            using (var reader = CreateReaderForMessage(encodedMessage))
            {
                document.Load(reader);
            }
            return document.OuterXml;
        }

        private XmlDictionaryReader CreateReaderForMessage(byte[] encodedMessage)
        {
            return XmlDictionaryReader.CreateBinaryReader(encodedMessage,
                                                          0,
                                                          encodedMessage.Length,
                                                          _wcfBinaryDictionary,
                                                          XmlDictionaryReaderQuotas.Max);
        }

        public void TextChanged(object sender, EventArgs e)
        {
            XmlDocument document = new XmlDocument();
            _isDirty = true;
            document.LoadXml(_myControl.Text);

            using (MemoryStream ms = new MemoryStream())
            {
                XmlDictionaryWriter binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(ms);
                document.WriteContentTo(binaryWriter);
                binaryWriter.Flush();
                ms.Position = 0;
                _entityBody = new byte[Int32.Parse(ms.Length.ToString())];
                ms.Read(_entityBody, 0, Int32.Parse(ms.Length.ToString()));
                ms.Flush();
            }
        }

        private static MemoryStream CreateMemoryStream(XmlDocument doc)
        {
            MemoryStream ms = new MemoryStream();
                XmlWriter w = XmlWriter.Create(ms);
                doc.WriteTo(w);
                w.Flush();
                ms.Position = 0;
                return ms;
        }
        private static XmlDictionaryWriter CreateWriterForMessage(XmlDocument doc)
        {
                MemoryStream ms = CreateMemoryStream(doc);
                return XmlDictionaryWriter.CreateBinaryWriter(ms);
        }
    }
}
