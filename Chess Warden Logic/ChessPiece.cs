using System;
using System.Linq;

namespace GameWarden.Chess
{
    public class ChessPiece : Piece
    {
        public PieceTypes Type;

        public ChessPiece()
        {
            
        }

        public Boolean CanMove(Position to, IGameState state, PieceTypes promotionTo)
        {
            return PossibleMoves.Any(m => m.CanApply(Pos, to, state) || m is Promotion && ((Promotion)m).CanApply(Pos, to, state, promotionTo));
        }

        public IConcreteMove GetPossibleMove(Position to, IGameState state, PieceTypes promotionTo)
        {
            try
            {
                foreach (ITemplateMove m in PossibleMoves)
                {
                    if (m is Promotion)
                    {
                        var p = m as Promotion;
                        if (p.CanApply(Pos, to, state, promotionTo))
                            return p.Concretize(Pos, to, promotionTo);
                    }
                    else
                    {
                        if (m.CanApply(Pos, to, state))
                            return m.Concretize(Pos, to);
                    }
                }

                throw new Exception("No possible move found.");
            }
            catch
            {
                throw new Exception("No possible move found.");
            }
        }

        public ChessPiece(ChessPiece copy)
            :base(copy)
        {
            Type = copy.Type;
        }
    }
}