using ScriptableObjects.KitchenObjects;
using UnityEngine;

public class KitchenObject : MonoBehaviour {
    [SerializeField] private KitchenObjectScriptable kitchenObjectScriptable;


    private IKitchenObjectParent _parent;


    public KitchenObjectScriptable GetKitchenObjectScriptable() {
        return kitchenObjectScriptable;
    }

    public IKitchenObjectParent GetParent() {
        return _parent;
    }

    public void SetParent(IKitchenObjectParent newParent) {
        if (newParent.HasKitchenObject()) {
            Debug.LogError("Trying to set kitchen object for a prent which already has one!");
        }
        newParent.SetKitchenObject(this);
        transform.parent = newParent.GetKitchenObjectFollowTransform();
        transform.localPosition = Vector3.zero;
        _parent = newParent;
    }
}
