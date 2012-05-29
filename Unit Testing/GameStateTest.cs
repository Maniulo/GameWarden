using GameWarden;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Unit_Testing
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
        public void PlacePieceTest()
        {
            var s = new GameState(5, 5);
            var p = new Piece();
            var pos = new Position(3, 3);

            s.PlacePiece(pos, p);

            Assert.AreEqual(p, s[pos]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PlacePieceTestException()
        {
            var s = new GameState(5, 5);
            var p = new Piece();
            var pos = new Position(7, 7);

            s.PlacePiece(pos, p);

            Assert.AreEqual(p, s[pos]);
        }

        [TestMethod]
        public void PlaceEmptyPieceTest()
        {
            var s = new GameState(5, 5);
            var pos = new Position(3, 3);

            s.PlaceEmptyPiece(pos);

            Assert.IsTrue(s[pos].IsEmpty);
        }

        [TestMethod]
        public void RemovePieceTest()
        {
            var s = new GameState(5, 5);
            var p = new Piece();
            var pos = new Position(3, 3);
            
            s.PlacePiece(pos, p);
            Assert.IsFalse(s[pos].IsEmpty);
            s.RemovePiece(pos);
            Assert.IsTrue(s[pos].IsEmpty);
        }

        [TestMethod]
        public void MovePieceTest()
        {
            var s = new GameState(5, 5);
            var p = new Piece();
            var from = new Position(3, 3);
            var to = new Position(5, 5);

            s.PlacePiece(from, p);
            s.MovePiece(from, to);

            Assert.AreEqual(p, s[to]);
            Assert.IsTrue(s[from].IsEmpty);
        }

        [TestMethod()]
        public void MovePieceNTest()
        {
            var s = new GameState(5, 5);
            var fromP = new Piece();
            var toP = new Piece();
            var from = new Position(3, 3);
            var to = new Position(5, 5);

            s.PlacePiece(from, fromP);
            s.PlacePiece(to, toP);
            s.MovePiece(from, to);
            s.MovePieceN(from, to);

            Assert.AreEqual(fromP, s[from]);
            Assert.AreEqual(toP, s[to]);
        }

        /// <summary>
        ///A test for PlaceEmptyPiece
        ///</summary>
        

        [TestMethod()]
        public void PlacePieceNTest()
        {
            int dimX = 0; // TODO: Initialize to an appropriate value
            int dimY = 0; // TODO: Initialize to an appropriate value
            GameState target = new GameState(dimX, dimY); // TODO: Initialize to an appropriate value
            Position pos = null; // TODO: Initialize to an appropriate value
            target.PlacePieceN(pos);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for RemovePiece
        ///</summary>
        

        /// <summary>
        ///A test for RemovePieceN
        ///</summary>
        [TestMethod()]
        public void RemovePieceNTest()
        {
            int dimX = 0; // TODO: Initialize to an appropriate value
            int dimY = 0; // TODO: Initialize to an appropriate value
            GameState target = new GameState(dimX, dimY); // TODO: Initialize to an appropriate value
            IPiece p = null; // TODO: Initialize to an appropriate value
            target.RemovePieceN(p);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        
    }
}
