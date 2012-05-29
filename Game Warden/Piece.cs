using System;
using System.Collections.Generic;
using System.Linq;

namespace GameWarden
{
    public interface IPiece
    {
        Boolean IsEmpty { get; }
        Player Player { get; set; }
        Position Pos { get; }
        void Move(Position pos);
        void Unmove();
        void AddPossibleMove(ITemplateMove move);
        void ResetPossibleMoves();
    }

    public class Piece : IPiece
    {
        public Stack<Position> Path;

        public Player Player { get; set; }
        public Position Pos  { get; private set; }

        public List<ITemplateMove> PossibleMoves = new List<ITemplateMove>();
        
        public void AddPossibleMove(ITemplateMove move)
        {
            PossibleMoves.Add(move);
        }

        public virtual void Move(Position pos)
        {
            Path.Push(new Position(pos)); // !!! ???
            Pos = Path.Peek();
        }
        
        public virtual void Unmove()
        {
            Path.Pop();
            Pos = Path.Peek();
        }

        public bool IsEmpty { get; set; }

        public Piece()
        {
            Path = new Stack<Position>();
        }

        public Piece(Piece copy)
        {
            IsEmpty = copy.IsEmpty;
            Player = copy.Player;
            if (copy.Pos != null)
                Pos = new Position(copy.Pos);

            PossibleMoves = new List<ITemplateMove>(copy.PossibleMoves);
            foreach (var m in copy.PossibleMoves)
                PossibleMoves.Add(m);

            Path = new Stack<Position>();
            foreach (Position pos in copy.Path)
                Path.Push(new Position(pos));
        }

        public virtual Boolean CanMove(Position to, IGameState state)
        {
            return PossibleMoves.Any(m => m.CanApply(Pos, to, state));
        }

        public virtual Boolean CanAttack(Position to, IGameState state)
        {
            return PossibleMoves.Any(m => m.IsCapture && m.CanApply(Pos, to, state));
        }

        public virtual IConcreteMove GetPossibleMove(Position to, IGameState state)
        {
            try
            {
                return PossibleMoves.FirstOrDefault(m => m.CanApply(Pos, to, state)).Concretize(Pos, to);
            }
            catch
            {
                throw new Exception("No possible move found.");
            }
        }

        public void ResetPossibleMoves()
        {
            PossibleMoves.Clear();
        }
    }
}