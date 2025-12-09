using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Sprint_3
{
    public partial class Form1 : Form
    {
        private BaseGame game;
        private Button[,] gridButtons;

        private Color _originalFormColor;
        private Color _originalTextColor;
        private Color _originalButtonColor;

        private IPlayer _bluePlayer;
        private IPlayer _redPlayer;
        private Timer _computerTimer;

        private Timer _replayTimer;
        private List<Move> _replayMoves;
        private int _replayIndex;
        public Form1()
        {
            InitializeComponent();

            _originalFormColor = this.BackColor;
            _originalTextColor = this.ForeColor;
            _originalButtonColor = SystemColors.Control;

            pnlBoard.Enabled = false;

            _computerTimer = new Timer();
            _computerTimer.Interval = 1000;
            _computerTimer.Tick += ComputerTimer_Tick;

            _replayTimer = new Timer();
            _replayTimer.Interval = 1000;
            _replayTimer.Tick += ReplayTimer_Tick;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnNewGame_Click(object sender, EventArgs e) //what happens when you click the new game button
        {
            Difficulty blueDiff = chkBlueHard.Checked ? Difficulty.Hard : Difficulty.Easy;

            Difficulty redDiff = chkRedHard.Checked ? Difficulty.Hard : Difficulty.Easy;

            _bluePlayer = radioButton1.Checked ? (IPlayer)new ComputerPlayer(blueDiff) : new HumanPlayer();
            _redPlayer = radioButton2.Checked ? (IPlayer)new ComputerPlayer(redDiff) : new HumanPlayer();

            chkBlueHard.Enabled = false;
            chkRedHard.Enabled = false;

            _computerTimer.Stop();
            _replayTimer.Stop();

            int boardSize = (int)numBoardSize.Value; //declares the boardSize the user inputs from the field

            var mode = simpleGameButton.Checked ? GameMode.Simple : GameMode.General; //checks which game mode is selected

            if (mode == GameMode.Simple)
            {
                game = new SimpleGame(boardSize);
            }
            else
            {
                game = new GeneralGame(boardSize);
            }

            chkBlueHard.Enabled = false;
            chkRedHard.Enabled = false;

            radioButton1.Enabled = false;
            radioButton2.Enabled = false;

            pnlBoard.Enabled = true;
            CreateBoardGrid(boardSize); //creates the grid based on the boardSize

            UpdateTurnLabel(); //updates the turn (red or blue)
            UpdateScores();

            CheckComputerTurn();
        }

        private void CheckComputerTurn()
        {
            if (game.State != GameState.InProgress)
            {
                return;
            }

            IPlayer currentPlayer = (game.CurrentTurn == Player.Blue) ? _bluePlayer : _redPlayer;

            if (currentPlayer is ComputerPlayer)
            {
                pnlBoard.Enabled = false;
                _computerTimer.Start();
            }
            else
            {
                pnlBoard.Enabled = true;
            }
        }

        private void ComputerTimer_Tick(object sender, EventArgs e)
        {
            _computerTimer.Stop();

            if (game.State != GameState.InProgress)
            {
                return;
            }

            IPlayer currentPlayer = (game.CurrentTurn == Player.Blue) ? _bluePlayer : _redPlayer;

            currentPlayer.MakeMove(game);

            UpdateBoardFromGame();
            UpdateScores();
            UpdateTurnLabel();
            CheckForGameOver();

            CheckComputerTurn();
        }

        private void CreateBoardGrid(int size)
        {
            pnlBoard.Controls.Clear(); //clears board
            gridButtons = new Button[size, size]; //sets the buttons on the grid
            int buttonSize = pnlBoard.Width / size; //sets the size for the buttons

            float fontSize = buttonSize * 0.5F;
            if (fontSize < 8) fontSize = 8;

            Font dynamicFont = new Font("Century Gothic", fontSize, FontStyle.Bold);

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    var button = new Button
                    {
                        //sets all the button parameters
                        Width = buttonSize,
                        Height = buttonSize,
                        Left = col * buttonSize,
                        Top = row * buttonSize,
                        Tag = new Point(row, col),
                        Font = dynamicFont
                    };

                    button.Click += GridButton_Click;
                    pnlBoard.Controls.Add(button);
                    gridButtons[row, col] = button;
                }
            }

            ApplyTheme();
        }

        private void GridButton_Click(object sender, EventArgs e)
        {
            IPlayer currentPlayer = (game.CurrentTurn == Player.Blue) ? _bluePlayer : _redPlayer;
            if (currentPlayer is ComputerPlayer)
            {
                return;
            }

            Button clickedButton = sender as Button;

            if (clickedButton == null || game == null || game.State != GameState.InProgress)
            {
                return;
            }

            Point position = (Point)clickedButton.Tag;

            if (game.GameBoard[position.X, position.Y] != Cell.Empty)
            {
                return;
            }

            Cell move;

            if (game.CurrentTurn == Player.Blue) //if the turn is the blue player, put the s or o down
            {
                move = blueSButton.Checked ? Cell.S : Cell.O;
            }
            else
            {
                move = redSButton.Checked ? Cell.S : Cell.O; //if not, put it for the red player
            }
            game.MakeMove(position.X, position.Y, move);
            UpdateBoardFromGame();
            UpdateScores();
            CheckForGameOver();
            UpdateTurnLabel();

            CheckComputerTurn();
        }

        private void UpdateTurnLabel() //updates turn label from red to blue and vice versa
        {
            if (game == null)
            {
                lblTurn.Text = "Current Turn: ";
                return;
            }

            lblTurn.Text = $"Current Turn: {game.CurrentTurn}";

            if (game.State != GameState.InProgress)
            {
                bluePlayer.Enabled = true;
                redPlayer.Enabled = true;
                chkBlueHard.Enabled = true;
                chkRedHard.Enabled = true;
                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                return;
            }
            bool isBlueHuman = (_bluePlayer is HumanPlayer);
            bool isRedHuman = (_redPlayer is HumanPlayer);

            if (game.CurrentTurn == Player.Blue && isBlueHuman)
            {
                bluePlayer.Enabled = true;
                redPlayer.Enabled = false;
            }
            else if (game.CurrentTurn == Player.Red && isRedHuman)
            {
                bluePlayer.Enabled = false;
                redPlayer.Enabled = true;
            }
            else
            {
                bluePlayer.Enabled = false;
                redPlayer.Enabled = false;
            }
        }
        private void numBoardSize_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblTurn_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void redOButton_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void UpdateBoardFromGame()
        {
            if (game == null)
            {
                return;
            }
            for (int r = 0; r < game.BoardSize; r++)
            {
                for (int c = 0; c < game.BoardSize; c++)
                {
                    Cell cellState = game.GameBoard[r, c];
                    if (cellState != Cell.Empty)
                    {
                        gridButtons[r, c].Text = cellState.ToString();
                        
                    }
                }
            }
        }

        private void UpdateScores()
        {
            if (game == null)
            {
                lblBlueScore.Text = "Blue: 0";
                lblRedScore.Text = "Red: 0";
            }
            else
            {
                lblBlueScore.Text = $"Blue: {game.BlueScore}";
                lblRedScore.Text = $"Red: {game.RedScore}";
            }
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void CheckForGameOver()
        {
            if (game.State != GameState.InProgress)
            {
                _computerTimer.Stop();
                pnlBoard.Enabled = false;
                string message = "";

                if (chkRecord.Checked)
                {
                    try
                    {
                        using (StreamWriter writer = new StreamWriter("lastgame.txt"))
                        {
                            writer.WriteLine($"{game.BoardSize},{(game is SimpleGame ? "SimpleGame" : "GeneralGame")}");
                            foreach (var move in game.MoveHistory)
                            {
                                writer.WriteLine($"{move.Row},{move.Column},{move.MoveType},{move.Player}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving recording: " + ex.Message);
                    }
                }
                if (game.State == GameState.Draw)
                {
                    message = "The game is a draw!";
                }
                else if (game.State == GameState.BlueWin)
                {
                    message = "Blue player wins!";
                }
                else
                {
                    message = "Red player wins!";
                }

                MessageBox.Show(message, "Game Over!");
            }
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void btnReplay_Click(object sender, EventArgs e)
        {
            if (!File.Exists("lastgame.txt"))
            {
                MessageBox.Show("No recording found!");
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines("lastgame.txt");

                string[] headerParts = lines[0].Split(',');
                int size = int.Parse(headerParts[0]);
                string modeType = headerParts[1];

                if (modeType == "SimpleGame")
                {
                    simpleGameButton.Checked = true;
                    game = new SimpleGame(size);
                }
                else
                {
                    generalGameButton.Checked = true;
                    game = new GeneralGame(size);
                }

                _replayMoves = new List<Move>();
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(',');
                    _replayMoves.Add(new Move(
                        int.Parse(parts[0]),
                        int.Parse(parts[1]),
                        (Cell)Enum.Parse(typeof(Cell), parts[2]),
                        (Player)Enum.Parse(typeof(Player), parts[3])
                    ));
                }

                pnlBoard.Enabled = false;
                _computerTimer.Stop();

                radioButton1.Enabled = false;
                radioButton2.Enabled = false;

                CreateBoardGrid(size);
                UpdateScores();
                UpdateTurnLabel();

                _replayIndex = 0;
                _replayTimer.Start();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error loading recording: " + ex.Message);
            }
        }

        private void ReplayTimer_Tick(object sender, EventArgs e)
        {
            if (_replayMoves == null || _replayIndex >= _replayMoves.Count)
            {
                _replayTimer.Stop();
                MessageBox.Show("Replay finished!");

                radioButton1.Enabled = true;
                radioButton2.Enabled = true;
                simpleGameButton.Enabled = true;
                generalGameButton.Enabled = true;
                numBoardSize.Enabled = true;
                return;
            }

            Move m = _replayMoves[_replayIndex];

            game.MakeMove(m.Row, m.Column, m.MoveType);

            UpdateBoardFromGame();
            UpdateScores();
            UpdateTurnLabel();

            _replayIndex++;
        }

        private bool _isDarkMode = false;

        private void btnTheme_Click(object sender, EventArgs e)
        {
            _isDarkMode = !_isDarkMode;
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            Color targetBackColor;
            Color targetForeColor;

            if (_isDarkMode)
            {
                targetBackColor = Color.Black;
                targetForeColor = Color.White;

                this.BackColor = Color.Black;
                this.ForeColor = Color.White;
                pnlBoard.BackColor = Color.Black;

                btnTheme.Text = "Light Mode";
            }
            else
            {
                targetBackColor = _originalFormColor;
                targetForeColor = _originalTextColor;

                this.BackColor = _originalFormColor;
                this.ForeColor = _originalTextColor;
                pnlBoard.BackColor = _originalFormColor;

                btnTheme.Text = "Dark Mode";
            }

            foreach (Control c in this.Controls)
            {
                if (c == pnlBoard) continue;

                if (c is Label || c is RadioButton || c is CheckBox || c is TextBox || c is GroupBox)
                {
                    c.BackColor = targetBackColor;
                    c.ForeColor = targetForeColor;
                }

                if (c is Button)
                {
                    if (_isDarkMode)
                    {
                        c.BackColor = Color.White;
                        c.ForeColor = Color.Black;
                        ((Button)c).FlatStyle = FlatStyle.Flat;
                    }
                    else
                    {
                        c.BackColor = Color.White;
                        c.ForeColor = _originalTextColor;
                        ((Button)c).FlatStyle = FlatStyle.Standard;
                    }
                }

                if (c is NumericUpDown)
                {
                    c.BackColor = targetBackColor;
                    c.ForeColor = targetForeColor;
                }
            }

            if (gridButtons != null)
            {
                foreach (Button btn in gridButtons)
                {
                    if (btn != null)
                    {
                        if (_isDarkMode)
                        {
                            btn.BackColor = Color.Black;
                            btn.ForeColor = Color.White;
                            btn.FlatStyle = FlatStyle.Flat;
                            btn.FlatAppearance.BorderColor = Color.White;
                            btn.FlatAppearance.BorderSize = 1;
                        }
                        else
                        {
                            btn.BackColor = Color.White;
                            btn.ForeColor = _originalTextColor;
                            btn.FlatStyle = FlatStyle.Standard;
                            btn.UseVisualStyleBackColor = true;
                        }
                    }
                }
            }
        }
    }
}

