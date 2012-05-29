using System;
using System.Collections.Generic;
using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    public enum PieceTypes
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public class ChessPieceFactory
    {
        static public void AddMoves(ChessPiece p)
        {
            switch (p.Type)
            {
                case PieceTypes.Pawn:
                    p.PossibleMoves.Add(new Promotion(new PawnMove(), new[] { PieceTypes.Knight, PieceTypes.Bishop, PieceTypes.Rook, PieceTypes.Queen }));
                    p.PossibleMoves.Add(new Promotion(new PawnCapture(), new[] { PieceTypes.Knight, PieceTypes.Bishop, PieceTypes.Rook, PieceTypes.Queen }));
                    p.PossibleMoves.Add(new PawnMove());
                    p.PossibleMoves.Add(new PawnCapture());
                    p.PossibleMoves.Add(new EnPassant());
                    break;
                case PieceTypes.Knight:
                    p.PossibleMoves.Add(new KnightMoveTemplate());
                    break;
                case PieceTypes.Bishop:
                    p.PossibleMoves.Add(new DiagonalMoveTemplate());
                    break;
                case PieceTypes.Rook:
                    p.PossibleMoves.Add(new HorizontalMoveTemplate());
                    p.PossibleMoves.Add(new VerticalMoveTemplate());
                    break;
                case PieceTypes.Queen:
                    p.PossibleMoves.Add(new HorizontalMoveTemplate());
                    p.PossibleMoves.Add(new VerticalMoveTemplate());
                    p.PossibleMoves.Add(new DiagonalMoveTemplate());
                    break;
                case PieceTypes.King:
                    p.PossibleMoves.Add(new HorizontalMoveTemplate(1));
                    p.PossibleMoves.Add(new VerticalMoveTemplate(1));
                    p.PossibleMoves.Add(new DiagonalMoveTemplate(1));
                    p.PossibleMoves.Add(new Castling(Castling.CastlingType.Kingside));
                    p.PossibleMoves.Add(new Castling(Castling.CastlingType.Queenside));
                    break;
            }
        }

        static public ChessPiece CreatePiece(Object pieceCode, IChessPresentation presentation, List<Player> players = null)
        {
            if (players == null)
                players = new List<Player> { new Player(1), new Player(2) };

            var p = new ChessPiece();

            if (!presentation.IsEmpty(pieceCode))
            {
                p.Type = presentation.GetPieceType(pieceCode);
                p.Player = players.Find(s => s.Order == presentation.GetPlayer(pieceCode));
                AddMoves(p);
            }
            else
            {
                p.IsEmpty = true;
            }
            
            return p;
        }
    }
}