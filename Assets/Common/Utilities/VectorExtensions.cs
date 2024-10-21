using UnityEngine;

namespace Common.Utility
{
    public static class VectorExtensions
    {
        public static Vector3 FlattenZ(this Vector3 vector, float amount)
        {
            return new Vector3(vector.x, vector.y, amount);
        }
        public static Vector3 FlattenZ(this Vector3 vector)
        {
            return vector.FlattenZ(0);
        }
        public static Vector3 FlattenX(this Vector3 vector, float amount)
        {
            return new Vector3(amount, vector.y, vector.z);
        }
        public static Vector3 FlattenX(this Vector3 vector)
        {
            return vector.FlattenX(0);
        }
        public static Vector3 FlattenY(this Vector3 vector, float amount)
        {
            return new Vector3(vector.x, amount, vector.z);
        }
        public static Vector3 FlattenY(this Vector3 vector)
        {
            return vector.FlattenY(0);
        }
        public static Vector2 FlattenX(this Vector2 vector, float amount)
        {
            return new Vector3(amount, vector.y);
        }
        public static Vector2 FlattenX(this Vector2 vector)
        {
            return vector.FlattenX(0);
        }
        public static Vector2 FlattenY(this Vector2 vector, float amount)
        {
            return new Vector2(vector.x, amount);
        }
        public static Vector2 FlattenY(this Vector2 vector)
        {
            return vector.FlattenY(0);
        }
    }
}
