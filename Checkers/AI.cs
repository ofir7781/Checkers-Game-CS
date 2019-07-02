using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers
{
    public class AI
    {
        private const string k_ComputerDefaultName = "Computer";
        private bool m_ExtraTurn;
        private Point m_CurrentComputerSoldierSaverForExtraTurn;

        public string ComputerDefaultName
        {
            get
            {
                return k_ComputerDefaultName;
            }
        } 

        public bool ExtraTurn
        {
            get
            {
                return m_ExtraTurn;
            }
        }

        public bool MakeComputerEatingMove(Board i_Board, ref Point io_StartingComputerPoint, ref Point io_DestinationComputerPoint)
        {
            bool canComputerMakeEatingMove = false;
            List<Point> soildersThatCanEat = null;
            List<Point> listOfAvailableMovesOfPointThatCanEat = null;

            if (m_ExtraTurn == true)
            {   // if we are in a middle of a turn, and the computer can eat again, no need to create a list of soldiers that can eat, just list of moves from the same last soldier
                listOfAvailableMovesOfPointThatCanEat = createListOfAvailableMovesOfPointThatCanEat(m_CurrentComputerSoldierSaverForExtraTurn, i_Board);
                io_DestinationComputerPoint = chooseRandomPointFromList(listOfAvailableMovesOfPointThatCanEat);
                canComputerMakeEatingMove = true;
                io_StartingComputerPoint = m_CurrentComputerSoldierSaverForExtraTurn;
                m_CurrentComputerSoldierSaverForExtraTurn = io_DestinationComputerPoint;
                m_ExtraTurn = checkForExtraEatingMove(io_DestinationComputerPoint, io_StartingComputerPoint, i_Board);
            }
            else
            {   // its a fresh turn, so we need to create a list of soldiers that can eat 
                soildersThatCanEat = createListOfSoildersThatCanEat(i_Board);
                if (soildersThatCanEat.Count > 0)
                {   // here we roll one of the soldiers that can eat
                    io_StartingComputerPoint = chooseRandomPointFromList(soildersThatCanEat);

                    // now we create its list of eating moves
                    listOfAvailableMovesOfPointThatCanEat = createListOfAvailableMovesOfPointThatCanEat(io_StartingComputerPoint, i_Board);

                    // roll one of the eating moves he can make
                    io_DestinationComputerPoint = chooseRandomPointFromList(listOfAvailableMovesOfPointThatCanEat);
                    canComputerMakeEatingMove = true;
                    m_CurrentComputerSoldierSaverForExtraTurn = io_DestinationComputerPoint;
                    m_ExtraTurn = checkForExtraEatingMove(io_DestinationComputerPoint, io_StartingComputerPoint, i_Board);
                }
            }

            return canComputerMakeEatingMove;
        }

        private List<Point> createListOfSoildersThatCanEat(Board i_Board)
        {
            List<Point> listOfSoildersThatCanEat = new List<Point>();
            int boardSize = i_Board.Size;

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (i_Board.GameBoard[row, col] == i_Board.PlayerTwoSign || i_Board.GameBoard[row, col] == i_Board.PlayerTwoKingSign)
                    {
                        if (i_Board.GameBoard[row, col] == i_Board.PlayerTwoSign)
                        {
                            bool soldierCanEat = checkIfComputerSoilderCanEat(row, col, i_Board);
                            if (soldierCanEat == true)
                            {
                                Point solderThatCanEat = new Point(row, col);
                                listOfSoildersThatCanEat.Add(solderThatCanEat);
                            }
                        }
                        else
                        {
                            bool kingCanEat = checkIfComputerKingCanEat(row, col, i_Board);
                            if (kingCanEat == true)
                            {
                                Point kingrThatCanEat = new Point(row, col);
                                listOfSoildersThatCanEat.Add(kingrThatCanEat);
                            }
                        }
                    }
                }
            }

            return listOfSoildersThatCanEat;
        }

        private bool checkIfComputerSoilderCanEat(int i_RowStartingPointOfComputer, int i_ColStartingPointOfComputer, Board i_Board)
        {
            int boardSize = i_Board.Size;
            bool isSoldierCanEat = false;

            if (i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer - 1 >= 0)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1] == i_Board.PlayerOneSign || i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1] == i_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfComputer + 2 < boardSize && i_ColStartingPointOfComputer - 2 >= 0)
                    {
                        if (i_Board.GameBoard[i_RowStartingPointOfComputer + 2, i_ColStartingPointOfComputer - 2] == i_Board.EmptySquare)
                        {
                            isSoldierCanEat = true;
                        }
                    }
                }
            }

            if (!isSoldierCanEat && i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer + 1 < boardSize)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1] == i_Board.PlayerOneSign || i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1] == i_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfComputer + 2 < boardSize && i_ColStartingPointOfComputer + 2 < boardSize)
                    {
                        if (i_Board.GameBoard[i_RowStartingPointOfComputer + 2, i_ColStartingPointOfComputer + 2] == i_Board.EmptySquare)
                        {
                            isSoldierCanEat = true;
                        }
                    }
                }
            }

            return isSoldierCanEat;
        }

        private bool checkIfComputerKingCanEat(int i_RowStartingPointOfComputer, int i_ColStartingPointOfComputer, Board i_Board)
        {
            int boardSize = i_Board.Size;
            bool kingCanEat = false;

            if (i_RowStartingPointOfComputer - 1 >= 0 && i_ColStartingPointOfComputer - 1 >= 0)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer - 1] == i_Board.PlayerOneSign || i_Board.GameBoard[i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer - 1] == i_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfComputer - 2 >= 0 && i_ColStartingPointOfComputer - 2 >= 0)
                    {
                        if (i_Board.GameBoard[i_RowStartingPointOfComputer - 2, i_ColStartingPointOfComputer - 2] == i_Board.EmptySquare)
                        {
                            kingCanEat = true;
                        }
                    }
                }
            }

            if (!kingCanEat && i_RowStartingPointOfComputer - 1 >= 0 && i_ColStartingPointOfComputer + 1 < boardSize)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer + 1] == i_Board.PlayerOneSign || i_Board.GameBoard[i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer + 1] == i_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfComputer - 2 >= 0 && i_ColStartingPointOfComputer + 2 < boardSize)
                    {
                        if (i_Board.GameBoard[i_RowStartingPointOfComputer - 2, i_ColStartingPointOfComputer + 2] == i_Board.EmptySquare)
                        {
                            kingCanEat = true;
                        }
                    }
                }
            }

            if (!kingCanEat && i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer - 1 >= 0)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1] == i_Board.PlayerOneSign || i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1] == i_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfComputer + 2 < boardSize && i_ColStartingPointOfComputer - 2 >= 0)
                    {
                        if (i_Board.GameBoard[i_RowStartingPointOfComputer + 2, i_ColStartingPointOfComputer - 2] == i_Board.EmptySquare)
                        {
                            kingCanEat = true;
                        }
                    }
                }
            }

            if (!kingCanEat && i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer + 1 < boardSize)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1] == i_Board.PlayerOneSign || i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1] == i_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfComputer + 2 < boardSize && i_ColStartingPointOfComputer + 2 < boardSize)
                    {
                        if (i_Board.GameBoard[i_RowStartingPointOfComputer + 2, i_ColStartingPointOfComputer + 2] == i_Board.EmptySquare)
                        {
                            kingCanEat = true;
                        }
                    }
                }
            }

            return kingCanEat;
        }

        private List<Point> createListOfAvailableMovesOfPointThatCanEat(Point i_RandomComputerEatingPoint, Board i_Board)
        {
            List<Point> listOfAvailableMovesOfPointThatCanEat = new List<Point>();
            char playerSign = i_Board.GameBoard[i_RandomComputerEatingPoint.Y, i_RandomComputerEatingPoint.X];

            if (playerSign == i_Board.PlayerTwoSign)
            {
                listOfAvailableMovesOfPointThatCanEat = findAvailableMovesOfComputerSoilderThatCanEat(i_RandomComputerEatingPoint.Y, i_RandomComputerEatingPoint.X, i_Board);
            }
            else if (playerSign == i_Board.PlayerTwoKingSign)
            {
                listOfAvailableMovesOfPointThatCanEat = findAvailableMovesOfComputerSoilderKingThatCanEat(i_RandomComputerEatingPoint.Y, i_RandomComputerEatingPoint.X, i_Board);
            }

            return listOfAvailableMovesOfPointThatCanEat;
        }

        private List<Point> findAvailableMovesOfComputerSoilderThatCanEat(int i_RowStartingPointOfComputer, int i_ColStartingPointOfComputer, Board i_Board)
        {
            int boardSize = i_Board.Size;
            List<Point> availableMovesOfComputerSoilder = new List<Point>(2);
            if (i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer - 1 >= 0)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1] == i_Board.PlayerOneSign || i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1] == i_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfComputer + 2 < boardSize && i_ColStartingPointOfComputer - 2 >= 0)
                    {
                        if (i_Board.GameBoard[i_RowStartingPointOfComputer + 2, i_ColStartingPointOfComputer - 2] == i_Board.EmptySquare)
                        {
                            availableMovesOfComputerSoilder.Add(new Point(i_RowStartingPointOfComputer + 2, i_ColStartingPointOfComputer - 2));
                        }
                    }
                }
            }

            if (i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer + 1 < boardSize)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1] == i_Board.PlayerOneSign || i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1] == i_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfComputer + 2 < boardSize && i_ColStartingPointOfComputer + 2 < boardSize)
                    {
                        if (i_Board.GameBoard[i_RowStartingPointOfComputer + 2, i_ColStartingPointOfComputer + 2] == i_Board.EmptySquare)
                        {
                            availableMovesOfComputerSoilder.Add(new Point(i_RowStartingPointOfComputer + 2, i_ColStartingPointOfComputer + 2));
                        }
                    }
                }
            }

            return availableMovesOfComputerSoilder;
        }

        private List<Point> findAvailableMovesOfComputerSoilderKingThatCanEat(int i_RowStartingPointOfComputer, int i_ColStartingPointOfComputer, Board i_Board)
        {
            int boardSize = i_Board.Size;
            List<Point> availableMovesOfComputerSoilderKing = new List<Point>(4);
            if (i_RowStartingPointOfComputer - 1 >= 0 && i_ColStartingPointOfComputer - 1 >= 0)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer - 1] == i_Board.PlayerOneSign || i_Board.GameBoard[i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer - 1] == i_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfComputer - 2 >= 0 && i_ColStartingPointOfComputer - 2 >= 0)
                    {
                        if (i_Board.GameBoard[i_RowStartingPointOfComputer - 2, i_ColStartingPointOfComputer - 2] == i_Board.EmptySquare)
                        {
                            availableMovesOfComputerSoilderKing.Add(new Point(i_RowStartingPointOfComputer - 2, i_ColStartingPointOfComputer - 2));
                        }
                    }
                }
            }

            if (i_RowStartingPointOfComputer - 1 >= 0 && i_ColStartingPointOfComputer + 1 < boardSize)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer + 1] == i_Board.PlayerOneSign || i_Board.GameBoard[i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer + 1] == i_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfComputer - 2 >= 0 && i_ColStartingPointOfComputer + 2 < boardSize)
                    {
                        if (i_Board.GameBoard[i_RowStartingPointOfComputer - 2, i_ColStartingPointOfComputer + 2] == i_Board.EmptySquare)
                        {
                            availableMovesOfComputerSoilderKing.Add(new Point(i_RowStartingPointOfComputer - 2, i_ColStartingPointOfComputer + 2));
                        }
                    }
                }
            }

            if (i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer - 1 >= 0)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1] == i_Board.PlayerOneSign || i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1] == i_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfComputer + 2 < boardSize && i_ColStartingPointOfComputer - 2 >= 0)
                    {
                        if (i_Board.GameBoard[i_RowStartingPointOfComputer + 2, i_ColStartingPointOfComputer - 2] == i_Board.EmptySquare)
                        {
                            availableMovesOfComputerSoilderKing.Add(new Point(i_RowStartingPointOfComputer + 2, i_ColStartingPointOfComputer - 2));
                        }
                    }
                }
            }

            if (i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer + 1 < boardSize)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1] == i_Board.PlayerOneSign || i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1] == i_Board.PlayerOneKingSign)
                {
                    if (i_RowStartingPointOfComputer + 2 < boardSize && i_ColStartingPointOfComputer + 2 < boardSize)
                    {
                        if (i_Board.GameBoard[i_RowStartingPointOfComputer + 2, i_ColStartingPointOfComputer + 2] == i_Board.EmptySquare)
                        {
                            availableMovesOfComputerSoilderKing.Add(new Point(i_RowStartingPointOfComputer + 2, i_ColStartingPointOfComputer + 2));
                        }
                    }
                }
            }

            return availableMovesOfComputerSoilderKing;
        }

        public void MakeComputerRegularMove(Board i_Board, ref Point io_StartingComputerPoint, ref Point io_DestinationComputerPoint)
        {
            List<Point> soildersThatCanMove = null;
            List<Point> listOfAvailableMovesOfPointThatCanMove = null;

            // create a list of soldiers that can move
            soildersThatCanMove = createListOfSoildersThatCanMove(i_Board);

            if (soildersThatCanMove.Count > 0)
            {   // roll a soldier from the list
                io_StartingComputerPoint = chooseRandomPointFromList(soildersThatCanMove);

                // create a list of moves that he can make
                listOfAvailableMovesOfPointThatCanMove = createListOfAvailableMovesOfPoint(io_StartingComputerPoint, i_Board);

                // rolls its move from the list
                io_DestinationComputerPoint = chooseRandomPointFromList(listOfAvailableMovesOfPointThatCanMove);
            }
        }

        private List<Point> createListOfSoildersThatCanMove(Board i_Board)
        {
            List<Point> listOfSoildersThatCanMove = new List<Point>();
            int boardSize = i_Board.Size;

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (i_Board.GameBoard[row, col] == i_Board.PlayerTwoSign || i_Board.GameBoard[row, col] == i_Board.PlayerTwoKingSign)
                    {
                        if (i_Board.GameBoard[row, col] == i_Board.PlayerTwoSign)
                        {
                            bool soldierCanMove = checkIfComputerSoilderCanMove(row, col, i_Board);
                            if (soldierCanMove == true)
                            {
                                Point solderThatCanMove = new Point(row, col);
                                listOfSoildersThatCanMove.Add(solderThatCanMove);
                            }
                        }
                        else
                        {
                            bool kingCanMove = checkIfComputerSoilderKingCanMove(row, col, i_Board);
                            if (kingCanMove == true)
                            {
                                Point kingrThatCanMove = new Point(row, col);
                                listOfSoildersThatCanMove.Add(kingrThatCanMove);
                            }
                        }
                    }
                }
            }

            return listOfSoildersThatCanMove;
        }

        private bool checkIfComputerSoilderCanMove(int i_RowStartingPointOfComputer, int i_ColStartingPointOfComputer, Board i_Board)
        {
            int boardSize = i_Board.Size;
            bool computerSoliderCanMove = false;

            if (i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer - 1 >= 0)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1] == i_Board.EmptySquare)
                {
                    computerSoliderCanMove = true;
                }  
            }

            if (!computerSoliderCanMove && i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer + 1 < boardSize)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1] == i_Board.EmptySquare)
                {
                    computerSoliderCanMove = true;
                }
            }

            return computerSoliderCanMove;
        }

        private bool checkIfComputerSoilderKingCanMove(int i_RowStartingPointOfComputer, int i_ColStartingPointOfComputer, Board i_Board)
        {
            int boardSize = i_Board.Size;
            bool computerSoliderKingCanMove = false;
            if (i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer - 1 >= 0)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1] == i_Board.EmptySquare)
                {
                    computerSoliderKingCanMove = true;
                }
            }

            if (!computerSoliderKingCanMove && i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer + 1 < boardSize)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1] == i_Board.EmptySquare)
                {
                    computerSoliderKingCanMove = true;
                }
            }

            if (!computerSoliderKingCanMove && i_RowStartingPointOfComputer - 1 >= 0 && i_ColStartingPointOfComputer - 1 >= 0)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer - 1] == i_Board.EmptySquare)
                {
                    computerSoliderKingCanMove = true;
                }
            }

            if (!computerSoliderKingCanMove && i_RowStartingPointOfComputer - 1 >= 0 && i_ColStartingPointOfComputer + 1 < boardSize)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer + 1] == i_Board.EmptySquare)
                {
                    computerSoliderKingCanMove = true;
                }
            }

            return computerSoliderKingCanMove;
        }

        private List<Point> createListOfAvailableMovesOfPoint(Point i_RandomComputerPointBeforeMoving, Board i_Board)
        {
            List<Point> listOfSoildersThatCanMove = new List<Point>();
            int boardSize = i_Board.Size;

            char playerSign = i_Board.GameBoard[i_RandomComputerPointBeforeMoving.Y, i_RandomComputerPointBeforeMoving.X];

            if (playerSign == i_Board.PlayerTwoSign)
            {
                listOfSoildersThatCanMove = findAvailableMovesOfComputerSoilderThatCanMove(i_RandomComputerPointBeforeMoving.Y, i_RandomComputerPointBeforeMoving.X, i_Board);
            }
            else if (playerSign == i_Board.PlayerTwoKingSign)
            {
                listOfSoildersThatCanMove = findAvailableMovesOfComputerSoilderKingThatCanMove(i_RandomComputerPointBeforeMoving.Y, i_RandomComputerPointBeforeMoving.X, i_Board);
            }

            return listOfSoildersThatCanMove;
        }

        private List<Point> findAvailableMovesOfComputerSoilderThatCanMove(int i_RowStartingPointOfComputer, int i_ColStartingPointOfComputer, Board i_Board)
        {
            int boardSize = i_Board.Size;
            List<Point> availableMovesOfComputerSoilder = new List<Point>(2);
            if (i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer - 1 >= 0)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1] == i_Board.EmptySquare)
                {
                    availableMovesOfComputerSoilder.Add(new Point(i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1));
                }
            }

            if (i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer + 1 < boardSize)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1] == i_Board.EmptySquare)
                {
                    availableMovesOfComputerSoilder.Add(new Point(i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1));
                }
            }

            return availableMovesOfComputerSoilder;
        }

        private List<Point> findAvailableMovesOfComputerSoilderKingThatCanMove(int i_RowStartingPointOfComputer, int i_ColStartingPointOfComputer, Board i_Board)
        {
            int boardSize = i_Board.Size;
            List<Point> availableMovesOfComputerSoilderKing = new List<Point>(4);

            if (i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer - 1 >= 0)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1] == i_Board.EmptySquare)
                {
                    availableMovesOfComputerSoilderKing.Add(new Point(i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer - 1));
                }
            }

            if (i_RowStartingPointOfComputer + 1 < boardSize && i_ColStartingPointOfComputer + 1 < boardSize)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1] == i_Board.EmptySquare)
                {
                    availableMovesOfComputerSoilderKing.Add(new Point(i_RowStartingPointOfComputer + 1, i_ColStartingPointOfComputer + 1));
                }
            }

            if (i_RowStartingPointOfComputer - 1 >= 0 && i_ColStartingPointOfComputer - 1 >= 0)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer - 1] == i_Board.EmptySquare)
                {
                    availableMovesOfComputerSoilderKing.Add(new Point(i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer - 1));
                }
            }

            if (i_RowStartingPointOfComputer - 1 >= 0 && i_ColStartingPointOfComputer + 1 < boardSize)
            {
                if (i_Board.GameBoard[i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer + 1] == i_Board.EmptySquare)
                {
                    availableMovesOfComputerSoilderKing.Add(new Point(i_RowStartingPointOfComputer - 1, i_ColStartingPointOfComputer + 1));
                }
            }

            return availableMovesOfComputerSoilderKing;
        }

        private Point chooseRandomPointFromList(List<Point> i_SoildersThatCanEat)
        {
            Point randomComputerEatingPoint;
            Random randomObject = new Random();
            int randomIndexInList = randomObject.Next(0, i_SoildersThatCanEat.Count);
            randomComputerEatingPoint = i_SoildersThatCanEat[randomIndexInList];
            return randomComputerEatingPoint;
        }

        private string convertComputerCommandFromPointTostring(Point i_RandomComputerPointBeforeEating, Point i_RandomComputerPointAfterEating, Board i_Board)
        {
            StringBuilder commandFromComputer = new StringBuilder();
            List<Point> pointBeforeAndAfterEating = new List<Point>();

            pointBeforeAndAfterEating.Add(i_RandomComputerPointBeforeEating);
            pointBeforeAndAfterEating.Add(i_RandomComputerPointAfterEating);

            for (int i = 0; i < pointBeforeAndAfterEating.Count; i++)
            {
                if (i == 1)
                {
                    commandFromComputer.Append('>');
                }

                commandFromComputer.Append((char)(pointBeforeAndAfterEating[i].X + i_Board.FirstUpperCaseFrame));
                commandFromComputer.Append((char)(pointBeforeAndAfterEating[i].Y + i_Board.FirstLowerCaseFrame));
            }

            return commandFromComputer.ToString();
        }

        private bool checkForExtraEatingMove(Point i_ComputerPositionAfterEatingMove, Point i_ComputerPositionBeforeEatingMove, Board i_Board)
        {
            bool canEatAgain = false;
            char playerSign = i_Board.GameBoard[i_ComputerPositionBeforeEatingMove.Y, i_ComputerPositionBeforeEatingMove.X];

            if (playerSign == i_Board.PlayerTwoSign)
            {
                canEatAgain = checkIfComputerSoilderCanEat(i_ComputerPositionAfterEatingMove.Y, i_ComputerPositionAfterEatingMove.X, i_Board);
            }
            else
            {
                canEatAgain = checkIfComputerKingCanEat(i_ComputerPositionAfterEatingMove.Y, i_ComputerPositionAfterEatingMove.X, i_Board);
            }

            if (i_ComputerPositionAfterEatingMove.Y == i_Board.Size - 1)
            {
                canEatAgain = checkIfComputerKingCanEat(i_ComputerPositionAfterEatingMove.Y, i_ComputerPositionAfterEatingMove.X, i_Board);
            }

            return canEatAgain;
        }
    }
}
