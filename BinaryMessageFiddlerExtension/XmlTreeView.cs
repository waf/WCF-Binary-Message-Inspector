using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace BinaryMessageFiddlerExtension
{
    public partial class XmlTreeView : UserControl
    {
        private StringBuilder elementString = new StringBuilder();

        // if the message body has fewer than this number of nodes, expand the entire body subtree.
        private const int AutoExpandAllThreshold = 50; //entirely arbitrary.

        public TreeView TreeView
        {
            get
            {
                return treeView1;
            }
        }

        public XmlTreeView()
        {
            InitializeComponent();
            BuildContextMenu();
        }

        private void BuildContextMenu()
        {
            TreeView.ContextMenu = new ContextMenu();

            // NodeMouseClick doesn't fire for middle click, so emulate that using mouseup/mousedown
            TreeView.MouseUp += new MouseEventHandler(TreeView_MouseUp);
            TreeView.MouseDown += new MouseEventHandler(TreeView_MouseDown);
            TreeView.ContextMenu.MenuItems.Add(new MenuItem("Expand All", ExpandAll_Click, Shortcut.AltRightArrow));
            TreeView.ContextMenu.MenuItems.Add(new MenuItem("Collapse All", CollapseAll_Click, Shortcut.AltLeftArrow));
            TreeView.ContextMenu.MenuItems.Add(new MenuItem("Copy", CopyMenuItem_Click, Shortcut.CtrlC));
            TreeView.ContextMenu.MenuItems.Add(new MenuItem("Copy Tree", CopyTreeMenuItem_Click, Shortcut.CtrlShiftC));
        }

        public void LoadXml(XmlDocument doc)
        {
            Clear();

            if (doc == null || doc.FirstChild == null)
            {
                DisplayError("Invalid XML Provided.");
                return;
            }

            TreeView.BeginUpdate();
            PopulateTreeNodesFromXml(doc.FirstChild);
            SetInitialTreeNodeVisibilty();
            TreeView.EndUpdate();
        }

        public void DisplayError(String error)
        {
            Clear();
            TreeView.Nodes.Add(error);
        }

        public void Clear()
        {
            TreeView.BeginUpdate();
            TreeView.Nodes.Clear();
            TreeView.EndUpdate();
        }

        /// <summary>
        /// Helper method, calls recursive PopulateTreeNodesFromXml
        /// </summary>
        private void PopulateTreeNodesFromXml(XmlNode xmlNode)
        {
            PopulateTreeNodesFromXml(TreeView.Nodes, xmlNode);
        }

        /// <summary>
        /// Construct a tree from the provided xml hierarchy
        /// </summary>
        /// <param name="treeNodes">The treenode to populate</param>
        /// <param name="xmlNode">The XML population source</param>
        private void PopulateTreeNodesFromXml(TreeNodeCollection treeNodes, XmlNode xmlNode)
        {
            TreeNode newTreeNode = treeNodes.Add(xmlNode.Name);
            // keep a reference to the xml node that this treenode represents
            newTreeNode.Tag = xmlNode; 

            // set the .Text property of our new treenode
            switch (xmlNode.NodeType)
            {
                case XmlNodeType.ProcessingInstruction:
                case XmlNodeType.XmlDeclaration:
                    newTreeNode.Text = "<?" + xmlNode.Name + " " + xmlNode.Value + "?>";
                    break;
                case XmlNodeType.Element:
                    elementString.Remove(0, elementString.Length);
                    elementString.Append("<" + xmlNode.Name);

                    // if we're working on an xml element, we need to print the attributes
                    if (xmlNode.Attributes != null)
                    {
                        foreach (XmlAttribute attribute in xmlNode.Attributes)
                        {
                            elementString.AppendFormat(" {0}=\"{1}\"", attribute.Name, attribute.Value);
                        }
                    }

                    // if there are no children, make this a self-closing tag
                    if (xmlNode.ChildNodes.Count == 0)
                        elementString.Append(" /");

                    elementString.Append(">");
                    newTreeNode.Text = elementString.ToString();
                    break;
                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                    String text = xmlNode.Value;
                    /* 
                     * If the text value is really long, the treeview can hang when the user
                     * selects it or hovers over it (due to the tooltip). Elide the value
                     * at 500 characters. Since copy-to-clipboard relies on the xmlnode, 
                     * rather than the treenode, when the user copies it they'll get the 
                     * full value.
                     */
                    if (xmlNode.Value != null && xmlNode.Value.Length > 500)
                        text = text.Substring(0, 500) + "...";
                    newTreeNode.Text = text;
                    break;
                case XmlNodeType.Comment:
                    newTreeNode.Text = "<!--" + xmlNode.Value + "-->";
                    break;
            }

            //recurse down the xml tree
            foreach (XmlNode childNode in xmlNode.ChildNodes) 
            {
                PopulateTreeNodesFromXml(newTreeNode.Nodes, childNode);
            }
        }

        /// <summary>
        /// To be nice to the user, autoexpand some of the nodes
        /// </summary>
        private void SetInitialTreeNodeVisibilty()
        {
            //expand the soap envelope and body nodes, if they exist
            if (TreeView.Nodes.Count > 0)
            {
                TreeNode envelopeElement = TreeView.Nodes[0];
                envelopeElement.Expand();

                if (envelopeElement.Nodes.Count == 2)
                {
                    TreeNode bodyElement = TreeView.Nodes[0].Nodes[1];

                    // if it's a small message, expand the full body tree
                    // otherwise, just expand the body
                    int bodySize = bodyElement.GetNodeCount(true);
                    if (bodySize < AutoExpandAllThreshold)
                        bodyElement.ExpandAll();
                    else
                        bodyElement.Expand();

                    bodyElement.EnsureVisible();
                }
            }
        }

        #region TreeView Actions

        private void CopySelectedNodeContentToClipboard()
        {
            if (TreeView.SelectedNode != null && TreeView.SelectedNode.Tag is XmlNode)
            {
                String copyText = String.Empty;
                XmlNode node = (XmlNode)TreeView.SelectedNode.Tag;
                switch (node.NodeType)
                {
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                        // we may truncate the display of Text and CDATA nodes, so use the original xmlnode value
                        copyText = node.Value;
                        break;
                    default:
                        // otherwise, just use the display string
                        copyText = TreeView.SelectedNode.Text;
                        break;
                }
                System.Windows.Forms.Clipboard.SetText(copyText);
            }
        }

        private void CopySubtreeContentToClipboard()
        {
            if (TreeView.SelectedNode == null)
                return;

            String subtreeContent = GenerateSubtreeString(TreeView.SelectedNode);
            if (subtreeContent != null)
                System.Windows.Forms.Clipboard.SetText(subtreeContent);
        }

        private string GenerateSubtreeString(TreeNode treeNode)
        {
            XmlNode xml = treeNode.Tag as XmlNode;
            return (xml == null) ? null : xml.OuterXml;
        }

        private void ExpandAllSelectedNode()
        {
            if (TreeView.SelectedNode != null)
            {
                TreeView.BeginUpdate();
                TreeView.SelectedNode.ExpandAll();
                TreeView.SelectedNode.EnsureVisible();
                TreeView.EndUpdate();
            }
        }

        private void CollapseAllSelectedNode()
        {
            if (TreeView.SelectedNode != null)
            {
                TreeView.BeginUpdate();
                TreeView.SelectedNode.Collapse(false);
                TreeView.EndUpdate();
            }
        }

        #endregion

        #region Event Handlers

        private void TreeView_MouseDown(object sender, MouseEventArgs e)
        {
            // we have a middle click action, but the event NodeMouseClick isn't fired for
            // middle click. We emulate nodemouseclick with mousedown/mouseup
            TreeNode node = TreeView.GetNodeAt(e.X, e.Y);
            if (node != null)
            {
                TreeView.SelectedNode = node;
            }
        } 

        private void TreeView_MouseUp(object sender, MouseEventArgs e)
        {
            // On middle click, toggle expandall/collapseall
            if (e.Button == MouseButtons.Middle)
            {   
                if (TreeView.GetNodeAt(e.X, e.Y) != TreeView.SelectedNode)
                    return;

                if (TreeView.SelectedNode.IsExpanded)
                    CollapseAllSelectedNode();
                else
                    ExpandAllSelectedNode();
            }
        } 

        private void CopyMenuItem_Click(Object sender, System.EventArgs e)
        {
            CopySelectedNodeContentToClipboard();
        }

        private void CopyTreeMenuItem_Click(Object sender, System.EventArgs e)
        {
            CopySubtreeContentToClipboard();
        }

        private void ExpandAll_Click(Object sender, System.EventArgs e)
        {
            ExpandAllSelectedNode();
        }

        private void CollapseAll_Click(Object sender, System.EventArgs e)
        {
            CollapseAllSelectedNode();
        }

        #endregion
    }
}
