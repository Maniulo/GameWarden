using System;

namespace GameWarden.Chess.Notations
{
    public interface IPiecePresentation
    {
        Char? GetPresentation(ChessPiece p);
        ChessPiece GetPiece(char? c);
        PieceTypes GetType(char? c);
    }

    public class EnglishPresentation : IPiecePresentation
    {
        virtual public Char? GetPresentation(ChessPiece p)
        {
            switch (p.Type)
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

        virtual public ChessPiece GetPiece(char? c)
        {
            return new ChessPiece { Type = GetType(c) };
        }

        virtual public PieceTypes GetType(char? c)
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

    public class EnglishFENPresentation : IPiecePresentation
    {
        EnglishPresentation Template = new EnglishPresentation();

        public char? GetPresentation(ChessPiece p)
        {
            char c;

            if (p.Type == PieceTypes.Pawn)
            {
                c = 'p';
            }
            else
            {
                c = Template.GetPresentation(p).Value;
            }

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

        public ChessPiece GetPiece(char? c)
        {
            var p = new ChessPiece();
            p.Type = GetType(c);
            if (Char.IsUpper(c.Value))
            {
                p.Player = new Player(1);
            }
            else
            {
                p.Player = new Player(2);
            }
            return p;
        }

        public PieceTypes GetType(char? c)
        {
            c = Char.ToUpper(c.Value);

            if (c == 'P')
            {
                return PieceTypes.Pawn;
            }
            else
            {
                return Template.GetType(c);
            }
        }

        public override string ToString()
        {
            return "NBRQKPnbrqkp";
        }
    }

    public class FigurinePresentation : IPiecePresentation
    {
        Char GetWhitePieceSymbol(ChessPiece p)
        {
            switch (p.Type)
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
                default:
                    throw new Exception();
            }
        }

        Char GetBlackPieceSymbol(ChessPiece p)
        {
            switch (p.Type)
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
                default:
                    throw new Exception();
            }
        }

        virtual public ChessPiece GetPiece(Char? o)
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

        virtual public Char? GetPresentation(ChessPiece p)
        {
            if (p.IsEmpty)
                return null;

            return p.Player.Order == 1 ? GetWhitePieceSymbol(p) : GetBlackPieceSymbol(p);
        }

        public PieceTypes GetType(char? c)
        {
            throw new NotImplementedException();
        }
    }
}