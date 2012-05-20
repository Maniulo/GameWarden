using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GameWarden.Chess.Notations;
using Microsoft.Win32;
using Microsoft.Windows.Controls.Ribbon;

namespace GameWarden.Chess
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Window : RibbonWindow
    {
        public Window()
        {
            InitializeComponent();
        }

        private void movesScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue > e.OldValue)
                theBoard.MakeMove();
            else
                theBoard.UndoMove();
        }

        private void OpenPGNBtn_Click(object sender, RoutedEventArgs e)
        {
            theBoard.Game = new FileIO(OpenFileDialog()).ImportPGN().ElementAt(0);
        }

        // File dialogs functions
        private static String OpenFileDialog()
        {
            var dlg = new OpenFileDialog { Filter = "PGN Files|*.pgn" };
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                String filename = dlg.FileName;
                return filename;
            }

            return null;
        }
        private static String SaveFileDialog()
        {
            var dlg = new SaveFileDialog { Filter = "PGN Files|*.pgn" };
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                String filename = dlg.FileName;
                return filename;
            }
            return null;
        }
    }

    

}
