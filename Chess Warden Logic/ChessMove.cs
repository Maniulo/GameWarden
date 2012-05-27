using System;

namespace GameWarden.Chess
{
    public class ChessMove : IConcreteMove
    {
        public Player Player;
        public Position From;
        public Position To;
        protected IConcreteMove Move;

        public PieceTypes PieceType;
        public String Desc;

        public Boolean CastlingKingside;
        public Boolean CastlingQueenside;
        public bool IsPromotion;
        public PieceTypes PromotionTo;
        private Position SavedEnPassant;

        protected virtual IConcreteMove Solve(ChessState state)
        {
            if (CastlingKingside)
            {
                From = new Position(5, Player.Order == 1 ? 1 : 8);
                To = new Position(7, Player.Order == 1 ? 1 : 8);
            }

            if (CastlingQueenside)
            {
                From = new Position(5, Player.Order == 1 ? 1 : 8);
                To = new Position(3, Player.Order == 1 ? 1 : 8);
            }

            foreach (ChessPiece p in state)
                if (!p.IsEmpty)
                    if (p.Type == PieceType &&
                        p.Player == Player &&
                        From.Equals(p.Pos) &&
                        p.CanMove(To, state, PromotionTo))
                    {
                        var mv = p.GetPossibleMove(To, state, PromotionTo);

                        mv.Apply(state);
                        if (!state.IsKingOpen(Player))
                        {
                            mv.Rollback(state);
                            From = p.Pos;
                            return mv;
                        }
                        else
                        {
                            mv.Rollback(state);
                        }
                    }

            return null;
        }

        public ChessMove(String desc)
        {
            Desc = desc;
        }

        public override string ToString()
        {
            return Desc;
        }

        public void Apply(IGameState state)
        {
            if (state is ChessState)    // ???
            {
                var cs = state as ChessState;

                if (Move == null)
                    Move = Solve(cs);

                if (Move == null)
                    throw new Exception(String.Format("Move \"{0}\" cannot be solved.", Desc));

                SavedEnPassant = ((ChessState) state).EnPassant;
                if (!(Move is EnPassantConcrete))
                    ((ChessState) state).EnPassant = null;
                Move.Apply(state);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void Rollback(IGameState state)
        {
            Move.Rollback(state);
            ((ChessState)state).EnPassant = SavedEnPassant;
        }
    }
}
