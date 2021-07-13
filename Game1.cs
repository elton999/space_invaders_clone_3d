using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using space_inveders_clone_3d.Entities;

namespace space_inveders_clone_3d
{
    public class Game1 : Game
    {
        public static Game1 Instance;

        private GraphicsDeviceManager _graphics;
        public SpriteBatch spriteBatch;

        public UI UI;
        public Player Player;
        public Weapon Weapon;
        public List<Enemy> Enemies = new List<Enemy>();
        public List<Bullet> Bullets = new List<Bullet>();

        public enum Status { START, PLAYING, GAMEOVER, WIN };
        public Status CurrentStatus = Status.START;

        private Effect Effect;

        public Game1()
        {
            if (Instance == null)
                Instance = this;

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            this.Effect = Content.Load<Effect>("effect");

            this.UI = new UI(GraphicsDevice);

            this.Player = new Player();
            this.Weapon = new Weapon();
        }

        private void CreateEnemies()
        {
            float totalColumns = 5f;
            bool moveRight = false;

            for (int y = 0; y < 4; y++)
            {
                int x = 0;
                moveRight = !moveRight;
                for (x = 0; x < totalColumns; x++)
                {
                    float yPosition = 15.0f - (y * 5);
                    float xPosition = x * 5.0f - 10.0f;

                    var enemy = new Enemy();
                    enemy.Position = new Vector3(xPosition, yPosition, 2.0f);
                    enemy.StartPosition = new Vector3(enemy.Position.X, enemy.Position.Y, enemy.Position.X);
                    enemy.MoveRight = moveRight;

                    this.Enemies.Add(enemy);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().GetPressedKeys().Length > 0 && CurrentStatus != Status.PLAYING)
            {
                this.CurrentStatus = Status.PLAYING;
                this.Bullets = new List<Bullet>();
                this.CreateEnemies();
            }

            this.Player.Update(gameTime);
            this.Weapon.Update(gameTime);

            foreach (Bullet bullet in this.Bullets)
                bullet.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.UI.DrawTexts(spriteBatch, GraphicsDevice);

            GraphicsDevice.Clear(Color.Black);

            Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 50.0f);
            Vector3 cameraTarget = new Vector3(0.0f, 0.0f, 0.0f);

            float fovAngle = MathHelper.ToRadians(45);
            float aspectRatio = _graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight;
            float near = 0.01f;
            float far = 100f;

            Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(fovAngle, 800f / 480f, near, far);

            this.Player.DrawModel(view, projection);

            this.DrawEnemies(gameTime, view, projection);

            this.DrawBullets(gameTime, view, projection);

            this.UI.Draw(gameTime, spriteBatch, GraphicsDevice);

            base.Draw(gameTime);
        }

        private void DrawEnemies(GameTime gameTime, Matrix view, Matrix projection)
        {
            foreach (Enemy enemy in this.Enemies)
            {
                if (enemy.Ilive)
                {
                    enemy.Update(gameTime);
                    enemy.DrawModel(view, projection);
                    if (enemy.IsCollision(this.Player.Model, this.Player.World))
                        this.CurrentStatus = Status.GAMEOVER;

                    foreach (Bullet bullet in this.Bullets)
                        if (bullet.IsCollision(enemy.Model, enemy.World))
                            enemy.Ilive = false;
                }
            }
        }

        private void DrawBullets(GameTime gameTime, Matrix view, Matrix projection)
        {
            foreach (Bullet bullet in this.Bullets)
                bullet.DrawModel(view, projection);
        }
    }

}
