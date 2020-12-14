#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using ECM.Controllers;
using ECM.Components;

public class CustomComponentEditor : EditorWindow
{
    [MenuItem("CustomEditor/CustomComponentEditor Menu")]
    static void Init()
    {
        CustomComponentEditor window = (CustomComponentEditor)EditorWindow.GetWindow(typeof(CustomComponentEditor));
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginToggleGroup("Utility", true);
        if (GUILayout.Button("Add GameCharacter to CharPrefabs")) AddGameCharComponentToCharPrefabs();
        if (GUILayout.Button("Add BoxCollider to CharPrefabs")) AddBoxColliderToCharPerfabs();
        if (GUILayout.Button("Add CharacterController to CharPrefabs")) AddCharControllerToCharPrefabs();
        if (GUILayout.Button("Add ECM Component to CharPrefabs")) AddECMComponentToCharPrefabs();
        if (GUILayout.Button("Add RigidBody to CharPrefabs")) AddRigidBodyToCharPrefabs();
        if (GUILayout.Button("Add Components to NPCPrefabs")) AddComponentsToNPCPrefabs();
        if (GUILayout.Button("Add Components to AnimalPrefabs")) AddComponentsToAnimalPrefabs();
        EditorGUILayout.EndToggleGroup();
    }

    private void AddECMComponentToCharPrefabs()
    {
        KojeomLogger.DebugLog("GameCharacter 컴포넌트 할당 작업 시작.");
        GameObject[] charPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER_RESOURCE_PATH);
        foreach (var charPrefab in charPrefabs)
        {
            if (charPrefab.GetComponent<BaseFirstPersonController>() == null) charPrefab.AddComponent<BaseFirstPersonController>();
            if (charPrefab.GetComponent<GroundDetection>() == null) charPrefab.AddComponent<GroundDetection>();
            if (charPrefab.GetComponent<CharacterMovement>() == null) charPrefab.AddComponent<CharacterMovement>();
            if (charPrefab.GetComponent<MouseLook>() == null) charPrefab.AddComponent<MouseLook>();
            PrefabUtility.SavePrefabAsset(charPrefab);
        }
        KojeomLogger.DebugLog("GameCharacter 컴포넌트 할당 작업 완료.");
    }

    private void AddGameCharComponentToCharPrefabs()
    {
        KojeomLogger.DebugLog("GameCharacter 컴포넌트 할당 작업 시작.");
        GameObject[] charPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER_RESOURCE_PATH);
        foreach(var charPrefab in charPrefabs)
        {
            if(charPrefab.GetComponent<GameCharacterInstance>() == null)
            {
                var gameCharacterComponent = charPrefab.AddComponent<GameCharacterInstance>();
            }
            else
            {
                KojeomLogger.DebugLog("이미 GameCharacter 컴포넌트가 붙어있습니다.");
            }
            PrefabUtility.SavePrefabAsset(charPrefab);
        }
        KojeomLogger.DebugLog("GameCharacter 컴포넌트 할당 작업 완료.");
    }

    private void AddBoxColliderToCharPerfabs()
    {
        KojeomLogger.DebugLog("BoxCollider 컴포넌트 할당 작업 시작.");
        GameObject[] charPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER_RESOURCE_PATH);
        foreach (var element in charPrefabs)
        {
            if (element.GetComponent<BoxCollider>() == null)
            {
                var coll = element.AddComponent<BoxCollider>();
                coll.size = new Vector3(0.3f, 0.7f, 0.3f);
                coll.center = new Vector3(0.0f, 0.5f, 0.0f);
            }
            else
            {
                KojeomLogger.DebugLog("이미 BoxCollider 컴포넌트가 붙어있습니다.");
            }
            PrefabUtility.SavePrefabAsset(element);
        }
        KojeomLogger.DebugLog("GameCharacter 컴포넌트 할당 작업 완료.");
    }

    private void AddComponentsToAnimalPrefabs()
    {
        AddComponentsToActor(false, new Vector3(0.3f, 1.0f, 0.3f), new Vector3(0.0f, 0.5f, 0.0f), 1.0f,
           true, RigidbodyConstraints.FreezeRotation, ACTOR_TYPE.ANIMAL, ConstFilePath.ANIMAL_PREFABS_RESOURCE_PATH);
    }

    private void AddComponentsToNPCPrefabs()
    {
        AddComponentsToActor(true, new Vector3(0.3f, 1.0f, 0.3f), new Vector3(0.0f, 0.5f, 0.0f), 1.0f,
            true, RigidbodyConstraints.FreezeRotation, ACTOR_TYPE.NPC, ConstFilePath.NPC_PREFABS_RESOURCE_PATH);
    }

    private void AddComponentsToActor(bool addCollider, Vector3 collSize, Vector3 collCenter, float rigidBodyMass, bool useGravity, RigidbodyConstraints rigidBodyConstraints, ACTOR_TYPE actorType, string prefabsPath)
    {
        KojeomLogger.DebugLog("Actor 프리팹에 필수적인 컴포넌트 추가 작업 시작.");
        GameObject[] actorPrefabs = Resources.LoadAll<GameObject>(prefabsPath);
        foreach (var element in actorPrefabs)
        {
            if(addCollider == true)
            {
                var collider = element.GetComponent<BoxCollider>();
                if (collider != null)
                {
                    DestroyImmediate(collider, true);
                }
                var coll = element.AddComponent<BoxCollider>();
                coll.size = collSize;
                coll.center = collCenter;
            }
            //
            ActorController controller = null;
            switch (actorType)
            {
                case ACTOR_TYPE.NPC:
                    controller = element.GetComponent<NPCController>();
                    if (controller != null)
                    {
                        DestroyImmediate(controller, true);
                    }
                    element.AddComponent<NPCController>();
                    break;
                case ACTOR_TYPE.ANIMAL:
                    controller = element.GetComponent<AnimalController>();
                    if (controller != null)
                    {
                        DestroyImmediate(controller, true);
                    }
                    element.AddComponent<AnimalController>();
                    break;
                case ACTOR_TYPE.MONSTER:
                    break;
            }
            
            //
            var comp = element.GetComponent<Rigidbody>();
            if (comp != null)
            {
                DestroyImmediate(comp, true);
            }
            var rigidBody = element.AddComponent<Rigidbody>();
            rigidBody.mass = rigidBodyMass;
            rigidBody.useGravity = useGravity;
            rigidBody.constraints = rigidBodyConstraints;
            //
            //ref : https://docs.unity3d.com/ScriptReference/PrefabUtility.InstantiatePrefab.html
            //if (element.GetComponentInChildren<TMPro.TextMeshPro>() == null)
            //{
            //    // 텍스처매쉬를 인스턴싱---> 계층구조를 설정하면 에러발생.
            //    var meshInstance = Instantiate(Resources.Load<TMPro.TextMeshPro>(ConstFilePath.ACTOR_COMMONS_NAME_RESOURCE_PATH), Vector3.zero, Quaternion.identity);
            //    meshInstance.transform.SetParent(element.transform);
            //    DestroyImmediate(meshInstance);
            //}
            PrefabUtility.SavePrefabAsset(element);
        }
        KojeomLogger.DebugLog("Actor 프리팹에 필수적인 컴포넌트 추가 작업 완료.");
    }

    private void AddCharControllerToCharPrefabs()
    {
        KojeomLogger.DebugLog("CharacterController 컴포넌트 할당 작업 시작.");
        GameObject[] charPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER_RESOURCE_PATH);
        foreach (var element in charPrefabs)
        {
            if (element.GetComponent<CharacterController>() == null)
            {
                var charController = element.AddComponent<CharacterController>();
            }
            else
            {
                KojeomLogger.DebugLog("이미 CharacterController 컴포넌트가 붙어있습니다.");
            }
            PrefabUtility.SavePrefabAsset(element);
        }
        KojeomLogger.DebugLog("CharacterController 컴포넌트 할당 작업 완료.");
    }

    private void AddRigidBodyToCharPrefabs()
    {
        KojeomLogger.DebugLog("Rigidbody 컴포넌트 할당 작업 시작.");
        GameObject[] charPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER_RESOURCE_PATH);
        foreach (var element in charPrefabs)
        {
            var comp = element.GetComponent<Rigidbody>();
            if (comp != null)
            {
                DestroyImmediate(comp, true);
            }
            var rigidBody = element.AddComponent<Rigidbody>();
            rigidBody.mass = 1.0f;
            rigidBody.useGravity = true;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            rigidBody.isKinematic = false;
            PrefabUtility.SavePrefabAsset(element);
        }
        KojeomLogger.DebugLog("Rigidbody 컴포넌트 할당 작업 완료.");
    }

    private void AddComponentToPrefab<T>()
    {

    }

    private void RemoveComponent<T>()
    {
        KojeomLogger.DebugLog("컴포넌트 삭제 작업 시작.");
        GameObject[] charPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER_RESOURCE_PATH);
        foreach (var element in charPrefabs)
        {
            T component = element.GetComponent<T>();
            if (component != null)
            {
                DestroyImmediate(component as Object, true);
            }
            PrefabUtility.SavePrefabAsset(element);
        }
    }
}
#endif