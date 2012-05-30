using System;
using System.Linq;
using GameWarden.Chess.Notations;
using GameWarden.Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class PGNParserTests
    {
        [TestMethod]
        public void JustParseMovetext()
        {
            string movetext = "1. a2 e4";
            var moves = new PGNParser().ParseMoves(movetext).ToList();
            Assert.AreEqual(2, moves.Count);
        }

        [TestMethod]
        public void ParseOneMove()
        {
            string movetext = "1. a2";
            var moves = new PGNParser().ParseMoves(movetext).ToList();
            Assert.AreEqual(1, moves.Count);
        }

        [TestMethod]
        public void ParseTwoMoves()
        {
            string movetext = "1. a2 e4 2. aa3";
            var moves = new PGNParser().ParseMoves(movetext).ToList();
            Assert.AreEqual(3, moves.Count);
        }

        [TestMethod]
        public void ParseComments()
        {
            string movetext = "1. a2 { comment } 1...e4 { comment 2 } 2. aa3";
            var moves = new PGNParser().ParseMoves(movetext).ToList();
            Assert.AreEqual(3, moves.Count);
        }

        [TestMethod]
        public void ParseFull()
        {
            var pgn = new String[] { "[Tag \"Value\"]",
                                     "",
                                     "1. a2 e4" };
            
            var game = new PGNParser().Parse(pgn);
            Assert.AreEqual(2, game.MovesCount);
            Assert.AreEqual("Value", game.Info["Tag"]);
        }

        [TestMethod]
        public void ParseMovetextMultiline()
        {
            var pgn = new String[] { "[Tag \"Value\"]",
                                     "",
                                     "1. a2",
                                     "e4"};

            var game = new PGNParser().Parse(pgn);
            Assert.AreEqual(2, game.MovesCount);
            Assert.AreEqual("Value", game.Info["Tag"]);
        }

        [TestMethod]
        public void ParseFullNoDelimeter()
        {
            var pgn = new String[] { "[Tag \"Value\"]",
                                     "1. a2 e4" };

            var game = new PGNParser().Parse(pgn);
            Assert.AreEqual(2, game.MovesCount);
            Assert.AreEqual("Value", game.Info["Tag"]);
        }
        
        [TestMethod]
        public void ParseTequilajazzzStartingFEN()
        {
            var pgn = new String[] { "[FEN \"1k6/1p6/p1p3b1/5q2/8/P7/1PP5/1K6 w KQkq - 0 42\"]",
                                     "1. a2 e4" };

            var game = new PGNParser().Parse(pgn);
            Assert.AreEqual("1k6/1p6/p1p3b1/5q2/8/P7/1PP5/1K6 w KQkq - 0 42", game.State.ToString());
        }

        [TestMethod]
        public void ParseNoMovetext()
        {
            var pgn = new String[] { "[FEN \"1k6/1p6/p1p3b1/5q2/8/P7/1PP5/1K6 w KQkq - 0 42\"]" };

            var game = new PGNParser().Parse(pgn);
        }
    }
}
