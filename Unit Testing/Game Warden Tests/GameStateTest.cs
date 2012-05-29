using GameWarden;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTesting
{
    [TestClass]
    public class GameStateTest
    {
        [TestMethod]
        public void GetEnumeratorTest()
        {
            const int dim = 5;
            var s = new GameState(dim, dim);

            int file = 1;
            int rank = 5;
            foreach (var p in s)
            {
                Assert.AreEqual(file, p.Pos.File);
                Assert.AreEqual(rank, p.Pos.Rank);
                if (++file > dim) 
                { 
                    --rank;
                    file = 1;
                }
            }
        }
        
        [TestMethod]
        public void ItemTest()
        {
            var s = new GameState(5, 5);
            var pos = new Position(3, 3);
            Assert.IsNotNull(s[pos]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ItemTestOutOfRange()
        {
            var s = new GameState(5, 5);
            var pos = new Position(7, 7);
            var p = s[pos];
        }

        [TestMethod]
        public void NewEmptyPiece()
        {
            var s = new GameState(5, 5);
            var pos = new Position(3, 3);
            s[pos] = new Piece();

            s.NewEmptyPiece(pos);

            Assert.IsTrue(s[pos].IsEmpty);
        }
    }
}
