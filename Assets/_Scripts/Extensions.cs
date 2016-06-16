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
    }
}