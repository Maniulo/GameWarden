using GameWarden.Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameWarden;

namespace UnitTesting
{
    [TestClass]
    public class ChessPieceTests
    {
        [TestMethod]
        public void ChessPieceCopyConstructor()
        {
            var p1 = new ChessPiece { Type = PieceTypes.King };
            var p2 = new ChessPiece (p1);

            Assert.AreEqual(p1.Type, p2.Type);
        }

        [TestMethod]
        public void CanAttack()
        {
            var s = new ChessState();
            var from = new Position(1, 1);
            var to   = new Position(2, 2);
            var p = new ChessPiece();
            s[from] = p;
            s[from].Move(from);
            s[to] = new ChessPiece();
            s[to].Move(to);

            s[from].PossibleMoves.Add(new MockTemplate(null, true));

            Assert.IsTrue(p.CanAttack(to, s));
        }

        [TestMethod]
        public void CanNotAttack()
        {
            var pos = new Position(1, 1);
            var s = new ChessState();
            var p = new ChessPiece();
            p.Move(new Position(2, 2));
            p.PossibleMoves.Add(new MockTemplate(null, false));

            Assert.IsFalse(p.CanAttack(pos, s));
        }

        [TestMethod]
        public void GetPossibleMove()
        {
            var pos = new Position(1, 1);
            var s = new ChessState();
            var p = new ChessPiece();
            p.Move(new Position(2, 2));
            p.PossibleMoves.Add(new MockTemplate());

            Assert.IsNotNull(p.GetPossibleMove(pos, s, PieceTypes.Pawn));
        }

        [TestMethod]
        public void NoPossibleMoves()
        {
            var s = new ChessState();
            var from = new Position(1, 1);
            var to = new Position(1, 3);
            var p = new ChessPiece();
            s[from] = p;
            s[from].Move(from);

            p.PossibleMoves.Add(new MockTemplate(1));

            Assert.IsNull(p.GetPossibleMove(to, s, PieceTypes.Pawn));
        }
    }
}
