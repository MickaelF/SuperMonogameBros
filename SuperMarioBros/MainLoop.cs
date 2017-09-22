using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SuperMarioBros.LevelComponent;
using SuperMarioBros.DisplayComponent;

namespace SuperMarioBros
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainLoop : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        LevelLoader mLevelLoader;
        Mario mPlayer;
        Camera mCamera;
        Texture2D mBackground;
        Color mClearColor;

        public MainLoop()
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
            mLevelLoader = new LevelLoader();
            mPlayer = new Mario();
            mCamera = new Camera();
            mClearColor = new Color(107, 140, 255);
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
            mLevelLoader.LoadContent(Content, GraphicsDevice);
            mPlayer.LoadContent(Content, GraphicsDevice);
            mPlayer.mPosition = mLevelLoader.mMarioStartPosition;
            mCamera.mViewportSize = new Point(256, 256);
            mCamera.CenterOn(mCamera.mViewportCenter);
            mBackground = Content.Load<Texture2D>("Maps/World1-1");

            // TODO: use this.Content to load your game content here
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
            mLevelLoader.Update(gameTime);
            mPlayer.Update(mLevelLoader.mObstacles, gameTime);
            if(mPlayer.mPosition.X > mCamera.mCenter.X)
            {
                 mCamera.Move(new Vector2(mPlayer.mPosition.X - mCamera.mCenter.X, 0.0f));
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(mClearColor);
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointWrap, null, null, null, mCamera.mTranslationMatrix);
            spriteBatch.Draw(mBackground, Vector2.Zero, Color.White);
            mLevelLoader.Draw(spriteBatch);
            mPlayer.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
