using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace GameWarden
{
    public interface IGameState : IEnumerable<IPiece>
    {
        void PlacePiece(Position pos, IPiece p);
        void RemovePiece(Position pos);
        void MovePiece(Position from, Position to);

        void PlacePieceN(Position pos);
        void RemovePieceN(IPiece p);
        void MovePieceN(Position from, Position to);

        IPiece this[int file, int rank] { get; }
        IPiece this[Position index] { get; }
    }

    public abstract class GameState : IGameState 
    {
        private readonly int DimX;
        private readonly int DimY;
        
        protected IPiece[,] Board;

        protected GameState(int dimX, int dimY)
        {
            Board = new IPiece[dimX, dimY];

            for (int file = 0; file < dimX; ++file)
                for (int rank = 0; rank < dimY; ++rank)
                    Board[file, rank] = PlaceEmptyPiece(new Position(file + 1, rank + 1));

            DimX = dimX;
            DimY = dimY;
        }

        // !!!!11
        public IPiece this[int file, int rank]
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

        public abstract IPiece PlaceEmptyPiece(Position pos);

        public virtual void PlacePiece(Position pos, IPiece p)
        {
            this[pos] = p;
            p.Move(pos);
        }

        public virtual void RemovePiece(Position pos)
        {
            this[pos] = PlaceEmptyPiece(pos);
        }

        public virtual void MovePiece(Position from, Position to)
        {
            this[to] = this[from];
            this[to].Move(to);
            this[from] = PlaceEmptyPiece(from);
        }

        public virtual void PlacePieceN(Position pos)
        {
            this[pos] = PlaceEmptyPiece(pos);
            // p.Move(pos);
        }

        public virtual void RemovePieceN(IPiece p)
        {
            this[p.Pos] = p;
        }

        public virtual void MovePieceN(Position from, Position to)
        {
            this[from] = this[to];
            this[from].Unmove();
            //this[to].Move(to);
            this[to] = PlaceEmptyPiece(to);
        }
        
        public IEnumerator<IPiece> GetEnumerator()
        {
            for (var rank = DimX - 1; rank >= 0; --rank)
                for (var file = 0; file < DimY; ++file)
                    yield return Board[file, rank];
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}