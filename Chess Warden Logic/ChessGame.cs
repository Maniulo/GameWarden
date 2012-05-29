using System;
using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    public class ChessGame : Game
    {
        public ChessGame()
            : this (new Meta())
        {
            
        }

        public ChessGame(ChessGame o)
        {
            throw new NotImplementedException();
        }

        public ChessGame(Meta metainfo)
        {
            Info = metainfo;

            Players.Add(new ChessPlayer(1));
            Players.Add(new ChessPlayer(2));

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
