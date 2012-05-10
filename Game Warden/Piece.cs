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
        void AddPossibleMove(TemplateMove move);
        Boolean CanMove(Position to, IGameState state);
        TemplateMove GetPossibleMove(Position to, IGameState state);
    }

    public class Piece : IPiece
    {
        public Player Player { get; set; }
        public Position Pos  { get; private set; }

        public List<TemplateMove> PossibleMoves = new List<TemplateMove>();
        
        public void AddPossibleMove(TemplateMove move)
        {
            PossibleMoves.Add(move);
        }

        public virtual void Move(Position pos)
        {
            Pos = pos;
        }

        private bool Empty;
        public bool IsEmpty
        {
            get { return Empty; }
            set
            {
                //if (value == true)
                    Empty = value;
                //else
                //    throw new Exception("Square cannot be marked as not empty at runtime.");
            }
        }

        public Piece() { }

        public Piece(Piece copy)
        {
            IsEmpty = copy.IsEmpty;
            Player = copy.Player;
            Pos = new Position(copy.Pos);
            PossibleMoves = new List<TemplateMove>();
            foreach (TemplateMove m in copy.PossibleMoves)
                PossibleMoves.Add(m);
        }

        public Boolean CanMove(Position to, IGameState state)
        {
            return PossibleMoves.Any(m => m.CanApply(Pos, to, state));
        }

        public Boolean CanAttack(Position to, IGameState state)
        {
            return PossibleMoves.Any(m => m.CanApply(Pos, to, state) && m.IsCapture);
        }

        public TemplateMove GetPossibleMove(Position to, IGameState state)
        {
            return PossibleMoves.FirstOrDefault(m => m.CanApply(Pos, to, state));
        }
    }
}