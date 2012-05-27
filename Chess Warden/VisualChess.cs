using System;
using System.ComponentModel;
using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    class VisualChess : INotifyPropertyChanged
    {
        public ChessGame Game;
        public ChessState State
        {
            get
            {
                return (ChessState)Game.State;
            }
            set
            {
                Game.State = value;
                OnPropertyChanged("State");
            }
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
                return Game.State.ToString();
            }
            set
            {
                try
                {
                    State = new FENParser().Parse(value);
                    OnPropertyChanged("FEN");
                }
                catch
                {
                    throw new ArgumentException();
                }
            }
        }

        public void PlacePiece(Position pos, IPiece p)
        {
            State.PlacePiece(pos, p);
            OnPropertyChanged("State");
            OnPropertyChanged("FEN");
        }

        public void MakeMove()
        {
            Game.MakeMove();
            OnPropertyChanged("FEN");
        }

        public void UndoMove()
        {
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