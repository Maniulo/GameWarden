using System;

namespace GameWarden.Chess
{
    public class CastlingConcrete : IConcreteMove
    {
        private readonly Position RookFrom;
        private readonly Position RookTo;
        private readonly Position From;
        private readonly Position To;

        public CastlingConcrete(Position from, Position to, Position rookFrom, Position rookTo)
        {
            From = from;
            To = to;
            RookFrom = rookFrom;
            RookTo = rookTo;
        }

        public void Apply(IGameState state)
        {
            state.MovePiece(From, To);
            state.MovePiece(RookFrom, RookTo);
        }
        
        public void Rollback(IGameState state)
        {
            state.MovePieceN(From, To);
            state.MovePieceN(RookFrom, RookTo);
        }
    }

    public class Castling : HorizontalMoveTemplate
    {
        public enum CastlingType
        {
            Kingside,
            Queenside
        }
        
        private readonly CastlingType CType;

        private int KingToFile { get { return CType == CastlingType.Kingside ? 7 : 3; } }
        private int RookFromFile { get { return CType == CastlingType.Kingside ? 8 : 1; } }
        private int RookToFile { get { return CType == CastlingType.Kingside ? 6 : 4; } }

        public Castling(CastlingType type)
            : base(null, false, true)
        {
            CType = type;
        }

        public override bool CanApply(Position from, Position to, IGameState state)
        {
            var cState = state as ChessState;   // !!!
            var rook = cState[RookFromFile, from.Rank] as ChessPiece;
            var king = cState[from] as ChessPiece;

            if (rook.Type == PieceTypes.Rook && to.File == KingToFile)
                if (king.Path.Count == 1 && rook.Path.Count == 1 && !cState.IsKingOpen(state[from].Player))
                    if (base.CanApply(from, to, state))
                    {
                        Boolean result = true;
                        switch (CType)
                        {
                            case CastlingType.Kingside:
                                for (int kingPathFile = from.File; kingPathFile <= to.File; ++kingPathFile)
                                    result &= !cState.IsUnderAttack(new Position(kingPathFile, from.Rank), state[from].Player);
                                break;
                            case CastlingType.Queenside:
                                for (int kingPathFile = from.File; kingPathFile >= to.File; --kingPathFile)
                                    result &= !cState.IsUnderAttack(new Position(kingPathFile, from.Rank),
                                                                                 state[from].Player);
                                break;
                        }
                        

                        return result;
                    }

            return false;
        }

        public override IConcreteMove Concretize(Position from, Position to)
        {
            return new CastlingConcrete(from, to, new Position(RookFromFile, from.Rank), new Position(RookToFile, to.Rank));
        }
    }
}