using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClipboardHelper.ViewModel;
using ClipboardHelper.ClipboardManagerMonitor;
using System.ComponentModel;
using MahApps.Metro.Controls;

namespace ClipboardHelper.View
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        TextHandler handler = new TextHandler(); // This is where text storing happens and adding

        public MainWindow()
        {
            InitializeComponent();
            handler.CreateJsonFile();
            DisplayToListBox();
            // Below this adds Commandbinding which activates user presses copys text from listbox
            CommandBinding cb = new CommandBinding(ApplicationCommands.Copy, CopyCmdExecuted, CopyCmdCanExecute);
            listBox1.CommandBindings.Add(cb);
            Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // This will start the ClipboardManager which will monitor when users clipboard changes
            var windowClipboardManager = new ClipboardManager(this);
            windowClipboardManager.ClipboardChanged += ClipboardChanged;
        }

        /// <summary>
        /// This will start when clipboard changes. It checks if its text, if it is it will add it to a list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClipboardChanged(object sender, EventArgs e)
        {

            // Handle your clipboard update here
            if (Clipboard.ContainsText())
            {
                handler.AddJson(Clipboard.GetText());
            }
            DisplayToListBox();
        }



        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        /// <summary>
        /// Handles text displaying for the listbox
        /// </summary>
        private void DisplayToListBox()
        {
            listBox1.Items.Clear();
            List<string> list2 = handler.GetCopiedJsonTexts();
            if (list2 != null)
            {
                foreach (string text in list2)
                {
                    listBox1.Items.Add(text);
                }
            }
        }

        /// <summary>
        /// Where the ctrl + c copying happens
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        void CopyCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            ListBox lb = e.OriginalSource as ListBox;
            // The SelectedItems could be ListBoxItem instances or data bound objects depending on how you populate the ListBox.  
            string copyContent = lb.SelectedItem.ToString();

            Clipboard.SetText(copyContent);
        }

        /// <summary>
        /// For checking if the user has selected some text, if not it does nothing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ListBox lb = e.OriginalSource as ListBox;
            // CanExecute only if there is selected item.
            if (lb.SelectedItem != null)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        /// <summary>
        /// This handles the right click menu copying, because it works differently.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                return;
            }
            string copyContent = listBox1.SelectedItem.ToString();
            Clipboard.SetText(copyContent);
        }

        private void MenuItemWithRadioButtons_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                RadioButton rb = mi.Icon as RadioButton;
                if (rb != null)
                {
                    rb.IsChecked = true;
                    handler.SetMaxListLength(int.Parse(mi.Header.ToString()));
                    DisplayToListBox();
                }
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuClear_Click(object sender, RoutedEventArgs e)
        {
            handler.RemoveFileAndFolder();
            Application.Current.Shutdown();
        }
    }
}