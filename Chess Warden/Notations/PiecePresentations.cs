using System;

namespace GameWarden.Chess.Notations
{
    public interface IPiecePresentation
    {
        Char? GetPresentation(IPiece p);
        IPiece GetPiece(char? c);
    }
}