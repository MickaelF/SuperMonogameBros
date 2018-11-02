using SuperMarioBros.PhysicComponent;
using SuperMarioBros.DisplayComponent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

public sealed class ObstacleAccessor
{
    private static readonly ObstacleAccessor instance = new ObstacleAccessor();
    private List<Obstacle> mPendingList;
    public Dictionary<int, Obstacle> mObstacleList;
    private List<int> mDrawableObstacles;
    private List<int> mPhysicalObstacles;
    public GameTime mGameTime;

    public FRectangle mInitialisationBoundingBox;
    public FRectangle mDeleteBoundingBox;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static ObstacleAccessor()
    {
    }

    private ObstacleAccessor()
    {
        mDrawableObstacles = new List<int>();
        mPhysicalObstacles = new List<int>();
        mPendingList = new List<Obstacle>();
         mObstacleList = new Dictionary<int, Obstacle>();
        mInitialisationBoundingBox = new FRectangle(0.0f, 0.0f, 500.0f, 600.0f);
        mDeleteBoundingBox = new FRectangle(-200.0f, 0.0f, 1.0f, 600.0f);
    }

    public static ObstacleAccessor Instance
    {
        get
        {
            return instance;
        }
    }

    public void Add(Obstacle obst)
    {
        mPendingList.Add(obst);
        /*if(obst.mIsCollidable)
        {
            mPhysicalObstacles.Add(obst.cId);
        }
        if ((obst as DrawableObstacle).mIsDisplayed)
        {
            mDrawableObstacles.Add(obst.cId);
        }*/
    }

    public void Remove(Obstacle obst)
    {
        Remove(obst.cId);
    }

    public void Remove(int id)
    {
        mObstacleList.Remove(id);
        mDrawableObstacles.Remove(mDrawableObstacles.Find(x => x == id));
        mPhysicalObstacles.Remove(mPhysicalObstacles.Find(x => x == id));
    }

    public void AddDrawnObstacle(int id)
    {
        mDrawableObstacles.Add(id);
    }

    public void AddPhysicalObstacle(int id)
    {
        mPhysicalObstacles.Add(id);
    }

    public void RemoveDrawnObstacle(int id)
    {
        mDrawableObstacles.Remove(mDrawableObstacles.Find(x => x == id));
    }

    public void RemovePhysicalObstacle(int id)
    {
        mPhysicalObstacles.Remove(mDrawableObstacles.Find(x => x == id));
    }

    public void Update(GameTime gameTime)
    {
        foreach(Obstacle o in mPendingList.ToList())
        {
            if(mInitialisationBoundingBox.Contain(o.mPosition))
            {
                mObstacleList.Add(o.cId, o);
                mPendingList.Remove(mPendingList.Find(x => x == o));
                if (o.mIsCollidable)
                {
                    mPhysicalObstacles.Add(o.cId);
                }
                if ((o as DrawableObstacle).mIsDisplayed)
                {
                    mDrawableObstacles.Add(o.cId);
                }
            }
        }
        foreach (Obstacle o in mObstacleList.Values.ToList())
        {
            if (o.Intersect(mDeleteBoundingBox))
            {
                o.AddEvent(o.DeleteObstacle);
            }
            else
            {
                o.Update(gameTime);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (int id in mDrawableObstacles.ToArray())
        {
            if (mDrawableObstacles.Contains(id))
            {
                (mObstacleList[id] as DrawableObstacle).Draw(spriteBatch);
            }
        }
    }

    public void Clear()
    {
        mDrawableObstacles.Clear();
        mPhysicalObstacles.Clear();
        mObstacleList.Clear();
    }
}
