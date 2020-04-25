using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class OnPointerDown : UnityEvent<IPointerDownHandler> { }

[System.Serializable]
public class OnPointerClick : UnityEvent<IPointerClickHandler> { }

[System.Serializable]
public class OnPointerUp : UnityEvent<IPointerUpHandler> { }

[System.Serializable]
public class OnPointerExit : UnityEvent<IPointerExitHandler> { }

[System.Serializable]
public class OnSelect : UnityEvent<ISelectHandler> { }

[System.Serializable]
public class OnDeselect : UnityEvent<IDeselectHandler> { }

[System.Serializable]
public class OnCancel : UnityEvent<ICancelHandler> { }