using System;
using GameWarden.Chess.Notations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameWarden.Chess;
using GameWarden;

namespace UnitTesting
{
    [TestClass]
    public class EnglishPresentationTests
    {
        [TestMethod]
        public void GetPieceTypeLower()
        {
            var p = new EnglishPresentation();
            Assert.AreEqual(PieceTypes.King, p.GetPieceType('k'));
        }

        [TestMethod]
        public void GetPieceTypeCapital()
        {
            var p = new EnglishPresentation();
            Assert.AreEqual(PieceTypes.King, p.GetPieceType('K'));
        }

        [TestMethod]
        public void GetPieceTypePawn()
        {
            var p = new EnglishPresentation();
            Assert.AreEqual(PieceTypes.Pawn, p.GetPieceType(null));
        }

        [TestMethod]
        public void GetPieceTypeString()
        {
            var p = new EnglishPresentation();
            Assert.AreEqual(PieceTypes.Bishop, p.GetPieceType("B"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPieceTypeException()
        {
            var p = new EnglishPresentation();
            Assert.AreEqual(PieceTypes.Pawn, p.GetPieceType('Z'));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPieceTypeWrongObjectException()
        {
            var p = new EnglishPresentation();
            Assert.AreEqual(PieceTypes.Pawn, p.GetPieceType(2));
        }

        [TestMethod]
        public void GetPresentation()
        {
            var p = new EnglishPresentation();
            var o = new ChessPiece {Type = PieceTypes.Rook};
            Assert.AreEqual('R', p.GetPresentation(o));
        }

        [TestMethod]
        public void GetPresentationPawn()
        {
            var p = new EnglishPresentation();
            var o = new ChessPiece { Type = PieceTypes.Pawn };
            Assert.AreEqual(null, p.GetPresentation(o));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPresentationException()
        {
            var p = new EnglishPresentation();
            var o = new Piece();
            p.GetPresentation(o);
        }

        [TestMethod]
        public void ToStringTest()
        {
            Assert.AreEqual("NBRQK", new EnglishPresentation().ToString());
        }
    }
}
