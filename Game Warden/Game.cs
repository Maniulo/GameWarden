using System;
using System.Collections.Generic;
using System.Linq;

namespace GameWarden
{
    public abstract class Game
    {
        public static int DimX { get { throw new NotImplementedException(); }  }
        public static int DimY  { get { throw new NotImplementedException(); }  }

        private int Mover = 0;

        public Meta Info;

        public List<IConcreteMove> Moves = new List<IConcreteMove>();

        public IGameState State;

        protected Game(int maxPlayers)
        {
            Info = new Meta();
            Players = new List<Player>();

            for (var i = 0; i < maxPlayers; ++i)
                Players.Add(new Player(i + 1));
        }

        public List<Player> Players;

        public bool MakeMove()
        {
            if (Mover < Moves.Count)
            {
                Moves[Mover++].Apply(State);
                return true;
            }

            return false;
        }

        public bool UndoMove()
        {
            if (Mover > 0)
            {
                Moves[--Mover].Rollback(State);
                return true;
            }

            return false;
        }
    }
}