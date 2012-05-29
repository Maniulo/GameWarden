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
        readonly IChessPieceTypePresentation Presentation;

        public AlgebraicNotation(IChessPieceTypePresentation presentation)
        {
            Presentation = presentation;
            String figureSymbols = Presentation.ToString();

            var rxMoveString = String.Format(
                "^(?:" +
                    "(?<Move>(?:" +
                        "(?:" +
                            "(?<Piece>[{0}])?" +
                            "(?<FromFile>[a-h])?" + "(?<FromRank>[1-8])?" +
                            "(?<Capture>x)?" +
                            "(?<ToFile>[a-h])(?<ToRank>[1-8])" +
                            "(?:=?(?<Promotion>[{0}]))?" +
                            "(?<EnPassant>e.p.)?" +
                        ")" +
                    "|" + "(?<Kingside>O-O)" +
                    "|" + "(?<Queenside>O-O-O)" +
                    ")" +
                    "(?<Check>[+#])?)" +
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
            var mv = RxMove.Match(anRecord);
            if (mv.Success)
            {
                var m = new ChessMove(mv.Groups["Move"].Value);

                var castlingKingside = mv.Groups["Kingside"].Success;
                var castlingQueenside = mv.Groups["Queenside"].Success;
                var promotion = mv.Groups["Promotion"].Success;

                m.IsPromotion = promotion;
                if (promotion)
                    m.PromotionTo = Presentation.GetPieceType(mv.Groups["Promotion"].Value);

                if (castlingKingside || castlingQueenside)
                {
                    m.PieceType = PieceTypes.King;
                    m.CastlingKingside = castlingKingside;
                    m.CastlingQueenside = castlingQueenside;
                }
                else
                {
                    if (mv.Groups["Piece"].Success)
                        m.PieceType = Presentation.GetPieceType(mv.Groups["Piece"].Value[0]);
                    else
                        m.PieceType = Presentation.GetPieceType(null);

                    m.To = new Position(mv.Groups["ToFile"].Value + mv.Groups["ToRank"].Value);
                    m.From = new Position();

                    int? file = null;
                    int? rank = null;

                    if (mv.Groups["FromFile"].Success)
                        file = Position.GetFile(mv.Groups["FromFile"].Value[0]);

                    if (mv.Groups["FromRank"].Success)
                        rank = Position.GetRank(mv.Groups["FromRank"].Value[0]);

                    m.From = new Position(file, rank);
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