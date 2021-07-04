﻿// Marathon is licensed under the MIT License:
/* 
 * MIT License
 * 
 * Copyright (c) 2021 HyperBE32
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Specialized;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Toolkit;
using WeifenLuo.WinFormsUI.Docking;
using Marathon.Components;
using Marathon.Components.Helpers;

namespace Marathon.Toolkit.Forms
{
    public partial class Workspace : KryptonForm
    {
        public Workspace()
        {
            InitializeComponent();

            // Adds the extended information to the status label.
            ToolStripStatusLabel_Version.Text = Program.GetExtendedInformation(string.Empty, false);

            // Self-explanatory.
            RestoreSettings();
            BackupRibbon();
            LoadRecentDocuments();
        }

        /// <summary>
        /// Updates the status label with the input text.
        /// </summary>
        /// <param name="status">Current status.</param>
        public void UpdateStatus(string status)
            => ToolStripStatusLabel_Status.Text = status;

        /// <summary>
        /// Restores previously saved settings.
        /// </summary>
        private void RestoreSettings()
        {
#if DEBUG
            // Displays the debug tab on the ribbon.
            KryptonRibbonGroup_Marathon_Developer.Visible = true;
#endif
            // Set window properties from settings.
            Width = Marathon.Properties.Settings.Default.Workspace_WindowWidth;
            Height = Marathon.Properties.Settings.Default.Workspace_WindowHeight;
            WindowState = Marathon.Properties.Settings.Default.Workspace_WindowState;

            // Set dock panel float window size based on user set form size.
            DockPanel_Workspace.DefaultFloatWindowSize = Size;

            // Set ribbon minimised state.
            KryptonRibbon_Workspace.MinimizedMode = Marathon.Properties.Settings.Default.Workspace_RibbonCollapsed;
        }

        /// <summary>
        /// Backs up the contents of the ribbon.
        /// </summary>
        private void BackupRibbon()
        {
            // Store the original ribbon for when there are no open documents.
            RibbonHelper.DefaultRibbonTabs = KryptonRibbon_Workspace.RibbonTabs.ToArray();
            RibbonHelper.DefaultRibbonAppButtonMenu = KryptonRibbon_Workspace.RibbonAppButton.AppButtonMenuItems.ToArray();
        }

        /// <summary>
        /// Adds a file to the recent documents list by path.
        /// </summary>
        private void AddRecentDocument(string path)
        {
            // Store collection for easier reference.
            StringCollection cmnCollection = Marathon.Properties.Settings.Default.Workspace_RecentDocuments;

            // Add the file to the collection.
            {
                // Remove if there's a duplicate.
                if (cmnCollection.Contains(path))
                    cmnCollection.Remove(path);

                // Insert at the top.
                cmnCollection.Insert(0, path);
            }

            // Refresh the recent documents.
            LoadRecentDocuments();
        }

        /// <summary>
        /// Loads the recent documents from the configuration.
        /// </summary>
        private void LoadRecentDocuments()
        {
            // Initialise the string collection if null.
            if (Marathon.Properties.Settings.Default.Workspace_RecentDocuments == null)
            {
                /* For whatever reason, .NET doesn't do this by default,
                   despite it being one of the main options for .NET Settings. */
                Marathon.Properties.Settings.Default.Workspace_RecentDocuments = new StringCollection();

                return;
            }

            // Store collection for easier reference.
            StringCollection cmnCollection = Marathon.Properties.Settings.Default.Workspace_RecentDocuments;

            // Clear the recent documents list.
            KryptonRibbon_Workspace.RibbonAppButton.AppButtonRecentDocs.Clear();

            for (int i = 0; i < cmnCollection.Count; i++)
            {
                // Recent document as a string.
                string document = cmnCollection[i];

                // Create recent document.
                KryptonRibbonRecentDoc recentDoc = new KryptonRibbonRecentDoc
                {
                    Text = document
                };

                // Click event for the document.
                recentDoc.Click += delegate
                {
                    // File exists, open the file.
                    if (File.Exists(document))
                    {
                        // Move to the top.
                        cmnCollection.Remove(document);
                        cmnCollection.Insert(0, document);

                        // Open the file.
                        OpenFile(document);
                    }

                    // Otherwise, remove it from the recent list.
                    else
                    {
                        MarathonMessageBox.Show("This file no longer exists - removing from recent documents...");

                        // Remove the current document.
                        cmnCollection.Remove(document);
                    }

                    // Refresh the recent documents.
                    LoadRecentDocuments();
                };

                // Add this file to the recent documents.
                KryptonRibbon_Workspace.RibbonAppButton.AppButtonRecentDocs.Add(recentDoc);
            }
        }

        /// <summary>
        /// Returns the current address of the active MarathonExplorer document.
        /// </summary>
        private string ActiveMarathonExplorerAddress()
        {
            // Return the current address from the active Marathon Explorer document.
            if (DockPanel_Workspace.ActiveDocument != null && DockPanel_Workspace.ActiveDocument is MarathonExplorer explorer)
                return explorer != null ? explorer.CurrentAddress : string.Empty;

            return string.Empty;
        }

        /// <summary>
        /// Launches Task Dashboard upon selecting a location.
        /// </summary>
        /// <param name="path">Requested location.</param>
        /// <param name="taskState">Task state.</param>
        private void OpenFile(string path, TaskDashboard.TaskState taskState = TaskDashboard.TaskState.Open)
            => new TaskDashboard(DockPanel_Workspace, path, taskState).ShowDialog(KryptonRibbon_Workspace);

        /// <summary>
        /// Displays the About form upon clicking.
        /// </summary>
        private void ButtonSpecAppMenu_AboutMarathon_Click(object sender, EventArgs e)
            => new About().ShowDialog();

        /// <summary>
        /// Prompts the user to create a new file.
        /// </summary>
        private void KryptonContextMenuItem_File_New_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Title = "New...",
                Filter = Resources.ParseFileTypesToFilter(Marathon.Properties.Resources.FileTypes),
                InitialDirectory = ActiveMarathonExplorerAddress()
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
                OpenFile(saveDialog.FileName, TaskDashboard.TaskState.New);
        }

        /// <summary>
        /// Prompts the user for a file...
        /// </summary>
        private void KryptonContextMenuItem_File_Open_Items_OpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Title = "Please select a file...",
                Filter = Resources.ParseFileTypesToFilter(Marathon.Properties.Resources.FileTypes),
                InitialDirectory = ActiveMarathonExplorerAddress()
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                // Store common path for easier reference.
                string cmnPath = fileDialog.FileName;

                // Add file to recent documents.
                AddRecentDocument(cmnPath);

                // Open the Task Dashboard.
                OpenFile(cmnPath);
            }
        }

        /// <summary>
        /// Prompts the user for a folder...
        /// </summary>
        private void KryptonContextMenuItem_File_Open_Items_OpenFolder_Click(object sender, EventArgs e)
        {
            OpenFolderDialog folderDialog = new OpenFolderDialog
            {
                Title = "Please select a folder..."
            };

            if (folderDialog.ShowDialog() == DialogResult.OK)
                new MarathonExplorer(folderDialog.SelectedPath).Show(DockPanel_Workspace, KryptonRibbon_Workspace);
        }

        /// <summary>
        /// Displays the MarathonLog form upon clicking.
        /// </summary>
        private void PendingParent_Output_Click(object sender, EventArgs e)
            => new Output().Show(DockPanel_Workspace);

        /// <summary>
        /// Displays the Windows form upon clicking.
        /// </summary>
        private void KryptonContextMenuItem_File_Windows_Click(object sender, EventArgs e)
            => new Windows().ShowDialog();

        /// <summary>
        /// Displays the Options form upon clicking.
        /// </summary>
        private void ButtonSpecAppMenu_MarathonOptions_Click(object sender, EventArgs e)
            => new Options().Show(DockPanel_Workspace);

        /// <summary>
        /// Exits the application upon clicking.
        /// </summary>
        private void ButtonSpecAppMenu_ExitMarathon_Click(object sender, EventArgs e)
            => Application.Exit();

        /// <summary>
        /// Displays the File Converter form upon clicking.
        /// </summary>
        private void PendingParent_FileConverter_Click(object sender, EventArgs e)
            => new FileConverter().Show(DockPanel_Workspace);

        /// <summary>
        /// Redirects the user to the GitHub issues page.
        /// </summary>
        private void PendingParent_SendFeedback_Click_Group(object sender, EventArgs e)
        {
            // TODO: find a suitable parent.

            //string commonTitle = "[Marathon.Toolkit]";

            //// Bug report...
            //if (sender.Equals(MenuStripDark_Main_Help_SendFeedback_ReportAProblem))
            //    Program.InvokeFeedback(commonTitle, string.Empty, "bug");

            //// Feature request...
            //else if (sender.Equals(MenuStripDark_Main_Help_SendFeedback_SuggestAFeature))
            //    Program.InvokeFeedback(commonTitle, string.Empty, "enhancement");
        }

        private void KryptonRibbonGroupButton_Home_Organise_BulkRenamer_Click(object sender, EventArgs e)
        {
            string path = ActiveMarathonExplorerAddress();

            if (!string.IsNullOrEmpty(path))
            {
                // Launch Bulk Renamer from Marathon Explorer.
                new BulkRenamer(path).ShowDialog();
            }
            else
            {
                OpenFolderDialog folderDialog = new OpenFolderDialog
                {
                    Title = "Please select a folder..."
                };

                // Launch Bulk Renamer from requested path.
                if (folderDialog.ShowDialog() == DialogResult.OK)
                    new BulkRenamer(folderDialog.SelectedPath).ShowDialog();
            }
        }

        /// <summary>
        /// Group event handler for KryptonRibbonGroup_Developer_Tools.
        /// </summary>
        private void KryptonRibbonGroup_Developer_Tools_Click_Group(object sender, EventArgs e)
        {
            // Displays the Debugger form upon clicking.
            if (sender == KryptonRibbonGroupButton_Marathon_Developer_Debugger)
            {
                new Debugger().Show(DockPanel_Workspace, KryptonRibbon_Workspace);
            }

            // Resets Marathon's settings upon clicking.
            else if (sender == KryptonRibbonGroupButton_Marathon_Developer_ResetSettings)
            {
                Marathon.Properties.Settings.Default.Reset();
            }
        }

        /// <summary>
        /// Closing events for the main form.
        /// </summary>
        private void Workspace_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Close all open documents.
            for (int i = 0; i < Application.OpenForms.OfType<DockContent>().Count(); i++)
            {
                Application.OpenForms.OfType<DockContent>().ToList()[i].Close();
            }

            // Save the last used window state.
            Marathon.Properties.Settings.Default.Workspace_WindowState = WindowState;

            // Save all modified settings.
            Marathon.Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Events for when the user stops resizing.
        /// </summary>
        private void Workspace_ResizeEnd(object sender, EventArgs e)
        {
            // Save the last used window size.
            Marathon.Properties.Settings.Default.Workspace_WindowWidth = Width;
            Marathon.Properties.Settings.Default.Workspace_WindowHeight = Height;

            // Set dock panel float window size based on current form size.
            DockPanel_Workspace.DefaultFloatWindowSize = Size;
        }

        /// <summary>
        /// Events for when the user closes controls from the dock panel.
        /// </summary>
        private void DockPanel_Workspace_ContentRemoved(object sender, DockContentEventArgs e)
        {
            // No documents are open, so the ribbon should be reset to default.
            if (DockPanel_Workspace.Contents.Count == 0 && !RibbonHelper.IsRibbonDefault(KryptonRibbon_Workspace))
                RibbonHelper.SetupRibbon(KryptonRibbon_Workspace, RibbonHelper.DefaultRibbonTabs, null);
        }

        /// <summary>
        /// Save the ribbon's minimised state.
        /// </summary>
        private void KryptonRibbon_Workspace_MinimizedModeChanged(object sender, EventArgs e)
            => Marathon.Properties.Settings.Default.Workspace_RibbonCollapsed = KryptonRibbon_Workspace.MinimizedMode;

        /// <summary>
        /// Opens the output tool window.
        /// </summary>
        private void KryptonContextMenuItem_File_Output_Click(object sender, EventArgs e)
            => new Output().Show(DockPanel_Workspace, DockState.DockBottom);
    }
}