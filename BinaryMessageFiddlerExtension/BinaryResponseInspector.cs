using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fiddler;
using System.Windows.Forms;
using System.Xml;

namespace BinaryMessageFiddlerExtension
{
    public class BinaryResponseInspector : Inspector2, IResponseInspector2
    {
        private XmlTreeView viewControl;  // read-only tree view of xml
        
        private byte[] binaryContent;

        private static Logger log = new Logger(true);

        public override void AddToTab(TabPage o)
        {
            o.Text = "WCF Binary";

            viewControl = new XmlTreeView();
            viewControl.BackColor = CONFIG.colorDisabledEdit;
            viewControl.Dock = DockStyle.Fill;
            o.Controls.Add(viewControl);
        }

        /// <summary>
        /// Update the controls with the new binary content
        /// </summary>
        private void UpdateControlContent()
        {
            try
            {
                XmlDocument document = WcfBinaryConverter.ConvertWcfBinaryMessageToXml(binaryContent);
                if (document == null || document.ChildNodes.Count == 0)
                {
                    this.Clear();
                }
                else
                {
                    viewControl.LoadXml(document);
                }
            }
            catch (XmlException ex)
            {
                log.LogString(ex.ToString());
                viewControl.DisplayError(ex.Message);
            }
        }

        #region IBaseInspector2 members

        public override int GetOrder()
        {
            return 1;
        }

        public bool bDirty 
        { 
            get { return false; } 
        }

        public bool bReadOnly 
        { 
            get { return false; }
            set { } 
        }

        public void Clear()
        {
            viewControl.Clear();
        }

        public byte[] body
        {
            get { return binaryContent; }
            set
            {
                // when fiddler updates this inspector's content, our control's content
                binaryContent = value;
                UpdateControlContent();
            }
        }

        #endregion

        #region IResponseInspector2 members

        public HTTPResponseHeaders headers
        {
            get { return null; }
            set { }
        }

        #endregion
    }
}
