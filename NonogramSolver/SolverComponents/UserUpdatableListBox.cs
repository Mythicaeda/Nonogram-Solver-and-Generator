using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace NonogramSolverGenerator.SolverComponents
{
    public partial class UserUpdatableListBox : UserControl
    {
        //public event EventHandler EnterPressed;
        //string pattern = @"(\b\d+\b\s*)+";
        [Description("The Regex pattern used to validate new entries to the ListBox."),Category("Data")] 
        public string Pattern { get; set; }

        //adding ToolTip compatibility to all the controls
        [Description("Determines the ToolTip shown when the mouse hovers over the Add Button.")]
        public string BtnAddToolTip
        {
            get
            {
                return toolTip.GetToolTip(btnAdd);
            }
            set
            {
                toolTip.SetToolTip(btnAdd, value);
            }
        }
        [Description("Determines the ToolTip shown when the mouse hovers over the Remove Button.")]
        public string BtnRemoveToolTip
        {
            get
            {
                return toolTip.GetToolTip(btnRemove);
            }
            set
            {
                toolTip.SetToolTip(btnRemove, value);
            }
        }
        [Description("Determines the ToolTip shown when the mouse hovers over the RemoveAll Button.")]
        public string BtnRemoveAllToolTip
        {
            get
            {
                return toolTip.GetToolTip(btnRemoveAll);
            }
            set
            {

                toolTip.SetToolTip(btnRemoveAll, value);
            }
        }
        [Description("Determines the ToolTip shown when the mouse hovers over the ListBox.")]
        public string ListBoxToolTip
        {
            get
            {
                return toolTip.GetToolTip(listBox);
            }
            set
            {

                toolTip.SetToolTip(listBox, value);
            }
        }
        [Description("Determines the ToolTip shown when the mouse hovers over the Name Label.")]
        public string LabelToolTip
        {
            get
            {
                return toolTip.GetToolTip(lblName);
            }
            set
            {

                toolTip.SetToolTip(lblName, value);
            }
        }

        //properties
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Bindable(true)]
        public override string Text { get { return lblName.Text; } set { lblName.Text = value; } }
        public ListBox ListBox { get { return listBox; } }
        public Button RemoveAllButtom { get { return btnRemoveAll; } }


        public UserUpdatableListBox()
        {
            InitializeComponent();
            Pattern = @"^(\b\d+\b\s*)+$";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (listBox.Items.Count < 1000)
            {
                listBox.Items.Add(tbNewEntry.Text.Trim());
                tbNewEntry.Text = "";
                //we have at least one item now
                if (!btnRemoveAll.Enabled) btnRemoveAll.Enabled = true;
            }
            else
            {
                MessageBox.Show("Too many items in List.", "Can't Add Item", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbNewEntry.Text = "";
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            //remove all the items currently selected
            for (int i = listBox.SelectedIndices.Count - 1; i >= 0; i--)
            {
                listBox.Items.RemoveAt(listBox.SelectedIndices[i]);
            }

            //if there are no more items, 
            if (listBox.Items.Count == 0) btnRemoveAll.Enabled = false;
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            btnRemoveAll.Enabled = false;
        }

        private void tbNewEntry_TextChanged(object sender, EventArgs e)
        {
            if (Regex.Matches(tbNewEntry.Text, Pattern).Count == 1)
            {
                btnAdd.Enabled = true;
            }   
            else
            {
                btnAdd.Enabled = false;
            }
        }

        private void tbNewEntry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && btnAdd.Enabled)
            {
                btnAdd_Click(this, e);
            }
        }

        //this method is raised whenever an item is selected or deselected in multiselect mode
        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox.SelectedIndices.Count >= 1)
            {
                btnRemove.Enabled = true;
            }
            else
            {
                btnRemove.Enabled = false;
            }
        }

        private void listBox_Leave(object sender, EventArgs e)
        {
            listBox.SelectedIndices.Clear();
        }
    }
}
