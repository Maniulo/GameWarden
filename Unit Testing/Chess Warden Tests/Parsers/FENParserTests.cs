using System;
using GameWarden.Chess.Notations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameWarden.Chess;
using System.Collections.Generic;

namespace GameWarden.Tests
{
    [TestClass]
    public class FENParserTests
    {
        [TestMethod]
        public void ParseDefault()
        {
            var parser = new FENParser();
            string fenRecord = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            
            Assert.AreEqual(fenRecord, parser.Parse(fenRecord).ToString());
        }

        [TestMethod]
        public void ParseEmptySpaces()
        {
            var parser = new FENParser();
            string fenRecord = "r1b2b2/1ppppp2/8/8/8/8/7P/RNBQKBNR w KQkq - 0 1";

            Assert.AreEqual(fenRecord, parser.Parse(fenRecord).ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseNoEnding()
        {
            var parser = new FENParser();
            string fenRecord = "r1b2b2/1ppppp2/8/8/8/8/7P/RNBQKBNR";
            parser.Parse(fenRecord);
        }
        
        [TestMethod]
        public void ParseBoardDefault()
        {
            var parser = new FENParser();
            string fenRecord = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
            Assert.AreEqual(fenRecord, parser.ParseBoard(fenRecord).ToStringShort());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseWrongBoard()
        {
            var parser = new FENParser();
            string fenRecord = "z1b2b2/1ppppp2/8/8/8/8/7P";
            parser.ParseBoard(fenRecord);
        }
    }
}
