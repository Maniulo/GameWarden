using System;
using GameWarden;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class BasicConcreteMovesTests
    {
        [TestMethod]
        public void SimpleMove()
        {
            var s = new GameState(5, 5);
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            s[from] = new Piece();
            var movingPiece = s[from];
            var m = new ConcreteMove(from, to);

            m.Apply(s);

            Assert.IsTrue(s[from].IsEmpty);
            Assert.AreNotEqual(s[from], movingPiece);
            Assert.AreEqual(s[to], movingPiece);
        }

        [TestMethod]
        public void CaptureMove()
        {
            var s = new GameState(5, 5);
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            s[from] = new Piece();
            var movingPiece = s[from];
            s[to] = new Piece();
            var m = new ConcreteMove(from, to);
            
            m.Apply(s);

            Assert.AreEqual(s[to], movingPiece);
        }

        [TestMethod]
        public void Rollback()
        {
            var s = new GameState(5, 5);
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            s[from] = new Piece();
            s[from].Move(from);
            var movingPiece = s[from];
            var m = new ConcreteMove(from, to);

            m.Apply(s);
            m.Rollback(s);

            Assert.AreEqual(s[from], movingPiece);
            Assert.AreNotEqual(s[to], movingPiece);
        }

        [TestMethod]
        public void RollbackCapture()
        {
            var s = new GameState(5, 5);
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            s[from] = new Piece();
            s[from].Move(from);
            var movingPiece = s[from];
            var capturedPiece = s[to];
            var m = new ConcreteMove(from, to);

            m.Apply(s);
            m.Rollback(s);

            Assert.AreEqual(s[from], movingPiece);
            Assert.AreEqual(s[to], capturedPiece);
        }

        [TestMethod]
        public void GetConcreteFromTemplate()
        {
            var s = new GameState(5, 5);
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            s[from] = new Piece();
            var movingPiece = s[from];
            var m = new MockTemplate().Concretize(from, to);

            m.Apply(s);

            Assert.IsTrue(s[from].IsEmpty);
            Assert.AreNotEqual(s[from], movingPiece);
            Assert.AreEqual(s[to], movingPiece);
        }
    }
}
