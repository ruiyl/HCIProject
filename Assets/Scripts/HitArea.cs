using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
	public class HitArea : MonoBehaviour, IPointerClickHandler
	{
		public UnityAction<Vector2> HitEvent;

		public void OnPointerClick(PointerEventData eventData)
		{
			RectTransform rT = transform as RectTransform;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rT, eventData.pointerCurrentRaycast.screenPosition, null, out Vector2 localPoint);
			HitEvent?.Invoke(Rect.PointToNormalized(rT.rect, localPoint));
		}
	}
}