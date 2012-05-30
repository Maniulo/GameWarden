using GameWarden;
using GameWarden.Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class KnightTests
    {
        [TestMethod]
        public void KnightUpRight()
        {
            var from = new Position(3, 3);
            var s = new ChessState();
            var m = new KnightMoveTemplate();
            Assert.IsTrue(m.CanApply(from, new Position(4, 5), s));
            Assert.IsTrue(m.CanApply(from, new Position(5, 4), s));
        }

        [TestMethod]
        public void KnightUpLeft()
        {
            var from = new Position(3, 3);
            var s = new ChessState();
            var m = new KnightMoveTemplate();
            Assert.IsTrue(m.CanApply(from, new Position(2, 5), s));
            Assert.IsTrue(m.CanApply(from, new Position(1, 4), s));
        }

        [TestMethod]
        public void KnightDownRight()
        {
            var from = new Position(3, 3);
            var s = new ChessState();
            var m = new KnightMoveTemplate();
            Assert.IsTrue(m.CanApply(from, new Position(4, 1), s));
            Assert.IsTrue(m.CanApply(from, new Position(5, 2), s));
        }

        [TestMethod]
        public void KnightDownLeft()
        {
            var from = new Position(3, 3);
            var s = new ChessState();
            var m = new KnightMoveTemplate();
            Assert.IsTrue(m.CanApply(from, new Position(2, 1), s));
            Assert.IsTrue(m.CanApply(from, new Position(1, 2), s));
        }

        [TestMethod]
        public void KnightNot()
        {
            var from = new Position(3, 3);
            var s = new ChessState();
            var m = new KnightMoveTemplate();
            Assert.IsFalse(m.CanApply(from, new Position(2, 2), s));
        }

        [TestMethod]
        public void KnightCanSkip()
        {
            var from = new Position(3, 3);
            var s = new ChessState();
            s[new Position(3, 4)] = new ChessPiece();
            var m = new KnightMoveTemplate();
            Assert.IsTrue(m.CanApply(from, new Position(4, 5), s));
        }
    }
}
