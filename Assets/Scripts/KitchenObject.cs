using ScriptableObjects;
using UnityEngine;
using UnityEngine.Serialization;

public class KitchenObject : MonoBehaviour {
    [SerializeField] private KitchenObjectScriptableObject kitchenObjectScriptableObject;


    private IKitchenObjectParent _parent;


    public static void SpawnKitchenObject(
        KitchenObjectScriptableObject kitchenObjectScriptableObject,
        IKitchenObjectParent parent
    ) {
        var kitchenObjectTransform = Instantiate(kitchenObjectScriptableObject.prefab);
        kitchenObjectTransform.GetComponent<KitchenObject>().SetParent(parent);
    }


    public KitchenObjectScriptableObject GetKitchenObjectScriptable() {
        return kitchenObjectScriptableObject;
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

    public void DestroySelf() {
        _parent.ClearKitchenObject();
        Destroy(gameObject);
    }
}
