using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    public partial class Board : UserControl, IEnumerable<Cell>
    {
        private int dimX;
        public int DimX
        {
            get { return dimX; }
            set
            {
                if (value > 0)
                {
                    dimX = value;
                    InitializeGrid();
                    PopulateGrid();
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private int dimY;
        public int DimY
        {
            get { return dimY; }
            set
            {
                if (value > 0)
                {
                    dimY = value;
                    InitializeGrid();
                    PopulateGrid();
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
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

        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
            "State", typeof(GameState), typeof(Board), new FrameworkPropertyMetadata(OnStateChanged, CoerceValueCallback));
        private static object CoerceValueCallback(DependencyObject d, object e)
        {
            ((Board)d).Refresh();
            return e;
        }
        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Board)d).Refresh();
        }
        public GameState State
        {
            get { return (GameState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        private Cell[,] Squares;
        private double CellSize; 

        // Drawing piece path
        private void DrawPath(Cell c)
        {
            ClearCanvas();
            DrawLine(
                c.Piece.Path.Select(pos => Squares[pos.File - 1, DimY - pos.Rank])
                );
        }
        private void DrawLine(IEnumerable<Cell> cells)
        {
            var myLine = new Polyline { Stroke = Brushes.Coral };

            foreach (Cell c in cells)
                myLine.Points.Add(GetCellCenter(c));

            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeThickness = 5;

            theCanvas.Children.Add(myLine);
        }
        private Point GetCellCenter(Cell cell)
        {
            return new Point((cell.X + 0.5) * CellSize, (cell.Y + 0.5) * CellSize);
        }
        private void ClearCanvas()
        {
            theCanvas.Children.Clear();
        }

        public void Refresh()
        {
            ClearCanvas();

            if (State != null)
                foreach (Piece p in State)
                    Squares[p.Pos.File - 1, p.Pos.Rank - 1].Piece = p;
        }

        // Initializers
        public Board()
        {
            InitializeComponent();
            BringCanvasToFront();
        }
        private void InitializeGrid()
        {
            if (DimX > 0 && DimY > 0)
            {
                Squares = new Cell[DimX,DimY];

                theGrid.ColumnDefinitions.Clear();
                theGrid.RowDefinitions.Clear();

                for (int file = 1; file <= DimX; ++file)
                    theGrid.ColumnDefinitions.Add(new ColumnDefinition());

                for (int rank = 1; rank <= DimY; ++rank)
                    theGrid.RowDefinitions.Add(new RowDefinition());
            }
        }
        private void PopulateGrid()
        {
            if (DimX > 0 && DimY > 0)
            {
                IPiecePresentation presentation = new FigurinePresentation();
                theGrid.Children.Clear();

                for (int file = 0; file < DimX; ++file)
                    for (int rank = 0; rank < DimY; ++rank)
                    {
                        Cell cell = Squares[file, rank] = new Cell(file, rank) {Presentation = presentation};
                        theGrid.Children.Add(cell);
                        Grid.SetColumn(cell, file);
                        Grid.SetRow(cell, DimY - rank - 1);
                        CreateContextMenu(cell);
                    }
            }
        }
        private void BringCanvasToFront()
        {
            var canvas = theGrid.Children[0];
            theGrid.Children.RemoveAt(0);
            theGrid.Children.Add(canvas);
        }
        
        // Context menu
        private void CreateContextMenu(Cell cell)
        {
            ContextMenu = new ContextMenu();
            var item = new MenuItem { Header = "Show path", CommandParameter = cell, Icon = new Image() { Source = new BitmapImage( new Uri("/Resourses/path.png", UriKind.Relative)) } };
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
            CellSize = Math.Min(constraint.Width / DimX,
                                constraint.Height / DimY);

            theGrid.Width = DimX * CellSize;
            theGrid.Height = DimY * CellSize;
            
            foreach (Cell c in Squares)
                c.FontSize = CellSize / 1.5;

            ClearCanvas();

            return base.ArrangeOverride(constraint);
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            for (int i = 0; i < DimX; ++i)
                for (int j = 0; j < DimY; ++j)
                    yield return Squares[i,j];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Cell : Label
    {
        public IPiecePresentation Presentation;

        public readonly int X;
        public readonly int Y;

        private Piece piece;
        public Piece Piece
        {
            get { return piece; }
            set
            {
                piece = value;
                Content = Presentation.GetPresentation(piece); //!!!
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
    }
}
