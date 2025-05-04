using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Editor {
    [CreateAssetMenu(fileName = "ColorPalette", menuName = "KitchenChaos/UI/Color Palette")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    public class EditorColorPalette : ScriptableObject {
        [Header("Background Colors")]
        [SerializeField]
        private Color accentBackground = new(0.298f, 0.133f, 0.117f); // #4C221E
        [SerializeField]
        private Color alertBackground = new(0.4f, 0.105f, 0.082f); // #661B15
        [SerializeField]
        private Color mutedAccentBackground = new(0.251f, 0.188f, 0.188f); // #402F2F
        [SerializeField]
        private Color warmBackground = new(0.321f, 0.243f, 0.235f); // #523E3C
        [SerializeField]
        private Color secondaryAccentBackground = new(0.149f, 0.266f, 0.364f); // #264459
        [SerializeField]
        private Color secondaryAlertBackground = new(0.106f, 0.204f, 0.301f); // #1B3450
        [SerializeField]
        private Color mutedSecondaryBackground = new(0.180f, 0.231f, 0.294f); // #2E3B4B
        [SerializeField]
        private Color coolBackground = new(0.200f, 0.258f, 0.337f); // #33425A

        [Header("Text Colors")]
        [SerializeField]
        private Color primaryText = new(0.925490f, 0.941176f, 0.945098f); // #ECF0F1
        [SerializeField]
        private Color secondaryText = new(0.741176f, 0.764706f, 0.780392f); // #BDC3C7
        [SerializeField]
        private Color accentText = new(0.945098f, 0.768627f, 0.058824f); // #F1C40F

        [Header("Primary Button Colors")]
        [SerializeField]
        private Color primaryButtonNormal = new(0.905882f, 0.298039f, 0.235294f); // #E74C3C
        [SerializeField]
        private Color primaryButtonHighlighted = new(0.752941f, 0.223529f, 0.168627f); // #C0392B
        [SerializeField]
        private Color primaryButtonPressed = new(0.631373f, 0.188235f, 0.141176f); // #A12B23
        [SerializeField]
        private Color primaryButtonSelected = new(0.752941f, 0.223529f, 0.168627f); // #C0392B
        [SerializeField]
        private Color primaryButtonDisabled = new(0.498039f, 0.549020f, 0.552941f, 0.5f); // #7F8C8D

        [Header("Secondary Button Colors")]
        [SerializeField]
        private Color secondaryButtonNormal = new(0.203922f, 0.596078f, 0.858824f); // #3498DB
        [SerializeField]
        private Color secondaryButtonHighlighted = new(0.160784f, 0.501961f, 0.725490f); // #2980B9
        [SerializeField]
        private Color secondaryButtonPressed = new(0.141176f, 0.447059f, 0.647059f); // #2471A3
        [SerializeField]
        private Color secondaryButtonSelected = new(0.160784f, 0.501961f, 0.725490f); // #2980B9
        [SerializeField]
        private Color secondaryButtonDisabled = new(0.498039f, 0.549020f, 0.552941f, 0.5f); // #7F8C8D


        [Header("UI Element Colors")]
        [SerializeField]
        private Color successColor = new(0.180392f, 0.800000f, 0.443137f); // #2ECC71
        [SerializeField]
        private Color warningColor = new(0.945098f, 0.768627f, 0.058824f); // #F1C40F
        [SerializeField]
        private Color errorColor = new(0.905882f, 0.298039f, 0.235294f); // #E74C3C
        [SerializeField]
        private Color borderColor = new(0.203922f, 0.286275f, 0.368627f); // #34495E

        [Header("Additional Colors")]
        [SerializeField]
        private Color highlightColor = new(0.945098f, 0.768627f, 0.058824f); // #F1C40F
        [SerializeField]
        private Color shadowColor = new(0.172549f, 0.243137f, 0.313725f, 0.3f); // #2C3E50
    }

    [CustomEditor(typeof(EditorColorPalette))]
    public class ColorPaletteEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            EditorGUILayout.HelpBox(
                "Use these colors in your UI elements by copying the color values from the inspector.",
                MessageType.Info
            );

            DrawDefaultInspector();
        }
    }
#endif
}
