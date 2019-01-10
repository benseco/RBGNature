using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RBGNature.Physics;

namespace RBGNature.Actor.Battle
{
    public enum GunMode
    {
        Default,
        Shotgun,
        Sniper,
        Cannon,
        Turret,
        Laser
    }

    public class PlayerWeapon : IAct, ICollide
    {
        static readonly Rectangle RectBulletDefault = new Rectangle(0, 0, 6, 6);
        static readonly Rectangle RectBulletCannon = new Rectangle(6, 0, 9, 9);
        static CollisionIdentity bulletIdentity = new CollisionIdentity()
        {
            Damage = 1
        };
        static CollisionIdentity cannonballIdentity = new CollisionIdentity()
        {
            Damage = 5
        };

        private Camera Camera;

        Texture2D textureBullet;
        Texture2D textureCannonball;

        SoundEffect soundEffectGunshot;
        SoundEffect soundEffectRhythmClick;


        List<Circle> bullets;
        List<Circle> bulletsToRemove;
        List<Circle> cannonballs;
        List<Circle> cannonballsToRemove;


        GunMode mode;

        private bool canChangeMode;
        private float timeBetweenShots;
        private bool justShot;
        private float shotRhythm;
        private bool countingRhythm;
        private bool rhythmReady;

        public PlayerWeapon(Camera camera)
        {
            Camera = camera;
            bullets = new List<Circle>();
            bulletsToRemove = new List<Circle>();
            cannonballs = new List<Circle>();
            cannonballsToRemove = new List<Circle>();
        }

        public void Collide(float s, PhysicsGroupType groupType, ICollide other)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                Circle bullet = bullets[i];
                CollisionResult bulletCollision = other.Collide(s, groupType, bullet, bulletIdentity);
                if (bulletCollision)
                {
                    bulletsToRemove.Add(bullet);
                    bullet.Position = bulletCollision.PositionA;
                    //bullet.Velocity = bulletCollision.VelocityA;
                }

            }
            for (int i = cannonballs.Count - 1; i >= 0; i--)
            {
                Circle cannonball = cannonballs[i];
                CollisionResult cannonballCollision = other.Collide(s, groupType, cannonball, cannonballIdentity);
                if (cannonballCollision)
                {
                    cannonballsToRemove.Add(cannonball);
                    cannonball.Position = cannonballCollision.PositionA;
                    //cannonball.Velocity = cannonballCollision.VelocityA;
                }

            }
        }

        public CollisionResult Collide(float s, PhysicsGroupType groupType, PhysicsObject physicsObject, CollisionIdentity identity)
        {
            CollisionResult response = CollisionResult.None;
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                Circle bullet = bullets[i];
                CollisionResult bulletCollision = physicsObject.Collide(s, bullet);
                if (bulletCollision)
                {
                    bulletsToRemove.Add(bullet);
                    bullet.Position = bulletCollision.PositionB;
                    //bullet.Velocity = bulletCollision.VelocityB;
                    bulletCollision.Identity = bulletIdentity;
                    response = bulletCollision;
                }
            }
            for (int i = cannonballs.Count - 1; i >= 0; i--)
            {
                Circle cannonball = cannonballs[i];
                CollisionResult cannonballCollision = physicsObject.Collide(s, cannonball);
                if (cannonballCollision)
                {
                    cannonballsToRemove.Add(cannonball);
                    cannonball.Position = cannonballCollision.PositionB;
                    //cannonball.Velocity = cannonballCollision.VelocityB;
                    cannonballCollision.Identity = cannonballIdentity;
                    response = cannonballCollision;
                }
            }
            return response;
        }

        public bool Dead()
        {
            return false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Circle bullet in bullets)
            {
                spriteBatch.Draw(textureBullet, bullet.Position, RectBulletDefault, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
            }
            foreach (Circle cannonball in cannonballs)
            {
                spriteBatch.Draw(textureCannonball, cannonball.Position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
            }
        }

        public void Light(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public void LoadContent(ContentManager contentManager)
        {
            textureBullet = contentManager.Load<Texture2D>("Sprites/bullet/bullet");
            textureCannonball = contentManager.Load<Texture2D>("Sprites/bullet/cannonball");
            soundEffectGunshot = contentManager.Load<SoundEffect>("Sound/effect/gunshot");
            soundEffectRhythmClick = contentManager.Load<SoundEffect>("Sound/effect/metronome");
        }

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            var kstate = Keyboard.GetState();

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

            //Remove any bullets that collided in the last update
            bulletsToRemove.ForEach(b => bullets.Remove(b));
            cannonballsToRemove.ForEach(b => cannonballs.Remove(b));

            var mstate = Mouse.GetState();
            timeBetweenShots += elapsedTime;
            if (mstate.LeftButton == ButtonState.Pressed)
            {
                if (timeBetweenShots > 100 && !justShot)
                {
                    if (mode == GunMode.Default)
                    {
                        bullets.Add(new Circle()
                        {
                            Position = Camera.Position,
                            Velocity = Vector2.Normalize(mstate.Position.ToVector2() - Camera.FocalPoint) * .5f,
                            Mass = 1,
                            Radius = 3
                        });
                    }
                    else if (mode == GunMode.Cannon)
                    {
                        cannonballs.Add(new Circle()
                        {
                            Position = Camera.Position,
                            Velocity = Vector2.Normalize(mstate.Position.ToVector2() - Camera.FocalPoint) * .25f,
                            Mass = 1,
                            Radius = 6
                        });
                    }

                    soundEffectGunshot.CreateInstance().Play();

                    if (countingRhythm)
                    {
                        if (shotRhythm == 0)
                        {
                            shotRhythm = timeBetweenShots;
                        }
                        else if (Math.Abs(timeBetweenShots - shotRhythm) < 60)
                        {
                            rhythmReady = true;
                        }
                        else
                        {
                            rhythmReady = false;
                            countingRhythm = false;
                        }
                    }
                    else
                    {
                        countingRhythm = true;
                        shotRhythm = 0;
                    }

                    timeBetweenShots = 0;
                    justShot = true;
                }
            }
            else
            {
                justShot = false;
            }

            if (rhythmReady && timeBetweenShots >= .5 * shotRhythm && rhythmReady)
            {
                soundEffectRhythmClick.CreateInstance().Play();
                rhythmReady = false;
            }

            foreach (Circle bullet in bullets)
            {
                bullet.Position += bullet.Velocity * elapsedTime;
            }
            foreach (Circle cannonball in cannonballs)
            {
                cannonball.Position += cannonball.Velocity * elapsedTime;
            }
        }
    }
}
