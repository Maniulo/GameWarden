using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public Char Player;
        public CastlingPossibility Castling;
        public Position EnPassant;
        public int HalfMoves;
        public int FullMoves;

        public Boolean IsKingOpen(Player defencePlayer)
        {
            var king = GetKing(defencePlayer);
            return king != null && IsUnderAttack(king.Pos, defencePlayer);
        }

        protected ChessPiece GetKing(Player player)
        {
            return ((ChessPlayer) player).MyKing;
        }

        public Boolean IsUnderAttack(Position pos, Player defencePlayer)
        {
            return this.Any(p => !p.IsEmpty && p.Player != defencePlayer && ((ChessPiece) p).CanAttack(pos, this));
        }

        public ChessState()
            : base(8, 8) { }

        public ChessState(ChessState o)
            : this()
        {
            Player = o.Player;

            Castling = o.Castling;

            if (o.EnPassant != null)
                EnPassant = new Position(o.EnPassant);
            HalfMoves = o.HalfMoves;
            FullMoves = o.FullMoves;

            foreach (var p in o)
                Board[p.Pos.File-1, p.Pos.Rank-1] = p;
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

        public override void PlaceEmptyPiece(Position pos)
        {
            var p = new ChessPiece { IsEmpty = true };
            PlacePiece(pos, p);
        }

        public override void PlacePiece(Position pos, IPiece p)
        {
            var cp = p as ChessPiece;

            if (cp != null)
            {
                if (cp.Type == PieceTypes.King)
                {
                    ((ChessPlayer)p.Player).MyKing = cp;
                }

                base.PlacePiece(pos, p);
            }
            else
            {
                throw new ArgumentException();
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
    }
}