using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    public class ChessState : GameState
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
            var king = GetKing(defencePlayer);
            return king != null && IsUnderAttack(king.Pos, defencePlayer);
        }

        protected ChessPiece GetKing(Player player)
        {
            return (ChessPiece)this.FirstOrDefault(p => !p.IsEmpty && ((ChessPiece)p).Type == PieceTypes.King && p.Player == player);
        }

        public Boolean IsUnderAttack(Position pos, Player defencePlayer)
        {
            return this.Any(p => !p.IsEmpty && p.Player != defencePlayer && ((ChessPiece) p).CanAttack(pos, this));
        }

        public ChessState()
            : base(8, 8) { }

        public override IPiece CreateEmptyPiece(Position pos) // !!!
        {
            var p = new ChessPiece { IsEmpty = true };
            p.Move(pos);
            return p;
        }

        public override string ToString()
        {
            return new FENParser().Generate(this);
        }

        
    }
}