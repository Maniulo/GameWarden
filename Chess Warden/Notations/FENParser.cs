using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GameWarden.Chess.Notations
{
    public class FENParser
    {
        public const String DefaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"; // !!!

        readonly IPiecePresentation Presentation;
        Regex rxFEN;

        public FENParser(IPiecePresentation presentation)
        {
            Presentation = presentation;
            String figureSymbols = Presentation.ToString();
            String rxFENString = "^(?<Board>(?:(?:[" + figureSymbols + "|1-8]{1,8})/){7}(?:[" + figureSymbols + "|1-8]{1,8})) (?<Player>[wb]) (?<K>K)?(?<Q>Q)?(?<k>k)?(?<q>q)? (?<EnPassant>(?:-|[a-h][1-8]))" + @" (?<HalfMoves>\d+) (?<FullMoves>\d+)$";
            rxFEN = new Regex(rxFENString, RegexOptions.None);
        }

        public FENParser()
            : this(new EnglishFENPresentation()) { }

        public ChessState Parse(String fenRecord, List<Player> players = null)
        {
            var gs = new ChessState();
            if (players == null)
                players = new List<Player> {new Player(1), new Player(2)};

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

                int file = 1, rank = 8;
                var factory = new ChessPieceFactory();
                
                foreach (Char? ch in ParseBoard(m.Groups["Board"].Value))
                {
                    IPiece p;
                    if (ch.HasValue)
                    {
                        Piece piece = factory.CreatePiece(ch, Presentation);    // !!!
                        p = piece;
                        piece.Player = players[piece.Player.Order - 1];
                    }
                    else
                        p = new ChessPiece { IsEmpty = true };

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

            result.Remove(result.Length - 1, 1);

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
}