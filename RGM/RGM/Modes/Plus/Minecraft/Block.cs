using AdminToys;
using Exiled.API.Features;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using System;
using UnityEngine;

namespace RGM.Modes;

public abstract class Block
{
    public BlockInfo Info { get; internal set; }
    public Vector3Int GridPos { get; internal set; }
    public SchematicObject Schematic { get; internal set; }

    public virtual void OnPlaced(Player player, RaycastHit hit) { }
    public virtual bool OnInteract(Player player) => false; // true면 상호작용이 처리됨
    public virtual void OnBroken(Player player) { }
}
public class BlockInfo
{
    public Type Type { get; set; }
    public string Name { get; set; }
    public string SchematicName { get; set; }
    public BlockType BlockType { get; set; }
    public float DestroyTime { get; set; }
}

[AttributeUsage(AttributeTargets.Class)]
public class BlockAttribute(
    string name,
    string SchematicName,
    BlockType blockType,
    float DestroyTime) : Attribute
{
    public string Name { get; } = name;
    public string SchematicName { get; } = SchematicName;
    public BlockType blockType = blockType;
    public float DestroyTime { get; } = DestroyTime;
}

public class BlockData
{
    public BlockType Type { get; set; }
    public ushort Count { get; set; }
}

public enum BlockType
{
    Air,
    WhiteConcrete
}

public enum BlockForm { Placed, Held, Dropped }
public class BlockVisual
{
    public SchematicObject Schematic { get; }
    public BlockForm Form { get; private set; }

    public BlockVisual(BlockInfo info, BlockForm form, Vector3 position)
    {
        Schematic = ObjectSpawner.SpawnSchematic(info.SchematicName, position);
        if (Schematic != null)
            SetForm(form);
    }

    public void SetForm(BlockForm form)
    {
        Form = form;

        // 충돌은 Placed일 때만
        bool collidable = form == BlockForm.Placed;
        foreach (GameObject go in Schematic.AttachedBlocks)
        {
            if (go.TryGetComponent(out PrimitiveObjectToy toy))
                toy.NetworkPrimitiveFlags = collidable
                    ? toy.NetworkPrimitiveFlags | PrimitiveFlags.Collidable
                    : toy.NetworkPrimitiveFlags & ~PrimitiveFlags.Collidable;
        }

        if (form == BlockForm.Placed)
            return;

        var anim = AnimationController.Get(Schematic);
        if (anim.Animators.Count > 0)
            anim.Play(form.ToString());
    }

    public void PlaySwing()
    {
        if (Schematic == null) return;
        var anim = AnimationController.Get(Schematic);
        if (anim.Animators.Count > 0)
            anim.Animators[0].Play("Swing", 0, 0f);
    }

    public void AttachTo(Transform parent)
    {
        if (Schematic == null) return;
        Schematic.transform.parent = parent;
        Schematic.transform.localPosition = Vector3.zero;
        Schematic.transform.localRotation = Quaternion.identity;
    }

    public void Destroy() => Schematic?.Destroy();
}