using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.IO;
using SillyGames.SGBase.Localization;

namespace SillyGames.SGBase.Localization
{
	/// <summary>
	/// Generic localization editor renderer.
	/// Caution :- This class uses reflection to check and replace LocalizationData<T> and LocalizationBase<T,V> values.
	/// Specifically, the List<V> rawData and the string language of LocalizationBase class;
	/// And the string key and T data of the LocalizationData class.
	/// change the values of the private const string fields in this class if the names of the above mentioned properties are changed.
	/// </summary>
	public class GenericLocalizationEditorRenderer<T,V> : IGenericLocalizationEditor where V : LocalizableData<T> 
	{
		/// <summary>
		/// The m new key instance.
		/// Instance that will store any data that a new key may need
		/// </summary>
		private V mNewKeyInstance = null;

		/// <summary>
		/// The group to search as input by the user.
		/// </summary>
		private string mGroupToSearch;

		/// <summary>
		/// The search key as input by the user
		/// </summary>
		private string mSearchKey = "";

		/// <summary>
		/// The search keys boolean to check if keys need be searched.
		/// </summary>
		private bool mSearchKeys = true;

		/// <summary>
		/// The search values boolean to check if values need be searched.
		/// </summary>
		private bool mSearchValues = false;


		/// <summary>
		/// The grouped data, Updated every onInspectorGUI, contains mLb.m_rawData values segregated by their groups.
		/// </summary>
		private Dictionary<string,List<V>> mGroupedData = null;

		/// <summary>
		/// The data to display, Updated every onInspectorGUI, contains grouped data that need be drawn on the custom editor
		/// Note :- modifying this dictionary will not change the rawData in any way, it is merely here for readability purposes.
		/// </summary>
		private Dictionary<string,List<V>> mDataToDisplay = null;

		/// <summary>
		/// The entry selection states. A dictionary grouped by group names that contains the selection states of the entries in that group
		/// Note :- group,key,boolean state in that order
		/// </summary>
		private Dictionary<string,Dictionary<string,bool>> mEntrySelectionStates = null;

		/// <summary>
		/// The group foldout states. A dictionary of group names that contains the foldout states of that group
		/// True if that group has been folded out by the user
		/// </summary>
		private Dictionary<string,bool> mGroupFoldoutStates = null;

		/// <summary>
		/// The group selection states.  A dictionary specifying if the group specified by its name is selected
		/// </summary>
		private Dictionary<string,bool> mGroupSelectionStates = null;


		/// <summary>
		/// Set dirty if you want any changes you made not explicitly to raw data to be recorded in the underlying localizationObject at the end of this update.
		/// calls ReserializeAndRefresh if true
		/// </summary>
		private bool _dirty;
		private bool mDirty{
			get
			{
				return _dirty;
			}
			set
			{
				_dirty = value;
			}
		}


		/// <summary>
		/// Constant strings that are to be updated if the name of the variables of rawData list<V>, language string, key T and data V are ever changed in the code
		/// of the underlying localizationBase object
		/// </summary>
		private const string mLocalizationBaseRawDataFieldName = "m_rawData";
		private const string mLanguageFieldName = "m_language";
		private const string mKeyFieldName = "key";
		private const string mDataFieldName = "data";


		/// <summary>
		/// Helper variables to store the width of the editor screen to help standarding the UI
		/// </summary>
		//private GUILayoutOption mWidth = null;
		private GUILayoutOption mKeyEntryWidth = null;
		private GUILayoutOption mColWidth = null;
		private GUILayoutOption mButtonWidth = null;
		private GUILayoutOption mIndexWidth = null;


		/// <summary>
		/// Variables that contain the underlying object as denoted by their name for which this editor is being drawn
		/// </summary>
		private LocalizationBase<T,V> mLb;
		private Object mTargetObject;
		private Editor mCallingWindow;


		/// <summary>
		/// Helper variables that store values that need be accessed in multiple points in the code
		/// Accurate Usage of the same is programmer dependent in that it needs to be cleared and used as he/she sees fit
		/// </summary>
		//private string mCommonKeyStorageSpace;
		private string mGroupToSwitchToStorageSpace;
		private string mSelectedKey;
		private string mSelectedGroup;
		//private bool mSelectedAnElementThisUpdate;

		/// <summary>
		/// A list containing the types of the different sorter classes
		/// Sorter class must be a subclass of BaseSorter<,> to be detected
		/// </summary>
		private List<System.Type> mSorterTypes;

		/// <summary>
		/// The class which will handle export and import of the localizationData
		/// </summary>
		private GenericLocalizationShipper<T,V> mShipper;

		/// <summary>
		/// The currently selected sorter's index
		/// </summary>
		private int mSelectedSortIndex;

		/// <summary>
		/// The names of the sorters
		/// </summary>
		private string[] mSortOptions;


		/// <summary>
		/// Gets a value indicating whether this
		/// <see cref="SillyGames.SGBase.Localization.GenericLocalizationEditorRenderer`2"/> is searching.
		/// </summary>
		/// <value><c>true</c> if is searching; otherwise, <c>false</c>.</value>
		private bool mIsSearching{
			get
			{
				return((mSearchKey != null && mSearchKey.Length > 0) || (mGroupToSearch != null && mGroupToSearch.Length > 0));
			}
		}


		/// <summary>
		/// Determines whether this instance is subclass of raw generic the specified generic toCheck.
		/// </summary>
		/// <returns><c>true</c> if this instance is subclass of raw generic the specified generic toCheck; otherwise, <c>false</c>.</returns>
		/// <param name="generic">Generic base type.</param>
		/// <param name="toCheck">type to check.</param>
		private bool IsSubclassOfRawGeneric(System.Type generic, System.Type toCheck) {
			while (toCheck != null && toCheck != typeof(object)) {
				var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
				if (generic == cur) {
					return true;
				}
				toCheck = toCheck.BaseType;
			}
			return false;
		}

		/// <summary>
		/// Populates the mSortMethods list.
		/// </summary>
		/// <param name="codelocation">Codelocation.</param>
		private void PopulateSortMethods(string codelocation)
		{
			//sift through assemblies to find subclasses of the BaseSorter<,> generic class
			foreach (Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (System.Type type in assembly.GetTypes())
				{
					if (IsSubclassOfRawGeneric (typeof(BaseSorter<,>),type) && !type.Equals(typeof(BaseSorter<,>)))
					{
						if (mSorterTypes == null)
						{
							mSorterTypes = new List<System.Type> ();
							mSorterTypes.Add (null);
						}
						System.Type[] typeArgs = { typeof(T), typeof(V) };
						System.Type makeme = type.MakeGenericType (typeArgs);
						mSorterTypes.Add (makeme);
					}
				}
			}

			//populate a list of options to display in editor
			if (mSorterTypes != null && mSorterTypes.Count > 0)
			{
				mSortOptions = new string[mSorterTypes.Count];
				for (int i = 0; i < mSorterTypes.Count; ++i)
				{
					if (mSorterTypes [i] != null)
					{
						string name = mSorterTypes [i].Name;
						name = name.Remove (name.Length - 2);
						mSortOptions [i] = name;
					}
				}
			}


		}



		/// <summary>
		/// Initializes the editor.
		/// </summary>
		/// <param name="targetObject">target object/localizationBase object.</param>
		/// <param name="callingWindow">Calling editor class.</param>
		public void InitializeEditor(Object targetObject,Editor callingWindow)
		{

			//initialize state maintainence variables
			mGroupFoldoutStates = new Dictionary<string,bool>();
			mGroupSelectionStates = new Dictionary<string,bool>();
			mEntrySelectionStates = new Dictionary<string,Dictionary<string,bool>>();
			mTargetObject = targetObject;
			if (mTargetObject == null)
			{
				Debug.Log ("null object sent as target, initialization failed");
				return;
			}

			//set base variables
			mLb = mTargetObject as LocalizationBase<T,V>;
			mCallingWindow = callingWindow;

			//initialize shipper
			mShipper = new GenericLocalizationShipper<T, V> ();

			//Populate the sorter methods
			string codeLocation = Application.dataPath;
			string[] allFiles = Directory.GetFiles (codeLocation, "*.cs", SearchOption.AllDirectories);
			string className = this.GetType ().Name;
			className = className.Remove (className.Length - 2, 2);
			for (int i = 0; i < allFiles.Length; ++i)
			{
				if (allFiles [i].Contains (className))
				{
					codeLocation = allFiles [i];
					break;
				}
			}
			codeLocation = Path.GetDirectoryName(codeLocation);
			codeLocation += "/Sorters";
			codeLocation = codeLocation.Replace ("\\", "/");
			codeLocation = codeLocation.Replace ("//", "/");
			PopulateSortMethods (codeLocation);
			mSelectedKey = "";
			mSelectedGroup = "";
			//mSelectedAnElementThisUpdate = false;
		}



		/// <summary>
		/// Reserializes the LocalizationBase object and refreshes editor.
		/// Also registers an undo state before this happens
		/// Called when the mDirty variable is set to true
		/// </summary>
		private void ReserializeAndRefresh()
		{
			if (!Application.isPlaying && mLb != null)
			{
				LocalizationBase<T,V> newBase = mTargetObject as LocalizationBase<T,V>;
				List<V> stuff = mLb.m_rawData == null?null:new List<V> (mLb.m_rawData);
				mCallingWindow.serializedObject.FindProperty (mLocalizationBaseRawDataFieldName).ClearArray ();
				mCallingWindow.serializedObject.FindProperty (mLanguageFieldName).stringValue = mLb.Language;
				mCallingWindow.serializedObject.ApplyModifiedProperties ();
				mCallingWindow.serializedObject.UpdateIfRequiredOrScript();
				Undo.RecordObject (mTargetObject,"LocalizationEditorUndo");
				newBase.m_rawData = stuff;
				newBase.UpdateFromRawData ();
				UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ());
				stuff = null;
				//clear all the residual pointers
				System.GC.Collect (System.GC.MaxGeneration);
			}
			else
			{
				Debug.LogWarning ("Cant serialize data in play mode, value will be lost on play mode end");
			}
		}


		/// <summary>
		/// Renders and processes input for Entries (Keys and Values).
		/// </summary>
		/// <param name="bdIn">Base Dictionary Index.</param>
		/// <param name="entry">Entry.</param>
		private void RenderAndProcessEntryUI(int bdIn,V entry)
		{
			EditorUtils.HAutoLayout(delegate {

				//draw index
				EditorGUILayout.LabelField (bdIn + ": ",mIndexWidth);

				//draw name with option to change it there
				Rect rect = EditorGUILayout.GetControlRect(mKeyEntryWidth);

				if(rect.Contains(Event.current.mousePosition) && !mIsSearching && Event.current.type == EventType.MouseDown)
				{
					mSelectedGroup = entry.group;
					mSelectedKey = entry.key;
					//mSelectedAnElementThisUpdate = true;
				}
				bool isSelectedLabel = mSelectedKey.Length > 0 
					&& mSelectedGroup.Length > 0
					&& mSelectedKey == entry.key 
					&& mSelectedGroup == entry.group;
				
				if(isSelectedLabel)
				{
					string toChangeTo = EditorGUI.TextField(rect,entry.key);
					bool keyExists = mGroupedData[entry.group].Find((x)=>x.key == toChangeTo) != null;
					if(toChangeTo != "" && toChangeTo != entry.key && !keyExists)
					{
						List<V> newList = new List<V>(mLb.m_rawData);
						entry = newList.Find((x)=>x.key.Equals(entry.key) && x.group.Equals(entry.group) && x.data.Equals(entry.data));
						entry.key = toChangeTo;
						mSelectedKey = toChangeTo;
						OverwriteRawData(newList);
						return;
					}
				}
				else
				{
					EditorGUI.SelectableLabel(rect,entry.key);
				}
				//print data type as an Object or a string, depending on if it is a Unity Object or not 
				Object objectToDisp = entry.data as Object;
				object someData = default(object);
				if(entry.data.GetType().IsSubclassOf(typeof(Object)))
				{
					//its an "Object", display as an Object field
					someData = EditorGUILayout.ObjectField(objectToDisp,typeof(T),true,mKeyEntryWidth);
				}
				else
				{
					//its an "object", display as a text field
					string toDisplay = "";
					if(entry.data != null)
					{
						toDisplay = System.Convert.ToString(entry.data);
					}
					someData = (T)System.Convert.ChangeType(EditorGUILayout.TextField(toDisplay,mKeyEntryWidth),typeof(T));
				}

				//if we detect a change, update it using Reflection
				if(!someData.Equals((object)entry.data))
				{
					FieldInfo dataField = entry.GetType().GetField(mDataFieldName);
					dataField.SetValue(entry,someData);
					mDirty = true;
				}

				//process its selection status
				mEntrySelectionStates[entry.group][entry.key] = EditorGUILayout.Toggle(mEntrySelectionStates[entry.group][entry.key],mIndexWidth);

				//check if user wants to delete this entry
				if (GUILayout.Button ("-",mButtonWidth))
				{
					bool del = false;
					//shift delete skips the confirm dialog box
					del = ConfirmUserActionWithShiftOverride("Delete Key","Are you sure you want to delete the key named \""+entry.key+"\"?","Yes","No");
					if(del)
					{
						// remove from the base list if delete poll was successful
						List<V> newList = new List<V>(mLb.m_rawData);
						newList.Remove (entry);
						OverwriteRawData(newList);
					}
				}
				if (GUILayout.Button ("*",mButtonWidth))
				{
					bool del = false;
					//shift delete skips the confirm dialog box
					del = ConfirmUserActionWithShiftOverride("Duplicate Key","Are you sure you want to duplicate the key named \""+entry.key+"\"?","Yes","No");
					if(del)
					{
						// remove from the base list if delete poll was successful
						List<V> newList = new List<V>(mLb.m_rawData);
						string groupName = entry.group;
						List<string> namesToAvoid = new List<string>();
						for(int i = 0; i < newList.Count; ++i)
						{
							if(newList[i].group == groupName)
							{
								namesToAvoid.Add(newList[i].key);
							}
						}
						V newEntry = System.Activator.CreateInstance<V>();
						newEntry.key = GetDuplicateName(entry.key,namesToAvoid);
						newEntry.group = groupName;
						newEntry.data = entry.data;
						int index = newList.IndexOf(entry);
						newList.Insert(index + 1,newEntry);

						OverwriteRawData(newList);
					}
				}

				if(!mIsSearching)
				{
					EditorUtils.HAutoLayout(delegate{
						//Praveen:- Hack yes, but it gets the job done. Horrendous memory consumption though
						//TODO :- refactor the PrepareAndProcessShift groups method
						if(GUILayout.Button("^",mButtonWidth))
						{
							List<V> toSend = new List<V>();
							toSend.Add(entry);
							PrepareAndProcessShiftEntries(1,toSend);	

						}
						if(GUILayout.Button("v",mButtonWidth))
						{
							List<V> toSend = new List<V>();
							toSend.Add(entry);
							PrepareAndProcessShiftEntries(-1,toSend);	

						}
					});
				}
			});
		}


		/// <summary>
		/// Renders and process the UI for a group.
		/// </summary>
		/// <param name="groupIndex">Group index in the mGroupedData dictionary.</param>
		/// <param name="groupName">Group name.</param>
		/// <param name="groupEntries">Group entries.</param>
		private void RenderAndProcessGroupUI(int groupIndex,string groupName,List<V> groupEntries)
		{
			EditorUtils.VAutoLayout (delegate
			{
				EditorUtils.HAutoLayout (delegate
				{
					bool toggle = false;
					string addendum = "";



					//draw the selection toggle only if user is not searching
					if(!mIsSearching)
					{
						toggle = mGroupSelectionStates[groupName];
						toggle = EditorGUILayout.Toggle(toggle,mIndexWidth);
						if(toggle!=mGroupSelectionStates[groupName])
						{
							mGroupSelectionStates[groupName] = toggle;
							ClearSelectionStates(false,true);
						}

						if(GUILayout.Button("-",mButtonWidth))
						{
							string toDisp = "Delete Group";
							bool confirm = ConfirmUserActionWithShiftOverride (toDisp,"Confirm?","Yes","No");
							if(confirm)
							{
								mGroupedData.Remove(groupName);
								OverwriteRawData(mGroupedData);
								return;
							}
						}

						if(GUILayout.Button("*",mButtonWidth))
						{
							string toDisp = "Duplicate Group";
							EditorGUILayout.LabelField(toDisp+":",mColWidth);

							bool confirm = ConfirmUserActionWithShiftOverride (toDisp,"Confirm?","Yes","No");
							if(confirm)
							{
								List<V> newList = new List<V>(mLb.m_rawData);
								List<string> keys = new List<string>(mGroupedData.Keys);
								List<V> toAdd = new List<V>();
								List<V> toCopy =mGroupedData[groupName];
								string groupNameToSet = GetDuplicateName(groupName,keys);
								for(int nIn = 0; nIn < toCopy.Count; ++nIn)
								{
									V newV = System.Activator.CreateInstance<V>();
									newV.key = toCopy[nIn].key;
									newV.data = toCopy[nIn].data;
									newV.group = groupNameToSet;
									toAdd.Add(newV);
								}
								newList.AddRange(toAdd);
								OverwriteRawData(newList);
							}

						}

						//Praveen:- Hack yes, but it gets the job done. Horrendous memory consumption though
						//TODO :- refactor the PrepareAndProcessShift groups method
						if(GUILayout.Button("^",mButtonWidth))
						{
							List<KeyValuePair<string,List<V>>> toSend = new List<KeyValuePair<string, List<V>>>();
							toSend.Add(new KeyValuePair<string,List<V>>(groupName,mGroupedData[groupName]));
							PrepareAndProcessShiftGroups(1,toSend);
						}
						if(GUILayout.Button("v",mButtonWidth))
						{
							List<KeyValuePair<string,List<V>>> toSend = new List<KeyValuePair<string, List<V>>>();
							toSend.Add(new KeyValuePair<string,List<V>>(groupName,mGroupedData[groupName]));
							PrepareAndProcessShiftGroups(-1,toSend);
						}

					}
					else
					{
						//else make sure the name displayes the found entry count
						addendum = " (Entries found: "+mDataToDisplay[groupName].Count+")";
					}



					//draw and update the group selection toggle
					EditorGUI.indentLevel += 1;

					toggle = mGroupFoldoutStates [groupName];

					Rect rect = EditorGUILayout.GetControlRect(mKeyEntryWidth);
					float width = rect.width;
					rect.width = 20;
					toggle = EditorGUI.Foldout (rect,toggle,"");
					rect.x += 20;
					rect.width = width;

					if(rect.Contains(Event.current.mousePosition) && !mIsSearching && Event.current.type == EventType.MouseDown)
					{
						mSelectedGroup = groupName;
						mSelectedKey = "";
						//mSelectedAnElementThisUpdate = true;
						mCallingWindow.Repaint();
					}
					bool isSelectedLabel = mSelectedKey.Length == 0 
						&& mSelectedGroup.Length > 0
						&& mSelectedGroup == groupName;
					
					if(isSelectedLabel)
					{
						string toChangeTo = groupName;
						toChangeTo = EditorGUI.TextField(rect,toChangeTo);
						List<string> names = new List<string>(mGroupedData.Keys);
						if(toChangeTo != "" && toChangeTo != groupName && !names.Contains(toChangeTo))
						{
							mGroupedData = SwapKeyInDictionary(mGroupedData,groupName,toChangeTo);
							groupName = toChangeTo;
							mSelectedGroup = toChangeTo;
							OverwriteRawData(mGroupedData);
							return;
						}
					}
					else
					{
						string foldoutName = groupName + addendum;
						EditorGUI.SelectableLabel(rect,foldoutName);
					}

					//ensure we clear any entry selections when the group is folded in or out
					if(toggle != mGroupFoldoutStates [groupName])
					{
						ClearSelectionStates(false,true);
						mGroupFoldoutStates[groupName] = toggle;
					}
					EditorGUI.indentLevel -= 1;

				});

				if(!mGroupedData.ContainsKey(groupName))
				{
					//the group has been deleted above.... return control
					return;
				}

				//if the group is folded out, render and process the entry UI
				if (mGroupFoldoutStates [groupName])
				{
					GUILayout.Box("",GUILayout.ExpandWidth(true),GUILayout.Height(1));
					EditorUtils.HAutoLayout (delegate
					{
						EditorGUILayout.LabelField ("Keys: " + mDataToDisplay[groupName].Count, mColWidth);
						EditorGUILayout.LabelField ("Values: ", mColWidth);
					});
					GUILayout.Box("",GUILayout.ExpandWidth(true),GUILayout.Height(1));

					for (int bdIn = 0; bdIn < groupEntries.Count; ++bdIn)
					{
						RenderAndProcessEntryUI (bdIn, groupEntries [bdIn]);	
					}
					GUILayout.Box("",GUILayout.ExpandWidth(true),GUILayout.Height(1));
				}
			});
		}


		/// <summary>
		/// Overwrites the raw data.
		/// WARNING :- Changes underlying raw data, use with caution
		/// </summary>
		/// <param name="groupDictionary">Group dictionary.</param>
		private void OverwriteRawData(Dictionary<string,List<V>> groupDictionary)
		{
			List<V> newSortedList = new List<V>();
			List<string> keys = new List<string> (groupDictionary.Keys);
			for (int bdIn = 0; bdIn < keys.Count; ++bdIn)
			{
				List<V> values = groupDictionary [keys [bdIn]];
				for (int i = 0; i < values.Count; ++i)
				{
					values [i].group = keys [bdIn];
				}
				newSortedList.AddRange (values);
			}
			OverwriteRawData (newSortedList);
		}

		/// <summary>
		/// Overwrites the raw data.
		/// WARNING :- Changes underlying raw data, use with caution
		/// </summary>
		/// <param name="groupDictionary">List containing new entries.</param>
		private void OverwriteRawData(List<V> dataToOverwriteWith)
		{
			//ensure to add an undo before messing with the value
			Undo.RecordObject (mTargetObject,"LocalizationEditorUndo");

			//mess with the value
			mLb.m_rawData = dataToOverwriteWith;

			//lock the values in
			ReserializeAndRefresh ();

			//update the local cache
			mGroupedData = GetGroupedData();
			UpdateStateDictionaries(mGroupedData);
			mDataToDisplay = GetDataToDisplay (mGroupedData);

		}


		/// <summary>
		/// Returns the underlying LocalizationBase objects rawData in a grouped dictionary format.
		/// </summary>
		/// <returns>grouped data.</returns>
		private Dictionary<string,List<V>> GetGroupedData()
		{
			List<V> rawData = mLb.m_rawData;
			Dictionary<string,List<V>> groupedData = new Dictionary<string, List<V>>();

			//group them based on their group names
			if (rawData == null)
			{
				return groupedData;
			}
			for (int bdIn = 0; bdIn < rawData.Count; ++bdIn)
			{
				List<V> listToAddTo = null;
				if (rawData [bdIn].group == null || rawData [bdIn].group == "")
				{
					rawData[bdIn].group = "unnamed";
				}
				string groupToCheck = rawData [bdIn].group;
				if (groupedData.TryGetValue (groupToCheck, out listToAddTo))
				{
					listToAddTo.Add (rawData [bdIn]);
				}
				else
				{
					listToAddTo = new List<V> ();
					listToAddTo.Add (rawData [bdIn]);
					groupedData.Add (groupToCheck, listToAddTo);
				}
			}

			//ensure we dont touch the underlying raw data
			rawData = new List<V> ();

			//sort all groups by group name and their current placement within the raw data list
			//Praveen :- kindoff unnecessary but it keeps things clean and readable
			List<string> keys = new List<string> (groupedData.Keys);
			for (int bdIn = 0; bdIn < keys.Count; ++bdIn)
			{
				rawData.AddRange (groupedData [keys [bdIn]]);
			}

			//set the rawdata to the now sorted list
			mLb.m_rawData = rawData;

			//return the pretty dictionary
			return groupedData;
		}


		/// <summary>
		/// Gets the data to display based on search criteria.
		/// </summary>
		/// <returns>The data to display.</returns>
		/// <param name="dataToSiftThrough">Data to sift through to return the required output.</param>
		private Dictionary<string,List<V>> GetDataToDisplay(Dictionary<string,List<V>> dataToSiftThrough)
		{
			if (dataToSiftThrough == null)
			{
				return null;
			}

			//create a new dictionary so we dont accidentally kill any underlying required elements
			Dictionary<string,List<V>> dataToReturn = new Dictionary<string, List<V>>(dataToSiftThrough);
			List<string> keys = new List<string> (dataToReturn.Keys);


			//remove all groups that dont contain the group search key
			bool res = (mGroupToSearch != null && mGroupToSearch.Length > 0);
			if (res)
			{
				for (int i = 0 ; i< keys.Count; ++i)
				{
					string key = keys [i];
					if (!key.ToLower ().Contains (mGroupToSearch))
					{
						dataToReturn.Remove (key);
					}
				}
			}

			//check if there is a search key to process through
			//if found, process the dataToSiftThrough and append any found elements to the dataToReturn
			res = (mSearchKey == null || mSearchKey.Length <= 0);
			if (!res)
			{
				string searchKey = mSearchKey.ToLower();
				bool isObjectData = typeof(V).IsSubclassOf(typeof(Object));

				if((mSearchKeys || mSearchValues) && dataToReturn.Count > 0)
				{
					keys = new List<string> (dataToReturn.Keys);
					for (int bdIn = 0; bdIn< keys.Count; ++bdIn)
					{
						string key = keys [bdIn];
						List<V> newValues = dataToReturn[key].FindAll(delegate(V x){
							if(mSearchKeys)
							{
								if(x.key.ToLower().Contains(searchKey))
								{
									return true;
								}
							}
							if(mSearchValues)
							{
								string stringData = "";
								if(isObjectData)
								{
									stringData = ((Object)(System.Convert.ChangeType(x.data,typeof(Object)))).name;
								}
								else
								{
									stringData = x.data.ToString();
								}
								stringData = stringData.ToLower();
								if(x.data != null && stringData!= null)
								{
									if(stringData.Contains(searchKey))
									{
										return true;
									}
								}
							}
							return false;
						});
						dataToReturn [key] = newValues;
					}

					//remove empty groups
					for (int i = 0; i < keys.Count; ++i)
					{
						string key = keys [i];
						if (dataToReturn [key].Count == 0)
						{
							dataToReturn.Remove (key);
						}
					}
				}
			}

			//return the pretty dictionary
			return dataToReturn;
		}


		/// <summary>
		/// Renders and processes the sort UI.
		/// </summary>
		private void RenderAndProcessSortUI()
		{
			if (mLb.m_rawData != null && mLb.m_rawData.Count > 0 && mSorterTypes != null)
			{
				EditorUtils.VAutoLayout (delegate
				{
					//display sort options detected
					EditorUtils.HAutoLayout(delegate {
						EditorGUILayout.LabelField("Sort Method:",mColWidth);
						mSelectedSortIndex = EditorGUILayout.Popup(mSelectedSortIndex,mSortOptions,mColWidth);
						if(mSelectedSortIndex == 0)
						{
							return;
						}
					});

					//process user input and call the required function as necessary
					if(mSelectedSortIndex != 0)
					{
						EditorUtils.HAutoLayout(delegate {
							string toAsk = "Sorting will change the current order of the {0}, You will lose custom shifted positions of selected {0} if any, or all the {0} if none are selected, Confirm sort?";
							short toSort = -1;
							bool sort = false;
							if(GUILayout.Button("SortGroups"))
							{
								sort = ConfirmUserActionWithShiftOverride("Sort Groups?",string.Format(toAsk,"groups"),"Yes","No");
								if(!sort)
								{
									return;
								}
								toSort = 1;
							}
							if(GUILayout.Button("SortKeys"))
							{
								sort = ConfirmUserActionWithShiftOverride("Sort Groups?",string.Format(toAsk,"keys and entries"),"Yes","No");
								if(!sort)
								{
									return;
								}
								toSort = 2;
							}
							if(GUILayout.Button("SortEntries"))
							{
								sort = ConfirmUserActionWithShiftOverride("Sort Groups?",string.Format(toAsk,"keys and entries"),"Yes","No");
								if(!sort)
								{
									return;
								}
								toSort = 3;
							}
							if(!sort)
							{
								return;
							}
							ISorter<V> sorter = (ISorter<V>)System.Activator.CreateInstance(mSorterTypes[mSelectedSortIndex]);
							object[] parameters = new object[1];
							parameters [0] = (object)mLb.m_rawData;
							switch(toSort)
							{
							case 1:
							{
								bool atleastOneGroupSelected = false;
								List<V> listToSort = new List<V>();
								foreach(KeyValuePair<string,List<V>> pair in mGroupedData)
								{
									if(mGroupSelectionStates[pair.Key])
									{
										listToSort.AddRange(pair.Value);
										atleastOneGroupSelected = true;
									}
								}
								if(listToSort.Count <= 0)
								{
									listToSort = mLb.m_rawData;
								}

								listToSort = sorter.SortGroups (listToSort);
								if(atleastOneGroupSelected)
								{
									List<KeyValuePair<string,List<V>>> groupedList = sorter.GetGroups(listToSort);
									List<V> newList = new List<V>();
									int index = 0;
									foreach(KeyValuePair<string,List<V>> pair in mGroupedData)
									{
										if(mGroupSelectionStates[pair.Key])
										{
											newList.AddRange(groupedList[index].Value);
											++index;
										}
										else
										{
											newList.AddRange(pair.Value);
										}

									}
									listToSort = newList;
								}
								OverwriteRawData(listToSort);
							}
							break;
							case 2:
							{
								bool atleastOneGroupSelected = false;
								bool atleastOneEntrySelected = false;
								List<V> listToSort = new List<V>();
								foreach(KeyValuePair<string,List<V>> pair in mGroupedData)
								{
									if(mGroupSelectionStates[pair.Key])
									{
										listToSort.AddRange(pair.Value);
										atleastOneGroupSelected = true;
									}

								}

								if(!atleastOneGroupSelected)
								{
									foreach(KeyValuePair<string,List<V>> pair in mGroupedData)
									{
										int index = 0;
										foreach(KeyValuePair<string,bool> entryPair in mEntrySelectionStates[pair.Key])
										{
											if(entryPair.Value)
											{
												listToSort.Add(pair.Value[index]);
												atleastOneEntrySelected = true;
											}
											++index;
										}
									}
								}

								if(listToSort.Count <= 0)
								{
									listToSort = mLb.m_rawData;
								}

								listToSort = sorter.SortKeys (listToSort);
								if(atleastOneGroupSelected)
								{
									List<KeyValuePair<string,List<V>>> groupedList = sorter.GetGroups(listToSort);
									List<V> newList = new List<V>();
									int index = 0;
									foreach(KeyValuePair<string,List<V>> pair in mGroupedData)
									{
										if(mGroupSelectionStates[pair.Key])
										{
											newList.AddRange(groupedList[index].Value);
											++index;
										}
										else
										{
											newList.AddRange(pair.Value);
										}

									}
									listToSort = newList;
								}
								if(atleastOneEntrySelected)
								{
									List<V> newList = new List<V>();
									int sortIndex = 0;
									foreach(KeyValuePair<string,List<V>> pair in mGroupedData)
									{
										int index = 0;
										foreach(KeyValuePair<string,bool> entryPair in mEntrySelectionStates[pair.Key])
										{
											if(entryPair.Value)
											{
												newList.Add(listToSort[sortIndex]);
												++sortIndex;
											}
											else
											{
												newList.Add(pair.Value[index]);
											}
											++index;
										}
									}
									listToSort = newList;
								}
								OverwriteRawData(listToSort);
							}
							break;
							case 3:
							{
								bool atleastOneGroupSelected = false;
								bool atleastOneEntrySelected = false;
								List<V> listToSort = new List<V>();
								foreach(KeyValuePair<string,List<V>> pair in mGroupedData)
								{
									if(mGroupSelectionStates[pair.Key])
									{
										listToSort.AddRange(pair.Value);
										atleastOneGroupSelected = true;
									}
								}
								if(!atleastOneGroupSelected)
								{
									foreach(KeyValuePair<string,List<V>> pair in mGroupedData)
									{
										int index = 0;
										foreach(KeyValuePair<string,bool> entryPair in mEntrySelectionStates[pair.Key])
										{
											if(entryPair.Value)
											{
												listToSort.Add(pair.Value[index]);
												atleastOneEntrySelected = true;
											}
											++index;
										}
									}
								}

								if(listToSort.Count <= 0)
								{
									listToSort = mLb.m_rawData;
								}

								listToSort = sorter.SortEntries (listToSort);
								if(atleastOneGroupSelected)
								{
									List<KeyValuePair<string,List<V>>> groupedList = sorter.GetGroups(listToSort);
									List<V> newList = new List<V>();
									int index = 0;
									foreach(KeyValuePair<string,List<V>> pair in mGroupedData)
									{
										if(mGroupSelectionStates[pair.Key])
										{
											newList.AddRange(groupedList[index].Value);
											++index;
										}
										else
										{
											newList.AddRange(pair.Value);
										}

									}
									listToSort = newList;
								}
								if(atleastOneEntrySelected)
								{
									List<V> newList = new List<V>();
									int sortIndex = 0;
									foreach(KeyValuePair<string,List<V>> pair in mGroupedData)
									{
										int index = 0;
										foreach(KeyValuePair<string,bool> entryPair in mEntrySelectionStates[pair.Key])
										{

											if(entryPair.Value)
											{
												newList.Add(listToSort[sortIndex]);
												++sortIndex;
											}
											else
											{
												newList.Add(pair.Value[index]);
											}
											++index;
										}
									}
									listToSort = newList;
								}
								OverwriteRawData(listToSort);

								//mLb.m_rawData = sorter.SortEntries (mLb.m_rawData);
							}
							break;
							}
							OverwriteRawData(mLb.m_rawData);
							mDirty = true;

						});
					}
					else
					{
						if(mSortOptions.Length>0)
						{
							EditorGUILayout.LabelField("Select a sort method above to enable sorting");
						}
						else
						{
							EditorGUILayout.LabelField("No sort methods found in this project, please create one and select it above to enable sorting");
						}
					}
				});

			}
		}



		/// <summary>
		/// Renders and processes the search UI.
		/// Fills in the required variables based on user input for the rest of the code to use
		/// </summary>
		private void RenderAndProcessSearchUI()
		{
			if(mLb.m_rawData != null && mLb.m_rawData.Count > 0)
			{
				//group to search for

				EditorUtils.HAutoLayout(delegate {
					EditorGUILayout.LabelField ("Group Name :",mColWidth);
					string group = mGroupToSearch;
					group = EditorGUILayout.TextField (mGroupToSearch,mColWidth);
					mGroupToSearch = group;
					EditorGUILayout.LabelField ("",mButtonWidth);
				});


				//key/value to search for
				EditorUtils.HAutoLayout(delegate {
					EditorGUILayout.LabelField ("Search :",mColWidth);
					string search = mSearchKey;
					search = EditorGUILayout.TextField (search,mColWidth);
					mSearchKey = search;
					EditorGUILayout.LabelField ("",mButtonWidth);
				});

				//toggles to denote search scope
				EditorUtils.HAutoLayout (delegate
				{
					EditorGUILayout.LabelField ("Search For :", mColWidth);
					EditorUtils.HAutoLayout (delegate
					{
						mSearchKeys = GUILayout.Toggle (mSearchKeys, "Keys", mColWidth);
						mSearchValues = GUILayout.Toggle (mSearchValues, "Values", mColWidth);
					});

				});
			}
		}


		/// <summary>
		/// Renders and processes the add new key UI.
		/// </summary>
		private void RenderAndProcessAddKeyUI()
		{
			//dont render if searching
			if (mIsSearching)
			{
				return;
			}

			EditorUtils.VAutoLayout (delegate
			{
				string logString = "";
				if (mNewKeyInstance == null)
				{
					mNewKeyInstance = (V)System.Activator.CreateInstance (typeof(V));
					mNewKeyInstance.key = "";
					mNewKeyInstance.data = default(T);
				}
				bool res = false;

				EditorGUILayout.LabelField("Add New Key");
				EditorUtils.VAutoLayout (delegate
				{
					List<V> values = null;
					EditorUtils.HAutoLayout(delegate {
						EditorGUILayout.LabelField("Group:",mColWidth);
						mNewKeyInstance.group = EditorGUILayout.TextField (mNewKeyInstance.group,mColWidth);
						res = mNewKeyInstance.group != null && mNewKeyInstance.group.Length > 0;
						if(!res)
						{
							logString = "Please enter a valid group name, null group is reserved for un-updated localization editors only";
							return;
						}
						if(mGroupedData.ContainsKey(mNewKeyInstance.group))
						{
							values = mGroupedData[mNewKeyInstance.group];
						}
					});

					if(res)
					{
						EditorUtils.HAutoLayout(delegate {
							EditorGUILayout.LabelField("Key:",mColWidth);
							mNewKeyInstance.key = EditorGUILayout.TextField (mNewKeyInstance.key,mColWidth);
							res = mNewKeyInstance.key != null && mNewKeyInstance.key.Length > 0;
							if(!res)
							{
								logString = "Please enter a valid key";
								return;
							}
							if(values != null)
							{
								res = values == null || values.Find((x)=>x.key == mNewKeyInstance.key && x.group == mNewKeyInstance.group) == null;
							}
							else
							{
								res = values == null || values.Find((x)=>x.key == mNewKeyInstance.key) == null;
							}
							if(!res)
							{
								logString = "Specified key already exists in the selected group";
								return;
							}
						});
					}
					if(res)
					{
						EditorUtils.HAutoLayout(delegate {
							EditorGUILayout.LabelField("Value:",mColWidth);

							Object objectToDisp = mNewKeyInstance.data as Object;
							System.Reflection.FieldInfo info = typeof(V).GetField(mDataFieldName);

							if(info.FieldType.IsSubclassOf(typeof(Object)))
							{
								Object ob = EditorGUILayout.ObjectField(objectToDisp,typeof(T),true,mColWidth);
								if(ob != null)
								{
									FieldInfo field = mNewKeyInstance.GetType().GetField(mDataFieldName);
									if(field != null)
									{
										field.SetValue(mNewKeyInstance,ob);
									}
								}
							}
							else
							{
								string toDisplay = "";
								if(mNewKeyInstance.data != null)
								{
									toDisplay = System.Convert.ToString(mNewKeyInstance.data);
								}
								mNewKeyInstance.data = (T)System.Convert.ChangeType(GUILayout.TextField(toDisplay,mColWidth),typeof(T));
							}

							res = mNewKeyInstance.data != null ;
							if (!res)
							{
								logString = "Enter valid data to set as a value";
								return;
							}

							if (GUILayout.Button ("+",mButtonWidth))
							{
								V newData = (V)System.Activator.CreateInstance (typeof(V));
								newData.group = mNewKeyInstance.group;
								newData.key = mNewKeyInstance.key;
								newData.data = mNewKeyInstance.data;
								if(mLb.m_rawData == null)
								{
									mLb.m_rawData = new List<V>();
								}
								mLb.m_rawData.Add (newData);
								newData = null;
								mNewKeyInstance.key = null;
								mNewKeyInstance.data = default(T);
								mNewKeyInstance.group = "";
								mNewKeyInstance = null;
								mDirty = true;
							}

						});
					}

				});


				EditorGUILayout.LabelField(logString);
			});
		}



		/// <summary>
		/// Swaps the key in a given dictionary.
		/// </summary>
		/// <param name="baseDictionary">Base dictionary.</param>
		/// <param name="currentKey">Current key.</param>
		/// <param name="keyToSet">Key to set.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <typeparam name="V">The 2nd type parameter.</typeparam>
		private Dictionary<T1,V1> SwapKeyInDictionary<T1,V1>(Dictionary<T1,V1> baseDictionary, T1 currentKey,T1 keyToSet)
		{
			if (baseDictionary == null || baseDictionary.Count <= 0 || !baseDictionary.ContainsKey (currentKey) || currentKey.Equals (keyToSet))
			{
				return baseDictionary;
			}
			//

			//find location
			int index = 0;
			int indexToSet = 0;
			foreach (KeyValuePair<T1,V1> pair in baseDictionary)
			{
				if (pair.Key.Equals(currentKey))
				{
					indexToSet = index;
					break;
				}
				++index;
			}

			//reorder
			index = 0;
			Dictionary<T1,V1> newDic = new Dictionary<T1, V1> ();
			foreach (KeyValuePair<T1,V1> pair in baseDictionary)
			{
				if (index == indexToSet)
				{
					newDic.Add (keyToSet, baseDictionary [currentKey]);
				}
				else
				{
					newDic.Add (pair.Key,pair.Value);
				}
				++index;
			}

			return newDic;
		}


		/// <summary>
		/// Updates the group state maintanence dictionaries.
		/// </summary>
		/// <param name="oldName">Old name.</param>
		/// <param name="newName">New name.</param>
//		private void UpdateGroupStateDictionaries(string oldName,string newName)
//		{
//			List<string> keys = new List<string>(mGroupedData.Keys);
//			if (mGroupFoldoutStates.ContainsKey (oldName))
//			{
//				bool state = mGroupFoldoutStates [oldName];
//				mGroupFoldoutStates.Remove (oldName);
//				mGroupFoldoutStates.Add (newName, state);
//			}
//
//			if (mGroupSelectionStates.ContainsKey (oldName))
//			{
//				bool state = mGroupSelectionStates [oldName];
//				mGroupSelectionStates.Remove (oldName);
//				mGroupSelectionStates.Add (newName, state);
//			}
//
//			if (mEntrySelectionStates.ContainsKey (oldName))
//			{
//				Dictionary<string,bool> selectionStates = mEntrySelectionStates [oldName];
//				mEntrySelectionStates.Remove (oldName);
//				mEntrySelectionStates.Add (newName, selectionStates);
//			}
//
//
//		}
			
		/// <summary>
		/// Renders and processes the add key to group UI.
		/// </summary>
		/// <param name="group">Group.</param>
		private void RenderAndProcessAddKeyToGroupUI (KeyValuePair<string,List<V>> group)
		{
			EditorUtils.VAutoLayout (delegate
			{
				string logString = "";
				if (mNewKeyInstance == null)
				{
					mNewKeyInstance = (V)System.Activator.CreateInstance (typeof(V));
					mNewKeyInstance.key = "";
					mNewKeyInstance.data = default(T);
				}
				bool res = false;

				EditorGUILayout.LabelField("Add New Key to "+group.Key+":");
				EditorUtils.VAutoLayout (delegate
				{

					EditorUtils.HAutoLayout(delegate {
						EditorGUILayout.LabelField("Key:",mColWidth);
						mNewKeyInstance.key = EditorGUILayout.TextField (mNewKeyInstance.key,mColWidth);
						res = mNewKeyInstance.key != null && mNewKeyInstance.key.Length > 0;
						if(!res)
						{
							logString = "Please enter a valid key";
							return;
						}
						res = group.Value == null || group.Value.Find((x)=>x.key == mNewKeyInstance.key) == null;
						if(!res)
						{
							logString = "Specified key already exists in the selected group";
							return;
						}
					});

					if(res)
					{
						EditorUtils.HAutoLayout(delegate {
							EditorGUILayout.LabelField("Value:",mColWidth);

							Object objectToDisp = mNewKeyInstance.data as Object;
							System.Reflection.FieldInfo info = typeof(V).GetField(mDataFieldName);

							if(info.FieldType.IsSubclassOf(typeof(Object)))
							{
								Object ob = EditorGUILayout.ObjectField(objectToDisp,typeof(T),true,mColWidth);
								if(ob != null)
								{
									FieldInfo field = mNewKeyInstance.GetType().GetField(mDataFieldName);
									if(field != null)
									{
										field.SetValue(mNewKeyInstance,ob);
									}
								}
							}
							else
							{
								//its an "object", display as a text field
								string toDisplay = "";
								if(mNewKeyInstance.data != null)
								{
									toDisplay = System.Convert.ToString(mNewKeyInstance.data);
								}
								mNewKeyInstance.data = (T)System.Convert.ChangeType(GUILayout.TextField(toDisplay,mColWidth),typeof(T));
							}

							res = mNewKeyInstance.data != null ;
							if (!res)
							{
								logString = "Enter valid data to set as a value";
								return;
							}

							if (GUILayout.Button ("+",mButtonWidth))
							{
								V newData = (V)System.Activator.CreateInstance (typeof(V));
								newData.group = group.Key;
								newData.key = mNewKeyInstance.key;
								newData.data = mNewKeyInstance.data;
								if(mLb.m_rawData == null)
								{
									mLb.m_rawData = new List<V>();
								}
								mLb.m_rawData.Add (newData);
								newData = null;
								mNewKeyInstance.key = null;
								mNewKeyInstance.data = default(T);
								mNewKeyInstance.group = "";
								mNewKeyInstance = null;
								mDirty = true;
							}

						});
					}

				});


				EditorGUILayout.LabelField(logString);
			});
		}


		/// <summary>
		/// Shifts any value that occurs within a list.
		/// </summary>
		/// <returns>The within list.</returns>
		/// <param name="direction">Direction.</param>
		/// <param name="baseList">Base list.</param>
		/// <param name="elementsToShift">Elements to shift.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		private List<T1> ShiftWithinList<T1>(int direction, List<T1> baseList,List<T1> elementsToShift)
		{
			List<T1> toReturn = new List<T1> (baseList);
			int increment = direction == 1?1:-1;
			for (int etsIn = (direction==1?0:elementsToShift.Count-1); direction == 1?( etsIn < elementsToShift.Count):etsIn >= 0;etsIn += increment)
			{
				T1 elementKey = elementsToShift [etsIn];
				if (baseList.Contains (elementKey))
				{
					int curIn = baseList.IndexOf (elementKey);
					int inToBeAt = curIn + Mathf.Clamp ((direction * -1), -1, 1);
					inToBeAt = Mathf.Clamp (inToBeAt, 0, baseList.Count-1);
					//only shift to a spot that is not currently selected
					if(curIn != inToBeAt)
					{
						if (!elementsToShift.Contains (baseList [inToBeAt]))
						{
							toReturn = new List<T1> ();
							for (int bdIn = 0; bdIn < baseList.Count; ++bdIn)
							{
								if (bdIn != curIn && bdIn != inToBeAt)
								{
									toReturn.Add (baseList [bdIn]);
								}
								else
								{
									if (bdIn == curIn)
									{
										toReturn.Add (baseList [inToBeAt]);
									}
									else
									{
										toReturn.Add (baseList [curIn]);
									}
								}
							}
							baseList = toReturn;
						}
					}
				}
			}
			return toReturn;
		}


		/// <summary>
		/// Prepares and processes the shifting of entries within the rawData.
		/// </summary>
		/// <param name="direction">Direction.</param>
		/// <param name="selectedEntries">Selected entries.</param>
		private void PrepareAndProcessShiftEntries(int direction,List<V> selectedEntries)
		{
			List<string> groupNames = new List<string> ();
			for (int i = 0; i < selectedEntries.Count; ++i)
			{
				if(!groupNames.Contains(selectedEntries[i].group))
				{
					groupNames.Add(selectedEntries[i].group);
				}
			}
			for (int i = 0; i < groupNames.Count; ++i)
			{
				Dictionary<string,List<V>> baseData = mGroupedData;
				List<V> baseList = baseData [groupNames [i]];
				List<V> elementsToShift = selectedEntries.FindAll ((x) => x.group == groupNames [i]);
				baseList = ShiftWithinList (direction, baseList, elementsToShift);
				baseData [groupNames [i]] = baseList;
				OverwriteRawData (baseData);
			}
		}

		private bool ConfirmUserActionWithShiftOverride(string dialogTitle,string dialogMessage,string yes,string no)
		{
			if (Event.current.shift)
			{
				return true;
			}
			return EditorUtility.DisplayDialog (dialogTitle, dialogMessage, yes, no);
		}
		/// <summary>
		/// Shifts keys within a dictionary.
		/// </summary>
		/// <returns>The within dictionary.</returns>
		/// <param name="direction">Direction.</param>
		/// <param name="baseDictionary">Base dictionary.</param>
		/// <param name="elementsToShift">Elements to shift.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <typeparam name="V">The 2nd type parameter.</typeparam>
		private Dictionary<T1,V1> ShiftWithinDictionary<T1,V1>(int direction, Dictionary<T1,V1> baseDictionary,Dictionary<T1,V1> elementsToShift)
		{
			Dictionary<T1,V1> toReturn = new Dictionary<T1, V1> (baseDictionary);
			List<T1> keys = new List<T1>(baseDictionary.Keys);

			List<T1> elementsKeys = new List<T1> (elementsToShift.Keys);

			int increment = direction == 1?1:-1;
			for (int etsIn = (direction==1?0:elementsKeys.Count-1); direction == 1?( etsIn < elementsKeys.Count):etsIn >= 0;etsIn += increment)
			{
				T1 elementKey = elementsKeys [etsIn];
				if (keys.Contains (elementKey))
				{
					int curIn = keys.IndexOf (elementKey);
					int inToBeAt = curIn + Mathf.Clamp ((direction * -1), -1, 1);
					inToBeAt = Mathf.Clamp (inToBeAt, 0, baseDictionary.Count-1);
					//only shift to a spot that is not currently selected
					if(curIn != inToBeAt)
					{
						if (!elementsToShift.ContainsKey (keys [inToBeAt]))
						{
							toReturn = new Dictionary<T1, V1> ();
							for (int bdIn = 0; bdIn < baseDictionary.Count; ++bdIn)
							{
								if (bdIn != curIn && bdIn != inToBeAt)
								{
									toReturn.Add (keys [bdIn], baseDictionary [keys [bdIn]]);
								}
								else
								{
									if (bdIn == curIn)
									{
										toReturn.Add (keys [inToBeAt], baseDictionary [keys [inToBeAt]]);
									}
									else
									{
										toReturn.Add (keys [curIn], baseDictionary [keys [curIn]]);
									}
								}
							}

							keys = new List<T1> (toReturn.Keys);
						}
					}
				}
			}

			return toReturn;
		}

		/// <summary>
		/// Prepares and processes the shifting of groups.
		/// </summary>
		/// <param name="direction">Direction.</param>
		/// <param name="selectedGroups">Selected groups.</param>
		private void PrepareAndProcessShiftGroups(int direction,List<KeyValuePair<string,List<V>>> selectedGroups )
		{
			Dictionary<string,List<V>> selectedGroupDictionary = new Dictionary<string, List<V>>();
			for(int bdIn = 0 ; bdIn< selectedGroups.Count; ++bdIn)
			{
				selectedGroupDictionary.Add(selectedGroups[bdIn].Key,selectedGroups[bdIn].Value);
			}

			//shift
			mGroupedData = ShiftWithinDictionary<string,List<V>>(direction,mGroupedData,selectedGroupDictionary);

			//update raw data
			OverwriteRawData (mGroupedData);

			//clear any local variables
			selectedGroupDictionary.Clear();
			selectedGroupDictionary = null;
			mDirty = true;
		}

		/// <summary>
		/// Gets the name of a duplicate for a Key.
		/// </summary>
		/// <returns>The duplicate name.</returns>
		/// <param name="originalKeyName">Original key name.</param>
		/// <param name="groupName">Group name.</param>
		private string GetDuplicateName(string originalKeyName,string groupName,List<string> additionalNamesToAvoid = null)
		{
			List<string> namesToConsider = new List<string> ();
			List<V> data = mLb.m_rawData.FindAll ((x) => x.group == groupName);
			for (int i = 0; i < data.Count; ++i)
			{
				if (!namesToConsider.Contains (data[i].key))
				{
					namesToConsider.Add (data[i].key);
				}
			}
			if (additionalNamesToAvoid != null)
			{
				namesToConsider.AddRange (additionalNamesToAvoid);
			}
			return GetDuplicateNameInternal (originalKeyName, namesToConsider);
		}


		/// <summary>
		/// Gets the name of the duplicate of a group.
		/// </summary>
		/// <returns>The duplicate name.</returns>
		/// <param name="originalGroupName">Original group name.</param>
		private string GetDuplicateName(string originalGroupName,List<string> additionalNamesToAvoid = null)
		{
			List<string> namesToConsider = new List<string> ();

			for (int i = 0; i < mLb.m_rawData.Count; ++i)
			{
				if (!namesToConsider.Contains (mLb.m_rawData [i].group))
				{
					namesToConsider.Add (mLb.m_rawData [i].group);
				}
			}
			if (additionalNamesToAvoid != null)
			{
				namesToConsider.AddRange (additionalNamesToAvoid);
			}
			return GetDuplicateNameInternal (originalGroupName, namesToConsider);

		}


		private string GetDuplicateNameInternal(string originalName,List<string> namesToConsider)
		{
			if (namesToConsider != null && originalName != null)
			{
				do
				{
					originalName += "_Duplicate";
				}
				while(namesToConsider.Contains (originalName));
			}
			return originalName;
		}


		/// <summary>
		/// Renders and process a UI common to one group selected and multiple groups selected.
		/// </summary>
		/// <param name="selectedGroups">Selected groups.</param>
		private void RenderAndProcessSelectedGroupsUI(List<KeyValuePair<string,List<V>>> selectedGroups)
		{
			if(selectedGroups.Count < 2)
			{
				return;
			}
			//Actions UI
			EditorUtils.VAutoLayout (delegate
			{
				bool plural = +selectedGroups.Count>1;
				if(mIsSearching)
				{
					EditorGUILayout.LabelField("Shift Groups not possible while searching, Clear search entries to shift",mColWidth);
				}
				else
				{


					EditorUtils.HAutoLayout(delegate{
						EditorGUILayout.LabelField("Shift Group"+(plural?"s":"")+" Up:",mColWidth);
						if(GUILayout.Button("^",mColWidth))
						{
							PrepareAndProcessShiftGroups(1,selectedGroups);
						}
					});

					EditorUtils.HAutoLayout(delegate{
						EditorGUILayout.LabelField("Shift Group"+(plural?"s":"")+" Down:",mColWidth);
						if(GUILayout.Button("v",mColWidth))
						{
							PrepareAndProcessShiftGroups(-1,selectedGroups);
						}
					});
				}

				EditorUtils.HAutoLayout(delegate{
					string toDisp = "Duplicate Group"+(plural?"s":"");
					EditorGUILayout.LabelField(toDisp+":",mColWidth);
					if(GUILayout.Button("*",mColWidth))
					{
						bool confirm = ConfirmUserActionWithShiftOverride (toDisp,"Confirm?","Yes","No");
						if(confirm)
						{
							List<V> newList = new List<V>(mLb.m_rawData);
							List<string> newNames = new List<string>();
							for(int sgIn = 0; sgIn<selectedGroups.Count; ++sgIn)
							{
								string groupName = GetDuplicateName(selectedGroups[sgIn].Key,newNames);
								if(groupName != selectedGroups[sgIn].Key)
									newNames.Add(groupName);
								
								List<V> toAdd = new List<V>();
								List<V> toCopy =selectedGroups[sgIn].Value;
								for(int nIn = 0; nIn < toCopy.Count; ++nIn)
								{
									V newV = System.Activator.CreateInstance<V>();
									newV.key = toCopy[nIn].key;
									newV.data = toCopy[nIn].data;
									newV.group = groupName;
									toAdd.Add(newV);
								}
								newList.AddRange(toAdd);
							}
							OverwriteRawData(newList);
							mDirty = true;
						}
					}
				});

				EditorUtils.HAutoLayout(delegate{
					string toDisp = "Delete Group"+(plural?"s":"");
					EditorGUILayout.LabelField(toDisp+":",mColWidth);
					if(GUILayout.Button("-",mColWidth))
					{
						bool confirm = ConfirmUserActionWithShiftOverride (toDisp,"Confirm?","Yes","No");
						if(confirm)
						{
							for(int bdIn = 0; bdIn<selectedGroups.Count; ++bdIn)
							{
								mGroupedData.Remove(selectedGroups[bdIn].Key);
							}
							OverwriteRawData(mGroupedData);
						}
					}
				});

			});
		}


		/// <summary>
		/// Clears the selection states within maintainence dictionaries.
		/// </summary>
		/// <param name="groups">If set to <c>true</c> groups.</param>
		/// <param name="entries">If set to <c>true</c> entries.</param>
		private void ClearSelectionStates(bool groups = true,bool entries = true)
		{
			List<string> keys;
			if (groups)
			{
				keys = new List<string> (mGroupSelectionStates.Keys);
				for (int i = 0; i < keys.Count; ++i)
				{
					mGroupSelectionStates [keys [i]] = false;
				}
				keys.Clear ();
				keys = null;
			}

			if (entries)
			{
				keys = new List<string> (mEntrySelectionStates.Keys);
				for (int i = 0; i < keys.Count; ++i)
				{
					List<string> innerKeys = new List<string> (mEntrySelectionStates [keys [i]].Keys);
					for (int j = 0; j < innerKeys.Count; ++j)
					{
						mEntrySelectionStates [keys [i]] [innerKeys [j]] = false;
					}
				}
			}
		}

		/// <summary>
		/// Clears the group foldout states.
		/// </summary>
		private void ClearGroupFoldoutStates()
		{
			List<string> keys;
			keys = new List<string> (mGroupFoldoutStates.Keys);
			for (int i = 0; i < keys.Count; ++i)
			{
				mGroupFoldoutStates [keys [i]] = false;
			}
			keys.Clear ();
			keys = null;
		}


		/// <summary>
		/// Renders and processes entries selected UI.
		/// </summary>
		/// <param name="selectedEntries">Selected entries.</param>
		private void RenderAndProcessEntriesSelectedUI(List<V> selectedEntries)
		{
			if (selectedEntries == null || selectedEntries.Count < 2d)
			{
				return;
			}
			EditorUtils.VAutoLayout (delegate
			{
				
				if(!mIsSearching)
				{
					EditorUtils.HAutoLayout(delegate {
						EditorGUILayout.LabelField("Shift Up:",mColWidth);
						if(GUILayout.Button("^",mColWidth))
						{
							PrepareAndProcessShiftEntries(1,selectedEntries);	
						}
					});


					EditorUtils.HAutoLayout(delegate {
						EditorGUILayout.LabelField("Shift Down:",mColWidth);
						if(GUILayout.Button("v",mColWidth))
						{
							PrepareAndProcessShiftEntries(-1,selectedEntries);	
						}
					});
				}
				else
				{
					EditorGUILayout.LabelField("Shifting entries not possible while searching:",mColWidth);
				}



				EditorUtils.HAutoLayout(delegate {
					EditorGUILayout.LabelField("Duplicate:",mColWidth);
					if(GUILayout.Button("*",mColWidth))
					{
						bool confirm = ConfirmUserActionWithShiftOverride ("Duplicate","Confirm?","Yes","No");
						if(confirm)
						{
							List<V> newList = new List<V>(mLb.m_rawData);
							List<string> newNames = new List<string>();
							for(int i = 0 ; i < selectedEntries.Count; ++i)
							{
								V newEntry = System.Activator.CreateInstance<V>();
								string nameToSet = GetDuplicateName(selectedEntries[i].key,selectedEntries[i].group,newNames);
								if(nameToSet != selectedEntries[i].key)
									newNames.Add(nameToSet);
								
								newEntry.key = nameToSet;
								newEntry.group = selectedEntries[i].group;
								newEntry.data = selectedEntries[i].data;
								newList.Add(newEntry);
							}
							OverwriteRawData(newList);
						}
					}
				});

				EditorUtils.HAutoLayout(delegate {
					EditorGUILayout.LabelField("Delete:",mColWidth);
					if(GUILayout.Button("-",mColWidth))
					{
						bool confirm = ConfirmUserActionWithShiftOverride ("Delete","Confirm?","Yes","No");
						if(confirm)
						{
							List<V> newList = new List<V>(mLb.m_rawData);
							newList.RemoveAll((x)=>selectedEntries.Contains(x));
							OverwriteRawData(newList);
						}
					}
				});

				EditorUtils.HAutoLayout(delegate {
					EditorGUILayout.LabelField("Shift Group:",mColWidth);
					mGroupToSwitchToStorageSpace = EditorGUILayout.TextField(mGroupToSwitchToStorageSpace,mColWidth);

				});
				if(mGroupToSwitchToStorageSpace != null && mGroupToSwitchToStorageSpace.Length > 0)
				{
					if(GUILayout.Button("ShiftToGroup"))	
					{
						bool res = ConfirmUserActionWithShiftOverride("Shift Entries?","Confirm selected entries shift to "+mGroupToSwitchToStorageSpace+"?\n" +
							"If the group exists, non unique keys will remain in this group,\n" +
							"If the group does not exists, it will be created for you","Yes","No");
						if(res)
						{
							for(int i = 0 ; i < selectedEntries.Count ; ++i)
							{
								List<V> vals = null;
								mGroupedData.TryGetValue(mGroupToSwitchToStorageSpace,out vals);
								if(vals != null)
								{
									if(vals.Find((x)=>x.key.Equals(selectedEntries[i].key)) != null)
									{
										continue;
									}
								}
								selectedEntries[i].group = mGroupToSwitchToStorageSpace;
							}
							mDirty = true;
						}
					}
				}

			});

		}


		/// <summary>
		/// Renders and processes import export UI.
		/// </summary>
		private void RenderAndProcessImportExportUI()
		{
			EditorUtils.VAutoLayout (delegate
			{
				EditorUtils.HAutoLayout(delegate {
					EditorGUILayout.LabelField("Path:",mColWidth);
					mShipper.mPath = EditorGUILayout.TextArea(mShipper.mPath,mColWidth);
					if(GUILayout.Button("Select",mButtonWidth))
					{
						mShipper.mPath = EditorUtility.OpenFilePanel("Select JSON","","json");
					}
				});

				if(mShipper.mIsPathValid)
				{
					EditorUtils.HAutoLayout(delegate {
						EditorGUILayout.LabelField("",mColWidth);
						if(GUILayout.Button("Export",mColWidth))
						{
							if(System.IO.File.Exists(mShipper.mPath))
							{
								mShipper.mOverwrite =  ConfirmUserActionWithShiftOverride("Overwrite File?","Would you like to overwrite any file found in the specified path?","Yes","No");
							}
							mShipper.ExportData(mLb);
						}
					});
					EditorUtils.HAutoLayout(delegate {
						EditorGUILayout.LabelField("",mColWidth);
						if(GUILayout.Button("Import",mColWidth))
						{
							if(ConfirmUserActionWithShiftOverride("Import Data?","Import will replace any existing keys in existing groups and add new groups and keys where applicable, Confirm?","Yes","No"))
							{
								mShipper.ImportData(mLb);
								OverwriteRawData(mLb.m_rawData);
							}
						}
					});

				}
			});
		}


		private void UpdateStateDictionaries(Dictionary<string,List<V>> groupedData)
		{
			//refresh group and entry selection states
			if (groupedData != null)
			{
				Dictionary<string,bool> newGroupSelectionStates = new Dictionary<string, bool>();
				Dictionary<string,bool> newGroupFoldoutStates = new Dictionary<string, bool>();
				Dictionary<string,Dictionary<string,bool>> newEntrySelectionStates = new Dictionary<string, Dictionary<string, bool>>();

				foreach (KeyValuePair<string,List<V>> pair in groupedData)
				{
					newGroupSelectionStates.Add (pair.Key, false);
					if (mGroupSelectionStates != null && mGroupSelectionStates.ContainsKey (pair.Key))
					{
						newGroupSelectionStates [pair.Key] = mGroupSelectionStates [pair.Key];
					}
					newGroupFoldoutStates.Add (pair.Key, false);
					if (mGroupFoldoutStates != null && mGroupFoldoutStates.ContainsKey (pair.Key))
					{
						newGroupFoldoutStates [pair.Key] = mGroupFoldoutStates [pair.Key];
					}
					newEntrySelectionStates.Add (pair.Key, new Dictionary<string, bool> ());
					Dictionary<string,bool> dictionaryToConsider = null;
					if (mEntrySelectionStates != null && mEntrySelectionStates.ContainsKey (pair.Key))
					{
						dictionaryToConsider = mEntrySelectionStates [pair.Key];
					}
					if (dictionaryToConsider != null)
					{
						Dictionary<string,bool> toAddTo = newEntrySelectionStates [pair.Key];
						foreach (V entry in pair.Value)
						{
							bool toSet = false;
							dictionaryToConsider.TryGetValue (entry.key, out toSet);
							toAddTo.Add (entry.key, toSet);
						}
					}
					else
					{
						Dictionary<string,bool> toAddTo = newEntrySelectionStates [pair.Key];
						foreach (V entry in pair.Value)
						{
							toAddTo.Add (entry.key, false);
						}
					}


				}
				mGroupSelectionStates.Clear ();
				mGroupSelectionStates = null;
				mGroupFoldoutStates.Clear ();
				mGroupFoldoutStates = null;
				mEntrySelectionStates.Clear ();
				mEntrySelectionStates = null;

				mGroupSelectionStates = newGroupSelectionStates;
				mGroupFoldoutStates = newGroupFoldoutStates;
				mEntrySelectionStates = newEntrySelectionStates;
			}
		}

		public void OnInspectorGUI()
		{
			if (Event.current.type == EventType.MouseDown || Event.current.Equals(Event.KeyboardEvent("return")))
			{
				EditorGUI.FocusTextInControl ("");
				mSelectedGroup = "";
				mSelectedKey = "";
			}

			//mSelectedAnElementThisUpdate = false;
			//display data along with removal option
			//mWidth = GUILayout.Width(EditorGUIUtility.currentViewWidth);
			mColWidth = GUILayout.Width(0.4f * EditorGUIUtility.currentViewWidth);
			mKeyEntryWidth = GUILayout.Width(0.3f * EditorGUIUtility.currentViewWidth);
			mButtonWidth = GUILayout.Width(0.05f * EditorGUIUtility.currentViewWidth);
			mIndexWidth = GUILayout.Width(0.025f * EditorGUIUtility.currentViewWidth);

			EditorUtils.VAutoLayout ( delegate
			{

				//draw language label
				EditorUtils.HAutoLayout ( delegate
				{

					EditorGUILayout.LabelField ("Language :",mColWidth);
					string val = mLb.Language;
					mLb.Language = EditorGUILayout.TextField (mLb.Language,mColWidth);
					if(val.Length != mLb.Language.Length)
					{
						mDirty = true;
					}
				});

				//search sort
				EditorUtils.VAutoLayout ( delegate
				{
					RenderAndProcessSearchUI();
					RenderAndProcessSortUI();

					//update data to display
					mGroupedData = GetGroupedData();
					UpdateStateDictionaries(mGroupedData);
					mDataToDisplay = GetDataToDisplay(mGroupedData);

					//display sorted results
					if(mDataToDisplay != null && mDataToDisplay.Count > 0)
					{
						//draw a line to simulate non-existent UI
						GUILayout.Box("",GUILayout.ExpandWidth(true),GUILayout.Height(1));

						//draw keys and values label
						EditorUtils.HAutoLayout(delegate {
							EditorGUILayout.LabelField ("Groups: "+mDataToDisplay.Count,mColWidth);
						});

						int index = 0;
						foreach(KeyValuePair<string,List<V>> pair in mDataToDisplay)
						{
							RenderAndProcessGroupUI(index,pair.Key,pair.Value);
							++index;
						}
					}
				});
			});

			//draw a line to simulate non-existent UI
			//signify start of the options UI
			GUILayout.Box("",GUILayout.ExpandWidth(true),GUILayout.Height(1));



			//make a list of all selected groups
			List<string> keys = new List<string> (mDataToDisplay.Keys);
			List<KeyValuePair<string,List<V>>> selectedGroups = new List<KeyValuePair<string, List<V>>> ();
			for (int bdIn = 0; bdIn < keys.Count; ++bdIn)
			{
				string key = keys [bdIn];
				if (mGroupSelectionStates [keys[bdIn]])
				{
					KeyValuePair <string,List<V>> pair = new KeyValuePair<string, List<V>> (key, mDataToDisplay [key]);
					selectedGroups.Add (pair);
				}
			}

			List<V> selectedEntries = new List<V> ();
			for (int i = 0; i < keys.Count; ++i)
			{
				if (mGroupFoldoutStates [keys [i]])
				{
					List<string> innerKeys = new List<string>(mEntrySelectionStates [keys [i]].Keys);
					for (int j = 0; j < innerKeys.Count; ++j)
					{
						if (mEntrySelectionStates [keys [i]] [innerKeys [j]])
						{
							selectedEntries.Add(mLb.m_rawData.FindAll((x)=>x.group==keys[i]).Find((x)=>x.key == innerKeys[j]));
						}
					}
				}
			}
			if (selectedEntries.Count > 0)
			{
				//entry UI
				ClearSelectionStates(true,false);
				RenderAndProcessEntriesSelectedUI(selectedEntries);
			}
			else if (selectedGroups.Count > 0)
			{
				if (selectedGroups.Count == 1)
				{
					//add new key UI and change name UI
					RenderAndProcessAddKeyToGroupUI(selectedGroups[0]);
				}
				RenderAndProcessSelectedGroupsUI(selectedGroups);
			}
			else
			{
				//check if any entries are selected
				RenderAndProcessAddKeyUI ();
			}

			EditorUtils.HAutoLayout (delegate
			{
				EditorGUILayout.LabelField("",mColWidth);
				if (GUILayout.Button ("Clear Selection",mColWidth))
				{
					ClearSelectionStates ();
				}	
			});


			RenderAndProcessImportExportUI ();


			//refresh and reserialize if necessary
			if (mDirty)
			{
				ReserializeAndRefresh();
				mDirty = false;
			}

		}

		/// <summary>
		/// Releases all references maintained within the editor.
		/// call initialize in order to be able to use it again
		/// </summary>
		public void ReleaseEditor()
		{
			if (mGroupedData != null)
			{
				mGroupedData.Clear ();
				mGroupedData = null;
			}
			if (mGroupFoldoutStates != null)
			{
				mGroupFoldoutStates.Clear ();
				mGroupFoldoutStates = null;
			}
			if (mGroupSelectionStates != null)
			{
				mGroupSelectionStates.Clear ();
				mGroupSelectionStates = null;
			}
			if (mEntrySelectionStates != null)
			{
				mEntrySelectionStates.Clear ();
				mEntrySelectionStates = null;
			}
			if (mDataToDisplay != null)
			{
				mDataToDisplay.Clear ();
				mDataToDisplay = null;
			}

		
			mLb = null;
			mNewKeyInstance = default(V);
			mShipper = null;
			mTargetObject = null;
			mCallingWindow = null;
		}
	}
}