using UnityEngine;

namespace Gongulus.MapEditor
{
    public class StyleManager : MonoBehaviour
    {
        public Vector2Int Offset;
        public ButtonStyle[] ButtonStyles;
    }

    [System.Serializable]
    public struct ButtonStyle
    {
        public Texture2D Icon;
        public string ButtonText;

        [HideInInspector] public GUIStyle NodeStyle;

        public GameObject PrefabObejct;
    }
}