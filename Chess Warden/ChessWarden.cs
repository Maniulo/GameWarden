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

        public ChessState()
            : base(8, 8) { }

        public override string ToString()
        {
            return new FEN().Generate(this);
        }
        
        public IEnumerator<IPiece> GetEnumerator()
        {
            for (var rank = 8; rank >= 1; --rank)
                for (var file = 1; file <= 8; ++file)
                    yield return this[file, rank];
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class ChessGame : Game<ChessState>
    {
        public ChessGame(Meta metainfo)
            : base(2)
        {
            Info = metainfo;
            States.Add(new FEN().Parse(metainfo["FEN"] ?? FEN.DefaultFEN, this.Players));
        }

        public override string ToString()
        {
            return new FEN().Generate(this.CurrentState);
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
                    p.AddPossibleMove(new Kingside());
                    p.AddPossibleMove(new Queenside());
                    break;
            }

            return p;
        }
    }

    public class ChessPiece : Piece
    {
        public PieceTypes Type;
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
