using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Fiddler;

namespace BinaryMessageFiddlerExtension
{
    public class BinaryInspector : Inspector2
    {
        private XmlTreeView _myControl;
        private byte[] _entityBody;

        private static readonly XmlDictionary _wcfBinaryDictionary = WcfBinaryDictionary.CreateWcfBinaryDictionary();
        private static Logger log = new Logger(true);

        public override void AddToTab(TabPage o)
        {
            _myControl = new XmlTreeView();
            o.Text = "WCF Binary";
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
            get { return false; }
        }

        public bool bReadOnly
        {
            get { return true; }
            set { }
        }

        public void Clear()
        {
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
                XmlDocument document = GetWcfBinaryMessageAsXml(_entityBody);

                if (document == null || document.ChildNodes.Count == 0)
                    this.Clear();
                else
                    _myControl.LoadXml(document);
            }
            catch (XmlException ex)
            {
                log.LogString(ex.ToString());
                _myControl.DisplayError(ex.Message);
            }
        }

        private XmlDocument GetWcfBinaryMessageAsXml(byte[] encodedMessage)
        {
            XmlDocument document = new XmlDocument();
            using (var reader = CreateReaderForMessage(encodedMessage))
            {
                document.Load(reader);
            }
            return document;
        }

        private XmlDictionaryReader CreateReaderForMessage(byte[] encodedMessage)
        {
            return XmlDictionaryReader.CreateBinaryReader(encodedMessage,
                                                          0, 
                                                          encodedMessage.Length,
                                                          _wcfBinaryDictionary,
                                                          XmlDictionaryReaderQuotas.Max);
        }
    }
}
