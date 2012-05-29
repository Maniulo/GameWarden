using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace GameWarden.Chess.Notations
{
    public class FENParser
    {
        public const String DefaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"; // !!!

        // static due to optimization
        private static readonly IChessPresentation Presentation = new EnglishFENPresentation();
        private static readonly Regex RxFEN;

        static FENParser()
        {
            String figureSymbols = Presentation.ToString();
            String rxFENString = "^(?<Board>(?:(?:[" + figureSymbols + "|1-8]{1,8})/){7}(?:[" + figureSymbols + "|1-8]{1,8})) (?<Player>[wb]) (?<K>K)?(?<Q>Q)?(?<k>k)?(?<q>q)? (?<EnPassant>(?:-|[a-h][1-8]))" + @" (?<HalfMoves>\d+) (?<FullMoves>\d+)$";
            RxFEN = new Regex(rxFENString, RegexOptions.None);
        }

        public ChessState Parse(String fenRecord, List<Player> players = null)
        {
            if (players == null)
                players = new List<Player> { new ChessPlayer(1), new ChessPlayer(2) };

            if (RxFEN.IsMatch(fenRecord))
            {
                Match m = RxFEN.Match(fenRecord);

                var gs = ParseBoard(m.Groups["Board"].Value, players);
                
                gs.Player = m.Groups["Player"].Value[0];
                
                gs.Castling.KingsideWhite = m.Groups["K"].Success;
                gs.Castling.QueensideWhite = m.Groups["Q"].Success;
                gs.Castling.KingsideBlack = m.Groups["k"].Success;
                gs.Castling.QueensideBlack = m.Groups["q"].Success;

                if (m.Groups["EnPassant"].Value != "-")
                    gs.EnPassant = m.Groups["EnPassant"].Value;

                gs.HalfMoves = Int32.Parse(m.Groups["HalfMoves"].Value);
                gs.FullMoves = Int32.Parse(m.Groups["FullMoves"].Value);

                return gs;
            }
            else
            {
                throw new ArgumentException(String.Format("\"{0}\" is not a valid FEN string.", fenRecord));
            }
        }

        public ChessState ParseBoard(String s, List<Player> players = null)
        {
            if (players == null)
                players = new List<Player> { new ChessPlayer(1), new ChessPlayer(2) };

            var gs = new ChessState();
            int file = 1, rank = 8;
            foreach (Char? ch in GetBoardChars(s))
            {
                IPiece p = ChessPieceFactory.CreatePiece(ch, Presentation, players);

                gs.PlacePiece(new Position(file, rank), p);

                if (++file > 8)
                {
                    --rank;
                    file = 1;
                }
            }

            return gs;
        }

        private IEnumerable<Char?> GetBoardChars(String boardString)
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

        public static String Generate(ChessState gameState)
        {
            return String.Format("{0} {1} {2} {3} {4} {5}",
                                 GenerateBoard(gameState), gameState.Player,
                                 (gameState.Castling.KingsideWhite ? "K" : "") +
                                 (gameState.Castling.QueensideWhite ? "Q" : "") +
                                 (gameState.Castling.KingsideBlack ? "k" : "") +
                                 (gameState.Castling.QueensideBlack ? "q" : ""),
                                 gameState.EnPassant == null ? "-" : gameState.EnPassant.ToString(),
                                 gameState.HalfMoves,
                                 gameState.FullMoves);
        }

        public static String GenerateBoard(ChessState gameState)
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
                result.Append(emptySpaces);
            emptySpaces = 0;
        }
    }
}