using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using space_inveders_clone_3d.Entities;

namespace space_inveders_clone_3d
{
    public class Weapon
    {
        public Weapon()
        {
            Game1.Instance.Content.Load<Model>("bullet");
        }


        private bool _cShoot = false;
        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                _cShoot = true;
            if (Keyboard.GetState().IsKeyUp(Keys.Space))
                _cShoot = false;

            this.FireCadence(gameTime);
        }

        private float _timer = 0;
        private float _timeIntervalCadence = 300;
        public void FireCadence(GameTime gameTime)
        {
            if (this._cShoot && _timer >= _timeIntervalCadence)
                this.Shoot();

            float totalMilliseconds = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            _timer = !this._cShoot ? 0 : _timer + totalMilliseconds;
        }

        public void Shoot()
        {
            var bullet = new Bullet();
            bullet.Position = new Vector3(
                Game1.Instance.Player.Position.X,
                Game1.Instance.Player.Position.Y,
                Game1.Instance.Player.Position.Z
            );
            Game1.Instance.Bullets.Add(bullet);
            _timer = 0;
        }
    }
}