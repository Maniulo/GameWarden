using System;
using System.Collections.Generic;
using System.Linq;
using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    public class ChessState : GameState, IEnumerable<ChessPiece>
    {
        public String Player;
        public Boolean CastlingKingsideWhite;
        public Boolean CastlingKingsideBlack;
        public Boolean CastlingQueensideWhite;
        public Boolean CastlingQueensideBlack;
        public Position EnPassant;
        public String HalfMoves;
        public String FullMoves;

        public Boolean IsKingOpen(Player defencePlayer)
        {
            ChessPiece king = GetKing(defencePlayer);
            if (king != null)
                return IsUnderAttack(king.Pos, defencePlayer);
            else
                return false;
        }

        protected ChessPiece GetKing(Player player)
        {
            return this.FirstOrDefault(p => !p.IsEmpty && p.Type == PieceTypes.King && p.Player == player);
        }

        public Boolean IsUnderAttack(Position pos, Player defencePlayer)
        {
            foreach (ChessPiece p in this)
                if (!p.IsEmpty && p.Player != defencePlayer && p.CanAttack(pos, this))
                    return true;

            return false;
        }

        public ChessState()
            : base(8, 8) { }

        public override IPiece CreateEmptyPiece(Position pos)
        {
            var p = new ChessPiece { IsEmpty = true };
            p.Move(pos);
            return p;
        }

        public override string ToString()
        {
            return new FENParser().Generate(this);
        }
        
        public IEnumerator<ChessPiece> GetEnumerator()
        {
            for (var rank = 8; rank >= 1; --rank)
                for (var file = 1; file <= 8; ++file)
                    yield return (ChessPiece)this[file, rank];
        }

        /*
        public ChessState(ChessState copy)
            : this()
        {
            Player = copy.Player;
            CastlingKingsideBlack = copy.CastlingKingsideBlack;
            CastlingKingsideWhite = copy.CastlingKingsideWhite;
            CastlingQueensideWhite = copy.CastlingQueensideWhite;
            CastlingQueensideBlack = copy.CastlingQueensideBlack;
            EnPassant = copy.EnPassant;
            HalfMoves = copy.HalfMoves;
            FullMoves = copy.FullMoves;

            for (int file = 0; file < 8; ++file)
                for (int rank = 0; rank < 8; ++rank)
                    Board[file, rank] = new ChessPiece((ChessPiece)copy.Board[file, rank]);
        }*/

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}