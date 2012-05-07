using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameWarden.Chess;
using GameWarden.Chess.Notations;

namespace GameWarden.Tests
{
    [TestClass]
    public class GameStateTests
    {
        [TestMethod]
        public void MakeMoveTest()
        {
            String[] pgn = new String[4];
            pgn[0] = "[Event \"Some Event\"]";
            pgn[1] = "[FEN \"8/8/8/8/8/8/P7/8 w KQkq - 0 1\"]";
            pgn[2] = "";
            pgn[3] = "1. a2a3 {}";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
            Assert.AreEqual("8/8/8/8/8/P7/8/8 w KQkq - 0 1", game.CurrentState.ToString());
        }

        [TestMethod]
        public void CaptureTest()
        {
            String[] pgn = new String[3];
            pgn[0] = "[FEN \"8/8/8/8/4p3/3P4/8/8 w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. d3xe4";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
            Assert.AreEqual("8/8/8/8/4P3/8/8/8 w KQkq - 0 1", game.CurrentState.ToString());
        }

        [TestMethod]
        public void EnPassantCaptureTest()
        {
            String[] pgn = new String[3];
            pgn[0] = "[FEN \"8/8/8/4Pp2/8/8/8/8 w KQkq f6 0 1\"]";
            pgn[1] = "";
            pgn[2] = "2. e5xf6e.p.";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
            Assert.AreEqual("8/8/5P2/8/8/8/8/8 w KQkq f6 0 1", game.CurrentState.ToString());
        }

        [TestMethod]
        public void RealMoveTest()
        {
            String[] pgn = new String[3];
            pgn[0] = "[FEN \"8/8/8/8/4P3/8/8/8 w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. e5";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
            Assert.AreEqual("8/8/8/4P3/8/8/8/8 w KQkq - 0 1", game.CurrentState.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void NotPossibleMoveTest()
        {
            String[] pgn = new String[3];
            pgn[0] = "[FEN \"8/8/8/8/4P3/8/8/8 w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. e8";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
        }

        [TestMethod]
        public void BishopTest()
        {
            String[] pgn = new String[3];
            pgn[0] = "[FEN \"8/7P/8/8/8/8/8/B7 w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. Bh8";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
            Assert.AreEqual("7B/7P/8/8/8/8/8/8 w KQkq - 0 1", game.CurrentState.ToString());
        }

        [TestMethod]
        public void RookTest()
        {
            String[] pgn = new String[3];
            pgn[0] = "[FEN \"8/8/8/8/8/8/8/R7 w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. Rh1";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
            Assert.AreEqual("8/8/8/8/8/8/8/7R w KQkq - 0 1", game.CurrentState.ToString());
        }

        [TestMethod]
        public void QueenTest()
        {
            String[] pgn = new String[3];
            pgn[0] = "[FEN \"7q/8/8/8/8/8/8/Q7 w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. Qa8 Qa1";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
            game.MakeMove();
            Assert.AreEqual("Q7/8/8/8/8/8/8/q7 w KQkq - 0 1", game.CurrentState.ToString());
        }

        [TestMethod]
        public void KnightTest()
        {
            String[] pgn = new String[3];
            pgn[0] = "[FEN \"8/8/3pp3/3P4/3N4/8/8/8 w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. Nxe6";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
            Assert.AreEqual("8/8/3pN3/3P4/8/8/8/8 w KQkq - 0 1", game.CurrentState.ToString());
        }

        [TestMethod]
        public void KingTest()
        {
            String[] pgn = new String[3];
            pgn[0] = "[FEN \"8/8/8/8/8/8/8/K7 w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. Kb2";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
            Assert.AreEqual("8/8/8/8/8/8/1K6/8 w KQkq - 0 1", game.CurrentState.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void KingLengthTest()
        {
            String[] pgn = new String[3];
            pgn[0] = "[FEN \"8/8/8/8/8/8/8/K7 w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. Kc3";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
            Assert.AreEqual("8/8/8/8/8/2K5/8/8 w KQkq - 0 1", game.CurrentState.ToString());
        }

        [TestMethod]
        public void WhiteCastlingKingsideTest()
        {
            String[] pgn = new String[3];
            pgn[0] = "[FEN \"8/8/8/8/8/8/8/4K2R w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. O-O";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
            Assert.AreEqual("8/8/8/8/8/8/8/5RK1 w KQkq - 0 1", game.CurrentState.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void WhiteCastlingKingsideBarrierTest()
        {
            String[] pgn = new String[3];
            pgn[0] = "[FEN \"8/8/8/8/8/8/8/4K1PR w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. O-O";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
            Assert.AreEqual("8/8/8/8/8/8/8/5RK1 w KQkq - 0 1", game.CurrentState.ToString());
        }
        /*
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void WhiteCastlingKingsideUnderAttackTest()
        {
            String[] pgn = new String[3];
            pgn[0] = "[FEN \"8/8/8/8/8/8/8/4K1PR w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. O-O";

            ChessGame game = new PGN().Parse(pgn, new AlgebraicNotation());
            game.MakeMove();
            Assert.AreEqual("8/8/8/8/8/8/8/5RK1 w KQkq - 0 1", game.CurrentState.ToString());
        }
         */
    }
}
