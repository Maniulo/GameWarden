using System;
using GameWarden;
using GameWarden.Chess;
using GameWarden.Chess.Notations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class EnglishFENPresentationTests
    {
        [TestMethod]
        public void GetPieceTypeLower()
        {
            var p = new EnglishFENPresentation();
            Assert.AreEqual(PieceTypes.King, p.GetPieceType('k'));
        }

        [TestMethod]
        public void GetPieceTypeCapital()
        {
            var p = new EnglishFENPresentation();
            Assert.AreEqual(PieceTypes.King, p.GetPieceType('K'));
        }

        [TestMethod]
        public void GetPieceTypePawn()
        {
            var p = new EnglishFENPresentation();
            Assert.AreEqual(PieceTypes.Pawn, p.GetPieceType('P'));
        }

        [TestMethod]
        public void GetPlayerLower()
        {
            var p = new EnglishFENPresentation();
            Assert.AreEqual(2, p.GetPlayer('k'));
        }

        [TestMethod]
        public void GetPlayerCapital()
        {
            var p = new EnglishFENPresentation();
            Assert.AreEqual(1, p.GetPlayer('K'));
        }

        [TestMethod]
        public void GetPieceTypeString()
        {
            var p = new EnglishFENPresentation();
            Assert.AreEqual(PieceTypes.Bishop, p.GetPieceType("B"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPieceTypeException()
        {
            var p = new EnglishFENPresentation();
            Assert.AreEqual(PieceTypes.Pawn, p.GetPieceType('Z'));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPieceTypeWrongObjectException()
        {
            var p = new EnglishFENPresentation();
            Assert.AreEqual(PieceTypes.Pawn, p.GetPieceType(2));
        }

        [TestMethod]
        public void GetPresentationWhite()
        {
            var p = new EnglishFENPresentation();
            var pl = new Player(1);
            var o = new ChessPiece { Type = PieceTypes.Rook, Player = pl };
            Assert.AreEqual('R', p.GetPresentation(o));
        }

        [TestMethod]
        public void GetPresentationBlack()
        {
            var p = new EnglishFENPresentation();
            var pl = new Player(2);
            var o = new ChessPiece { Type = PieceTypes.Rook, Player = pl };
            Assert.AreEqual('r', p.GetPresentation(o));
        }

        [TestMethod]
        public void GetPresentationPawn()
        {
            var p = new EnglishFENPresentation();
            var pl = new Player(2);
            var o = new ChessPiece { Type = PieceTypes.Pawn, Player = pl };
            Assert.AreEqual('p', p.GetPresentation(o));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPresentationException()
        {
            var p = new EnglishFENPresentation();
            var o = new Piece();
            p.GetPresentation(o);
        }

        [TestMethod]
        public void ToStringTest()
        {
            Assert.AreEqual("PNBRQKpnbrqk", new EnglishFENPresentation().ToString());
        }
    }
}
