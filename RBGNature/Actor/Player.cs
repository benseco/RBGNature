using Microsoft.Xna.Framework;
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
        Vector2 stepDirection;

        Texture2D textureCollisionRect;

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
                Center = new Vector2(camera.Position.X, camera.Position.Y)
            };
        }

        public override void LoadContent(ContentManager contentManager)
        {
            textureMan = contentManager.Load<Texture2D>("Sprites/mc/front");
            textureBullet = contentManager.Load<Texture2D>("Sprites/bullet/bullet");
            textureCollisionRect = contentManager.Load<Texture2D>("Sprites/debug/collisionrect");
        }

        public override void Update(GameTime gameTime)
        {
            stepDirection = Vector2.Zero;
            float speed = .05f;
            float elapsedTime = gameTime.ElapsedGameTime.Milliseconds;
            float distance = speed * elapsedTime;

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.W))
                stepDirection.Y -= distance;

            if (kstate.IsKeyDown(Keys.S))
                stepDirection.Y += distance;

            if (kstate.IsKeyDown(Keys.A))
                stepDirection.X -= distance;

            if (kstate.IsKeyDown(Keys.D))
                stepDirection.X += distance;

            camera.Move(stepDirection);


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
            spriteBatch.Draw(textureCollisionRect, camera.Position - new Vector2(10, 10), null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);

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
                collision.Center = new Vector2(camera.Position.X, camera.Position.Y);
                return collision;
            }
            return null;
        }

        public void OnCollide(PhysicsGroupType groupType, CollisionResult collisionResult)
        {
            camera.Move(Vector2.Negate(stepDirection));
        }
    }
}
