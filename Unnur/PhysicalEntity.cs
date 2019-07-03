﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unnur
{
    class PhysicalEntity : MoveableEntity
    {
        public PhysicalEntity(Vector2 dimensions, Vector2 coordinates, Vector2 aabbDimensions) : base(dimensions, coordinates, aabbDimensions)
        {

        }
        public void ApplyGravity()
        {
            int maxSpeed = 11;
            Vector2 newVelocity = new Vector2((float)velocity.X / 2, Math.Min(maxSpeed, velocity.Y + 1));
            velocity = newVelocity;
        }
        public override void Deflect(Entity impactSurface)
        {
            float newXVelocity = (float)(-velocity.X * 0.5);
            float newYVelocity = (float)(-velocity.Y * 0.5);
            if (Math.Abs(newXVelocity) < 0.1)
            {
                newXVelocity = 0;
            }
            if (Math.Abs(newYVelocity) < 0.1)
            {
                newYVelocity = 0;
            }
            velocity = new Vector2(newXVelocity, newYVelocity);
        }
    }
}
