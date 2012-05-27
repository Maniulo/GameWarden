using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GameWarden.Chess
{
    public class DBLoader : BackgroundWorker
    {
        private ObservableCollection<DBGame> l;
        public DBLoader(ObservableCollection<DBGame> list)
        {
            l = list;
            WorkerReportsProgress = true;
            DoWork += Import;
            ProgressChanged += MakeProgress;
            RunWorkerCompleted += FinishWork;
        }

        private void Import(object sender, DoWorkEventArgs e)
        {
            var filename = (String)e.Argument;
            var io = new FileIO(filename);

            int count = io.Count();
            
            foreach (ChessGame g in io.ImportPGN())
            {
                ReportProgress(0, g);
            }
        }
        private void FinishWork(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void MakeProgress(object sender, ProgressChangedEventArgs e)
        {
            var dg = new DBGame((ChessGame) e.UserState);
            l.Add(dg);
        }
    }
}