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
        {
            Info = metainfo;

            Players.Add(new Player(1));
            Players.Add(new Player(2));

            State = new FENParser().Parse(
                (Info["FEN"] == null) || (Info["FEN"].Equals("")) ? FENParser.DefaultFEN : Info["FEN"],
                 Players);
        }

        public override string ToString()
        {
            return Info["White"] + " vs. " + Info["Black"] + 
                (Info["Event"] != "" && Info["Event"] != "?" ? " on " + Info["Event"] : "");
        }
    }
}
