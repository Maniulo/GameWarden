﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GameWarden
{
    public abstract class TemplateMove
    {
        public abstract Boolean IsCapture { get; }
        public Player Player { get; set; }
        public abstract bool CanApply(Position from, Position to, IGameState state);
        public abstract void Apply(Position@from, Position to, IGameState state);
        public abstract void Rollback(Position from, Position to, IGameState state);
    }
     
    public abstract class ConcreteMove
    {
        public Player Player;
        public Position From;
        public Position To;
        protected TemplateMove Move = null;
        private IPiece CapturedPiece;

        public virtual void Rollback(IGameState state)
        {
            Move.Rollback(From, To, state);
            
            if (Move.IsCapture)
                state.RemovePieceN(To, CapturedPiece);
        }

        public virtual void Apply(IGameState state)
        {
            if (Move.IsCapture)
                CapturedPiece = state[To];

            Move.Apply(From, To, state);
        }

        public virtual bool CanApply(IGameState state)
        {
            return Move.CanApply(From, To, state);
        }
    }

    public abstract class Move : TemplateMove
    {
        protected bool? Capture;
        protected int? MaxLength;
        protected Boolean PathCheck;

        public override Boolean IsCapture
        {
            get { return Capture.HasValue ? Capture.Value : true; }
        }

        protected List<Position> Path;

        protected Move(int? maxLength = null, bool? capture = null, Boolean pathCheck = true)
        {
            Path = new List<Position>();
            Capture = capture;
            PathCheck = pathCheck;
            MaxLength = maxLength;
        }

        public override void Apply(Position from, Position to, IGameState state)
        {
            state.MovePiece(from, to);
        }

        public override void Rollback(Position from, Position to, IGameState state)
        {
            state.MovePieceN(from, to);
        }

        private bool CheckBarriers(IGameState state)
        {
            // The piece either can leap over other pieces or we have to check that its path is empty
            return !PathCheck || Path.Aggregate(true, (noBarriers, p) => noBarriers && state[p].IsEmpty);
        }

        private bool CheckCapture(Position to, IGameState state)
        {
            // If this is a capture then destination square is not empty
            return !Capture.HasValue || Capture.Value ^ state[to].IsEmpty;
        }

        private bool CheckLength()
        {
            // Path should be less or equal than MaxLegth if it has value
            // (+1 is for destination square which is not in the path)
            return !MaxLength.HasValue || Path.Count + 1 <= MaxLength.Value;
        }

        public override bool CanApply(Position from, Position to, IGameState state)
        {
            Boolean result =
                CheckBarriers(state) &&
                CheckCapture(to, state) &&
                CheckLength();
            Path.Clear();
            return result;
        }
    }
}
