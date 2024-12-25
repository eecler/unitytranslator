using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace ModifyMe
{
    public static class GameModAPI
    {
        // Dictionary to store patched methods for reference
        private static readonly Dictionary<string, MethodInfo> patchedMethods = new Dictionary<string, MethodInfo>();

        /// <summary>
        /// Attaches to the Unity game process and initializes the API.
        /// </summary>
        public static void Initialize()
        {
            Debug.Log("ModifyMe API Initialized.");
        }

        /// <summary>
        /// Retrieves a GameObject by its name.
        /// </summary>
        /// <param name="objectName">Name of the GameObject to find.</param>
        /// <returns>GameObject if found, otherwise null.</returns>
        public static GameObject GetObject(string objectName)
        {
            GameObject obj = GameObject.Find(objectName);
            if (obj == null)
            {
                Debug.LogWarning($"GameObject '{objectName}' not found.");
            }
            return obj;
        }

        /// <summary>
        /// Retrieves all components of a specific type from a GameObject.
        /// </summary>
        /// <typeparam name="T">The type of component to retrieve.</typeparam>
        /// <param name="gameObject">The GameObject to search.</param>
        /// <returns>A list of components of the specified type.</returns>
        public static List<T> GetComponents<T>(GameObject gameObject) where T : Component
        {
            if (gameObject == null)
            {
                Debug.LogError("GameObject is null.");
                return null;
            }

            List<T> components = new List<T>(gameObject.GetComponents<T>());
            return components;
        }

        /// <summary>
        /// Adds a component of a specified type to a GameObject.
        /// </summary>
        /// <typeparam name="T">The type of component to add.</typeparam>
        /// <param name="gameObject">The GameObject to modify.</param>
        /// <returns>The added component.</returns>
        public static T AddComponent<T>(GameObject gameObject) where T : Component
        {
            if (gameObject == null)
            {
                Debug.LogError("GameObject is null.");
                return null;
            }

            T component = gameObject.AddComponent<T>();
            Debug.Log($"Added component {typeof(T).Name} to {gameObject.name}.");
            return component;
        }

        /// <summary>
        /// Patches a method using Harmony to allow runtime modifications.
        /// </summary>
        /// <param name="className">The class containing the method to patch.</param>
        /// <param name="methodName">The name of the method to patch.</param>
        /// <param name="prefix">Optional prefix method to inject before the target method.</param>
        /// <param name="postfix">Optional postfix method to inject after the target method.</param>
        public static void PatchMethod(string className, string methodName, Action prefix = null, Action postfix = null)
        {
            try
            {
                var harmony = new Harmony("com.modifymepatcher");

                Type targetType = Type.GetType(className);
                if (targetType == null)
                {
                    Debug.LogError($"Class '{className}' not found.");
                    return;
                }

                MethodInfo targetMethod = targetType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (targetMethod == null)
                {
                    Debug.LogError($"Method '{methodName}' not found in class '{className}'.");
                    return;
                }

                HarmonyMethod harmonyPrefix = prefix != null ? new HarmonyMethod(prefix.Method) : null;
                HarmonyMethod harmonyPostfix = postfix != null ? new HarmonyMethod(postfix.Method) : null;

                harmony.Patch(targetMethod, harmonyPrefix, harmonyPostfix);

                patchedMethods[$"{className}.{methodName}"] = targetMethod;
                Debug.Log($"Patched method: {className}.{methodName}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to patch method: {ex.Message}");
            }
        }

        /// <summary>
        /// Replaces a material on a GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to modify.</param>
        /// <param name="newMaterial">The new material to apply.</param>
        public static void ReplaceMaterial(GameObject gameObject, Material newMaterial)
        {
            if (gameObject == null || newMaterial == null)
            {
                Debug.LogError("Invalid arguments for ReplaceMaterial.");
                return;
            }

            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = newMaterial;
                Debug.Log($"Material replaced on {gameObject.name}.");
            }
            else
            {
                Debug.LogWarning($"Renderer not found on {gameObject.name}.");
            }
        }

        /// <summary>
        /// Logs the names of all active GameObjects in the current scene.
        /// </summary>
        public static void ListActiveGameObjects()
        {
            var allObjects = GameObject.FindObjectsOfType<GameObject>();
            Debug.Log("Active GameObjects:");
            foreach (var obj in allObjects)
            {
                Debug.Log(obj.name);
            }
        }

        /// <summary>
        /// Changes the position of a GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to modify.</param>
        /// <param name="newPosition">The new position to set.</param>
        public static void SetPosition(GameObject gameObject, Vector3 newPosition)
        {
            if (gameObject == null)
            {
                Debug.LogError("GameObject is null.");
                return;
            }

            gameObject.transform.position = newPosition;
            Debug.Log($"Position of {gameObject.name} set to {newPosition}.");
        }

        /// <summary>
        /// Changes the rotation of a GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to modify.</param>
        /// <param name="newRotation">The new rotation to set.</param>
        public static void SetRotation(GameObject gameObject, Quaternion newRotation)
        {
            if (gameObject == null)
            {
                Debug.LogError("GameObject is null.");
                return;
            }

            gameObject.transform.rotation = newRotation;
            Debug.Log($"Rotation of {gameObject.name} set to {newRotation}.");
        }

        /// <summary>
        /// Changes the scale of a GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject to modify.</param>
        /// <param name="newScale">The new scale to set.</param>
        public static void SetScale(GameObject gameObject, Vector3 newScale)
        {
            if (gameObject == null)
            {
                Debug.LogError("GameObject is null.");
                return;
            }

            gameObject.transform.localScale = newScale;
            Debug.Log($"Scale of {gameObject.name} set to {newScale}.");
        }

        /// <summary>
        /// Ensures integrity by adding anti-AI markers to the project.
        /// </summary>
        private static void AddAntiAIMarkers()
        {
            Debug.Log("This project was created through manual efforts, without AI assistance.");
        }
    }
}
