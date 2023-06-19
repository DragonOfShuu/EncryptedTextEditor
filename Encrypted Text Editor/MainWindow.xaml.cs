using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Encrypted_Text_Editor.Utils;
using Path = System.IO.Path;

namespace Encrypted_Text_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string? currentFile;
        private bool IsSaved = true;
        private bool isSaved
        {
            get { return IsSaved; }
            set
            {
                IsSaved = value;
                UpdateTitle();
            }
        }

        private string TitleBarText = "Untitled";
        private string titleBarText
        {
            get { return TitleBarText; }
            set
            {
                TitleBarText = value;
                UpdateTitle();
            }
        }

        public static RoutedCommand SaveFile = new RoutedCommand();
        public static RoutedCommand SaveAsFile = new RoutedCommand();

        private void UpdateTitle()
        {
            TheTitleBar.Text = titleBarText + (isSaved ? "" : "*");
        }

        public MainWindow()
        {
            // Initiate Shortcuts
            SaveFile.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            SaveAsFile.InputGestures.Add( new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) );

            InitializeComponent();
        }

        private void XButtonClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBarDragged(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void MaximizeWindow(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            } 
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void UpdateMainTextBox(object sender, TextChangedEventArgs e)
        {
            isSaved = false;
            var range = new TextRange(MainTextBox.Document.ContentStart, MainTextBox.Document.ContentEnd);
            CharacterCounter.Text = range.Text.Trim().Length.ToString();
            WordCounter.Text = range.Text.Trim().Split(" ").Length.ToString();
            LineCounter.Text = range.Text.Trim().Split("\n").Length.ToString();

        }

        private void Save()
        {
            if (currentFile == null)
            {
                SaveAs();
            }
            else
            {
                TextRange range;
                FileStream fStream;
                range = new TextRange(MainTextBox.Document.ContentStart, MainTextBox.Document.ContentEnd);
                fStream = new FileStream(currentFile, FileMode.Create);
                //range.Save(fStream, DataFormats.XamlPackage);
                //fStream.Write()
                fStream.Close();
                isSaved = true;
            }
        }

        private void SaveAs()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Document"; 
            dlg.DefaultExt = ".rat"; 
            dlg.Filter = "Ratch Documents (.ratch)|*.ratch"; 

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                titleBarText = dlg.SafeFileName;
                currentFile = Path.GetFullPath(dlg.FileName);
                Save();
            }
        }

        private void MenuItem_Click_Save(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void MenuItem_Click_SaveAs(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void MenuItem_Click_Quit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CommandBinding_Executed_Save(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private void CommandBinding_Executed_SaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            SaveAs();
        }

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Ratch Documents (.ratch)|*.ratch";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                titleBarText = dlg.SafeFileName;
                string fileName = Path.GetFullPath(dlg.FileName);

                TextRange range;
                FileStream fStream;
                if (File.Exists(currentFile))
                {
                    try
                    {
                        range = new TextRange(MainTextBox.Document.ContentStart, MainTextBox.Document.ContentEnd);
                        fStream = new FileStream(fileName, FileMode.OpenOrCreate);
                        range.Load(fStream, DataFormats.XamlPackage);
                        fStream.Close();
                    } catch (Exception f)
                    {
                        MessageBox.Show(f.Message);
                        return;
                    }
                    
                }
            }
        }

        //private override void OnClosing(object sender, CancelEventArgs e)
        //{
        //    e.Cancel = true;
        //    //Do whatever you want here..
        //}

        protected override void OnClosing(CancelEventArgs e)
        {
            // base.OnClosing(e);
            if (!isSaved)
            {
                e.Cancel = ShowSaveDialog();
            }
        }

        /// <summary>
        ///     <returns>
        ///     Returns false if the application should continue/quit, true if cancel
        ///     </returns>
        ///</summary>
        private bool ShowSaveDialog()
        {
            SaveBeforeClosing PopUpWindow = new SaveBeforeClosing();
            PopUpWindow.Owner = this;
            //var location = 
            PopUpWindow.ShowDialog();

            switch (PopUpWindow.SaveBeforeCloseChosen)
            {
                case SaveBeforeClosing.SaveConfig.DONTSAVE:
                    return false;
                case SaveBeforeClosing.SaveConfig.SAVE:
                    Save();
                    return false;
                case SaveBeforeClosing.SaveConfig.CANCEL:
                    return true;
                default:
                    return true;
            }
        }
    }
}
