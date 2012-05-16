using System;

namespace GameWarden.Chess
{
    public class PawnMove : VerticalMove
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
    }

    public class PawnCapture : Move
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

    public class EnPassant : TemplateMove
    {
        private IPiece capturedPiece;

        Position EnemyPawn(ChessState state)
        {
            return new Position(state.EnPassant.File, state.EnPassant.Rank - 1);
        }

        public override void Apply(Position @from, Position to, IGameState state)
        {
            capturedPiece = state[EnemyPawn((ChessState)state)];    // !!!
            state.RemovePiece(capturedPiece.Pos);
            state.MovePiece(@from, to);
        }

        public override bool CanApply(Position from, Position to, IGameState state)
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

        public override bool IsCapture
        {
            get { return true; }
        }

        public override void Rollback(Position from, Position to, IGameState state)
        {

        }
    }
}