using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LegendaryTools.EditorTime
{
    /// <summary>
    /// A helper class for instantiating ScriptableObjects in the editor.
    /// </summary>
    public class ScriptableObjectFactory
    {
        [MenuItem("ScriptableObject/Create")]
        public static void CreateScriptableObject()
        {
            var assembly = GetAssembly();

            // Get all classes derived from ScriptableObject
            var allScriptableObjects = (from t in assembly.GetTypes()
                                        where t.IsSubclassOf(typeof(UnityEngine.ScriptableObject)) && !t.IsAbstract
                                        select t).ToArray();

            // Show the selection window.
            ScriptableObjectWindow.Init(allScriptableObjects);
        }

        /// <summary>
        /// Returns the assembly that contains the script code for this project (currently hard coded)
        /// </summary>
        private static Assembly GetAssembly()
        {
            return Assembly.Load(new AssemblyName("Assembly-CSharp"));
        }
    }
}