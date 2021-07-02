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

        public Player Player;
        public List<Enemy> Enemies = new List<Enemy>();

        public enum Status { PLAYING, GAMEOVER, WIN };
        public Status CurrentStatus = Status.PLAYING;

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

            this.Player = new Player();

            float totalEnemies = 20f;
            float totalColumns = 5f;
            float totalLines = 4f;
            bool moveRight = false;

            for (int i = 0; i < totalEnemies; i++)
            {
                int x = 0;
                moveRight = !moveRight;
                for (x = 0; x < totalColumns; x++)
                {
                    int line = (int)(i / totalEnemies * totalLines);
                    var enemy = new Enemy();
                    enemy.Position = new Vector3(x * 5.0f - 10.0f, 15.0f - (line * 5), 2.0f);
                    enemy.StartPosition = new Vector3(enemy.Position.X, enemy.Position.Y, enemy.Position.X);
                    enemy.MoveRight = moveRight;
                    this.Enemies.Add(enemy);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (this.Player != null)
                this.Player.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 50.0f);
            Vector3 cameraTarget = new Vector3(0.0f, 0.0f, 0.0f);

            float fovAngle = MathHelper.ToRadians(45);
            float aspectRatio = _graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight;
            float near = 0.01f;
            float far = 100f;

            //Matrix world = Matrix.CreateTranslation(new Vector3(0, 10.0f, 0));
            Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(fovAngle, 800f / 480f, near, far);

            DrawModel(this.Player.Ship, this.Player.World, view, projection);

            foreach (Enemy enemy in this.Enemies)
            {
                enemy.Update(gameTime);
                DrawModel(enemy.Model, enemy.World, view, projection);
            }
            /* 
            System.Console.Clear();
            if (IsCollision(this.Player.Ship, this.Player.World, this.Enemy.Model, this.Enemy.World))
                System.Console.WriteLine("Ok");
            else
                System.Console.WriteLine("not ok");
            */
            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }

        // code from http://rbwhitaker.wikidot.com/collision-detection
        private bool IsCollision(Model model1, Matrix world1, Model model2, Matrix world2)
        {
            for (int meshIndex1 = 0; meshIndex1 < model1.Meshes.Count; meshIndex1++)
            {
                BoundingSphere sphere1 = model1.Meshes[meshIndex1].BoundingSphere;
                sphere1 = sphere1.Transform(world1);

                for (int meshIndex2 = 0; meshIndex2 < model2.Meshes.Count; meshIndex2++)
                {
                    BoundingSphere sphere2 = model2.Meshes[meshIndex2].BoundingSphere;
                    sphere2 = sphere2.Transform(world2);

                    if (sphere1.Intersects(sphere2))
                        return true;
                }
            }
            return false;
        }
    }

}
