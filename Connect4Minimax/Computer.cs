namespace Connect4Minimax;

public class Computer
{
    private const int MAX_DEPTH = 6;
    private const double WIN_LOSE_SCORE = 1000.0;

    // Pistepainoarvot molemmille pelaajille
    private double[] computerWeights = {0.5, 3.0, 9.0};
    private double[] playerWeights = {-0.5, -2.0, -100.0};

    private int pieceCount = 1; // Montako siirtoa on tehty (heittää yhellä jos tekoäly alottaa, ei mitään väliä)

    /// <summary>
    /// Metodi tietyn pelaajan pelikentän nykyisen tilan arvioimiseksi
    /// Palauttaa arvion pelimerkkien sijainnista laudalla
    /// </summary>
    /// <param name="player">Pelilaudan solu, joka voi olla pelaajan tai tekoälyn hallinnassa tai tyhjä</param>
    /// <param name="gameBoard">Pelilautaa kuvaava array</param>
    /// <returns></returns>
    public double EvaluateBoard(BoardCell[,] gameBoard)
    {
        double score = 0;

        // Tarkastaa kaikki vaakarivit
        for (int row = 0; row < gameBoard.GetLength(0); row++)
        {
            for (int col = 0; col < 4; col++)
            {
                score += EvaluateLine(gameBoard, row, col, 0, 1);
            }
        }

        // Tarkastaa kaikki pystysuorat sarakkeet
        for (int col = 0; col < gameBoard.GetLength(1); col++)
        {
            for (int row = 0; row < 3; row++)
            {
                score += EvaluateLine(gameBoard, row, col, 1, 0);
            }
        }

        // Tarkastaa diagonaaliset linjat vasemmalta oikealle
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                score += EvaluateLine(gameBoard, row, col, 1, 1);
            }
        }

        // Tarkastaa diagonaaliset linjat oikealta vasemmalle
        for (int row = 0; row < 3; row++)
        {
            for (int col = 3; col < 7; col++)
            {
                score += EvaluateLine(gameBoard, row, col, 1, -1);
            }
        }

        return score / pieceCount;
    }


    /// <summary>
    /// arvioi neljän solun rivin (pysty, vaaka tai diagonaalinen)
    /// </summary>
    /// <param name="player">Pelilaudan solu, joka voi olla pelaajan tai tekoälyn hallinnassa tai tyhjä</param>
    /// <param name="gameBoard">Pelilautaa kuvaava array</param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="deltaRow"></param>
    /// <param name="deltaCol"></param>
    /// <returns></returns>
    private double EvaluateLine(BoardCell[,] gameBoard, int row, int col, int deltaRow, int deltaCol)
    {
        // https://www.deepexploration.org/blog/minimax-algorithm-for-connect-4

        int playerCount = 0, computerCount = 0;
        double finalScore = 0.0;

        for (int i = 0; i < 4; i++)
        {
            BoardCell cell = gameBoard[row + i * deltaRow, col + i * deltaCol];

            // Jos solu kuuluu tekoälylle, lisätään computer-laskuria
            if (cell == BoardCell.Computer)
            {
                finalScore += computerWeights[computerCount];
                computerCount++;
            }
            // Jos solu kuuluu ihmispelaajalle, lisätään pelaajalaskuria
            else if (cell == BoardCell.Player)
            {
                // Jostain syystä tässä tapahtuu virhe, kun painoarvoista on vaan 3 elementtiä
                // Vaikka teoriassa sen ei pitäs olla mahollista, koska Minimaxissa tarkistetaan voitto ennen tätä
                finalScore += playerWeights[Math.Min(playerCount, 2)];
                playerCount++;
            }
        }

        return finalScore;
    }

    
    /// <summary>
    /// Minimax-algoritmi Alpha-Beta-karsinnalla.
    /// Määrittää parhaan mahdollisen siirron tietokoneelle.
    /// </summary>
    /// <param name = "isMaximizing">Lippu, joka osoittaa, kumman vuoro on. True — tietokoneen vuoro (maksimointi), false — pelaajan vuoro (minimointi)</param>
    /// <param name="alpha">Paras arvo, jonka maksimoiva osapuoli voi taata nykyisellä tasolla tai korkeammalla.</param>
    ///<param name="beta">Paras arvo, jonka minimoiva osapuoli voi taata nykyisellä tasolla tai korkeammalla.</param>

    public (double score, int column) MinimaxAndAlphaBeta(Board board, int depth, bool isMaximizing, double alpha, double beta)
    {
        // pelin nykyinen tila tietokoneelle
        GameState state = board.GetGameState(BoardCell.Computer);
        double lengthWeight = (MAX_DEPTH - depth) / (double)MAX_DEPTH; // Suositaan nopeita voittoja ja hitaita häviöitä

        //jos tietokone voittaa, palauta korkein pistemäärä ja -1 siirrosta
        if (state == GameState.ComputerWin)
        {
            return (WIN_LOSE_SCORE - 0.25 * WIN_LOSE_SCORE * lengthWeight, -1);
        } else if (state == GameState.PlayerWin)
        {
            // Jos pelaaja voittaa, palauta alhainen pistemäärä ja -1 siirrosta
            return (-WIN_LOSE_SCORE + 0.25 * WIN_LOSE_SCORE * lengthWeight, -1);
        } else if (state == GameState.Draw || depth <= 0)
        {
            //jos on tasapeli tai hakusyvyys saavuttaa 0, palauta laudan pisteet
            double evaluatedScore = EvaluateBoard(board.GetBoard());
            evaluatedScore -= 0.25 * evaluatedScore * lengthWeight;
            return (evaluatedScore, -1);
        }

        List<int> freeColumns = board.GetFreeColumns(); //luettelo ilmaisista sarakkeista

        //  muuttujan alustaminen laskennan parantamiseksi
        double bestScore;
        int bestColumn = -1;

        if (isMaximizing) // jos se on tietokoneen liike
        {
            bestScore = int.MinValue; // alustetaan maksimiarvo mahdollisimman pieneksi
            foreach (int column in freeColumns) //käy läpi vapaat sarakkeet
            {
                Board newBoard = board.Clone();
                newBoard.UpdateBoard(column, BoardCell.Computer);

                // Jos uusi pelilauta johtaa pelaajan voittoon, estetään tämä siirto välittömästi
                if (newBoard.GetGameState(BoardCell.Player) == GameState.PlayerWin)
                {
                    return (-WIN_LOSE_SCORE / 2, column); //palautetaan estävä siirto korkealla prioriteetilla
                }

                //kutsutaan rekursiivisesti Minimax seuraavalle siirrolle (pelaajan siirto)
                double score = MinimaxAndAlphaBeta(newBoard, depth - 1, false, alpha, beta).score;
                /// jos nykyinen siirto antaa paremman tuloksen, päivitetään paras tulos ja sarake
                if (score > bestScore)
                {
                    bestScore = score;
                    bestColumn = column;
                }

                // päivitä Alpha
                alpha = Math.Max(alpha, score);
                if (alpha >= beta)
                {
                    break;
                }
            }
        }

        else // Jos ihmispelaajan vuoro
        {
            bestScore = int.MaxValue; //alustetaan minimiarvo mahdollisimman suureksi.
            foreach (int column in freeColumns)
            {
                Board newBoard = board.Clone();
                newBoard.UpdateBoard(column, BoardCell.Player);

                // //kutsutaan rekursiivisesti Minimax seuraavalle siirrolle (tietokoneen  siirto)
                double score = MinimaxAndAlphaBeta(newBoard, depth - 1, true, alpha, beta).score;
                if (score < bestScore) // jos nykyinen siirto antaa huonoimman tuloksen tietokoneelle, päivitetään minimiarvo
                {
                    bestScore = score;
                    bestColumn = column;
                }

                //päivitä Beta
                beta = Math.Min(beta, score);
                if (alpha >= beta)
                {
                    break;
                }
            }
        }

        return (bestScore, bestColumn);
    }
    

    /// <summary>
    /// Haetaan tekoälyn siirto ja palautetaan se käyttöliittymälle
    /// </summary>
    /// <param name="gameBoard">Board-luokka, joka sisältää tiedot pelilaudan senhetkisestä tilasta</param>
    /// <returns></returns>
    public async Task<int> GetAiMove(Board gameBoard)
    {
        pieceCount += 2; // Sinäänsä ei pidä ihan paikkaansa, mutta "noin" arvio on ihan ok (nopeuttahan tässä haetaan)
        var freeColumns = gameBoard.GetFreeColumns();
        if (freeColumns.Count > 0)
        {
            //Minimax valitsee tekoälylle parhaan siirron
            var (score, bestColumn) = MinimaxAndAlphaBeta(gameBoard, MAX_DEPTH, true, int.MinValue, int.MaxValue);
            return bestColumn;
        }
        return -1;
    }
}
