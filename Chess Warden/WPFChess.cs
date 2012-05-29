using System;
using System.ComponentModel;
using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    class VisualChess : INotifyPropertyChanged
    {
        public ChessGame Game;

        public ChessState state;
        public ChessState State
        {
            get
            {
                if (state != null)
                    return state;
                
                if (Game == null)
                    return null; 
                
                return(ChessState)Game.State;
            }
            set
            {
                state = value;
                OnPropertyChanged("State");
                OnPropertyChanged("FEN");
            }
        }

        public String Movetext
        {
            get { return new PGNParser().GenerateMovetext(Game); }
        }
        public String Event { get { return Game.Info["Event"]; } }
        public String Site { get { return Game.Info["Site"]; } }
        public String Data { get { return Game.Info["Data"]; } }
        public String Round { get { return Game.Info["Round"]; } }
        public String Result { get { return Game.Info["Result"]; } }
        public String White { get { return Game.Info["White"]; } }
        public String Black { get { return Game.Info["Black"]; } }
        public String FEN
        {
            get
            {
                return FENParser.GenerateBoard(State);
            }
            set
            {
                try
                {
                    State = new FENParser().ParseBoard(value);
                    OnPropertyChanged("FEN");
                }
                catch
                {
                    throw new ArgumentException();
                }
            }
        }
        public int Moves { get { return Game.MovesCount; } }
        public int CurrentMove { get { return Game.CurrentMove; } }

        public void PlacePiece(Position pos, IPiece p)
        {
            State[pos] = p;
            p.Move(pos); // !!!!!!
            OnPropertyChanged("State");
            OnPropertyChanged("FEN");
        }

        public void MakeMove()
        {
            state = null;
            Game.MakeMove();
            OnPropertyChanged("FEN");
        }

        public void UndoMove()
        {
            state = null;
            Game.UndoMove();
            OnPropertyChanged("FEN");
        }

        public void Reset()
        {
            while (Game.UndoMove()) ;
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