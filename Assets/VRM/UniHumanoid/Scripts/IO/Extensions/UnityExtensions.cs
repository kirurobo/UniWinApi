using UnityEngine;


namespace UniHumanoid
{
    public static class UnityExtensions
    {
        public static Quaternion ReverseX(this Quaternion quaternion)
        {
            float angle;
            Vector3 axis;
            quaternion.ToAngleAxis(out angle, out axis);

            return Quaternion.AngleAxis(-angle, new Vector3(-axis.x, axis.y, axis.z));
        }
    }
}
