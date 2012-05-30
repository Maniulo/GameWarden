using GameWarden;
using GameWarden.Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class PawnTests
    {
        [TestMethod]
        public void PawnUp()
        {
            var from = new Position("a2");
            var to = new Position("a3");
            var s = new ChessState();
            s[from] = new ChessPiece {Player = new Player(1)};
            var m = new PawnMoveTemplate();

            Assert.IsTrue(m.CanApply(from,to,s));
        }

        [TestMethod]
        public void PawnNotCapture()
        {
            var from = new Position("a2");
            var to = new Position("a3");
            var s = new ChessState();
            s[from] = new ChessPiece { Player = new Player(1) };
            s[to] = new ChessPiece();
            var m = new PawnMoveTemplate();

            Assert.IsFalse(m.CanApply(from, to, s));
        }

        [TestMethod]
        public void PawnDownWhite()
        {
            var from = new Position("a3");
            var to = new Position("a2");
            var s = new ChessState();
            s[from] = new ChessPiece { Player = new Player(1) };
            var m = new PawnMoveTemplate();

            Assert.IsFalse(m.CanApply(from, to, s));
        }

        [TestMethod]
        public void PawnDown()
        {
            var from = new Position("a3");
            var to = new Position("a2");
            var s = new ChessState();
            s[from] = new ChessPiece { Player = new Player(2) };
            var m = new PawnMoveTemplate();

            Assert.IsTrue(m.CanApply(from, to, s));
        }

        [TestMethod]
        public void PawnTwoCells()
        {
            var from = new Position("a2");
            var to = new Position("a4");
            var s = new ChessState();
            s[from] = new ChessPiece { Player = new Player(1) };
            var m = new PawnMoveTemplate();

            Assert.IsTrue(m.CanApply(from, to, s));
        }

        [TestMethod]
        public void PawnTwoCellsBlack()
        {
            var from = new Position("a7");
            var to = new Position("a5");
            var s = new ChessState();
            s[from] = new ChessPiece { Player = new Player(2) };
            var m = new PawnMoveTemplate();

            Assert.IsTrue(m.CanApply(from, to, s));
        }

        [TestMethod]
        public void PawnTwoCellsNot()
        {
            var from = new Position("a3");
            var to = new Position("a5");
            var s = new ChessState();
            s[from] = new ChessPiece { Player = new Player(1) };
            var m = new PawnMoveTemplate();

            Assert.IsFalse(m.CanApply(from, to, s));
        }

        [TestMethod]
        public void PawnCaptureRight()
        {
            var from = new Position("a2");
            var to = new Position("b3");
            var s = new ChessState();
            s[from] = new ChessPiece { Player = new Player(1) };
            s[to] = new ChessPiece();
            var m = new PawnCaptureTemplate();

            Assert.IsTrue(m.CanApply(from, to, s));
        }

        [TestMethod]
        public void PawnCaptureLeft()
        {
            var from = new Position("b3");
            var to = new Position("a4");
            var s = new ChessState();
            s[from] = new ChessPiece { Player = new Player(1) };
            s[to] = new ChessPiece();
            var m = new PawnCaptureTemplate();

            Assert.IsTrue(m.CanApply(from, to, s));
        }

        [TestMethod]
        public void EnPassant() //a2-a4+ep(a3)
        {
            var from = new Position("b4");
            var to = new Position("a3");
            var s = new ChessState();
            s.EnPassant = to;
            s[from] = new ChessPiece { Player = new Player(2) };
            var m = new EnPassantTemplate();

            Assert.IsTrue(m.CanApply(from, to, s));
        }

        [TestMethod]
        public void EnPassantFar() //a2-a4+ep
        {
            var from = new Position("c5");
            var to = new Position("a3");
            var s = new ChessState();
            s.EnPassant = to;
            s[from] = new ChessPiece { Player = new Player(2) };
            var m = new EnPassantTemplate();

            Assert.IsFalse(m.CanApply(from, to, s));
        }

        [TestMethod]
        public void EnPassantCapture() //a2-a4+ep
        {
            var enmy = new Position("a4");
            var from = new Position("b4");
            var to = new Position("a3");
            var s = new ChessState();
            s.EnPassant = to;
            s[from] = new ChessPiece { Player = new Player(2) };
            s[from].Move(from);
            s[enmy] = new ChessPiece();
            s[enmy].Move(enmy);
            var m = new EnPassantConcrete(from, to);

            Assert.IsFalse(s[enmy].IsEmpty);
            m.Apply(s);
            Assert.IsTrue(s[enmy].IsEmpty);
        }

        [TestMethod]
        public void EnPassantRollback() //a2-a4+ep
        {
            var enmy = new Position("a4");
            var from = new Position("b4");
            var to = new Position("a3");
            var s = new ChessState();
            s.EnPassant = to;
            s[from] = new ChessPiece { Player = new Player(2) };
            s[from].Move(from);
            s[enmy] = new ChessPiece();
            s[enmy].Move(enmy);
            var m = new EnPassantConcrete(from, to);

            m.Apply(s);
            m.Rollback(s);

            Assert.IsFalse(s[enmy].IsEmpty);
        }

        [TestMethod]
        public void EnPassantSetting()
        {
            var from = new Position("a2");
            var to = new Position("a4");
            var s = new ChessState();
            s[from] = new ChessPiece { Player = new Player(1) };
            var m = new PawnConcreteMove(from, to);

            m.Apply(s);

            Assert.AreEqual(new Position("a3"), s.EnPassant);
        }
    }
}
