using System;
using GameWarden;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace UnitTesting
{
    [TestClass]
    public class PositionTests
    {
        [TestMethod]
        public void PositionFromString()
        {
            var p = new Position("C7");
            Assert.AreEqual(3, p.File);
            Assert.AreEqual(7, p.Rank);
        }

        [TestMethod]
        public void GetFileLetter()
        {
            Assert.AreEqual(3, Position.GetFile('c'));
        }

        [TestMethod]
        public void GetFileLetterCapital()
        {
            Assert.AreEqual(3, Position.GetFile('c'));
        }

        [TestMethod]
        public void GetFileLetterAdvanced()
        {
            Assert.AreEqual(9, Position.GetFile('i'));
        }

        [TestMethod]
        public void GetRankNumber()
        {
            Assert.AreEqual(4, Position.GetRank('4'));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetFileLetterOutOfRange()
        {
            Position.GetFile('*');
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GetRankNumberOutOfRange()
        {
            Position.GetRank('z');
        }

        [TestMethod]
        public void FileDistance()
        {
            Assert.AreEqual(7, Position.FileDistance(new Position(1, null), new Position(8, null)));
        }

        [TestMethod]
        public void RankDistance()
        {
            Assert.AreEqual(4, Position.RankDistance(new Position(null, 3), new Position(null, 7)));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileDistanceNull()
        {
            Position.FileDistance(new Position(), new Position());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RankDistanceNull()
        {
            Position.RankDistance(new Position(1), new Position(2));
        }

        [TestMethod]
        public void ToStringMethod()
        {
            var p = new Position(2, 5);
            Assert.AreEqual("b5", p.ToString());
        }

        [TestMethod]
        public void Equality()
        {
            Assert.AreEqual(new Position(1, 1), new Position(1, 1));
        }

        [TestMethod]
        public void EqualityIn()
        {
            Assert.AreEqual(new Position(null, 1), new Position(1, 1));
        }

        [TestMethod]
        public void EqualityLine()
        {
            Assert.AreEqual(new Position(null, 1), new Position(null, 1));
        }

        [TestMethod]
        public void EqualityNotIn()
        {
            Assert.AreNotEqual(new Position(null, 1), new Position(1, 2));
        }

        [TestMethod]
        public void EqualityMixed()
        {
            Assert.AreEqual(new Position(null, 1), new Position(1, null));
        }
    }
}
