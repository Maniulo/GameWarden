using System;
using GameWarden;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class BasicConcreteMovesTests
    {
        [TestMethod]
        public void SimpleMove()
        {
            var s = new GameState(5, 5);
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            s.PlacePiece(from, new Piece());
            var movingPiece = s[from];
            var m = new ConcreteMove(from, to, false);

            m.Apply(s);

            Assert.IsTrue(s[from].IsEmpty);
            Assert.AreNotEqual(s[from], movingPiece);
            Assert.AreEqual(s[to], movingPiece);
        }

        [TestMethod]
        public void CaptureMove()
        {
            var s = new GameState(5, 5);
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            s.PlacePiece(from, new Piece());
            var movingPiece = s[from];
            s.PlacePiece(to, new Piece());
            var m = new ConcreteMove(from, to, true);
            
            m.Apply(s);

            Assert.AreEqual(s[to], movingPiece);
        }

        [TestMethod]
        public void Rollback()
        {
            var s = new GameState(5, 5);
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            s.PlacePiece(from, new Piece());
            var movingPiece = s[from];
            var m = new ConcreteMove(from, to, false);

            m.Apply(s);
            m.Rollback(s);

            Assert.AreEqual(s[from], movingPiece);
            Assert.AreNotEqual(s[to], movingPiece);
        }

        [TestMethod]
        public void RollbackCapture()
        {
            var s = new GameState(5, 5);
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            s.PlacePiece(from, new Piece());
            var movingPiece = s[from];
            var capturedPiece = s[to];
            var m = new ConcreteMove(from, to, true);

            m.Apply(s);
            m.Rollback(s);

            Assert.AreEqual(s[from], movingPiece);
            Assert.AreEqual(s[to], capturedPiece);
        }

        class MockTemplate : TemplateMove
        {
            public MockTemplate(int? maxLength = null, bool? capture = null, Boolean pathCheck = true)
                : base(maxLength, capture, pathCheck) { }

            public override bool CanApply(Position from, Position to, IGameState state)
            {
                for (int rank = Math.Min(from.Rank, to.Rank) + 1; rank < Math.Max(from.Rank, to.Rank); ++rank)
                    Path.Add(new Position(from.File, rank));

                for (int file = Math.Min(from.File, to.File) + 1; file < Math.Max(from.File, to.File); ++file)
                    Path.Add(new Position(file, from.Rank));

                return base.CanApply(from, to, state);
            }
        }

        [TestMethod]
        public void GetConcreteFromTemplate()
        {
            var s = new GameState(5, 5);
            var from = new Position(1, 1);
            var to = new Position(2, 2);
            s.PlacePiece(from, new Piece());
            var movingPiece = s[from];
            var m = new MockTemplate().Concretize(from, to);

            m.Apply(s);

            Assert.IsTrue(s[from].IsEmpty);
            Assert.AreNotEqual(s[from], movingPiece);
            Assert.AreEqual(s[to], movingPiece);
        }
    }
}
