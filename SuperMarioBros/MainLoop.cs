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
        Color mClearColor;
        GameInfos mUI;

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
            mLevelLoader = new LevelLoader();
            mPlayer = new Mario();
            mCamera = new Camera();            
            mUI = new GameInfos();
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
            mLevelLoader.LoadContent(Content, graphics);
            mPlayer.LoadContent(Content, graphics);
            mUI.LoadContent(Content, graphics);
            mPlayer.mPosition = mLevelLoader.mMarioStartPosition;
            //mCamera.mViewportSize = new Point(240, 220);
            mCamera.ZoomUp(1.0f);
            mCamera.CenterOn(new Vector2(0, 0));            
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
            //mPlayer.Update(gameTime);
            if(mPlayer.mPosition.X > mCamera.mCenter.X + 120)
            {
                 mCamera.Move(new Vector2(mPlayer.mPosition.X - mCamera.mCenter.X - 120, 0.0f));
            }
            mUI.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(mClearColor);
            Matrix mat = mCamera.mTranslationMatrix;
            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointWrap, null, null, null, mCamera.mTranslationMatrix);
            mLevelLoader.Draw(spriteBatch);
            mUI.Draw(spriteBatch, ref mPlayer, ref mCamera);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
