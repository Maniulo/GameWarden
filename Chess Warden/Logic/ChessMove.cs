using System;

namespace GameWarden.Chess
{
    public class ChessMove : ConcreteMove
    {
        public PieceTypes PieceType;
        public String Desc;

        public Boolean CastlingKingside;
        public Boolean CastlingQueenside;
        
        protected virtual TemplateMove Solve(ChessState state)
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
                        p.CanMove(To, state))
                    {
                        var cpState = new ChessState(state);    // !!!
                        var mv = p.GetPossibleMove(To, state);
                        mv.Apply(p.Pos, To, cpState);
                        if (!cpState.IsKingOpen(Player))
                        {
                            From = p.Pos;
                            return mv;
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

        public override void Apply(IGameState state)
        {
            if (state is ChessState)    // ???
            {
                var cs = state as ChessState;

                if (Move == null)
                    Move = Solve(cs);

                if (Move == null)
                    throw new Exception(String.Format("Move \"{0}\" cannot be solved.", Desc));

                base.Apply(state);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public override bool CanApply(IGameState state)
        {
            if (state is ChessState)    // ???
            {
                var cs = state as ChessState;

                if (Move == null)
                    Move = Solve(cs);

                if (Move == null)
                    throw new Exception(String.Format("Move \"{0}\" cannot be solved.", Desc));

                return base.CanApply(cs);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
