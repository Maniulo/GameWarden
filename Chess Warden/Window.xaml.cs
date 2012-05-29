using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GameWarden.Chess
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Window
    {
        private readonly ChessEntities DB = new ChessEntities();
        private readonly VisualChess TheGame = new VisualChess();
        private GameCollection GamesCollection;
        
        public Window()
        {
            InitializeComponent();
            InitializeEngine();
            InitializeFilters();
            InitializePieceButtons();
        }

        private void ResetGUI()
        {
            WhiteBest.Content = "?";
            BlackBest.Content = "?";
            UncheckButton(this, null);
        }

        private void movesScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue > e.OldValue)
                TheGame.MakeMove();
            else
                TheGame.UndoMove();

            theBoard.Refresh();
        }

        private void SetBindings(Object context)
        {
            LayoutRoot.DataContext = context;

            foreach (Label l in InfoGroup.Children)
                l.DataContext = context;

            eventLabel.SetBinding(ContentProperty, "Event");
            siteLabel.SetBinding(ContentProperty, "Site");
            roundLabel.SetBinding(ContentProperty, "Round");
            dateLabel.SetBinding(ContentProperty, "Date");
            whiteLabel.SetBinding(ContentProperty, "White");
            blackLabel.SetBinding(ContentProperty, "Black");
            resultLabel.SetBinding(ContentProperty, "Result");

            Movetext.DataContext = context;
            Movetext.SetBinding(TextBox.TextProperty, new Binding("Movetext") {Mode = BindingMode.OneWay});

            var b = new Binding("FEN");
            var v = new ExceptionValidationRule();
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b.ValidationRules.Add(v);
            FENSearch.DataContext = context;
            FENSearch.SetBinding(TextBox.TextProperty, b);

            theBoard.DataContext = context;
            theBoard.SetBinding(Board.StateProperty, "State");
        }

        private void ResultsListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                TheGame.Game = ((DBGame)ResultsList.SelectedItem).Game;
                SetBindings(TheGame);
                Ribbon.SelectedItem = HomeTab;
            }
        }
        
        private void RibbonSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Contains(SearchTab))
            {
                if (TheGame.State != null)
                {
                    TheGame.State = new ChessState(TheGame.State);
                }
                else
                {
                    TheGame.Game = new ChessGame();
                    TheGame.State = new ChessState();
                    SetBindings(TheGame);
                }
            }

            if (e.AddedItems.Contains(HomeTab))
            {
                TheGame.State = null;
                ResetGUI();
            }
        }
    }
}
