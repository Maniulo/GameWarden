using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GameWarden.Chess
{
    public partial class Window
    {
        private ChessEntities DB;
        private readonly VisualChess TheGame = new VisualChess();
        private GameCollection GamesCollection;
        
        public Window()
        {
            InitializeComponent();
            try
            {
                DisableControlsGameNotLoaded();
                InitializeDB();
                InitializeEngine();
                InitializeFilters();
                InitializePieceButtons();
                ResetStats();
            }
            catch (Exception)
            {
                Message.IsOpen = true;
                MessageText.Text = "Database connection error.";
                DisableControls();
            }
            
        }

        private void DisableControls()
        {
            ResultsList.IsEnabled = false;
            SearchTab.IsEnabled = false;
            DisableControlsGameNotLoaded();
            OpenPGNBtn.IsEnabled = false;
        }

        private void DisableControlsGameNotLoaded()
        {
            movesScrollBar.IsEnabled = false;
            Calculate.IsEnabled = false;
            SavePGNBtn.IsEnabled = false;
        }

        private void EnableControlsGameIsLoaded()
        {
            movesScrollBar.IsEnabled = true;
            Calculate.IsEnabled = true;
            SavePGNBtn.IsEnabled = true;
        }

        private void InitializeDB()
        {
            DB = new ChessEntities();
        }

        private void ResetGUI()
        {
            WhiteBest.Content = "?";
            BlackBest.Content = "?";
            foreach (var btn in PieceButtons)
                btn.IsChecked = false;
        }

        private void MovesScrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int Amount = (int)e.NewValue - (int)e.OldValue;

            if (Amount > 0)
                for (; Amount > 0; --Amount )
                    TheGame.MakeMove();
            else
                for (; Amount < 0; ++Amount)
                    TheGame.UndoMove();

            ResetStats();

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

            movesScrollBar.Maximum = ((VisualChess) context).Moves;
            movesScrollBar.Value = 0;
            movesScrollBar.ViewportSize = 10;

            Movetext.DataContext = context;
            Movetext.SetBinding(TextBox.TextProperty, new Binding("Movetext") {Mode = BindingMode.OneWay});
            
            theBoard.DataContext = context;
            theBoard.SetBinding(Board.StateProperty, "State");
        }

        private void SetBindingsSearch(Object context)
        {
            var b = new Binding("FEN");
            var v = new ExceptionValidationRule();
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b.ValidationRules.Add(v);
            FENSearch.DataContext = context;
            FENSearch.SetBinding(TextBox.TextProperty, b);
        }

        private void ResultsListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                EnableControlsGameIsLoaded();
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
                SetBindingsSearch(TheGame);
            }

            if (e.AddedItems.Contains(HomeTab))
            {
                TheGame.State = null;
                SetBindingsSearch(null);
                ResetGUI();
            }
        }
    }
}
