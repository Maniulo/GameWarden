using System;
using System.Collections.Generic;
using System.Linq;

namespace GameWarden
{
    public interface IPiece
    {
        Boolean IsEmpty { get; }
        Player Player { get; set; }
        Position Pos { get; set; }
        void AddPossibleMove(TemplateMove move);
        Boolean CanMove(Position to, IGameState state);
        TemplateMove GetPossibleMove(Position to, IGameState state);
    }

    public class Piece : IPiece
    {
        public Player Player { get; set; }
        public Position Pos  { get; set; }

        public List<TemplateMove> PossibleMoves = new List<TemplateMove>();
        
        public void AddPossibleMove(TemplateMove move)
        {
            PossibleMoves.Add(move);
        }

        private readonly bool Empty;
        public bool IsEmpty { get { return Empty; } }

        public Piece(bool isEmptySquare = false)
        {
            Empty = isEmptySquare;
        }

        public Boolean CanMove(Position to, IGameState state)
        {
            return PossibleMoves.Any(m => m.CanApply(Pos, to, state));
        }

        public TemplateMove GetPossibleMove(Position to, IGameState state)
        {
            return PossibleMoves.FirstOrDefault(m => m.CanApply(Pos, to, state));
        }
    }
}