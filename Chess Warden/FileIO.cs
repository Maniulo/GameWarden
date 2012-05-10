using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameWarden.Chess.Notations;

namespace GameWarden.Chess
{
    public class FileIO
    {
        private readonly String Filepath;

        public FileIO(String filepath)
        {
            Filepath = filepath;
        }

        /*
        public int CountGames()
        {
            String[] lines = ReadLines();
            return SeparateGames(lines).Count();
        }*/

        public IEnumerable<ChessGame> ImportPGN()
        {
            PGNParser parser = new PGNParser();
            List<String> lines = ReadLines();
            IEnumerable<List<String>> games = SeparateGames(lines);
            ChessGame game;

            foreach (List<String> ss in games)
            {
                    game = parser.Parse(ss, new AlgebraicNotation());
                
                yield return game;
            }
        }

        private List<String> ReadLines()
        {
            var reader = new StreamReader(Filepath);
            var lines = new List<String>();

            while (!reader.EndOfStream)
                lines.Add(reader.ReadLine());

            reader.Close();

            return lines;
        }

        private static IEnumerable<List<String>> SeparateGames(List<String> lines)
        {
            var result = new List<String>();
            var emptyLines = 0;
            int i;

            //remove trailing empty lines
            for (i = 0; i < lines.Count() && lines[i].Equals(""); )
                lines.RemoveAt(i);

            for (; i < lines.Count(); ++i)
            {
                result.Add(lines[i]);
            
                if (lines[i].Equals(""))
                    ++emptyLines;

                if (emptyLines == 2)
                {
                    result.RemoveAt(result.Count-1);
                    yield return result;
                    emptyLines = 0;
                    result.Clear();
                }
            }

            yield return result;
        }

        /*
        public void ExportPGN(IEnumerable<Game> games)
        {
            var writer = new StreamWriter(Filepath);
            foreach (Game game in games)
            {
                ExportSinglePGN(game, writer);
            }
            writer.Close();
        }
        private static void ExportSinglePGN(Game game, TextWriter writer)
        {
            foreach (var s in new PGNPresentation().GetPresentation(game))
            {
                writer.WriteLine(s);
            }
            writer.WriteLine("");
        }
         */
    }
}
