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

        public ChessState CurrentState
        {
            get
            {
                return (ChessState)this.State;
            }
        }

        public override string ToString()
        {
            return new FENParser().Generate(CurrentState);
        }
    }
}
