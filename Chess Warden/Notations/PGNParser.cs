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
        readonly Regex RxFullMove = new Regex(
            @"(?<FullMove>" +
            @"(?<Number>\d+)\. (?<Move1>[^ {}]*)" +
            @"((?: \{(?<Comment1>[^\}]*)\} \k<Number>\.\.\.)?" +
            @" (?<Move2>[^ {}]*))?" +
            @"(?: \{(?<Comment2>[^\}]*)\})?" +
            @") ?" + @"|(?<Number>\d+)\.\.\. (?<Move2>[^ {}]*)(?: \{(?<Comment2>[^\}]*)\})? ?"
            );

        Meta ParseTags(IEnumerator<string> lines)
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
        
        public KeyValuePair<string, string> ParseTag(String line)
        {
            if (RxTag.IsMatch(line))
            {
                var m = RxTag.Match(line);
                return new KeyValuePair<String, String>(m.Groups["Tag"].Value, m.Groups["Value"].Value);
            }
            else
            {
                throw new ArgumentException(String.Format("\"{0}\" is not a valid tag string.", line));
            }
        }

        protected static String ParseMovetext(IEnumerator<String> lines)
        {
            var movetext = new StringBuilder();

            while (lines.MoveNext())
                movetext.Append(lines.Current + " ");
            movetext.Remove(movetext.Length - 1, 1);

            return movetext.ToString();
        }

        public ChessGame Parse(IEnumerable<String> pgn, IChessMoveNotation moveNotation)
        {
            var lines = pgn.GetEnumerator();
            var metainfo = ParseTags(lines);
            var movetext = ParseMovetext(lines);

            var game = new ChessGame(metainfo);
            game.Moves = ParseMoves(movetext, moveNotation, game.Players).ToList();
            
            return game;
        }

        public IEnumerable<ChessMove> ParseMoves(String movetext, IChessMoveNotation moveNotation, IEnumerable<Player> players = null)
        {
            if (players == null)
                players = new List<Player>() { new Player(1), new Player(2) };
            
            var playersList = players.ToList();

            foreach (Match m in RxFullMove.Matches(movetext))
            {
                ChessMove move;

                if (m.Groups["Move1"].Captures.Count > 0 && m.Groups["Move1"].Value != "")
                {
                    move = moveNotation.Parse(m.Groups["Move1"].Value);
                    move.Player = playersList[0];
                    yield return move;
                }

                if (m.Groups["Move2"].Captures.Count > 0 && m.Groups["Move2"].Value != "")
                {
                    move = moveNotation.Parse(m.Groups["Move2"].Value);
                    move.Player = playersList[1];
                    yield return move;
                }
            }
        }
    }
}