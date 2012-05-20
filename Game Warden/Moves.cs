using System;
using System.Collections.Generic;
using System.Linq;

namespace GameWarden
{
    public interface ITemplateMove
    {
        Boolean IsCapture { get; }
        bool CanApply(Position from, Position to, IGameState state);
        IConcreteMove Concretize(Position from, Position to);
    }

    public interface IConcreteMove
    {
        void Apply(IGameState state);
        void Rollback(IGameState state);
    }

    public abstract class TemplateMove : ITemplateMove
    {
        protected bool? Capture;
        protected int? MaxLength;
        protected Boolean PathCheck;
        
        public Boolean IsCapture
        {
            get { return Capture.HasValue ? Capture.Value : true; }
        }

        public Player Player { get; set; }

        protected List<Position> Path = new List<Position>();

        protected TemplateMove(int? maxLength = null, bool? capture = null, Boolean pathCheck = true)
        {
            Capture = capture;
            PathCheck = pathCheck;
            MaxLength = maxLength;
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

        public virtual bool CanApply(Position from, Position to, IGameState state)
        {
            Boolean result =
                CheckBarriers(state) &&
                CheckCapture(to, state) &&
                CheckLength();
            Path.Clear();

            return result;
        }

        public virtual IConcreteMove Concretize(Position from, Position to)
        {
            return new ConcreteMove(from, to, IsCapture);
        }
    }

    public class ConcreteMove : IConcreteMove
    {
        private readonly Position From;
        private readonly Position To;
        private readonly Boolean Capture;
        private IPiece CapturedPiece;

        public ConcreteMove(Position from, Position to, Boolean capture)
        {
            From = from;
            To = to;
            Capture = capture;
        }

        public virtual void Apply(IGameState state)
        {
            if (Capture)
                CapturedPiece = state[To];
            state.MovePiece(From, To);
        }

        public virtual void Rollback(IGameState state)
        {
            state.MovePieceN(From, To);
            if (Capture)
                state.RemovePieceN(CapturedPiece);
        }
    }
}