using UnityEngine;
using UnityEngine.EventSystems;

public class ControlSchemeUpdater : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private InputReader inputReader;
    private void OnEnable() {
        inputReader = FindObjectOfType<InputReader>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(inputReader == null) return;
        inputReader.EnableUIInput();
    }

    public void OnPointerExit(PointerEventData eventData) {
        if(inputReader == null) return;
        inputReader.EnableGameplayInput();
    }
}
