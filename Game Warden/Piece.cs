using System;
using System.Collections.Generic;

namespace GameWarden
{
    public interface IPiece
    {
        List<ITemplateMove> 
                 PossibleMoves { get; }
        Boolean  IsEmpty { get; }
        Player   Player { get; set; }
        Position Pos { get; }

        void Move(Position pos);
        void Unmove();
    }

    public class Piece : IPiece
    {
        private readonly Stack<Position> path;
        public IEnumerable<Position> Path { get { return path; } }
        public int PathLength { get { return path.Count; } }
        public List<ITemplateMove> 
                        PossibleMoves { get; private set; }
        public Boolean  IsEmpty { get; set; }
        public Player   Player { get; set; }
        public Position Pos  { get; private set; }
        
        public Piece()
        {
            PossibleMoves = new List<ITemplateMove>();
            path = new Stack<Position>();
        }
        public Piece(Piece copy)
        {
            IsEmpty = copy.IsEmpty;
            Player = copy.Player;
            Pos = copy.Pos;
            PossibleMoves = new List<ITemplateMove>(copy.PossibleMoves);
            path = new Stack<Position>(copy.Path);
        }

        public virtual void Move(Position pos)
        {
            path.Push(pos);
            Pos = path.Peek();
        }
        public virtual void Unmove()
        {
            path.Pop();
            Pos = path.Count > 0 ? path.Peek() : null;
        }
    }
}