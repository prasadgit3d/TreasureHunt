using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SillyGames.SGBase.Localization;


/// <summary>
/// Localization object.
/// A non generic abstraction of the LocalizationBase class
/// </summary>

namespace SillyGames.SGBase.Localization
{
	public abstract class LocalizationObject : MonoBehaviour 
    {	
		public abstract System.Type KeyType ();
		public abstract System.Type DataType ();
	}
}
