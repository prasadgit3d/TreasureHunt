using System;
using UnityEngine;

namespace SillyGames.SGBase
{
    public static class RectAlignment
    {
        // enumeration for the various alignments available for the control
        public enum AlignmentType
        {
            None,
            LeftCenter,
            RightCenter,
            TopCenter,
            BottomCenter,
            Center,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            //Total,
        }

        //private static readonly Vector2 vecNone = Vector2.zero;

        private static readonly Vector2 vecLeftTop = new Vector2(-1.0f, -1.0f);
        private static readonly Vector2 vecCenterTop = new Vector2(0.0f, -1.0f);
        private static readonly Vector2 vecRightTop = new Vector2(1.0f, -1.0f);

        private static readonly Vector2 vecLeftCenter = new Vector2(-1.0f, 0.0f);
        private static readonly Vector2 vecCenter = new Vector2(0.0f, 0.0f);
        private static readonly Vector2 vecRightCenter = new Vector2(1.0f, 0.0f);

        private static readonly Vector2 vecLeftBottom = new Vector2(-1.0f, 1.0f);
        private static readonly Vector2 vecCenterBottom = new Vector2(0.0f, 1.0f);
        private static readonly Vector2 vecRightBottom = new Vector2(1.0f, 1.0f);

        public static Rect AlignWith(this Rect a_rect, Rect a_parentRect, AlignmentType a_eAlignmentType, Vector2 a_vecOffset)
        {
            switch (a_eAlignmentType)
            {
                case AlignmentType.TopLeft:
                    return AlignWith(a_rect, a_parentRect, RectAlignment.vecLeftTop, a_vecOffset);
                case AlignmentType.TopCenter:
                    return AlignWith(a_rect, a_parentRect, RectAlignment.vecCenterTop, a_vecOffset);
                case AlignmentType.TopRight:
                    return AlignWith(a_rect, a_parentRect, RectAlignment.vecRightTop, a_vecOffset);
                case AlignmentType.LeftCenter:
                    return AlignWith(a_rect, a_parentRect, RectAlignment.vecLeftCenter, a_vecOffset);
                case AlignmentType.Center:
                    return AlignWith(a_rect, a_parentRect, RectAlignment.vecCenter, a_vecOffset);
                case AlignmentType.RightCenter:
                    return AlignWith(a_rect, a_parentRect, RectAlignment.vecRightCenter, a_vecOffset);
                case AlignmentType.BottomLeft:
                    return AlignWith(a_rect, a_parentRect, RectAlignment.vecLeftBottom, a_vecOffset);
                case AlignmentType.BottomCenter:
                    return AlignWith(a_rect, a_parentRect, RectAlignment.vecCenterBottom, a_vecOffset);
                case AlignmentType.BottomRight:
                    return AlignWith(a_rect, a_parentRect, RectAlignment.vecRightBottom, a_vecOffset);
                default:
                    return a_rect;
            }
        }

        public static Rect AlignWith(this Rect a_rect, Rect a_parentRect, AlignmentType a_eAlignmentType)
        {
            return AlignWith(a_rect, a_parentRect, a_eAlignmentType, Vector2.zero);
        }
        public static Rect AlignWith(this Rect a_rect, AlignmentType a_eAlignmentType, Vector2 a_vecOffset)
        {
            Rect rect = new Rect(0, 0, Screen.width, Screen.height);
            return AlignWith(a_rect, rect, a_eAlignmentType, a_vecOffset);
        }
        public static Rect AlignWith(this Rect a_rect, AlignmentType a_eAlignmentType)
        {
            return AlignWith(a_rect, a_eAlignmentType, Vector2.zero);
        }
        public static Rect AlignWith(this Rect a_rect, Rect a_parentRect, Vector2 a_vecAlignment, Vector2 a_vecOffset)
        {
            Vector2 vecHalfParentSize = new Vector2(a_parentRect.width / 2.0f, a_parentRect.height / 2.0f);
            Vector2 vecHalfChildSize = new Vector2(a_rect.width / 2.0f, a_rect.height / 2.0f);

            Vector2 vecParentCenter = new Vector2(a_parentRect.x + vecHalfParentSize.x,
                                                    a_parentRect.y + vecHalfParentSize.y);

            //Vector2 vecChildDim = new Vector2(a_rect.width,a_rect.height);
            Rect rectAligned = new Rect(
                                        vecParentCenter.x - vecHalfChildSize.x,
                                        vecParentCenter.y - vecHalfChildSize.y,
                                        a_rect.width, a_rect.height);
            rectAligned.x += a_vecAlignment.x * (vecHalfParentSize.x - vecHalfChildSize.x) + a_vecOffset.x;
            rectAligned.y += a_vecAlignment.y * (vecHalfParentSize.y - vecHalfChildSize.y) + a_vecOffset.y;
            return rectAligned;
        }
    }
}