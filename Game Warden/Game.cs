using System.Collections.Generic;
using System.Linq;

namespace GameWarden
{
    public abstract class Game<T, TK> where T : GameState where TK: ConcreteMove
    {
        private IEnumerator<TK> Mover;

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
            if (Mover == null)
                Mover = Moves.GetEnumerator();

            if (Mover.MoveNext())
            {
                Mover.Current.Apply(CurrentState);
                return true;
            }
            return false;
        }
    }
}