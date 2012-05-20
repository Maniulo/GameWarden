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
using System.Windows.Navigation;
using System.Windows.Shapes;
using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {
        public int DimX
        {
            get { return (int)GameType.GetProperty("DimX").GetValue(null, null); }
        }
        public int DimY
        {
            get { return (int)GameType.GetProperty("DimY").GetValue(null, null); }
        }

        private Type gameType = typeof(ChessGame);
        public Type GameType
        {
            get { return gameType; }
            set { gameType = value; }
        }

        private Brush _LightBackground;
        public Brush LightBackground
        {
            get
            {
                return _LightBackground;
            }
            set
            {
                _LightBackground = value;
                for (int file = 0; file < DimX; ++file)
                    for (int rank = 0; rank < DimY; ++rank)
                        if ((file + rank) % 2 == 1)
                            Squares[file, rank].Background = _LightBackground;
            }
        }

        private Brush _DarkBackground;
        public Brush DarkBackground
        {
            get
            {
                return _DarkBackground;
            }
            set
            {
                _DarkBackground = value;
                for (int file = 0; file < DimX; ++file)
                    for (int rank = 0; rank < DimY; ++rank)
                        if ((file + rank) % 2 == 0)
                            Squares[file, rank].Background = _DarkBackground;
            }
        }

        private Game game;
        public Game Game
        {
            get { return game; }
            set
            {
                game = value;
                Refresh();
            }
        }
        
        private Cell[,] Squares;

        public void MakeMove()
        {
            Game.MakeMove();
            Refresh();
        }
        public void UndoMove()
        {
            Game.UndoMove();
            Refresh();
        }

        private void ClearCanvas()
        {
            theCanvas.Children.Clear();
        }

        public void DrawPath(Cell c)
        {
            ClearCanvas();
            DrawLine(
                c.Piece.Path.Select(pos => Squares[pos.File - 1, DimY - pos.Rank + 1])
                );
        }
        public void DrawLine(IEnumerable<Cell> cells)
        {
            var myLine = new Polyline { Stroke = Brushes.Coral };

            foreach (Cell c in cells)
                myLine.Points.Add(c.Center);

            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 5;

            theCanvas.Children.Add(myLine);
        }

        public void Refresh()
        {
            ClearCanvas();

            /*W.eventLabel.Content = Game.Info["Event"];
            W.siteLabel.Content = Game.Info["Site"];
            W.blackLabel.Content = Game.Info["Black"];
            W.whiteLabel.Content = Game.Info["White"];
            W.dateLabel.Content = Game.Info["Date"];
            W.resultLabel.Content = Game.Info["Result"];
            W.roundLabel.Content = Game.Info["Round"];*/

            foreach (var p in Game.State)
                Squares[p.Pos.File - 1, p.Pos.Rank - 1].Piece = (Piece)p;
        }

        private void InitializeGrid()
        {
            Squares = new Cell[DimX, DimY];

            for (int file = 1; file <= DimX; ++file)
                theGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int rank = 1; rank <= DimY; ++rank)
                theGrid.RowDefinitions.Add(new RowDefinition());
        }

        private void PopulateGrid()
        {
            IPiecePresentation presentation = new FigurinePresentation();
            int ww = (int)theGrid.Width / DimX;
            int hh = (int)theGrid.Height / DimY;

            for (int file = 0; file < DimX; ++file)
                for (int rank = 0; rank < DimY; ++rank)
                {
                    Cell cell = Squares[file, rank] = new Cell(ww * file, hh * rank) { Presentation = presentation };
                    theGrid.Children.Add(cell);
                    Grid.SetColumn(cell, file);
                    Grid.SetRow(cell, DimY - rank - 1);
                    CreateContextMenu(cell);
                }
        }

        public Board()
        {
            InitializeComponent();
            InitializeGrid();
            PopulateGrid();
        }

        private void CreateContextMenu(Cell cell)
        {
            ContextMenu = new ContextMenu();
            var item = new MenuItem { Header = "Show path", CommandParameter = cell };
            item.Click += ItemOnClick;
            ContextMenu.Items.Add(item);
            ContextMenuService.SetContextMenu(cell, ContextMenu);
        }

        private void ItemOnClick(object sender, RoutedEventArgs e)
        {
            DrawPath(((MenuItem)sender).CommandParameter as Cell);
        }

        protected override Size ArrangeOverride(Size constraint)
        {
            var size = Math.Min(constraint.Width/DimX, constraint.Height/DimY);

            theGrid.Width = DimX * size;
            theGrid.Height = DimY * size;
            ClearCanvas();

            foreach (Cell c in Squares)
                c.FontSize = size / 2;

            return base.ArrangeOverride(constraint);
        }
    }

    public class Cell : Label
    {
        public IPiecePresentation Presentation;

        private readonly int X;
        private readonly int Y;

        private Piece p;
        public Piece Piece
        {
            get { return p; }
            set
            {
                p = value;
                Content = Presentation.GetPresentation(p); //!!!
            }
        }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;

            Padding = new Thickness(0, 0, 0, 0);
            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;
        }

        public Point Center
        {
            get
            {
                return
                    new Point(
                        X + ActualWidth / 2,
                        Y + ActualHeight / 2);
            }
        }

    }
}
