using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameWarden;
using System.Text.RegularExpressions;
using GameWarden.Chess.Notations;
namespace GameWarden.Chess
{
    public class ChessState : GameState, IEnumerable<IPiece>
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
            return this.Cast<ChessPiece>().FirstOrDefault(p => !p.IsEmpty && p.Type == PieceTypes.King && p.Player == player);
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
        
        public IEnumerator<IPiece> GetEnumerator()
        {
            for (var rank = 8; rank >= 1; --rank)
                for (var file = 1; file <= 8; ++file)
                    yield return this[file, rank];
        }

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
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class ChessGame : Game<ChessState, ChessMove>
    {
        public ChessGame(Meta metainfo)
            : base(2)
        {
            Info = metainfo;
            States.Add(new FENParser().Parse(metainfo["FEN"] ?? FENParser.DefaultFEN, this.Players));
        }

        public override string ToString()
        {
            return new FENParser().Generate(this.CurrentState);
        }
    }

    public class ChessPieceFactory
    {
        public ChessPiece CreatePiece(Char? type, IPiecePresentation presentation)
        {
            ChessPiece p = presentation.GetPiece(type);

            switch (p.Type)
            {
                case PieceTypes.Pawn:
                    p.AddPossibleMove(new PawnMove());
                    p.AddPossibleMove(new PawnCapture());
                    p.AddPossibleMove(new EnPassant());
                    break;
                case PieceTypes.Knight:
                    p.AddPossibleMove(new KnightMove());
                    break;
                case PieceTypes.Bishop:
                    p.AddPossibleMove(new DiagonalMove());
                    break;
                case PieceTypes.Rook:
                    p.AddPossibleMove(new HorizontalMove());
                    p.AddPossibleMove(new VerticalMove());
                    break;
                case PieceTypes.Queen:
                    p.AddPossibleMove(new HorizontalMove());
                    p.AddPossibleMove(new VerticalMove());
                    p.AddPossibleMove(new DiagonalMove());
                    break;
                case PieceTypes.King:
                    p.AddPossibleMove(new HorizontalMove(1));
                    p.AddPossibleMove(new VerticalMove(1));
                    p.AddPossibleMove(new DiagonalMove(1));
                    p.AddPossibleMove(new Kingside());
                    p.AddPossibleMove(new Queenside());
                    break;
            }

            return p;
        }
    }

    public class ChessPiece : Piece
    {
        public List<Position> Path;
        public PieceTypes Type;

        public ChessPiece()
        {
            Path = new List<Position>();
        }

        public override void Move(Position pos)
        {
            Path.Add(pos);
            base.Move(pos);
        }

        public ChessPiece(ChessPiece copy)
            :base(copy)
        {
            Path = new List<Position>();
            foreach (Position pos in copy.Path)
                Path.Add(new Position(pos));
            Type = copy.Type;
        }
    }

    public enum PieceTypes
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }
}
