// -----------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Veraida Pty Ltd">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TextFileSplitter
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// The main form, where the user selects the file to be split and enters the character
    /// on which the file should be split.
    /// The user also selects the directory where the new files are to be saved, and their
    /// extension.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// The splitter class, that does the actual file splitting
        /// </summary>
        private Splitter splitter;

        /// <summary>
        /// The status form, to show current status of the file splitting
        /// </summary>
        private StatusForm formStatus = new StatusForm();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.splitter = new Splitter();
            this.splitter.FileProcessedEvent += new FileProcessedHandler(this.Splitter_FileProcessedEvent);
            this.splitter.FileWrittenEvent += new FileWrittenHandler(this.Splitter_FileWrittenEvent);
            this.splitter.FileCompleteEvent += new FileCompleteHandler(this.Splitter_FileCompleteEvent);
        }

        /// <summary>
        /// Splitter_s the file complete event.
        /// Updates the Status dialog
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void Splitter_FileCompleteEvent(object sender)
        {
            this.formStatus.lblStatus.Text = "Split Complete...";
            this.formStatus.lblProgress.Text = string.Format("Files Found: {0}, Files Written: {1}, Errors: {2}", this.splitter.FileCount.ToString(), this.splitter.WrittenCount.ToString(), this.splitter.ErrorCount.ToString());
            this.formStatus.CloseButton.Enabled = true;
        }

        /// <summary>
        /// Splitter_s the file processed event.
        /// Updates the Status dialog
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void Splitter_FileProcessedEvent(object sender)
        {
            this.formStatus.lblStatus.Text = "File read, writing files...";
            this.formStatus.lblProgress.Text = string.Format("File {0} of {1} written...", this.splitter.WrittenCount, this.splitter.FileCount);
            this.formStatus.pbProgress.Maximum = this.splitter.FileCount;
            this.formStatus.pbProgress.Minimum = 0;
            this.formStatus.pbProgress.Value = 0;
            this.formStatus.Refresh();
            Application.DoEvents();
        }

        /// <summary>
        /// Splitter_s the file written event.
        /// Updates the status dialog
        /// </summary>
        /// <param name="sender">The sender.</param>
        private void Splitter_FileWrittenEvent(object sender)
        {
            this.formStatus.lblProgress.Text = string.Format("File {0} of {1} written...", this.splitter.WrittenCount, this.splitter.FileCount);
            this.formStatus.pbProgress.Value = this.splitter.WrittenCount;
            this.formStatus.Refresh();
            Application.DoEvents();
        }

        /// <summary>
        /// Handles the MouseClick event of the tbFilename control.
        /// Opens a file dialog for the user to select the file to be split
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void FilenameField_MouseClick(object sender, MouseEventArgs e)
        {
            this.openFileDialog1.ShowDialog();
            this.FilenameField.Text = this.openFileDialog1.FileName;
        }

        /// <summary>
        /// Handles the MouseClick event of the tbOutputDir control.
        /// Opens a folder browser dialog so the user can navigate and select a directory
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void OutputDirectoryField_MouseClick(object sender, MouseEventArgs e)
        {
            this.folderBrowserDialog1.ShowDialog();
            this.OutputDirectoryField.Text = this.folderBrowserDialog1.SelectedPath;
        }

        /// <summary>
        /// Handles the Click event of the btnSplit control.
        /// Validates the user entry before starting the file split process
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SplitButton_Click(object sender, EventArgs e)
        {
            if (this.ValidEntry())
            {
                this.formStatus.Show();
                this.ProcessFile();
            }
        }

        /// <summary>
        /// Processes the file.
        /// Calls the <see cref="Splitter"/> class
        /// </summary>
        private void ProcessFile()
        {
            this.splitter.ProcessFile(this.FilenameField.Text, this.SplitCharField.Text, this.OutputDirectoryField.Text, this.OutputExtField.Text);
        }

        /// <summary>
        /// Valids the entry.
        /// </summary>
        /// <returns>True if valid</returns>
        private bool ValidEntry()
        {
            bool valid = true;

            if (!File.Exists(this.FilenameField.Text))
            {
                this.errorProv.SetError(this.FilenameField, "Cannot find file, please update before trying again.");
                valid = false;
            }

            if (this.SplitCharField.Text.Length < 1)
            {
                this.errorProv.SetError(this.SplitCharField, "Split character required");
                valid = false;
            }

            if (!Directory.Exists(this.OutputDirectoryField.Text))
            {
                this.errorProv.SetError(this.OutputDirectoryField, "Unable to locate output directory, please update before trying again.");
                valid = false;
            }

            if (this.OutputExtField.Text.Length < 1)
            {
                this.errorProv.SetError(this.OutputExtField, "Output file extension required");
                valid = false;
            }

            return valid;
        }
    }
}