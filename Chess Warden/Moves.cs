using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameWarden.Chess
{
    public class UnsolvedMove : concreteMove
    {
        public PieceTypes PieceType;
        public String Desc;
        
        protected virtual templateMove Solve(ChessState state)
        {
            foreach (IPiece ip in state)
                if (!ip.IsEmpty)
                {
                    var p = ip as ChessPiece;
                    if (p.Type == PieceType && p.Player == Player && From.Equals(p.Pos) && p.CanMove(To, state))
                    {
                        this.From = p.Pos;
                        return p.GetPossibleMove(To, state);
                    }
                }

            return null;
        }

        public UnsolvedMove(String desc)
        {
            Desc = desc;
        }

        public override string ToString()
        {
            return Desc;
        }

        public override void Apply(IGameState state)
        {
            if (Move == null)
                Move = Solve((ChessState)state);  // !!!

            if (Move == null)
                throw new Exception(String.Format("Move \"{0}\" cannot be solved.", Desc));

            base.Apply(state);
        }

        public override bool CanApply(IGameState state)
        {
            if (Move == null)
                Move = Solve((ChessState)state);  // !!!

            if (Move == null)
                throw new Exception(String.Format("Move \"{0}\" cannot be solved.", Desc));

            return base.CanApply(state);
        }
    }

    public abstract class Move : templateMove
    {
        protected bool? Capture;
        protected int? MaxLength;
        protected Boolean CheckBarriers;

        protected List<Position> Path;

        protected Move(Boolean checkBarriers = true, bool? capture = null, int? maxLength = null)
        {
            Path = new List<Position>();
            Capture = capture;
            CheckBarriers = checkBarriers;
            MaxLength = maxLength;
        }

        public override void Apply(Position from, Position to, IGameState state)
        {
            // state[from].Pos = to; // !!!?
            state.MovePiece(from, to);
        }

        public override bool CanApply(Position from, Position to, IGameState state)
        {
            return  
                // The piece either can leap over other pieces and thus we have to check that its path is empty
                // or not
                (!CheckBarriers || Path.Aggregate(true, (current, p) => current && state[p].IsEmpty)) &&
                // This is either capture and thus destination square has a figure
                // or not
                (!Capture.HasValue || Capture.Value ^ state[to].IsEmpty) &&
                // Oh, and the path should be less or equal than MaxLegth if it has value
                // +1 is for destination square which is not in path
                (!MaxLength.HasValue || Path.Count + 1 <= MaxLength.Value);
        }
    }

    public class VerticalMove : Move
    {
        public VerticalMove(Boolean checkBarriers = true, bool? capture = null, int? maxLength = null)
            : base (checkBarriers, capture, maxLength) { }

        public override bool CanApply(Position from, Position to, IGameState state)
        {
            if (from.File == to.File)
            {
                for (int rank = Math.Min(from.Rank.Value, to.Rank.Value) + 1; rank < Math.Max(from.Rank.Value, to.Rank.Value); ++rank)
                    Path.Add(new Position(from.File, rank));

                return base.CanApply(from, to, state);
            }

            return false;
        }
    }

    public class HorizontalMove : Move
    {
        public HorizontalMove(Boolean checkBarriers = true, bool? capture = null, int? maxLength = null)
            : base (checkBarriers, capture, maxLength) { }

        public override bool CanApply(Position From, Position To, IGameState state)
        {
            if (From.Rank == To.Rank)
            {
                for (int file = Math.Min(From.File.Value, To.File.Value) + 1; file < Math.Max(From.File.Value, To.File.Value); ++file)
                    Path.Add(new Position(file, From.Rank));

                return base.CanApply(From, To, state);
            }

            return false;
        }
    }

    public class DiagonalMove : Move
    {
        public DiagonalMove(Boolean checkBarriers = true, bool? capture = null, int? maxLength = null)
            : base (checkBarriers, capture, maxLength) { }

        public override bool CanApply(Position From, Position To, IGameState state)
        {
            if (Position.FileDistance(From, To) == Position.RankDistance(From, To))
            {
                int file, rank, maxFile;

                // left to right
                if (From.File < To.File)
                {
                    file = From.File.Value + 1;
                    maxFile = To.File.Value;
                    
                    // bottom to top
                    if (From.Rank < To.Rank)
                        for (rank = From.Rank.Value + 1; file < maxFile; ++file, ++rank)
                            Path.Add(new Position(file, rank));
                    // top to bottom
                    else
                        for (rank = From.Rank.Value - 1; file < maxFile; ++file, --rank)
                            Path.Add(new Position(file, rank));
                }
                // right to left
                else
                {
                    file = To.File.Value + 1;
                    maxFile = From.File.Value;
                    
                    // bottom to up
                    if (From.Rank < To.Rank)
                        for (rank = To.Rank.Value - 1; file < maxFile; ++file, --rank)
                            Path.Add(new Position(file, rank));
                    // up to bottom
                    else
                        for (rank = To.Rank.Value + 1; file < maxFile; ++file, ++rank)
                            Path.Add(new Position(file, rank));
                }
                return base.CanApply(From, To, state);
            }

            return false;
        }
    }

    public class PawnCapture : Move
    {
        public PawnCapture()
            : base(true, true, 1)
        {

        }

        public override bool CanApply(Position From, Position To, IGameState state)
        {
            if (Position.FileDistance(To, From) == 1)
            {
                switch (state[From].Player.Order)
                {
                    case 1:
                        return To.Rank - From.Rank == 1 && base.CanApply(From, To, state);
                    case 2:
                        return From.Rank - To.Rank == 1 && base.CanApply(From, To, state);
                }
            }

            return false;
        }
    }

    public class PawnMove : VerticalMove
    {
        public PawnMove()
            : base(true, false, 2) { }

        public override bool CanApply(Position From, Position To, IGameState state)
        {
            switch (state[From].Player.Order)
            {
                case 1:
                    if (To.Rank - From.Rank <= 2)
                    {
                        return base.CanApply(From, To, state);
                    }
                    else return false;
                case 2:
                    if (To.Rank - From.Rank >= -2)
                    {
                        return base.CanApply(From, To, state);
                    }
                    else return false;
                default:
                    throw new ArgumentException();
            }
        }
    }

    public class EnPassant : templateMove
    {
        Position EnemyPawn(ChessState state)
        {
            return new Position(state.EnPassant.File, state.EnPassant.Rank - 1);
        }

        public override void Apply(Position From, Position To, IGameState state)
        {
            state.RemovePiece( EnemyPawn((ChessState)state) ); // !!!
            state.MovePiece(From, To);
        }

        public override bool CanApply(Position From, Position To, IGameState state)
        {
            return To.Equals( ((ChessState)state).EnPassant );
        }
    }

    public class KnightMove : Move
    {
        public override bool CanApply(Position From, Position To, IGameState state)
        {
            if (Position.FileDistance(From, To) == 2 && Position.RankDistance(From, To) == 1 ||
                Position.FileDistance(From, To) == 1 && Position.RankDistance(From, To) == 2)
            {
                return base.CanApply(From, To, state);
            }
            else
            {
                return false;
            }
        }
    }

    public class Kingside : Castling
    {
        public Kingside()
        {
            rookFrom = new Position(8, null);
            rookTo = new Position(6, null);
        }
    }

    public class Queenside : Castling
    {
        public Queenside()
        {
            rookFrom = new Position(1, null);
            rookTo = new Position(3, null);
        }
    }

    public abstract class Castling : HorizontalMove
    {
        protected void RefreshRanks(Position From, Position To, IGameState state)
        {
            if (state[From].Player.Order == 1)
            {
                To.Rank = 1;
            }
            else if (state[From].Player.Order == 2)
            {
                To.Rank = 8;
            }

            rookFrom.Rank = From.Rank;
            rookTo.Rank = To.Rank;
        }

        protected Position rookFrom;
        protected Position rookTo;

        public Castling()
            : base(true, false)
        {

        }

        public override void Apply(Position From, Position To, IGameState state)
        {
            RefreshRanks(From, To, state);
            state.MovePiece(From, To);
            state.MovePiece(rookFrom, rookTo);
        }

        public override bool CanApply(Position From, Position To, IGameState state)
        {
            RefreshRanks(From, To, state);
            return base.CanApply(From, To, state);
        }
    }
}
