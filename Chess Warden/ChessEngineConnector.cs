using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using GameWarden.Chess.Notations;
using Microsoft.Windows.Controls.Ribbon;

namespace GameWarden.Chess
{
    class ChessEngineConnector : INotifyPropertyChanged
    {
        private int depth = 5;
        public int Depth
        {
            get { return depth; }
            set
            {
                if (value > 0)
                {
                    depth = value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        private ChessMove bestMove = null;
        public ChessMove BestMove
        {
            get { return bestMove; }
        }

        private String enginePath;
        public String Path
        {
            get { return enginePath; }
            set { enginePath = value; OnPropertyChanged("State"); }
        }

        public String State
        {
            get
            {
                if (Path == null)
                    return "N/A";

                if (Recheck)
                    return "???";

                if (BestMove == null)
                    return "---";

                return BestMove.ToString();
            }
        }

        public Boolean recheck = false;
        public Boolean Recheck
        {
            get { return recheck; }
            set
            {
                recheck = value;
                OnPropertyChanged("State");
            }
        }
        
        private readonly IChessMoveNotation Parser = new AlgebraicNotation();
        private readonly Regex RxBestMove = new Regex("bestmove (?<Move>.*) ponder");

        

        private Process P;
        private void StartProcess()
        {


            var StartInfo = new ProcessStartInfo
                                {
                                    FileName = enginePath,
                                    RedirectStandardInput = true,
                                    RedirectStandardOutput = true,
                                    UseShellExecute = false,
                                    CreateNoWindow = true
                                };

            P = Process.Start(StartInfo);

            P.StandardInput.WriteLine("ucinewgame");

            if (st != null)
                P.StandardInput.WriteLine("position fen " + new FENParser().Generate(st));

            P.StandardInput.WriteLine("go depth " + depth);

            string s;
            while ((s = P.StandardOutput.ReadLine()) != null)
                if (RxBestMove.Match(s).Groups["Move"].Success)
                {
                    P.Kill();
                    bestMove = Parser.Parse(RxBestMove.Match(s).Groups["Move"].Value);
                    OnPropertyChanged("BestMove");
                    OnPropertyChanged("State");
                }
        }

        private Thread Worker;

        public ChessEngineConnector()
        {
             Worker = new Thread(StartProcess);
        }

        private ChessState st;
        public ChessMove FindBestMove(ChessState state)
        {
            bestMove = null;
            Recheck = false;
            
            if (P != null && !P.HasExited)
                P.Kill();
        
            while (Worker.IsAlive)
                Worker.Abort();

            Worker = new Thread(StartProcess);
            Worker.Start();
            st = state;

            return null;
	    }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
