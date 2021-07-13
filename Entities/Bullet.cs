using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace space_inveders_clone_3d.Entities
{
    public class Bullet : Entity
    {
        public Bullet()
        {
            this.Model = Game1.Instance.Content.Load<Model>("bullet");
            this.Color = new Vector4(1, 1, 0, 1);
        }

        public override void Update(GameTime gameTime)
        {
            this.Position.Y += 0.5f;
            base.Update(gameTime);
        }
    }
}