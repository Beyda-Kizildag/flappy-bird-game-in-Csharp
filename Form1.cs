using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace flappyBird
{
    public partial class Form1 : Form
    {
        private Timer gameTimer;
        private PictureBox bird;
        private List<PictureBox> pipes;
        private Label scoreLabel;
        private int gravity = 3;
        private int pipeSpeed = 3;
        private int gap = 130;
        private int score = 0;
        private Random rand = new Random();
        private bool gameOver = false;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.BackColor = Color.SkyBlue;
            this.Width = 500;
            this.Height = 600;
            StartGame();
        }

        private void StartGame()
        {
            Controls.Clear();
            pipes = new List<PictureBox>();
            score = 0;
            gameOver = false;

            // Bird
            bird = new PictureBox
            {
                Size = new Size(40, 40),
                Location = new Point(80, 250),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Image = Properties.Resources.YWuNbr // PNG dosyanızın adı "bird.png" ise
            };
            Controls.Add(bird);

            // Score Label
            scoreLabel = new Label
            {
                Text = "Score: 0",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            Controls.Add(scoreLabel);

            // Pipes (2 sets)
            for (int i = 0; i < 2; i++)
            {
                CreatePipeSet(400 + i * 250);
            }

            // Timer
            gameTimer = new Timer();
            gameTimer.Interval = 20;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void CreatePipeSet(int x)
        {
            int min = 50;
            int max = this.ClientSize.Height - gap - 50;
            int pipeTopHeight = rand.Next(min, max);

            var pipeTop = new PictureBox
            {
                Size = new Size(60, pipeTopHeight),
                Location = new Point(x, 0),
                BackColor = Color.Green
            };
            var pipeBottom = new PictureBox
            {
                Size = new Size(60, this.ClientSize.Height - (pipeTopHeight + gap)),
                Location = new Point(x, pipeTopHeight + gap),
                BackColor = Color.Green
            };
            Controls.Add(pipeTop);
            Controls.Add(pipeBottom);
            pipes.Add(pipeTop);
            pipes.Add(pipeBottom);
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (gameOver) return;

            // Bird gravity
            bird.Top += gravity;

            // Move pipes
            for (int i = 0; i < pipes.Count; i += 2)
            {
                pipes[i].Left -= pipeSpeed;
                pipes[i + 1].Left -= pipeSpeed;

                // Skor: Kuş boru setini geçtiyse
                if (!pipes[i].Tag?.Equals("scored") ?? true)
                {
                    if (pipes[i].Right < bird.Left)
                    {
                        score++;
                        scoreLabel.Text = $"Score: {score}";
                        pipes[i].Tag = "scored";
                        pipes[i + 1].Tag = "scored";
                    }
                }

                // If pipes go off screen, reset them
                if (pipes[i].Right < 0)
                {
                    Controls.Remove(pipes[i]);
                    Controls.Remove(pipes[i + 1]);
                    pipes.RemoveAt(i + 1);
                    pipes.RemoveAt(i);
                    CreatePipeSet(this.ClientSize.Width);
                    break;
                }
            }

            // Collision detection
            foreach (var pipe in pipes)
            {
                if (bird.Bounds.IntersectsWith(pipe.Bounds))
                {
                    EndGame();
                    return;
                }
            }

            // Top/bottom collision
            if (bird.Top < 0 || bird.Bottom > this.ClientSize.Height)
            {
                EndGame();
                return;
            }
        }

        private void EndGame()
        {
            gameTimer.Stop();
            gameOver = true;
            var result = MessageBox.Show($"Game Over!\nScore: {score}\nRestart?", "Flappy Bird", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                StartGame();
            }
            else
            {
                Application.Exit();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (gameOver) return;
            if (e.KeyCode == Keys.Space)
            {
                bird.Top -= 40;
            }
        }
    }
}
