﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GameWarden.Chess
{
    public class Promotion : ITemplateMove
    {
        private readonly ITemplateMove BaseMove;
        private List<PieceTypes> CanPromoteToList;

        public Promotion(ITemplateMove baseMove, IEnumerable<PieceTypes> canPromoteTo)
        {
            BaseMove = baseMove;
            CanPromoteToList = canPromoteTo.ToList();
        }

        public bool IsCapture
        {
            get { return BaseMove.IsCapture; }
        }

        [Obsolete]
        public bool CanApply(Position from, Position to, IGameState state)
        {
            return CanApply(from, to, state, ((ChessPiece)state[from]).Type);
        }

        public bool CanApply(Position from, Position to, IGameState state, PieceTypes promoteTo)
        {
            if (state[from].Player.Order == 1 && to.Rank == 8 || state[from].Player.Order == 2 && to.Rank == 1)
                return BaseMove.CanApply(from, to, state) && CanPromoteToList.Contains(promoteTo);

            return false;
        }

        [Obsolete]
        public IConcreteMove Concretize(Position from, Position to)
        {
            throw new Exception("Promotion type is not defined.");
        }

        public IConcreteMove Concretize(Position from, Position to, PieceTypes promoteTo)
        {
            return new PromotionConcrete(BaseMove, promoteTo, from, to);
        }
    }

    class PromotionConcrete : IConcreteMove
    {
        private readonly IConcreteMove BaseMove;
        public PieceTypes PromoteTo;
        private PieceTypes PromoteFrom;
        private readonly Position From;

        public PromotionConcrete(ITemplateMove baseMove, PieceTypes promoteTo, Position from, Position to)
        {
            BaseMove = baseMove.Concretize(from, to);
            PromoteTo = promoteTo;
            From = from;
        }

        public void Apply(IGameState state)
        {
            var p = state[From] as ChessPiece;
            PromoteFrom = p.Type;
            BaseMove.Apply(state);
            p.Type = PromoteTo;
            p.PossibleMoves.Clear();
            ChessPieceFactory.AddMoves(p);
        }

        public void Rollback(IGameState state)
        {
            BaseMove.Rollback(state);
            var p = state[From] as ChessPiece;
            p.Type = PromoteFrom;
            p.PossibleMoves.Clear();
            ChessPieceFactory.AddMoves(p);
        }
    }
}
