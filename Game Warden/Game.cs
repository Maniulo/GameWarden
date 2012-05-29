using System.Collections.Generic;

namespace GameWarden
{
    public abstract class Game
    {
        public Meta Info;

        private readonly List<IConcreteMove> moves = new List<IConcreteMove>();
        public IEnumerable<IConcreteMove> Moves
        {
            get { return new List<IConcreteMove>(moves); }
        }
        public int CurrentMove { get; private set; }
        public int MovesCount
        {
            get { return moves.Count; }
        }
        public IGameState State;
        public List<Player> Players = new List<Player>();

        protected Game()
        {
            CurrentMove = 0;
        }

        public void AddMove(IConcreteMove m)
        {
            moves.Add(m);
        }
        public bool MakeMove()
        {
            if (CurrentMove < MovesCount)
            {
                moves[CurrentMove++].Apply(State);
                return true;
            }

            return false;
        }
        public bool UndoMove()
        {
            if (CurrentMove > 0)
            {
                moves[--CurrentMove].Rollback(State);
                return true;
            }

            return false;
        }
    }
}