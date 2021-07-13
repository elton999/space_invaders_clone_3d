using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace space_inveders_clone_3d.Entities
{
    public class Enemy : Entity
    {
        public Vector3 StartPosition;
        public bool Ilive = true;
        public bool MoveRight = false;
        public Enemy()
        {
            this.Model = Game1.Instance.Content.Load<Model>("enemy");
            this.Position = new Vector3(0f, -12.0f, 2.0f);
            this.World = Matrix.CreateTranslation(this.Position);
            this.Color = new Vector4(1, 0, 0, 1);
        }

        private float _speedY = 0.0002f;
        private float _speedX = 0.001f;
        public override void Update(GameTime gameTime)
        {
            if (Game1.Instance.CurrentStatus == Game1.Status.PLAYING)
            {
                float totalMilliseconds = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                this.Position.Y -= totalMilliseconds * _speedY;
                this.Position.X = this.StartPosition.X + (float)Math.Sin(gameTime.TotalGameTime.TotalMilliseconds * (MoveRight ? _speedX : -_speedX)) * 10.0f;

            }
            base.Update(gameTime);
        }


    }
}