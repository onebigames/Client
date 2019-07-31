using System.Collections;
using System.Collections.Generic;
using Brisk.Entities;
using Brisk.Serialization;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[DisallowMultipleComponent]
public class Containable : NetBehaviour, IInteractable {
    public Sprite generatedIcon = null;

    [SerializeField] private ContainableSize size = ContainableSize.Normal;
    [SerializeField] private Vector3 pickPosition = Vector3.zero;
    [SerializeField] private Vector3 pickRotation = Vector3.zero;
    [SerializeField] private Vector3 iconRotation = Vector3.zero;

    public Vector3 IconRotation => iconRotation;
    public Vector3 PickPosition => pickPosition;
    public Vector3 PickRotation => pickPosition;

    public ContainableSize Size => size;

    private Container container;

    [SyncReliable] public NetEntity ContainerEntity { get; set; }
    
    [SyncReliable]
    public Container Container {
        get => container;
        set {
            if (value != null && container != null) {
                Debug.LogWarning("Tried to contain an already contained object");
                return;
            }

            if (container == value) return;

            if (value != null && !value.CanContain(this)) return;
            
            var oldContainer = container;
            container = value;
            ContainerEntity = container != null ? container.Entity : null;
            
            if (oldContainer) oldContainer.Remove(this);
            if (container) container.Contain(this);
        }
    }

    public bool IsInteractable(GameObject source) {
        // Check to see if we can remove it from the current container
        if (container != null && container.Restricted) {
            return false;
        }

        var inventory = source.GetComponent<CharacterInventory>();
        return inventory != null && inventory.GetActiveHand().CanContain(this);
    }

    public void Interact(GameObject source) {
        if (!IsInteractable(source)) return;
        var inventory = source.GetComponent<CharacterInventory>();
        if (inventory != null) Container = inventory.GetActiveHand();
    }
}
