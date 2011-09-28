// -----------------------------------------------------------------------
// <copyright file="Splitter.cs" company="Veraida Pty Ltd">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace TextFileSplitter
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Completed file split event
    /// </summary>
    /// <param name="sender">The sender.</param>
    public delegate void FileProcessedHandler(object sender);

    /// <summary>
    /// File written event
    /// </summary>
    /// <param name="sender">The sender.</param>
    public delegate void FileWrittenHandler(object sender);

    /// <summary>
    /// Split file complete event
    /// </summary>
    /// <param name="sender">The sender.</param>
    public delegate void FileCompleteHandler(object sender);

    /// <summary>
    /// This is class where all of the magic happens.  It reads the file, then splits it based
    /// on a regular expression pattern.  It then writes all of the strings to separate files
    /// into the specified directory.
    /// While doing this, it provides a numbers of events (hooks) so that any calling class
    /// can keep track of the status of the split.
    /// </summary>
    public class Splitter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Splitter"/> class.
        /// </summary>
        public Splitter()
        {
            this.FileCount = 0;
            this.WrittenCount = 0;
            this.ErrorCount = 0;
        }

        /// <summary>
        /// Occurs when [file processed event].
        /// </summary>
        public event FileProcessedHandler FileProcessedEvent;

        /// <summary>
        /// Occurs when [file written event].
        /// </summary>
        public event FileWrittenHandler FileWrittenEvent;

        /// <summary>
        /// Occurs when [file complete event].
        /// </summary>
        public event FileCompleteHandler FileCompleteEvent;

        /// <summary>
        /// Gets or sets the File name of the file to be split.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the character, on which to split the file.
        /// </summary>
        /// <value>
        /// The split char.
        /// </value>
        public string SplitChar { get; set; }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        /// <value>
        /// The output directory.
        /// </value>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the output extension, of the new files.
        /// </summary>
        /// <value>
        /// The output extension.
        /// </value>
        public string OutputExtension { get; set; }

        /// <summary>
        /// Gets the number of files that will be created.
        /// </summary>
        public int FileCount { get; private set; }

        /// <summary>
        /// Gets the number of files actually written.
        /// </summary>
        public int WrittenCount { get; private set; }

        /// <summary>
        /// Gets the number of errors encountered.
        /// </summary>
        public int ErrorCount { get; private set; }

        /// <summary>
        /// Processes the file.
        /// Fires the FileProcessed and FileComplete events
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="splitCharacter">The split character.</param>
        /// <param name="outputDirectory">The output directory.</param>
        /// <param name="outputExtension">The output extension.</param>
        public void ProcessFile(string filename, string splitCharacter, string outputDirectory, string outputExtension)
        {
            string file = this.ReadFile(filename);
            string[] splitFiles = this.SplitFile(file, splitCharacter);
            this.FileCount = splitFiles.Length;
            this.FileProcessedEvent(this);
            this.SaveFiles(splitFiles, outputDirectory, outputExtension);
            this.FileCompleteEvent(this);
        }

        /// <summary>
        /// Creates a file.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>Boolean, true if successful</returns>
        private bool CreateFile(string content, string filename)
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(content);
                    sw.Flush();
                }

                return true;
            }
            catch (Exception x)
            {
                throw x;
            }
        }

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The contents of the file</returns>
        private string ReadFile(string filePath)
        {
            TextReader tr = new StreamReader(filePath);
            string file = tr.ReadToEnd();
            return file;
        }

        /// <summary>
        /// Splits the file, based on a string (RegEx) pattern.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns>String array, substring of 'file' formed by the split</returns>
        private string[] SplitFile(string file, string pattern)
        {
            return file.Split(pattern.ToCharArray());
        }

        /// <summary>
        /// Saves the files.
        /// Fires the 'FileWritten' event
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="outputDirectory">The output directory.</param>
        /// <param name="fileExtension">The file extension.</param>
        private void SaveFiles(string[] files, string outputDirectory, string fileExtension)
        {
            foreach (string file in files)
            {
                if (file.Length != 0)
                {
                    if (this.CreateFile(file, string.Format("{0}\\output_{1}.{2}", outputDirectory, this.WrittenCount.ToString(), fileExtension)))
                    {
                        this.WrittenCount++;
                    }
                    else
                    {
                        this.ErrorCount++;
                    }
                }
                else
                {
                    this.ErrorCount++;
                }

                this.FileWrittenEvent(this);
            }
        }
    }
}
