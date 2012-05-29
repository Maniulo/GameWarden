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

            Players.Add(new Player(1));
            Players.Add(new Player(2));

            State = new FENParser().Parse(
                (Info["FEN"] == null) || (Info["FEN"].Equals("")) ? FENParser.DefaultFEN : Info["FEN"],
                 Players); // !!! We probably should delete this after testing
        }

        public override string ToString()
        {
            return Info["Event"] + ": " + Info["White"] + " vs. " + Info["Black"];// +" on " + Info["Date"] + " (" + Info["Result"] + ")";
        }
    }
}
