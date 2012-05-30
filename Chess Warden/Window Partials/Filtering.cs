using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace GameWarden.Chess
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class Window
    {
        private CollectionViewSource Filter;

        private void InitializeFilters()
        {
            GamesCollection = new GameCollection(DB);
            Filter = new CollectionViewSource { Source = GamesCollection };
            ResultsList.ItemsSource = Filter.View;
            Filter.Filter += Contains;
        }

        public void Contains(object sender, FilterEventArgs e)
        {
            e.Accepted = false;

            try
            {
                var game = e.Item as DBGame;
                if (game != null)
                    if (game.Event.Contains(EventSearch.Text) &&
                        game.Site.Contains(SiteSearch.Text) &&
                        game.White.Contains(WhiteSearch.Text) &&
                        game.Black.Contains(BlackSearch.Text) &&
                        game.Result.Contains(ResultSearch.Text) &&
                        game.Date.Contains(DateSearch.Text) &&
                        game.Round.Contains(RoundSearch.Text) &&
                        (game.GameStates.Any(state => state.FEN.Contains(FENSearch.Text)) || FENSearch.Text == "" || FENSearch.Text == "8/8/8/8/8/8/8/8")
                        )
                    {
                        e.Accepted = true;
                    }
            }
            catch { }
        }
        
        private void TextboxSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            Filter.View.Refresh();
        }
    }

    class GameCollection : ObservableCollection<DBGame>
    {
        private readonly ChessEntities DB;

        public GameCollection(ChessEntities db)
            : base(db.Games)
        {
            DB = db;
        }

        public void AddGame(IEnumerable<DBGameState> states, DBGame item)
        {
            var num = 1;
            foreach (var gs in states)
            {
                gs.Num = num++;
                gs.Games = item;
                item.GameStates.Add(gs);
            }
        }

        protected override void InsertItem(int index, DBGame item)
        {
            DB.AddToGames(item);
            base.InsertItem(index, item);
        }
    }
}
