using GameWarden;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class PieceTests
    {
        [TestMethod]
        public void MoveTest()
        {
            var p = new Piece();
            var pos = new Position(1, 1);
            p.Move(pos);

            Assert.AreEqual(1, p.PathLength);
            Assert.AreEqual(pos, p.Pos);
        }

        [TestMethod]
        public void UnmoveTest()
        {
            var to = new Position(1, 1);
            var p = new Piece();
            p.Move(to);
            p.Unmove();

            Assert.AreEqual(0, p.PathLength);
            Assert.AreEqual(null, p.Pos);
        }

        [TestMethod]
        public void UnmoveLong()
        {
            var to = new Position(1, 1);
            var p = new Piece();
            p.Move(to);
            p.Move(to);
            p.Move(to);
            p.Unmove();
            p.Unmove();
            p.Unmove();

            Assert.AreEqual(0, p.PathLength);
            Assert.AreEqual(null, p.Pos);
        }
    }
}
