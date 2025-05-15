using System;

public class Minesweeper
{
    /**************************************************************************\
    |* Game Constants                                                         *|
    \**************************************************************************/

    const int ROWS = 5;
    const int COLS = 10;
    const int MINES = 2;

    const string FLAG = "flag";
    const string UNFLAG = "unflag";
    const string SWEEP = "sweep";

    const string FLAG_SYMBOL = "#";
    const string MINE_SYMBOL = "*";

    /**************************************************************************\
    |* Game State                                                             *|
    \**************************************************************************/

    bool isFirstSweep;
    bool isGameLost;
    GridCell[,] cells;

    /**************************************************************************\
    |* GridCell Class                                                         *|
    \**************************************************************************/

    public class GridCell
    {
        public int mineCount;
        public bool isFlagged;
        public bool isSwept;
        public bool isMine;

        public GridCell()
        {
            mineCount = 0;
            isFlagged = false;
            isSwept = false;
            isMine = false;
        }

        public override string ToString()
        {
            if (isFlagged) return FLAG_SYMBOL;
            if (!isSwept) return "·";
            if (isMine) return MINE_SYMBOL;
            return mineCount > 0 ? mineCount.ToString() : " ";
        }
    }

    /**************************************************************************\
    |* Main                                                                   *|
    \**************************************************************************/

    public static void Main(string[] args)
    {
        Minesweeper game = new Minesweeper();
        game.Start();
    }

    public void Start()
    {
        string input;
        Init();
        ShowGameStartScreen();

        do
        {
            ShowBoard();
            ShowInputOptions();
            input = GetInput();
            if (IsValidInput(input))
            {
                ProcessInput(input);
            }
        }
        while (!IsGameOver());

        ShowBoard();
        ShowGameOverScreen();
    }

    public void Init()
    {
        isFirstSweep = true;
        isGameLost = false;
        cells = new GridCell[ROWS, COLS];

        for (int i = 0; i < ROWS; i++)
            for (int j = 0; j < COLS; j++)
                cells[i, j] = new GridCell();
    }

    public void ShowGameStartScreen()
    {
        Console.WriteLine("Welcome to Minesweeper!");
        Console.WriteLine("Try to sweep all non-mine cells without stepping on a mine.");
    }

    public void ShowBoard()
    {
        Console.Clear();
        Console.Write("   ");
        for (int j = 0; j < COLS; j++) Console.Write($"{j} ");
        Console.WriteLine();

        for (int i = 0; i < ROWS; i++)
        {
            Console.Write($"{i} |");
            for (int j = 0; j < COLS; j++)
            {
                Console.Write(cells[i, j].ToString() + " ");
            }
            Console.WriteLine();
        }
    }

    public void ShowInputOptions()
    {
        Console.Write($"Enter [ {FLAG} | {UNFLAG} | {SWEEP} ] [0-{ROWS - 1}] [0-{COLS - 1}]: ");
    }

    public string GetInput()
    {
        return Console.ReadLine().Trim();
    }

    public bool IsValidInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Invalid input: empty.");
            return false;
        }

        var parts = input.Split(' ');
        if (parts.Length != 3)
        {
            Console.WriteLine("Invalid input: must contain 3 parts.");
            return false;
        }

        string command = parts[0];
        if (command != FLAG && command != UNFLAG && command != SWEEP)
        {
            Console.WriteLine("Invalid command.");
            return false;
        }

        if (!int.TryParse(parts[1], out int row) || row < 0 || row >= ROWS)
        {
            Console.WriteLine("Invalid row.");
            return false;
        }

        if (!int.TryParse(parts[2], out int col) || col < 0 || col >= COLS)
        {
            Console.WriteLine("Invalid column.");
            return false;
        }

        return true;
    }

    public void ProcessInput(string input)
    {
        var parts = input.Split(' ');
        string action = parts[0];
        int row = int.Parse(parts[1]);
        int col = int.Parse(parts[2]);

        if (action == FLAG)
        {
            cells[row, col].isFlagged = true;
        }
        else if (action == UNFLAG)
        {
            cells[row, col].isFlagged = false;
        }
        else if (action == SWEEP)
        {
            if (isFirstSweep)
            {
                PlaceMines(row, col);
                isFirstSweep = false;
            }

            if (cells[row, col].isMine)
            {
                isGameLost = true;
                SweepAllCells();
            }
            else
            {
                SweepCell(row, col);
            }
        }
    }

    public void PlaceMines(int excludeRow, int excludeCol)
    {
        Random rnd = new Random();
        int minesPlaced = 0;

        while (minesPlaced < MINES)
        {
            int r = rnd.Next(ROWS);
            int c = rnd.Next(COLS);
            if ((r == excludeRow && c == excludeCol) || cells[r, c].isMine)
                continue;

            cells[r, c].isMine = true;
            minesPlaced++;

            for (int i = r - 1; i <= r + 1; i++)
                for (int j = c - 1; j <= c + 1; j++)
                    if (i >= 0 && i < ROWS && j >= 0 && j < COLS && !(i == r && j == c))
                        cells[i, j].mineCount++;
        }
    }

    public void SweepCell(int row, int col)
    {
        if (row < 0 || row >= ROWS || col < 0 || col >= COLS) return;
        if (cells[row, col].isSwept || cells[row, col].isFlagged) return;

        cells[row, col].isSwept = true;

        if (cells[row, col].mineCount == 0)
        {
            // Recursively sweep neighbors
            for (int i = row - 1; i <= row + 1; i++)
                for (int j = col - 1; j <= col + 1; j++)
                    if (!(i == row && j == col))
                        SweepCell(i, j);
        }
    }

    public void SweepAllCells()
    {
        for (int i = 0; i < ROWS; i++)
            for (int j = 0; j < COLS; j++)
                cells[i, j].isSwept = true;
    }

    public bool IsGameOver()
    {
        if (isGameLost) return true;

        for (int i = 0; i < ROWS; i++)
            for (int j = 0; j < COLS; j++)
                if (!cells[i, j].isSwept && !cells[i, j].isMine)
                    return false;

        return true;
    }

    public void ShowGameOverScreen()
    {
        Console.WriteLine(isGameLost ? "You hit a mine! Game Over!" : "You win! All safe cells swept.");
    }
}
