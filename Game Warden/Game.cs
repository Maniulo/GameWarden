using System.Collections.Generic;
using System.Linq;

namespace GameWarden
{
    public abstract class Game<T, TK> : IEnumerable<T> where T : GameState where TK: ConcreteMove
    {
        private int Mover = 0;

        public Meta Info;

        public List<TK> Moves;

        protected List<T> States;

        public virtual T CurrentState
        {
            get
            {
                return States.Last();
            }
        }

        protected Game(int maxPlayers)
        {
            Info = new Meta();
            States = new List<T>();
            Players = new List<Player>();

            for (var i = 0; i < maxPlayers; ++i)
                Players.Add(new Player(i + 1));
        }

        public List<Player> Players;

        public bool MakeMove()
        {
            if (Mover < Moves.Count)
            {
                Moves[Mover++].Apply(CurrentState);
                return true;
            }

            return false;
        }

        public bool UndoMove()
        {
            if (Mover > 0)
            {
                Moves[--Mover].Rollback(CurrentState);
                return true;
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return States.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return States.GetEnumerator();
        }
    }
}