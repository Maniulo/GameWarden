using GameWarden;
using GameWarden.Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class CastlingTests
    {
        [TestMethod]
        public void CastlingKingside()
        {
            var c = new CastlingTemplate(CastlingTemplate.CastlingType.Kingside);

            var from = new Position(5, 1);
            var to   = new Position(7, 1);
            var rook = new Position(8, 1);

            var s = new ChessState();
            s[rook] = new ChessPiece {Type = PieceTypes.Rook};
            s[rook].Move(rook);
            s[from] = new ChessPiece {Type = PieceTypes.King};
            s[from].Move(from);

            Assert.IsTrue(c.CanApply(from, to, s));
        }

        [TestMethod]
        public void CastlingQueenside()
        {
            var c = new CastlingTemplate(CastlingTemplate.CastlingType.Queenside);

            var from = new Position(5, 1);
            var to = new Position(3, 1);
            var rook = new Position(1, 1);

            var s = new ChessState();
            s[rook] = new ChessPiece { Type = PieceTypes.Rook };
            s[rook].Move(rook);
            s[from] = new ChessPiece { Type = PieceTypes.King };
            s[from].Move(from);

            Assert.IsTrue(c.CanApply(from, to, s));
        }

        [TestMethod]
        public void CastlingQueensideNotKingside()
        {
            var c = new CastlingTemplate(CastlingTemplate.CastlingType.Queenside);

            var from = new Position(5, 1);
            var to = new Position(7, 1);
            var rook = new Position(8, 1);

            var s = new ChessState();
            s[rook] = new ChessPiece { Type = PieceTypes.Rook };
            s[rook].Move(rook);
            s[from] = new ChessPiece { Type = PieceTypes.King };
            s[from].Move(from);

            Assert.IsFalse(c.CanApply(from, to, s));
        }

        [TestMethod]
        public void CastlingRookMoved()
        {
            var c = new CastlingTemplate(CastlingTemplate.CastlingType.Kingside);

            var from = new Position(5, 1);
            var to = new Position(7, 1);
            var rook = new Position(8, 1);

            var s = new ChessState();
            s[rook] = new ChessPiece { Type = PieceTypes.Rook };
            s[rook].Move(rook);
            s[rook].Move(rook);
            s[from] = new ChessPiece { Type = PieceTypes.King };
            s[from].Move(from);

            Assert.IsFalse(c.CanApply(from, to, s));
        }

        [TestMethod]
        public void CastlingKingMoved()
        {
            var c = new CastlingTemplate(CastlingTemplate.CastlingType.Kingside);

            var from = new Position(5, 1);
            var to = new Position(7, 1);
            var rook = new Position(8, 1);

            var s = new ChessState();
            s[rook] = new ChessPiece { Type = PieceTypes.Rook };
            s[rook].Move(rook);
            s[from] = new ChessPiece { Type = PieceTypes.King };
            s[from].Move(from);
            s[from].Move(from);

            Assert.IsFalse(c.CanApply(from, to, s));
        }

        [TestMethod]
        public void KingUnderAttack()
        {
            var c = new CastlingTemplate(CastlingTemplate.CastlingType.Kingside);

            var from = new Position(5, 1);
            var to = new Position(7, 1);
            var rook = new Position(8, 1);
            var enmy = new Position(5, 3);
            var white = new Player(1);

            var s = new ChessState();
            s[rook] = new ChessPiece { Type = PieceTypes.Rook, Player = white };
            s[rook].Move(rook);
            s[from] = new ChessPiece { Type = PieceTypes.King, Player = white };
            s[from].Move(from);
            s[enmy] = new ChessPiece { Type = PieceTypes.Rook, Player = new Player(2) };
            s[enmy].Move(enmy);
            ChessPieceFactory.AddMoves((ChessPiece) s[enmy]);
            
            Assert.IsFalse(c.CanApply(from, to, s));
        }

        [TestMethod]
        public void KingPathUnderAttack()
        {
            var c = new CastlingTemplate(CastlingTemplate.CastlingType.Kingside);

            var from = new Position(5, 1);
            var to = new Position(7, 1);
            var rook = new Position(8, 1);
            var enmy = new Position(6, 3);
            var white = new Player(1);

            var s = new ChessState();
            s[rook] = new ChessPiece { Type = PieceTypes.Rook, Player = white };
            s[rook].Move(rook);
            s[from] = new ChessPiece { Type = PieceTypes.King, Player = white };
            s[from].Move(from);
            s[enmy] = new ChessPiece { Type = PieceTypes.Rook, Player = new Player(2) };
            s[enmy].Move(enmy);
            ChessPieceFactory.AddMoves((ChessPiece)s[enmy]);

            Assert.IsFalse(c.CanApply(from, to, s));
        }

        [TestMethod]
        public void RookPathUnderAttack()
        {
            var c = new CastlingTemplate(CastlingTemplate.CastlingType.Kingside);

            var from = new Position(5, 1);
            var to = new Position(7, 1);
            var rook = new Position(8, 1);
            var enmy = new Position(8, 3);
            var white = new Player(1);

            var s = new ChessState();
            s[rook] = new ChessPiece { Type = PieceTypes.Rook, Player = white };
            s[rook].Move(rook);
            s[from] = new ChessPiece { Type = PieceTypes.King, Player = white };
            s[from].Move(from);
            s[enmy] = new ChessPiece { Type = PieceTypes.Rook, Player = new Player(2) };
            s[enmy].Move(enmy);
            ChessPieceFactory.AddMoves((ChessPiece)s[enmy]);

            Assert.IsTrue(c.CanApply(from, to, s));
        }

        [TestMethod]
        public void CastlingBarrier()
        {
            var c = new CastlingTemplate(CastlingTemplate.CastlingType.Kingside);

            var from = new Position(5, 1);
            var to = new Position(7, 1);
            var rook = new Position(8, 1);
            var enmy = new Position(6, 1);
            var white = new Player(1);

            var s = new ChessState();
            s[rook] = new ChessPiece { Type = PieceTypes.Rook, Player = white };
            s[rook].Move(rook);
            s[from] = new ChessPiece { Type = PieceTypes.King, Player = white };
            s[from].Move(from);
            s[enmy] = new ChessPiece { Type = PieceTypes.Rook, Player = white };
            s[enmy].Move(enmy);

            Assert.IsFalse(c.CanApply(from, to, s));
        }

        [TestMethod]
        public void CastlingBarrierQ()
        {
            var c = new CastlingTemplate(CastlingTemplate.CastlingType.Queenside);

            var from = new Position(5, 1);
            var to = new Position(3, 1);
            var rook = new Position(1, 1);
            var enmy = new Position(2, 1);
            var white = new Player(1);

            var s = new ChessState();
            s[rook] = new ChessPiece { Type = PieceTypes.Rook, Player = white };
            s[rook].Move(rook);
            s[from] = new ChessPiece { Type = PieceTypes.King, Player = white };
            s[from].Move(from);
            s[enmy] = new ChessPiece { Type = PieceTypes.Rook, Player = white };
            s[enmy].Move(enmy);

            Assert.IsFalse(c.CanApply(from, to, s));
        }
    }
}
