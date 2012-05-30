using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    class ChessEngineConnector : INotifyPropertyChanged
    {
        private int _Depth = 5;
        public int Depth
        {
            get { return _Depth; }
            set
            {
                if (value > 0)
                {
                    _Depth = value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        private ChessMove _BestMove = null;
        public ChessMove BestMove
        {
            get { return _BestMove; }
        }

        private String _Path;
        public String Path
        {
            get { return _Path; }
            set { _Path = value; OnPropertyChanged("State"); }
        }

        private Boolean Error = false;

        public String State
        {
            get
            {
                if (Error)
                    return "N/A";

                if (Path == null)
                    return "N/A";

                if (Recheck)
                    return "???";

                if (BestMove == null)
                    return "...";

                return BestMove.ToString();
            }
        }
        private ChessState GameState;

        public Boolean _Recheck = false;
        public Boolean Recheck
        {
            get { return _Recheck; }
            set
            {
                _Recheck = value;
                OnPropertyChanged("State");
            }
        }

        private readonly IChessMoveNotation Parser = new AlgebraicNotation();
        private readonly Regex RxBestMove = new Regex("bestmove (?<Move>.*) ponder");

        private Thread Worker;
        public ChessEngineConnector()
        {
            Worker = new Thread(StartProcess);
        }
        public ChessMove FindBestMove(ChessState state)
        {
            _BestMove = null;
            Recheck = false;
            Error = false;

            if (P != null && !P.HasExited)
                P.Kill();

            while (Worker.IsAlive)
                Worker.Abort();

            Worker = new Thread(StartProcess);
            Worker.Start();
            GameState = state;

            return null;
        }

        private Process P;
        private void StartProcess()
        {
            var startInfo = new ProcessStartInfo
                                {
                                    FileName = _Path,
                                    RedirectStandardInput = true,
                                    RedirectStandardOutput = true,
                                    UseShellExecute = false,
                                    CreateNoWindow = true
                                };

            P = Process.Start(startInfo);

            try
            {
                StartNewGame(P);
                SetPosition(P, GameState);
                Go(P, _Depth);
                _BestMove = GetBest(P);
                P.Kill();

                OnPropertyChanged("BestMove");
                OnPropertyChanged("State");
            }
            catch (Exception)
            {
                Recheck = true;
                _BestMove = null;
                Error = true;
                OnPropertyChanged("BestMove");
                OnPropertyChanged("State");
            }
        }

        private void StartNewGame(Process engine)
        {
            engine.StandardInput.WriteLine("ucinewgame");
        }
        private void SetPosition(Process engine, ChessState gameState)
        {
            if (gameState != null)
                engine.StandardInput.WriteLine("position fen " + FENParser.Generate(gameState));
        }
        private void Go(Process engine, int depth)
        {
            engine.StandardInput.WriteLine("go depth " + depth);
        }
        private ChessMove GetBest(Process engine)
        {
            string s;

            while ((s = engine.StandardOutput.ReadLine()) != null)
                if (RxBestMove.Match(s).Groups["Move"].Success)
                    return Parser.Parse(RxBestMove.Match(s).Groups["Move"].Value);

            throw new Exception();
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

    public struct Engine
    {
        private readonly String path;
        public String Path { get { return path; } }
        private readonly String name;
        public String Name { get { return name; } }

        public Engine(String npath)
        {
            path = npath;
            name = path.Substring(path.LastIndexOf('\\') + 1, path.Length - 5 - path.LastIndexOf('\\'));
        }

        public Engine(String npath, String caption)
        {
            path = npath;
            name = caption;
        }

        public override String ToString()
        {
            return Name;
        }
    }
}
