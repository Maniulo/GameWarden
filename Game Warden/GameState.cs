using System;
using System.Collections.Generic;

namespace GameWarden
{
    public interface IGameState : IEnumerable<IPiece>
    {
        IPiece this[Position index] { get; set; }
        void NewEmptyPiece(Position pos);
    }

    public class GameState : IGameState 
    {
        protected readonly int DimX;
        protected readonly int DimY;
        protected IPiece[,] Board;

        public GameState(int dimX, int dimY)
        {
            DimX = dimX;
            DimY = dimY;

            Board = new IPiece[DimX, DimY];

            for (int file = 1; file <= DimX; ++file)
                for (int rank = 1; rank <= DimY; ++rank)
                    NewEmptyPiece(new Position(file, rank));
        }
        public virtual void NewEmptyPiece(Position pos)
        {
            var p = new Piece {IsEmpty = true};
            this[pos] = p;
            p.Move(pos);
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
        public    IPiece this[Position index]
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

            set
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