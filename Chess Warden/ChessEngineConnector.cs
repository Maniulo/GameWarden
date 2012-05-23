using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using GameWarden.Chess.Notations;
using Microsoft.Windows.Controls.Ribbon;

namespace GameWarden.Chess
{
    class ChessEngineConnector
    {
        private Process p = new Process();

        private readonly Regex RxBestMove = new Regex("bestmove (?<Move>.*) ponder");

        public String FindBestMove(String enginePath, ChessState state) 
	    {
            p.StartInfo = new ProcessStartInfo
                              {
                                  FileName = enginePath,
                                  RedirectStandardInput = true,
                                  RedirectStandardOutput = true,
                                  UseShellExecute = false
                              };
            p.Start();

            p.StandardInput.WriteLine("ucinewgame");
            
            if (state != null)
                p.StandardInput.WriteLine("position fen " + new FENParser().Generate(state));

            p.StandardInput.WriteLine("go depth 5");
	        string s;
            while ((s = p.StandardOutput.ReadLine()) != null)
                if (RxBestMove.Match(s).Groups["Move"].Success)
                {
                    p.Kill();
                    //if (!p.HasExited)
                    //    p.WaitForExit();
                    return RxBestMove.Match(s).Groups["Move"].Value;
                }

            return null;
	    } 
    }
}
