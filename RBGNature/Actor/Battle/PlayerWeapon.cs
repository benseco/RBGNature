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
        static CollisionIdentity snipeIdentity = new CollisionIdentity()
        {
            Damage = 10
        };

        private const float SNIPE_DECAY = 1 / 80f; 

        private Camera Camera;

        Texture2D textureBullet;
        Texture2D textureCannonball;
        Texture2D textureSnipePoint;

        SoundEffect soundEffectGunshot;
        SoundEffect soundEffectRhythmClick;


        List<Circle> bullets;
        List<Circle> bulletsToRemove;
        List<Circle> cannonballs;
        List<Circle> cannonballsToRemove;
        List<Circle> snipes;
        List<Circle> snipesToRemove;


        private List<Tuple<Vector2, float>> snipePoints;


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
            snipes = new List<Circle>();
            snipesToRemove = new List<Circle>();
            snipePoints = new List<Tuple<Vector2, float>>();
        }

        public void Collide(float s, PhysicsGroupType groupType, ICollide other)
        {
            CollideBulletsSimple(s, other, groupType, bullets, bulletIdentity, bulletsToRemove);
            CollideBulletsSimple(s, other, groupType, cannonballs, cannonballIdentity, cannonballsToRemove);
            CollideBulletsSimple(s, other, groupType, snipes, snipeIdentity, snipesToRemove);
        }

        public CollisionResult Collide(float s, PhysicsGroupType groupType, PhysicsObject physicsObject, CollisionIdentity identity)
        {
            CollisionResult response = CollisionResult.None;
            response = CollideBulletsSimple(s, physicsObject, bullets, bulletIdentity, bulletsToRemove);
            response = CollideBulletsSimple(s, physicsObject, cannonballs, cannonballIdentity, cannonballsToRemove);
            response = CollideBulletsSimple(s, physicsObject, snipes, snipeIdentity, snipesToRemove);
            return response;
        }

        private void CollideBulletsSimple(float s, ICollide other, PhysicsGroupType groupType, List<Circle> list, CollisionIdentity identity, List<Circle> removalList)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                Circle c = list[i];
                CollisionResult collisionResult = other.Collide(s, groupType, c, identity);
                if (collisionResult)
                {
                    removalList.Add(c);
                }
            }
        }

        private CollisionResult CollideBulletsSimple(float s, PhysicsObject physicsObject, List<Circle> list, CollisionIdentity identity, List<Circle> removalList)
        {
            CollisionResult response = CollisionResult.None;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                Circle c = list[i];
                CollisionResult collisionResult = physicsObject.Collide(s, c);
                if (collisionResult)
                {
                    removalList.Add(c);
                    collisionResult.Identity = identity;
                    response = collisionResult;
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
            foreach (Tuple<Vector2, float> snipeTuple in snipePoints)
            {
                spriteBatch.Draw(textureSnipePoint, snipeTuple.Item1, null, Color.White * snipeTuple.Item2, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
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
            textureSnipePoint = contentManager.Load<Texture2D>("Sprites/bullet/snipe");
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
                    switch (mode)
                    {
                        case GunMode.Default:
                            mode = GunMode.Cannon;
                            break;
                        case GunMode.Cannon:
                            mode = GunMode.Sniper;
                            break;
                        default:
                            mode = GunMode.Default;
                            break;
                    }
                }
            }
            else { canChangeMode = true; }

            //Remove any bullets that collided in the last update
            RemoveCollidedBullets();

            var mstate = Mouse.GetState();
            timeBetweenShots += elapsedTime;
            if (mstate.LeftButton == ButtonState.Pressed)
            {
                if (timeBetweenShots > 100 && !justShot)
                {
                    Shoot(mstate.Position.ToVector2());

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

            UpdateSnipePoints(elapsedTime);
            MoveBullets(elapsedTime, bullets, cannonballs, snipes);
        }

        private void RemoveCollidedBullets()
        {
            bulletsToRemove.ForEach(b => bullets.Remove(b));
            cannonballsToRemove.ForEach(b => cannonballs.Remove(b));
            snipesToRemove.ForEach(b => snipes.Remove(b));
        }

        private void UpdateSnipePoints(float elapsedTime)
        {
            ///TODO: Performance - no need to make all these tuples!

            // First lower the opacity of the existing points
            for (int i = snipePoints.Count - 1; i >= 0; i--)
            {
                Tuple<Vector2, float> snipePoint = snipePoints[i];
                snipePoints[i] = new Tuple<Vector2, float>(snipePoint.Item1, snipePoint.Item2 - SNIPE_DECAY);
            }
            
            // Remove any points which are no longer visible
            for (int i = snipePoints.Count - 1; i >= 0; i--)
            {
                Tuple<Vector2, float> snipePoint = snipePoints[i];
                if (snipePoint.Item2 <= 0)
                    snipePoints.RemoveAt(i);
            }

            // Lastly, add new points for the current position of any snipe bullets
            foreach (Circle snipe in snipes)
            {
                Vector2 totalVel = snipe.Velocity * elapsedTime;
                for (int i = 1; i <= 4; i++)
                {
                    snipePoints.Add(new Tuple<Vector2, float>(snipe.Position + totalVel * i / 4f, 1));
                }
            }
        }

        private void MoveBullets(float elapsedTime, params List<Circle>[] lists)
        {
            foreach (List<Circle> list in lists)
            {
                foreach (Circle c in list)
                {
                    c.Position += c.Velocity * elapsedTime;
                }
            }
        }

        private void Shoot(Vector2 target)
        {
            switch (mode)
            {
                case GunMode.Default:
                    bullets.Add(new Circle()
                    {
                        Position = Camera.Position,
                        Velocity = Vector2.Normalize(target - Camera.FocalPoint) * .5f,
                        Mass = 1,
                        Radius = 3
                    });
                    break;
                case GunMode.Cannon:
                    cannonballs.Add(new Circle()
                    {
                        Position = Camera.Position,
                        Velocity = Vector2.Normalize(target - Camera.FocalPoint) * .25f,
                        Mass = 1,
                        Radius = 6
                    });
                    break;
                case GunMode.Sniper:
                    snipes.Add(new Circle()
                    {
                        Position = Camera.Position,
                        Velocity = Vector2.Normalize(target - Camera.FocalPoint) * 2f,
                        Mass = 1,
                        Radius = 1
                    });
                    break;
            }
        }
    }
}
