using System;
using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    public enum PieceTypes
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public interface IChessPiecePresentation : IPiecePresentation
    {
        PieceTypes GetPieceType(char? c);
    }

    public class EnglishPresentation : IChessPiecePresentation
    {
        virtual public Char? GetPresentation(IPiece p)
        {
            var piece = p as ChessPiece;

            if (piece != null)
                switch (piece.Type)
                {
                    case PieceTypes.Pawn:
                        return null;
                    case PieceTypes.Knight:
                        return 'N';
                    case PieceTypes.Bishop:
                        return 'B';
                    case PieceTypes.Rook:
                        return 'R';
                    case PieceTypes.Queen:
                        return 'Q';
                    case PieceTypes.King:
                        return 'K';
                }

            throw new ArgumentException();
        }

        virtual public IPiece GetPiece(char? c)
        {
            return new ChessPiece { Type = GetPieceType(c) };
        }

        virtual public PieceTypes GetPieceType(char? c)
        {
            if (c.HasValue)
            {
                switch (c.Value)
                {
                    case 'N':
                        return PieceTypes.Knight;
                    case 'B':
                        return PieceTypes.Bishop;
                    case 'R':
                        return PieceTypes.Rook;
                    case 'Q':
                        return PieceTypes.Queen;
                    case 'K':
                        return PieceTypes.King;
                }

                throw new ArgumentException();
            }
            else
                return PieceTypes.Pawn;
        }

        public override string ToString()
        {
            return "NBRQK";
        }
    }

    public class EnglishFENPresentation : IChessPiecePresentation
    {
        readonly EnglishPresentation Template = new EnglishPresentation();

        public char? GetPresentation(IPiece p)
        {
            var piece = p as ChessPiece;

            if (piece == null)
                throw new ArgumentException();

            var c = (piece.Type == PieceTypes.Pawn) ? 'p' : Template.GetPresentation(piece).Value;

            switch (p.Player.Order)
            {
                case 1:
                    c = Char.ToUpper(c);
                    break;
                case 2:
                    c = Char.ToLower(c);
                    break;
            }

            return c;
        }

        public IPiece GetPiece(char? c)
        {
            return new ChessPiece { Type = GetPieceType(c), Player = Char.IsUpper(c.Value) ? new Player(1) : new Player(2) };
        }

        public PieceTypes GetPieceType(char? c)
        {
            c = Char.ToUpper(c.Value);

            return c == 'P' ? PieceTypes.Pawn : Template.GetPieceType(c);
        }

        public override string ToString()
        {
            return "NBRQKPnbrqkp";
        }
    }

    public class FigurinePresentation : IChessPiecePresentation
    {
        Char GetWhitePieceSymbol(IPiece p)
        {
            var piece = p as ChessPiece;

            if (piece != null)
                switch (piece.Type)
                {
                    case PieceTypes.Pawn:
                        return '♙';
                    case PieceTypes.Knight:
                        return '♘';
                    case PieceTypes.Bishop:
                        return '♗';
                    case PieceTypes.Rook:
                        return '♖';
                    case PieceTypes.Queen:
                        return '♕';
                    case PieceTypes.King:
                        return '♔';
                }

            throw new Exception();
        }

        Char GetBlackPieceSymbol(IPiece p)
        {
            var piece = p as ChessPiece;

            if (piece != null)
                switch (piece.Type)
                {
                    case PieceTypes.Pawn:
                        return '♟';
                    case PieceTypes.Knight:
                        return '♞';
                    case PieceTypes.Bishop:
                        return '♝';
                    case PieceTypes.Rook:
                        return '♜';
                    case PieceTypes.Queen:
                        return '♛';
                    case PieceTypes.King:
                        return '♚';
                }

            throw new Exception();
        }

        virtual public IPiece GetPiece(Char? o)
        {
            return new ChessPiece();
            /*
            switch (o)
            {
                case '♙': return new Pawn(PieceColour.White);
                case '♘': return new Knight(PieceColour.White);
                case '♗': return new Bishop(PieceColour.White);
                case '♖': return new Rook(PieceColour.White);
                case '♕': return new Queen(PieceColour.White);
                case '♔': return new King(PieceColour.White);
                case '♟': return new Pawn(PieceColour.Black);
                case '♞': return new Knight(PieceColour.Black);
                case '♝': return new Bishop(PieceColour.Black);
                case '♜': return new Rook(PieceColour.Black);
                case '♛': return new Queen(PieceColour.Black);
                case '♚': return new King(PieceColour.Black);
                default: return new NullPiece();
            }*/
        }

        virtual public Char? GetPresentation(IPiece p)
        {
            if (p.IsEmpty)
                return null;

            return p.Player.Order == 1 ? GetWhitePieceSymbol(p) : GetBlackPieceSymbol(p);
        }

        public PieceTypes GetPieceType(char? c)
        {
            throw new NotImplementedException();
        }
    }

    public class ChessPieceFactory
    {
        public ChessPiece CreatePiece(Char? type, IPiecePresentation presentation)
        {
            var p = presentation.GetPiece(type) as ChessPiece;
            if (p == null)
                throw new ArgumentException();
            
            switch (p.Type)
            {
                case PieceTypes.Pawn:
                    p.AddPossibleMove(new PawnMove());
                    p.AddPossibleMove(new PawnCapture());
                    p.AddPossibleMove(new EnPassant());
                    break;
                case PieceTypes.Knight:
                    p.AddPossibleMove(new KnightMoveTemplate());
                    break;
                case PieceTypes.Bishop:
                    p.AddPossibleMove(new DiagonalMoveTemplate());
                    break;
                case PieceTypes.Rook:
                    p.AddPossibleMove(new HorizontalMoveTemplate());
                    p.AddPossibleMove(new VerticalMoveTemplate());
                    break;
                case PieceTypes.Queen:
                    p.AddPossibleMove(new HorizontalMoveTemplate());
                    p.AddPossibleMove(new VerticalMoveTemplate());
                    p.AddPossibleMove(new DiagonalMoveTemplate());
                    break;
                case PieceTypes.King:
                    p.AddPossibleMove(new HorizontalMoveTemplate(1));
                    p.AddPossibleMove(new VerticalMoveTemplate(1));
                    p.AddPossibleMove(new DiagonalMoveTemplate(1));
                    p.AddPossibleMove(new Castling(Castling.CastlingType.Kingside));
                    p.AddPossibleMove(new Castling(Castling.CastlingType.Queenside));
                    break;
            }

            return p;
        }
    }
}