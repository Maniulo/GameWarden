using System;
using GameWarden.Chess.Notations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameWarden.Chess;

namespace UnitTesting
{

    [TestClass()]
    public class AlgebraicNotationTests
    {
        [TestMethod]
        public void ParseFull()
        {
            var parser = new AlgebraicNotation();
            var moveRecord = "a2a4";
            ChessMove move = parser.Parse(moveRecord);

            Assert.AreEqual(1, move.From.File);
            Assert.AreEqual(2, move.From.Rank);
            Assert.AreEqual(1, move.To.File);
            Assert.AreEqual(4, move.To.Rank);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParseLight()
        {
            var parser = new AlgebraicNotation();
            var moveRecord = "a4";
            ChessMove move = parser.Parse(moveRecord);

            Assert.AreEqual(null, move.From.File);
            Assert.AreEqual(null, move.From.Rank);
        }

        [TestMethod]
        public void ParseFigure()
        {
            var parser = new AlgebraicNotation();
            var moveRecord = "Ka4";
            ChessMove move = parser.Parse(moveRecord);
            
            Assert.AreEqual(PieceTypes.King, move.PieceType);
        }

        [TestMethod]
        public void ParsePawnFigure()
        {
            var parser = new AlgebraicNotation();
            var moveRecord = "a4";
            ChessMove move = parser.Parse(moveRecord);

            Assert.AreEqual(PieceTypes.Pawn, move.PieceType);
        }

        [TestMethod]
        public void ParseFromFile()
        {
            var parser = new AlgebraicNotation();
            var moveRecord = "aa4";
            ChessMove move = parser.Parse(moveRecord);

            Assert.AreEqual(1, move.From.File);
            Assert.AreEqual(1, move.To.File);
            Assert.AreEqual(4, move.To.Rank);
        }

        [TestMethod]
        public void ParseFromRank()
        {
            var parser = new AlgebraicNotation();
            var moveRecord = "3a4";
            ChessMove move = parser.Parse(moveRecord);

            Assert.AreEqual(3, move.From.Rank);
            Assert.AreEqual(1, move.To.File);
            Assert.AreEqual(4, move.To.Rank);
        }

        [TestMethod]
        public void ParseKingside()
        {
            var parser = new AlgebraicNotation();
            var moveRecord = "O-O";
            ChessMove move = parser.Parse(moveRecord);
            
            Assert.IsTrue(move.CastlingKingside);
        }

        [TestMethod]
        public void ParseQueenside()
        {
            var parser = new AlgebraicNotation();
            var moveRecord = "O-O-O";
            ChessMove move = parser.Parse(moveRecord);

            Assert.IsTrue(move.CastlingQueenside);
        }

        [TestMethod]
        public void ParseNoCastling()
        {
            var parser = new AlgebraicNotation();
            var moveRecord = "e2e4";
            ChessMove move = parser.Parse(moveRecord);

            Assert.IsFalse(move.CastlingKingside);
            Assert.IsFalse(move.CastlingQueenside);
        }

        [TestMethod]
        public void ParsePromotion()
        {
            var parser = new AlgebraicNotation();
            var moveRecord = "a8Q";
            ChessMove move = parser.Parse(moveRecord);
            
            Assert.IsTrue(move.IsPromotion);
            Assert.AreEqual(PieceTypes.Queen, move.PromotionTo);
        }

        [TestMethod]
        public void ParsePromotion2()
        {
            var parser = new AlgebraicNotation();
            var moveRecord = "a8=Q";
            ChessMove move = parser.Parse(moveRecord);

            Assert.IsTrue(move.IsPromotion);
            Assert.AreEqual(PieceTypes.Queen, move.PromotionTo);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseError()
        {
            var parser = new AlgebraicNotation();
            var moveRecord = "z4";
            ChessMove move = parser.Parse(moveRecord);
        }
    }
}
