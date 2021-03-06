/*  This file is part of Chummer5a.
 *
 *  Chummer5a is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Chummer5a is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Chummer5a.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  You can obtain the full source code for Chummer5a at
 *  https://github.com/chummer5a/chummer5a
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Chummer
{
    static class WinFormsExtensions
    {
        #region Controls Extensions
        /// <summary>
        /// Runs code on a WinForms control in a thread-safe manner.
        /// </summary>
        /// <param name="objControl">Parent control from which Invoke would need to be called.</param>
        /// <param name="funcToRun">Code to run in the form of a delegate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DoThreadSafe(this Control objControl, Action funcToRun)
        {
            if (objControl?.InvokeRequired == true)
                objControl.Invoke(funcToRun);
            else
                funcToRun.Invoke();
        }
        #endregion

        #region ComboBox Extensions
        public static bool IsInitalized(this ComboBox cboThis, bool isLoading)
        {
            return (isLoading || string.IsNullOrEmpty(cboThis?.SelectedValue?.ToString()));
        }
        #endregion

        #region TreeNode Extensions
        public static TreeNode GetTopParent(this TreeNode objThis)
        {
            TreeNode objReturn = objThis;
            while (objReturn.Parent != null)
                objReturn = objReturn.Parent;
            return objReturn;
        }

        /// <summary>
        /// Find a TreeNode in a TreeNode based on its Tag.
        /// </summary>
        /// <param name="strGuid">InternalId of the Node to find.</param>
        /// <param name="objNode">TreeNode to search.</param>
        public static TreeNode FindNode(this TreeNode objNode, string strGuid)
        {
            if (objNode != null && strGuid != Guid.Empty.ToString("D"))
            {
                TreeNode objFound;
                foreach (TreeNode objChild in objNode.Nodes)
                {
                    if (objChild.Tag.ToString() == strGuid)
                        return objChild;

                    objFound = objChild.FindNode(strGuid);
                    if (objFound != null)
                        return objFound;
                }
            }
            return null;
        }
        #endregion

        #region TreeView Extensions
        /// <summary>
        /// Sort the contents of a TreeView alphabetically within each group Node.
        /// </summary>
        /// <param name="treView">TreeView to sort.</param>
        public static void SortCustom(this TreeView treView, string strSelectedNodeTag = "")
        {
            TreeNodeCollection lstTreeViewNodes = treView?.Nodes;
            if (lstTreeViewNodes == null)
                return;
            if (string.IsNullOrEmpty(strSelectedNodeTag))
                strSelectedNodeTag = treView.SelectedNode?.Tag.ToString();
            for (int i = 0; i < lstTreeViewNodes.Count; ++i)
            {
                TreeNode objLoopNode = lstTreeViewNodes[i];
                TreeNodeCollection objLoopNodeChildren = objLoopNode.Nodes;
                int intChildrenCount = objLoopNodeChildren.Count;
                if (intChildrenCount > 0)
                {
                    TreeNode[] lstNodes = new TreeNode[intChildrenCount];
                    objLoopNodeChildren.CopyTo(lstNodes, 0);
                    objLoopNodeChildren.Clear();
                    Array.Sort(lstNodes, CompareTreeNodes.CompareText);
                    objLoopNodeChildren.AddRange(lstNodes);

                    objLoopNode.Expand();
                }
            }

            TreeNode objSelectedNode = treView.FindNode(strSelectedNodeTag);
            if (objSelectedNode != null)
                treView.SelectedNode = objSelectedNode;
        }

        /// <summary>
        /// Clear the background colour for all TreeNodes except the one currently being hovered over during a drag-and-drop operation.
        /// </summary>
        /// <param name="treTree">TreeView to check.</param>
        /// <param name="objHighlighted">TreeNode that is currently being hovered over.</param>
        public static void ClearNodeBackground(this TreeView treView, TreeNode objHighlighted)
        {
            treView?.Nodes.ClearNodeBackground(objHighlighted);
        }

        /// <summary>
        /// Find a TreeNode in a TreeView based on its Tag.
        /// </summary>
        /// <param name="strGuid">InternalId of the Node to find.</param>
        /// <param name="treTree">TreeView to search.</param>
        public static TreeNode FindNode(this TreeView treTree, string strGuid, bool blnDeep = true)
        {
            if (treTree != null && !string.IsNullOrEmpty(strGuid) && strGuid != Guid.Empty.ToString("D"))
            {
                TreeNode objFound;
                foreach (TreeNode objNode in treTree.Nodes)
                {
                    if (objNode.Tag.ToString() == strGuid)
                        return objNode;

                    if (blnDeep)
                    {
                        objFound = objNode.FindNode(strGuid);
                        if (objFound != null)
                            return objFound;
                    }
                }
            }
            return null;
        }
        #endregion

        #region TreeNodeCollection Extensions
        /// <summary>
        /// Recursive method to clear the background colour for all TreeNodes except the one currently being hovered over during a drag-and-drop operation.
        /// </summary>
        /// <param name="objNodes">Parent TreeNodeCollection to check.</param>
        /// <param name="objHighlighted">TreeNode that is currently being hovered over.</param>
        public static void ClearNodeBackground(this TreeNodeCollection objNodes, TreeNode objHighlighted)
        {
            foreach (TreeNode objChild in objNodes)
            {
                if (objChild != objHighlighted)
                    objChild.BackColor = SystemColors.Window;
                objChild.Nodes.ClearNodeBackground(objHighlighted);
            }
        }
        #endregion
    }
}
