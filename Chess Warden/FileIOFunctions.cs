using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace GameWarden.Chess
{
    public partial class Window
    {
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

        private void OpenPGNClick(object sender, RoutedEventArgs e)
        {
            var filename = OpenFileDialog();

            if (filename != null)
                new DBLoader(Games).RunWorkerAsync(filename);
        }

        private void SavePGNClick(object sender, RoutedEventArgs e)
        {
            var filename = SaveFileDialog();
            var items = ((IList)ResultsList.SelectedItems).Cast<ChessGame>();

            if (filename != null)
                new FileIO(filename).ExportPGN(items);
        }
    }
}
