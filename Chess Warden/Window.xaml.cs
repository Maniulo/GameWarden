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
using Microsoft.Win32;
using Microsoft.Windows.Controls.Ribbon;

namespace GameWarden.Chess
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Window : RibbonWindow
    {
        readonly Board theBoard;

        public Window()
        {
            InitializeComponent();

            String[] pgn = new String[3];
            //pgn[0] = "[FEN \"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1\"]";
            pgn[0] = "[FEN \"6k1/8/8/2p5/8/8/8/4K2R w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "12.O-O c4 13.e5 Nb4 14.Ne4 dxe5 15.d6 Qd8 16.fxe5 Nc6 17.Bg5 Qb6 18.Nf6+ Kh8 19.Nd5 Qa5 20.Nd2 Ndxe5 21.Nb3 Bg4 22.Qc1 Nf3+ 23.Rxf3 Bxf3 24.Nxa5 Bxd5 25.Nxc6 bxc6 26.Qxc5";
            //pgn[2] = "1.d4 Nf6 2.c4 c5 3.d5 e6 4.Nc3 exd5 5.cxd5 d6 6.e4 g6 7.f4 Bg7 8.Bb5+ Nfd7 9.a4 Qh4+ 10.g3 Qe7 11.Nf3 O-O 12.O-O Na6 13.e5 Nb4 14.Ne4 dxe5 15.d6 Qd8 16.fxe5 Nc6 17.Bg5 Qb6 18.Nf6+ Kh8 19.Nd5 Qa5 20.Nd2 Ndxe5 21.Nb3 Bg4 22.Qc1 Nf3+ 23.Rxf3 Bxf3 24.Nxa5 Bxd5 25.Nxc6 bxc6 26.Qxc5";

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
        }

        private void OpenPGNBtn_Click(object sender, RoutedEventArgs e)
        {
            theBoard.Game = new FileIO(OpenFileDialog()).ImportPGN().ElementAt(0);
            theBoard.Refresh();
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
    }

    public class Board : Grid
    {
        public ChessGame Game;
        private readonly Label[,] Squares;

        public void MakeMove()
        {
            Game.MakeMove();
            Refresh();
        }

        public void Refresh()
        {
            IPiecePresentation presentation = new FigurinePresentation();

            foreach (Piece p in Game.CurrentState)
               Squares[p.Pos.File - 1, p.Pos.Rank - 1].Content = presentation.GetPresentation((ChessPiece)p);
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
                    var s = new Label {FontSize = 32};
                    Children.Add(s);
                    SetColumn(s, file);
                    SetRow(s, 7 - rank);
                    Squares[file, rank] = s;
                }

            Refresh();
        }
    }
}
