using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unnur
{
    class PhysicalEntity : MovingEntity
    {
        public enum PhysicalObjectState
        {
            Rest,
            Fall
        }
        public PhysicalObjectState CurrentState = PhysicalObjectState.Rest;
        private int maxSpeed = 11;

        public PhysicalEntity(Vector2 dimensions, Vector2 coordinates, Vector2 aabbDimensions, Scene currentScene) : base(dimensions, coordinates, aabbDimensions, currentScene)
        {

        }
        public void ApplyGravity()
        {
            int maxSpeed = 11;
            Vector2 newVelocity = new Vector2((float)Velocity.X / 2, Math.Min(maxSpeed, Velocity.Y + 1));
            Velocity = newVelocity;
        }
        public virtual void Update(KeyboardState keyState, MouseState mouseState, Collision collision, Scene currentScene)
        {
            switch (CurrentState)
            {
                case PhysicalObjectState.Rest:
                    SetVelocity(0, 0);
                    /// some spritesheet setting logic should go here too
                    if (!OnGround)
                    {
                        CurrentState = PhysicalObjectState.Fall;
                        break;
                    }
                    break;
                case PhysicalObjectState.Fall:
                    /// gravity thingus dingus
                    /// 
                    /// Animation timing stuff goes here
                    SetVelocity(Velocity.X, Math.Min(maxSpeed, Velocity.Y + 1));

                    //if we hit the ground
                    if (OnGround)
                    {
                        //if there's no movement change state to standing

                        CurrentState = PhysicalObjectState.Rest;
                        SetVelocity(Velocity.X, 0);
                    }
                    break;
            }

            UpdatePhysics();

            if (OnGround && !WasOnGround)
            {
                /// play landing sound effect
            }
            if (!WasAtCeiling && AtCeiling
                || !PushedLeftWall && PushesLeftWall
                || !PushedRightWall && PushesRightWall)
            {
                /// play bumping sound if it's different from planding sound
            }
            OnGround = IsOnGround(collision, currentScene);
            ResetOccupiedTiles(currentScene);

        }
        public bool IsOnGround(Collision collision, Scene currentScene)
        {
            var leftPixel = (int)Aabb.GetLeft();
            var rightPixel = (int)Aabb.GetRight();
            var yPixel = (int)Aabb.GetBottom();

            var tileWidth = 32;
            var tileY = yPixel / tileWidth;

            var localCollides = Enumerable
                .Range((leftPixel / tileWidth), rightPixel / tileWidth) // tile X indices
                .Select(tileX => new Point(tileX, tileY))               // tile Point coordinates
                .Select(tileIndex => currentScene.GetTile(tileIndex))   // tiles
                .SelectMany(tile => tile.GetLocalEntities())            // entities in those tiles
                .Where(entity => entity.IsCollideable());               // collidable entities

            var bottomEdgePixels = Enumerable.Range(leftPixel, rightPixel - leftPixel);

            // do any of the pixels on our bottom edge collide with the nearby entities?
            return bottomEdgePixels.Any(x =>
                localCollides.Any(
                    collideableEntity =>
                        collision.PointCollisionCheck(
                            new Vector2(x, yPixel + 1),
                            collideableEntity)));
        }
    }
}
