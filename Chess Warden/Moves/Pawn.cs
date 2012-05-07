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
                    if (to.Rank - @from.Rank <= 2)
                    {
                        return base.CanApply(@from, to, state);
                    }
                    else return false;
                case 2:
                    if (to.Rank - @from.Rank >= -2)
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
        Position EnemyPawn(ChessState state)
        {
            return new Position(state.EnPassant.File, state.EnPassant.Rank - 1);
        }

        public override void Apply(Position From, Position To, IGameState state)
        {
            state.RemovePiece(EnemyPawn((ChessState)state)); // !!!
            state.MovePiece(From, To);
        }

        public override bool CanApply(Position @from, Position to, IGameState state)
        {
            return to.Equals(((ChessState)state).EnPassant);
        }
    }
}