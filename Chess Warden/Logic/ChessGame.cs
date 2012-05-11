using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    public class ChessGame : Game<ChessState, ChessMove>
    {
        public ChessGame(Meta metainfo)
            : base(2)
        {
            Info = metainfo;
            States.Add(new FENParser().Parse(metainfo["FEN"] ?? FENParser.DefaultFEN, this.Players));
        }

        public override string ToString()
        {
            return new FENParser().Generate(this.CurrentState);
        }
    }

    
}
