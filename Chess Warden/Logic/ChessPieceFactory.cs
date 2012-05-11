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
        public ChessPiece CreatePiece(Char? type, IPiecePresentation presentation)
        {
            ChessPiece p = presentation.GetPiece(type);

            switch (p.Type)
            {
                case PieceTypes.Pawn:
                    p.AddPossibleMove(new PawnMove());
                    p.AddPossibleMove(new PawnCapture());
                    p.AddPossibleMove(new EnPassant());
                    break;
                case PieceTypes.Knight:
                    p.AddPossibleMove(new KnightMove());
                    break;
                case PieceTypes.Bishop:
                    p.AddPossibleMove(new DiagonalMove());
                    break;
                case PieceTypes.Rook:
                    p.AddPossibleMove(new HorizontalMove());
                    p.AddPossibleMove(new VerticalMove());
                    break;
                case PieceTypes.Queen:
                    p.AddPossibleMove(new HorizontalMove());
                    p.AddPossibleMove(new VerticalMove());
                    p.AddPossibleMove(new DiagonalMove());
                    break;
                case PieceTypes.King:
                    p.AddPossibleMove(new HorizontalMove(1));
                    p.AddPossibleMove(new VerticalMove(1));
                    p.AddPossibleMove(new DiagonalMove(1));
                    p.AddPossibleMove(new Kingside());
                    p.AddPossibleMove(new Queenside());
                    break;
            }

            return p;
        }
    }
}