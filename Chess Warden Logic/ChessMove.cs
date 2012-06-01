using System;
using System.Collections.Generic;

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

        public ChessMove(String desc)
        {
            Desc = desc;
        }

        public void Apply(IGameState state)
        {
            if (state is ChessState)
            {
                var cs = state as ChessState;

                if (Move == null)
                    Move = Solve(cs);

                SavedEnPassant = cs.EnPassant;
                if (!(Move is EnPassantConcrete))
                    cs.EnPassant = null;

                if (Player.Order == 2)
                    ++cs.FullMoves;
                cs.SwitchPlayers();

                Move.Apply(state);
            }
            else
            {
                throw new ArgumentException();
            }
        }
        public void Rollback(IGameState state)
        {
            if (state is ChessState)
            {
                var cs = state as ChessState;
                Move.Rollback(state);
                cs.SwitchPlayers();
                cs.EnPassant = SavedEnPassant;
                if (Player.Order == 2)
                    --cs.FullMoves;
            }
            else
            {
                throw new ArgumentException();
            }
        }

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

            var possibleMoves = new List<IConcreteMove>();
            foreach (ChessPiece p in state)
                if (!p.IsEmpty &&
                    p.Type == PieceType &&
                    p.Player == Player &&
                    From.Equals(p.Pos))
                {
                    var mv = p.GetPossibleMove(To, state, PromotionTo);

                    if (mv != null)
                    {
                        possibleMoves.Add(mv);
                        mv.Apply(state);
                        if (!state.IsKingOpen(Player))
                            From = p.Pos;

                        mv.Rollback(state);
                    }
                }

            if (possibleMoves.Count != 1)
            {
                throw new Exception("Ambiguous move.");
            } 
            else
            {
                return possibleMoves[0];
            }
        }

        public override string ToString()
        {
            return Desc;
        }
    }
}
