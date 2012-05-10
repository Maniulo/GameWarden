using System;
using System.Text.RegularExpressions;

namespace GameWarden.Chess.Notations
{
    public interface IChessMoveNotation
    {
        ChessMove Parse(String record);
    }

    public class AlgebraicNotation : IChessMoveNotation
    {
        public Regex RxMove;
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
                            "(?<Promotion>=?[{0}])?" +
                            "(?<EnPassant>e.p.)?" +
                        ")" +
                    "|" + "(?<Kingside>O-O)" +
                    "|" + "(?<Queenside>O-O-O)" +
                    ")" +
                    "(?<Check>[+#])?" +
                ")$"
            , figureSymbols);

            RxMove = new Regex(rxMoveString);
        }

        /// <summary>
        /// Creates Standard Algebraic Notation parser.
        /// </summary>
        public AlgebraicNotation()
            : this(new EnglishPresentation()) { }

        public ChessMove Parse(String anRecord)
        {
            if (RxMove.IsMatch(anRecord))
            {
                var mv = RxMove.Match(anRecord);
                var m = new ChessMove(mv.Groups["Move"].Value);

                Boolean castlingKingside = mv.Groups["Kingside"].Success;
                Boolean castlingQueenside = mv.Groups["Queenside"].Success;

                if (castlingKingside || castlingQueenside)
                {
                    m.PieceType = PieceTypes.King;
                    m.CastlingKingside = castlingKingside;
                    m.CastlingQueenside = castlingQueenside;
                }
                else
                {
                    if (mv.Groups["Piece"].Success)
                        m.PieceType = Presentation.GetType(mv.Groups["Piece"].Value[0]);
                    else
                        m.PieceType = Presentation.GetType(null);

                    m.To = new Position(mv.Groups["ToFile"].Value + mv.Groups["ToRank"].Value);
                    m.From = new Position();

                    try { m.From.File = m.From.GetFile(mv.Groups["FromFile"].Value[0]); }
                    catch { }
                    try { m.From.Rank = m.From.GetRank(mv.Groups["FromRank"].Value[0]); }
                    catch { }
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