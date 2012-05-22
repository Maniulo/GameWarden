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

        private static void RemoveTrailingEmptyLines(List<String> lines)
        {
            while (lines.Any() && lines[0].Equals(""))
                lines.RemoveAt(0);
        }

        public int Count()
        {
            var emptyLines = 0;
            var lines = ReadFile();

            RemoveTrailingEmptyLines(lines);

            for (var i = 0; i < lines.Count(); ++i)
                if (lines[i].Equals(""))
                    ++emptyLines;

            // +1 for a case if there is no ending empty line
            return (emptyLines + 1) / 2;
        }

        public IEnumerable<ChessGame> ImportPGN()
        {
            var pgnParser = new PGNParser();
            var lines = ReadFile();
            var pgnGames = SeparateGames(lines);

            return pgnGames.Select(pgnParser.Parse);
        }

        private List<String> ReadFile()
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

            RemoveTrailingEmptyLines(lines);

            for (int i = 0; i < lines.Count(); ++i)
            {
                result.Add(lines[i]);

                if (lines[i].Equals(""))
                {
                    ++emptyLines;
                    while (lines[i].Equals(""))
                    {
                        ++i;
                    }
                    --i;
                }

                if (emptyLines == 2)
                {
                    result.RemoveAt(result.Count-1);
                    yield return result;
                    emptyLines = 0;
                    result = new List<String>();
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
