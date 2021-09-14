using UnityEngine;

namespace NullFrameworkException.Mobile.InputHandling
{
    public class MobileInputManager : MonoSingleton<MobileInputManager>
    {
        public JoystickInputHandler joystick;

        /// <summary> Attempt to get the axis of the joystick attached to the system. </summary>
        public static Vector2 GetJoystickAxis() => Instance.joystick != null ? Instance.joystick.Axis : Vector2.zero;
        private void Start() => RunnableUtils.Setup(ref joystick, gameObject, true, this);
        private void Update() => RunnableUtils.Run(ref joystick, gameObject, true);
    }
}