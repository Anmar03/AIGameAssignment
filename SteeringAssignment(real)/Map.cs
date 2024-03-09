using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteeringAssignment_real.Models;
using System;
using System.Collections.Generic;

namespace SteeringAssignment_real
{
    public class Map
    {
        private readonly Point _mapTileSize = new(12, 10);
        private readonly Sprite[,] _tiles;
        private readonly int noTextures = 5; // Set number of tile textures
        public Point TileSize { get; private set; }
        public Point MapSize { get; private set; }

        public Map()
        {
            _tiles = new Sprite[_mapTileSize.X, _mapTileSize.Y];

            List<Texture2D> textures = new(noTextures);
            for (int i = 1; i < noTextures+1; i++) textures.Add(Globals.Content.Load<Texture2D>($"tile{i}"));

            TileSize = new(textures[0].Width, textures[0].Height);
            MapSize = new(TileSize.X * _mapTileSize.X, TileSize.Y * _mapTileSize.Y);

            Random random = new();

            for (int y = 0; y < _mapTileSize.Y; y++)
            {
                for (int x = 0; x < _mapTileSize.X; x++)
                {
                    int r = random.Next(0, textures.Count);
                    _tiles[x, y] = new(textures[r], new(x * TileSize.X, y * TileSize.Y));
                }
            }
        }

        public void Draw(Lighting lighting)
        {
            for (int y = 0; y < _mapTileSize.Y; y++)
            {
                for (int x = 0; x < _mapTileSize.X; x++)
                {
                    Vector2 tilePosition = new Vector2(x * TileSize.X + TileSize.X / 2, y * TileSize.Y + TileSize.Y / 2);
                    _tiles[x, y].Color = lighting.CalculateLighting(tilePosition);
                    _tiles[x, y].Draw();
                }
            }
        }
    }
}
