using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

namespace SillyGames.TreasureHunt.HuntEditor
{
    public class Node : MonoBehaviour
    {
        public static Vector3 MousePosition
        {
            get; set;
        }

        public static Node GetNodeWithInstanceID(int a_iInstanceID)
        {
            var nodes = FindObjectsOfType<Node>();
            foreach (var item in nodes)
            {
                if (item.GetInstanceID() == a_iInstanceID)
                {
                    return item;
                }
            }
            return null;
        }

        public static Vector2 WindowSize { get; set; }

        [SerializeField]
        private Node m_ref = null;

        public Rect Area
        {
            get
            {
                return new Rect(transform.position, transform.localScale);
            }

            set
            {
                Position = new Vector2(value.x, value.y);
                Size = value.size;
            }
        }

        public Vector2 Position
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        public Vector2 Size
        {
            get
            {
                return transform.localScale;
            }
            set
            {
                transform.localScale = value;
            }
        }

        void Reset()
        {
            Area = new Rect(10, 50, 200, 250);
        }

        private const float IconSize = 20;
        
       public void DrawNode()
        {
            GUILayout.BeginArea(GetRect(), GUI.skin.box);
            //GUILayout.Label("hi", GUI.skin.button, GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));
            var alignPrev = GUI.skin.label.alignment;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            //GUI.Label(new Rect(IconSize, 0, Size.x - (IconSize * 2.0f), IconSize), GetType().Name.ToString());
            GUILayout.Label(GetType().Name.ToString() + (DragInfo.NodeBeingDragged == this? "(Dragging)":""));
            GUI.skin.label.alignment = alignPrev;
            //var l_selectionRect = GUILayoutUtility.GetLastRect();
            //if (IsMouseEventInRect(l_selectionRect, EventType.MouseUp))
            //{
            //      Selection.activeObject = gameObject;
            //   }
           // Debug.Log("DrawNode");
           

            GUI.changed = false;
            var newName = GUILayout.TextField(name);
            if(GUI.changed)
            {
                name = newName;
            }

            //DrawHook(new Rect(0, 0, IconSize, IconSize));
            //DrawRing(new Rect(Size.x - IconSize, 0, IconSize, IconSize));
            OnDrawNode();
            GUILayout.Label("",GUILayout.ExpandWidth(true),GUILayout.Height(20));
            var lastRect = GUILayoutUtility.GetLastRect();
            
            GUI.Button(new Rect(lastRect.width -25, lastRect.y, 30, 20), "ss");
            //GUI.Button(lastRect, "button");
            
            GUILayout.EndArea();
          
            HandleMouseInput();
           

        }

        public virtual void DrawConnections()
        {
            if(DragInfo.IsDragging && DragInfo.NodeBeingDragged == this)
            {
                DrawLine(MousePosition, HookPoint);
            }

            if (IsReference)
            {
                if (Ref != null)
                {
                    DrawLine(RingPoint, Ref.HookPoint);
                }
            }
        }

        protected virtual void OnDrawNode()
        {
            DrawDefaultInpector();
            DrawHook();
            if (IsReference)
            {
                DrawRing();
            }
            if (DrawSelect())
            {
                Selection.activeObject = gameObject;
            }
            if (DrawDelete())
            {
                DestroyImmediate(gameObject);
                return;
            }
            IsReference = GUILayout.Toggle(IsReference, IsReference ? "Reference Type" : "Value Type", GUI.skin.button);
        }

        private Editor m_customEditor = null;

        protected Editor CustomEditor
        {
            get
            {
                if(m_customEditor == null)
                {
                    m_customEditor = Editor.CreateEditor(this);
                }
                return m_customEditor;
            }
        }

        private void DrawDefaultInpector()
        {
            CustomEditor.OnInspectorGUI();
        }
        protected virtual Rect GetRect()
        {
            return Area;
        }

        private static Texture s_ringTexture = null;
        private static Texture s_hookTexture = null;
        private static Texture s_selectTexture = null;
        private static Texture s_deleteTexture = null;
        private static Texture RingTexture
        {
            get
            {
                if (s_ringTexture == null)
                {
                    s_ringTexture = Resources.Load<Texture>(RING_ICON_PATH);
                }
                return s_ringTexture;
            }
        }
        private static Texture HookTexture
        {
            get
            {
                if (s_hookTexture == null)
                {
                    s_hookTexture = Resources.Load<Texture>(HOOK_ICON_PATH);
                }
                return s_hookTexture;
            }
        }

        private static Texture SelectTexture
        {
            get
            {
                if (s_selectTexture == null)
                {
                    s_selectTexture = Resources.Load<Texture>(SELECT_ICON_PATH);
                }
                return s_selectTexture;
            }
        }
        private static Texture DeleteTexture
        {
            get
            {
                if (s_deleteTexture== null)
                {
                    s_deleteTexture = Resources.Load<Texture>(DELETE_ICON_PATH);
                }
                return s_deleteTexture;
            }
        }

        public static bool DrawRing(Rect a_rect)
        {
            
            return DrawIcon(RingTexture, a_rect,true);
        }

        private class DragInformation
        {
            public Node NodeBeingDragged = null;

            public bool IsDragging
            {
                get
                {
                    return NodeBeingDragged != null;
                }
            }
        }

        private static DragInformation DragInfo = new DragInformation();

        internal protected Rect RingRect { get; set; }
        internal protected Rect HookRect { get; set; }
        internal protected Rect SelectRect { get; set; }
        internal protected Rect DeleteRect { get; set; }

        protected internal void DrawRing(Node a_linkedNode = null)
        {
            RingRect = new Rect(Size.x - IconSize, 0, IconSize, IconSize);
            if (IsMouseEventInRect(RingRect, EventType.MouseUp))
            {
                if(DragInfo.NodeBeingDragged != null)
                {
                    TryLinking(DragInfo.NodeBeingDragged);
                }
                DragInfo.NodeBeingDragged = null;
                IsMoving = false;
            }

            if(DrawRing(RingRect))
            {
                Unlink(a_linkedNode);
            }            
        }

        [HideInInspector]
        [SerializeField]
        private bool m_bIsReference = false;
        public bool IsReference
        {
            get
            {
                return m_bIsReference;
            }
            set
            {
                ///when set to true, need to check if existing ref value is not causing circular reference
                if (value && Ref != null)
                {
                    if (IsRecursiveRef(Ref))
                    {
                        Debug.LogWarning("Recursive ref! can not set node value type to ref.: " + this + ", instance id: " + this.GetInstanceID());
                        return;
                    }

                }
                m_bIsReference = value;

            }
        }

        public Node Ref
        {
            get
            {
                if (RefInstanceID == 0)
                {
                    return null;
                }
                return m_ref;
            }

            set
            {
                if (!IsRecursiveRef(value))
                {
                    //RefInstanceID = value != null ? value.GetInstanceID() : 0;
                    m_ref = value;
                }
                else
                {
                    Debug.LogWarning("Value can not be assigned due to recursive ref, Node: " + this + ", ref: " + value);
                }
            }
        }

        //[SerializeField]
        //private int m_refInstanceID = 0;

        private int RefInstanceID
        {
            get
            {
                return m_ref != null ? m_ref.GetInstanceID() : 0;
            }
            //set
            //{
            //    m_refInstanceID = value;

            //}
        }

        public bool IsRecursiveRef(Node a_ref)
        {
            if (a_ref == this)
            {
                return true;
            }
            var tempRef = a_ref;

            while (tempRef != null && tempRef.IsReference && tempRef != this)
            {
                tempRef = tempRef.Ref;
            }

            return tempRef == this;
        }

        protected virtual void Unlink(Node a_linkedNode)
        {
            Ref = null;
        }

        protected virtual void TryLinking(Node a_node)
        {
            if (a_node.GetType() == GetType())
            {
                //Debug.Log("Linking : " + a_node.GetHashCode() + ", to : " + GetHashCode());
                Ref = a_node;
            }
        }

        public static void DrawHook(Rect a_rect)
        {
            DrawIcon(HookTexture, a_rect, false);
        }

        public static bool DrawSelect(Rect a_rect)
        {
            var l_isSelectCliked = false;
            if (PressedKey == KeyCode.LeftAlt)
            {
                l_isSelectCliked = (GUI.Button(a_rect, SelectTexture));
               
            }
            return l_isSelectCliked;
        }

        public static bool DrawDelete(Rect a_rect)
        {
            var l_isSelectCliked = false;
            if (PressedKey == KeyCode.LeftAlt)
            {
                l_isSelectCliked = (GUI.Button(a_rect, DeleteTexture));

            }
            return l_isSelectCliked;
        }

        protected internal void DrawHook()
        {
            HookRect = new Rect(0, 0, IconSize, IconSize);
            if(IsMouseEventInRect(HookRect, EventType.MouseDown))
            {
                Event.current.Use();
                DragInfo.NodeBeingDragged = this;
            }
            DrawHook(HookRect);
        }

        protected internal bool DrawSelect()
        {
            SelectRect = new Rect(HookRect.width, 0, IconSize, IconSize);
            //if (IsMouseEventInRect(SelectRect, EventType.MouseDown))
            {
                //Event.current.Use();
                //DragInfo.NodeBeingDragged = this;
            }
           return DrawSelect(SelectRect);
        }

        protected internal bool DrawDelete()
        {
            DeleteRect = new Rect((SelectRect.x+SelectRect.width), 0, IconSize, IconSize);
            //if (IsMouseEventInRect(SelectRect, EventType.MouseDown))
            {
                //Event.current.Use();
                //DragInfo.NodeBeingDragged = this;
            }
            return DrawDelete(DeleteRect);
        }


        public Vector3 HookPoint
        {
            get
            {
                var pos = Position;
                return new Vector2(pos.x + (IconSize/2.0f), pos.y + (IconSize/2.0f));
            }
        }

        public Vector3 RingPoint
        {
            get
            {
                var pos = Position;
                var size = Size;
                return new Vector2(pos.x + size.x - (IconSize / 2.0f), pos.y + (IconSize / 2.0f));
            }
        }

        private bool IsMouseEventInRect(Rect a_rect, EventType a_eventType)
        {
            if (Event.current.isMouse)
            {
                if (Event.current.type == a_eventType)
                {
                    var rectCheck = new Rect(a_rect);
                    rectCheck.x += Position.x;
                    rectCheck.y += Position.y;
                    if (rectCheck.Contains(MousePosition))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private const string RING_ICON_PATH = "Icons/ring";
        private const string HOOK_ICON_PATH = "Icons/hook";
        private const string SELECT_ICON_PATH = "Icons/select";
        private const string DELETE_ICON_PATH = "Icons/delete";



        public static KeyCode PressedKey = KeyCode.None;
        public static bool DrawIcon(Texture a_texture , Rect a_rect, bool a_bAllowClose)
        {
            if (a_texture == null)
            {
                return false;
            }
            var color = GUI.color;
            GUI.color = Color.black;
            GUI.Label(a_rect, a_texture);
            GUI.color = color;            
            var buttonClikced = false;
            if (a_bAllowClose)
            {
                if (PressedKey == KeyCode.LeftControl)
                {
                    buttonClikced = GUI.Button(a_rect, "X");
                }
            }
            return buttonClikced;
        }

        public static void ResetDragInfo()
        {
            DragInfo.NodeBeingDragged = null;
        }

        private bool IsMoving { get; set; }
        private Vector3 VecMousePos { get; set; }

        private Vector3 vecTouchPos { get; set; }

        public void EditorUpdate()
        {
            if (IsMoving)
            {
                var pos = MousePosition - vecTouchPos;
                pos.x = Mathf.Clamp(pos.x, 0, WindowSize.x - Size.x);
                pos.y = Mathf.Clamp(pos.y, 0, WindowSize.y - Size.y);
                transform.position = pos;
            }
        }

        private void HandleMouseInput()
        {
            if (Event.current.isMouse)
            {
                var eventType = Event.current.type;
                if (eventType == EventType.MouseDown)
                {
                    if(Event.current.button == 0 && Area.Contains(MousePosition))
                    {
                        vecTouchPos = MousePosition - transform.position;
                        IsMoving = true;
                    }
                }

                if (eventType == EventType.MouseUp)
                {
                    IsMoving = false;
                }
            }
        }

        public static void DrawLine(Vector3 a_vecStart, Vector3 a_vecEnd)
        {
            UnityEditor.Handles.DrawLine(a_vecStart, a_vecEnd);
        }

        private void TitleBarControls()
        {

        }
    }

}
