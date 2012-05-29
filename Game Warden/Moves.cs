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

    public abstract class BaseConcreteMove : IConcreteMove
    {
        protected void MovePiece(Position from, Position to, IGameState state)
        {
            state[to] = state[@from];
            state[to].Move(to);
            state.NewEmptyPiece(from);
        }

        protected void RollbackMovePiece(Position from, Position to, IGameState state)
        {
            state[from] = state[to];
            state[from].Unmove();
            state.NewEmptyPiece(to);
        }

        protected void RemovePiece(Position from, IGameState state)
        {
            state.NewEmptyPiece(from);
        }

        protected void RollbackRemovePiece(IPiece p, IGameState state)
        {
            state[p.Pos] = p;
        }

        public abstract void Apply(IGameState state);

        public abstract void Rollback(IGameState state);
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

        public TemplateMove(int? maxLength = null, bool? capture = null, Boolean pathCheck = true)
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
            return new ConcreteMove(from, to);
        }
    }

    public class ConcreteMove : BaseConcreteMove
    {
        private readonly Position From;
        private readonly Position To;
        private IPiece CapturedPiece;

        public ConcreteMove(Position from, Position to)
        {
            From = from;
            To = to;
        }

        public override void Apply(IGameState state)
        {
            CapturedPiece = state[To];
            MovePiece(From, To, state);
        }

        public override void Rollback(IGameState state)
        {
            RollbackMovePiece(From, To, state);
            RollbackRemovePiece(CapturedPiece, state);
        }
    }
}