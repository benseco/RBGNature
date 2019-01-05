using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RBGNature.Actor.Scene;
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
        RenderTarget2D lightTarget;
        SceneManager sceneManager;

        private static bool Paused { get; set; }
        private static bool JustPaused { get; set; }

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

        private static BlendState Multiply = new BlendState()
        {
            AlphaSourceBlend = Blend.DestinationAlpha,
            AlphaDestinationBlend = Blend.Zero,
            AlphaBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero,
            ColorBlendFunction = BlendFunction.Add
        };

        private static BlendState Lighten = new BlendState()
        {
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.Max
        };

        public Game1()
        {
            IsFixedTimeStep = false;
            //TargetElapsedTime = System.TimeSpan.FromSeconds(1.0f / 240.0f);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public static void TogglePause()
        {
            if (JustPaused) return;
            Paused = !Paused;
            JustPaused = true;
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
            //graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();

            // set up render target
            PresentationParameters presentationParameters = graphics.GraphicsDevice.PresentationParameters;
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice, 640, 360, false, SurfaceFormat.Color,
                DepthFormat.None, presentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            lightTarget = new RenderTarget2D(graphics.GraphicsDevice, 640, 360, false, SurfaceFormat.Color,
                DepthFormat.None, presentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);

            sceneManager = new SceneManager(DemoScene.Instance);


            this.IsMouseVisible = true;
            SoundEffect.MasterVolume = 0.2f;
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
            sceneManager.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.OemTilde))
            {
                TogglePause();
            }
            else { JustPaused = false; }

            if (Paused) return;

            // TODO: Add your update logic here
            sceneManager.Update(gameTime);

            base.Update(gameTime);

            updateCount++;
        }

        int updateCount = 0;
        double previousFrameTime;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            double currentFrameTime = gameTime.ElapsedGameTime.TotalSeconds;

            //System.Console.WriteLine("FPS:" + 1 / currentFrameTime + " | " + updateCount);
            updateCount = 0;

            if (currentFrameTime > previousFrameTime)
            {
                //System.Console.WriteLine("Frame took too long: " + gameTime.ElapsedGameTime.TotalSeconds + "s");
            }
            previousFrameTime = currentFrameTime;



            // Clear lightTarget with atmosphere color from scene
            GraphicsDevice.SetRenderTarget(lightTarget);
            GraphicsDevice.Clear(sceneManager.Scene.Atmosphere);

            // Draw lights from scene onto lightTarget
            spriteBatch.Begin(blendState: Lighten, transformMatrix: sceneManager.Scene.Camera.GetTransform());
            sceneManager.Light(spriteBatch);
            spriteBatch.End();
            
            //game
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;


            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: sceneManager.Scene.Camera.GetTransform(), sortMode: SpriteSortMode.FrontToBack);
            sceneManager.Draw(gameTime, spriteBatch);
            spriteBatch.End();
            


            //apply lights target
            spriteBatch.Begin(blendState: Multiply);
            spriteBatch.Draw(lightTarget, new Rectangle(0,0,640,360), Color.White);
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
