﻿using Dino.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dino
{
    public partial class Form1 : Form
    {
        Player player;
        Timer mainTimer;
        bool isMouseDown = false;
        private Label scoreLabel;
        private Button startButton;
        private bool isGameStarted = false;

        public Form1()
        {
            InitializeComponent();

            this.Width = 700;
            this.Height = 300;
            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(DrawGame);
            this.KeyUp += new KeyEventHandler(OnKeyboardUp);
            this.KeyDown += new KeyEventHandler(OnKeyboardDown);
            this.MouseDown += new MouseEventHandler(OnMouseDown);
            this.MouseUp += new MouseEventHandler(OnMouseUp);

            mainTimer = new Timer();
            mainTimer.Interval = 10;
            mainTimer.Tick += new EventHandler(Update);

            Init();
        }

        private void OnKeyboardDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (!player.physics.isJumping)
                    {
                        player.physics.isCrouching = true;
                        player.physics.transform.size.Height = 25;
                        player.physics.transform.position.Y = 174;
                    }
                    break;
            }
        }

        private void OnKeyboardUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (!player.physics.isCrouching)
                    {
                        player.physics.isCrouching = false;
                        player.physics.AddForce();
                    }
                    break;
                case Keys.Down:
                    player.physics.isCrouching = false;
                    player.physics.transform.size.Height = 50;
                    player.physics.transform.position.Y = 150.2f;
                    break;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Левый клик - прыжок
                if (!player.physics.isCrouching)
                {
                    player.physics.isCrouching = false;
                    player.physics.AddForce();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Правый клик - присесть
                if (!player.physics.isJumping)
                {
                    if (!player.physics.isCrouching)
                    {
                        player.physics.isCrouching = true;
                        player.physics.transform.size.Height = 25;
                        player.physics.transform.position.Y = 174;
                    }
                    else
                    {
                        // Если уже в состоянии приседания, отменить приседание
                        player.physics.isCrouching = false;
                        player.physics.transform.size.Height = 50;
                        player.physics.transform.position.Y = 150.2f;
                    }
                }
            }

            isMouseDown = true;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Отпускание левой кнопки мыши
                isMouseDown = false;
            }
        }

        public void Init()
        {
            GameController.Init();
            player = new Player(new PointF(50, 149), new Size(50, 50));

            scoreLabel = new Label();
            scoreLabel.Text = "0";
            scoreLabel.AutoSize = true;
            scoreLabel.Font = new Font(scoreLabel.Font.FontFamily, scoreLabel.Font.Size * 3, FontStyle.Bold);
            scoreLabel.Location = new Point(this.Width - scoreLabel.Width - 10, 10);
            this.Controls.Add(scoreLabel);

            startButton = new Button();
            startButton.Text = "Start";
            startButton.AutoSize = true;
            startButton.Location = new Point(10, 10);
            startButton.Click += new EventHandler(StartButton_Click);
            this.Controls.Add(startButton);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            GameController.Init();
            player = new Player(new PointF(50, 149), new Size(50, 50));
            player.score = 0;
            mainTimer.Start();
            isGameStarted = true;
            startButton.Enabled = false;
        }

        public void Update(object sender, EventArgs e)
        {
            if (!isGameStarted)
                return;

            player.score++;
            scoreLabel.Text = player.score.ToString();
            if (player.physics.Collide())
            {
                EndGame();
                return;
            }
            player.physics.ApplyPhysics();
            GameController.MoveMap();
            Invalidate();
        }

        private void EndGame()
        {
            mainTimer.Stop();
            isGameStarted = false;
            startButton.Enabled = true;
        }

        private void DrawGame(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            player.DrawSprite(g);
            GameController.DrawObjets(g);
        }
    }
}
