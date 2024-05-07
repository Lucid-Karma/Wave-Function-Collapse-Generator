using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityToolbarExtender.Examples
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle commandButtonStyle;

		static ToolbarStyles()
		{
			commandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageLeft,
				fontStyle = FontStyle.Bold
			};
		}
	}

	[InitializeOnLoad]
	public class SceneSwitchLeftButton {
		static SceneSwitchLeftButton() {
			//Debug.Log("left button before");
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		//Debug.Log("left button after");
		}

        static List<SceneAsset> scenes = new List<SceneAsset>();

        static void OnToolbarGUI() {

            GUILayout.BeginHorizontal();

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) {
				//Debug.Log("?");
				var scene = EditorBuildSettings.scenes[i].path;
				if (scene.Contains("Level"))
					continue;
				//Debug.Log("??");
                if (GUILayout.Button(new GUIContent(i.ToString()), ToolbarStyles.commandButtonStyle)) {
                    SceneHelper.OpenScene(EditorBuildSettings.scenes[i].path);
					//Debug.Log("winn");
                }
            }

            GUILayout.EndHorizontal();

        }

    }

	static class SceneHelper
	{
		static string sceneToOpen;

		public static void OpenScene(string scene)
		{
			if(EditorApplication.isPlaying)
			{
                return;
			}
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				EditorSceneManager.OpenScene(scene);
		}
	}
}