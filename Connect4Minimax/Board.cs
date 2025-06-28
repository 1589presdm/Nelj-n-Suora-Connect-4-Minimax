namespace Connect4Minimax;

public enum BoardCell {
    Empty = 0, // Default
    Player = 1,
    Computer = 2,
}

public enum GameState {
    Continue = 0, // Default, peli vielä käynnissä
    PlayerWin = 1, // Ihmispelaajan voitto
    ComputerWin = 2, // Tietokoneen voitto
    Draw = 3, // Tasapeli
    InvalidMove = 4, // Laiton siirto
}


// Pelilautaluokka
public class Board
{
    private BoardCell[,] gameBoard = new BoardCell[6, 7];

    /// <summary>
    /// Pelilaudan getteri
    /// </summary>
    public BoardCell[,] GetBoard()
    {
        return gameBoard;
    }

    /// <summary>
    /// Pelilaudan päivitys
    /// Palauttaa tämänhetkisen pelitilanteen (voitto, tasapeli, peli jatkuu)
    /// </summary>
    public GameState UpdateBoard(int column, BoardCell player)
    {
        // Käydään sarake läpi alhaalta ylöspäin
        for (int row = gameBoard.GetLength(0) - 1; row >= 0; row--)
        {
            // Katotaan tyhjät paikat
            if (gameBoard[row, column] == BoardCell.Empty)
            {
                // Paikka löyty
                gameBoard[row, column] = player;
                return GetGameState(player);
            }
        }
        // Sarake on täynnä
        return GameState.InvalidMove;
    }

    /// <summary>
    /// Metodi käsittelee sekä pelilaudan päivityksen että pelitilan tarkistuksen
    /// </summary>
    /// <param name="column"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public GameState MakeMove(int column, BoardCell player)
    {
        var gameState = UpdateBoard(column, player);
        if (gameState == GameState.Continue)
        {
            gameState = GetGameState(player); // Tarkistetaan, jos peli on päättynyt
        }
        return gameState;
    }

    /// <summary>
    /// Tarkistetaan vapaat sarakkeet ja palautetaan indeksit
    /// Jos palauttaa tyhjän listan, lauta on täynnä ja peli pelattu
    /// </summary>
    public List<int> GetFreeColumns()
    {
        List<int> freeColumns = new List<int>();

        // Luupataan ylimmän rivin kaikki sarakkeet, katotaan onko tyhjät vai ei
        for (int i = 0; i < gameBoard.GetLength(1); i++)
        {
            if (gameBoard[0, i] == BoardCell.Empty)
            {
                freeColumns.Add(i);
            }
        }

        return freeColumns;
    }

    /// <summary>
    /// Tarkistetaan onko pelilauta täysin tyhjä
    /// </summary>
    /// <returns>True, jos lauta on tyhjä, muutoin false</returns>
    public bool IsBoardEmpty()
    {
        for (int row = 0; row < gameBoard.GetLength(0); row++)
        {
            for (int column = 0; column < gameBoard.GetLength(1); column++)
            {
                if (gameBoard[row, column] != BoardCell.Empty)
                {
                    return false; // Löydetty pelimerkki, lauta ei ole tyhjä
                }
            }
        }
        return true; // Kaikki solut ovat tyhjiä
    }

    /// <summary>
    /// Tarkistetaan voittaja
    /// </summary>
    public GameState GetGameState(BoardCell player)
    {
        // Tarkista vaakasuorat, pystysuorat ja vinot voitot
        for (int row = 0; row < gameBoard.GetLength(0); row++)
        {
            for (int col = 0; col < gameBoard.GetLength(1); col++)
            {
                if (CheckDirection(row, col, 1, 0, player) ||  // vaakasuora
                    CheckDirection(row, col, 0, 1, player) ||  // pystysuora
                    CheckDirection(row, col, 1, 1, player) ||  // diagonaalinen /
                    CheckDirection(row, col, 1, -1, player))    // diagonaalinen \
                {
                    return (GameState)player;
                }
            }
        }

        // Onko tasapeli
        if (!GetFreeColumns().Any())
        {
            return GameState.Draw;
        }
        // Peli jatkuu normaalisti
        return GameState.Continue;
    }

    // Tarkista suunta
    private bool CheckDirection(int row, int col, int deltaRow, int deltaCol, BoardCell player)
    {
        for (int i = 0; i < 4; i++)
        {
            int r = row + i * deltaRow;
            int c = col + i * deltaCol;
            if (r < 0 || r >= gameBoard.GetLength(0) || c < 0 || c >= gameBoard.GetLength(1) || gameBoard[r, c] != player)
            {
                return false;
            }
        }
        return true;
    }

    public Board Clone()
    {
        Board newBoard = new Board();
        newBoard.gameBoard = (BoardCell[,])gameBoard.Clone();
        return newBoard;
    }
}
