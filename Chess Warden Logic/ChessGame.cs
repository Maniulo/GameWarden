using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    public class ChessGame : Game
    {
        public ChessGame()
            : this (new Meta())
        {
            
        }

        public ChessGame(Meta metainfo)
            : base(2)
        {
            Info = metainfo;

            State = new FENParser().Parse(
                (Info["FEN"] == null) || (Info["FEN"].Equals("")) ? FENParser.DefaultFEN : Info["FEN"],
                 Players); // !!! We probably should delete this after testing
        }

        public override string ToString()
        {
            return Info["Event"] + ": " + Info["White"] + " vs. " + Info["Black"];// +" on " + Info["Date"] + " (" + Info["Result"] + ")";
        }

        public new static int DimX { get { return 8; } }
        public new static int DimY { get { return 8; } }
    }
}
