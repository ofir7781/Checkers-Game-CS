using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers
{
    public class UserInterface
    {
        private const string k_WelcomeMsg = "Welcome to Checkers by David Sidi and Ofir Cohen!";
        private const string k_GetPlayerOneNameMsg = "Please enter your name:";
        private const string k_GetPlayerTwoNameMsg = "Please enter second player name:";
        private const string k_GetBoardSizeMsg = "Please insert the size of the board you wish to play: (6, 8, or 10)";
        private const string k_AskForRematchMsg = "Would you like to play again? Y / N";
        private const string k_ChooseGameModeMsg =
@"Please choose the mode you wish to play:
1. Player Vs Player
2. Player Vs Computer";

        private const string k_InvalidNameLengthMsg = "Invalid name! acceptable name length is between 1 - 20 characters.";
        private const string k_InvalidBoardSizeMsg = "Invalid board size! please choose between 6, 8 or 10.";
        private const string k_InvalidModeMsg = "Invalid choice! please choose between 1 or 2.";
        private const string k_InvalidTurnSyntaxMsg = "Invalid input! please use the correct format: COLrow>COLrow or Q to forfeit.";
        private const string k_InvalidMoveMsg = "Illegal move!";
        private const string k_InvalidForfeitMsg = "Invalid input! current player cannot forfeit.";
        private const string k_InvalidRematchSyntaxMsg = "Invalid input! please type Y or N.";
        private const int k_MaxNameLength = 20;
        private const string k_SmallBoardSize = "6";
        private const string k_MediumBoardSize = "8";
        private const string k_LargeBoardSize = "10";
        private const string k_PlayerVsPlayerChoice = "1";
        private const string k_PlayerVsComputerChoice = "2";
        private const int k_ValidTurnSyntaxLength = 5;
        private const int k_ValidForfeitSyntaxLength = 1;
        private const int k_ValidRematchSyntaxLength = 1;
        private const char k_ForfeitSign = 'Q';
        private const char k_ValidRematchYesSign = 'Y';
        private const char k_ValidRematchNoSign = 'N';

        private LogicUnit m_LogicUnit;
        private string playerOneLoggerSaver = string.Empty; // DEBUG delete later
        private string playerTwoLoggerSaver = string.Empty; // DEBUG delete later

        public enum eMessages
        {
            Welcome,
            GetPlayerOneName,
            InvalidNameLength,
            GetBoardSize,
            InvalidBoardSize,
            ChooseGameMode,
            InvalidGameMode,
            GetPlayerTwoName,
            InvalidTurnSyntax,
            InvalidInputMove,
            InvalidForfeit,
            AskForRematch,
            InvalidRematchSyntax,
        }

        public void Run()
        {
            initializeGame();
            switch (m_LogicUnit.Mode)
            {
                case LogicUnit.eGameMode.PlayerVsPlayer:
                    playPlayerVsPlayer();
                    break;
                case LogicUnit.eGameMode.PlayerVsComputer:
                    playPlayerVsComputer();
                    break;
                default:
                    break;
            }
        }

        private void initializeGame()
        {
            m_LogicUnit = new LogicUnit();
            displayMessages(eMessages.Welcome);
            string playerOneName = getPlayerOneNameScreen();
            m_LogicUnit.CreatePlayerOne(playerOneName);
            int boardSize = getBoardSizeScreen();
            m_LogicUnit.CreateBoard(boardSize);
            getGameModeScreen();
            switch (m_LogicUnit.Mode)
            {
                case LogicUnit.eGameMode.PlayerVsPlayer:
                    string playerTwoName = getPlayerTwoNameScreen();
                    m_LogicUnit.CreatePlayerTwo(playerTwoName);
                    break;
                case LogicUnit.eGameMode.PlayerVsComputer:
                    m_LogicUnit.InitializeAI();
                    m_LogicUnit.CreatePlayerTwo(m_LogicUnit.ComputerDefaultName);
                    break;
                default:
                    break;
            }

            m_LogicUnit.InitializeCoins();
            m_LogicUnit.Status = LogicUnit.eGameStatus.Play;
        }

        private string getPlayerOneNameScreen()
        {
            displayMessages(eMessages.GetPlayerOneName);
            string playerOneName = Console.ReadLine();
            while (string.IsNullOrEmpty(playerOneName) || char.IsWhiteSpace(playerOneName[0]) || playerOneName.Length > k_MaxNameLength)
            {
                Ex02.ConsoleUtils.Screen.Clear();
                displayMessages(eMessages.InvalidNameLength);
                displayMessages(eMessages.GetPlayerOneName);
                playerOneName = Console.ReadLine();
            }

            return playerOneName;
        }

        private int getBoardSizeScreen()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            displayMessages(eMessages.GetBoardSize);
            string boardSizeString = Console.ReadLine();
            while (boardSizeString != k_SmallBoardSize && boardSizeString != k_MediumBoardSize && boardSizeString != k_LargeBoardSize)
            {
                Ex02.ConsoleUtils.Screen.Clear();
                displayMessages(eMessages.GetBoardSize);
                displayMessages(eMessages.InvalidBoardSize);
                boardSizeString = Console.ReadLine();
            }

            return int.Parse(boardSizeString);
        }

        private void getGameModeScreen()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            displayMessages(eMessages.ChooseGameMode);
            string gameModeString = Console.ReadLine();
            while (gameModeString != k_PlayerVsPlayerChoice && gameModeString != k_PlayerVsComputerChoice)
            {
                Ex02.ConsoleUtils.Screen.Clear();
                displayMessages(eMessages.ChooseGameMode);
                displayMessages(eMessages.InvalidGameMode);
                gameModeString = Console.ReadLine();
            }

            int userChoice = int.Parse(gameModeString);
            switch (userChoice)
            {
                case 1:
                    m_LogicUnit.Mode = LogicUnit.eGameMode.PlayerVsPlayer;
                    break;
                case 2:
                    m_LogicUnit.Mode = LogicUnit.eGameMode.PlayerVsComputer;
                    break;
                default:
                    break;
            }
        }

        private string getPlayerTwoNameScreen()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            displayMessages(eMessages.GetPlayerTwoName);
            string playerTwoName = Console.ReadLine();
            while (string.IsNullOrEmpty(playerTwoName) || char.IsWhiteSpace(playerTwoName[0]) || playerTwoName.Length > k_MaxNameLength)
            {
                Ex02.ConsoleUtils.Screen.Clear();
                displayMessages(eMessages.GetPlayerTwoName);
                displayMessages(eMessages.InvalidNameLength);
                playerTwoName = Console.ReadLine();
            }

            return playerTwoName;
        }

        private void playPlayerVsPlayer()
        {
            while (m_LogicUnit.Status != LogicUnit.eGameStatus.Quit)
            {
                printBoard();
                displayLastTurnAndCurrentPlayer(); // display the moves dialog
                handleHumanTurn(); // get a valid input and preform it
                manageTasksBeforeNextTurn(); // check if either player has won, lost or its a tie
            }
        }

        private void handleHumanTurn()
        {
            bool continueTurn = true;
            Point startingPointOfPlayer = null;
            Point destinationPointOfPlayer = null;
            string currentCommand = string.Empty;
            while (continueTurn == true)
            {
                currentCommand = getCommandFromCurrentPlayer(); // getting the input from the user this is after syntax verification
                if (currentCommand[0] == k_ForfeitSign)
                {   // in case Q was entered to forfeit, check if player can forfeit
                    bool isValidForfeit = m_LogicUnit.CheckIfValidForfeit();
                    if (isValidForfeit == false)
                    {   // invalid forfeit, display error message and continue turn
                        displayMessages(eMessages.InvalidForfeit);
                        continueTurn = true;
                    }
                    else
                    {
                        // valid forfeit, turn is over
                        continueTurn = false;
                    }
                }
                else
                {   // valid syntax was entered and its not forfeit, we parse the command from string to points and then preforming it
                    parseCommandFromStringToPoints(currentCommand, ref startingPointOfPlayer, ref destinationPointOfPlayer);
                    if (m_LogicUnit.PreformMove(startingPointOfPlayer, destinationPointOfPlayer) == false)
                    {   // preform the move, if false returned, it was an invalid move, we display message and turn continues
                        displayMessages(eMessages.InvalidInputMove);
                        continueTurn = true;
                    }
                    else
                    {   // move was valid and it was preformed
                        printBoard(); // print the board after a valid move
                        if (m_LogicUnit.CanEatAgain == true)
                        {   // check if extra turn is needed for the current user (for double or higher jumping in a row)
                            displayExtraTurnScreen();
                            continueTurn = true;
                        }
                        else
                        {   // if not, then the current turn is over
                            continueTurn = false;
                        }
                    }
                }
            }
        }

        private string readCommandFromConsole(Player i_CurrentPlayer)
        {
            bool isValidSyntax = false;
            while (!isValidSyntax)
            {   // keep looping untill a valid syntax was entered
                i_CurrentPlayer.LastTurn = Console.ReadLine();
                isValidSyntax = checkIfTurnSyntaxIsValid(i_CurrentPlayer); // check if the syntax is valid
                if (isValidSyntax == false)
                {
                    displayMessages(eMessages.InvalidTurnSyntax);
                }
            }

            return i_CurrentPlayer.LastTurn;
        }

        private void parseCommandFromStringToPoints(string i_CurrentCommand, ref Point io_StartingPlayerPoint, ref Point io_DestinationPlayerPoint)
        {
            int xStartingPointOfPlayer = m_LogicUnit.FindPlaceOfLetterOnBoard(i_CurrentCommand[0]);
            int yStartingPointOfPlayer = m_LogicUnit.FindPlaceOfLetterOnBoard(i_CurrentCommand[1]);
            io_StartingPlayerPoint = new Point(yStartingPointOfPlayer, xStartingPointOfPlayer);
            int xDestinationPointOfPlayer = m_LogicUnit.FindPlaceOfLetterOnBoard(i_CurrentCommand[3]);
            int yDestinationPointOfPlayer = m_LogicUnit.FindPlaceOfLetterOnBoard(i_CurrentCommand[4]);
            io_DestinationPlayerPoint = new Point(yDestinationPointOfPlayer, xDestinationPointOfPlayer);
        }

        private bool checkIfTurnSyntaxIsValid(Player i_CurrentPlayer)
        {
            bool isValidInput = true;
            bool isValidForfeitInput = false;
            int inputStringLength = i_CurrentPlayer.LastTurn.Length;
            char rangeOfLowerCaseFrame = m_LogicUnit.Board.FirstLowerCaseFrame;
            rangeOfLowerCaseFrame += (char)m_LogicUnit.Board.Size;
            char rangeOfUpperCaseFrame = m_LogicUnit.Board.FirstLowerCaseFrame;
            rangeOfUpperCaseFrame += (char)m_LogicUnit.Board.Size;

            if (inputStringLength != k_ValidTurnSyntaxLength)
            {
                if (inputStringLength == k_ValidForfeitSyntaxLength && i_CurrentPlayer.LastTurn[0] == k_ForfeitSign)
                {
                    isValidForfeitInput = true;
                }
                else
                {
                    isValidInput = false;
                }
            }

            for (int i = 0; i < inputStringLength && isValidInput && !isValidForfeitInput; i++)
            {
                if ((i + 1) % 3 == 0 && i_CurrentPlayer.LastTurn[i] != '>')
                {
                    isValidInput = false;
                }
                else if ((i + 1) % 3 == 1)
                {
                    if (i_CurrentPlayer.LastTurn[i] < m_LogicUnit.Board.FirstUpperCaseFrame || i_CurrentPlayer.LastTurn[i] >= rangeOfUpperCaseFrame)
                    {
                        isValidInput = false;
                    }
                }
                else if ((i + 1) % 3 == 2)
                {
                    if (i_CurrentPlayer.LastTurn[i] < m_LogicUnit.Board.FirstLowerCaseFrame || i_CurrentPlayer.LastTurn[i] >= rangeOfLowerCaseFrame)
                    {
                        isValidInput = false;
                    }
                }
            }

            return isValidInput;
        }

        private void manageTasksBeforeNextTurn()
        {
            m_LogicUnit.CheckIfBothSidesHaveMoreAvailableMoves();
            bool isItATie = m_LogicUnit.CheckForATie();
            if (isItATie == true)
            {
                displayItsATieScreen();
                displayCurrentScoresScreen();
            }
            else
            {
                bool isCurrentPlayerWon = m_LogicUnit.CheckIfCurrentPlayerWon();
                bool isCurrentPlayerForfeit = m_LogicUnit.CheckIfCurrentPlayerForfeit();
                if (isCurrentPlayerForfeit == true)
                {   // check if current player lost because he wanted to forfeit or he has no more moves
                    displayCurrentPlayerForfeitScreen();
                    displayCurrentScoresScreen();
                }
                else if (isCurrentPlayerWon == true)
                {
                    // check if current player won, either by destroying all the opponents coins or the opponents has no more moves
                    displayCurrentPlayerWonScreen();
                    displayCurrentScoresScreen();
                }
            }

            if (m_LogicUnit.Status == LogicUnit.eGameStatus.EndOfRound)
            {   // if the round is over, check for rematch
                bool isRematch = askForRematch();
                if (isRematch == true)
                {
                    m_LogicUnit.InitializeRematch();
                }
                else
                {
                    m_LogicUnit.Status = LogicUnit.eGameStatus.Quit;
                }
            }
            else
            {   // otherwise, switch turns
                m_LogicUnit.SwitchTurns();
            }
        }

        private void displayMessages(eMessages i_MessageToPrint)
        {
            Console.ForegroundColor = ConsoleColor.White;
            switch (i_MessageToPrint)
            {
                case eMessages.Welcome:
                    Console.WriteLine(k_WelcomeMsg);
                    break;
                case eMessages.GetPlayerOneName:
                    Console.WriteLine(k_GetPlayerOneNameMsg);
                    break;
                case eMessages.InvalidNameLength:
                    Console.WriteLine(k_InvalidNameLengthMsg);
                    break;
                case eMessages.GetBoardSize:
                    Console.WriteLine(k_GetBoardSizeMsg);
                    break;
                case eMessages.InvalidBoardSize:
                    Console.WriteLine(k_InvalidBoardSizeMsg);
                    break;
                case eMessages.ChooseGameMode:
                    Console.WriteLine(k_ChooseGameModeMsg);
                    break;
                case eMessages.InvalidGameMode:
                    Console.WriteLine(k_InvalidModeMsg);
                    break;
                case eMessages.GetPlayerTwoName:
                    Console.WriteLine(k_GetPlayerTwoNameMsg);
                    break;
                case eMessages.InvalidTurnSyntax:
                    Console.WriteLine(k_InvalidTurnSyntaxMsg);
                    break;
                case eMessages.InvalidInputMove:
                    Console.WriteLine(k_InvalidMoveMsg);
                    break;
                case eMessages.InvalidForfeit:
                    Console.WriteLine(k_InvalidForfeitMsg);
                    break;
                case eMessages.AskForRematch:
                    Console.WriteLine(k_AskForRematchMsg);
                    break;
                case eMessages.InvalidRematchSyntax:
                    Console.WriteLine(k_InvalidRematchSyntaxMsg);
                    break;
                default:
                    break;
            }
        }

        private void printBoard()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            char colsMark = m_LogicUnit.Board.FirstUpperCaseFrame;
            char rowsMark = m_LogicUnit.Board.FirstLowerCaseFrame;
            Console.ForegroundColor = ConsoleColor.Green;
            for (int cols = 0; cols < m_LogicUnit.Board.Size; cols++)
            {
                Console.Write("   ");
                Console.Write(colsMark);
                colsMark++;
            }

            Console.Write(Environment.NewLine);
            for (int rows = 0; rows < m_LogicUnit.Board.Size; rows++)
            {
                for (int cols = 0; cols < m_LogicUnit.Board.Size + 1; cols++)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    if (cols == 0)
                    {
                        Console.Write(" ");
                    }
                    else if (cols == m_LogicUnit.Board.Size)
                    {
                        Console.Write("=====");
                    }
                    else
                    {
                        Console.Write("====");
                    }
                }

                Console.Write(Environment.NewLine);
                for (int cols = 0; cols < m_LogicUnit.Board.Size + 1; cols++)
                {
                    if (cols == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(rowsMark);
                        rowsMark++;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("| ");
                        if (m_LogicUnit.Board.GameBoard[rows, cols - 1] == m_LogicUnit.Board.PlayerOneSign || m_LogicUnit.Board.GameBoard[rows, cols - 1] == m_LogicUnit.Board.PlayerOneKingSign)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }

                        Console.Write(m_LogicUnit.Board.GameBoard[rows, cols - 1]);
                        Console.Write(" ");
                    }

                    if (cols == m_LogicUnit.Board.Size)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("|");
                    }
                }

                Console.Write(Environment.NewLine);
            }

            for (int cols = 0; cols < m_LogicUnit.Board.Size + 1; cols++)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                if (cols == 0)
                {
                    Console.Write(" ");
                }
                else if (cols == m_LogicUnit.Board.Size)
                {
                    Console.Write("=====");
                }
                else
                {
                    Console.Write("====");
                }
            }

            Console.Write(Environment.NewLine);
        }

        private void displayLastTurnAndCurrentPlayer()
        {
            Console.ForegroundColor = ConsoleColor.White;
            StringBuilder displayLastTurnAndCurrentPlayerMsg = new StringBuilder();
            if (m_LogicUnit.PlayerOne.LastTurn == string.Empty)
            {
                displayLastTurnAndCurrentPlayerMsg.Append(m_LogicUnit.PlayerOne.Name);
                displayLastTurnAndCurrentPlayerMsg.Append("'s turn (");
                displayLastTurnAndCurrentPlayerMsg.Append(m_LogicUnit.Board.PlayerOneSign);
                displayLastTurnAndCurrentPlayerMsg.Append("):");
                Console.WriteLine(displayLastTurnAndCurrentPlayerMsg);
            }
            else
            {
                switch (m_LogicUnit.CurrentTurn)
                {
                    case LogicUnit.eCurrentShapeTurn.Circle:
                        displayLastTurnAndCurrentPlayerMsg.Append(m_LogicUnit.PlayerOne.Name);
                        displayLastTurnAndCurrentPlayerMsg.Append("'s move was (");
                        displayLastTurnAndCurrentPlayerMsg.Append(m_LogicUnit.Board.PlayerOneSign);
                        displayLastTurnAndCurrentPlayerMsg.Append("): ");
                        displayLastTurnAndCurrentPlayerMsg.Append(m_LogicUnit.PlayerOne.LastTurn);
                        displayLastTurnAndCurrentPlayerMsg.AppendLine();
                        displayLastTurnAndCurrentPlayerMsg.Append(m_LogicUnit.PlayerTwo.Name);
                        displayLastTurnAndCurrentPlayerMsg.Append("'s turn (");
                        displayLastTurnAndCurrentPlayerMsg.Append(m_LogicUnit.Board.PlayerTwoSign);
                        break;
                    case LogicUnit.eCurrentShapeTurn.Ex:
                        displayLastTurnAndCurrentPlayerMsg.Append(m_LogicUnit.PlayerTwo.Name);
                        displayLastTurnAndCurrentPlayerMsg.Append("'s move was (");
                        displayLastTurnAndCurrentPlayerMsg.Append(m_LogicUnit.Board.PlayerTwoSign);
                        displayLastTurnAndCurrentPlayerMsg.Append("): ");
                        displayLastTurnAndCurrentPlayerMsg.Append(m_LogicUnit.PlayerTwo.LastTurn);
                        displayLastTurnAndCurrentPlayerMsg.AppendLine();
                        displayLastTurnAndCurrentPlayerMsg.Append(m_LogicUnit.PlayerOne.Name);
                        displayLastTurnAndCurrentPlayerMsg.Append("'s turn (");
                        displayLastTurnAndCurrentPlayerMsg.Append(m_LogicUnit.Board.PlayerOneSign);
                        break;
                    default:
                        break;
                }

                displayLastTurnAndCurrentPlayerMsg.Append("):");
                Console.WriteLine(displayLastTurnAndCurrentPlayerMsg);
            }
        }

        private void displayExtraTurnScreen()
        {
            Console.ForegroundColor = ConsoleColor.White;
            StringBuilder displayExtraTurnScreenMsg = new StringBuilder();
            switch (m_LogicUnit.CurrentTurn)
            {
                case LogicUnit.eCurrentShapeTurn.Circle:
                    displayExtraTurnScreenMsg.Append(m_LogicUnit.PlayerTwo.Name);
                    displayExtraTurnScreenMsg.Append("'s move was (");
                    displayExtraTurnScreenMsg.Append(m_LogicUnit.Board.PlayerTwoSign);
                    displayExtraTurnScreenMsg.Append("): ");
                    displayExtraTurnScreenMsg.Append(m_LogicUnit.PlayerTwo.LastTurn);
                    displayExtraTurnScreenMsg.AppendLine();
                    displayExtraTurnScreenMsg.Append(m_LogicUnit.PlayerTwo.Name);
                    displayExtraTurnScreenMsg.Append("'s turn (");
                    displayExtraTurnScreenMsg.Append(m_LogicUnit.Board.PlayerTwoSign);
                    break;
                case LogicUnit.eCurrentShapeTurn.Ex:
                    displayExtraTurnScreenMsg.Append(m_LogicUnit.PlayerOne.Name);
                    displayExtraTurnScreenMsg.Append("'s move was (");
                    displayExtraTurnScreenMsg.Append(m_LogicUnit.Board.PlayerOneSign);
                    displayExtraTurnScreenMsg.Append("): ");
                    displayExtraTurnScreenMsg.Append(m_LogicUnit.PlayerOne.LastTurn);
                    displayExtraTurnScreenMsg.AppendLine();
                    displayExtraTurnScreenMsg.Append(m_LogicUnit.PlayerOne.Name);
                    displayExtraTurnScreenMsg.Append("'s turn (");
                    displayExtraTurnScreenMsg.Append(m_LogicUnit.Board.PlayerOneSign);
                    break;
                default:
                    break;
            }

            displayExtraTurnScreenMsg.Append("):");
            Console.WriteLine(displayExtraTurnScreenMsg);
        }

        private string getCommandFromCurrentPlayer()
        {
            string commandFromUser = string.Empty;
            switch (m_LogicUnit.CurrentTurn)
            {
                case LogicUnit.eCurrentShapeTurn.Circle:
                    commandFromUser = readCommandFromConsole(m_LogicUnit.PlayerTwo);
                    break;
                case LogicUnit.eCurrentShapeTurn.Ex:
                    commandFromUser = readCommandFromConsole(m_LogicUnit.PlayerOne);
                    break;
                default:
                    break;
            }

            return commandFromUser;
        }

        private void displayItsATieScreen()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            printBoard();
            Console.ForegroundColor = ConsoleColor.White;
            StringBuilder displayItsATieScreenMsg = new StringBuilder();
            displayItsATieScreenMsg.Append("Both players have no available moves.");
            displayItsATieScreenMsg.AppendLine();
            displayItsATieScreenMsg.Append("It's a tie!");
            Console.WriteLine(displayItsATieScreenMsg);
        }

        public void displayCurrentScoresScreen()
        {
            StringBuilder displayCurrentScoresMsg = new StringBuilder();
            displayCurrentScoresMsg.Append(m_LogicUnit.PlayerOne.Name);
            displayCurrentScoresMsg.Append(" score is: ");
            displayCurrentScoresMsg.Append(m_LogicUnit.PlayerOne.Score);
            displayCurrentScoresMsg.AppendLine();
            displayCurrentScoresMsg.Append(m_LogicUnit.PlayerTwo.Name);
            displayCurrentScoresMsg.Append(" score is: ");
            displayCurrentScoresMsg.Append(m_LogicUnit.PlayerTwo.Score);
            Console.WriteLine(displayCurrentScoresMsg);
        }

        private bool askForRematch()
        {
            bool isRematch = false;
            displayMessages(eMessages.AskForRematch);
            string userRematchInput = Console.ReadLine();
            while (userRematchInput.Length != k_ValidRematchSyntaxLength || (userRematchInput[0] != k_ValidRematchYesSign && userRematchInput[0] != k_ValidRematchNoSign))
            {
                displayMessages(eMessages.InvalidRematchSyntax);
                userRematchInput = Console.ReadLine();
            }

            if (userRematchInput[0] == k_ValidRematchYesSign)
            {
                isRematch = true;
            }

            return isRematch;
        }

        private void displayCurrentPlayerWonScreen()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            printBoard();
            Console.ForegroundColor = ConsoleColor.White;
            StringBuilder displayCurrentPlayerWonMsg = new StringBuilder();
            switch (m_LogicUnit.CurrentTurn)
            {
                case LogicUnit.eCurrentShapeTurn.Circle:
                    displayCurrentPlayerWonMsg.Append(m_LogicUnit.PlayerTwo.Name);
                    break;
                case LogicUnit.eCurrentShapeTurn.Ex:
                    displayCurrentPlayerWonMsg.Append(m_LogicUnit.PlayerOne.Name);
                    break;
                default:
                    break;
            }

            displayCurrentPlayerWonMsg.Append(" has won!");
            Console.WriteLine(displayCurrentPlayerWonMsg);
        }

        private void displayCurrentPlayerForfeitScreen()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            printBoard();
            Console.ForegroundColor = ConsoleColor.White;
            StringBuilder displayCurrentPlayerForfeitMsg = new StringBuilder();
            switch (m_LogicUnit.CurrentTurn)
            {
                case LogicUnit.eCurrentShapeTurn.Circle:
                    displayCurrentPlayerForfeitMsg.Append(m_LogicUnit.PlayerOne.Name);
                    break;
                case LogicUnit.eCurrentShapeTurn.Ex:
                    displayCurrentPlayerForfeitMsg.Append(m_LogicUnit.PlayerTwo.Name);
                    break;
                default:
                    break;
            }

            displayCurrentPlayerForfeitMsg.Append(" has won!");
            Console.WriteLine(displayCurrentPlayerForfeitMsg);
        }

        private void playPlayerVsComputer()
        {
            while (m_LogicUnit.Status != LogicUnit.eGameStatus.Quit)
            {
                printBoard();
                displayLastTurnAndCurrentPlayer(); // display the last moves dialog
                switch (m_LogicUnit.CurrentTurn)
                {
                    case LogicUnit.eCurrentShapeTurn.Circle:
                        handleComputerTurn(); // handles the AI turn and relevant prints
                        break;
                    case LogicUnit.eCurrentShapeTurn.Ex:
                        handleHumanTurn(); // gets a valid input and prints it when its non-AI turn
                        break;
                    default:
                        break;
                }

                manageTasksBeforeNextTurn(); // check if either player has won, lost or its a tie
            }
        }

        private void handleComputerTurn()
        {
            bool continueTurn = true;
            while (continueTurn == true)
            {
                Point startingComputerPoint = null;
                Point destinationComputerPoint = null;
                m_LogicUnit.GetAnAIMove(ref startingComputerPoint, ref destinationComputerPoint); // make an AI move
                m_LogicUnit.PreformMove(startingComputerPoint, destinationComputerPoint); // preform the move (no need to use the returned boolean value because the AI always choose valid moves)
                string aiMove = convertComputerCommandFromPointsToString(startingComputerPoint, destinationComputerPoint);
                m_LogicUnit.PlayerTwo.LastTurn = aiMove; // save its move to print it in the history
                printBoard(); // print the board after a valid move
                if (m_LogicUnit.ExtraAITurn == true)
                {   // AI can make extra move
                    displayExtraTurnScreen();
                    continueTurn = true;
                }
                else
                {
                    continueTurn = false;
                }
            }
        }

        private string convertComputerCommandFromPointsToString(Point i_RandomComputerPointBeforeEating, Point i_RandomComputerPointAfterEating)
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

                commandFromComputer.Append((char)(pointBeforeAndAfterEating[i].X + m_LogicUnit.Board.FirstUpperCaseFrame));
                commandFromComputer.Append((char)(pointBeforeAndAfterEating[i].Y + m_LogicUnit.Board.FirstLowerCaseFrame));
            }

            return commandFromComputer.ToString();
        }
    }
}
