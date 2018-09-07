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

        public static Vector2 WindowSize { get; set; }

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
            Area = new Rect(10, 50, 200, 150);
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
        }

        protected virtual void OnDrawNode()
        {
            
        }

        protected virtual Rect GetRect()
        {
            return Area;
        }

        private static Texture s_ringTexture = null;
        private static Texture s_hookTexture = null;

        public static bool DrawRing(Rect a_rect)
        {
            if (s_ringTexture == null)
            {
                s_ringTexture = Resources.Load<Texture>(RING_ICON_PATH);
            }
            return DrawIcon(s_ringTexture, a_rect,true);
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
                    OnNodeDropped(DragInfo.NodeBeingDragged);
                }
                DragInfo.NodeBeingDragged = null;
                IsMoving = false;
            }

            if(DrawRing(RingRect))
            {
                OnUnlicked(a_linkedNode);
            }

          
            
        }

        protected virtual void OnUnlicked(Node a_linkedNode)
        {
            
        }

        protected virtual void OnNodeDropped(Node a_node)
        {
            
        }

        public static void DrawHook(Rect a_rect)
        {
            if (s_hookTexture == null)
            {
                s_hookTexture = Resources.Load<Texture>(HOOK_ICON_PATH);

            }
            
            DrawIcon(s_hookTexture, a_rect,false);
        }

        public static bool DrawSelect(Rect a_rect)
        {
            var l_isSelectCliked = false;
            if (PressedKey == KeyCode.LeftAlt)
            {
                l_isSelectCliked = (GUI.Button(a_rect, "y"));
               
            }
            return l_isSelectCliked;
        }

        public static bool DrawDelete(Rect a_rect)
        {
            var l_isSelectCliked = false;
            if (PressedKey == KeyCode.LeftAlt)
            {
                l_isSelectCliked = (GUI.Button(a_rect, "d"));

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
