using System;

namespace GameWarden.Chess
{
    public class CastlingConcrete : IConcreteMove
    {
        private Position RookFrom;
        private Position RookTo;
        private Position From;
        private Position To;

        public CastlingConcrete(Position from, Position to, Position rookFrom, Position rookTo)
        {
            From = from;
            To = to;
            RookFrom = rookFrom;
            RookTo = rookTo;
        }

        public void Rollback(IGameState state)
        {
            state.MovePieceN(From, To);
            state.MovePieceN(RookFrom, RookTo);
        }

        public void Apply(IGameState state)
        {
            state.MovePiece(From, To);
            state.MovePiece(RookFrom, RookTo);
        }
    }

    public abstract class Castling : HorizontalMove
    {
        protected Castling()
            : base(null, false, true)
        {

        }

        protected abstract Position RookFrom(Position king);
        protected abstract Position RookTo(Position king);

        public override bool CanApply(Position from, Position to, IGameState state)
        {
            if (((ChessPiece)state[RookFrom(from)]).Type == PieceTypes.Rook)
                if (((ChessPiece)state[from]).Path.Count == 1 && ((ChessPiece)state[RookFrom(from)]).Path.Count == 1)
                    if (base.CanApply(from, to, state))
                    {
                        Boolean result = true;
                        for (int kingPathFile = from.File; kingPathFile <= to.File; ++kingPathFile)
                            result &= !((ChessState) state).IsUnderAttack(new Position(kingPathFile, from.Rank),
                                                                         state[from].Player);

                        return result;
                    }

            return false;
        }

        public override IConcreteMove Concretize(Position from, Position to)
        {
            return new CastlingConcrete(from, to, RookFrom(from), RookTo(from));
        }
    }

    public class Kingside : Castling
    {
        protected override Position RookFrom(Position king)
        {
            return new Position(8, king.Rank);
        }

        protected override Position RookTo(Position king)
        {
            return new Position(6, king.Rank);
        }
    }

    public class Queenside : Castling
    {
        protected override Position RookFrom(Position king)
        {
            return new Position(1, king.Rank);
        }

        protected override Position RookTo(Position king)
        {
            return new Position(3, king.Rank);
        }
    }
}