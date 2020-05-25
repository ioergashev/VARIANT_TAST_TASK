using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Variant
{
    public enum ControlMode
    {
        None,
        Stick,
        Drag,
    }

    public class GameControl : MonoBehaviour
    {
        private static GameControl instance;

        public static GameControl Instance 
        {
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<GameControl>();
                }

                return instance;
            }
        }

        public ControlMode Mode = ControlMode.Drag;

        public Hand.AttachmentFlags DragAttachmentFlags = Hand.defaultAttachmentFlags;
        public Hand.AttachmentFlags StickAttachmentFlags = Hand.defaultAttachmentFlags;

        public Hand.AttachmentFlags AttachmentFlags
        {
            get
            {
                switch(Mode)
                {
                    case ControlMode.Drag:
                        return DragAttachmentFlags;

                    case ControlMode.Stick:
                        return StickAttachmentFlags;

                    default:
                        return Hand.defaultAttachmentFlags;
                }
            }
        }

        public void SetDragMode()
        {
            Mode = ControlMode.Drag;
        }

        public void SetStickMode()
        {
            Mode = ControlMode.Stick;
        }
    }
}