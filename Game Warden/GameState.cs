using System;

namespace GameWarden
{
    public interface IGameState
    {
        void PlacePiece(Position pos, IPiece p);
        void RemovePiece(Position pos);
        void MovePiece(Position from, Position to);
        IPiece this[Position index] { get; set; }
    }

    public abstract class GameState : IGameState
    {
        protected IPiece[,] Board;

        protected GameState(int dimX, int dimY)
        {
            Board = new IPiece[dimX, dimY];

            for (int i = 0; i < dimX; ++i)
                for (int j = 0; j < dimY; ++j)
                    Board[i,j] = new Piece(true);
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
                    return this[index.File, index.Rank];
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
                    this[index.File, index.Rank] = value;
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
            this[pos] = new Piece(true);
        }

        public void MovePiece(Position from, Position to)
        {
            this[to] = this[from];
            this[to].Pos = to;
            this[from] = new Piece(true);
        }
    }
}