using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Samples.EntityDataReader;

namespace GameWarden.Chess
{
    public class DBLoader : BackgroundWorker
    {
        private ChessEntities ch;
        private readonly List<DBGame> GamesToInsert = new List<DBGame>();
        private readonly List<DBGameState> GameStatesToInsert = new List<DBGameState>();

        private int lastID;

        private int Total;
        private int Done;


        private ObservableCollection<DBGame> l;
        public DBLoader(ObservableCollection<DBGame> list, ChessEntities chh)
        {
            ch = chh;
            l = list;
            WorkerReportsProgress = true;
            DoWork += Import;
            ProgressChanged += MakeProgress;

            var lastGame = ch.Games.OrderByDescending(u => u.ID).FirstOrDefault();
            lastID = (lastGame == null) ? 0 : lastGame.ID + 1;
        }

        private void Import(object sender, DoWorkEventArgs e)
        {
            var filename = (String)e.Argument;
            var io = new FileIO(filename);

            Total = io.Count();

            foreach (ChessGame g in io.ImportPGN())
            {
                var dbGame = new DBGame(g) { ID = lastID };

                var num = 1;

                do
                {
                    var gs = new DBGameState
                             {FEN = ((ChessState)dbGame.Game.State).ToStringShort(), Games = dbGame, Num = num++, Game = lastID};
                    dbGame.GameStates.Add(gs);
                    GameStatesToInsert.Add(gs);
                } while (dbGame.Game.MakeMove());

                while (dbGame.Game.UndoMove()) { }

                GamesToInsert.Add(dbGame);
                ++lastID;
                ++Done;

                ReportProgress(Done, dbGame);

                if (Done % 200 == 0)
                    SaveChanges();
            }

            SaveChanges();

            if (Done == Total)
            {
                e.Result = Done;
            }
            else
            {
                var exc = new Exception();
                exc.Data.Add("Errors", Total - Done);
                exc.Data.Add("Done", Done);
                throw exc;
            }
        }

        private void SaveChanges()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ChessDB"].ConnectionString;

            using (var bc = new SqlBulkCopy(connectionString))
            {
                bc.DestinationTableName = "Games";
                bc.WriteToServer(GamesToInsert.AsDataReader());
                GamesToInsert.Clear();

                bc.DestinationTableName = "GameStates";
                bc.WriteToServer(GameStatesToInsert.AsDataReader());
                GameStatesToInsert.Clear();
            }
        }

        private void MakeProgress(object sender, ProgressChangedEventArgs e)
        {
            l.Add((DBGame)e.UserState);
        }
    }
}