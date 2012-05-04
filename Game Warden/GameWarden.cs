using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameWarden
{
    public class Meta
    {
        Dictionary<String, String> info;

        public Meta()
        {
            info = new Dictionary<String, String>();
        }

        public String this[String key]
        {
            get
            {
                if (info.ContainsKey(key))
                {
                    return info[key];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (info.ContainsKey(key))
                {
                    info[key] = value;
                }
                else
                {
                    info.Add(key, value);
                }
            }
        }
    }

    public abstract class Game<T> where T : GameState
    {
        private IEnumerator<concreteMove> Mover;

        public Meta Info;

        public List<concreteMove> Moves;

        protected List<T> States;

        public virtual T CurrentState
        {
            get
            {
                return States.Last();
            }

            private set { }
        }

        public Game(int maxPlayers)
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

    
    public interface IPiece
    {
        Player Player { get; set; }
        Position Pos  { get; set; }
        void AddPossibleMove(templateMove move);
        Boolean IsEmpty { get; }
        Boolean CanMove(Position to, IGameState state);
        templateMove GetPossibleMove(Position to, IGameState state);
    }

    public class EmptySquare : IPiece
    {
        public Player Player
        {
            get { return null; }
            set { }
        }

        public Position Pos
        {
            get { return null; }
            set { }
        }

        public void AddPossibleMove(templateMove move)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty
        {
            get { return true; }
        }

        public bool CanMove(Position to, IGameState state)
        {
            return false;
        }

        public templateMove GetPossibleMove(Position to, IGameState state)
        {
            return null;
        }
    }

    public class Piece : IPiece
    {
        public Player Player { get; set; }
        public Position Pos  { get; set; }

        public List<templateMove> PossibleMoves = new List<templateMove>();
        
        public void AddPossibleMove(templateMove move)
        {
            PossibleMoves.Add(move);
        }

        public bool IsEmpty
        {
            get { return false; }
        }

        public Boolean CanMove(Position to, IGameState state)
        {
            return PossibleMoves.Any(m => m.CanApply(Pos, to, state));
        }

        public templateMove GetPossibleMove(Position to, IGameState state)
        {
            return PossibleMoves.FirstOrDefault(m => m.CanApply(Pos, to, state));
        }
    }

    

    public abstract class GameState : IGameState
    {
        protected IPiece[,] Board;

        protected GameState(int dimX, int dimY)
        {
            Board = new IPiece[dimX, dimY];
            for (int i = 0; i < dimX; ++i)
                for (int j = 0; j < dimY; ++j)
                    Board[i,j] = new EmptySquare();
        }

        protected IPiece this[int file, int rank]
        {
            get
            {
                return Board[file - 1, rank - 1];
            }

            set
            {
                Board[file - 1, rank - 1] = value;
            }
        }

        public IPiece this[Position index]
        {
            get
            {
                try
                {
                    return this[(int)index.File, (int)index.Rank];
                }
                catch
                {
                    throw new ArgumentNullException();
                }
            }

            set
            {
                try
                {
                    this[(int)index.File, (int)index.Rank] = value;
                }
                catch
                {
                    throw new ArgumentNullException();
                }
            }
        }

        public void PlacePiece(Position pos, IPiece p)
        {
            this[pos] = p;
            p.Pos = pos;
        }

        public void RemovePiece(Position pos)
        {
            this[pos].Pos = null;
            this[pos] = new EmptySquare();
        }

        public void MovePiece(Position from, Position to)
        {
            this[to] = this[from];
            this[to].Pos = to;
            this[from] = new EmptySquare();
        }
    }

    public interface IGameState
    {
        void PlacePiece(Position pos, IPiece p);
        void RemovePiece(Position pos);
        void MovePiece(Position from, Position to);
        IPiece this[Position index] { get; set; }
    }

    public class Player
    {
        public int Order;

        public Player(int order)
        {
            Order = order;
        }
    }

    public abstract class templateMove
    {
        public Player Player;

        abstract public void Apply(Position from, Position to, IGameState state);
        abstract public bool CanApply(Position from, Position to, IGameState state);
    }
    
    public abstract class concreteMove
    {
        public Player Player;
        public Position From;
        public Position To;
        protected templateMove Move = null;

        public virtual void Apply(IGameState state)
        {
            Move.Apply(From, To, state);
        }
        public virtual bool CanApply(IGameState state)
        {
            return Move.CanApply(From, To, state);
        }
    }
    
    public class Position
    {
        public int? File;
        public int? Rank;

        /// <summary>
        /// !!!
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            if (obj != null)
            {
                try
                {
                    var pos = obj as Position;
                    return (File == pos.File || pos.File == null || File == null) && (Rank == pos.Rank || pos.Rank == null || Rank == null);
                }
                catch
                {
                    throw new ArgumentException();
                }
            }

            return false;
        }

        protected Char FileLetter
        {
            get
            {
                switch (File)
                {
                    case 1:
                        return 'a';
                    case 2:
                        return 'b';
                    case 3:
                        return 'c';
                    case 4:
                        return 'd';
                    case 5:
                        return 'e';
                    case 6:
                        return 'f';
                    case 7:
                        return 'g';
                    case 8:
                        return 'h';
                }
                throw new ArgumentException();
            }
        }

        public Position(int? file = null, int? rank = null)
        {
            File = file;
            Rank = rank;
        }

        public Position(String s)
        {
            File = GetFile(s.ToLower()[0]);
            Rank = GetRank(s[1]);
        }
        // !!!
        public static implicit operator Position(String s)
        {
            return new Position(s);
        }
        
        public static int GetFile(Char c)
        {
            switch (c)
            {
                case 'a': return 1;
                case 'b': return 2;
                case 'c': return 3;
                case 'd': return 4;
                case 'e': return 5;
                case 'f': return 6;
                case 'g': return 7;
                case 'h': return 8;
            }
            throw new ArgumentException();
        }
        public static int GetRank(Char c)
        {
            try
            {
                return (int)Char.GetNumericValue(c);
            }
            catch
            {
                throw new ArgumentException();
            }
        }

        public static int RankDistance(Position from, Position to)
        {
            try
            {
                return Math.Abs(from.Rank.Value - to.Rank.Value);
            }
            catch
            {
                throw new ArgumentNullException();
            }
        }
        public static int FileDistance(Position from, Position to)
        {
            try
            {
                return Math.Abs(from.File.Value - to.File.Value);
            }
            catch
            {
                throw new ArgumentNullException();
            }
        }

        public override string ToString()
        {
            return String.Format("{0}{1}", FileLetter, Rank);
        }
    }
}
