using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum ePos { go, Arkania, TheEmperorsFavor1, Armatura, ImperialTithe, Phalanx, Tallarn, chance1,
    Lucius, GryphonneIV, AwaitingOrders, StygiesVIII, PromethiumProcessingPlant,
    Agripinaa, Ryza, TheRock, Vostroya, TheEmperorsFavor2, Necromunda, Gathalamor,
    SafeHarbor, Inwit, chance2, Nocturne, Fenris, TheMacraggesHonour, Krieg, PlasmaGenerator,
    Catachan, Vigilus, GotoTheBlackShips, Armageddon, Baal, TheEmperorsFavor3,
    Ultramar, IronRevenant, chance3, Mars, AdeptusMechanicusFee, HolyTerra, terminator }

public class NamedArrayAttribute : PropertyAttribute
{
    public Type TargetEnum;
    public NamedArrayAttribute(Type TargetEnum)
    {
        this.TargetEnum = TargetEnum;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(NamedArrayAttribute))]
public class NamedArrayDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Properly configure height for expanded contents.
        return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Replace label with enum name if possible.
        try
        {
            var config = attribute as NamedArrayAttribute;
            var enum_names = System.Enum.GetNames(config.TargetEnum);
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            var enum_label = enum_names.GetValue(pos) as string;
            // Make names nicer to read (but won't exactly match enum definition).
            enum_label = ObjectNames.NicifyVariableName(enum_label.ToLower());
            label = new GUIContent(enum_label);
        }
        catch
        {
            // keep default label
        }
        EditorGUI.PropertyField(position, property, label, property.isExpanded);
    }
}
#endif

public class Defs : MonoBehaviour
{

}
