using System;

namespace GameWarden
{
    public interface IGameState
    {
        void PlacePiece(Position pos, IPiece p);
        void RemovePiece(Position pos);
        void MovePiece(Position from, Position to);

        void PlacePieceN(Position pos);
        void RemovePieceN(IPiece p);
        void MovePieceN(Position from, Position to);

        IPiece this[Position index] { get; }
    }

    public abstract class GameState : IGameState
    {
        protected IPiece[,] Board;

        protected GameState(int dimX, int dimY)
        {
            Board = new IPiece[dimX, dimY];

            for (int file = 0; file < dimX; ++file)
                for (int rank = 0; rank < dimY; ++rank)
                    Board[file, rank] = CreateEmptyPiece(new Position(file + 1, rank + 1));
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
                    throw new Exception();
                }
            }

            private set
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

        public abstract IPiece CreateEmptyPiece(Position pos);

        public void PlacePiece(Position pos, IPiece p)
        {
            this[pos] = p;
            p.Move(pos);
        }

        public void RemovePiece(Position pos)
        {
            this[pos] = CreateEmptyPiece(pos);
        }

        public void MovePiece(Position from, Position to)
        {
            this[to] = this[from];
            this[to].Move(to);
            this[from] = CreateEmptyPiece(from);
        }

        public void PlacePieceN(Position pos)
        {
            this[pos] = CreateEmptyPiece(pos);
            // p.Move(pos);
        }

        public void RemovePieceN(IPiece p)
        {
            this[p.Pos] = p;
            // p.Pos = pos; //CreateEmptyPiece(pos);
        }

        public void MovePieceN(Position from, Position to)
        {
            this[from] = this[to];
            this[from].Unmove();
            //this[to].Move(to);
            this[to] = CreateEmptyPiece(to);
        }
    }
}