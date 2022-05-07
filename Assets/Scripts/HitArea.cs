using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
	// Semi-transparent rectangle in the middle of the screen. Tap for hitting the ball
	public class HitArea : MonoBehaviour, IPointerClickHandler
	{
		public UnityAction<Vector2> HitEvent;

		// Fire event with normalised position of the tap inside the rectangle
		public void OnPointerClick(PointerEventData eventData)
		{
			RectTransform rT = transform as RectTransform;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rT, eventData.pointerCurrentRaycast.screenPosition, null, out Vector2 localPoint);
			HitEvent?.Invoke(Rect.PointToNormalized(rT.rect, localPoint));
		}
	}
}