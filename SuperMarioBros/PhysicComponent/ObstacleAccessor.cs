using SuperMarioBros.PhysicComponent;
using SuperMarioBros.DisplayComponent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public sealed class ObstacleAccessor
{
    private static readonly ObstacleAccessor instance = new ObstacleAccessor();

    public Dictionary<int, Obstacle> mObstacleList;
    List<int> mDrawableObstacle;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static ObstacleAccessor()
    {
    }

    private ObstacleAccessor()
    {
        mDrawableObstacle = new List<int>();
        mObstacleList = new Dictionary<int, Obstacle>();
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
        mObstacleList.Add(obst.cId, obst);
        if(obst is DrawableObstacle)
        {
            mDrawableObstacle.Add(obst.cId);
        }
    }

    public void Remove(Obstacle obst)
    {
        Remove(obst.cId);
    }

    public void Remove(int id)
    {
        mObstacleList.Remove(id);
        mDrawableObstacle.Remove(mDrawableObstacle.Find(x => x == id));
    }

    public void Update(GameTime gameTime)
    {
        foreach(int id in mDrawableObstacle.ToArray())
        {
            if (mObstacleList.ContainsKey(id))
            {
                (mObstacleList[id] as DrawableObstacle).Update(gameTime);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (int id in mDrawableObstacle.ToArray())
        {
            if (mDrawableObstacle.Contains(id))
            {
                (mObstacleList[id] as DrawableObstacle).Draw(spriteBatch);
            }
        }
    }

    public void Clear()
    {
        mDrawableObstacle.Clear();
        mObstacleList.Clear();
    }
}
