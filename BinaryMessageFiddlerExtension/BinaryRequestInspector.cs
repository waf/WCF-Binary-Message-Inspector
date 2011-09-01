using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fiddler;
using System.Windows.Forms;
using System.Xml;

namespace BinaryMessageFiddlerExtension
{
    public class BinaryRequestInspector : Inspector2, IRequestInspector2
    {
        private bool isReadOnly;
        private XmlTreeView viewControl;  // read-only tree view of xml
        private TextBox editControl; // writeable text area of xml

        private byte[] binaryContent;

        private static Logger log = new Logger(true);

        public override void AddToTab(TabPage o)
        {
            o.Text = "WCF Binary XML";

            isReadOnly = true;

            // create and add xml tree view as initial control
            viewControl = new XmlTreeView();
            viewControl.BackColor = CONFIG.colorDisabledEdit;
            viewControl.Dock = DockStyle.Fill;
            o.Controls.Add(viewControl);

            // create edit view, but don't add it to the UI yet
            editControl = new TextBox { Multiline = true, ScrollBars = ScrollBars.Vertical };
            editControl.LostFocus += ParseEditedXML;
            editControl.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Update the UI to show either the viewControl or the EditControl, based on the value of isReadOnly
        /// </summary>
        private void SwitchControl()
        {
            Control parent = viewControl.Parent ?? editControl.Parent;
            Control widget = isReadOnly ? viewControl : editControl as Control;
            parent.Controls.Clear();
            parent.Controls.Add(widget);
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
                    editControl.Text = document.OuterXml;
                }
            }
            catch (XmlException ex)
            {
                log.LogString(ex.ToString());
                viewControl.DisplayError(ex.Message);
            }
        }

        /// <summary>
        /// Parse the editControl's text into wcf binary xml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParseEditedXML(object sender, EventArgs e)
        {
            XmlDocument document = new XmlDocument();
            bDirty = true;
            try
            {
                document.LoadXml(editControl.Text);

                binaryContent = WcfBinaryConverter.ConvertXmlToWcfBinary(document);
            }
            catch (XmlException)
            {
                // it's perfectly possible that the user did not enter valid XML.
                // the binaryContent will not have been overwritten, so do nothing
            }
        }

        #region IBaseInspector2 members

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
                // show the correct control based on whether we're in view (read-only) or edit mode.
                SwitchControl();
            }
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
                // when fiddler updates this inspector's content, our controls' content
                binaryContent = value;
                UpdateControlContent();
            }
        }

        #endregion

        #region IRequestInspector2 members

        public HTTPRequestHeaders headers
        {
            get { return null; }
            set { }
        }

        #endregion
    }
}
