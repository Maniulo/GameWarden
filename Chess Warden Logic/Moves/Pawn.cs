using System;

namespace GameWarden.Chess
{
    public class PawnMove : VerticalMoveTemplate
    {
        public PawnMove()
            : base(2, false) { }

        public override bool CanApply(Position @from, Position to, IGameState state)
        {
            switch (state[@from].Player.Order)
            {
                case 1:
                    if (to.Rank - @from.Rank == 1 || to.Rank - @from.Rank == 2 && from.Rank == 2)
                    {
                        return base.CanApply(@from, to, state);
                    }
                    else return false;
                case 2:
                    if (from.Rank - to.Rank == 1 || from.Rank - to.Rank == 2 && from.Rank == 7)
                    {
                        return base.CanApply(@from, to, state);
                    }
                    else return false;
                default:
                    throw new ArgumentException();
            }
        }

        public override IConcreteMove Concretize(Position from, Position to)
        {
            return new PawnConcreteMove(from, to);
        }
    }

    public class PawnConcreteMove : ConcreteMove
    {
        private Position From;
        private Position To;
        private bool IsEnPassant;
        private Position EnPassantOld;

        public PawnConcreteMove(Position @from, Position to) : base(@from, to, false)
        {
            From = from;
            To = to;
            IsEnPassant = Position.RankDistance(from, to) == 2;
        }

        public override void Apply(IGameState state)
        {
            if (IsEnPassant)
            {
                EnPassantOld = ((ChessState) state).EnPassant;
                switch (state[From].Player.Order)
                {
                    case 1:
                        ((ChessState)state).EnPassant = new Position(To.File, To.Rank - 1);
                        break;
                    case 2:
                        ((ChessState)state).EnPassant = new Position(To.File, To.Rank + 1);
                        break;
                }
            }
            base.Apply(state);
        }

        public override void Rollback(IGameState state)
        {
            ((ChessState)state).EnPassant = EnPassantOld;
            base.Rollback(state);
        }
    }

    public class PawnCapture : TemplateMove
    {
        public PawnCapture()
            : base(1, true)
        {

        }

        public override bool CanApply(Position @from, Position to, IGameState state)
        {
            if (Position.FileDistance(to, @from) == 1)
            {
                switch (state[@from].Player.Order)
                {
                    case 1:
                        return to.Rank - @from.Rank == 1 && base.CanApply(@from, to, state);
                    case 2:
                        return @from.Rank - to.Rank == 1 && base.CanApply(@from, to, state);
                }
            }

            return false;
        }
    }

    public class EnPassant : ITemplateMove
    {
        public bool IsCapture
        {
            get { return true; }
        }

        public bool CanApply(Position from, Position to, IGameState state)
        {
            if (to.Equals(((ChessState)state).EnPassant))
                if (Position.FileDistance(to, from) == 1)
                    switch (state[from].Player.Order)
                    {
                        case 1:
                            return to.Rank - from.Rank == 1;
                        case 2:
                            return from.Rank - to.Rank == 1;
                    }

            return false;
        }

        public IConcreteMove Concretize(Position from, Position to)
        {
            return new EnPassantConcrete(from, to);
        }
    }

    public class EnPassantConcrete : IConcreteMove
    {
        private readonly Position From;
        private readonly Position To;
        private IPiece CapturedPiece;

        public EnPassantConcrete(Position from, Position to)
        {
            From = from;
            To = to;
        }

        private static Position EnemyPawn(ChessState state)
        {
            switch (state.EnPassant.Rank)
            {
                case 3: return new Position(state.EnPassant.File, state.EnPassant.Rank + 1);
                case 6: return new Position(state.EnPassant.File, state.EnPassant.Rank - 1);
            }
            throw new ArgumentException();
        }

        public void Apply(IGameState state)
        {
            CapturedPiece = state[EnemyPawn((ChessState)state)];    // !!!
            state.RemovePiece(CapturedPiece.Pos);
            state.MovePiece(From, To);
        }

        public void Rollback(IGameState state)
        {
            state.MovePieceN(From, To);
            state.RemovePieceN(CapturedPiece);
        }
    }
}