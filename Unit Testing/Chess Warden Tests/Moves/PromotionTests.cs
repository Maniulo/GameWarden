using GameWarden;
using GameWarden.Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class PromotionTests
    {
        [TestMethod]
        public void Promotion()
        {
            var from = new Position("a7");
            var s = new ChessState();
            var piece = new ChessPiece {Player = new Player(1)};
            s[from] = piece;
            var m = new Promotion(new PawnMoveTemplate(), new[] { PieceTypes.Knight, PieceTypes.Bishop, PieceTypes.Rook, PieceTypes.Queen });
            Assert.IsTrue(m.CanApply(from, new Position("a8"), s, PieceTypes.Queen));
        }

        [TestMethod]
        public void PromotionWrongType()
        {
            var from = new Position("a7");
            var s = new ChessState();
            var piece = new ChessPiece { Player = new Player(1) };
            s[from] = piece;
            var m = new Promotion(new PawnMoveTemplate(), new[] { PieceTypes.Knight, PieceTypes.Bishop, PieceTypes.Rook, PieceTypes.Queen });
            Assert.IsFalse(m.CanApply(from, new Position("a8"), s, PieceTypes.Pawn));
        }

        [TestMethod]
        public void PromotionBadPos()
        {
            var from = new Position("a6");
            var s = new ChessState();
            var piece = new ChessPiece { Player = new Player(1) };
            s[from] = piece;
            var m = new Promotion(new PawnMoveTemplate(), new[] { PieceTypes.Knight, PieceTypes.Bishop, PieceTypes.Rook, PieceTypes.Queen });
            Assert.IsFalse(m.CanApply(from, new Position("a7"), s, PieceTypes.Pawn));
        }

        [TestMethod]
        public void PromotionApply()
        {
            var from = new Position("a7");
            var to   = new Position("a8");
            var piece = new ChessPiece { Type = PieceTypes.Pawn, Player = new Player(1) };
            var s = new ChessState();
                s[from] = piece;
            var m = new Promotion(new PawnMoveTemplate(), new[] { PieceTypes.Knight, PieceTypes.Bishop, PieceTypes.Rook, PieceTypes.Queen });
            var concretePromotion = m.Concretize(from, to, PieceTypes.Queen);

            concretePromotion.Apply(s);

            Assert.AreEqual(PieceTypes.Queen, piece.Type);
        }

        [TestMethod]
        public void PromotionRollback()
        {
            var from = new Position("a7");
            var to = new Position("a8");
            var piece = new ChessPiece { Type = PieceTypes.Pawn, Player = new Player(1) };
            var s = new ChessState();
            s[from] = piece;
            var m = new Promotion(new PawnMoveTemplate(), new[] { PieceTypes.Knight, PieceTypes.Bishop, PieceTypes.Rook, PieceTypes.Queen });
            var concretePromotion = m.Concretize(from, to, PieceTypes.Queen);

            concretePromotion.Apply(s);
            concretePromotion.Rollback(s);

            Assert.AreEqual(PieceTypes.Pawn, piece.Type);
        }
    }
}
