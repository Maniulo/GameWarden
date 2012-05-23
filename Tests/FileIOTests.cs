using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GameWarden.Chess;
using GameWarden.Chess.Notations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameWarden.Tests
{
    [TestClass]
    public class FileIOTests
    {
        const String Moves = "1. e4 c5 2. Nf3 d6 3. Bb5+ Bd7 4. Bxd7+ Qxd7 5. c4 Nc6 6. Nc3 Nf6 7. O-O g6 8. d4 cxd4 9. Nxd4 Bg7 10. Nde2 Qe6 11. Nd5 Qxe4 12. Nc7+ Kd7 13. Nxa8 Qxc4 14. Nb6+ axb6 15. Nc3 Ra8 16. a4 Ne4 17. Nxe4 Qxe4 18. Qb3 f5 19. Bg5 Qb4 20. Qf7 Be5 21. h3 Rxa4 22. Rxa4 Qxa4 23. Qxh7 Bxb2 24. Qxg6 Qe4 25. Qf7 Bd4 26. Qb3 f4 27. Qf7 Be5 28. h4 b5 29. h5 Qc4 30. Qf5+ Qe6 31. Qxe6+ Kxe6 32. g3 fxg3 33. fxg3 b4 34. Bf4 Bd4+ 35. Kh1 b3 36. g4 Kd5 37. g5 e6 38. h6 Ne7 39. Rd1 e5 40. Be3 Kc4 41. Bxd4 exd4 42. Kg2 b2 43. Kf3 Kc3 44. h7 Ng6 45. Ke4 Kc2 46. Rh1 d3 47. Kf5 b1=Q 48. Rxb1 Kxb1 49. Kxg6 d2 50. h8=Q d1=Q 51. Qh7 b5 52. Kf6+ Kb2 53. Qh2+ Ka1 54. Qf4 b4 55. Qxb4 Qf3+ 56. Kg7 d5 57. Qd4+ Kb1 58. g6 Qe4 59. Qg1+ Kb2 60. Qf2+ Kc1 61. Kf6 d4 62. g7";
        readonly String[] FirstGamePGN = new String[] { "[Event \"Some Event\"]", "[Site \"Some Site\"]", "[Date \"Some Date\"]", "[Round \"Some Round\"]", "[White \"Some Player\"]", "[Black \"Some Player\"]", "[Result \"Some Result\"]", "", Moves };
        readonly String[] SecondGamePGN = new String[] { "[Event \"Another Event\"]", "[Site \"Another Site\"]", "[Date \"Another Date\"]", "[Round \"Another Round\"]", "[White \"Another Player\"]", "[Black \"Another Player\"]", "[Result \"Another Result\"]", "", Moves };
        const string Filepath = "C:/in.pgn";

        public FileIOTests()
        {
            var s = new StreamWriter(Filepath);

            foreach (String str in FirstGamePGN)
                s.WriteLine(str);
            s.WriteLine("");

            foreach (String str in SecondGamePGN)
                s.WriteLine(str);
            s.Close();
        }

        [TestMethod]
        public void ImportCountTest()
        {
            var f = new FileIO(Filepath);
            IEnumerable<ChessGame> gs = f.ImportPGN();
            Assert.AreEqual(2, gs.Count());
        }

        [TestMethod]
        public void ImportCountFuncTest()
        {
            var f = new FileIO(Filepath);
            Assert.AreEqual(2, f.Count());
        }

        [TestMethod]
        public void ImportTest()
        {
            var f = new FileIO(Filepath);
            IEnumerable<ChessGame> gs = f.ImportPGN();
            String[] pgn = new PGNParser().Generate(gs.ElementAt(0)).ToArray();
            CollectionAssert.AreEqual(FirstGamePGN, pgn);
        }
    }
}
