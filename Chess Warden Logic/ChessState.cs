using System;
using System.Linq;
using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    public struct CastlingPossibility
    {
        public Boolean KingsideWhite;
        public Boolean KingsideBlack;
        public Boolean QueensideWhite;
        public Boolean QueensideBlack;
    }

    public class ChessState : GameState
    {
        public Char Player = 'w';
        public CastlingPossibility Castling;
        public Position EnPassant;
        public int HalfMoves;
        public int FullMoves = 1;

        public ChessState() : base(8, 8) { }
        public ChessState(ChessState o) : this()
        {
            Player = o.Player;
            Castling = o.Castling;

            EnPassant = o.EnPassant;
            HalfMoves = o.HalfMoves;
            FullMoves = o.FullMoves;
            
            foreach (ChessPiece p in o)
                Board[p.Pos.File - 1, p.Pos.Rank - 1] = new ChessPiece(p);
        }

        public Boolean IsUnderAttack(Position pos, Player defencePlayer)
        {
            return this.Cast<ChessPiece>().Any(p => !p.IsEmpty && p.Player != defencePlayer && p.CanAttack(pos, this));
        }
        public Boolean IsKingOpen(Player defencePlayer)
        {
            var king = GetKing(defencePlayer);
            return king != null && IsUnderAttack(king.Pos, defencePlayer);
        }
        protected ChessPiece GetKing(Player player)
        {
            return this.Cast<ChessPiece>().FirstOrDefault(p => !p.IsEmpty && p.Type == PieceTypes.King && p.Player == player);
        }
        
        public void SwitchPlayers()
        {
            switch (Player)
                {
                    case 'w':
                        Player = 'b';
                        break;
                    case 'b':
                        Player = 'w';
                        break;
                }
        }

        public override string ToString()
        {
            return FENParser.Generate(this);
        }
        public string ToStringShort()
        {
            return FENParser.GenerateBoard(this);
        }

        public override void NewEmptyPiece(Position pos)
        {
            var p = new ChessPiece { IsEmpty = true };
            this[pos] = p;
            p.Move(pos);
        }
    }
}