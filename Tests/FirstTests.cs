using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameWarden;
using GameWarden.Chess;

namespace GameWarden.Tests
{
    [TestClass]
    public class FirstTests
    {
        /*
        [TestMethod]
        public void CreateGame()
        {
            Game g = new Game();
            Assert.AreEqual("First Game", g.Meta);
        }
        */
        /*
        [TestMethod]
        public void CreateSampleGame()
        {
            Game g = new Game();
            g.SetInfo("Description", "Second Game");
            Assert.AreEqual("Second Game", g.GetInfo("Description"));
        }*/
        /*
        [TestMethod]
        public void AddPiece()
        {
            Game g = new Game("First Game");
            Piece p = g.AddPiece(ChessType.Pawn);
            Assert.AreEqual(ChessType.Pawn, p.type);
        }
        *//*
        [TestMethod]
        public void CreateSamplePiece()
        {
            Game g = new Game();
            Piece p = g.AddPiece(ChessType.Pawn, "A2");
            Assert.AreEqual(ChessType.Pawn, g.CurrentState["A2"].type);
        }

        [TestMethod]
        public void MoveSamplePiece()
        {
            Game g = new Game();
            Piece p = g.AddPiece(ChessType.Pawn, "A2");
            g.CurrentState["A2"].Move("A3");
            Assert.AreEqual(null, g.CurrentState["A2"]);
            Assert.AreEqual(ChessType.Pawn, g.CurrentState["A3"].type);
        }

        [TestMethod]
        public void CaptureSamplePiece()
        {
            Game g = new Game();
            Piece p = g.AddPiece(ChessType.Pawn, "A2");
            Piece p2 = g.AddPiece(ChessType.Pawn, "B3");
            g.CurrentState["A2"].Move("B3");
            Assert.AreEqual(null, g.CurrentState["A2"]);
            Assert.AreEqual(p, g.CurrentState["B3"]);
        }

        [TestMethod]
        public void IncorrectMoveCheckPiece()
        {
            Game g = new Game();
            Piece p = g.AddPiece(ChessType.Pawn, "A2");
            Assert.AreEqual(false, g.CurrentState["A2"].CanMove("A5"));
        }

        [TestMethod]
        public void IncorrectMoveCheckWithBoardPiece()
        {
            Game g = new Game();
            Piece p = g.AddPiece(ChessType.Pawn, "A2");
            Piece p2 = g.AddPiece(ChessType.Pawn, "A3");
            Assert.AreEqual(false, g.CurrentState["A2"].CanMove("A4"));
        }

        [TestMethod]
        public void SingleMoveType()
        {
            Game g = new Game();
            Piece p = g.AddPiece(ChessType.Pawn, "A2");
            g.CurrentState["A2"].Move("B3");
            Assert.AreEqual(p, g.CurrentState["B3"]);
        }*/
    }
}
