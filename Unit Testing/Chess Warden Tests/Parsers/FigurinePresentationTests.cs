using System;
using GameWarden;
using GameWarden.Chess;
using GameWarden.Chess.Notations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class FigurinePresentationTests
    {
        [TestMethod]
        public void GetPieceType()
        {
            var p = new FigurinePresentation();
            Assert.AreEqual(PieceTypes.King, p.GetPieceType('♚'));
        }

        [TestMethod]
        public void GetPlayerWhite()
        {
            var p = new FigurinePresentation();
            Assert.AreEqual(1, p.GetPlayer('♔'));
        }

        [TestMethod]
        public void GetPlayerBlack()
        {
            var p = new FigurinePresentation();
            Assert.AreEqual(2, p.GetPlayer('♜'));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPieceTypeException()
        {
            var p = new FigurinePresentation();
            p.GetPieceType('Z');
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPieceTypeWrongObjectException()
        {
            var p = new FigurinePresentation();
            p.GetPieceType(2);
        }

        [TestMethod]
        public void GetPresentationWhite()
        {
            var p = new FigurinePresentation();
            var pl = new Player(1);
            var o = new ChessPiece { Type = PieceTypes.Rook, Player = pl };
            Assert.AreEqual('♖', p.GetPresentation(o));
        }

        [TestMethod]
        public void GetPresentationBlack()
        {
            var p = new FigurinePresentation();
            var pl = new Player(2);
            var o = new ChessPiece { Type = PieceTypes.Rook, Player = pl };
            Assert.AreEqual('♜', p.GetPresentation(o));
        }

        [TestMethod]
        public void GetEmptyCell()
        {
            var p = new FigurinePresentation();
            var pl = new Player(2);
            var o = new ChessPiece { IsEmpty = true };
            Assert.AreEqual(null, p.GetPresentation(o));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPresentationException()
        {
            var p = new FigurinePresentation();
            var o = new Piece();
            p.GetPresentation(o);
        }

        [TestMethod]
        public void ToStringTest()
        {
            Assert.AreEqual("♙♘♗♖♕♔♟♞♝♜♛♚", new FigurinePresentation().ToString());
        }
    }
}
