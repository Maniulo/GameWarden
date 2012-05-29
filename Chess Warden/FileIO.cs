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
        private readonly String ErrorFilepath;

        public FileIO(String filepath, String errors = @"C:\1.txt")
        {
            Filepath = filepath;
            ErrorFilepath = errors;
        }

        private static void RemoveTrailingEmptyLines(List<String> lines)
        {
            while (lines.Any() && lines[0].Equals(""))
                lines.RemoveAt(0);

            while (lines.Any() && lines[lines.Count-1].Equals(""))
                lines.RemoveAt(lines.Count-1);

            lines.Add("");
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
            ChessGame result = null;

            foreach (var game in pgnGames)
            {
                try
                {
                    result = pgnParser.Parse(game);
                }
                catch (Exception)
                {
                    if (ErrorFilepath != null)
                    {
                        var writer = new StreamWriter(ErrorFilepath, true);
                        foreach (var s in game) writer.WriteLine(s);
                        writer.Close();
                    }
                }

                if (result != null)
                    yield return result;
            }
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
                    while (i < lines.Count && lines[i].Equals(""))
                        ++i;

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

            if (result.Count > 0)
                yield return result;
        }
        
        public void ExportPGN(IEnumerable<ChessGame> games)
        {
            var writer = new StreamWriter(Filepath);
            foreach (ChessGame game in games)
                ExportSinglePGN(game, writer);
            
            writer.Close();
        }

        private static void ExportSinglePGN(ChessGame game, TextWriter writer)
        {
            var pgn = new PGNParser();
            foreach (var s in pgn.Generate(game))
                writer.WriteLine(s);

            writer.WriteLine("");
        }
        
    }
}
