using System;
using System.Collections;
using System.Collections.Generic;
using Brisk.Entities;
using Brisk.Serialization;
using Lidgren.Network;
using UnityEngine;

public enum ContainableSize : ushort
{
    Tiny = 0,
    Small = 1,
    Normal = 2,
    Bulky = 3,
    Huge = 4,
    Gigantic = 5,
}

[RequireComponent(typeof(NetEntity))]
[DisallowMultipleComponent]
public class Container : NetBehaviour
{
    [SerializeField] private int numSlots = 0;
    [SerializeField] private ContainableSize maxSize = ContainableSize.Normal;
    [SerializeField] private bool restricted; // Can't grab out of this container
    public bool Restricted { get; set; }

    private readonly List<Containable> contained = new List<Containable>();

    public delegate void ContainerUpdated(Containable containable);

    public ContainerUpdated OnContained { get; set; } = delegate { };
    public ContainerUpdated OnRemoved { get; set; } = delegate { };

    public bool CanContain(Containable containable)
    {
        return contained.Count < numSlots && containable.Size <= maxSize;;
    }

    public void Contain(Containable containable)
    {
        contained.Add(containable);
        OnContained(containable);
    }

    public void Remove(Containable containable)
    {
        contained.Remove(containable);
        OnRemoved(containable);
    }

    public IList<Containable> GetContained()
    {
        return contained.AsReadOnly();
    }

    [Serialize(typeof(Container))]
    public static void Serialize(Container container, NetOutgoingMessage msg)
    {
        if (container != null) msg.Write(container.Entity.Behaviour(container));
        else msg.Write((byte)255);
    }
    [Deserialize(typeof(Container))]
    public static Container Deserialize(NetEntity entity, NetIncomingMessage msg)
    {
        var id = msg.ReadByte();
        if (id == 255) return null;
        return entity.Behaviour(id) as Container;
    }
}