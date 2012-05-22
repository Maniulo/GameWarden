using System;
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
        public void PopulatePiece(ChessPiece p, PieceTypes type)
        {
            switch (p.Type)
            {
                case PieceTypes.Pawn:
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

        public ChessPiece CreatePiece(Char? pieceCode, IPiecePresentation presentation)
        {
            var p = presentation.GetPiece(pieceCode) as ChessPiece;
            if (p == null)
                throw new ArgumentException();

            PopulatePiece(p, p.Type);

            return p;
        }
    }
}