using System;

namespace Checkers
{
    public class LogicUnit
    {
        private const int k_ValueOfStandardPlayer = 1;
        private const int k_ValueOfKingPlayer = 4;

        private eGameStatus m_Status;
        private eGameMode m_GameMode;
        private eCurrentShapeTurn m_CurrentShapeTurn;
        private Player m_PlayerOne;
        private Player m_PlayerTwo;
        private Board m_Board;
        private AI m_AI;
        private bool m_CanEatAgain;
        private bool m_Forfeit;
        private bool m_EatPlayer;
        private bool m_FirstTurnOfPlayerCanEat;
        private Point m_CurrentPlayerSoldierSaverForExtraTurn;

        public enum eGameStatus
        {
            Initialize,
            Play,
            EndOfRound,
            Quit,
        }

        public enum eGameMode
        {
            PlayerVsPlayer,
            PlayerVsComputer,
        }

        public enum eCurrentShapeTurn
        {
            Circle,
            Ex,
        }

        public LogicUnit()
        {
            m_Status = eGameStatus.Initialize;
            m_Board = new Board();
            m_CurrentShapeTurn = eCurrentShapeTurn.Ex;
            m_Forfeit = false;
            m_CanEatAgain = false;
            m_EatPlayer = false;
            m_FirstTurnOfPlayerCanEat = false;
        }

        public Player PlayerOne
        {
            get
            {
                return m_PlayerOne;
            }
        }

        public Player PlayerTwo
        {
            get
            {
                return m_PlayerTwo;
            }
        }

        public void CreatePlayerOne(string i_Name)
        {
            m_PlayerOne = new Player(i_Name, m_Board.PlayerOneSign);
        }

        public Board Board
        {
            get
            {
                return m_Board;
            }
        }

        public void CreateBoard(int i_BoardSize)
        {
            m_Board.InitializeBoardArray(i_BoardSize);
            m_Board.BuildBoard();
        }

        public eGameMode Mode
        {
            get
            {
                return m_GameMode;
            }

            set
            {
                m_GameMode = value;
            }
        }

        public void InitializeAI()
        {
            m_AI = new AI();
        }

        public void CreatePlayerTwo(string i_Name)
        {
            m_PlayerTwo = new Player(i_Name, m_Board.PlayerTwoSign);
        }

        public eGameStatus Status
        {
            get
            {
                return m_Status;
            }

            set
            {
                m_Status = value;
            }
        }

        public void InitializeCoins()
        {
            m_Board.InitializeCoins(PlayerOne, PlayerTwo);
        }

        public eCurrentShapeTurn CurrentTurn
        {
            get
            {
                return m_CurrentShapeTurn;
            }
        }

        public bool PreformMove(Point i_StartingPlayerPoint, Point i_DestinationPlayerPoint)
        {
            bool isValidMove = true;
            char playerSign = m_Board.EmptySquare;

            isValidMove = checkIfThereIsAPlayerOnSquare(i_StartingPlayerPoint);
            if (isValidMove)
            {
                playerSign = m_Board.GameBoard[i_StartingPlayerPoint.Y, i_StartingPlayerPoint.X];
                isValidMove = validTurnOfPlayer(playerSign);
            }

            if (isValidMove)
            {
                isValidMove = checkAndPerformIfPlayerCanEatOtherPlayer(playerSign, i_StartingPlayerPoint, i_DestinationPlayerPoint);
            }

            if (!m_CanEatAgain && m_EatPlayer && isValidMove)
            {
                checkIfPlayerCanEatAgain(ref playerSign, i_DestinationPlayerPoint);
                if (!m_CanEatAgain)
                {
                    m_FirstTurnOfPlayerCanEat = false;
                }
            }

            if (!m_EatPlayer && isValidMove)
            {
                isValidMove = checkAndPerformIfPlayerCanMove(playerSign, i_StartingPlayerPoint, i_DestinationPlayerPoint);
            }

            if (isValidMove)
            {
                checkForNewKing(ref playerSign, i_DestinationPlayerPoint);
            }

            return isValidMove;
        }

        private bool checkIfThereIsAPlayerOnSquare(Point i_StartingPointOfPlayer)
        {
            bool isValidMove = true;
            if (m_Board.GameBoard[i_StartingPointOfPlayer.Y, i_StartingPointOfPlayer.X] == m_Board.EmptySquare)
            {
                isValidMove = false;
            }

            return isValidMove;
        }

        public int FindPlaceOfLetterOnBoard(char i_Letter)
        {
            int countedValueOfLetter;
            if (i_Letter >= m_Board.FirstUpperCaseFrame && i_Letter <= m_Board.LastUpperCaseFrame)
            {
                countedValueOfLetter = i_Letter - m_Board.FirstUpperCaseFrame;
            }
            else
            {
                countedValueOfLetter = i_Letter - m_Board.FirstLowerCaseFrame;
            }

            return countedValueOfLetter;
        }

        private bool validTurnOfPlayer(char i_PlayerSign)
        {
            bool isValidMove = true;
            switch (CurrentTurn)
            {
                case eCurrentShapeTurn.Ex:
                    isValidMove = i_PlayerSign == m_Board.PlayerOneSign || i_PlayerSign == m_Board.PlayerOneKingSign;
                    break;
                case eCurrentShapeTurn.Circle:
                    isValidMove = i_PlayerSign == m_Board.PlayerTwoSign || i_PlayerSign == m_Board.PlayerTwoKingSign;
                    break;
                default:
                    isValidMove = false;
                    break;
            }

            return isValidMove;
        }

        private bool checkIfValidExtraTurnEatingMove(char i_PlayerSign, Point i_DestinationPointOfPlayer)
        {
            bool isValidExtraTurnEatingMove = true;

            int rowRemovingPlayer = (m_CurrentPlayerSoldierSaverForExtraTurn.Y + i_DestinationPointOfPlayer.Y) / 2;
            int colRemovingPlayer = (m_CurrentPlayerSoldierSaverForExtraTurn.X + i_DestinationPointOfPlayer.X) / 2;

            if (Math.Abs(i_DestinationPointOfPlayer.Y - m_CurrentPlayerSoldierSaverForExtraTurn.Y) != 2 || Math.Abs(i_DestinationPointOfPlayer.X - m_CurrentPlayerSoldierSaverForExtraTurn.X) != 2)
            {
                isValidExtraTurnEatingMove = false;
            }

            if (i_PlayerSign == m_Board.PlayerOneSign || i_PlayerSign == m_Board.PlayerOneKingSign)
            {
                if (m_Board.GameBoard[rowRemovingPlayer, colRemovingPlayer] != m_Board.PlayerTwoSign && m_Board.GameBoard[rowRemovingPlayer, colRemovingPlayer] != m_Board.PlayerTwoKingSign)
                {
                    isValidExtraTurnEatingMove = false;
                }
            }
            else
            {
                if (m_Board.GameBoard[rowRemovingPlayer, colRemovingPlayer] != m_Board.PlayerOneSign && m_Board.GameBoard[rowRemovingPlayer, colRemovingPlayer] != m_Board.PlayerOneKingSign)
                {
                    isValidExtraTurnEatingMove = false;
                }
            }

            return isValidExtraTurnEatingMove;
        }

        private void checkIfPlayerCanEatAgain(ref char io_PlayerSign, Point i_DestinationPointOfPlayer)
        {
            checkForNewKing(ref io_PlayerSign, i_DestinationPointOfPlayer);

            if (io_PlayerSign == m_Board.PlayerOneSign)
            {
                m_CanEatAgain = checkIfPlayerOneCanEat(i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X);
            }
            else if (io_PlayerSign == m_Board.PlayerTwoSign)
            {
                m_CanEatAgain = checkIfPlayerTwoCanEat(i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X);
            }
            else if (io_PlayerSign == m_Board.PlayerOneKingSign)
            {
                m_CanEatAgain = checkIfPlayerOneKingCanEat(i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X);
            }
            else
            {
                m_CanEatAgain = checkIfPlayerTwoKingCanEat(i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X);
            }

            if(m_CanEatAgain)
            {
                m_CurrentPlayerSoldierSaverForExtraTurn = i_DestinationPointOfPlayer;
            }
        }

        private bool checkIfPlayerOneCanEat(int i_RowDestinationPointOfPlayer, int i_ColDestinationPointOfPlayer)
        {
            bool canEatAgain = false;
            int boardSize = m_Board.Size;

            if (i_RowDestinationPointOfPlayer - 1 >= 0 && i_ColDestinationPointOfPlayer - 1 >= 0)
            {
                if (m_Board.GameBoard[i_RowDestinationPointOfPlayer - 1, i_ColDestinationPointOfPlayer - 1] == m_Board.PlayerTwoSign || m_Board.GameBoard[i_RowDestinationPointOfPlayer - 1, i_ColDestinationPointOfPlayer - 1] == m_Board.PlayerTwoKingSign)
                {
                    if (i_RowDestinationPointOfPlayer - 2 >= 0 && i_ColDestinationPointOfPlayer - 2 >= 0)
                    {
                        if (m_Board.GameBoard[i_RowDestinationPointOfPlayer - 2, i_ColDestinationPointOfPlayer - 2] == m_Board.EmptySquare)
                        {
                            canEatAgain = true;
                        }
                    }
                }
            }

            if (!canEatAgain && i_RowDestinationPointOfPlayer - 1 >= 0 && i_ColDestinationPointOfPlayer + 1 < boardSize)
            {
                if (m_Board.GameBoard[i_RowDestinationPointOfPlayer - 1, i_ColDestinationPointOfPlayer + 1] == m_Board.PlayerTwoSign || m_Board.GameBoard[i_RowDestinationPointOfPlayer - 1, i_ColDestinationPointOfPlayer + 1] == m_Board.PlayerTwoKingSign)
                {
                    if (i_RowDestinationPointOfPlayer - 2 >= 0 && i_ColDestinationPointOfPlayer + 2 < boardSize)
                    {
                        if (m_Board.GameBoard[i_RowDestinationPointOfPlayer - 2, i_ColDestinationPointOfPlayer + 2] == m_Board.EmptySquare)
                        {
                            canEatAgain = true;
                        }
                    }
                }
            }

            return canEatAgain;
        }

        private bool checkIfPlayerTwoCanEat(int i_RowDestinationPointOfPlayer, int i_ColDestinationPointOfPlayer)
        {
            bool canEatAgain = false;
            int boardSize = m_Board.Size;

            if (i_RowDestinationPointOfPlayer + 1 < boardSize && i_ColDestinationPointOfPlayer - 1 >= 0)
            {
                if (m_Board.GameBoard[i_RowDestinationPointOfPlayer + 1, i_ColDestinationPointOfPlayer - 1] == m_Board.PlayerOneSign || m_Board.GameBoard[i_RowDestinationPointOfPlayer + 1, i_ColDestinationPointOfPlayer - 1] == m_Board.PlayerOneKingSign)
                {
                    if (i_RowDestinationPointOfPlayer + 2 < boardSize && i_ColDestinationPointOfPlayer - 2 >= 0)
                    {
                        if (m_Board.GameBoard[i_RowDestinationPointOfPlayer + 2, i_ColDestinationPointOfPlayer - 2] == m_Board.EmptySquare)
                        {
                            canEatAgain = true;
                        }
                    }
                }
            }

            if (!canEatAgain && i_RowDestinationPointOfPlayer + 1 < boardSize && i_ColDestinationPointOfPlayer + 1 < boardSize)
            {
                if (m_Board.GameBoard[i_RowDestinationPointOfPlayer + 1, i_ColDestinationPointOfPlayer + 1] == m_Board.PlayerOneSign || m_Board.GameBoard[i_RowDestinationPointOfPlayer + 1, i_ColDestinationPointOfPlayer + 1] == m_Board.PlayerOneKingSign)
                {
                    if (i_RowDestinationPointOfPlayer + 2 < boardSize && i_ColDestinationPointOfPlayer + 2 < boardSize)
                    {
                        if (m_Board.GameBoard[i_RowDestinationPointOfPlayer + 2, i_ColDestinationPointOfPlayer + 2] == m_Board.EmptySquare)
                        {
                            canEatAgain = true;
                        }
                    }
                }
            }

            return canEatAgain;
        }

        private bool checkIfPlayerOneKingCanEat(int i_RowDestinationPointOfPlayer, int i_ColDestinationPointOfPlayer)
        {
            bool canEatAgain = false;
            int boardSize = m_Board.Size;

            if (i_RowDestinationPointOfPlayer - 1 >= 0 && i_ColDestinationPointOfPlayer - 1 >= 0)
            {
                if (m_Board.GameBoard[i_RowDestinationPointOfPlayer - 1, i_ColDestinationPointOfPlayer - 1] == m_Board.PlayerTwoSign || m_Board.GameBoard[i_RowDestinationPointOfPlayer - 1, i_ColDestinationPointOfPlayer - 1] == m_Board.PlayerTwoKingSign)
                {
                    if (i_RowDestinationPointOfPlayer - 2 >= 0 && i_ColDestinationPointOfPlayer - 2 >= 0)
                    {
                        if (m_Board.GameBoard[i_RowDestinationPointOfPlayer - 2, i_ColDestinationPointOfPlayer - 2] == m_Board.EmptySquare)
                        {
                            canEatAgain = true;
                        }
                    }
                }
            }

            if (!canEatAgain && i_RowDestinationPointOfPlayer - 1 >= 0 && i_ColDestinationPointOfPlayer + 1 < boardSize)
            {
                if (m_Board.GameBoard[i_RowDestinationPointOfPlayer - 1, i_ColDestinationPointOfPlayer + 1] == m_Board.PlayerTwoSign || m_Board.GameBoard[i_RowDestinationPointOfPlayer - 1, i_ColDestinationPointOfPlayer + 1] == m_Board.PlayerTwoKingSign)
                {
                    if (!canEatAgain && i_RowDestinationPointOfPlayer - 2 >= 0 && i_ColDestinationPointOfPlayer + 2 < boardSize)
                    {
                        if (m_Board.GameBoard[i_RowDestinationPointOfPlayer - 2, i_ColDestinationPointOfPlayer + 2] == m_Board.EmptySquare)
                        {
                            canEatAgain = true;
                        }
                    }
                }
            }

            if (!canEatAgain && i_RowDestinationPointOfPlayer + 1 < boardSize && i_ColDestinationPointOfPlayer - 1 >= 0)
            {
                if (m_Board.GameBoard[i_RowDestinationPointOfPlayer + 1, i_ColDestinationPointOfPlayer - 1] == m_Board.PlayerTwoSign || m_Board.GameBoard[i_RowDestinationPointOfPlayer + 1, i_ColDestinationPointOfPlayer - 1] == m_Board.PlayerTwoKingSign)
                {
                    if (i_RowDestinationPointOfPlayer + 2 < boardSize && i_ColDestinationPointOfPlayer - 2 >= 0)
                    {
                        if (m_Board.GameBoard[i_RowDestinationPointOfPlayer + 2, i_ColDestinationPointOfPlayer - 2] == m_Board.EmptySquare)
                        {
                            canEatAgain = true;
                        }
                    }
                }
            }

            if (!canEatAgain && i_RowDestinationPointOfPlayer + 1 < boardSize && i_ColDestinationPointOfPlayer + 1 < boardSize)
            {
                if (m_Board.GameBoard[i_RowDestinationPointOfPlayer + 1, i_ColDestinationPointOfPlayer + 1] == m_Board.PlayerTwoSign || m_Board.GameBoard[i_RowDestinationPointOfPlayer + 1, i_ColDestinationPointOfPlayer + 1] == m_Board.PlayerTwoKingSign)
                {
                    if (i_RowDestinationPointOfPlayer + 2 < boardSize && i_ColDestinationPointOfPlayer + 2 < boardSize)
                    {
                        if (m_Board.GameBoard[i_RowDestinationPointOfPlayer + 2, i_ColDestinationPointOfPlayer + 2] == m_Board.EmptySquare)
                        {
                            canEatAgain = true;
                        }
                    }
                }
            }

            return canEatAgain;
        }

        private bool checkIfPlayerTwoKingCanEat(int i_RowDestinationPointOfPlayer, int i_ColDestinationPointOfPlayer)
        {
            bool canEatAgain = false;
            int boardSize = m_Board.Size;

            if (i_RowDestinationPointOfPlayer - 1 >= 0 && i_ColDestinationPointOfPlayer - 1 >= 0)
            {
                if (m_Board.GameBoard[i_RowDestinationPointOfPlayer - 1, i_ColDestinationPointOfPlayer - 1] == m_Board.PlayerOneSign || m_Board.GameBoard[i_RowDestinationPointOfPlayer - 1, i_ColDestinationPointOfPlayer - 1] == m_Board.PlayerOneKingSign)
                {
                    if (i_RowDestinationPointOfPlayer - 2 >= 0 && i_ColDestinationPointOfPlayer - 2 >= 0)
                    {
                        if (m_Board.GameBoard[i_RowDestinationPointOfPlayer - 2, i_ColDestinationPointOfPlayer - 2] == m_Board.EmptySquare)
                        {
                            canEatAgain = true;
                        }
                    }
                }
            }

            if (!canEatAgain && i_RowDestinationPointOfPlayer - 1 >= 0 && i_ColDestinationPointOfPlayer + 1 < boardSize)
            {
                if (m_Board.GameBoard[i_RowDestinationPointOfPlayer - 1, i_ColDestinationPointOfPlayer + 1] == m_Board.PlayerOneSign || m_Board.GameBoard[i_RowDestinationPointOfPlayer - 1, i_ColDestinationPointOfPlayer + 1] == m_Board.PlayerOneKingSign)
                {
                    if (i_RowDestinationPointOfPlayer - 2 >= 0 && i_ColDestinationPointOfPlayer + 2 < boardSize)
                    {
                        if (m_Board.GameBoard[i_RowDestinationPointOfPlayer - 2, i_ColDestinationPointOfPlayer + 2] == m_Board.EmptySquare)
                        {
                            canEatAgain = true;
                        }
                    }
                }
            }

            if (!canEatAgain && i_RowDestinationPointOfPlayer + 1 < boardSize && i_ColDestinationPointOfPlayer - 1 >= 0)
            {
                if (m_Board.GameBoard[i_RowDestinationPointOfPlayer + 1, i_ColDestinationPointOfPlayer - 1] == m_Board.PlayerOneSign || m_Board.GameBoard[i_RowDestinationPointOfPlayer + 1, i_ColDestinationPointOfPlayer - 1] == m_Board.PlayerOneKingSign)
                {
                    if (i_RowDestinationPointOfPlayer + 2 < boardSize && i_ColDestinationPointOfPlayer - 2 >= 0)
                    {
                        if (m_Board.GameBoard[i_RowDestinationPointOfPlayer + 2, i_ColDestinationPointOfPlayer - 2] == m_Board.EmptySquare)
                        {
                            canEatAgain = true;
                        }
                    }
                }
            }

            if (!canEatAgain && i_RowDestinationPointOfPlayer + 1 < boardSize && i_ColDestinationPointOfPlayer + 1 < boardSize)
            {
                if (m_Board.GameBoard[i_RowDestinationPointOfPlayer + 1, i_ColDestinationPointOfPlayer + 1] == m_Board.PlayerOneSign || m_Board.GameBoard[i_RowDestinationPointOfPlayer + 1, i_ColDestinationPointOfPlayer + 1] == m_Board.PlayerOneKingSign)
                {
                    if (i_RowDestinationPointOfPlayer + 2 < boardSize && i_ColDestinationPointOfPlayer + 2 < boardSize)
                    {
                        if (m_Board.GameBoard[i_RowDestinationPointOfPlayer + 2, i_ColDestinationPointOfPlayer + 2] == m_Board.EmptySquare)
                        {
                            canEatAgain = true;
                        }
                    }
                }
            }

            return canEatAgain;
        }

        private bool checkIfPlayerFirstTurnCanEat(char i_PlayerSign, Point i_StartingPointOfPlayer)
        {
            bool isFirstTurnOfPlayerCanEat = false;
            int boardSize = m_Board.Size;

            for (int row = 0; row < boardSize && !isFirstTurnOfPlayerCanEat; row++)
            {
                for (int col = 0; col < boardSize && !isFirstTurnOfPlayerCanEat; col++)
                {
                    if (i_PlayerSign == m_Board.PlayerOneSign || i_PlayerSign == m_Board.PlayerOneKingSign)
                    {
                        if (m_Board.GameBoard[row, col] == m_Board.PlayerOneSign)
                        {
                            isFirstTurnOfPlayerCanEat = checkIfPlayerOneCanEat(row, col);
                        }
                        else if (m_Board.GameBoard[row, col] == m_Board.PlayerOneKingSign)
                        {
                            isFirstTurnOfPlayerCanEat = checkIfPlayerOneKingCanEat(row, col);
                        }
                    }
                    else
                    {
                        if (m_Board.GameBoard[row, col] == m_Board.PlayerTwoSign)
                        {
                            isFirstTurnOfPlayerCanEat = checkIfPlayerTwoCanEat(row, col);
                        }
                        else if (m_Board.GameBoard[row, col] == m_Board.PlayerTwoKingSign)
                        {
                            isFirstTurnOfPlayerCanEat = checkIfPlayerTwoKingCanEat(row, col);
                        }
                    }
                }
            }

            return isFirstTurnOfPlayerCanEat;
        }

        private bool checkAndPerformIfPlayerCanEatOtherPlayer(char i_PlayerSign, Point i_StartingPointOfPlayer, Point i_DestinationPointOfPlayer)
        {
            bool isValidMove = true;
            m_EatPlayer = false;

            if (!m_FirstTurnOfPlayerCanEat)
            {
                m_FirstTurnOfPlayerCanEat = checkIfPlayerFirstTurnCanEat(i_PlayerSign, i_StartingPointOfPlayer);
            }

            if (m_CanEatAgain || m_FirstTurnOfPlayerCanEat)
            {
                isValidMove = checkIfDestinationIsAValidEatingMove(i_PlayerSign, i_StartingPointOfPlayer, i_DestinationPointOfPlayer);

                if (m_CanEatAgain && isValidMove)
                {
                    isValidMove = checkIfValidExtraTurnEatingMove(i_PlayerSign, i_DestinationPointOfPlayer);
                }

                if (isValidMove)
                {
                    m_EatPlayer = true;
                    m_CanEatAgain = false;
                }
            }

            if (m_EatPlayer && isValidMove)
            {
                eatPlayerAndChangeBoard(i_PlayerSign, i_StartingPointOfPlayer, i_DestinationPointOfPlayer);
            }

            return isValidMove;
        }

        private bool checkIfDestinationIsAValidEatingMove(char i_PlayerSign, Point i_StartingPointOfPlayer, Point i_DestinationPointOfPlayer)
        {
            bool isDestinationIsAValidEatingMove = false;
            bool legalMove;
            int rowRemovingPlayer = (i_StartingPointOfPlayer.Y + i_DestinationPointOfPlayer.Y) / 2;
            int colRemovingPlayer = (i_StartingPointOfPlayer.X + i_DestinationPointOfPlayer.X) / 2;

            legalMove = checkIfLegalMove(i_PlayerSign, i_StartingPointOfPlayer, i_DestinationPointOfPlayer);

            if ((legalMove && i_PlayerSign == m_Board.PlayerOneSign) || i_PlayerSign == m_Board.PlayerOneKingSign)
            {
                if (m_Board.GameBoard[rowRemovingPlayer, colRemovingPlayer] == m_Board.PlayerTwoSign || m_Board.GameBoard[rowRemovingPlayer, colRemovingPlayer] == m_Board.PlayerTwoKingSign)
                {
                        if (m_Board.GameBoard[i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X] == m_Board.EmptySquare)
                        {
                            isDestinationIsAValidEatingMove = true;
                        }
                }
            }
            else if ((legalMove && i_PlayerSign == m_Board.PlayerTwoSign) || i_PlayerSign == m_Board.PlayerTwoKingSign)
            {
                if (m_Board.GameBoard[rowRemovingPlayer, colRemovingPlayer] == m_Board.PlayerOneSign || m_Board.GameBoard[rowRemovingPlayer, colRemovingPlayer] == m_Board.PlayerOneKingSign)
                {
                    if (m_Board.GameBoard[i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X] == m_Board.EmptySquare)
                    {
                        isDestinationIsAValidEatingMove = true;
                    }
                }
            }

            return isDestinationIsAValidEatingMove;
        }

        private bool checkIfLegalMove(char i_PlayerSign, Point i_StartingPointOfPlayer, Point i_DestinationPointOfPlayer)
        {
            bool legalMove = true;
            int rowRemovingPlayer = (i_StartingPointOfPlayer.Y + i_DestinationPointOfPlayer.Y) / 2;
            int colRemovingPlayer = (i_StartingPointOfPlayer.X + i_DestinationPointOfPlayer.X) / 2;

            if (i_PlayerSign == m_Board.PlayerOneSign)
            {
                if (i_DestinationPointOfPlayer.Y >= i_StartingPointOfPlayer.Y)
                {
                    legalMove = false;
                }
            }
            else if (i_PlayerSign == m_Board.PlayerTwoSign)
            {
                if (i_DestinationPointOfPlayer.Y <= i_StartingPointOfPlayer.Y)
                {
                    legalMove = false;
                }
            }

            if (legalMove)
            {
                if (Math.Abs(i_DestinationPointOfPlayer.Y - rowRemovingPlayer) != 1 || Math.Abs(i_DestinationPointOfPlayer.X - colRemovingPlayer) != 1 || Math.Abs(rowRemovingPlayer - i_StartingPointOfPlayer.Y) != 1 || Math.Abs(colRemovingPlayer - i_StartingPointOfPlayer.X) != 1)
                {
                    legalMove = false;
                }
            }

            return legalMove;
        }

        private bool checkAndPerformIfPlayerCanMove(char i_PlayerSign, Point i_StartingPointOfPlayer, Point i_DestinationPointOfPlayer)
        {
            bool isPlayerCanMove = false;
            if (i_PlayerSign == m_Board.PlayerOneSign)
            {
                isPlayerCanMove = checkIfPlayerOneCanMove(i_StartingPointOfPlayer, i_DestinationPointOfPlayer);
            }
            else if (i_PlayerSign == m_Board.PlayerTwoSign)
            {
                isPlayerCanMove = checkIfPlayerTwoCanMove(i_StartingPointOfPlayer, i_DestinationPointOfPlayer);
            }
            else
            {
                isPlayerCanMove = checkIfKingCanMove(i_StartingPointOfPlayer, i_DestinationPointOfPlayer);
            }

            if (isPlayerCanMove)
            {
                m_Board.MovePlayerToNewPlace(i_PlayerSign, i_StartingPointOfPlayer, i_DestinationPointOfPlayer);
            }

            return isPlayerCanMove;
        }

        private bool checkIfPlayerOneCanMove(Point i_StartingPointOfPlayer, Point i_DestinationPointOfPlayer)
        {
            bool isPlayerOneCanMove = false;

            if (i_DestinationPointOfPlayer.Y + 1 == i_StartingPointOfPlayer.Y && i_DestinationPointOfPlayer.X - 1 == i_StartingPointOfPlayer.X)
            {
                if (m_Board.GameBoard[i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X] == m_Board.EmptySquare)
                {
                    isPlayerOneCanMove = true;
                }
            }
            else if (i_DestinationPointOfPlayer.Y + 1 == i_StartingPointOfPlayer.Y && i_DestinationPointOfPlayer.X + 1 == i_StartingPointOfPlayer.X)
            {
                if (m_Board.GameBoard[i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X] == m_Board.EmptySquare)
                {
                    isPlayerOneCanMove = true;
                }
            }

            return isPlayerOneCanMove;
        }

        private bool checkIfPlayerTwoCanMove(Point i_StartingPointOfPlayer, Point i_DestinationPointOfPlayer)
        {
            bool isPlayerTwoCanMove = false;

            if (i_DestinationPointOfPlayer.Y - 1 == i_StartingPointOfPlayer.Y && i_DestinationPointOfPlayer.X + 1 == i_StartingPointOfPlayer.X)
            {
                if (m_Board.GameBoard[i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X] == m_Board.EmptySquare)
                {
                    isPlayerTwoCanMove = true;
                }
            }
            else if (i_DestinationPointOfPlayer.Y - 1 == i_StartingPointOfPlayer.Y && i_DestinationPointOfPlayer.X - 1 == i_StartingPointOfPlayer.X)
            {
                if (m_Board.GameBoard[i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X] == m_Board.EmptySquare)
                {
                    isPlayerTwoCanMove = true;
                }
            }

            return isPlayerTwoCanMove;
        }

        private bool checkIfKingCanMove(Point i_StartingPointOfPlayer, Point i_DestinationPointOfPlayer)
        {
            bool isKingCanMove = false;

            if (i_DestinationPointOfPlayer.Y + 1 == i_StartingPointOfPlayer.Y && i_DestinationPointOfPlayer.X - 1 == i_StartingPointOfPlayer.X)
            {
                if (m_Board.GameBoard[i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X] == m_Board.EmptySquare)
                {
                    isKingCanMove = true;
                }
            }
            else if (i_DestinationPointOfPlayer.Y + 1 == i_StartingPointOfPlayer.Y && i_DestinationPointOfPlayer.X + 1 == i_StartingPointOfPlayer.X)
            {
                if (m_Board.GameBoard[i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X] == m_Board.EmptySquare)
                {
                    isKingCanMove = true;
                }
            }
            else if (i_DestinationPointOfPlayer.Y - 1 == i_StartingPointOfPlayer.Y && i_DestinationPointOfPlayer.X + 1 == i_StartingPointOfPlayer.X)
            {
                if (m_Board.GameBoard[i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X] == m_Board.EmptySquare)
                {
                    isKingCanMove = true;
                }
            }
            else if (i_DestinationPointOfPlayer.Y - 1 == i_StartingPointOfPlayer.Y && i_DestinationPointOfPlayer.X - 1 == i_StartingPointOfPlayer.X)
            {
                if (m_Board.GameBoard[i_DestinationPointOfPlayer.Y, i_DestinationPointOfPlayer.X] == m_Board.EmptySquare)
                {
                    isKingCanMove = true;
                }
            }

            return isKingCanMove;
        }

        private void eatPlayerAndChangeBoard(char i_PlayerSign, Point i_StartingPointOfPlayer, Point i_DestinationPointOfPlayer)
        {
            char rivalPlayerSign;
            rivalPlayerSign = m_Board.RemoveRivalPlayerSign(i_StartingPointOfPlayer, i_DestinationPointOfPlayer);
            m_Board.MovePlayerToNewPlace(i_PlayerSign, i_StartingPointOfPlayer, i_DestinationPointOfPlayer);
            updateScoreOfPlayerAfterEating(rivalPlayerSign);
        }

        private void updateScoreOfPlayerAfterEating(char i_RivalPlayerSign)
        {
            if (i_RivalPlayerSign == m_Board.PlayerOneSign)
            {
                PlayerOne.Coins -= k_ValueOfStandardPlayer;
            }
            else if (i_RivalPlayerSign == m_Board.PlayerOneKingSign)
            {
                PlayerOne.Coins -= k_ValueOfKingPlayer;
            }
            else if (i_RivalPlayerSign == m_Board.PlayerTwoSign)
            {
                PlayerTwo.Coins -= k_ValueOfStandardPlayer;
            }
            else if (i_RivalPlayerSign == m_Board.PlayerTwoKingSign)
            {
                PlayerTwo.Coins -= k_ValueOfKingPlayer;
            }
        }

        private void updateScoreOfPlayerAfterTurningKing(char i_PlayerSign)
        {
            if (i_PlayerSign == m_Board.PlayerOneKingSign)
            {
                PlayerOne.Coins += k_ValueOfKingPlayer - k_ValueOfStandardPlayer;
            }
            else if (i_PlayerSign == m_Board.PlayerTwoKingSign)
            {
                PlayerTwo.Coins += k_ValueOfKingPlayer - k_ValueOfStandardPlayer;
            }
        }

        public bool CheckIfValidForfeit()
        {
            switch (CurrentTurn)
            {
                case eCurrentShapeTurn.Circle:
                    if (PlayerTwo.Coins < PlayerOne.Coins)
                    {
                        m_Forfeit = true;
                    }

                    break;
                case eCurrentShapeTurn.Ex:
                    if (PlayerOne.Coins < PlayerTwo.Coins)
                    {
                        m_Forfeit = true;
                    }

                    break;
                default:
                    break;
            }

            return m_Forfeit;
        }

        public bool CanEatAgain
        {
            get
            {
                return m_CanEatAgain;
            }
        }

        public string ComputerDefaultName
        {
            get
            {
                return m_AI.ComputerDefaultName;
            }
        }

        public void CheckIfBothSidesHaveMoreAvailableMoves()
        {
            int boardSize = m_Board.Size;
            char[,] checkersGameBoard = m_Board.GameBoard;
            char currentSquarePlayer = m_Board.EmptySquare;

            PlayerOne.HasMoves = false;
            PlayerTwo.HasMoves = false;
            for (int rowOfCheckersBoard = 0; rowOfCheckersBoard < boardSize; rowOfCheckersBoard++)
            {
                for (int colOfCheckersBoard = 0; colOfCheckersBoard < boardSize; colOfCheckersBoard++)
                {
                    currentSquarePlayer = checkersGameBoard[rowOfCheckersBoard, colOfCheckersBoard];
                    if (currentSquarePlayer == m_Board.PlayerOneSign)
                    {
                        if (!PlayerOne.HasMoves)
                        {
                            PlayerOne.HasMoves = checkIfPlayerOneHasMoreAvailableMoves(rowOfCheckersBoard, colOfCheckersBoard);
                        }
                    }
                    else if (currentSquarePlayer == m_Board.PlayerTwoSign)
                    {
                        if (!PlayerTwo.HasMoves)
                        {
                            PlayerTwo.HasMoves = checkIfPlayerTwoHasMoreAvailableMoves(rowOfCheckersBoard, colOfCheckersBoard);
                        }
                    }
                    else if (currentSquarePlayer == m_Board.PlayerOneKingSign)
                    {
                        if (!PlayerOne.HasMoves)
                        {
                            PlayerOne.HasMoves = checkIfPlayerOneKingHasMoreAvailableMoves(rowOfCheckersBoard, colOfCheckersBoard);
                        }
                    }
                    else if (currentSquarePlayer == m_Board.PlayerTwoKingSign)
                    {
                        if (!PlayerTwo.HasMoves)
                        {
                            PlayerTwo.HasMoves = checkIfPlayerTwoKingHasMoreAvailableMoves(rowOfCheckersBoard, colOfCheckersBoard);
                        }
                    }
                }
            }
        }

        private bool checkIfPlayerOneHasMoreAvailableMoves(int i_RowStartingPointOfPlayer, int i_ColStartingPointOfPlayer)
        {
            bool hasMoreMoves = false;
            int boardSize = m_Board.Size;

            if (i_RowStartingPointOfPlayer - 1 >= 0 && i_ColStartingPointOfPlayer - 1 >= 0)
            {
                if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer - 1] == m_Board.EmptySquare)
                {
                    hasMoreMoves = true;
                }
                else if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer - 1] == m_Board.PlayerTwoSign || m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer - 1] == m_Board.PlayerTwoKingSign)
                {
                    if (i_RowStartingPointOfPlayer - 2 >= 0 && i_ColStartingPointOfPlayer - 2 >= 0)
                    {
                        if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 2, i_ColStartingPointOfPlayer - 2] == m_Board.EmptySquare)
                        {
                            hasMoreMoves = true;
                        }
                    }
                }
            }

            if (!hasMoreMoves && i_RowStartingPointOfPlayer - 1 >= 0 && i_ColStartingPointOfPlayer + 1 < boardSize)
            {
                if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer + 1] == m_Board.EmptySquare)
                {
                    hasMoreMoves = true;
                }
                else if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer + 1] == m_Board.PlayerTwoSign || m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer + 1] == m_Board.PlayerTwoKingSign)
                {
                    if (!hasMoreMoves && i_RowStartingPointOfPlayer - 2 >= 0 && i_ColStartingPointOfPlayer + 2 < boardSize)
                    {
                        if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 2, i_ColStartingPointOfPlayer + 2] == m_Board.EmptySquare)
                        {
                            hasMoreMoves = true;
                        }
                    }
                }
            }

            return hasMoreMoves;
        }

        private bool checkIfPlayerTwoHasMoreAvailableMoves(int i_RowStartingPointOfPlayer, int i_ColStartingPointOfPlayer)
        {
            bool hasMoreMoves = false;
            int boardSize = m_Board.Size;

            if (i_RowStartingPointOfPlayer + 1 < boardSize && i_ColStartingPointOfPlayer - 1 >= 0)
            {
                if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer - 1] == m_Board.EmptySquare)
                {
                    hasMoreMoves = true;
                }
                else if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer - 1] == m_Board.PlayerOneSign || m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer - 1] == m_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfPlayer + 2 < boardSize && i_ColStartingPointOfPlayer - 2 >= 0)
                    {
                        if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 2, i_ColStartingPointOfPlayer - 2] == m_Board.EmptySquare)
                        {
                            hasMoreMoves = true;
                        }
                    }
                }
            }

            if (!hasMoreMoves && i_RowStartingPointOfPlayer + 1 < boardSize && i_ColStartingPointOfPlayer + 1 < boardSize)
            {
                if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer + 1] == m_Board.EmptySquare)
                {
                    hasMoreMoves = true;
                }
                else if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer + 1] == m_Board.PlayerOneSign || m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer + 1] == m_Board.PlayerOneKingSign)
                {
                    if (!hasMoreMoves && i_RowStartingPointOfPlayer + 2 < boardSize && i_ColStartingPointOfPlayer + 2 < boardSize)
                    { 
                        if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 2, i_ColStartingPointOfPlayer + 2] == m_Board.EmptySquare)
                        {
                            hasMoreMoves = true;
                        }
                    }
                }
            }

            return hasMoreMoves;
        }

        private bool checkIfPlayerOneKingHasMoreAvailableMoves(int i_RowStartingPointOfPlayer, int i_ColStartingPointOfPlayer)
        {
            bool hasMoreMoves = false;
            int boardSize = m_Board.Size;

            if (i_RowStartingPointOfPlayer - 1 >= 0 && i_ColStartingPointOfPlayer - 1 >= 0)
            {
                if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer - 1] == m_Board.EmptySquare)
                {
                    hasMoreMoves = true;
                }
                else if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer - 1] == m_Board.PlayerTwoSign || m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer - 1] == m_Board.PlayerTwoKingSign)
                {
                    if (i_RowStartingPointOfPlayer - 2 >= 0 && i_ColStartingPointOfPlayer - 2 >= 0)
                    {
                        if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 2, i_ColStartingPointOfPlayer - 2] == m_Board.EmptySquare)
                        {
                            hasMoreMoves = true;
                        }
                    }
                }
            }

            if (!hasMoreMoves && i_RowStartingPointOfPlayer - 1 >= 0 && i_ColStartingPointOfPlayer + 1 < boardSize)
            {
                if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer + 1] == m_Board.EmptySquare)
                {
                    hasMoreMoves = true;
                }
                else if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer + 1] == m_Board.PlayerTwoSign || m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer + 1] == m_Board.PlayerTwoKingSign)
                {
                    if (!hasMoreMoves && i_RowStartingPointOfPlayer - 2 >= 0 && i_ColStartingPointOfPlayer + 2 < boardSize)
                    {
                        if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 2, i_ColStartingPointOfPlayer + 2] == m_Board.EmptySquare)
                        {
                            hasMoreMoves = true;
                        }
                    }
                }
            }

            if (!hasMoreMoves && i_RowStartingPointOfPlayer + 1 < boardSize && i_ColStartingPointOfPlayer - 1 >= 0)
            {
                if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer - 1] == m_Board.EmptySquare)
                {
                    hasMoreMoves = true;
                }
                else if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer - 1] == m_Board.PlayerTwoSign || m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer - 1] == m_Board.PlayerTwoKingSign)
                {
                    if (!hasMoreMoves && i_RowStartingPointOfPlayer + 2 < boardSize && i_ColStartingPointOfPlayer - 2 >= 0)
                    {
                        if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 2, i_ColStartingPointOfPlayer - 2] == m_Board.EmptySquare)
                        {
                            hasMoreMoves = true;
                        }
                    }
                }
            }

            if (!hasMoreMoves && i_RowStartingPointOfPlayer + 1 < boardSize && i_ColStartingPointOfPlayer + 1 < boardSize)
            {
                if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer + 1] == m_Board.EmptySquare)
                {
                    hasMoreMoves = true;
                }
                else if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer + 1] == m_Board.PlayerTwoSign || m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer + 1] == m_Board.PlayerTwoKingSign)
                {
                    if (!hasMoreMoves && i_RowStartingPointOfPlayer + 2 < boardSize && i_ColStartingPointOfPlayer + 2 < boardSize)
                    {
                        if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 2, i_ColStartingPointOfPlayer + 2] == m_Board.EmptySquare)
                        {
                            hasMoreMoves = true;
                        }
                    }
                }
            }

            return hasMoreMoves;
        }

        private bool checkIfPlayerTwoKingHasMoreAvailableMoves(int i_RowStartingPointOfPlayer, int i_ColStartingPointOfPlayer)
        {
            bool hasMoreMoves = false;
            int boardSize = m_Board.Size;

            if (i_RowStartingPointOfPlayer - 1 >= 0 && i_ColStartingPointOfPlayer - 1 >= 0)
            {
                if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer - 1] == m_Board.EmptySquare)
                {
                    hasMoreMoves = true;
                }
                else if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer - 1] == m_Board.PlayerOneSign || m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer - 1] == m_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfPlayer - 2 >= 0 && i_ColStartingPointOfPlayer - 2 >= 0)
                    {
                        if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 2, i_ColStartingPointOfPlayer - 2] == m_Board.EmptySquare)
                        {
                            hasMoreMoves = true;
                        }
                    }
                }
            }

            if (!hasMoreMoves && i_RowStartingPointOfPlayer - 1 >= 0 && i_ColStartingPointOfPlayer + 1 < boardSize)
            {
                if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer + 1] == m_Board.EmptySquare)
                {
                    hasMoreMoves = true;
                }
                else if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer + 1] == m_Board.PlayerOneSign || m_Board.GameBoard[i_RowStartingPointOfPlayer - 1, i_ColStartingPointOfPlayer + 1] == m_Board.PlayerOneKingSign)
                {
                    if (!hasMoreMoves && i_RowStartingPointOfPlayer - 2 >= 0 && i_ColStartingPointOfPlayer + 2 < boardSize)
                    {
                        if (m_Board.GameBoard[i_RowStartingPointOfPlayer - 2, i_ColStartingPointOfPlayer + 2] == m_Board.EmptySquare)
                        {
                            hasMoreMoves = true;
                        }
                    }
                }
            }

            if (!hasMoreMoves && i_RowStartingPointOfPlayer + 1 < boardSize && i_ColStartingPointOfPlayer - 1 >= 0)
            {
                if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer - 1] == m_Board.EmptySquare)
                {
                    hasMoreMoves = true;
                }
                else if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer - 1] == m_Board.PlayerOneSign || m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer - 1] == m_Board.PlayerOneKingSign)
                {
                    if (!hasMoreMoves && i_RowStartingPointOfPlayer + 2 < boardSize && i_ColStartingPointOfPlayer - 2 >= 0)
                    {
                        if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 2, i_ColStartingPointOfPlayer - 2] == m_Board.EmptySquare)
                        {
                            hasMoreMoves = true;
                        }
                    }
                }
            }

            if (!hasMoreMoves && i_RowStartingPointOfPlayer + 1 < boardSize && i_ColStartingPointOfPlayer + 1 < boardSize)
            {
                if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer + 1] == m_Board.EmptySquare)
                {
                    hasMoreMoves = true;
                }
                else if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer + 1] == m_Board.PlayerOneSign || m_Board.GameBoard[i_RowStartingPointOfPlayer + 1, i_ColStartingPointOfPlayer + 1] == m_Board.PlayerOneKingSign)
                {
                    if (!hasMoreMoves && i_RowStartingPointOfPlayer + 1 < boardSize && i_ColStartingPointOfPlayer + 2 < boardSize)
                    {
                        if (m_Board.GameBoard[i_RowStartingPointOfPlayer + 2, i_ColStartingPointOfPlayer + 2] == m_Board.EmptySquare)
                        {
                            hasMoreMoves = true;
                        }
                    }
                }
            }

            return hasMoreMoves;
        }

        private void checkForNewKing(ref char io_PlayerSign, Point i_DestinationPointOfPlayer)
        {
            bool isKingWasMade = false;

            isKingWasMade = m_Board.CheckIfKingWasMade(io_PlayerSign, i_DestinationPointOfPlayer);
            if (isKingWasMade)
            {
                m_Board.ChangePlayerSignToKing(ref io_PlayerSign, i_DestinationPointOfPlayer);
                updateScoreOfPlayerAfterTurningKing(io_PlayerSign);
            }
        }

        public bool CheckForATie()
        {
            bool isItATie = false;
            if (PlayerOne.HasMoves == false && m_PlayerTwo.HasMoves == false)
            {   // no more moves, its a tie
                m_PlayerOne.Score++;
                m_PlayerTwo.Score++;
                m_Status = eGameStatus.EndOfRound;
                isItATie = true;
            }

            return isItATie;
        }

        public bool CheckIfCurrentPlayerWon()
        {
            bool isCurrentPlayerWon = false;
            switch (CurrentTurn)
            {
                case eCurrentShapeTurn.Circle:
                    if (PlayerOne.Coins == 0 || PlayerOne.HasMoves == false)
                    {   // Circle wins if ex coins is 0 or ex has no more valid moves
                        PlayerTwo.Score++;
                        isCurrentPlayerWon = true;
                        Status = eGameStatus.EndOfRound;
                    }

                    break;
                case eCurrentShapeTurn.Ex:
                    if (PlayerTwo.Coins == 0 || PlayerTwo.HasMoves == false)
                    {   // Ex wins if circle coins is 0 or ex has no more valid moves
                        PlayerOne.Score++;
                        isCurrentPlayerWon = true;
                        Status = eGameStatus.EndOfRound;
                    }

                    break;
                default:
                    break;
            }

            return isCurrentPlayerWon;
        }

        public bool CheckIfCurrentPlayerForfeit()
        {
            bool isCurrentPlayerForfeit = false;
            switch (CurrentTurn)
            {
                case eCurrentShapeTurn.Circle:
                    if (m_Forfeit == true)
                    {
                        PlayerOne.Score++;
                        isCurrentPlayerForfeit = true;
                        Status = eGameStatus.EndOfRound;
                    }

                    break;
                case eCurrentShapeTurn.Ex:
                    if (m_Forfeit == true)
                    {
                        PlayerTwo.Score++;
                        isCurrentPlayerForfeit = true;
                        Status = eGameStatus.EndOfRound;
                    }

                    break;
                default:
                    break;
            }

            return isCurrentPlayerForfeit;
        }

        public void InitializeRematch()
        {
            m_PlayerOne.Reset();
            m_PlayerTwo.Reset();
            InitializeCoins();
            m_Board.BuildBoard();
            m_CurrentShapeTurn = eCurrentShapeTurn.Ex;
            m_Forfeit = false;
            m_CanEatAgain = false;
            m_EatPlayer = false;
            m_FirstTurnOfPlayerCanEat = false;
            Status = eGameStatus.Play;
        }

        public void SwitchTurns()
        {
            switch (CurrentTurn)
            {
                case eCurrentShapeTurn.Circle:
                    m_CurrentShapeTurn = eCurrentShapeTurn.Ex;
                    break;
                case eCurrentShapeTurn.Ex:
                    m_CurrentShapeTurn = eCurrentShapeTurn.Circle;
                    break;
                default:
                    break;
            }
        }

        public bool ExtraAITurn
        {   
            get
            {
                return m_AI.ExtraTurn;
            }
        }

        public void GetAnAIMove(ref Point io_StartingComputerPoint, ref Point io_DestinationComputerPoint)
        {
            bool canAIMakeEatingMove = m_AI.MakeComputerEatingMove(m_Board, ref io_StartingComputerPoint, ref io_DestinationComputerPoint);
            if (canAIMakeEatingMove == false)
            {   // computer cant make an eating move
                m_AI.MakeComputerRegularMove(m_Board, ref io_StartingComputerPoint, ref io_DestinationComputerPoint); // make a regular move
            }
        }
    }
}