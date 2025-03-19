
using UnityEngine;

/// <summary>
/// 현재 미사용
/// </summary>
public abstract class IShowHide : MonoBehaviour {
    protected abstract void Show();
    protected abstract void Hide();
    protected abstract void Toggle(); // 클래스명과 안맞음
}
