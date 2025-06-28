using System;
using System.Data.Common;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Connect4Minimax;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    // Ihmispelaaja.Väri = Coral
    // AI.Väri = Yellow
    private Board gameBoard = new Board();
    private Computer computer = new Computer();
    private string youText = "SINUN VUOROSI";
    private string aiText = "TEKOÄLYN VUORO";
    private string yourInfo = "Aseta pelimerkkisi pelilaudalle";
    private bool yourTurn = true;
    Random rnd = new Random();

    public MainWindow()
    {
        InitializeComponent();
        NewGame.Visibility = Visibility.Collapsed;
        StartGame();
    }

    /// <summary>
    /// Aloitetaan peli arpomalla aloittaja
    /// </summary>
    private async void StartGame()
    {
        // Alustetaan pelilauta tyhjäksi
        gameBoard = new Board();
        // Tyhjennetään pelilauta
        ResetBoard();
        
        //Valitaan randomilla 0 tai 1
        var order = rnd.Next(2);

        if (order == 0) // Jos satunnaisluku on 0 = ihmis-pelaaja aloittaa
        {
            SetPlayerTurnUI();
            yourTurn = true;
        }
        else // muutoin tekoäly aloittaa
        {
            SetAITurnUI();
            yourTurn = false;
            await AiMove();
        }
    }

    private void NewGame_clicked(object sender, RoutedEventArgs e)
    {
        // Tyhjennetään pelilauta ja aloitetaan uusi peli
        NewGame.Visibility = Visibility.Collapsed; // Piilotetaan "Uusi peli" -nappi, kun peli alkaa
        StartGame();
    }

    /// <summary>
    /// Tyhjennetään pelilaudan napit ja palautetaan ne alkutilaan.
    /// </summary>
    private void ResetBoard()
    {
        foreach (var child in Gameboard.Children)
        {
            if (child is Ellipse ellipse)
            {
                ellipse.Fill = Brushes.Black; // Palautetaan oletusväri (musta)
                ellipse.IsEnabled = true; // Tehdään nappi taas aktiiviseksi
            }
        }
    }


    /// <summary>
    /// Kun kursori saapuu ruutuun, tarkistetaan onko ruutu varattu, ja värjätään ruudukon ellipsi pelaajan värillä(Coral) jos se on vapaana.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Ellipse_MouseEnter(object sender, MouseEventArgs e)
    {
        if (!yourTurn) return;

        if (sender is Ellipse ellipse)
        {
            // Haetaan sarakkeen (column) tiedot
            int column = Grid.GetColumn(ellipse);
            // Käydään sarake läpi alhaalta ylöspäin ja etsitään ensimmäinen vapaa solu
            BoardCell[,] board = gameBoard.GetBoard();
            for (int row = board.GetLength(0) - 1; row >= 0; row--)
            {
                // Jos solu on vapaa, värjätään se (alimmainen vapaa solu)
                if (board[row, column] == BoardCell.Empty)
                {
                    Ellipse targetEllipse = GetEllipseAtPosition(row, column);
                    if (targetEllipse != null)
                    {
                        // Korostetaan mahdollinen pelattava ruutu pelaajan värillä
                        targetEllipse.Fill = Brushes.Coral;
                    }
                    break; // Vain ensimmäinen vapaa solu värjätään
                }
            }
        }
    }

    /// <summary>
    /// Palauttaa ellipsin värin takaisin oletusarvoiseksi(musta)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Ellipse_MouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Ellipse ellipse)
        {
            // Haetaan sarake (column) tiedot
            int column = Grid.GetColumn(ellipse);
            // Käydään sarake läpi alhaalta ylöspäin ja etsitään ensimmäinen vapaa solu
            BoardCell[,] board = gameBoard.GetBoard();
            for (int row = board.GetLength(0) - 1; row >= 0; row--)
            {
                // Jos solu on vapaa, palautetaan sen väri oletusväriin (esim. musta)
                if (board[row, column] == BoardCell.Empty)
                {
                    Ellipse targetEllipse = GetEllipseAtPosition(row, column);
                    if (targetEllipse != null)
                    {
                        // Palautetaan väri takaisin oletusarvoon
                        targetEllipse.Fill = Brushes.Black;
                    }
                    break; // Vain ensimmäinen vapaa solu palautetaan
                }
            }
        }
    }

    /// <summary>
    /// Kun ellipsiä painetaan, asetetaan pelaajan pelimerkki ruutuun jos mahdollista. Jos on tekoälyn vuoro, klikkausta ei huomioida.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!yourTurn) return;

        if (sender is Ellipse ellipse)
        {
            // int row = Grid.GetRow(ellipse);
            int playerColumn = Grid.GetColumn(ellipse);

            // Päivitetään ihmispelaajan merkki pelilaudalle(enum), jos mahdollista
            var gameState = gameBoard.MakeMove(playerColumn, BoardCell.Player);
            // Päivitetään myös käyttöliittymään pelimerkki
            UpdateUIforMove(playerColumn, BoardCell.Player);

            // Tarkistetaan, päättyikö peli
            if (CheckGameState(gameState)) return;

            // Vaihdetaan vuoro tekoälylle
            yourTurn = false;
            SetAITurnUI();
            await AiMove(); // Huomioidaan asynkroninen kutsu
        }
    }

    /// <summary>
    /// Palauttaa ellipsin, joka on tietyssä rivissä ja sarakkeessa.
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    private Ellipse GetEllipseAtPosition(int row, int column)
    {
        foreach (var child in Gameboard.Children)
        {
            if (child is Ellipse ellipse && Grid.GetRow(ellipse) == row && Grid.GetColumn(ellipse) == column)
            {
                return ellipse;
            }
        }
        return null;
    }
    /// <summary>
    /// Haetaan tekoälyn siirto Computer-luokasta
    /// </summary>
    /// <returns></returns>
    private async Task AiMove()
    {
        int bestColumn = await computer.GetAiMove(gameBoard);
        if (bestColumn != -1)
        {
            // Pyydetään Board-luokkaa päivittämään pelilauta ja palauttamaan pelitila
            var gameState = gameBoard.MakeMove(bestColumn, BoardCell.Computer);
            UpdateUIforMove(bestColumn, BoardCell.Computer);

            // Tarkistetaan, päättyikö peli
            if (CheckGameState(gameState)) return;

            // Palautetaan vuoro ihmispelaajalle
            yourTurn = true;
            SetPlayerTurnUI();
        }
    }
    /// <summary>
    /// Ihmispelaajan vuorolla asetetaan oikeat ilmoitukset ja pelimerkin väri.
    /// </summary>
    private void SetPlayerTurnUI()
    {
        TBTurn.Foreground = Brushes.Coral;
        TBTurn.Text = youText;
        TBInfo.Foreground = Brushes.Coral;
        TBInfo.Text = yourInfo;
    }
    /// <summary>
    /// Tekoälyn vuorolla asetetaan oikeat ilmoitukset ja pelimerkin väri.
    /// </summary>
    private void SetAITurnUI()
    {
        TBTurn.Foreground = Brushes.Yellow;
        TBTurn.Text = aiText;
        TBInfo.Text = "";
    }

    /// <summary>
    /// Päivitetään käyttöliittymään vuorossa olleen pelaajan pelimerkki, vuoron aikana pelattuun ruutuun
    /// </summary>
    /// <param name="column"></param>
    /// <param name="player"></param>
    private void UpdateUIforMove(int column, BoardCell player)
    {
        foreach (var child in Gameboard.Children)
        {
            if (child is Ellipse elli && Grid.GetColumn(elli) == column)
            {
                {
                    int row = Grid.GetRow(elli);
                    // Tarkistetaan, onko pelilaudalla kyseisen rivin ja sarakkeen kenttä, vuorossa olevan pelaajan kenttä
                    BoardCell currentCell = gameBoard.GetBoard()[row, column];

                    if (currentCell == player)
                    {
                        // Jos kyseessä on tietokonepelaaja, käytetään keltaista väriä
                        if (player == BoardCell.Computer)
                        {
                            elli.Fill = Brushes.Yellow;
                        }
                        // Muussa tapauksessa, pelaaja on ihminen, käytetään corallia
                        else
                        {
                            elli.Fill = Brushes.Coral;
                        }

                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tarkistetaan pelitilanne ja päivitetään käyttöliittymä
    /// </summary>
    private bool CheckGameState(GameState gameState)
    {
        if (gameState == GameState.PlayerWin)
        {
            TBTurn.Text = "Voitit pelin!";
            NewGame.Visibility = Visibility.Visible; // Näytetään "Uusi peli" -nappi
            TBInfo.Text = "Aloita uusi peli painikkeesta";
            return true;
        }
        else if (gameState == GameState.ComputerWin)
        {
            TBInfo.Foreground = Brushes.Yellow;
            TBTurn.Text = "Hävisit!";
            TBInfo.Text = "Aloita uusi peli painikkeesta";
            NewGame.Visibility = Visibility.Visible; // Näytetään "Uusi peli" -nappi
            return true;
        }
        else if (gameState == GameState.Draw)
        {
            TBTurn.Text = "Tasapeli!";
            TBInfo.Text = "Aloita uusi peli painikkeesta";
            NewGame.Visibility = Visibility.Visible; // Näytetään "Uusi peli" -nappi
            return true;
        }
        return false;
    }
}
