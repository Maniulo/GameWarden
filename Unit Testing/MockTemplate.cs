using System;
using GameWarden;

namespace UnitTesting
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
}