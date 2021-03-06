﻿using System.Numerics;
using MessagePack;

namespace ImperialStudio.Api.Serialization
{
    [MessagePackObject]
    public struct SVector3
    {
        [Key(0)]
        public float X;
        [Key(1)]
        public float Y;
        [Key(2)]
        public float Z;

        public SVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public SVector3(Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        public Vector3 ToSystemVector()
        {
            return new Vector3(X, Y, Z);
        }

        public static implicit operator SVector3(Vector3 vector)
        {
            return new SVector3(vector);
        }

        public static implicit operator Vector3(SVector3 vector)
        {
            return vector.ToSystemVector();
        }
    }

    [MessagePackObject]
    public struct SVector2
    {
        [Key(0)]
        public float X;
        [Key(1)]
        public float Y;

        public SVector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public SVector2(Vector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public static implicit operator SVector2(Vector2 vector)
        {
            return new SVector2(vector);
        }

        public static implicit operator Vector2(SVector2 vector)
        {
            return vector.ToSystemVector();
        }
        
        public Vector2 ToSystemVector()
        {
            return new Vector2(X, Y);
        }
    }
}

