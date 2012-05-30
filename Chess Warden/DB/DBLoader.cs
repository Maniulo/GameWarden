using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Samples.EntityDataReader;

namespace GameWarden.Chess
{
    public class DBLoader : BackgroundWorker
    {
        private readonly ChessEntities DB;
        private readonly ObservableCollection<DBGame> Games;
        private readonly List<DBGame> GamesToInsert = new List<DBGame>();
        private readonly List<DBGameState> GameStatesToInsert = new List<DBGameState>();
        private readonly String ConnectionString;

        private const int GamesToLoad = 200;

        private int LastID;
        private int Total;
        private int Done;

        public DBLoader(ObservableCollection<DBGame> games, ChessEntities db, String connectionString)
        {
            DB = db;
            Games = games;
            ConnectionString = connectionString;
            WorkerReportsProgress = true;
            DoWork += Import;
            ProgressChanged += MakeProgress;

            var lastGame = DB.Games.OrderByDescending(u => u.ID).FirstOrDefault();
            if (lastGame == null) LastID = 0;
                             else LastID = lastGame.ID + 1;
        }

        private void Import(object sender, DoWorkEventArgs e)
        {
            var filename = (String)e.Argument;
            var io = new FileIO(filename);
            Total = io.Count();

            foreach (ChessGame g in io.ImportPGN())
            {
                var dbGame = new DBGame(g) { ID = LastID };

                GenerateAndAddStates(dbGame);
                AddGame(dbGame);
                ReportProgress(Done, dbGame);

                if (Done % GamesToLoad == 0)
                    SaveChanges();
            }

            SaveChanges();
            MakeResults(e);
        }

        private void MakeProgress(object sender, ProgressChangedEventArgs e)
        {
            Games.Add((DBGame)e.UserState);
        }
        private void GenerateAndAddStates(DBGame dbGame)
        {
            var num = 1;
            do
            {
                var gs = new DBGameState { FEN = ((ChessState)dbGame.Game.State).ToStringShort(), Games = dbGame, Num = num++, Game = LastID };
                dbGame.GameStates.Add(gs);
                GameStatesToInsert.Add(gs);
            } while (dbGame.Game.MakeMove());

            while (dbGame.Game.UndoMove()) { }
        }
        private void AddGame(DBGame dbGame)
        {
            GamesToInsert.Add(dbGame);
            ++LastID;
            ++Done;
        }
        private void SaveChanges()
        {
            using (var bc = new SqlBulkCopy(ConnectionString))
            {
                bc.DestinationTableName = "Games";
                bc.WriteToServer(GamesToInsert.AsDataReader());
                GamesToInsert.Clear();

                bc.DestinationTableName = "GameStates";
                bc.WriteToServer(GameStatesToInsert.AsDataReader());
                GameStatesToInsert.Clear();
            }
        }
        private void MakeResults(DoWorkEventArgs e)
        {
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
    }
}