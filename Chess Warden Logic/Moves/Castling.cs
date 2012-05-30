using System;

namespace GameWarden.Chess
{
    public class Castling : BaseConcreteMove
    {
        private readonly Position RookFrom;
        private readonly Position RookTo;
        private readonly Position From;
        private readonly Position To;

        private CastlingPossibility _Castling;

        public Castling(Position from, Position to, Position rookFrom, Position rookTo)
        {
            From = from;
            To = to;
            RookFrom = rookFrom;
            RookTo = rookTo;
        }

        public override void Apply(IGameState state)
        {
            var s = state as ChessState;

            _Castling = s.Castling;
            switch(state[From].Player.Order)
            {
                case 1:
                    s.Castling.KingsideWhite  = false;
                    s.Castling.QueensideWhite = false;
                    break;
                case 2:
                    s.Castling.KingsideBlack  = false;
                    s.Castling.QueensideBlack = false;
                    break;
            }
            
            MovePiece(From, To, state);
            MovePiece(RookFrom, RookTo, state);
        }

        public override void Rollback(IGameState state)
        {
            var s = state as ChessState;
            s.Castling = _Castling;
            RollbackMovePiece(From, To, state);
            RollbackMovePiece(RookFrom, RookTo, state);
        }
    }

    public class CastlingTemplate : HorizontalMoveTemplate
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

        public CastlingTemplate(CastlingType type)
            : base(null, false, true)
        {
            CType = type;
        }

        public override bool CanApply(Position from, Position to, IGameState state)
        {
            var cState = state as ChessState;
            var rook = cState[new Position(RookFromFile, from.Rank)] as ChessPiece;
            var king = cState[from] as ChessPiece;

            return  rook.Type == PieceTypes.Rook &&
                    rook.Player == king.Player &&
                    rook.PathLength == 1 &&
                    king.PathLength == 1 &&
                    to.File == KingToFile &&
                    !cState.IsKingOpen(state[from].Player) &&
                    base.CanApply(from, to, state) &&
                    new HorizontalMoveTemplate(null, false).CanApply(new Position(RookFromFile, from.Rank), new Position(RookToFile, to.Rank), state) &&
                    IsKingsPathFree(from, to, cState);
        }

        private Boolean IsKingsPathFree(Position from, Position to, ChessState cState)
        {
            Boolean result = true;

            switch (CType)
            {
                case CastlingType.Kingside:
                    for (int kingPathFile = from.File + 1; kingPathFile <= to.File; ++kingPathFile)
                        result &= !cState.IsUnderAttack(new Position(kingPathFile, from.Rank), cState[from].Player);
                    break;
                case CastlingType.Queenside:
                    for (int kingPathFile = from.File - 1; kingPathFile >= to.File; --kingPathFile)
                        result &= !cState.IsUnderAttack(new Position(kingPathFile, from.Rank), cState[from].Player);
                    break;
            }

            return result;
        }

        public override IConcreteMove Concretize(Position from, Position to)
        {
            return new Castling(from, to, new Position(RookFromFile, from.Rank), new Position(RookToFile, to.Rank));
        }
    }
}