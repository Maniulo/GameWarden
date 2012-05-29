using System;
using GameWarden;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTesting
{
    [TestClass]
    public class BasicMovesTests
    {
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
        public void TemplateMoveNoRestrictions()
        {
            var m = new MockTemplate();
            var s = new GameState(5, 5);
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(1, 2), s));
        }

        [TestMethod]
        public void TemplateMoveNoMaxLength()
        {
            var m = new MockTemplate(null);
            var s = new GameState(5, 5);
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(3, 4), s));
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(3, 5), s));
        }

        [TestMethod]
        public void TemplateMoveMaxLength()
        {
            var m = new MockTemplate(1);
            var s = new GameState(5, 5);
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(3, 4), s));
            Assert.IsFalse(m.CanApply(new Position(3, 3), new Position(3, 5), s));
        }

        [TestMethod]
        public void TemplateMoveCaptureDefault()
        {
            var m = new MockTemplate(null);
            var s = new GameState(5, 5);
            s[new Position(5, 5)]  = new Piece();
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(5, 5), s));
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(4, 4), s));
        }

        [TestMethod]
        public void TemplateMoveCapture()
        {
            var m = new MockTemplate(null, true);
            var s = new GameState(5, 5);
            s[new Position(5, 5)] = new Piece();
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(5, 5), s));
            Assert.IsFalse(m.CanApply(new Position(3, 3), new Position(4, 4), s));
        }

        [TestMethod]
        public void TemplateMoveNotCapture()
        {
            var m = new MockTemplate(null, false);
            var s = new GameState(5, 5);
            s[new Position(5, 5)] = new Piece();
            Assert.IsFalse(m.CanApply(new Position(3, 3), new Position(5, 5), s));
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(4, 4), s));
        }

        [TestMethod]
        public void TemplateMovePathCheck()
        {
            var m = new MockTemplate(null, null, true);
            var s = new GameState(5, 5);
            s[new Position(3, 4)] = new Piece();
            Assert.IsFalse(m.CanApply(new Position(3, 3), new Position(3, 5), s));
        }

        [TestMethod]
        public void TemplateMoveNoPathCheck()
        {
            var m = new MockTemplate(null, null, false);
            var s = new GameState(5, 5);
            s[new Position(3, 4)] = new Piece();
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(3, 5), s));
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(5, 3), s));
        }

        [TestMethod]
        public void VerticalMove()
        {
            var m = new VerticalMoveTemplate();
            var s = new GameState(2, 2);
            Assert.IsTrue(m.CanApply(new Position(1, 1), new Position(1, 2), s));
        }

        [TestMethod]
        public void VerticalMovePathCheck()
        {
            var m = new VerticalMoveTemplate();
            var s = new GameState(3, 3);
            s[new Position(1, 2)] = new Piece();
            Assert.IsFalse(m.CanApply(new Position(1, 1), new Position(1, 3), s));
        }

        [TestMethod]
        public void VerticalMoveBackwards()
        {
            var m = new VerticalMoveTemplate();
            var s = new GameState(2, 2);
            Assert.IsTrue(m.CanApply(new Position(1, 2), new Position(1, 1), s));
        }

        [TestMethod]
        public void VerticalMoveNotHorizontal()
        {
            var m = new VerticalMoveTemplate();
            var s = new GameState(2, 2);
            Assert.IsFalse(m.CanApply(new Position(1, 1), new Position(2, 1), s));
        }

        [TestMethod]
        public void VerticalMoveNotDiagonal()
        {
            var m = new VerticalMoveTemplate();
            var s = new GameState(2, 2);
            Assert.IsFalse(m.CanApply(new Position(1, 1), new Position(2, 2), s));
        }

        [TestMethod]
        public void HorizontalMove()
        {
            var m = new HorizontalMoveTemplate();
            var s = new GameState(2, 2);
            Assert.IsTrue(m.CanApply(new Position(1, 1), new Position(2, 1), s));
        }

        [TestMethod]
        public void HorizontalMovePathCheck()
        {
            var m = new HorizontalMoveTemplate();
            var s = new GameState(3, 3);
            s[new Position(2, 1)] = new Piece();
            Assert.IsFalse(m.CanApply(new Position(1, 1), new Position(3, 1), s));
        }
        
        [TestMethod]
        public void HorizontalMoveBackwards()
        {
            var m = new HorizontalMoveTemplate();
            var s = new GameState(2, 2);
            Assert.IsTrue(m.CanApply(new Position(2, 1), new Position(1, 1), s));
        }
        
        [TestMethod]
        public void HorizontalMoveNotVertical()
        {
            var m = new HorizontalMoveTemplate();
            var s = new GameState(2, 2);
            Assert.IsFalse(m.CanApply(new Position(1, 1), new Position(1, 2), s));
        }

        [TestMethod]
        public void HorizontalMoveNotDiagonal()
        {
            var m = new HorizontalMoveTemplate();
            var s = new GameState(2, 2);
            Assert.IsFalse(m.CanApply(new Position(1, 1), new Position(2, 2), s));
        }

        [TestMethod]
        public void DiagonalMoveUpRight()
        {
            var m = new DiagonalMoveTemplate();
            var s = new GameState(5, 5);
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(5, 5), s));
        }

        [TestMethod]
        public void DiagonalMoveUpLeft()
        {
            var m = new DiagonalMoveTemplate();
            var s = new GameState(5, 5);
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(1, 5), s));
        }

        [TestMethod]
        public void DiagonalMoveDownRight()
        {
            var m = new DiagonalMoveTemplate();
            var s = new GameState(5, 5);
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(5, 1), s));
        }

        [TestMethod]
        public void DiagonalMoveDownLeft()
        {
            var m = new DiagonalMoveTemplate();
            var s = new GameState(5, 5);
            Assert.IsTrue(m.CanApply(new Position(3, 3), new Position(1, 1), s));
        }

        [TestMethod]
        public void DiagonalMovePathCheck()
        {
            var m = new DiagonalMoveTemplate();
            var s = new GameState(3, 3);
            s[new Position(2, 2)] = new Piece();
            Assert.IsFalse(m.CanApply(new Position(1, 1), new Position(3, 3), s));
        }

        [TestMethod]
        public void DiagonalMoveError()
        {
            var m = new DiagonalMoveTemplate();
            var s = new GameState(5, 5);
            Assert.IsFalse(m.CanApply(new Position(3, 3), new Position(1, 2), s));
        }
    }
}
