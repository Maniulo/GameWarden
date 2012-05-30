using System;
using System.Collections.Generic;
using GameWarden;
using GameWarden.Chess;
using GameWarden.Chess.Notations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class ChessMoveTests
    {
        [TestMethod]
        public void SimpleMove()
        {
            string from = "8/8/8/8/8/8/P7/8 w KQkq - 0 1";
            string   to = "8/8/8/8/8/P7/8/8 b KQkq - 0 1";

            var players = new List<Player> { new Player(1), new Player(2) };
            var s = new FENParser().Parse(from, players);
            var m = new ChessMove("a3") { PieceType = PieceTypes.Pawn, Player = players[0], From = new Position(), To = "a3"};
            m.Apply(s);
            Assert.AreEqual(to, s.ToString());
        }

        [TestMethod]
        public void SolveFile()
        {
            string from = "R7/8/8/8/8/8/8/R7 w KQkq - 0 1";
            string   to = "R7/8/8/8/R7/8/8/8 b KQkq - 0 1";

            var players = new List<Player> { new Player(1), new Player(2) };
            var s = new FENParser().Parse(from, players);
            var m = new ChessMove("R1a4") { PieceType = PieceTypes.Rook, Player = players[0], From = new Position(null, 1), To = "a4"};
            m.Apply(s);
            Assert.AreEqual(to, s.ToString());
        }

        [TestMethod]
        public void SolveRank()
        {
            string from = "8/8/8/8/8/8/8/R6R w KQkq - 0 1";
            string   to = "8/8/8/8/8/8/8/3R3R b KQkq - 0 1";

            var players = new List<Player> { new Player(1), new Player(2) };
            var s = new FENParser().Parse(from, players);
            var m = new ChessMove("Rad1") { PieceType = PieceTypes.Rook, Player = players[0], From = new Position(1, null), To = "d1" };
            m.Apply(s);
            Assert.AreEqual(to, s.ToString());
        }

        [TestMethod]
        public void SolveNothing()
        {
            string from = "8/8/8/8/8/3R4/2R1R3/3R4 w KQkq - 0 1";
            string   to = "8/8/8/8/8/3R4/2RRR3/8 b KQkq - 0 1";

            var players = new List<Player> { new Player(1), new Player(2) };
            var s = new FENParser().Parse(from, players);
            var m = new ChessMove("Rd1d2") { PieceType = PieceTypes.Rook, Player = players[0], From = "d1", To = "d2" };
            m.Apply(s);
            Assert.AreEqual(to, s.ToString());
        }

        [TestMethod]
        public void SolveRightPlayer()
        {
            string from = "8/8/8/8/8/8/8/R6r w KQkq - 0 1";
            string to = "8/8/8/8/8/8/8/3R3r b KQkq - 0 1";

            var players = new List<Player> { new Player(1), new Player(2) };
            var s = new FENParser().Parse(from, players);
            var m = new ChessMove("Rd1") { PieceType = PieceTypes.Rook, Player = players[0], From = new Position(), To = "d1" };
            m.Apply(s);
            Assert.AreEqual(to, s.ToString());
        }

        [TestMethod]
        public void SolveRightType()
        {
            string from = "8/8/8/8/8/B7/8/R7 w KQkq - 0 1";
            string to = "8/8/8/8/8/B7/8/2R5 b KQkq - 0 1";

            var players = new List<Player> { new Player(1), new Player(2) };
            var s = new FENParser().Parse(from, players);
            var m = new ChessMove("Rc1") { PieceType = PieceTypes.Rook, Player = players[0], From = new Position(), To = "c1" };
            m.Apply(s);
            Assert.AreEqual(to, s.ToString());
        }

        [TestMethod]
        public void SolveKingUnderCheck()
        {
            string from = "3r4/8/8/8/8/8/3N1N2/3K4 w KQkq - 0 1";
            string to = "3r4/8/8/8/4N3/8/3N4/3K4 b KQkq - 0 1";

            var players = new List<Player> { new Player(1), new Player(2) };
            var s = new FENParser().Parse(from, players);
            var m = new ChessMove("Ne4") { PieceType = PieceTypes.Knight, Player = players[0], From = new Position(), To = "e4" };
            m.Apply(s);
            Assert.AreEqual(to, s.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CannotSolveKingSuicide()
        {
            string from = "1r6/8/8/8/8/8/7r/K7 w KQkq - 0 1";

            var players = new List<Player> { new Player(1), new Player(2) };
            var s = new FENParser().Parse(from, players);
            var m = new ChessMove("Kb2") { PieceType = PieceTypes.King, Player = players[0], From = new Position(), To = "b2" };
            m.Apply(s);
        }
    }
}
