using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace space_inveders_clone_3d.Entities
{
    public class Player : Entity
    {
        public Player()
        {
            this.Model = Game1.Instance.Content.Load<Model>("player");
            this.Position = new Vector3(0f, -12.0f, 2.0f);
            this.World = Matrix.CreateTranslation(this.Position);
            this.Color = new Vector4(1, 1, 1, 1);
        }

        public override void Update(GameTime gameTime)
        {
            this.Input();
            this.Move(gameTime);
            this.Animation(gameTime);
            base.Update(gameTime);
        }

        private bool _cRight = false;
        private bool _cLeft = false;
        private void Input()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                _cRight = true;
            if (Keyboard.GetState().IsKeyUp(Keys.Right))
                _cRight = false;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                _cLeft = true;
            if (Keyboard.GetState().IsKeyUp(Keys.Left))
                _cLeft = false;
        }

        private float _speed = 200f / 10000;
        private void Move(GameTime gameTime)
        {
            float totalMilliseconds = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_cRight)
                this.Position.X += _speed * totalMilliseconds;
            if (_cLeft)
                this.Position.X -= _speed * totalMilliseconds;
        }

        private float _maxRotationX = 0.5f;
        public float Rotation = 0;
        private void Animation(GameTime gameTime)
        {
            if (this._cRight && Rotation <= _maxRotationX)
                Rotation += 0.03f;

            if (this._cLeft && Rotation >= -_maxRotationX)
                Rotation -= 0.03f;

            if (!this._cLeft && !this._cRight && Rotation != 0)
                Rotation = Rotation > 0 ? Rotation - 0.03f : Rotation + 0.03f;
        }
    }
}