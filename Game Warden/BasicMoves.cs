using System;

namespace GameWarden
{
    public class VerticalMove : Move
    {
        public VerticalMove(int? maxLength = null, bool? capture = null, Boolean pathCheck = true)
            : base (maxLength, capture, pathCheck) { }

        public override bool CanApply(Position from, Position to, IGameState state)
        {
            if (from.File == to.File)
            {
                for (int rank = Math.Min(from.Rank, to.Rank) + 1; rank < Math.Max(from.Rank, to.Rank); ++rank)
                    Path.Add(new Position(from.File, rank));

                return base.CanApply(from, to, state);
            }

            return false;
        }
    }

    public class HorizontalMove : Move
    {
        public HorizontalMove(int? maxLength = null, bool? capture = null, Boolean pathCheck = true)
            : base(maxLength, capture, pathCheck) { }

        public override bool CanApply(Position from, Position to, IGameState state)
        {
            if (from.Rank == to.Rank)
            {
                for (int file = Math.Min(from.File, to.File) + 1; file < Math.Max(from.File, to.File); ++file)
                    Path.Add(new Position(file, from.Rank));

                return base.CanApply(from, to, state);
            }

            return false;
        }
    }

    public class DiagonalMove : Move
    {
        public DiagonalMove(int? maxLength = null, bool? capture = null, Boolean pathCheck = true)
            : base(maxLength, capture, pathCheck) { }

        public override bool CanApply(Position from, Position to, IGameState state)
        {
            if (Position.FileDistance(from, to) == Position.RankDistance(from, to))
            {
                int file, rank, maxFile;

                // left to right
                if (from.File < to.File)
                {
                    file = from.File + 1;
                    maxFile = to.File;

                    // bottom to top
                    if (from.Rank < to.Rank)
                        for (rank = from.Rank + 1; file < maxFile; ++file, ++rank)
                            Path.Add(new Position(file, rank));
                    // top to bottom
                    else
                        for (rank = from.Rank - 1; file < maxFile; ++file, --rank)
                            Path.Add(new Position(file, rank));
                }
                // right to left
                else
                {
                    file = to.File + 1;
                    maxFile = from.File;

                    // bottom to up
                    if (from.Rank < to.Rank)
                        for (rank = to.Rank - 1; file < maxFile; ++file, --rank)
                            Path.Add(new Position(file, rank));
                    // up to bottom
                    else
                        for (rank = to.Rank + 1; file < maxFile; ++file, ++rank)
                            Path.Add(new Position(file, rank));
                }
                return base.CanApply(from, to, state);
            }

            return false;
        }
    }
}