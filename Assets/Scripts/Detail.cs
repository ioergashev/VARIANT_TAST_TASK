using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Valve.VR.InteractionSystem;

namespace Variant
{
	public enum DetailType
	{
		None,
		Wall,
		Floor,
		Roof
	}

	//-------------------------------------------------------------------------
	[RequireComponent(typeof(Interactable))]
    public class Detail : MonoBehaviour
    {
		private Interactable interactable;

		public DetailType detailType = DetailType.Wall;

		private float vertOffset = 0.1f;

		//-------------------------------------------------
		void Awake()
		{
			interactable = this.GetComponent<Interactable>();

			var ray = new Ray(transform.position, Vector3.down);

			RaycastHit hitInfo;

			if (Physics.Raycast(ray, out hitInfo))
			{
				vertOffset = Vector3.Distance(hitInfo.point, transform.position);
			}
		}


		//-------------------------------------------------
		// Called every Update() while a Hand is hovering over this object
		//-------------------------------------------------
		private void HandHoverUpdate(Hand hand)
		{
			GrabTypes startingGrabType = hand.GetGrabStarting();
			bool isGrabEnding = hand.IsGrabEnding(this.gameObject);

			if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
			{
				// Call this to continue receiving HandHoverUpdate messages,
				// and prevent the hand from hovering over anything else
				hand.HoverLock(interactable);

				// Attach this object to the hand
				hand.AttachObject(gameObject, startingGrabType, GameControl.Instance.AttachmentFlags);
			}
			else if (isGrabEnding)
			{
				// Detach this object from the hand
				hand.DetachObject(gameObject);

				// Call this to undo HoverLock
				hand.HoverUnlock(interactable);
			}
		}

		//-------------------------------------------------
		// Called every Update() while this GameObject is attached to the hand
		//-------------------------------------------------
		private void HandAttachedUpdate(Hand hand)
		{
			FollowHand();
		}

		private void FollowHand()
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			RaycastHit hitInfo;

			if (Physics.Raycast(ray, out hitInfo, 100, GetTargetLayer()))
			{
				transform.rotation = Quaternion.LookRotation(GetTargetForward(), hitInfo.normal);

				if (GameControl.Instance.Mode == ControlMode.Stick)
				{
					transform.position = hitInfo.point + hitInfo.normal * vertOffset;
				}
			}
		}

		private Vector3 GetTargetForward()
		{
			DetailType detailType = this.detailType;

			Vector3 forward;

			switch (detailType)
			{
				case DetailType.Floor:
					forward = Vector3.forward;
					break;
				case DetailType.Wall:
					forward = Vector3.down;
					break;
				case DetailType.Roof:
					forward = Vector3.forward;
					break;
				default:
					forward = Vector3.forward;
					break;
			}

			return forward;
		}

		private LayerMask GetTargetLayer()
		{
			DetailType detailType = this.detailType;

			string layerName;

			switch(detailType)
			{
				case DetailType.Floor:
					layerName = "floor";
					break;
				case DetailType.Wall:
					layerName = "wall";
					break;
				case DetailType.Roof:
					layerName = "roof";
					break;
				default:
					layerName = "floor";
					break;
			}

			LayerMask mask = LayerMask.GetMask(layerName);

			return mask;
		}
	}
}