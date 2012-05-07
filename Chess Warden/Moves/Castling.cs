namespace GameWarden.Chess
{
    public abstract class Castling : HorizontalMove
    {
        protected void RefreshRanks(Position From, Position To, IGameState state)
        {
            if (state[From].Player.Order == 1)
            {
                To.Rank = 1;
            }
            else if (state[From].Player.Order == 2)
            {
                To.Rank = 8;
            }

            rookFrom.Rank = From.Rank;
            rookTo.Rank = To.Rank;
        }

        protected Position rookFrom;
        protected Position rookTo;

        public Castling()
            : base(null, false)
        {

        }

        public override void Apply(Position From, Position To, IGameState state)
        {
            RefreshRanks(From, To, state);
            state.MovePiece(From, To);
            state.MovePiece(rookFrom, rookTo);
        }

        public override bool CanApply(Position @from, Position to, IGameState state)
        {
            RefreshRanks(@from, to, state);
            
            if (@from.File == 5 && state[@from].Player.Order == 1 ? @from.Rank == 1 : @from.Rank == 8)
            {
                return base.CanApply(@from, to, state);
            }
            else
            {
                return false;
            }
        }
    }

    public class Kingside : Castling
    {
        public Kingside()
        {
            rookFrom = new Position(8, null);
            rookTo = new Position(6, null);
        }
    }

    public class Queenside : Castling
    {
        public Queenside()
        {
            rookFrom = new Position(1, null);
            rookTo = new Position(3, null);
        }
    }
}