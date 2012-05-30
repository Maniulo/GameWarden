using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GameWarden.Chess.Notations;
using Microsoft.Samples.EntityDataReader;
using Microsoft.Win32;
using Microsoft.Windows.Controls.Ribbon;

namespace GameWarden.Chess
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Window
    {
        private readonly List<RibbonToggleButton> PieceButtons = new List<RibbonToggleButton>();

        private ChessPiece PlacedPiece;
        private readonly FigurinePresentation Figures = new FigurinePresentation();

        private void InitializePieceButtons()
        {
            foreach (Char c in Figures.ToString())
                PieceButtons.Add(new RibbonToggleButton
                                    {
                                        Label = c.ToString(),
                                        Content = ChessPieceFactory.CreatePiece(c, Figures)
                                    });
            
            PieceButtons.Add(new RibbonToggleButton
                                    {
                                        Label = "x",
                                        Content = ChessPieceFactory.CreatePiece(null, Figures)
                                    });

            foreach (var btn in PieceButtons)
            {
                btn.FontSize = 15;
                btn.Height = 30;
                btn.Width = 30;
                btn.Padding = new Thickness(0, 0, 0, 0);

                Position.Items.Add(btn);
                btn.Checked += CheckButton;
                btn.Unchecked += UncheckButton;
            }

            foreach (Cell c in theBoard)
                c.MouseUp += PlacePiece;
        }

        void PlacePiece(object sender, MouseButtonEventArgs e)
        {
            var c = sender as Cell;
            if (e.ChangedButton == MouseButton.Left && PlacedPiece != null)
                TheGame.PlacePiece(new Position(c.X + 1, c.Y + 1), new ChessPiece(PlacedPiece));

            theBoard.Refresh();
        }

        void CheckButton(object sender, RoutedEventArgs e)
        {
            foreach (var btn in PieceButtons.Where(btn => btn != sender))
                btn.IsChecked = false;

            if (sender is RibbonToggleButton)
            {
                var o = sender as RibbonToggleButton;
                PlacedPiece = (ChessPiece)o.Content;
            }

            Cursor = Cursors.Hand;
        }

        void UncheckButton(object sender, RoutedEventArgs e)
        {
            if (sender is RibbonToggleButton)
            {
                var o = sender as RibbonToggleButton;
                if (PlacedPiece == o.Content)
                    PlacedPiece = null;
            }
            
            Cursor = Cursors.Arrow;
        }
    }
}
