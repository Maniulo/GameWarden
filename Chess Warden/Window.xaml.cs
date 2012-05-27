using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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
        private readonly ChessEngineConnector ChessEngine = new ChessEngineConnector();
        private readonly ChessEntities DB = new ChessEntities();
        private GameCollection GamesCollection;
        private readonly VisualChess theGame = new VisualChess();

        private ObservableCollection<Game> Games;
        private CollectionViewSource View;
        private Object placedPiece;
        private readonly FigurinePresentation figures = new FigurinePresentation();

        private void ClearDB()
        {
            foreach (var o in from c in DB.GameStates select c)
                DB.GameStates.DeleteObject(o);
            foreach (var o in from c in DB.Games select c)
                DB.Games.DeleteObject(o);
            DB.SaveChanges();
        }
        
        public Window()
        {
            InitializeComponent();
            InitializeEngine();
            //ClearDB();
            InitializeDB();
            InitializePieceButtons();
        }

        private void InitializeDB()
        {
            GamesCollection = new GameCollection(DB);
            View = new CollectionViewSource { Source = GamesCollection };
            ResultsList.ItemsSource = View.View;
            View.Filter += Contains;

            //WhiteBest.SetBinding()
        }

        private void InitializeEngine()
        {
            String path = 
                Assembly.GetEntryAssembly().Location.Substring(0, Assembly.GetEntryAssembly().Location.LastIndexOf('\\')) +
                Properties.Settings.Default.EnginesPath;

            var engines = Directory.GetFiles(path, "*.exe")
                                    .Select(s => new Engine(s)).ToList();

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

        private void InitializePieceButtons()
        {
            var PieceButtons = new List<RibbonToggleButton>();

            foreach (Char c in figures.ToString())
                PieceButtons.Add(new RibbonToggleButton { Label = c.ToString(), Content = c });
            PieceButtons.Add(new RibbonToggleButton { Label = "x" });

            foreach (var btn in PieceButtons)
            {
                btn.FontSize = 18;
                btn.Height = 30;
                btn.Width = 30;
                btn.Padding = new Thickness(0, 0, 0, 0);

                Position.Items.Add(btn);
                btn.Click += btn_Click;
            }

            foreach (Cell c in theBoard)
                c.MouseUp += c_MouseUp;
        }

        void c_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var c = sender as Cell;
            if (e.ChangedButton == MouseButton.Left)
                theGame.PlacePiece(new Position(c.X+1, c.Y+1), ChessPieceFactory.CreatePiece(placedPiece, figures, theGame.Game.Players)); // !!!
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            placedPiece = ((RibbonToggleButton)sender).Content;
            Cursor = Cursors.Hand;
        }

        private void movesScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue > e.OldValue)
                theGame.MakeMove();
            else
                theGame.UndoMove();

            theBoard.Refresh();

            
            //ChessEngine.FindBestMove(theGame.State);
        }

        public void Contains(object sender, FilterEventArgs e)
        {
            e.Accepted = false;

            try {
                var game = e.Item as DBGame;
                if (game != null)
                    if (game.Event.Contains(EventSearch.Text) &&
                        game.Site.Contains(SiteSearch.Text) &&
                        game.White.Contains(WhiteSearch.Text) &&
                        game.Black.Contains(BlackSearch.Text) &&
                        game.Result.Contains(ResultSearch.Text) &&
                        game.Date.Contains(DateSearch.Text) &&
                        game.Round.Contains(RoundSearch.Text) &&
                        (game.GameStates.Any(state => state.FEN.Contains(FENSearch.Text)) || FENSearch.Text == "" || FENSearch.Text == "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
                        )
                    {
                        e.Accepted = true;
                    }
            } catch { }
        }

        private void SetBindings(Object context)
        {
            LayoutRoot.DataContext = context;

            foreach (Label l in InfoGroup.Children)
                l.DataContext = context;

            theBoard.SetBinding(Board.StateProperty, "State");
            eventLabel.SetBinding(ContentProperty, "Event");
            siteLabel.SetBinding(ContentProperty, "Site");
            roundLabel.SetBinding(ContentProperty, "Round");
            dateLabel.SetBinding(ContentProperty, "Date");
            whiteLabel.SetBinding(ContentProperty, "White");
            blackLabel.SetBinding(ContentProperty, "Black");
            resultLabel.SetBinding(ContentProperty, "Result");

            var b = new Binding("FEN");
            var v = new ExceptionValidationRule();
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b.ValidationRules.Add(v);
            FENSearch.SetBinding(TextBox.TextProperty, b);
        }

        private void ResultsListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                theGame.Game = ((DBGame) e.AddedItems[0]).Game;
                SetBindings(theGame);
                ChessEngine.FindBestMove(theGame.State);
            }
        }

        private void TextboxSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            View.View.Refresh();
        }

        private void RefreshBest(object sender, RoutedEventArgs e)
        {
            ChessEngine.FindBestMove(theGame.State);
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

        private void RibbonButton_Click(object sender, RoutedEventArgs e)
        {
            DB.SaveChanges();

            var cState = theGame.State.ToString();
            var Query = from game in DB.Games
                        join state in DB.GameStates on game.ID equals state.Game
                        where state.FEN.Equals(cState)
                        select game.ID;
            int total = Query.AsEnumerable().Distinct().Count();

            if (total > 0)
            {

                var Query2 = from game in DB.Games
                             join state in DB.GameStates on game.ID equals state.Game
                             where state.FEN.Equals(cState) && game.Result.Equals("1-0")
                        select game.ID;
                WhiteBest.Content = (double)Query2.AsEnumerable().Distinct().Count() / total;

                Query2 = from game in DB.Games
                         join state in DB.GameStates on game.ID equals state.Game
                         where state.FEN.Equals(cState) && game.Result.Equals("0-1")
                        select game.ID;
                BlackBest.Content = (double)Query2.AsEnumerable().Distinct().Count() / total;

                this.Title = total.ToString();
            }
        }
    }

    public struct Engine
    {
        private String path;
        public String Path { get { return path; } }
        private String name;
        public String Name { get { return name; } }

        public Engine(String npath)
        {
            path = npath;
            name = path.Substring(path.LastIndexOf('\\') + 1, path.Length - 5 - path.LastIndexOf('\\'));
        }

        public Engine(String npath, String caption)
        {
            path = npath;
            name = caption;
        }

        public override String ToString()
        {
            return Name;
        }
    }
}
