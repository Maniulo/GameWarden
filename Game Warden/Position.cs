using System;

namespace GameWarden
{
    public class Position
    {
        public int File
        {
            get
            { 
                try
                {
                    return _File.Value;
                }
                catch
                {
                    throw new Exception("Attempt to get file value from partially initialized position.");
                }
            }
            set { _File = value; }
        }
        public int Rank
        {
            get
            {
                try
                {
                    return _Rank.Value;
                }
                catch
                {
                    throw new Exception("Attempt to get rank value from partially initialized position.");
                }
            }
            set { _Rank = value; }
        }

        protected int? _File;
        protected int? _Rank;

        private const int FileCharACode = 'a';
        private const int FileCharZCode = 'z';

        protected static Char GetFileLetter(int file)
        {
            if (file + FileCharACode - 1 <= FileCharZCode)
            {
                return (Char)(file + FileCharACode - 1);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public Position(int? file = null, int? rank = null)
        {
            _File = file;
            _Rank = rank;
        }

        public Position(String s)
        {
            _File = GetFile(s.ToLower()[0]);
            _Rank = GetRank(s[1]);
        }

        public static implicit operator Position(String s)
        {
            return new Position(s);
        }
        
        public int GetFile(Char c)
        {
            if (FileCharACode <= c && c <= FileCharZCode)
            {
                return (c - FileCharACode) + 1;
            }
            else
            {
                throw new ArgumentException();
            }
            
        }
        public int GetRank(Char c)
        {
            try
            {
                return (int)Char.GetNumericValue(c);
            }
            catch
            {
                throw new ArgumentException();
            }
        }

        public static int RankDistance(Position from, Position to)
        {
            try
            {
                return Math.Abs(from.Rank - to.Rank);
            }
            catch
            {
                throw new ArgumentNullException();
            }
        }
        public static int FileDistance(Position from, Position to)
        {
            try
            {
                return Math.Abs(from.File - to.File);
            }
            catch
            {
                throw new ArgumentNullException();
            }
        }

        public override string ToString()
        {
            return String.Format("{0}{1}", GetFileLetter(File), Rank);
        }

        public static string ToString(int file, int rank)
        {
            return String.Format("{0}{1}", GetFileLetter(file), rank);
        }

        public override bool Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Position)) return false;
            return Equals((Position)obj);
        }
        public bool Equals(Position other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (_File == other._File || other._File == null || _File == null) && (_File == other._File || other._File == null || _File == null); ;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_File.HasValue ? File : 0)*397) ^ (_Rank.HasValue ? Rank : 0);
            }
        }
    }
}