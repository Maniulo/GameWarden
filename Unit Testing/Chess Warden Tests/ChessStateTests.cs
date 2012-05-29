using System.Collections.Generic;
using GameWarden.Chess;
using GameWarden.Chess.Notations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameWarden;

namespace UnitTesting
{
    
    [TestClass]
    public class ChessStateTests
    {
        [TestMethod]
        public void IsKingOpen()
        {
            var s = new ChessState();
            var players = new List<Player> {new Player(1), new Player(2)};
            var from = new Position(1, 1);
            var to = new Position(3, 3);
            s[from] = ChessPieceFactory.CreatePiece('K', new EnglishFENPresentation(), players);
            s[to] = ChessPieceFactory.CreatePiece('q', new EnglishFENPresentation(), players);
            s[from].Move(from);
            s[to].Move(to);

            Assert.IsTrue(s.IsKingOpen(players[0]));
        }

        [TestMethod]
        public void IsKingNotOpen()
        {
            var s = new ChessState();
            var players = new List<Player> { new Player(1), new Player(2) };
            var from = new Position(1, 1);
            var to = new Position(3, 3);
            s[from] = ChessPieceFactory.CreatePiece('K', new EnglishFENPresentation(), players);
            s[to] = ChessPieceFactory.CreatePiece('p', new EnglishFENPresentation(), players);
            s[from].Move(from);
            s[to].Move(to);

            Assert.IsFalse(s.IsKingOpen(players[0]));
        }

        [TestMethod]
        public void IsUnderAttack()
        {
            var s = new ChessState();
            var players = new List<Player> { new Player(1), new Player(2) };
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            s[from] = ChessPieceFactory.CreatePiece('K', new EnglishFENPresentation(), players);
            s[to] = ChessPieceFactory.CreatePiece('p', new EnglishFENPresentation(), players);
            s[from].Move(from);
            s[to].Move(to);

            Assert.IsTrue(s.IsUnderAttack(to, players[1]));
        }

        [TestMethod]
        public void IsNotUnderAttack()
        {
            var s = new ChessState();
            var players = new List<Player> { new Player(1), new Player(2) };
            var from = new Position(1, 1);
            var to = new Position(3, 3);
            s[from] = ChessPieceFactory.CreatePiece('K', new EnglishFENPresentation(), players);
            s[from].Move(from);

            Assert.IsFalse(s.IsUnderAttack(to, players[0]));
        }

        [TestMethod]
        public void IsUnderAttackEmpty()
        {
            var s = new ChessState();
            var players = new List<Player> { new Player(1), new Player(2) };
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            s[from] = ChessPieceFactory.CreatePiece('K', new EnglishFENPresentation(), players);
            s[from].Move(from);

            Assert.IsTrue(s.IsUnderAttack(to, players[1]));
        }

        [TestMethod]
        public void NewEmptyPieceTest()
        {
            var s = new ChessState();
            var pos = new Position(1, 1);
            s[pos] = new ChessPiece();
            s.NewEmptyPiece(pos);
            Assert.IsTrue(s[pos].IsEmpty);
        }

        [TestMethod]
        public void SwitchPlayersTest()
        {
            var s = new ChessState();

            Assert.AreEqual('w', s.Player);
            s.SwitchPlayers();
            Assert.AreEqual('b', s.Player);
            s.SwitchPlayers();
            Assert.AreEqual('w', s.Player);
        }
    }
}
