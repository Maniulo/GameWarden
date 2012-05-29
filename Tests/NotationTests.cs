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
    public class NotationTests
    {
        private const String DefaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq e6 0 1";

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseFENException()
        {
            new FENParser().Parse("NOT VALID FEN STRING");
        }

        /*
        [TestMethod]
        public void ParseFENPosition()
        {
            ChessGame game = new ChessGame();
            FEN FENParser = new FEN();
            game = FEN.Parse(DefaultFENPosition);
            Assert.AreEqual(DefaultFENPosition, game.ToString());
        }
         * 
        [TestMethod]
        public void ParseFENState()
        {
            ChessGame game = new ChessGame();
            FEN FENParser = new FEN();
            game = FENParser.ParseState(DefaultFENState);
            Assert.AreEqual(DefaultFENState, game.ToString());
        }
         */

        [TestMethod]
        public void ParseFEN()
        {
            Assert.AreEqual(DefaultFEN, new FENParser().Parse(DefaultFEN).ToString());
        }

        [TestMethod]
        public void ParseFENCastling()
        {
            ChessState gameState = new FENParser().Parse("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w Kq - 0 1");
            Assert.IsTrue(gameState.Castling.KingsideWhite);
            Assert.IsFalse(gameState.Castling.QueensideWhite);
            Assert.IsFalse(gameState.Castling.KingsideBlack);
            Assert.IsTrue(gameState.Castling.QueensideBlack);
        }

        /*
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseTagsException()
        {
            var tags = new String[2];
            tags[0] = "[Event \"Some Event\"]";
            tags[1] = "[BadTag]";
            PGN PGNParser = new PGN();
            PGNParser.Parse(tags, new AN());
        }
        */

        /*
        [TestMethod]
        public void ParseTags()
        {
            var tags = new String[2];
            tags[0] = "[Event \"Some Event\"]";
            tags[1] = "[Site \"Some Site\"]";
            PGN PGNParser = new PGN();
            Assert.AreEqual("Some Event", PGNParser.Parse(tags, new AN()).Info["Event"]);
        }
        */

        [TestMethod]
        public void ParseSANSingleMove()
        {
            var move = new AlgebraicNotation().Parse("e2e4");
            Assert.AreEqual("e2e4", move.ToString());
        }

        [TestMethod]
        public void ParsePGNFullMove()
        {
            var moves = new PGNParser().ParseMoves("1. e2e4 e2e5", new AlgebraicNotation()).ToArray();
            Assert.AreEqual("e2e4", ((ChessMove)moves[0]).Desc);
            Assert.AreEqual("e2e5", ((ChessMove)moves[1]).Desc);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParsePGNFullMoveException()
        {
            var moves = new PGNParser().ParseMoves("1. e2q4 Qqe5", new AlgebraicNotation()).ToArray();
            Assert.AreEqual("e2q4", ((ChessMove)moves[0]).Desc);
            Assert.AreEqual("Qqe5", ((ChessMove)moves[1]).Desc);
        }

        [TestMethod]
        public void ParsePGNFullMoveWithComments()
        {
            var moves = new PGNParser().ParseMoves("1. e2e4 {Comment.} 1... e2e5", new AlgebraicNotation()).ToArray();
            Assert.AreEqual("e2e4", ((ChessMove)moves[0]).Desc);
            Assert.AreEqual("e2e5", ((ChessMove)moves[1]).Desc);
        }

        [TestMethod]
        public void ParsePGN()
        {
            var pgn = new String[5];
            pgn[0] = "[Event \"Some Event\"]";
            pgn[1] = "[Site \"Some Site\"]";
            pgn[2] = "";
            pgn[3] = "1. e2e4 {Comment.} 1... e2e5 2. e2e6";
            pgn[4] = "e2e7 {}";

            var moves = new String[4];
            moves[0] = "e2e4";
            moves[1] = "e2e5";
            moves[2] = "e2e6";
            moves[3] = "e2e7";

            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            Assert.AreEqual("Some Event", game.Info["Event"]);
            for (int i = 0; i < 4; ++i)
                Assert.AreEqual(moves[i], ((ChessMove)game.Moves().ToList()[i]).Desc);
        }

        [TestMethod]
        public void PGNStartingPositionTest()
        {
            var pgn = new String[3];
            pgn[0] = "[FEN \"8/8/8/8/8/8/3P4/8 w KQkq - 0 1\"]";
            pgn[1] = "";
            pgn[2] = "1. d2d3";
            
            ChessGame game = new PGNParser().Parse(pgn, new AlgebraicNotation());
            Assert.AreEqual("8/8/8/8/8/8/3P4/8 w KQkq - 0 1", game.State.ToString());
        }
    }
}
