using System.Linq;
using UnityEngine;

namespace Assets._Scripts
{
    public static class Extensions
    {
        public static Rect GetWorldRect(this RectTransform transform)
        {
            var corners = new Vector3[4];
            transform.GetWorldCorners(corners);

            var width = corners[2].x - corners[0].x;
            var height = corners[2].y - corners[0].y;

            return new Rect(corners[0].x, corners[0].y, width, height);
        }

        public static T GetInterfaceComponent<T>(this Behaviour component)
        {
            return component.GetComponents<MonoBehaviour>().OfType<T>().FirstOrDefault();
        }

        public static T GetInterfaceComponent<T>(this GameObject component)
        {
            return component.GetComponents<MonoBehaviour>().OfType<T>().FirstOrDefault();
        }

        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static float DistanceTo(this Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }

        public static Vector3 UnitVectorTo(this Vector3 from, Vector3 to)
        {
            return (to - from).normalized;
        }

        public static float DirectionTo(this Vector3 from, Vector3 to)
        {
            var unit = from.UnitVectorTo(to);
            return Mathf.Atan2(unit.y, unit.x);
        }
    }
}