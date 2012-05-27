using System;

namespace GameWarden
{
    public interface IPiecePresentation
    {
        Object GetPresentation(IPiece p);
    }
}