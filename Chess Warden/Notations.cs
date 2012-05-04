using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GameWarden.Chess.Notations
{

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
            var p = new ChessPiece();
            p.Type = GetType(c);
            return p;
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

    public interface IPiecePresentation
    {
        Char? GetPresentation(ChessPiece p);
        ChessPiece GetPiece(char? c);
        PieceTypes GetType(char? c);
    }

    public class FEN
    {
        public const String DefaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"; // !!!

        readonly IPiecePresentation Presentation;
        Regex rxFEN;

        public FEN(IPiecePresentation presentation)
        {
            Presentation = presentation;
            String figureSymbols = Presentation.ToString();
            String rxFENString = "^(?<Board>(?:(?:[" + figureSymbols + "|1-8]{1,8})/){7}(?:[" + figureSymbols + "|1-8]{1,8})) (?<Player>[wb]) (?<K>K)?(?<Q>Q)?(?<k>k)?(?<q>q)? (?<EnPassant>(?:-|[a-h][1-8]))" + @" (?<HalfMoves>\d+) (?<FullMoves>\d+)$";
            rxFEN = new Regex(rxFENString, RegexOptions.None);
        }

        public FEN()
            : this(new EnglishFENPresentation()) { }

        public ChessState Parse(String fenRecord, List<Player> players = null)
        {
            ChessState gs = new ChessState();
            if (players == null)
                players = new List<Player>() {new Player(1), new Player(2)};

            if (rxFEN.IsMatch(fenRecord))
            {
                Match m = rxFEN.Match(fenRecord);
                gs.Player = m.Groups["Player"].Value;

                gs.CastlingKingsideWhite = m.Groups["K"].Success;
                gs.CastlingQueensideWhite = m.Groups["Q"].Success;
                gs.CastlingKingsideBlack = m.Groups["k"].Success;
                gs.CastlingQueensideBlack = m.Groups["q"].Success;

                if (m.Groups["EnPassant"].Value != "-")
                    gs.EnPassant = m.Groups["EnPassant"].Value;
                gs.HalfMoves = m.Groups["HalfMoves"].Value;
                gs.FullMoves = m.Groups["FullMoves"].Value;

                int file = 1;
                int rank = 8;
                var factory = new ChessPieceFactory();
                
                foreach (Char? ch in ParseBoard(m.Groups["Board"].Value))
                {
                    IPiece p;
                    if (ch.HasValue)
                    {
                        Piece piece = factory.CreatePiece(ch, new EnglishFENPresentation());    // !!!
                        p = piece;
                        piece.Player = players[piece.Player.Order - 1];

                    }
                    else
                        p = new EmptySquare();

                    gs.PlacePiece(new Position(file, rank), p);

                    if (++file > 8)
                    {
                        --rank;
                        file = 1;
                    }                    
                }

                return gs;
            }
            else
            {
                throw new ArgumentException(String.Format("\"{0}\" is not a valid FEN string.", fenRecord));
            }
        }

        private IEnumerable<Char?> ParseBoard(String boardString)
        {
            foreach (Char c in boardString)
            {
                if (Char.IsDigit(c))
                {
                    int emptySpaces = Int32.Parse(c.ToString());
                    while (emptySpaces-- > 0)
                        yield return null;
                }
                else if (c == '/')
                {
                    continue;
                }
                else
                {
                    yield return c;
                }
            }            
        }

        private String GenerateBoard(ChessState gameState)
        {
            var result = new StringBuilder();

            int emptySpaces = 0;
            int line = 0;

            foreach (IPiece p in gameState)
            {
                if (p.IsEmpty)
                    ++emptySpaces;
                else
                {
                    AddEmptySpaces(result, ref emptySpaces);
                    result.Append(Presentation.GetPresentation((ChessPiece)p).ToString());
                }

                if (++line == 8)
                {
                    AddEmptySpaces(result, ref emptySpaces);
                    result.Append("/");
                    line = 0;
                }
            }

            result.Remove(result.Length-1, 1);

            return result.ToString();
        }

        private static void AddEmptySpaces(StringBuilder result, ref int emptySpaces)
        {
            if (emptySpaces > 0)
                result.Append(emptySpaces.ToString());
            emptySpaces = 0;
        }

        public String Generate(ChessState gameState)
        {
            return String.Format("{0} {1} {2} {3} {4} {5}",
                                                GenerateBoard(gameState), gameState.Player,
                                                (gameState.CastlingKingsideWhite ? "K" : "") + 
                                                (gameState.CastlingQueensideWhite ? "Q" : "") + 
                                                (gameState.CastlingKingsideBlack ? "k" : "") + 
                                                (gameState.CastlingQueensideBlack ? "q" : ""),
                                                gameState.EnPassant == null ? "-" : gameState.EnPassant.ToString(),
                                                gameState.HalfMoves,
                                                gameState.FullMoves);
        }
    }

    public class PGN
    {
        Regex rxTag = new Regex(@"\[(?<Tag>\w+) ""(?<Value>.*)""\]");
        readonly Regex rxFullMove = new Regex(
                @"(?<FullMove>" +
                    @"(?<Number>\d+)\. (?<Move1>[^ {}]*)" +
                    @"((?: \{(?<Comment1>[^\}]*)\} \k<Number>\.\.\.)?" +
                    @" (?<Move2>[^ {}]*))?" +
                    @"(?: \{(?<Comment2>[^\}]*)\})?" +
                @") ?" + @"|(?<Number>\d+)\.\.\. (?<Move2>[^ {}]*)(?: \{(?<Comment2>[^\}]*)\})? ?"
                );

        Meta ParseTags(IEnumerator<String> lines)
        {
            Meta metainfo = new Meta();
            while (lines.MoveNext())
            {
                try
                {
                    var pair = ParseTag(lines.Current);
                    metainfo[pair.Key] = pair.Value;
                }
                catch
                {
                    break;
                }
            }

            return metainfo;
        }
        
        public KeyValuePair<String, String> ParseTag(String line)
        {
            if (rxTag.IsMatch(line))
            {
                Match m = rxTag.Match(line);
                return new KeyValuePair<String, String>(m.Groups["Tag"].Value, m.Groups["Value"].Value);
            }
            else
            {
                throw new ArgumentException(String.Format("\"{0}\" is not a valid tag string.", line));
            }
        }
        
        String ParseMovetext(IEnumerator<String> lines)
        {
            var movetext = new StringBuilder();

            while (lines.MoveNext())
                movetext.Append(lines.Current + " ");
            movetext.Remove(movetext.Length - 1, 1);

            return movetext.ToString();
        }

        public ChessGame Parse(IEnumerable<String> pgn, IMoveNotation moveNotation)
        {
            var lines = pgn.GetEnumerator();
            var metainfo = ParseTags(lines);
            var movetext = ParseMovetext(lines);

            var game = new ChessGame(metainfo);
            game.Moves = ParseMoves(movetext, moveNotation, game.Players).ToList();
            
            return game;
        }

        public IEnumerable<concreteMove> ParseMoves(String movetext, IMoveNotation moveNotation, IEnumerable<Player> players = null)
        {
            if (players == null)
                players = new List<Player>() { new Player(1), new Player(2) };
            
            var playersList = players.ToList();

            foreach (Match m in rxFullMove.Matches(movetext))
            {
                UnsolvedMove move;

                if (m.Groups["Move1"].Captures.Count > 0 && m.Groups["Move1"].Value != "")
                {
                    move = (UnsolvedMove)moveNotation.Parse(m.Groups["Move1"].Value);
                    move.Player = playersList[0];
                    yield return move;
                }

                if (m.Groups["Move2"].Captures.Count > 0 && m.Groups["Move2"].Value != "")
                {
                    move = (UnsolvedMove)moveNotation.Parse(m.Groups["Move2"].Value);
                    move.Player = playersList[1];
                    yield return move;
                }
            }
        }
    }

    public interface IMoveNotation
    {
        concreteMove Parse(String record);
    }

    public class AlgebraicNotation : IMoveNotation
    {
        public Regex rxMove;
        readonly IPiecePresentation Presentation;

        public AlgebraicNotation(IPiecePresentation presentation)
        {
            Presentation = presentation;
            String figureSymbols = Presentation.ToString();
        
            var rxMoveString = String.Format(
                "^(?:" +
                    "(?<Move>" +
                        "(?:" +
                            "(?<Piece>[{0}])?" +
                            "(?<FromFile>[a-h])?" + "(?<FromRank>[1-8])?" +
                            "(?<Capture>x)?" +
                            "(?<ToFile>[a-h])(?<ToRank>[1-8])" +
                            "(?<Promotion>[{0}])?" +
                            "(?<EnPassant>e.p.)?" +
                        ")" +
                    "|" + "(?<Kingside>O-O)" +
                    "|" + "(?<Queenside>O-O-O)" +
                    ")" +
                    "(?<Check>[+#])?" +
                ")$"
            , figureSymbols);

            rxMove = new Regex(rxMoveString);
        }

        /// <summary>
        /// Creates Standard Algebraic Notation parser.
        /// </summary>
        public AlgebraicNotation()
            : this( new EnglishPresentation() ) { }

        public concreteMove Parse(String anRecord)
        {
            if (rxMove.IsMatch(anRecord))
            {
                var mv = rxMove.Match(anRecord);
                var m = new UnsolvedMove(mv.Groups["Move"].Value);

                Boolean CastlingKingside = mv.Groups["Kingside"].Success;
                Boolean CastlingQueenside = mv.Groups["Queenside"].Success;

                if (CastlingKingside || CastlingQueenside)
                {
                    m.PieceType = PieceTypes.King; // !!!

                    m.From = new Position(5, null);
                    
                    if (CastlingKingside)
                        m.To = new Position(7, null);

                    if (CastlingQueenside)
                        m.To = new Position(3, null);
                }
                else
                {
                    if (mv.Groups["Piece"].Success)
                        m.PieceType = Presentation.GetType(mv.Groups["Piece"].Value[0]); // !!! to char?
                    else
                        m.PieceType = Presentation.GetType(null);

                    m.To   = new Position(mv.Groups["ToFile"].Value + mv.Groups["ToRank"].Value);
                    m.From = new Position();

                    try { m.From.File = Position.GetFile(mv.Groups["FromFile"].Value[0]); } catch { }
                    try { m.From.Rank = Position.GetRank(mv.Groups["FromRank"].Value[0]); } catch { }
                }

                return m;
            }
            else
            {
                throw new ArgumentException(String.Format("\"{0}\" is not a valid AN string.", anRecord));
            }

        }
    }

}
