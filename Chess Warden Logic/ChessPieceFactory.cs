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
                    p.AddPossibleMove(new Promotion(new PawnMove(), new[] { PieceTypes.Knight, PieceTypes.Bishop, PieceTypes.Rook, PieceTypes.Queen }));
                    p.AddPossibleMove(new Promotion(new PawnCapture(), new[] { PieceTypes.Knight, PieceTypes.Bishop, PieceTypes.Rook, PieceTypes.Queen }));
                    p.AddPossibleMove(new PawnMove());
                    p.AddPossibleMove(new PawnCapture());
                    p.AddPossibleMove(new EnPassant());
                    break;
                case PieceTypes.Knight:
                    p.AddPossibleMove(new KnightMoveTemplate());
                    break;
                case PieceTypes.Bishop:
                    p.AddPossibleMove(new DiagonalMoveTemplate());
                    break;
                case PieceTypes.Rook:
                    p.AddPossibleMove(new HorizontalMoveTemplate());
                    p.AddPossibleMove(new VerticalMoveTemplate());
                    break;
                case PieceTypes.Queen:
                    p.AddPossibleMove(new HorizontalMoveTemplate());
                    p.AddPossibleMove(new VerticalMoveTemplate());
                    p.AddPossibleMove(new DiagonalMoveTemplate());
                    break;
                case PieceTypes.King:
                    p.AddPossibleMove(new HorizontalMoveTemplate(1));
                    p.AddPossibleMove(new VerticalMoveTemplate(1));
                    p.AddPossibleMove(new DiagonalMoveTemplate(1));
                    p.AddPossibleMove(new Castling(Castling.CastlingType.Kingside));
                    p.AddPossibleMove(new Castling(Castling.CastlingType.Queenside));
                    break;
            }
        }

        static public ChessPiece CreatePiece(Object pieceCode, IChessPresentation presentation, List<Player> players)
        {
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