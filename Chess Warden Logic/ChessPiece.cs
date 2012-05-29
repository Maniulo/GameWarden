using System;
using System.Linq;

namespace GameWarden.Chess
{
    public class ChessPiece : Piece
    {
        public PieceTypes Type;

        public ChessPiece() { }
        public ChessPiece(ChessPiece copy) : base(copy)
        {
            Type = copy.Type;
        }

        public virtual Boolean CanAttack(Position to, IGameState state)
        {
            return PossibleMoves.Any(m => m.IsCapture && m.CanApply(Pos, to, state));
        }
        public IConcreteMove GetPossibleMove(Position to, IGameState state, PieceTypes promotionTo)
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

            return null;
        }
    }
}