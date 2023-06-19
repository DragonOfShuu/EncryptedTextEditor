using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace Encrypted_Text_Editor
{
    /// <summary>
    /// Interaction logic for SaveBeforeClosing.xaml
    /// </summary>
    public partial class SaveBeforeClosing : Window
    {
        // Save before closing chosen value
        public SaveConfig SaveBeforeCloseChosen { get; set; }

        public enum SaveConfig
        {
            SAVE,
            DONTSAVE,
            CANCEL
        }

        public SaveBeforeClosing()
        {
            SaveBeforeCloseChosen= 0;
            InitializeComponent();
        }

        private void TitleBarDragged(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void XButtonClicked(object sender, RoutedEventArgs e)
        {
            SaveBeforeCloseChosen = SaveConfig.CANCEL;
            this.Close();
        }

        private void Save_Clicked(object sender, RoutedEventArgs e)
        {
            SaveBeforeCloseChosen= SaveConfig.SAVE;
            this.Close();
        }

        private void DontSave_Clicked(object sender, RoutedEventArgs e)
        {
            SaveBeforeCloseChosen= SaveConfig.DONTSAVE;
            this.Close();
        }
    }
}
