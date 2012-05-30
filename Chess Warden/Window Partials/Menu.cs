using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace GameWarden.Chess
{
    public partial class Window
    {
        private static String OpenFileDialog(String filter)
        {
            var dlg = new OpenFileDialog { Filter = filter };
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                String filename = dlg.FileName;
                return filename;
            }

            return null;
        }

        private static String SaveFileDialog(String filter)
        {
            var dlg = new SaveFileDialog { Filter = filter };
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
            var filename = OpenFileDialog("PGN Files|*.pgn");

            if (filename != null)
            {
                var worker = new DBLoader(GamesCollection, DB, ConfigurationManager.ConnectionStrings["ChessDB"].ConnectionString);
                worker.RunWorkerAsync(filename);
                worker.RunWorkerCompleted += PGNImported;
            }
        }

        void PGNImported(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            String msg;
            if (e.Error == null)
            {
                msg = String.Format("Games successfully imported: {0}.\n", e.Result);
            }
            else
            {
                msg = String.Format("Games successfully imported: {0}.\nErrors: {1}.\nSee app log for details.", e.Error.Data["Done"], e.Error.Data["Errors"]);
            }
            
            Message.Dispatcher.BeginInvoke(new Action(
                                               delegate
                                                   {
                                                       Message.IsOpen = true;
                                                       MessageText.Text = msg;
                                                   }
                                               ));
        }

        private void SavePGNClick(object sender, RoutedEventArgs e)
        {
            var filename = SaveFileDialog("PGN Files|*.pgn");
            var items = ResultsList.SelectedItems.Cast<DBGame>();

            if (filename != null)
                new FileIO(filename).ExportPGN(items.Select(c => c.Game));
        }
    }
}
