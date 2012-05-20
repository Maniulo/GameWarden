using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    public class ChessGame : Game
    {
        public ChessGame(Meta metainfo)
            : base(2)
        {
            Info = metainfo;
            this.State = new FENParser().Parse(metainfo["FEN"] ?? FENParser.DefaultFEN, this.Players); // !!! We probably should delete this after testing
        }

        public override string ToString()
        {
            return new FENParser().Generate((ChessState)State);
        }

        public new static int DimX { get { return 8; } }
        public new static int DimY { get { return 8; } }
    }
}
