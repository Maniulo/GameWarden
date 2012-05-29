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

        //IPiece this[int file, int rank] { get; }
        IPiece this[Position index] { get; }
    }

    public class GameState : IGameState 
    {
        private readonly int DimX;
        private readonly int DimY;
        
        protected IPiece[,] Board;

        public GameState(int dimX, int dimY)
        {
            Board = new IPiece[dimX, dimY];

            for (int file = 0; file < dimX; ++file)
                for (int rank = 0; rank < dimY; ++rank)
                    PlaceEmptyPiece(new Position(file + 1, rank + 1));

            DimX = dimX;
            DimY = dimY;
        }

        private IPiece this[int file, int rank]
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
                    throw new ArgumentOutOfRangeException("index");
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
                    throw new ArgumentOutOfRangeException("index");
                }
            }
        }

        public virtual void PlaceEmptyPiece(Position pos)
        {
            PlacePiece(pos, new Piece { IsEmpty = true });
        }

        public virtual void PlacePiece(Position pos, IPiece p)
        {
            this[pos] = p;
            p.Move(pos);
        }

        public virtual void RemovePiece(Position pos)
        {
            PlaceEmptyPiece(pos);
        }

        public virtual void MovePiece(Position from, Position to)
        {
            this[to] = this[from];
            this[to].Move(to);
            PlaceEmptyPiece(from);
        }

        public virtual void PlacePieceN(Position pos)
        {
            PlaceEmptyPiece(pos);
        }

        public virtual void RemovePieceN(IPiece p)
        {
            this[p.Pos] = p;
        }

        public virtual void MovePieceN(Position from, Position to)
        {
            this[from] = this[to];
            this[from].Unmove();
            PlaceEmptyPiece(to);
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