using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Lidgren.Network;
using UnityEngine.UI;

public class CharacterInventory : MonoBehaviour
{
    [SerializeField] private Container[] slots = new Container[0];
    [SerializeField] private int numHands = 2;
    
    [SerializeField] private Transform[] hands = new Transform[0];
    
    private int activeHand = 0;
    public int ActiveHand
    {
        get => activeHand;
        private set
        {
            slots[activeHand].OnContained -= OnAddedToHand;
            slots[activeHand].OnRemoved -= OnRemovedFromHand;
            activeHand = value;
            slots[activeHand].OnContained += OnAddedToHand;
            slots[activeHand].OnRemoved += OnRemovedFromHand;
        }
    }

    private void Start()
    {
        ActiveHand = 0;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleHands();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            DropInHand();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            UseItemInActiveHand();
        }
    }

    public void ToggleHands()
    {
        if (++ActiveHand >= numHands)
        {
            ActiveHand = 0;
        }
    }

    public Container GetActiveHand() {
        return slots[ActiveHand];
    }

    public Container GetSlotById(int id)
    {
        return slots.Length > id ? slots[id] : null;
    }

    public Containable GetInHand() {
        var contained = GetActiveHand().GetContained();
        return contained.Count == 0 ? null : contained[0];
    }

    private void OnAddedToHand(Containable containable) 
    {
        containable.transform.SetParent(hands[ActiveHand], true);
        containable.transform.localPosition = containable.PickPosition;
        containable.transform.localEulerAngles = containable.PickRotation;
        var containableRb = containable.GetComponent<Rigidbody>();
        if (containableRb) {
            containableRb.isKinematic = true;
        }
    }

    private void OnRemovedFromHand(Containable containable)
    {
        containable.transform.parent = null;
        var containableRb = containable.GetComponent<Rigidbody>();
        if (containableRb) {
            containableRb.isKinematic = false;
            containableRb.velocity = Vector3.zero;
        }
    }

    private void DropInHand()
    {
        var inHand = GetInHand();
        if (!inHand) return;
        inHand.Container = null;
    }

    public void UseItemInActiveHand()
    {
        
    }

    public void SwapActiveItem(int id)
    {
        GetSlotById(id).GetContained()[0].Container = GetActiveHand();
    }

}