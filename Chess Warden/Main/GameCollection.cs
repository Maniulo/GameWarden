using System.Collections.ObjectModel;

namespace GameWarden.Chess
{
    class GameCollection : ObservableCollection<DBGame>
    {
        private readonly ChessEntities DB;

        public GameCollection(ChessEntities db)
            : base(db.Games)
        {
            DB = db;
        }

        protected override void InsertItem(int index, DBGame item)
        {
            var num = 1;
            while (item.Game.MakeMove())
            {
                var gs = new DBGameState {FEN = item.Game.State.ToString(), Games = item, Num = num++};
                item.GameStates.Add(gs);
                DB.AddToGameStates(gs);
            }

            DB.AddToGames(item);
            base.InsertItem(index, item);
        }
    }

    class StatesCollection : ObservableCollection<DBGameState>
    {
        private readonly ChessEntities DB;

        public StatesCollection(ChessEntities db)
        {
            DB = db;
        }

        protected override void InsertItem(int index, DBGameState item)
        {
            //while (item.Game.MakeMove())

            //DB.AddToGames(item);
            base.InsertItem(index, item);
        }
    }
}