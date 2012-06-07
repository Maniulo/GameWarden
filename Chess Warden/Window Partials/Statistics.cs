using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Windows.Controls.Ribbon;

namespace GameWarden.Chess
{
    public partial class Window
    {
        private readonly ChessEngineConnector ChessEngine = new ChessEngineConnector();
        
        private void InitializeEngine()
        {
            String path =
                Assembly.GetEntryAssembly().Location.Substring(0, Assembly.GetEntryAssembly().Location.LastIndexOf('\\')) +
                Properties.Settings.Default.EnginesPath;

            var engines = new List<Engine>();
            try
            {
                engines = Directory.GetFiles(path, "*.exe")
                    .Select(s => new Engine(s)).ToList();
            }
            catch
            {
                Directory.CreateDirectory("Engines");
            }

            engines.Add(new Engine("zzz", "Browse..."));

            EnginesCategory.Items.SortDescriptions.Add(new SortDescription("Path", ListSortDirection.Ascending));
            EnginesCategory.ItemsSource = engines;
            if (engines.Count > 1)
                EnginesGallery.SelectedItem = EnginesCategory.Items[0];

            var b = new Binding("Depth");
            var v = new ExceptionValidationRule();
            Depth.DataContext = ChessEngine;
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b.ValidationRules.Add(v);
            Depth.SetBinding(TextBox.TextProperty, b);
            BestMove.DataContext = ChessEngine;
            BestMove.SetBinding(ContentProperty, "State");
        }

        private void RefreshBest(object sender, RoutedEventArgs e)
        {
            ChessEngine.FindBestMove(TheGame.State);
            RefreshStatistics();
        }

        private void ResetStats()
        {
            ChessEngine.Recheck = true;
            WhiteBest.Content = "?";
            BlackBest.Content = "?";
        }

        private void RefreshStatistics()
        {
            var cState = TheGame.State.ToStringShort();

            var total = (from game in DB.Games
                         join state in DB.GameStates on game.ID equals state.Game
                         where state.FEN.Equals(cState)
                         select game.ID).AsEnumerable().Distinct().Count();

            if (total > 0)
            {
                double wins = (from game in DB.Games
                               join state in DB.GameStates on game.ID equals state.Game
                               where state.FEN.Equals(cState) && game.Result.Equals("1-0")
                               select game.ID).AsEnumerable().Distinct().Count();
                WhiteBest.Content = String.Format("{0:0.##}%", wins * 100 / total);

                wins = (from game in DB.Games
                        join state in DB.GameStates on game.ID equals state.Game
                        where state.FEN.Equals(cState) && game.Result.Equals("0-1")
                        select game.ID).AsEnumerable().Distinct().Count();
                BlackBest.Content = String.Format("{0:0.##}%", wins * 100 / total);
            }
        }

        private void EngineSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var value = (Engine)e.NewValue;
            if (value.Name == "Browse...")
            {
                var list = sender as RibbonGallery;
                var category = list.Items[0] as RibbonGalleryCategory;
                var filename = OpenFileDialog("Chess Engine|*.exe");

                if (filename != null)
                {
                    IEditableCollectionViewAddNewItem items = category.Items;
                    if (items.CanAddNewItem)
                    {
                        object newitem = items.AddNewItem(new Engine(filename));
                        items.CommitNew();
                        list.SelectedItem = newitem;
                    }
                }
                else
                    list.SelectedItem = category.Items[0];
            }
            else
                ChessEngine.Path = value.Path;
        }
    }
}
