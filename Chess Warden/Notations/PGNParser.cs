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
            @" (?<Move2>[^ {}]*))?" +
            @"(?: \{(?<Comment2>[^\}]*)\})?" +
            @") ?"
            );

        Meta ParseTags(IEnumerator<String> lines)
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

        public List<String> Generate(ChessGame game)
        {
            var result = game.Info.Select(tag => "[" + tag.Key + " \"" + tag.Value + "\"]").ToList();

            result.Add("");

            // AlgebraicNotation AN = new AlgebraicNotation();
            int i = 1;
            var movetext = new StringBuilder();
            var mover = game.Moves.GetEnumerator();

            while (mover.MoveNext())
            {
                movetext.Append((i++) + ". ");
                movetext.Append(mover.Current + " ");
                if (mover.MoveNext())
                    movetext.Append(mover.Current + " ");
            }

            movetext.Remove(movetext.Length - 1, 1);
            result.Add(movetext.ToString());

            return result;
        }
    }
}