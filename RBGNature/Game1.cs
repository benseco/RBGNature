using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RBGNature.Physics;

namespace RBGNature
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D renderTarget;

        Texture2D textureMan;
        Texture2D textureMap0;
        Texture2D textureMap1;
        Texture2D textureMap2;

        Camera camera;

        int[,,] collision = {
            {{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0}},
            {{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0}},
            {{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0}},
            {{0,0,0,1},{0,0,0,1},{0,0,0,1},{0,0,0,1},{0,0,0,1},{0,0,0,1},{0,0,0,1},{0,0,0,1},{0,0,0,0},{0,0,0,0}},
            {{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{1,0,0,0},{0,0,1,0}},
            {{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0}},
            {{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{1,0,0,0},{0,0,1,0}},
            {{0,1,0,0},{0,1,0,0},{0,1,0,0},{0,1,0,0},{0,1,0,0},{0,1,0,0},{0,1,0,0},{0,1,0,0},{0,0,0,0},{0,0,0,0}},
            {{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0}},
            {{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0},{0,0,0,0}}
        };


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here


            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 2560;
            graphics.PreferredBackBufferHeight = 1440;
            graphics.ApplyChanges();

            // set up render target
            PresentationParameters presentationParameters = graphics.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, 640, 360, false, SurfaceFormat.Color,
                DepthFormat.None, presentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            // set up camera
            camera = new Camera
            {
                //Zoom = 2,
                Origin = new Vector2(320, 180)
            };

            //load json
            //string json = @"{'3':{'0':{'3':{'collide':true}},'1':{'0':{'collide':false},'2':{'collide':false},'3':{'collide':true}},'2':{'3':{'collide':true}},'3':{'3':{'collide':true}},'4':{'3':{'collide':true}},'5':{'3':{'collide':true}},'6':{'3':{'collide':true}}},'4':{'1':{'1':{'collide':false},'2':{'collide':false},'3':{'collide':false}},'7':{'1':{'collide':true},'2':{'collide':true}},'9':{'2':{'collide':true}}},'5':{'1':{'1':{'collide':false},'2':{'collide':false},'3':{'collide':false}}},'6':{'1':{'2':{'collide':false}},'7':{'2':{'collide':true},'3':{'collide':true}},'9':{'2':{'collide':true}}},'7':{'0':{'1':{'collide':true}},'1':{'1':{'collide':true}},'2':{'1':{'collide':true}},'3':{'1':{'collide':true}},'4':{'1':{'collide':true}},'5':{'1':{'collide':true}},'6':{'1':{'collide':true}}}}";
             

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            textureMan = Content.Load<Texture2D>("Sprites/mc/front");
            textureMap0 = Content.Load<Texture2D>("Maps/test1/0");
            textureMap1 = Content.Load<Texture2D>("Maps/test1/1");
            textureMap2 = Content.Load<Texture2D>("Maps/test1/2");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            Vector2 direction = Vector2.Zero;
            float speed = .25f;
            float elapsedTime = gameTime.ElapsedGameTime.Milliseconds;
            float distance = speed * elapsedTime;

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Up))
                direction.Y -= distance;

            if (kstate.IsKeyDown(Keys.Down))
                direction.Y += distance;

            if (kstate.IsKeyDown(Keys.Left))
                direction.X -= distance;

            if (kstate.IsKeyDown(Keys.Right))
                direction.X += distance;

            if (Tri.PIT(camera.Position.X, camera.Position.Y, 100, 100, 100, 600, 600, 100) && direction.Y == 0)
            {
                if (direction.X > 0)
                {
                    direction.Y += distance;
                }
                else if (direction.X < 0)
                {
                    direction.Y -= distance;
                }
            }

            camera.Move(direction);

            if (framecount++ % 120 == 0)System.Console.WriteLine("Camera position: " + camera.Position.X + ", " + camera.Position.Y);


            base.Update(gameTime);
        }
        private int framecount = 0;
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            // TODO: Add your drawing code here

            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            //spriteBatch.Begin(samplerState:SamplerState.PointClamp, transformMatrix:camera.GetTransform());
            //spriteBatch.Draw(textureMap, new Vector2(0, 0), Color.White);
            //spriteBatch.Draw(textureMan, camera.Position, Color.White);
            //spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.GetTransform());
            spriteBatch.Draw(textureMap0, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(textureMan, camera.Position, Color.White);
            spriteBatch.Draw(textureMap1, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(textureMap2, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(renderTarget, 
                destinationRectangle: new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), 
                color: Color.White);
            spriteBatch.End();



            base.Draw(gameTime);
        }
    }
}
