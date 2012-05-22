using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        private readonly VisualChess theGame = new VisualChess();

        private readonly ObservableCollection<Game> Games = new ObservableCollection<Game>();
        private readonly CollectionViewSource View;
        private Object placedPiece;
        private FigurinePresentation figures = new FigurinePresentation();
        public Window()
        {
            InitializeComponent();

            View = new CollectionViewSource { Source = Games };
            View.Filter += Contains;
            ResultsList.ItemsSource = View.View;

            var PieceButtons = new List<RibbonToggleButton>();

            foreach (Char c in figures.ToString())
                PieceButtons.Add(new RibbonToggleButton { Label = c.ToString() });
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
            theGame.PlacePiece(new Position(c.X+1, c.Y+1), figures.GetPiece(placedPiece));
        }

        void btn_Click(object sender, RoutedEventArgs e)
        {
            placedPiece = ((RibbonToggleButton)sender).Label[0];
            Cursor = Cursors.Hand;
        }

        private void movesScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue > e.OldValue)
                theGame.MakeMove();
            else
                theGame.UndoMove();

            theBoard.Refresh();
        }
        
        private void OpenPGNClick(object sender, RoutedEventArgs e)
        {
            var filename = OpenFileDialog();

            if (filename != null)
            {
                var ldr = new DBLoader(Games);

                
                ldr.RunWorkerAsync(filename);
            }
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

        private void ResultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            theGame.Game = (ChessGame)e.AddedItems[0];

            LayoutRoot.DataContext = theGame;
            theBoard.SetBinding(Board.StateProperty, "State");
            eventLabel.SetBinding(Label.ContentProperty, "Event");
            siteLabel.SetBinding(Label.ContentProperty, "Site");
            roundLabel.SetBinding(Label.ContentProperty, "Round");
            dateLabel.SetBinding(Label.ContentProperty, "Date");
            whiteLabel.SetBinding(Label.ContentProperty, "White");
            blackLabel.SetBinding(Label.ContentProperty, "Black");
            resultLabel.SetBinding(Label.ContentProperty, "Result");
            
            var b = new Binding("FEN");
            var v = new ExceptionValidationRule();
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            b.ValidationRules.Add(v);
            FENSearch.SetBinding(TextBox.TextProperty, b);
            
        }

        private void TextboxSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            View.View.Refresh();
        }
    }

    public class DBLoader : BackgroundWorker
    {
        private ObservableCollection<Game> l;
        public DBLoader(ObservableCollection<Game> list)
        {
            l = list;
            WorkerReportsProgress = true;
            DoWork += Import;
            ProgressChanged += MakeProgress;
            RunWorkerCompleted += FinishWork;
        }

        private void Import(object sender, DoWorkEventArgs e)
        {
            var filename = (String)e.Argument;
            var io = new FileIO(filename);

            int count = io.Count();
            
            foreach (Game g in io.ImportPGN())
            {
                ReportProgress(0, g); //l.Invoke(Invoke(new MethodInvoker(Delegate {l.Items.Add(g);})));
            }
        }
        private void FinishWork(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void MakeProgress(object sender, ProgressChangedEventArgs e)
        {
            l.Add((Game)e.UserState);
        }
    }

    class VisualChess : INotifyPropertyChanged
    {
        public ChessGame Game;
        public ChessState State
        {
            get
            {
                return (ChessState)Game.State;
            }
            set
            {
                Game.State = value;
                OnPropertyChanged("State");
            }
        }

        public String Event { get { return Game.Info["Event"]; } }
        public String Site { get { return Game.Info["Site"]; } }
        public String Data { get { return Game.Info["Data"]; } }
        public String Round { get { return Game.Info["Round"]; } }
        public String Result { get { return Game.Info["Result"]; } }
        public String White { get { return Game.Info["White"]; } }
        public String Black { get { return Game.Info["Black"]; } }
        public String FEN
        {
            get
            {
                return Game.State.ToString();
            }
            set
            {
                try
                {
                    State = new FENParser().Parse(value);
                    OnPropertyChanged("FEN");
                }
                catch
                {
                    throw new ArgumentException();
                }
            }
        }

        public void PlacePiece(Position pos, IPiece p)
        {
            State.PlacePiece(pos, p);
            OnPropertyChanged("State");
            OnPropertyChanged("FEN");
        }

        public void MakeMove()
        {
            Game.MakeMove();
            OnPropertyChanged("FEN");
        }

        public void UndoMove()
        {
            Game.UndoMove();
            OnPropertyChanged("FEN");
        }

        public void Reset()
        {
            while (Game.UndoMove()) ;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
