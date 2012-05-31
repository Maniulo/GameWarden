using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GameWarden.Chess.Notations
{
    public class PGNParser
    {
        readonly Regex RxTag = new Regex(@"\[(?<Tag>\w+) ""(?<Value>.*)""\]");

        private readonly Regex RxFullMove = new Regex(
            @"(?<Number>\d+)\.\.\. (?<Move2>[^ {}\.]*)(?: \{(?<Comment2>[^\}]*)\})? ?|" +
            @"(?<FullMove>" +
            @"(?<Number>\d+)\. ?(?<Move1>[^ {}\.]*)" +
            @"((?: \{(?<Comment1>[^\}]*)\} \k<Number>\.\.\.)?" +
            @" ?(?<Move2>[^ {}]*))?" +
            @"(?: \{(?<Comment2>[^\}]*)\})?" +
            @") ?"
            );

        private Meta ParseTags(IEnumerator<String> lines)
        {
            var metainfo = new Meta();
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

        private KeyValuePair<string, string> ParseTag(String line)
        {
            if (RxTag.IsMatch(line))
            {
                var m = RxTag.Match(line);
                return new KeyValuePair<String, String>(m.Groups["Tag"].Value, m.Groups["Value"].Value);
            }

            throw new ArgumentException(String.Format("\"{0}\" is not a valid tag string.", line));
        }

        protected static String ParseMovetext(IEnumerator<String> lines)
        {
            var movetext = new StringBuilder();

            try {
                // remove empty lines
                while (lines.Current.Trim() == "")
                    lines.MoveNext();

                do
                {
                    movetext.Append(lines.Current + " ");
                } while (lines.MoveNext());

                // remove trailing space
                movetext.Remove(movetext.Length - 1, 1);

                if (movetext[movetext.Length - 1] == '*')
                    movetext.Remove(movetext.Length - 1, 1);

                return movetext.ToString().TrimEnd(new char[] {'*', '1', '0', '-', ' '});
            } catch { }

            return "";
        }

        public ChessGame Parse(IEnumerable<String> pgn)
        {
            return Parse(pgn, new AlgebraicNotation());
        }
            
        public ChessGame Parse(IEnumerable<String> pgn, IChessMoveNotation moveNotation)
        {
            var lines = pgn.GetEnumerator();
            var metainfo = ParseTags(lines);
            var movetext = ParseMovetext(lines);
            var game = new ChessGame(metainfo);

            foreach (ChessMove cm in ParseMoves(movetext, moveNotation, game.Players))
                game.AddMove(cm);
            
            return game;
        }

        public IEnumerable<ChessMove> ParseMoves(String movetext, List<Player> players = null)
        {
            return ParseMoves(movetext, new AlgebraicNotation(), players);
        }

        public IEnumerable<ChessMove> ParseMoves(String movetext, IChessMoveNotation moveNotation, List<Player> players = null)
        {
            if (players == null)
                players = new List<Player> { new Player(1), new Player(2) };

            foreach (Match m in RxFullMove.Matches(movetext))
            {
                ChessMove move;

                move = TryGetMove(moveNotation, m.Groups["Move1"], players[0]);
                if (move != null) yield return move;

                move = TryGetMove(moveNotation, m.Groups["Move2"], players[1]);
                if (move != null) yield return move;
            }
        }

        private static ChessMove TryGetMove(IChessMoveNotation moveNotation, Group g, Player p)
        {
            if (g.Captures.Count > 0 && g.Value != "")
            {
                ChessMove move = moveNotation.Parse(g.Value);
                move.Player = p;
                return move;
            }

            return null;
        }

        public List<String> Generate(ChessGame game)
        {
            var result = game.Info.Select(tag => "[" + tag.Key + " \"" + tag.Value + "\"]").ToList();
            result.Add("");
            result.Add(GenerateMovetext(game));
            return result;
        }

        public String GenerateMovetext(ChessGame game)
        {
            var movetext = new StringBuilder();
            var moveCount = 1;

            foreach (var m in game.Moves)
            {
                if (moveCount % 2 == 1)
                    movetext.Append((moveCount+1)/2 + ". ");
                movetext.Append(m + " ");
                
                moveCount++;
            }

            if (movetext.Length > 0)
                movetext.Remove(movetext.Length - 1, 1);

            return movetext.ToString();
        }
    }
}