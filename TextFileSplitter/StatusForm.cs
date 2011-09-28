// -----------------------------------------------------------------------
// <copyright file="StatusForm.cs" company="Veraida Pty Ltd">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TextFileSplitter
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The form that displays the real-time status of the file split
    /// </summary>
    public partial class StatusForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusForm"/> class.
        /// </summary>
        public StatusForm()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the CloseButton control.
        /// Closes the application.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CloseButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}