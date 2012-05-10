using System;

namespace GameWarden.Chess
{
    public abstract class Castling : HorizontalMove
    {
        protected void RefreshRanks(Position from, Position to, IGameState state)
        {
            rookFrom.Rank = from.Rank;
            rookTo.Rank = to.Rank;
        }

        protected Position rookFrom;
        protected Position rookTo;

        protected Castling()
            : base(null, false, true)
        {

        }

        public override void Apply(Position from, Position to, IGameState state)
        {
            RefreshRanks(from, to, state);
            state.MovePiece(from, to);
            state.MovePiece(rookFrom, new Position(rookTo));
        }

        public override bool CanApply(Position from, Position to, IGameState state)
        {
            RefreshRanks(from, to, state);
            if (((ChessPiece)state[rookFrom]).Type == PieceTypes.Rook)
                if (((ChessPiece)state[from]).Path.Count == 1 && ((ChessPiece)state[rookFrom]).Path.Count == 1)
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