using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Warband
{
    class Radar : Figures
    {
        protected List<int> addresses;
        protected List<PlayerData> enemies;
        protected Vector2 screenPos = new Vector2(85, 110);
        public Radar(Device device) : base(device)
        {

        }
        protected float entityYaw;

        private List<PlayerData> SetPlayers()
        {
            enemies = new List<PlayerData>();
            addresses = ReadAndInject.ReadAddress();

            for (int x = 0; x != addresses.Count; x++)
            {
                PlayerData entity = new PlayerData(addresses[x]);

                if (addresses[x] != MainPlayer.address && entity.Active == true && entity.Health > 0)
                {
                    if (entity.Rider == -1)
                    {
                        Player player = new Player(entity.Address);
                        enemies.Add(player);
                    }
                    else
                    {
                        Horse player = new Horse(entity.Address);
                        enemies.Add(player);
                    }
                }
            }

            return enemies;

        }

        private void SetHorses()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] is Horse)
                {
                    Horse horse = (Horse)enemies[i];
                    horse.SetPlayerOnHorse(enemies);
                }
            }
        }

        public void SetRadar()
        {
            DrawCircle(85, 110, 80, 120, Color.Gray);
            //DrawCircle(85, 110, 1, 120, Color.Purple);
            SetPlayers();
            SetHorses();
            DrawEnemies();
            Vector4 vector = new Vector4(85, 110, 0, 0);
            Vector4 rotation = new Vector4(vector.X, vector.Y - 4, 0, 0);
            RotatePoint(new Vector2(rotation.X,rotation.Y), new Vector2(0, 0), (float)RadianToDegree(entityYaw));
            DrawLine(vector, new Vector4(vector.X, vector.Y - 4, 0, 0), Color.Aquamarine);
        }

        private static Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, float angle)
        {
            float cosTheta = (float)Math.Cos(angle);
            float sinTheta = (float)Math.Sin(angle);
            Vector2 returnVec = new Vector2(
                cosTheta * (pointToRotate.X - centerPoint.X) - sinTheta * (pointToRotate.Y - centerPoint.Y),
                sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y)
            );
            returnVec += centerPoint;
            return returnVec;
        }

        private void DrawEnemies()
        {

            for (int x = 0; x != enemies.Count; x++)
            {
                Vector2 pointToRotate = new Vector2(MainPlayer.x - enemies[x].Vec[0], (MainPlayer.y - enemies[x].Vec[1]) * -1);

                entityYaw = (float)(Math.Atan2(MainPlayer.yR, MainPlayer.xR));

                
                //float distance1 = pointToRotate.Length() * (0.02f * 200);
                //distance1= Math.Min(distance1, 80);
                //pointToRotate.Normalize();
                //pointToRotate *= distance1;
                pointToRotate += screenPos;
                pointToRotate = RotatePoint(pointToRotate, new Vector2(85, 110), DegreeToRadian(90));
                pointToRotate = RotatePoint(pointToRotate, new Vector2(85, 110), entityYaw);
                float distance = CalculateDistance(new Vector2(85, 110), pointToRotate);
                var rect = new Rectangle<float>(pointToRotate.X, pointToRotate.Y, 1, 1);
                Debug.WriteLine(distance);
                if (enemies[x].GetType() == typeof(Player) && distance < 78.7f)
                {

                    Player player = (Player)enemies[x];

                    if (player.Team != MainPlayer.team)
                    {
                        DrawRect(rect, Color.Red);
                    }
                    else
                    {
                        DrawRect(rect, Color.Green);
                    }
                }
            }
        }

        public float CalculateDistance(Vector2 location1, Vector2 location2)
        { 
            return (location1 - location2).Length();
        }
    }
}
