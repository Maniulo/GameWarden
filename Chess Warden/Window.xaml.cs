using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private readonly VisualChess theGame = new VisualChess();

        private readonly ObservableCollection<Game> Games = new ObservableCollection<Game>();
        private readonly CollectionViewSource View;
        private Object placedPiece;
        private FigurinePresentation figures = new FigurinePresentation();

        public Window()
        {
            InitializeComponent();
            InitializeEngine();

            View = new CollectionViewSource { Source = Games };
            View.Filter += Contains;
            ResultsList.ItemsSource = View.View;

            InitializePieceButtons();
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
            Game game = e.Item as Game;

            if (game.Info["Event"].Contains(EventSearch.Text) &&
                game.Info["Site"].Contains(SiteSearch.Text) &&
                game.Info["White"].Contains(WhiteSearch.Text) &&
                game.Info["Black"].Contains(BlackSearch.Text) &&
                game.Info["Result"].Contains(ResultSearch.Text) &&
                game.Info["Date"].Contains(DateSearch.Text) &&
                game.Info["Round"].Contains(RoundSearch.Text)
                )
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;    
            }
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
            theGame.Game = (ChessGame)e.AddedItems[0];
            SetBindings(theGame);
            ChessEngine.FindBestMove(theGame.State);
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
