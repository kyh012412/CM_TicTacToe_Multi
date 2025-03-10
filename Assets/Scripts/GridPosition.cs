using UnityEngine;

public class GridPosition : MonoBehaviour {

    [SerializeField] private int x, y;
    private void OnMouseDown() {
        // Debug.Log(name + " Clicked!");
        Debug.Log("Click! " + x + ", " + y);
    }
}
