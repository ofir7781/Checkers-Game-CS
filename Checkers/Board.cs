namespace Checkers
{
    public class Board
    {
        private const int k_SmallSize = 6;
        private const int k_MediumSize = 8;
        private const int k_LargeSize = 10;
        private const int k_SmallSizeCoins = 6;
        private const int k_MediumSizeCoins = 12;
        private const int k_LargeSizeCoins = 20;
        private const char k_FirstLowerCaseFrame = 'a';
        private const char k_LastLowerCaseFrame = 'z';
        private const char k_FirstUpperCaseFrame = 'A';
        private const char k_LastUpperCaseFrame = 'Z';
        private const char k_EmptySquareOnBoard = ' ';
        private const char k_PlayerOneSign = 'X';
        private const char k_PlayerTwoSign = 'O';
        private const char k_PlayerOneKingSign = 'K';
        private const char k_PlayerTwoKingSign = 'U';
        private int m_BoardSize;
        private char[,] m_GameBoard;

        public void InitializeBoardArray(int i_BoardSize)
        {
            m_BoardSize = i_BoardSize;
            m_GameBoard = new char[m_BoardSize, m_BoardSize];
        }

        public int Size
        {
            get
            {
                return m_BoardSize;
            }
        }

        public char[,] GameBoard
        {
            get
            {
                return m_GameBoard;
            }
        }

        public char PlayerOneSign
        {
            get
            {
                return k_PlayerOneSign;
            }
        }

        public char PlayerTwoSign
        {
            get
            {
                return k_PlayerTwoSign;
            }
        }

        public char PlayerOneKingSign
        {
            get
            {
                return k_PlayerOneKingSign;
            }
        }

        public char PlayerTwoKingSign
        {
            get
            {
                return k_PlayerTwoKingSign;
            }
        }

        public char EmptySquare
        {
            get
            {
                return k_EmptySquareOnBoard;
            }
        }

        public char FirstLowerCaseFrame
        {
            get
            {
                return k_FirstLowerCaseFrame;
            }
        }

        public char FirstUpperCaseFrame
        {
            get
            {
                return k_FirstUpperCaseFrame;
            }
        }

        public char LastLowerCaseFrame
        {
            get
            {
                return k_LastLowerCaseFrame;
            }
        }

        public char LastUpperCaseFrame
        {
            get
            {
                return k_LastUpperCaseFrame;
            }
        }

        public void InitializeCoins(Player i_PlayerOne, Player i_PlayerTwo)
        {
            switch (m_BoardSize)
            {
                case k_SmallSize:
                    i_PlayerOne.Coins = k_SmallSizeCoins;
                    i_PlayerTwo.Coins = k_SmallSizeCoins;
                    break;
                case k_MediumSize:
                    i_PlayerOne.Coins = k_MediumSizeCoins;
                    i_PlayerTwo.Coins = k_MediumSizeCoins;
                    break;
                case k_LargeSize:
                    i_PlayerOne.Coins = k_LargeSizeCoins;
                    i_PlayerTwo.Coins = k_LargeSizeCoins;
                    break;
                default:
                    break;
            }
        }

        public void BuildBoard()
        {
            for (int rows = 0; rows < (m_BoardSize / 2) - 1; rows++)
            {
                for (int cols = 0; cols < m_BoardSize; cols++)
                {
                    if ((rows % 2 == 0 && cols % 2 == 0) || (rows % 2 != 0 && cols % 2 != 0))
                    {
                        m_GameBoard[rows, cols] = EmptySquare;
                    }
                    else
                    {
                        m_GameBoard[rows, cols] = k_PlayerTwoSign;
                    }
                }
            }

            for (int rows = (m_BoardSize / 2) - 1; rows < (m_BoardSize / 2) + 1; rows++)
            {
                for (int cols = 0; cols < m_BoardSize; cols++)
                {
                    m_GameBoard[rows, cols] = EmptySquare;
                }
            }

            for (int rows = (m_BoardSize / 2) + 1; rows < m_BoardSize; rows++)
            {
                for (int cols = 0; cols < m_BoardSize; cols++)
                {
                    if ((rows % 2 != 0 && cols % 2 == 0) || (rows % 2 == 0 && cols % 2 != 0))
                    {
                        m_GameBoard[rows, cols] = k_PlayerOneSign;
                    }
                    else
                    {
                        m_GameBoard[rows, cols] = EmptySquare;
                    }
                }
            }
        }

        public char RemoveRivalPlayerSign(Point i_StartingPointOfPlayer, Point i_DestinationPointOfPlayer)
        {
            char rivalPlayerSign;
            int rowRemovingPlayer = (i_StartingPointOfPlayer.Y + i_DestinationPointOfPlayer.Y) / 2;
            int colRemovingPlayer = (i_StartingPointOfPlayer.X + i_DestinationPointOfPlayer.X) / 2;
            rivalPlayerSign = m_GameBoard[rowRemovingPlayer, colRemovingPlayer];
            m_GameBoard[rowRemovingPlayer, colRemovingPlayer] = EmptySquare;
            return rivalPlayerSign;
        }

        public void MovePlayerToNewPlace(char i_PlayerSign, Point i_StartingPointOfPlayer, Point i_DestinationPointOfPlayer)
        {
            m_GameBoard[i_StartingPointOfPlayer.Y, i_StartingPointOfPlayer.X] = EmptySquare;
            m_GameBoard[i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X] = i_PlayerSign;
        }

        public bool CheckIfKingWasMade(char i_PlayerSign, Point i_DestinationPointOfPlayer)
        {
            bool isKingWasMade = false;

            if (i_PlayerSign == PlayerOneSign)
            {
                if (i_DestinationPointOfPlayer.Y == 0)
                {
                    isKingWasMade = true;
                }
            }
            else if (i_PlayerSign == PlayerTwoSign)
            {
                if (i_DestinationPointOfPlayer.Y == m_BoardSize - 1)
                {
                    isKingWasMade = true;
                }
            }

            return isKingWasMade;
        }

        public void ChangePlayerSignToKing(ref char io_PlayerSign, Point i_DestinationPointOfPlayer)
        {
            if (io_PlayerSign == PlayerOneSign)
            {
                m_GameBoard[i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X] = k_PlayerOneKingSign;
                io_PlayerSign = k_PlayerOneKingSign;
            }
            else
            {
                m_GameBoard[i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X] = k_PlayerTwoKingSign;
                io_PlayerSign = k_PlayerTwoKingSign;
            }
        }
    }
}
