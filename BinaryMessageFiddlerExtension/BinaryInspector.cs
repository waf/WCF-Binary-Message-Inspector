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
        private Control container;
        private XmlTreeView viewControl;
        private TextBox editControl;

        private byte[] binaryContent;

        private static readonly XmlDictionary _wcfBinaryDictionary = WcfBinaryDictionary.CreateWcfBinaryDictionary();
        private static Logger log = new Logger(true);
        private bool isReadOnly;

        public override void AddToTab(TabPage o)
        {
            o.Text = "WCF Binary XML";

            viewControl = new XmlTreeView();
            viewControl.BackColor = CONFIG.colorDisabledEdit;
            viewControl.Dock = DockStyle.Fill;

            editControl = new TextBox { Multiline = true, ScrollBars = ScrollBars.Vertical };
            editControl.TextChanged += TextChanged;
            editControl.Dock = DockStyle.Fill;
            
            container = o;
        }

        #region IBaseInspector2 Properties

        public override int GetOrder()
        {
            return 1;
        }

        public bool bDirty { get; set; }

        public bool bReadOnly
        {
            get { return isReadOnly; }
            set 
            {
                isReadOnly = value;
                ChangeWidget();
            }
        }

        private void ChangeWidget()
        {
            Control widget = isReadOnly ? viewControl : editControl as Control;
            container.Controls.Clear();
            container.Controls.Add(widget);
        }

        public void Clear()
        {
            viewControl.Clear();
            editControl.Clear();
            bDirty = false;
            bReadOnly = true;
        }

        public byte[] body
        {
            get { return binaryContent; }
            set 
            {
                binaryContent = value;
                UpdateContent(); 
            }
        }

        #endregion

        private void UpdateContent()
        {
            try
            {
                XmlDocument document = GetWcfBinaryMessageAsXml(binaryContent);

                if (document == null || document.ChildNodes.Count == 0)
                {
                    this.Clear();
                }
                else
                {
                    viewControl.LoadXml(document);
                    editControl.Text = document.OuterXml;
                }
            }
            catch (XmlException ex)
            {
                log.LogString(ex.ToString());
                viewControl.DisplayError(ex.Message);
            }
        }

        private XmlDocument GetWcfBinaryMessageAsXml(byte[] encodedMessage)
        {
            XmlDocument document = new XmlDocument();
            
            XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(
                encodedMessage,
                0,
                encodedMessage.Length,
                _wcfBinaryDictionary,
                XmlDictionaryReaderQuotas.Max);

            using (reader)
            {
                document.Load(reader);
            }

            return document;
        }

        public void TextChanged(object sender, EventArgs e)
        {
            XmlDocument document = new XmlDocument();
            bDirty = true;
            try
            {
                document.LoadXml(editControl.Text);

                using (MemoryStream ms = new MemoryStream())
                {
                    XmlDictionaryWriter binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(ms);
                    document.WriteContentTo(binaryWriter);
                    binaryWriter.Flush();
                    ms.Position = 0;
                    binaryContent = new byte[Int32.Parse(ms.Length.ToString())];
                    ms.Read(binaryContent, 0, Int32.Parse(ms.Length.ToString()));
                    ms.Flush();
                }
            }
            catch (XmlException ex)
            {
                log.LogString("An XML error occured, reverting to plain text");
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                binaryContent = encoding.GetBytes(editControl.Text);
            }
        }
    }
}
