using GameWarden.Chess;
using GameWarden.Chess.Notations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameWarden;

namespace UnitTesting
{
    [TestClass]
    public class ChessGameTests
    {
        [TestMethod]
        public void ChessGameFromFEN()
        {
            const string fenString = "rnbqkbnr/pppppppp/8/8/8/3P4/PPP1PPPP/RNBQKBNR w KQkq - 0 1";
            var i = new Meta();
            i["FEN"] = fenString;
            var g = new ChessGame(i);
            Assert.AreEqual(fenString, g.State.ToString());
        }

        [TestMethod]
        public void ChessGameDefaultFEN()
        {
            var g = new ChessGame();
            Assert.AreEqual(FENParser.DefaultFEN, g.State.ToString());
        }
    }
}
