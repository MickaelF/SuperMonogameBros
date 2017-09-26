using System.Xml;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SuperMarioBros.LevelComponent
{
    public class TMXParser
    {
        private string mFilePath;
        private string _mContextName;
        private XmlReader mReader;
        public Point mLevelSizeInTile;
        public Point mTileSize;
        public List<Tile> mTileList;
        public int[][] mTilePosition;
        public int[][] mPipeTilePosition;
        public int[][] mBackgroundTilePosition;
        public int[][] mBonusTilePosition;

        public string mContextName { get => _mContextName; private set => _mContextName = value; }

        public TMXParser(string filePath)
        {
            mFilePath = filePath;
            mReader = XmlReader.Create(mFilePath);
            mTileList = new List<Tile>();
            Read();
        }

        private void Read()
        {
            string currentElement = "";
            while (mReader.Read())
            {
                switch (mReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (mReader.Name.Equals("map"))
                        {
                            // Récuperer taille du niveau, et taille de tuile dans les arguments
                            int width = int.Parse(mReader.GetAttribute("width"));
                            int height = int.Parse(mReader.GetAttribute("height"));
                            mLevelSizeInTile = new Point(width, height);
                            int widthTile = int.Parse(mReader.GetAttribute("tilewidth"));
                            int heightTile = int.Parse(mReader.GetAttribute("tileheight"));
                            mTileSize = new Point(widthTile, heightTile);
                            mTilePosition = new int[mLevelSizeInTile.Y][];
                            mPipeTilePosition = new int[mLevelSizeInTile.Y][];
                            mBackgroundTilePosition = new int[mLevelSizeInTile.Y][];
                            mBonusTilePosition = new int[mLevelSizeInTile.Y][];
                            for (int i = 0; i < mTilePosition.Length; i++)
                            {
                                mTilePosition[i] = new int[mLevelSizeInTile.X];
                                mPipeTilePosition[i] = new int[mLevelSizeInTile.X];
                                mBackgroundTilePosition[i] = new int[mLevelSizeInTile.X];
                                mBonusTilePosition[i] = new int[mLevelSizeInTile.X];
                            }
                            mContextName = mReader.GetAttribute("context");
                        }
                        else if (mReader.Name.Equals("tileset"))
                        {
                            // Récupérer l'id de la tuile, le nom de la tuille et le nombre de tuille
                            int idTile = Int32.Parse(mReader.GetAttribute("firstgid"));
                            string name = mReader.GetAttribute("name");
                            int tileCount = Int32.Parse(mReader.GetAttribute("tilecount"));
                            int columns = Int32.Parse(mReader.GetAttribute("columns"));
                            // Puis récupérer le chemin de l'image de la tuile
                            mReader.ReadToDescendant("image");
                            string source = mReader.GetAttribute("source");
                            mReader.ReadToDescendant("/tileset");
                            mTileList.Add(new Tile(idTile, tileCount, columns, source));
                        }
                        else if (mReader.Name.Equals("layer"))
                        {
                            currentElement = mReader.GetAttribute("name");
                        }
                        break;
                    case XmlNodeType.Text:
                        string[] strArray = mReader.Value.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        for (int i = 0; i < strArray.Length; i++)
                        {
                            string[] lineArray = strArray[i].Split(',').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            for (int j = 0; j < lineArray.Length; j++)
                            {
                                if (currentElement == "Pipe")
                                {
                                    mPipeTilePosition[i][j] += int.Parse(lineArray[j]);
                                }
                                else if (currentElement.Substring(0, 2) == "BG")
                                {
                                    mBackgroundTilePosition[i][j] += int.Parse(lineArray[j]);
                                }
                                else if (currentElement.Substring(0, 5) == "Bonus")
                                {
                                    mBonusTilePosition[i][j] += int.Parse(lineArray[j]);
                                }
                                else
                                {
                                    mTilePosition[i][j] += int.Parse(lineArray[j]);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public Tile WhichTile(int id)
        {
            foreach (Tile tile in mTileList)
            {
                if (tile.TileCorrespondance(id))
                {
                    return tile;
                }
            }
            return new Tile(-1, -1, -1, "j");
        }

        public Dictionary<int, string> IdNameTileDictionnary()
        {
            Dictionary<int, string> dictionnary = new Dictionary<int, string>();
            foreach (Tile tile in mTileList)
            {
                for (int i = 0; i < tile.mTileCount; i++)
                {
                    dictionnary[tile.mFirstId + i] = tile.mTextureName;
                }
            }
            return dictionnary;
        }
    }
}
