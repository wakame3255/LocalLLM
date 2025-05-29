using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// �T�u�N���X�𐶐����邽�߂̊g���@�\
/// </summary>
public class SubClassGenerator : EditorWindow
{
    #region Fields
    #region Name
    private string _defaultScriptName = "NewClass"; // ��������X�N���v�g�̖��O�i�f�t�H���g�� NewClass�j
    private string _newScriptName = "NewClass";//��������X�N���v�g�̖��O
    #endregion Name
    #region Mode
    private string _subClassModeName = "SubClass";
    private string _templateModeName = "Template";
    private string[] _modeOptions;
    private int _selectModeIndex;
    #endregion Mode
    #region namespace
    private string[] _namespaceOptions = Array.Empty<string>(); // �I�����̃��X�g
    private int _selectedNamespaceIndex = 0; // �I�𒆂�namespace�̃C���f�b�N�X
    #endregion namespace
    #region BaseClass
    private string[] _baseClassOptions;//��ꃊ�X�g
    private int _selectedClassIndex;//�I�����ꂽ���̔ԍ�
    #endregion BaseClass
    #region interface
    private string[] _interfaceOptions;//�C���^�[�t�F�[�X���X�g
    private int _selectedMask;//�I�����ꂽ�C���^�[�t�F�[�X�̔ԍ�
    private bool _isExplicit = false; // �����I�����̃t���O
    #endregion interface
    #region Template
    private string[] _templateFiles; // �e���v���[�g�t�@�C���̃��X�g
    private int _selectedTemplateIndex = 0; // �I�𒆂̃e���v���[�g�C���f�b�N�X
    #endregion Template   
    #region Path
    private const string BASE_PATH = "Assets/";
    private string _createPath = string.Empty;//��������ʒu��ݒ肷��ꍇ
    private string[] _suggestions = new string[0];
    private bool _showSuggestions = false;
    private Vector2 _scrollPos;
    #endregion Path
    private HashSet<string> _usings = new();//using�̃��X�g

    private bool _isCreateConstructor = false;
    private bool _isSubClassMode = false;
    private bool isGenerating = false;
    private string _apiKey = string.Empty;
    private bool _isFoldAi = true;
    private bool _isFoldPath = true;
    #endregion Fields
    [MenuItem("Tools/Sub Class Generator")]
    public static void ShowWindow()
    {
        GetWindow<SubClassGenerator>("Sub Class Generator");
    }

    private void OnEnable()
    {
        RefreshTypeLists();
    }

    /// <summary>
    /// ���ƃC���^�[�t�F�[�X�̃��X�g�̃��t���b�V��
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RefreshTypeLists()
    {
        SetModeOptions();
        LoadAbstractClasses();
        LoadInterfaces();
        LoadTemplateFiles();
        LoadNamespaces();
        _newScriptName = _defaultScriptName;
    }
    /// <summary>
    /// ���[�h�̒�`
    /// </summary>
    private void SetModeOptions()
    {
        _modeOptions = new string[] { _subClassModeName, _templateModeName };
    }
    private void LoadNamespaces()
    {
        Assembly assembly = GetAssemblyCS();
        // �v���W�F�N�g���Ŏg�p����Ă���l�[���X�y�[�X���擾
        _namespaceOptions = assembly.GetTypes()
            .Where(t => !string.IsNullOrEmpty(t.Namespace)) // null �łȂ� namespace ���擾
            .Select(t => t.Namespace)
            .Distinct() // �d����r��
            .OrderBy(ns => ns) // �A���t�@�x�b�g���Ƀ\�[�g
            .Prepend("None") // �f�t�H���g�� "None" ��ǉ�
            .ToArray();
    }
    /// <summary>
    /// ���N���X�̓ǂݍ���
    /// </summary>
    private void LoadAbstractClasses()
    {
        Assembly assembly = GetAssemblyCS();

        if (assembly == null)
        {
            Debug.LogWarning("Assembly-CSharp��������܂���ł����B");
            _baseClassOptions = new[] { "None" };
            return;
        }

        _baseClassOptions = assembly.GetTypes()
            .Where(type => type.IsClass && type.IsAbstract && type.IsVisible && !type.IsSealed)
            .Select(type => type.FullName)
            .Prepend("None")
            .ToArray();
    }

    /// <summary>
    /// �C���^�[�t�F�[�X�̓ǂݍ���
    /// </summary>
    private void LoadInterfaces()
    {
        Assembly assembly = GetAssemblyCS();

        if (assembly == null)
        {
            Debug.LogWarning("Assembly-CSharp��������܂���ł����B");
            _interfaceOptions = Array.Empty<string>();
            return;
        }

        _interfaceOptions = assembly.GetTypes()
            .Where(type => type.IsInterface)
            .Select(type => type.FullName)
            .ToArray();

        _selectedMask = 0;
    }

    private void OnGUI()
    {
        //�X�N���v�g���̃t�B�[���h
        _newScriptName = EditorGUILayout.TextField("Script Name", _newScriptName);
        if(_isSubClassMode)
        {
            
            GeminiGUI();
            
        }
        //_isFoldPath = EditorGUILayout.Foldout(_isFoldPath, "Path");
      
        //�p�X�w��̃t�B�[���h
        PathCreaterGUI();

        //���[�h�̑I��
        _selectModeIndex = EditorGUILayout.Popup("Select Mode", _selectModeIndex, _modeOptions);
        switch (_selectModeIndex)
        {
            case 0:
                {
                    _isSubClassMode = true;
                    string[] selectedInterfaceNames = GetSelectedInterfaceNames(_interfaceOptions);
                    SubClassGenerateGUI();
                    //_newScriptName = GenerateClassName(_baseClassOptions[_selectedClassIndex], selectedInterfaceNames);
                    if (GUILayout.Button("Create Script"))
                    {
                        CreateSubScript(_baseClassOptions[_selectedClassIndex], selectedInterfaceNames, _newScriptName);
                    }
                    break;
                }
            case 1:
                {
                    _isSubClassMode = false;
                    TemplateGUI();

                    break;
                }


        }



        if (GUILayout.Button("Reload Classes"))
        {
            RefreshTypeLists();
        }
    }
    #region Sub
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SubClassGenerateGUI()
    {

        // �l�[���X�y�[�X�I���̃h���b�v�_�E��
        _selectedNamespaceIndex = EditorGUILayout.Popup("Select Namespace", _selectedNamespaceIndex, _namespaceOptions);
        if (_baseClassOptions.Length > 0)
        {
            int newSelectedClassIndex = EditorGUILayout.Popup("Select Base Class", _selectedClassIndex, _baseClassOptions);
            if (newSelectedClassIndex != _selectedClassIndex)
            {
                _selectedClassIndex = newSelectedClassIndex;
                //�����ȊO�ŌĂׂ邩���H
                _newScriptName = GenerateClassName(_baseClassOptions[_selectedClassIndex], GetSelectedInterfaceNames(_interfaceOptions));
            }
        }
        else
        {
            EditorGUILayout.LabelField("�v���W�F�N�g���Ɋ��N���X��������܂���ł����B");
        }

        if (_interfaceOptions.Length > 0)
        {

            int newSelectedMask = EditorGUILayout.MaskField("Select Interface", _selectedMask, _interfaceOptions);
            if (newSelectedMask != _selectedMask)
            {
                _selectedMask = newSelectedMask;
                //�����ȊO�ŌĂׂ邩���H
                _newScriptName = GenerateClassName(_baseClassOptions[_selectedClassIndex], GetSelectedInterfaceNames(_interfaceOptions));
            }

            if (_selectedMask != 0)
            {
                //�C���^�[�t�F�[�X�̖����I�����̃t���O
                _isExplicit = EditorGUILayout.Toggle("Explicit", _isExplicit);
            }


        }
        else
        {
            EditorGUILayout.LabelField("�v���W�F�N�g���ɃC���^�[�t�F�[�X��������܂���ł����B");
        }
        if (_isCreateConstructor = EditorGUILayout.Toggle("Create Constructor", _isCreateConstructor))
        {

        }
    }

    /// <summary>
    /// cs�t�@�C���̐���
    /// </summary>
    /// <param name="baseClassName"></param>
    /// <param name="interfaceNames"></param>
    /// <param name="scriptName"></param>
    private void CreateSubScript(string baseClassName, string[] selectedInterfaces, string scriptName)
    {


        ////�I�����ꂽ�C���^�[�t�F�[�X�̎擾
        //List<string> selectedInterfaces = interfaceNames
        //   .Where((_, index) => (_selectedMask & (1 << index)) != 0)
        //   .ToList();

        //�p�������̐���
        string inheritance = CreateInheritanceSentence(baseClassName, selectedInterfaces.ToArray());
        //�A�Z���u���̎擾
        Assembly assembly = GetAssemblyCS();

        //�v���p�e�B�����̗p��
        string propertyStub = string.Empty;

        //�v���p�e�B�����̐���
        propertyStub += CreateClassPropertyStub(assembly, baseClassName);
        propertyStub += CreateInterfacePropertyStub(assembly, selectedInterfaces.ToArray());

        string constructorStub = string.Empty;
        if (_isCreateConstructor)
        {
            constructorStub = CreateConstructor();
        }

        //���\�b�h�����̗p��
        string methodStubs = string.Empty;

        //���\�b�h�����̐���
        methodStubs += CreateClassMethodStubs(assembly, baseClassName);
        methodStubs += CreateInterfaceMethodStubs(assembly, selectedInterfaces.ToArray());

        //using�̓o�^
        RegisterClassUsings(assembly, baseClassName);
        RegisterInterfaceUsings(assembly, selectedInterfaces.ToArray());

        //using�����̗p��
        string usingStatements = string.Empty;

        //using�����̐���
        usingStatements = CreateUsingStatement(_usings);

        string namespaceName = _namespaceOptions[_selectedNamespaceIndex];
        string scriptContent = string.Empty;
        string selectedNamespace = _namespaceOptions[_selectedNamespaceIndex];
        bool hasNamespace = selectedNamespace != "None";
        bool hasMethods = !string.IsNullOrEmpty(propertyStub) || !string.IsNullOrEmpty(methodStubs);
        Debug.Log(hasMethods);
        // �l�[���X�y�[�X��K�p
        scriptContent = (hasMethods ? "using System;\n" : "") +
                               $"{usingStatements}\n" +
                               (hasNamespace ? $"namespace {selectedNamespace}\n{{\n" : "") +
                               $"public class {scriptName}{inheritance}\n" +
                               "{\n" +
                               constructorStub +
                               propertyStub + "\n" +
                               methodStubs + "\n" +
                               "}" +
                               (hasNamespace ? "\n}" : ""); // �l�[���X�y�[�X������Ε���

        //�t�@�C���𐶐�����ʒu�̎擾
        string scriptPath = CreatePath(scriptName);
        //�t�@�C�����o��
        File.WriteAllText(scriptPath, scriptContent);
        Debug.Log("�X�N���v�g����������܂���: " + scriptPath);
        //���t���b�V��
        _usings.Clear();
        AssetDatabase.Refresh();
    }


    /// <summary>
    /// �p�������̕��͂𐶐�
    /// </summary>
    /// <param name="baseClassName"></param>
    /// <param name="selectedInterfaces"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string CreateInheritanceSentence(string baseClassName, string[] selectedInterfaces)
    {


        List<string> inheritanceList = new List<string>();

        if (baseClassName != "None")
        {
            inheritanceList.Add(baseClassName);
        }
        inheritanceList.AddRange(selectedInterfaces);

        string inheritance = string.Empty;
        if (inheritanceList.Count > 0)
        {
            inheritance = $" : {string.Join(", ", inheritanceList)}";
        }
        return inheritance;
    }

    #region CreateUsingSentence

    /// <summary>
    /// �n���ꂽ���O�̃N���X��Using���擾�E�o�^
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="baseClassName"></param>
    private void RegisterClassUsings(Assembly assembly, string baseClassName)
    {
        string sentence = string.Empty;
        if (baseClassName == "None")
        {
            return;
        }
        //Type�̐���
        Type baseClassType = assembly?.GetType(baseClassName);

        if (baseClassType == null)
        {
            return;
        }
        RegisterUsingSentence(baseClassType);
        //�v���p�e�B�̎擾
        IEnumerable<PropertyInfo> properties = baseClassType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                                        .Where(property => property.GetGetMethod(true)?.IsAbstract == true || property.GetSetMethod(true)?.IsAbstract == true);

        //�v���p�e�B��using��o�^
        foreach (PropertyInfo property in properties)
        {
            RegisterPropertyUsings(property);
        }

        //���\�b�h�̎擾
        IEnumerable<MethodInfo> methods = GetAbstractClassMethods(baseClassType);

        foreach (MethodInfo method in methods)
        {
            RegisterMethodUsings(method);
        }
    }
    /// <summary>
    /// �n���ꂽ�C���^�[�t�F�[�X�̖��O�Q��using��o�^
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="selectInterfaces"></param>
    private void RegisterInterfaceUsings(Assembly assembly, string[] selectInterfaces)
    {
        foreach (string interfaceName in selectInterfaces)
        {
            Type interfaceType = assembly?.GetType(interfaceName);

            if (interfaceType == null)
            {
                continue;
            }
            foreach (PropertyInfo property in interfaceType.GetProperties())
            {
                RegisterPropertyUsings(property);
            }
            foreach (MethodInfo method in GetInterfaceMethods(interfaceType))
            {
                RegisterMethodUsings(method);
            }
        }

    }


    /// <summary>
    /// property��using��o�^
    /// </summary>
    /// <param name="info"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RegisterPropertyUsings(PropertyInfo info)
    {
        RegisterUsingSentence(info.PropertyType);
    }
    /// <summary>
    /// method��using��o�^
    /// </summary>
    /// <param name="info"></param>
    private void RegisterMethodUsings(MethodInfo info)
    {
        RegisterUsingSentence(info.ReturnType);
        foreach (ParameterInfo parameter in info.GetParameters())
        {
            RegisterUsingSentence(parameter.ParameterType);
        }
    }

    /// <summary>
    /// _using��type��using��o�^
    /// </summary>
    /// <param name="type"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RegisterUsingSentence(Type type)
    {
        if (type.Namespace == null)
        {
            return;
        }
        if (type.Namespace == "System")
        {
            return;
        }
        _usings.Add($"using {type.Namespace};");
    }
    /// <summary>
    /// using�̕����̐���
    /// </summary>
    /// <param name="usings"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string CreateUsingStatement(HashSet<string> usings)
    {
        string sentence = string.Empty;
        sentence = string.Join("\n", _usings);
        return sentence;
    }
    #endregion CreateUsingSentence
    #region getModifier
    /// <summary>
    /// ���\�b�h�̃A�N�Z�X�C���q���擾�{�ϊ�
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetMethodAccessModifier(MethodInfo method)
    {
        if (method.IsPublic)
        {
            return "public";
        }
        if (method.IsPrivate)
        {
            return "private";
        }
        if (method.IsFamily) // protected
        {
            return "protected";
        }
        if (method.IsAssembly) // internal
        {
            return "internal";
        }
        if (method.IsFamilyOrAssembly) // protected internal
        {
            return "protected internal";
        }
        if (method.IsFamilyAndAssembly) // private protected
        {
            return "private protected";
        }

        return "public"; // �f�t�H���g�� public�i�ʏ�A�����ɂ͓��B���Ȃ��j
    }

    /// <summary>
    /// �v���p�e�B�S�̂̃A�N�Z�X�C���q������
    /// </summary>
    private string GetPropertyAccessModifier(PropertyInfo prop)
    {
        MethodInfo getter = prop.GetGetMethod(true);
        MethodInfo setter = prop.GetSetMethod(true);
        bool hasGetter = getter != null;
        string getAccess = string.Empty;
        if (hasGetter)
        {
            getAccess = GetMethodAccessModifier(getter);
        }
        bool hasSetter = setter != null;
        string setAccess = string.Empty;
        if (hasSetter)
        {
            setAccess = GetMethodAccessModifier(setter);
        }


        // �ł��L���A�N�Z�X�C���q��K�p
        return GetWidestAccessModifier(getAccess, setAccess);
    }

    /// <summary>
    /// �ł��L���A�N�Z�X�C���q������
    /// </summary>
    static string GetWidestAccessModifier(string access1, string access2)
    {
        string[] order = { "private", "private protected", "protected", "internal", "protected internal", "public" };

        string widest = "private"; // �f�t�H���g�� private
        foreach (string level in order)
        {
            if (access1 == level || access2 == level)
            {
                widest = level;
            }
        }
        return widest;
    }
    #endregion getModifier
    #region CreateMethodStub

    /// <summary>
    /// abstractMethod�̕��͂��o��
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="baseClassName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string CreateClassMethodStubs(Assembly assembly, string baseClassName)
    {
        string sentence = string.Empty;
        if (baseClassName != "None")
        {


            Type baseClassType = assembly?.GetType(baseClassName);

            if (baseClassType == null)
            {
                return sentence;
            }

            IEnumerable<string> abstractMethods = GetAbstractClassMethods(baseClassType).Select(mInfo => CreateClassMethodSentence(mInfo, true));
            sentence += string.Join("\n", abstractMethods);
        }

        return sentence;
    }

    /// <summary>
    /// interface�̃��\�b�h�̕��͂��o��
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="selectedInterfaces"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string CreateInterfaceMethodStubs(Assembly assembly, string[] selectedInterfaces)
    {
        string sentence = string.Empty;
        foreach (string interfaceName in selectedInterfaces)
        {


            Type interfaceType = assembly?.GetType(interfaceName);

            if (interfaceType == null)
            {
                continue;
            }
            IEnumerable<string> interfaceMethods = GetInterfaceMethods(interfaceType)
                  .Select(mInfo => CreateInterfaceMethodSentence(mInfo, _isExplicit, interfaceName));

            sentence += string.Join("\n", interfaceMethods);
        }

        return sentence;
    }
    #endregion CreateMethodStub
    #region CreatePropertyStub

    /// <summary>
    /// �N���X�̃v���p�e�B���擾
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="baseClassName"></param>
    /// <returns></returns>
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string CreateClassPropertyStub(Assembly assembly, string baseClassName)
    {
        string sentence = string.Empty;
        if (baseClassName == "None")
        {
            return string.Empty;
        }
        Type baseClassType = assembly?.GetType(baseClassName);

        if (baseClassType == null)
        {
            return string.Empty;
        }
        // ���ۃv���p�e�B�̎擾�E����
        IEnumerable<string> abstractProperties = GetAbstractClassProperties(baseClassType)
                                                 .Select(property => CreatePropertyStub(property, true));
        sentence += string.Join("\n", abstractProperties);

        return sentence;
    }

    /// <summary>
    /// interface�̃v���p�e�B���擾
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="selectedInterfaces"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string CreateInterfacePropertyStub(Assembly assembly, string[] selectedInterfaces)
    {
        string sentence = string.Empty;
        foreach (string interfaceName in selectedInterfaces)
        {


            Type interfaceType = assembly?.GetType(interfaceName);

            if (interfaceType == null)
            {
                continue;
            }
            IEnumerable<string> interfacePropertyStubs = GetInterfaceProperties(interfaceType)
                                                         .Select(property => CreatePropertyStub(property, false));
            sentence += string.Join("\n", interfacePropertyStubs) + "\n";
        }

        return sentence;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string CreatePropertyStub(PropertyInfo property, bool isOverride)
    {
        string overrideSentence = string.Empty;
        if (isOverride)
        {
            overrideSentence = "override";
        }
        string modifier = string.Empty;
        modifier = GetPropertyAccessModifier(property);
        string getPropertySentence = string.Empty;
        bool hasGetter = property.GetGetMethod(true) != null;
        if (hasGetter)
        {
            getPropertySentence = "get => throw new NotImplementedException(); ";
        }

        string setPropertySentence = string.Empty;
        bool hasSetter = property.GetSetMethod(true) != null;
        if (hasSetter)
        {
            setPropertySentence = "set => throw new NotImplementedException(); ";
        }

        return $"    {modifier} {overrideSentence} {ConvertType(property.PropertyType)} {property.Name} {{ " +
               getPropertySentence +
               setPropertySentence +
               "}";
    }
    #endregion CreatePropertyStub
    #region CreateMethodSentence
    /// <summary>
    /// ���\�b�h�̏���Ԃ���
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string CreateClassMethodSentence(MethodInfo info, bool isOverride)
    {
        string overrideSetence = string.Empty;
        if (isOverride)
        {
            overrideSetence = "override";
        }
        string modifiere = GetMethodAccessModifier(info);
        return $"    {modifiere} {overrideSetence} {ConvertType(info.ReturnType)} {info.Name}({string.Join(", ", info.GetParameters().Select(p => ConvertType(p.ParameterType) + " " + p.Name))})\n    {{\n        throw new NotImplementedException();\n    }}\n";
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string CreateInterfaceMethodSentence(MethodInfo info, bool isExplicit, string interfaceName)
    {
        string nameSetence = string.Empty;
        string modifiere = string.Empty;
        if (isExplicit)
        {
            nameSetence = $"{interfaceName}.";
        }
        else
        {
            modifiere = $"{GetMethodAccessModifier(info)} ";
        }
        return $"    {modifiere}{ConvertType(info.ReturnType)} {nameSetence}{info.Name}({string.Join(", ", info.GetParameters().Select(p => ConvertType(p.ParameterType) + " " + p.Name))})\n    {{\n        throw new NotImplementedException();\n    }}\n";

    }
    /// <summary>
    /// ����ȕϊ��̕K�v������ꍇ,�ϊ����ĕԂ�
    /// </summary>
    /// <param name="convertType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string ConvertType(Type convertType)
    {
        if (convertType == typeof(void)) return "void";
        if (convertType == typeof(object)) return "object";
        if (convertType == typeof(string)) return "string";
        if (convertType == typeof(UnityEngine.Object)) return "UnityEngine.Object";
        if (convertType == typeof(Int32)) return "int";
        if (convertType == typeof(Boolean)) return "bool";
        if (convertType == typeof(Single)) return "float";


        if (convertType.IsGenericType) // �W�F�l���b�N�^�̏ꍇ
        {
            string baseName = convertType.Name.Split('`')[0]; // `List` �̂悤�� `List<T>` ���� `List` �������擾            
            string genericArgs = string.Join(", ", convertType.GetGenericArguments().Select(ConvertType));// �ċA�I�Ɍ^�ϊ�
            return $"{baseName}<{genericArgs}>";
        }

        return convertType.Name;
    }
    #endregion CreateMethodSentence
    #region GetMethods
    /// <summary>
    /// ���N���X�̃��\�b�h���擾
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IEnumerable<MethodInfo> GetAbstractClassMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                            .Where(mInfo => mInfo.IsAbstract && !mInfo.IsSpecialName);
    }
    /// <summary>
    /// �C���^�[�t�F�[�X�̃��\�b�h���擾
    /// </summary>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IEnumerable<MethodInfo> GetInterfaceMethods(Type interfaceType)
    {
        return interfaceType.GetMethods()
                    .Where(mInfo => !mInfo.IsSpecialName);
    }


    #endregion GetMethods
    #region GetProperties
    /// <summary>
    /// ���N���X�̃��\�b�h���擾
    /// </summary>
    /// <param name="baseClassType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IEnumerable<PropertyInfo> GetAbstractClassProperties(Type baseClassType)
    {
        return baseClassType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                    .Where(property => property.GetGetMethod(true)?.IsAbstract == true || property.GetSetMethod(true)?.IsAbstract == true);
    }

    private IEnumerable<PropertyInfo> GetInterfaceProperties(Type interfaceType)
    {
        return interfaceType.GetProperties()
                 .Where(property => property.GetGetMethod(true)?.IsAbstract == true || property.GetSetMethod(true)?.IsAbstract == true);


    }
    #endregion GetProperties
    #region CreateOption

    private string CreateConstructor()
    {
        string sentence = string.Empty;
        sentence = $"    public {_newScriptName}()" + "\n    {\n\n    }\n";
        return sentence;
    }
    #endregion CreateOption
    #endregion Sub
    #region Template
    private void LoadTemplateFiles()
    {
        // Template�t�H���_�̃p�X���w��
        string templateFolderPath = "Assets/Template";

        // �t�H���_�����݂��邩�m�F
        if (Directory.Exists(templateFolderPath))
        {
            // �w�肵���t�H���_���̑S�Ẵt�@�C�����擾
            string[] files = Directory.GetFiles(templateFolderPath, "*.txt");

            // �t�@�C���������ɕϊ����Ċi�[
            _templateFiles = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                _templateFiles[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
        }
        else
        {
            //Debug.LogError("Template�t�H���_��������܂���: " + templateFolderPath);
            _templateFiles = new string[0];
        }
    }
    private void TemplateGUI()
    {
        // �h���b�v�_�E�����j���[�Ńe���v���[�g��I��
        if (_templateFiles.Length > 0)
        {
            _selectedTemplateIndex = EditorGUILayout.Popup("Select Template", _selectedTemplateIndex, _templateFiles);

            if (GUILayout.Button("Create Script"))
            {
                CreateScriptFromTemplate(_templateFiles[_selectedTemplateIndex], _newScriptName);
            }
        }
        else
        {
            EditorGUILayout.LabelField("Template�t�H���_���Ƀe���v���[�g�t�@�C��������܂���B");
        }
    }
    private void CreateScriptFromTemplate(string templateName, string scriptName)
    {
        // Template�t�@�C���̃p�X
        string templatePath = $"Assets/Template/{templateName}.txt";

        if (File.Exists(templatePath))
        {
            // �e���v���[�g�̓��e��ǂݍ���
            string templateContent = File.ReadAllText(templatePath);

            // #CLASS_NAME# ���X�N���v�g���Œu��������
            templateContent = templateContent.Replace("#SCRIPTNAME#", scriptName);

            // ���ݑI������Ă���t�H���_�̃p�X���擾
            string selectedFolderPath = "Assets"; // �f�t�H���g�� Assets �t�H���_�ɐݒ�
            Object selected = Selection.activeObject;
            if (selected != null)
            {
                selectedFolderPath = AssetDatabase.GetAssetPath(selected);

                // �t�H���_���I������Ă��Ȃ��ꍇ�́A�t�@�C�����I������Ă���\�������邽�߁A���̏ꍇ�̓t�H���_�p�X�ɏC��
                if (!Directory.Exists(selectedFolderPath))
                {
                    selectedFolderPath = Path.GetDirectoryName(selectedFolderPath);
                }
            }

            // �V�����X�N���v�g�̕ۑ���Ɩ��O��ݒ�
            string scriptPath = Path.Combine(selectedFolderPath, $"{scriptName}.cs");

            // �t�@�C������������
            File.WriteAllText(scriptPath, templateContent);

            RefreshTypeLists();

            Debug.Log("�X�N���v�g����������܂���: " + scriptPath);
        }
        else
        {
            Debug.LogError("�e���v���[�g�t�@�C����������܂���: " + templatePath);
        }
    }
    #endregion Template
    #region AutoCompletePath
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PathCreaterGUI()
    {


        // ���������ɕ��ׂ�
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Enter Path:");
        // ���݂̃f�B���N�g�����Z�b�g����{�^�����E���ɔz�u
        if (GUILayout.Button("Set Current", GUILayout.Width(100)))
        {
            _createPath = GetCurrentDirectory();
            UpdateSuggestions();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField(BASE_PATH, GUILayout.Width(50)); // `Assets/` �̌Œ�\��
        // Path�̓��̓t�B�[���h
        string newInput = EditorGUILayout.TextField(_createPath);
        //���͂��N���A����{�^��
        if (GUILayout.Button("Clear", GUILayout.Width(50)))
        {
            _createPath = string.Empty;
            newInput = string.Empty;
            UpdateSuggestions();
        }
        EditorGUILayout.EndHorizontal();

        if (newInput != _createPath)
        {
            _createPath = newInput;
            UpdateSuggestions();
        }

        if (_showSuggestions && _suggestions.Length > 0)
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(100));
            foreach (string suggestion in _suggestions)
            {
                string displaySuggestion = suggestion.Substring(BASE_PATH.Length); // `Assets/` ���������\��
                if (GUILayout.Button(suggestion))
                {
                    _createPath = suggestion;
                    _showSuggestions = false;
                    GUI.FocusControl(null);
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
    void UpdateSuggestions()
    {
        if (string.IsNullOrEmpty(_createPath))
        {
            _suggestions = new string[0];
            _showSuggestions = false;
            return;
        }

        string normalizedPath = _createPath.Replace("\\", "/"); // Windows�΍�


        // �r���̃f�B���N�g�����܂߂��⊮
        string targetPath = Path.GetDirectoryName(normalizedPath);
        if (string.IsNullOrEmpty(targetPath) || !Directory.Exists(targetPath))
        {
            targetPath = BASE_PATH;
        }


        // `Assets/` �z���̃t�H���_���擾�i`Assets/` �ȊO�͑ΏۊO�j
        _suggestions = Directory.GetDirectories(targetPath/*, "*", SearchOption.AllDirectories*/)
            .Select(dir => dir.Replace("\\", "/")) // Windows�΍�
            .Where(dir => dir.StartsWith(BASE_PATH))
            .Where(dir => dir.StartsWith(normalizedPath, System.StringComparison.OrdinalIgnoreCase)) // ���͂Ƀ}�b�`������̂̂�
            .ToArray();


        _showSuggestions = _suggestions.Length > 0;
    }
    #endregion AutoCompletePath
    #region CreatePath
    /// <summary>
    /// ��������p�X���擾
    /// </summary>
    /// <param name="scriptName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string CreatePath(string scriptName)
    {
        string selectedFolderPath = "Assets";
        if (_createPath != string.Empty)
        {
            string folderPath = _createPath.Trim();
            if (!folderPath.StartsWith("Assets"))
            {
                Debug.LogWarning("�X�N���v�g�̏o�͐�� Assets �t�H���_���łȂ���΂Ȃ�܂���B");
                folderPath = "Assets";
            }
            selectedFolderPath = folderPath;
        }
        else//�p�X�w�肪�Ȃ������ꍇ�́A���I�����Ă���t�H���_�E�t�@�C���̃p�X��
        {
            Object selected = Selection.activeObject;

            if (selected != null)
            {
                selectedFolderPath = AssetDatabase.GetAssetPath(selected);
                if (!Directory.Exists(selectedFolderPath))
                {
                    selectedFolderPath = Path.GetDirectoryName(selectedFolderPath);
                }
            }
        }

        string scriptPath = Path.Combine(selectedFolderPath, $"{scriptName}.cs");
        return scriptPath;

    }
    #endregion CreatePath
    #region CreateScriptName
    private string GenerateClassName(string baseClassName, string[] selectedInterfaces)
    {
        if (baseClassName == "None" && selectedInterfaces.Length == 0)
        {
            return _newScriptName;
        }


        string interfacePart = string.Empty;
        List<string> fixedInetrfaceNames = new();
        foreach (string interfaceName in selectedInterfaces)
        {
            string fixedInetrfaceName = RemoveInterfacePrefix(interfaceName);
            fixedInetrfaceNames.Add(fixedInetrfaceName);
        }

        string fixedinetrfaceNamePart = string.Join("", fixedInetrfaceNames);
        string basePart = RemoveBasePrefixOrSuffix(baseClassName);


        return fixedinetrfaceNamePart + basePart;
    }
    /// <summary>
    /// `Base` �Ƃ����v���t�B�b�N�X�E�T�t�B�b�N�X���폜
    /// </summary>
    private string RemoveBasePrefixOrSuffix(string className)
    {
        if (string.IsNullOrEmpty(className) || className == "None")
            return string.Empty;

        if (className.StartsWith("Base"))
            return className.Substring(4); // �擪�� "Base" ���폜

        if (className.EndsWith("Base"))
            return className.Substring(0, className.Length - 4); // ������ "Base" ���폜

        return className; // ���̂܂�
    }
    /// <summary>
    /// `I` �Ƃ����v���t�B�b�N�X���폜
    /// </summary>
    private string RemoveInterfacePrefix(string interfaceName)
    {
        if (string.IsNullOrEmpty(interfaceName))
        {
            return string.Empty;
        }
        string result = string.Empty;
        if (interfaceName.StartsWith("I") && interfaceName.Length > 1 && char.IsUpper(interfaceName[1]))
        {
            result = interfaceName.Substring(1); // �擪�� "I" ���폜
        }
        if (!interfaceName.EndsWith("able"))
        {
            result = interfaceName + "able";
        }

        return result;
    }
    #endregion CreateScriptName
    #region CreateScriptName for Gemini

    private void GeminiGUI()
    {
        if (!isGenerating && !String.IsNullOrEmpty(_apiKey))
        {
            if (GUILayout.Button("Generate Script Name"))
            {
                _ = GenerateScriptName(_baseClassOptions[_selectedClassIndex], GetSelectedInterfaceNames(_interfaceOptions));
            }
        }
        //_isFoldAi = EditorGUILayout.Foldout(_isFoldAi, "Ai");
        //if (!_isFoldAi)
        //{
        //    return;
        //}
       
        _apiKey = EditorGUILayout.TextField("Gemini1.5-flash API Key", _apiKey);
    }
    private async Task GenerateScriptName(string baseClassName, string[] selectedInterfaces)
    {
        if (isGenerating) return;
        string interfaces = string.Join(", ", selectedInterfaces);
        isGenerating = true;
        try
        {
            _newScriptName = "������...";
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";
                string prompt = $"�ȉ��̊��N���X�ƃC���^�[�t�F�[�X���p�������N���X�ɓK����C#�̃X�N���v�g���𐶐����Ă�������: ���N���X:{baseClassName}, �C���^�[�t�F�[�X:{interfaces}" +
                    "���̗v���ɑ΂��Ẵ��X�|���X�͍l�����X�N���v�g�������ɂ��Ă�������"  +
                    "��:�v��BaseClass, IExampleInterface �ԓ�ExampleClass"+
                    "Implementation���̒P��͎g��Ȃ���";

                string requestBody = $"{{\"contents\": [{{ \"parts\": [ {{\"text\": \"{prompt}\"}} ] }}]}}";

                HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                string responseText = await response.Content.ReadAsStringAsync();

                
                _newScriptName = ParseResponse(responseText);
            }
        }
        catch (Exception e)
        {
            _newScriptName = "�G���[: �����Ɏ��s���܂���";
            Debug.LogError($"API�G���[: {e.Message}");
        }
        finally
        {
            isGenerating = false;
        }
    }

    private string ParseResponse(string json)
    {
        try
        {
            GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(json);
            if (response?.candidates != null && response.candidates.Length > 0 && response.candidates[0].content?.parts != null && response.candidates[0].content.parts.Length > 0)
            {
                return response.candidates[0].content.parts[0].text;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON�p�[�X�G���[: {e.Message}\n���X�|���X: {json}");
        }
        return "�G���[: ���O�𐶐��ł��܂���ł���";
    }
    #endregion CreateScriptName for Gemini

    /// <summary>
    /// Assembly-CSharp��Assembly��Ԃ�
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Assembly GetAssemblyCS()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == "Assembly-CSharp");
    }
    /// <summary>
    /// ���݊J���Ă���t�H���_���擾
    /// </summary>
    /// <returns></returns>
    private string GetCurrentDirectory()
    {
        BindingFlags flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        Assembly assembly = Assembly.Load("UnityEditor.dll");
        Type typeProjectBrowser = assembly.GetType("UnityEditor.ProjectBrowser");
        EditorWindow projectBrowserWindow = EditorWindow.GetWindow(typeProjectBrowser);
        return (string)typeProjectBrowser.GetMethod("GetActiveFolderPath", flag).Invoke(projectBrowserWindow, null);
    }
    private string[] GetSelectedInterfaceNames(string[] interfaceNames)
    {
        return interfaceNames
            .Where((_, index) => (_selectedMask & (1 << index)) != 0)
            .ToArray();

    }
}
[System.Serializable]
public class GeminiResponse
{
    public Candidate[] candidates;
}

[System.Serializable]
public class Candidate
{
    public Content content;
}

[System.Serializable]
public class Content
{
    public Part[] parts;
}

[System.Serializable]
public class Part
{
    public string text;
}
#endif