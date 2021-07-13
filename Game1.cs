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
        public Weapon Weapon;
        public List<Enemy> Enemies = new List<Enemy>();
        public List<Bullet> Bullets = new List<Bullet>();

        public enum Status { START, PLAYING, GAMEOVER, WIN };
        public Status CurrentStatus = Status.START;

        private Effect Effect;
        private Effect SpriteEffect;
        private SpriteFont Font;

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
            this.SpriteEffect = Content.Load<Effect>("SpriteWave");
            this.Font = Content.Load<SpriteFont>("font");

            this._BackBuffer = new RenderTarget2D(_graphics.GraphicsDevice, 1500, 1000);

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

        private RenderTarget2D _BackBuffer;
        private void DrawTexts()
        {
            _graphics.GraphicsDevice.SetRenderTarget(this._BackBuffer);
            GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            if (CurrentStatus == Status.START)
                spriteBatch.DrawString(this.Font, "Press Any Key To Start", new Vector2(450, 400), Color.White);
            if (CurrentStatus == Status.GAMEOVER)
                spriteBatch.DrawString(this.Font, "Game Over", new Vector2(570, 200), Color.White);
            spriteBatch.End();
            _graphics.GraphicsDevice.SetRenderTarget(null);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.DrawTexts();

            GraphicsDevice.Clear(Color.Black);

            Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 50.0f);
            Vector3 cameraTarget = new Vector3(0.0f, 0.0f, 0.0f);

            float fovAngle = MathHelper.ToRadians(45);
            float aspectRatio = _graphics.PreferredBackBufferWidth / _graphics.PreferredBackBufferHeight;
            float near = 0.01f;
            float far = 100f;

            Matrix view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(fovAngle, 800f / 480f, near, far);

            DrawModel(this.Player.Ship, this.Player.World, view, projection);

            foreach (Enemy enemy in this.Enemies)
            {
                if (enemy.Ilive)
                {
                    enemy.Update(gameTime);
                    DrawModel(enemy.Model, enemy.World, view, projection);
                    if (IsCollision(this.Player.Ship, this.Player.World, enemy.Model, enemy.World))
                        this.CurrentStatus = Status.GAMEOVER;

                    foreach (Bullet bullet in this.Bullets)
                        if (IsCollision(enemy.Model, enemy.World, bullet.Model, bullet.World))
                            enemy.Ilive = false;
                }
            }

            foreach (Bullet bullet in this.Bullets)
                DrawModel(bullet.Model, bullet.World, view, projection);

            amount = (float)gameTime.TotalGameTime.TotalSeconds * 10;
            this.SpriteEffect.Parameters["amount"].SetValue(amount);
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, this.SpriteEffect, null);
            spriteBatch.Draw((Texture2D)this._BackBuffer, new Vector2(0, 0), Color.White);
            spriteBatch.End();


            base.Draw(gameTime);
        }
        float amount = 0;

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                /*foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                }*/
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = this.Effect;

                    this.Effect.Parameters["World"].SetValue(world);
                    this.Effect.Parameters["View"].SetValue(view);
                    this.Effect.Parameters["Projection"].SetValue(projection);

                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                    this.Effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                    this.Effect.Parameters["AmbientColor"].SetValue(Color.Red.ToVector4());
                    this.Effect.Parameters["AmbientIntensity"].SetValue(0.6f);
                    this.Effect.Parameters["DiffuseIntensity"].SetValue(0.8f);
                    this.Effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(82.0f, 12.0f, -100.0f));
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
