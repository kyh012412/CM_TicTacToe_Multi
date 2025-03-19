using UnityEngine;

public abstract class MonoBehaviourCustom : MonoBehaviour {
    protected virtual void Show() {
        gameObject.SetActive(true);
    }

    protected virtual void Hide() {
        gameObject.SetActive(false);
    }

    protected virtual void Toggle() {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
