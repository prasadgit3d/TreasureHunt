using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;




namespace SillyGames.SGBase.Localization
{
	/// <summary>
	/// Interface for the GenericLocalizationEditor class.
	/// Non generic abstraction of the GenericLocalizationEditor<T,V> class.
	/// </summary>
	public interface IGenericLocalizationEditor 
    {
		void InitializeEditor (Object targetObject, Editor callingWindow);
		void OnInspectorGUI ();
		void ReleaseEditor ();
	}
}