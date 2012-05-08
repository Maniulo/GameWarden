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
using System.Windows.Shapes;
using GameWarden.Chess.Notations;
using Microsoft.Windows.Controls.Ribbon;

namespace GameWarden.Chess
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Window : RibbonWindow
    {
        Board theBoard;

        public Window()
        {
            InitializeComponent();

            String[] pgn = new String[3];
            pgn[0] = "[FEN \"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq e6 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. e4";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            theBoard = new Board(game);
            LayoutRoot.Children.Add(theBoard);

            Thickness margin = theBoard.Margin;
            margin.Top = 137;
            margin.Left = 0;
            theBoard.Margin = margin;
            theBoard.HorizontalAlignment = HorizontalAlignment.Left;
            theBoard.Background = Brushes.BlanchedAlmond; // new SolidColorBrush(new Color("#FFD18B47"));
        }

        private void movesScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            theBoard.MakeMove();
            theBoard.InvalidateArrange();
        }

        
    }

    public class Board : Grid
    {
        public ChessGame Game;
        private Label[,] Squares;

        public void MakeMove()
        {
            Game.MakeMove();
            Refresh();
        }

        protected void Refresh()
        {
            IPiecePresentation presentation = new FigurinePresentation();
            
            foreach (Piece p in Game.CurrentState)
                Squares[p.Pos.File - 1, p.Pos.Rank - 1].Content = presentation.GetPresentation((ChessPiece)p);

            Squares[0, 0].Content = "!!!";
        }

        public Board(ChessGame game)
        {
            Game = game;

            Height = 400;
            Width = 400;

            Squares = new Label[8, 8];

            for (int file = 1; file <= 8; ++file)
                this.RowDefinitions.Add(new RowDefinition());

            for (int rank = 1; rank <= 8; ++rank)
                this.ColumnDefinitions.Add(new ColumnDefinition());

            for (int file = 0; file < 8; ++file)
                for (int rank = 0; rank < 8; ++rank)
                {
                    var s = new Label {FontSize = 32}; // { Content = Position.ToString(file + 1, rank + 1) };
                    Children.Add(s);
                    SetColumn(s, file);
                    SetRow(s, 7 - rank);
                    Squares[file, rank] = s;
                }

            Refresh();
        }
    }
}
