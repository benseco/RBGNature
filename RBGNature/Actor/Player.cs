﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RBGNature.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBGNature.Actor
{
    class Player : BaseActor, ICollide
    {
        static readonly Rectangle RectBulletDefault = new Rectangle(0, 0, 6, 6);
        static readonly Rectangle RectBulletCannon = new Rectangle(6, 0, 9, 9);

        Camera camera;
        Texture2D textureMan;
        Texture2D textureBullet;
        List<Tuple<Vector2,Vector2>> bullets;
        Circle collision;

        Texture2D textureCircle10;
        Texture2D textureCircle200;

        GunMode mode;
        private bool canChangeMode;
        public enum GunMode
        {
            Default,
            Shotgun,
            Sniper,
            Cannon,
            Turret,
            Laser
        }

        public Player(Camera camera)
        {
            this.camera = camera;
            this.bullets = new List<Tuple<Vector2, Vector2>>();
            collision = new Circle()
            {
                Radius = 10,
                Position = new Vector2(camera.Position.X, camera.Position.Y),
                Mass = 1
            };
        }

        public override void LoadContent(ContentManager contentManager)
        {
            textureMan = contentManager.Load<Texture2D>("Sprites/mc/front");
            textureBullet = contentManager.Load<Texture2D>("Sprites/bullet/bullet");
            textureCircle10 = contentManager.Load<Texture2D>("Sprites/debug/circle10");
            textureCircle200 = contentManager.Load<Texture2D>("Sprites/debug/circle200");
        }

        public override void Update(GameTime gameTime)
        {
            collision.Position = camera.Position;
            float speed = .5f;
            float elapsedTime = gameTime.ElapsedGameTime.Milliseconds;
            float distance = speed * elapsedTime;

            Vector2 inputVelocity = Vector2.Zero;

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.W))
                inputVelocity.Y -= distance;

            if (kstate.IsKeyDown(Keys.S))
                inputVelocity.Y += distance;

            if (kstate.IsKeyDown(Keys.A))
                inputVelocity.X -= distance;

            if (kstate.IsKeyDown(Keys.D))
                inputVelocity.X += distance;

            collision.Velocity = collision.Velocity * 0.7f + inputVelocity * 0.3f;

            camera.Move(collision.Velocity);


            if (kstate.IsKeyDown(Keys.F))
            {
                if (canChangeMode)
                {
                    canChangeMode = false;
                    if (mode == GunMode.Default) mode = GunMode.Cannon;
                    else mode = GunMode.Default;
                }
            }
            else { canChangeMode = true; }


            var mstate = Mouse.GetState();
            if (mstate.LeftButton == ButtonState.Pressed)
            {
                bullets.Add(Tuple.Create(Vector2.Normalize(mstate.Position.ToVector2() - camera.FocalPoint), camera.Position));
            }

            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i] = Tuple.Create(bullets[i].Item1, bullets[i].Item1 * gameTime.ElapsedGameTime.Milliseconds / 2 + bullets[i].Item2);
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(textureMan, camera.Position - new Vector2(10,30), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, .5f);
            spriteBatch.Draw(textureCircle10, camera.Position - new Vector2(10, 10), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
            spriteBatch.Draw(textureCircle200, new Vector2(100,100), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);

            if (mode == GunMode.Default)
            {
                foreach (Tuple<Vector2, Vector2> bullet in bullets)
                {
                    spriteBatch.Draw(textureBullet, bullet.Item2, RectBulletDefault, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, .5f);
                }
            }
            else if (mode == GunMode.Cannon)
            {
                foreach (Tuple<Vector2, Vector2> bullet in bullets)
                {
                    spriteBatch.Draw(textureBullet, bullet.Item2, RectBulletCannon, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, .5f);
                }

            }

        }
        
        public PhysicsObject GetCollisionObject(PhysicsGroupType groupType)
        {
            if (groupType == PhysicsGroupType.Physical)
            {
                return collision;
            }
            return null;
        }
        
        public void OnCollide(PhysicsGroupType groupType, CollisionResult collisionResult)
        {
            camera.Position = collisionResult.PositionA;
            collision.Velocity = collisionResult.VelocityA;
        }
    }
}
