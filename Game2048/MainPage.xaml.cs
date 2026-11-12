
using System.Text.Json;

namespace Game2048;

public partial class MainPage : ContentPage
{
    private int[,] board = new int[4, 4];
    private Button[,] tileButtons = new Button[4, 4];

    private int score;
    private bool hasWon;
    private bool isGameOver;

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public MainPage()
    {
        InitializeComponent();
        InitializeBoard();
        ShowMenu();
    }

    private void InitializeBoard()
    {
        BoardGrid.RowDefinitions.Clear();
        BoardGrid.ColumnDefinitions.Clear();
        BoardGrid.Children.Clear();

        for (int i = 0; i < 4; i++)
        {
            BoardGrid.RowDefinitions.Add(new RowDefinition
            {
                Height = GridLength.Star
            });

            BoardGrid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Star
            });
        }

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                Button tile = new Button
                {
                    Text = "",
                    FontSize = 24,
                    FontAttributes = FontAttributes.Bold,
                    CornerRadius = 8,
                    Padding = 0,
                    Margin = 0,
                    BackgroundColor = Color.FromArgb("#3B3B43"),
                    TextColor = Colors.White,
                    IsEnabled = false
                };

                tileButtons[row, col] = tile;

                BoardGrid.Add(tile, col, row);
            }
        }
    }

    private void ShowMenu()
    {
        MenuPanel.IsVisible = true;
        GamePanel.IsVisible = false;
    }

    private void ShowGame()
    {
        MenuPanel.IsVisible = false;
        GamePanel.IsVisible = true;

        UpdateBoard();
    }

    private void StartGame_Clicked(object sender, EventArgs e)
    {
        board = new int[4, 4];
        score = 0;
        hasWon = false;
        isGameOver = false;

        SpawnTile();
        SpawnTile();

        ShowGame();
    }

    private void SpawnTile()
    {
        List<(int row, int col)> emptyCells = new();

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (board[row, col] == 0)
                {
                    emptyCells.Add((row, col));
                }
            }
        }

        if (emptyCells.Count == 0)
            return;

        int index = Random.Shared.Next(emptyCells.Count);
        var cell = emptyCells[index];

        board[cell.row, cell.col] =
            Random.Shared.Next(10) == 0 ? 4 : 2;
    }

    private void Move(Direction direction)
    {
        if (isGameOver)
            return;

        int[,] newBoard = new int[4, 4];

        for (int line = 0; line < 4; line++)
        {
            List<int> values = new();

            for (int i = 0; i < 4; i++)
            {
                int row = direction switch
                {
                    Direction.Up => i,
                    Direction.Down => 3 - i,
                    Direction.Left => line,
                    Direction.Right => line,
                    _ => 0
                };

                int col = direction switch
                {
                    Direction.Up => line,
                    Direction.Down => line,
                    Direction.Left => i,
                    Direction.Right => 3 - i,
                    _ => 0
                };

                if (board[row, col] != 0)
                {
                    values.Add(board[row, col]);
                }
            }

            for (int i = 0; i < values.Count - 1; i++)
            {
                if (values[i] == values[i + 1])
                {
                    values[i] *= 2;
                    score += values[i];

                    values.RemoveAt(i + 1);
                }
            }

            for (int i = 0; i < values.Count; i++)
            {
                int row = direction switch
                {
                    Direction.Up => i,
                    Direction.Down => 3 - i,
                    Direction.Left => line,
                    Direction.Right => line,
                    _ => 0
                };

                int col = direction switch
                {
                    Direction.Up => line,
                    Direction.Down => line,
                    Direction.Left => i,
                    Direction.Right => 3 - i,
                    _ => 0
                };

                newBoard[row, col] = values[i];
            }
        }

        bool changed = false;

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (board[row, col] != newBoard[row, col])
                {
                    changed = true;
                    break;
                }
            }
        }

        if (!changed)
            return;

        board = newBoard;

        SpawnTile();
        UpdateBoard();

        if (!hasWon && Has2048())
        {
            hasWon = true;
            DisplayAlert("Перемога!", "Ти отримав плитку 2048!", "Продовжити");
        }

        if (CheckGameOver())
        {
            isGameOver = true;
            DisplayAlert("Гру завершено", $"Твій рахунок: {score}", "Добре");
        }
    }

    private bool Has2048()
    {
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (board[row, col] >= 2048)
                    return true;
            }
        }

        return false;
    }

    private bool CheckGameOver()
    {
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (board[row, col] == 0)
                    return false;

                if (row < 3 && board[row, col] == board[row + 1, col])
                    return false;

                if (col < 3 && board[row, col] == board[row, col + 1])
                    return false;
            }
        }

        return true;
    }

    private void UpdateBoard()
    {
        ScoreLabel.Text = score.ToString();

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                int value = board[row, col];

                tileButtons[row, col].Text =
                    value == 0 ? "" : value.ToString();

                tileButtons[row, col].BackgroundColor =
                    GetTileColor(value);

                tileButtons[row, col].TextColor =
                    value <= 4 ? Color.FromArgb("#51483F") : Colors.White;

                tileButtons[row, col].FontSize =
                    value >= 1024 ? 19 : 24;
            }
        }
    }

    private Color GetTileColor(int value)
    {
        return value switch
        {
            0 => Color.FromArgb("#3B3B43"),
            2 => Color.FromArgb("#EEE4DA"),
            4 => Color.FromArgb("#EDE0C8"),
            8 => Color.FromArgb("#F2B179"),
            16 => Color.FromArgb("#F59563"),
            32 => Color.FromArgb("#F67C5F"),
            64 => Color.FromArgb("#F65E3B"),
            128 => Color.FromArgb("#EDCF72"),
            256 => Color.FromArgb("#EDCC61"),
            512 => Color.FromArgb("#EDC850"),
            1024 => Color.FromArgb("#EDC53F"),
            2048 => Color.FromArgb("#EDC22E"),
            _ => Color.FromArgb("#3C3A32")
        };
    }

    private void Up_Clicked(object sender, EventArgs e)
        => Move(Direction.Up);

    private void Down_Clicked(object sender, EventArgs e)
        => Move(Direction.Down);

    private void Left_Clicked(object sender, EventArgs e)
        => Move(Direction.Left);

    private void Right_Clicked(object sender, EventArgs e)
        => Move(Direction.Right);

    private void Menu_Clicked(object sender, EventArgs e)
    {
        ShowMenu();
    }

    private void Exit_Clicked(object sender, EventArgs e)
    {
        Application.Current?.Quit();
    }

    private async void SaveGame_Clicked(object sender, EventArgs e)
    {
        SaveGame();

        await DisplayAlert(
            "Збереження",
            "Гру успішно збережено!",
            "Добре");
    }

    private void SaveGame()
    {
        int[] flatBoard = new int[16];

        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                flatBoard[row * 4 + col] = board[row, col];
            }
        }

        GameSave save = new()
        {
            Board = flatBoard,
            Score = score,
            HasWon = hasWon,
            IsGameOver = isGameOver
        };

        string json = JsonSerializer.Serialize(save);

        Preferences.Default.Set("Game2048_Save", json);
    }

    private async void LoadGame_Clicked(object sender, EventArgs e)
    {
        if (!Preferences.Default.ContainsKey("Game2048_Save"))
        {
            await DisplayAlert(
                "Помилка",
                "Збереженої гри не знайдено!",
                "Добре");

            return;
        }

        try
        {
            string json = Preferences.Default.Get("Game2048_Save", "");

            GameSave? save = JsonSerializer.Deserialize<GameSave>(json);

            if (save == null || save.Board == null || save.Board.Length != 16)
                throw new Exception();

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    board[row, col] = save.Board[row * 4 + col];
                }
            }

            score = save.Score;
            hasWon = save.HasWon;
            isGameOver = save.IsGameOver;

            ShowGame();
        }
        catch
        {
            await DisplayAlert(
                "Помилка",
                "Не вдалося завантажити збереження.",
                "Добре");
        }
    }

    private class GameSave
    {
        public int[] Board { get; set; } = new int[16];
        public int Score { get; set; }
        public bool HasWon { get; set; }
        public bool IsGameOver { get; set; }
    }
}
