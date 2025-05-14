using UnityEngine;

namespace UnityEditor
{



    [CustomEditor(typeof(CustomRuleTile))]
    [CanEditMultipleObjects]
    public class CustomRuleTileEditor : RuleTileEditor
    {
        public Texture2D any;
        public Texture2D specified;
        public Texture2D empty;

        public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
        {
            switch (neighbor)
            {
                case 3:
                    GUI.DrawTexture(rect, any);
                    break;
                case 4:
                    GUI.DrawTexture(rect, specified);
                    break;
                case 5:
                    GUI.DrawTexture(rect, empty);
                    break;

            }
            base.RuleOnGUI(rect, position, neighbor);
        }
    }
}
