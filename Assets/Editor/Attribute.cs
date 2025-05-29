using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field)]
public class SceneReferenceAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneReferenceAttribute))]
public class SceneReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // **�^�`�F�b�N**
        if (property.propertyType != SerializedPropertyType.String)
        {
            // �G���[��\��
            EditorGUI.LabelField(position, label.text, "[SceneReference] �� string �ɂ̂ݓK�p�\�I");
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        // ���݂̃V�[���A�Z�b�g���擾
        SceneAsset sceneAsset = null;
        if (!string.IsNullOrEmpty(property.stringValue))
        {
            sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(property.stringValue);
        }

        // �V�[���A�Z�b�g�̑I���t�B�[���h��\��
        SceneAsset newScene = (SceneAsset)EditorGUI.ObjectField(position, label, sceneAsset, typeof(SceneAsset), false);

        // �I�����ꂽ�V�[���̃p�X��ۑ�
        if (newScene != null)
        {
            string scenePath = AssetDatabase.GetAssetPath(newScene);
            if (scenePath.EndsWith(".unity"))
            {
                property.stringValue = scenePath;
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif

