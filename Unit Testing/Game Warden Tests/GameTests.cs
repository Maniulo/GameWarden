using GameWarden;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class GameTests
    {
        class MockGame : Game
        {
            public MockGame()
            {
                State = new GameState(5, 5);
            }
        }


        [TestMethod]
        public void AddMove()
        {
            var g    = new MockGame();
            var from = new Position(1, 1);
            var to   = new Position(2, 2);
            var m    = new ConcreteMove(from, to);

            g.AddMove(m);

            Assert.AreEqual(1, g.MovesCount);
        }

        [TestMethod]
        public void MakeNoMove()
        {
            var g = new MockGame();

            bool result = g.MakeMove();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MakeMove()
        {
            var g = new MockGame();
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            var p  = new Piece();
            var m = new ConcreteMove(from, to);
            g.State[from] = p;
            p.Move(from);
            g.AddMove(m);

            bool result = g.MakeMove();

            Assert.AreEqual(1, g.MovesCount);
            Assert.AreEqual(1, g.CurrentMove);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MakeMoveIsStateChanged()
        {
            var g = new MockGame();
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            var p = new Piece();
            var m = new ConcreteMove(from, to);
            g.State[from] = p;
            p.Move(from);
            g.AddMove(m);

            g.MakeMove();

            Assert.AreEqual(p, g.State[to]);
        }
    }
}
