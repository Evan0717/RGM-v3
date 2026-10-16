using AdminToys;
using Exiled.API.Features;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using System;
using UnityEngine;
using MEC;

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
    public string Color { get; set; } = "FFFFFF";
}

[AttributeUsage(AttributeTargets.Class)]
public class BlockAttribute(
    string name,
    string SchematicName,
    BlockType blockType,
    float DestroyTime,
    string color = "FFFFFF") : Attribute
{
    public string Name { get; } = name;
    public string SchematicName { get; } = SchematicName;
    public BlockType blockType = blockType;
    public float DestroyTime { get; } = DestroyTime;
    public string Color { get; } = color;
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

    public void Destroy()
    {
        if (Schematic != null)   // 유니티의 null 체크는 파괴된 오브젝트도 걸러냄
            Schematic.Destroy();
    }
}
public static class BlockEffects
{
    private const string DestroyEffectSchematic = "BlockDestroyEffect";
    private const float EffectLifetime = 1f;

    public static void SpawnDestroyEffect(BlockInfo info, Vector3 position)
    {
        try
        {
            SchematicObject effect = ObjectSpawner.SpawnSchematic(DestroyEffectSchematic, position);
            if (effect == null)
            {
                Log.Error($"Schematic '{DestroyEffectSchematic}' not found.");
                return;
            }

            // 모든 프리미티브에 블록 색을 입힘
            Color color = ParseColor(info.Color);
            foreach (GameObject go in effect.AttachedBlocks)
            {
                if (go.TryGetComponent(out PrimitiveObjectToy toy))
                    toy.NetworkMaterialColor = color;
            }

            // 5초 뒤 삭제
            Timing.CallDelayed(EffectLifetime, () =>
            {
                if (effect != null)   // 이미 사라졌으면 건너뜀
                    effect.Destroy();
            });
        }
        catch (Exception e)
        {
            // 이펙트 때문에 블록 파괴 흐름이 끊기지 않게 예외는 여기서 처리
            Log.Error($"Failed to spawn block destroy effect: {e}");
        }
    }

    private static Color ParseColor(string hex)
    {
        if (!string.IsNullOrEmpty(hex) && ColorUtility.TryParseHtmlString("#" + hex.TrimStart('#'), out Color color))
            return color;

        return Color.white; // 잘못된 값이면 흰색
    }
}