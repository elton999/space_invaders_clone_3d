using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace space_inveders_clone_3d.Entities
{
    public class Bullet
    {
        public Matrix World;
        public Vector3 Position;
        public Model Model;
        public Bullet()
        {
            this.Model = Game1.Instance.Content.Load<Model>("bullet");
        }

        public void Update(GameTime gameTime)
        {
            this.Position.Y += 0.5f;
            this.World = Matrix.CreateTranslation(this.Position);
        }
    }
}