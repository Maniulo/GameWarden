using System;

namespace GameWarden.Chess.Notations
{
    public interface IChessPieceTypePresentation
    {
        PieceTypes GetPieceType(Object c);
    }

    public interface IChessPiecePlayerPresentation
    {
        int GetPlayer(Object c);
    }

    public interface IChessPresentation : IChessPieceTypePresentation, IChessPiecePlayerPresentation
    {
        bool IsEmpty(Object c);
    }

    public class EnglishPresentation : IChessPieceTypePresentation
    {
        virtual public Object GetPresentation(IPiece p)
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
        virtual public PieceTypes GetPieceType(Object c)
        {
            char ch = ' ';

            if (c == null)
                return PieceTypes.Pawn;

            if (c is String)
                ch = ((String)c)[0];

            if (c is char)
                ch = Char.ToUpper((char)c);

            switch (ch)
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

        public override string ToString()
        {
            return "NBRQK";
        }
    }

    public class EnglishFENPresentation : IChessPresentation
    {
        readonly EnglishPresentation Template = new EnglishPresentation();

        public Object GetPresentation(IPiece p)
        {
            var piece = p as ChessPiece;

            if (piece == null)
                throw new ArgumentException();

            var c = (piece.Type == PieceTypes.Pawn) ? 'p' : ((char?)Template.GetPresentation(piece)).Value;

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

        virtual public bool IsEmpty(Object c)
        {
            return c == null;
        }
        virtual public PieceTypes GetPieceType(Object c)
        {
            char ch = ' ';
            
            if (c is String)
                ch = ((String)c)[0];

            if (c is char?)
                ch = Char.ToUpper(((char?)c).Value);

            if (c is char)
                ch = Char.ToUpper((char)c);
            
            return ch == 'P' ? PieceTypes.Pawn : Template.GetPieceType(ch);
        }
        virtual public int GetPlayer(Object o)
        {
            return Char.IsUpper(((char?) o).Value) ? 1 : 2;
        }
        
        public override string ToString()
        {
            return "PNBRQKpnbrqk";
        }
    }

    public class FigurinePresentation : IChessPresentation
    {
        private Char GetWhitePieceSymbol(ChessPiece p)
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
            }

            throw new ArgumentException();
        }
        private Char GetBlackPieceSymbol(ChessPiece p)
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
            }

            throw new ArgumentException();
        }
        virtual public Object GetPresentation(IPiece p)
        {
            if (p.IsEmpty)
                return null;

            var piece = p as ChessPiece;
            if (piece != null)
                return piece.Player.Order == 1 ? GetWhitePieceSymbol(piece) : GetBlackPieceSymbol(piece);

            throw new ArgumentException();
        }

        virtual public bool IsEmpty(Object c)
        {
            return c == null;
        }
        virtual public PieceTypes GetPieceType(Object c)
        {
            switch (c as Char?)
            {
                case '♟':
                case '♙':
                    return PieceTypes.Pawn;
                case '♞':
                case '♘':
                    return PieceTypes.Knight;
                case '♝':
                case '♗':
                    return PieceTypes.Bishop;
                case '♜':
                case '♖':
                    return PieceTypes.Rook;
                case '♛':
                case '♕':
                    return PieceTypes.Queen;
                case '♚':
                case '♔':
                    return PieceTypes.King;
            }

            throw new ArgumentException();
        }
        virtual public int GetPlayer(Object o)
        {
            try
            {
                switch ((Char?) o)
                {
                    case '♙':
                    case '♘':
                    case '♗':
                    case '♖':
                    case '♕':
                    case '♔':
                        return 1;
                    case '♟':
                    case '♞':
                    case '♝':
                    case '♜':
                    case '♛':
                    case '♚':
                        return 2;
                }
            } catch { }

            throw new ArgumentException();
        }
        
        public override string ToString()
        {
            return "♙♘♗♖♕♔♟♞♝♜♛♚";
        }
    }
}
