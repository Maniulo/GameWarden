namespace GameWarden.Chess
{
    public class KnightMove : Move
    {
        public KnightMove()
            : base(null, null, false) {}

        public override bool CanApply(Position @from, Position to, IGameState state)
        {
            if (Position.FileDistance(@from, to) == 2 && Position.RankDistance(@from, to) == 1 ||
                Position.FileDistance(@from, to) == 1 && Position.RankDistance(@from, to) == 2)
            {
                return base.CanApply(@from, to, state);
            }
            else
                return false;
        }
    }
}