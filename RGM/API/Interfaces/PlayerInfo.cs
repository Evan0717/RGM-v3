using CustomPlayerEffects;
using Exiled.API.Features.Items;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;

namespace RGM.API.Interfaces
{
    public class PlayerInfo
    {
        public RoleTypeId RoleType { get; set; }
        public float MaxHealth { get; set; }
        public float Health { get; set; }
        public IEnumerable<StatusEffectBase> ActiveEffects { get; set; }
        public IReadOnlyCollection<Item> Items { get; set; }
        public Item CurrentItem { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public int 공격력 { get; set; } = 0; // 🔪 입히는 피해량 0% 증가
        public int 방어력 { get; set; } = 0; // 📋 피해를 0% 적게 받음
        public int 저항력 { get; set; } = 0; // 😡 0% 디버프 감소
        public int 흡혈력 { get; set; } = 0; // 💦 입힌 피해의 0% 체력 회복
        public int 기술력 { get; set; } = 0; // 🔧 특정 기술의 효과 0% 증가
        public int 행운력 { get; set; } = 0; // 🍀 행운 0% 증가
        public int 지구력 { get; set; } = 0; // 🏃 이동 속도와 아이템 먹는 속도 0% 증가
        public int 진화력 { get; set; } = 0; // 🀄 크기 0 만큼 감소 (1당 0.01만큼, 최대 0.5까지)
    }
}
