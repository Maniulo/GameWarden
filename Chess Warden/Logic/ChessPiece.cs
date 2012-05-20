using System.Collections.Generic;

namespace GameWarden.Chess
{
    public class ChessPiece : Piece
    {
        public PieceTypes Type;

        public ChessPiece()
        {
            
        }
        
        public ChessPiece(ChessPiece copy)
            :base(copy)
        {
            Type = copy.Type;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}