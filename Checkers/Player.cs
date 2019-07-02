namespace Checkers
{
    public class Player
    {
        private readonly string r_Name;
        private readonly char r_Sign;
        private int m_Coins;
        private int m_Score;
        private string m_LastTurn;
        private bool m_HasMoves;

        public Player(string i_Name, char i_Sign)
        {
            r_Name = i_Name;
            r_Sign = i_Sign;
            m_Coins = 0;
            m_Score = 0;
            m_LastTurn = string.Empty;
            m_HasMoves = false;
        }

        public void Reset()
        {
            m_Coins = 0;
            m_LastTurn = string.Empty;
            m_HasMoves = false;
        }

        public string Name
        {
            get
            {
                return r_Name;
            }
        }

        public char Sign
        {
            get
            {
                return r_Sign;
            }
        }

        public int Coins
        {
            get
            {
                return m_Coins;
            }

            set
            {
                m_Coins = value;
            }
        }

        public int Score
        {
            get
            {
                return m_Score;
            }

            set
            {
                m_Score = value;
            }
        }

        public string LastTurn
        {
            get
            {
                return m_LastTurn;
            }

            set
            {
                m_LastTurn = value;
            }
        }

        public bool HasMoves
        {
            get
            {
                return m_HasMoves;
            }

            set
            {
                m_HasMoves = value;
            }
        }
    }
}