using UnityEngine;

namespace Gongulus.MapEditor
{
    public class Node
    {
        private Rect rect;
        public GUIStyle Style;

        public Node(Vector2 position, float width, float height, GUIStyle defaultStyle)
        {
            rect = new Rect(position.x, position.y, width, height);
            Style = defaultStyle;
        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
        }
        public void Draw()
        {
            GUI.Box(rect, "", Style);
        }

        public void SetStyle(GUIStyle nodeStyle)
        {
            Style = nodeStyle;
        }
    }
}