using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using UnityEngine;
using MultiBroadcast.API;
using PlayerRoles;
using RGM.API;
using InventorySystem.Items.Usables.Scp330;
using MEC;

using Exiled.API.Features.Items;
using Exiled.API.Enums;
using Exiled.API.Features.Roles;

using static RGM.Modes.ABattleVariables.Abilities;
using static RGM.Modes.ABattleVariables.AbiltiyManagers;
using static RGM.Modes.ABattleVariables.MainManagers;
using static RGM.Modes.ABattleVariables.Cooldowns;
using static RGM.Modes.ABattleVariables.Serials;

using static RGM.Modes.ABattleFunctions.AbilityManagers;
using static RGM.Modes.ABattleFunctions.MainManagers;
using static RGM.Modes.ABattleFunctions.SpecificAbilities;

namespace RGM.Modes.ABattleFunctions
{
    public static class AbilityManagers
    {
        public static string PickAbilityGrade(Player player, string force = null)
        {
            int grade = UnityEngine.Random.Range(1, 10001);
            string abilityGrade;
            if (force != null)
                abilityGrade = "[" + force.Substring(force.IndexOf('[') + 1, force.IndexOf(']') - force.IndexOf('[') - 1) + "]".Trim();

            else if (player.Role.Type == RoleTypeId.Scp079)
                abilityGrade = "[전용]";

            else
            {
                if (grade <= 5) // 0.05%
                    abilityGrade = "[신화]";

                else if (grade <= 40) // 0.35%
                    abilityGrade = "[전설]";

                else if (grade <= 250) // 2.1%
                    abilityGrade = "[영웅]";

                else if (grade <= 1500) // 12.5%
                    abilityGrade = "[희귀]";

                else if (grade <= 2000) // 5%
                    abilityGrade = "[전용]";

                else // 80%
                    abilityGrade = "[일반]";
            }

            return abilityGrade;
        }

        public static Dictionary<string, string> AbilityList(Player player, string abilityGrade, bool get = true)
        {
            if (abilityGrade == "[일반]")
                return CommonAbilities;

            else if (abilityGrade == "[희귀]")
                return RareAbilities;

            else if (abilityGrade == "[영웅]")
            {
                if (get)
                {
                    Cassie.Clear();
                    Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color={RatingColor["영웅"]}>[영웅]</color> 업그레이드를 입수하였습니다.");
                }
                return EpicAbilities;
            }
            else if (abilityGrade == "[전설]")
            {
                if (get)
                {
                    Cassie.Clear();
                    Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color={RatingColor["전설"]}>[전설]</color> 업그레이드를 입수하였습니다.");
                }
                return LegendAbilities;
            }
            else if (abilityGrade == "[신화]")
            {
                if (get)
                {
                    Cassie.Clear();
                    Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color={RatingColor["신화"]}>[신화]</color> 업그레이드를 입수하였습니다.");
                }
                return MythicAbilities;
            }
            else if (abilityGrade == "[전용]")
            {
                Dictionary<string, string> Format()
                {
                    RoleTypeId role = player.Role.Type;

                    if (role == RoleTypeId.ClassD)
                        return ClassDAbilities;

                    else if (role == RoleTypeId.Scientist)
                        return ScientistAbilities;

                    else if (role == RoleTypeId.FacilityGuard)
                        return GuardAbilities;

                    else if (player.IsNTF)
                        return NtfAbilities;

                    else if (player.IsCHI)
                        return ChaosAbilities;

                    else if (role == RoleTypeId.Tutorial)
                        return SnakeAbilities;

                    else if (role == RoleTypeId.Scp173)
                        return Scp173Abilities;

                    else if (role == RoleTypeId.Scp049)
                        return Scp049Abilities;

                    else if (role == RoleTypeId.Scp0492)
                        return Scp0492Abilities;

                    else if (role == RoleTypeId.Scp096)
                        return Scp096Abilities;

                    else if (role == RoleTypeId.Scp106)
                        return Scp106Abilities;

                    else if (role == RoleTypeId.Scp939)
                        return Scp939Abilities;

                    else if (role == RoleTypeId.Scp3114)
                        return Scp3114Abilities;

                    else
                        return Scp079Abilities;
                }

                // Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color={RatingColor["전용"]}>[전용]</color> 업그레이드를 입수하였습니다.");
                return Format();
            }
            else
            {
                Dictionary<string, string> SynergiesFormat = new Dictionary<string, string>();

                foreach (var kvp in Synergies)
                {
                    if (kvp.Value.Count > 0)
                        SynergiesFormat.Add(kvp.Key, kvp.Value[0]);
                }

                Cassie.Clear();
                Server.ExecuteCommand($"/cassie_sl {player.DisplayNickname}(이)가 <color={RatingColor["시너지"]}>[시너지]</color> 효과를 입수하였습니다.");
                return SynergiesFormat;
            }
        }

        public static void ApplyGiveAbility(Player player, string abilityGrade, string abilityName)
        {
            PlayerAbilities[player].Add(abilityName);
            string styleName = ColorFormat(abilityName);

            string Message = $"<size=20>{styleName}</size>\n<size=15>{AbilityList(player, abilityGrade)[abilityName]}</size>";
            player.AddBroadcast(10, Message);
            player.SendConsoleMessage($"\n{Message}", "white");
        }

        public static async void AddAbilityVote(Player player)
        {
            List<string> AbilitesVote = new List<string>();
            List<string> DisplayVote = new List<string>();
            int SelectedAbilityNumber = 0;

            for (int i = 1; i < 4; i++)
            {
                List<string> abilityList = AbilityList(player, PickAbilityGrade(player), false).Keys.ToList();

                AbilitesVote.Add(Tools.GetRandomValue(abilityList));
            }

            for (int i = 1; i < 4; i++)
                DisplayVote.Add($"[{i}] {ColorFormat(AbilitesVote[i - 1])}");

            for (int i = 1; i < 21; i++)
            {
                if (player.IsDead)
                    return;

                player.ShowHint($"<align=left><size=30>{string.Join("\n", DisplayVote)}</size>\n\n<size=25><b>{21 - i}초 안에 [.(번호)] 명령어로 원하는 능력을 선택하세요. (ex .1)</b></size></align>\n\n", 1.2f);

                if (PlayerVotes.ContainsKey(player))
                {
                    SelectedAbilityNumber = int.Parse(PlayerVotes[player]);
                    break;
                }

                await Task.Delay(1000);
            }

            if (SelectedAbilityNumber == 0) SelectedAbilityNumber = UnityEngine.Random.Range(1, 4);

            AddAbility(player, AbilitesVote[SelectedAbilityNumber - 1]);

            if (AbilitesVote.All(x => x == AbilitesVote[0]))
            {
                AddAbility(player, "[시너지] 중복 기연");

                for (int i = 1; i < 3; i++)
                    AddAbility(player, AbilitesVote[0]);
            }
        }

        public static async void AddAbility(Player player, string force = null)
        {
            string abilityGrade = PickAbilityGrade(player, force);
            string abilityName = force == null ? Tools.GetRandomValue(AbilityList(player, abilityGrade).Keys.ToList()) : force;

            ApplyGiveAbility(player, abilityGrade, abilityName);

            string aT = abilityName.Replace("[전용] ", "").Replace("[일반] ", "").Replace("[희귀] ", "").Replace("[영웅] ", "").Replace("[전설] ", "").Replace("[신화] ", "").Replace("[시너지] ", "");

            switch (aT)
            {
                case "운동":
                    float maxHealth = player.MaxHealth;
                    float healthIncrease = maxHealth * 0.25f;
                    player.MaxHealth += healthIncrease;
                    player.Health += healthIncrease;
                    break;
                case "경공": player.GetEffect(EffectType.MovementBoost).Intensity += 10; break;
                case "진화": player.Scale = new Vector3(player.Scale.x - 0.12f, player.Scale.y - 0.12f, player.Scale.z - 0.12f); break;
                case "체력 보충":
                    player.TryAddCandy(CandyKindID.Blue);

                    if (player.IsScp)
                        Server.ExecuteCommand($"/forceeq {player.Id} 42");
                    break;
                case "랜덤박스":
                    List<ItemType> RandomBox = Tools.EnumToList<ItemType>();

                    Item RandomBoxItem = player.AddItem(Tools.GetRandomValue(RandomBox));

                    if (player.IsScp)
                        player.CurrentItem = RandomBoxItem;
                    break;
                case "위치 추적":
                    Player target1 = Tools.GetRandomValue(Player.List.Where(x => x.IsAlive).ToList());

                    for (int i = 1; i < 11; i++)
                    {
                        player.ShowHint($"소속이 <color={target1.Role.Color.ToHex()}>{target1.Role.Name}</color>인 ???은(는) <b>{target1.CurrentRoom.Name}</b>에 있습니다.", 1.2f);
                        await Task.Delay(1000);
                    }
                    break;
                case "뽑기":
                    Item pc = player.AddItem(ItemType.Coin);
                    PickCoinSerials.Add(pc.Serial);

                    if (player.IsScp)
                        player.CurrentItem = pc;
                    break;
                case "보급":
                    List<ItemType> Ammos = new List<ItemType>
                    {
                        ItemType.Ammo12gauge,
                        ItemType.Ammo44cal,
                        ItemType.Ammo9x19,
                        ItemType.Ammo556x45,
                        ItemType.Ammo762x39
                    };

                    for (int i = 1; i < 4; i++)
                        player.AddItem(Tools.GetRandomValue(Ammos));
                    break;
                case "정화":
                    player.TryAddCandy(CandyKindID.Green);

                    if (player.IsScp)
                        Server.ExecuteCommand($"/forceeq {player.Id} 42");
                    break;
                case "신내림":
                    bool Pass = false;

                    for (int i = 1; i < 61; i++)
                    {
                        if (player.CurrentSpectatingPlayers.Count() > 0)
                        {
                            Timing.CallDelayed(1f, () => { player.AddBroadcast(5, $"<size=25>당신은 {player.CurrentSpectatingPlayers.ToList()[0].Nickname}로부터 신내림을 받았습니다.</size>"); });
                            player.CurrentSpectatingPlayers.ToList()[0].AddBroadcast(5, $"<size=25>당신은 {player.Nickname}(에)게 신내림을 내려주었습니다.</size>");
                            Pass = true;
                            break;
                        }

                        await Task.Delay(100);
                    }

                    if (Pass)
                    {
                        for (int i = 1; i < UnityEngine.Random.Range(3, 5); i++)
                            AddAbility(player);
                    }
                    break;
                case "횃불":
                    player.AddItem(ItemType.Lantern);

                    if (player.HasItem(ItemType.SCP330))
                        player.TryAddCandy(CandyKindID.Yellow);

                    else
                    {
                        Scp330Bag yc = new Scp330Bag 
                        {
                            Candies = new List<CandyKindID> { CandyKindID.Yellow }
                        };
                        player.AddItem(yc);
                    }

                    if (player.IsScp)
                        Server.ExecuteCommand($"/forceeq {player.Id} 42");
                    break;
                case "잠행": player.GetEffect(EffectType.SilentWalk).Intensity += 3; break;
                case "위기 탈출":
                    Item ec = player.AddItem(ItemType.Coin);
                    EscapeCoinSerials.Add(ec.Serial);

                    if (player.IsScp)
                        player.CurrentItem = ec;
                    break;
                case "지진":
                    Item ec_1 = player.AddItem(ItemType.Coin);
                    EarthquakeCoinSerials.Add(ec_1.Serial);

                    if (player.IsScp)
                        player.CurrentItem = ec_1;
                    break;
                case "우애":
                    if (Tools.TryGetNearestPlayer(player, out Player nearestPlayer, out float radius))
                    {
                        Item Own = Tools.GetRandomValue(player.Items.ToList());

                        nearestPlayer.AddItem(Own.Type);

                        player.ShowHint($"{nearestPlayer.Nickname}(에)게 {Translations.Item[Own.Type]}(을)를 나누어 주었습니다.");
                        nearestPlayer.ShowHint($"{player.Nickname}(으)로부터 {Translations.Item[Own.Type]}(을)를 나누어 받았습니다.");

                        if (nearestPlayer.IsScp)
                            nearestPlayer.CurrentItem = Own;
                    }
                    break;
                case "무지개":
                    if (player.HasItem(ItemType.SCP330))
                        player.TryAddCandy(CandyKindID.Rainbow);

                    else
                    {
                        Scp330Bag rc = new Scp330Bag
                        {
                            Candies = new List<CandyKindID> { CandyKindID.Rainbow }
                        };
                        player.AddItem(rc);
                    }

                    if (player.IsScp)
                        Server.ExecuteCommand($"/forceeq {player.Id} 42");
                    break;
                case "바디백": player.GetEffect(EffectType.BodyshotReduction).Intensity += 6; break;
                case "강철 껍질": player.GetEffect(EffectType.DamageReduction).Intensity += 10; break;
                case "투명 망토": player.EnableEffect(EffectType.Invisible, 1, 25); break;
                case "순간이동":
                    Item fc = player.AddItem(ItemType.Coin);
                    FollowCoinSerials.Add(fc.Serial);

                    if (player.IsScp)
                        player.CurrentItem = fc;
                    break;
                case "봄버맨":
                    var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, player);
                    g.FuseTime = 3f;
                    g.SpawnActive(Tools.GetRandomValue(Player.List.ToList().Where(x => x.IsAlive && x.Role.Team != player.Role.Team && player != x).ToList()).Position, player);
                    break;
                case "갈고리":
                    Item gc = player.AddItem(ItemType.Coin);
                    GrapCoinSerials.Add(gc.Serial);

                    if (player.IsScp)
                        player.CurrentItem = gc;
                    break;
                case "회중시계":
                    Item cc = player.AddItem(ItemType.Coin);
                    ClockCoinSerials.Add(cc.Serial);

                    if (player.IsScp)
                        player.CurrentItem = cc;
                    break;
                case "스테로이드":
                    player.GetEffect(EffectType.MovementBoost).Intensity += 50;

                    Timing.CallDelayed(25, () =>
                    {
                        if (player.GetEffect(EffectType.MovementBoost).Intensity >= 50)
                            player.GetEffect(EffectType.MovementBoost).Intensity -= 50;

                        else
                            player.GetEffect(EffectType.MovementBoost).Intensity = 0;
                    });
                    break;
                case "하이패스":
                    RGM.Instance.GodModePlayers.Add(player);

                    Timing.CallDelayed(25, () =>
                    {
                        if (RGM.Instance.GodModePlayers.Contains(player))
                            RGM.Instance.GodModePlayers.Remove(player);
                    });
                    break;
                case "트리플악셀":
                    Item COM45 = player.AddItem(ItemType.GunCom45);
                    COM45.As<Firearm>().MaxAmmo /= 2;
                    COM45.As<Firearm>().Ammo = COM45.As<Firearm>().MaxAmmo;

                    player.AddItem(ItemType.Ammo9x19);

                    if (player.IsScp)
                        player.CurrentItem = COM45;
                    break;
                case "연금":
                    for (int i = 1; i < 4; i++)
                    {
                        List<ItemType> Pension = Tools.EnumToList<ItemType>();

                        Item pen = player.AddItem(Tools.GetRandomValue(Pension));

                        if (player.IsScp)
                            player.CurrentItem = pen;

                        await Task.Delay(60 * 1000);
                    }
                    break;
                case "계약":
                    Item cc_1 = player.AddItem(ItemType.Coin);
                    ContractCoinSerials.Add(cc_1.Serial);

                    if (player.IsScp)
                        player.CurrentItem = cc_1;
                    break;
                case "무기 전문가":
                    Item scp1853 = player.AddItem(ItemType.SCP1853);

                    if (player.IsScp)
                        player.CurrentItem = scp1853;
                    break;
                case "만병통치약":
                    Item scp500 = player.AddItem(ItemType.SCP500);

                    if (player.IsScp)
                        player.CurrentItem = scp500;
                    break;
                case "테러리스트의 유품":
                    if (player.HasItem(ItemType.SCP330))
                        player.TryAddCandy(CandyKindID.Pink);

                    else
                    {
                        Scp330Bag _pc = new Scp330Bag
                        {
                            Candies = new List<CandyKindID> { CandyKindID.Pink }
                        };
                        player.AddItem(_pc);
                    }

                    if (player.IsScp)
                        Server.ExecuteCommand($"/forceeq {player.Id} 42");
                    break;
                case "랜덤상자":
                    List<ItemType> RandomChest = new List<ItemType>()
                    {
                        ItemType.ParticleDisruptor,
                        ItemType.Jailbird,
                        ItemType.MicroHID,
                        ItemType.SCP018,
                        ItemType.SCP1576,
                        ItemType.SCP2176,
                        ItemType.SCP207,
                        ItemType.AntiSCP207,
                        ItemType.SCP268,
                        ItemType.SCP500,
                        ItemType.KeycardO5
                    };

                    for (int i = 1; i < 3; i++)
                    {
                        Item RandomChestItem = player.AddItem(Tools.GetRandomValue(RandomChest));

                        if (player.IsScp)
                            player.CurrentItem = RandomChestItem;
                    }
                    break;
                case "럭키비키": PlayerWorkstation[player].Clear(); break;
                case "슈퍼 스타": Server.ExecuteCommand($"/speak {player.Id} enable"); break;
                case "초재생":
                    Item AntiScp207 = player.AddItem(ItemType.AntiSCP207);

                    if (player.IsScp)
                        player.CurrentItem = AntiScp207;
                    break;
                case "고스트룰": player.GetEffect(EffectType.Ghostly).Intensity += 1; break;
                case "잠수부": player.EnableEffect(EffectType.Invigorated); break;
                case "스피드왜건": player.GetEffect(EffectType.MovementBoost).Intensity += 100; break;
                case "뱀의 손 무전기":
                    Item SH = player.AddItem(ItemType.Radio);
                    CallSnakeHandsSerials.Add(SH.Serial);

                    if (player.IsScp)
                        player.CurrentItem = SH;
                    break;
                case "랜덤택배":
                    List<ItemType> ItemTypes = Tools.EnumToList<ItemType>();

                    for (int i = 1; i < Server.PlayerCount + 1; i++)
                    {
                        Item Item = Item.Create(Tools.GetRandomValue(ItemTypes));

                        Item.CreatePickup(new Vector3(player.Position.x, player.Position.y + 2, player.Position.z));
                    }
                    break;
                case "플래시라이트":
                    Item fl = player.AddItem(ItemType.Flashlight);
                    FlashLightSerials.Add(fl.Serial);

                    if (player.IsScp)
                        player.CurrentItem = fl;
                    break;
                case "화염 방사기":
                    Item ft = player.AddItem(ItemType.MicroHID);
                    FlamethrowerSerials.Add(ft.Serial);

                    if (player.IsScp)
                        player.CurrentItem = ft;
                    break;
                case "주거칩입죄": player.AddItem(ItemType.SCP268); break;
                case "반란의 씨앗": Respawn.ChaosTickets += (int)(Player.List.Count() * 0.2); break;
                case "05 평의회": player.AddItem(ItemType.KeycardO5); break;
                case "공학 전공": player.AddItem(ItemType.SCP2176); break;
                case "특수부대의 씨앗": Respawn.NtfTickets += (int)(Player.List.Count() * 0.2); break;
                case "관리 의무자":
                    List<ItemType> ManageDuty = new List<ItemType>()
                    {
                        ItemType.GunCrossvec,
                        ItemType.Flashlight,
                        ItemType.Ammo9x19,
                        ItemType.Ammo9x19
                    };

                    foreach (var item in ManageDuty)
                        player.AddItem(item);
                    break;
                case "보건소 직원":
                    List<ItemType> HealItem = new List<ItemType>()
                    {
                        ItemType.Medkit,
                        ItemType.Painkillers,
                        ItemType.Adrenaline,
                        ItemType.SCP500,
                        ItemType.SCP330
                    };

                    foreach (var team in Player.List.Where(x => !x.IsNPC && x.IsAlive && x.LeadingTeam == player.LeadingTeam && Vector3.Distance(player.Position, x.Position) < 11))
                        team.AddItem(Tools.GetRandomValue(HealItem));

                    break;
                case "산업재해보험":
                    for (int i = 1; i < 4; i++)
                        AddAbility(player, "[일반] 보험");

                    break;
                case "격리 의무자":
                    List<ItemType> ContainDuty = new List<ItemType>()
                    {
                        ItemType.GrenadeFlash,
                        ItemType.GrenadeHE
                    };

                    foreach (var item in ContainDuty)
                        player.AddItem(item);
                    break;
                case "레이더":
                    Item rd = player.AddItem(ItemType.Radio);
                    RadarSerials.Add(rd.Serial);

                    if (player.IsScp)
                        player.CurrentItem = rd;
                    break;
                case "혼돈의 카오스":
                    Item c = player.AddItem(ItemType.SCP018);

                    if (player.IsScp)
                        player.CurrentItem = c;
                    break;
                case "혼돈의 손길":
                    Item Ch = player.AddItem(ItemType.Coin);
                    ChaosCoinSerials.Add(Ch.Serial);

                    if (player.IsScp)
                        player.CurrentItem = Ch;
                    break;
                case "혼돈의 가방":
                    List<ItemType> ChaosBag = Tools.EnumToList<ItemType>();

                    foreach (var cb in player.Items)
                    {
                        if (!cb.IsAmmo)
                        {
                            player.RemoveItem(cb);
                            player.AddItem(Tools.GetRandomValue(ChaosBag));
                        }
                    }
                    break;
                case "세치 혀":
                    Item Scp1576 = player.AddItem(ItemType.SCP1576);

                    if (player.IsScp)
                        player.CurrentItem = Scp1576;
                    break;
                case "제3세력":
                    List<Player> DeadPlayers = Player.List.Where(x => x.IsDead).ToList();
                    DeadPlayers.ShuffleList();

                    CallSnakeHand(player, DeadPlayers.Take(2).ToList());
                    break;
                case "SCP 연구자":
                    List<ItemType> SCPItems = new List<ItemType>()
                    {
                        ItemType.SCP018,
                        ItemType.SCP1576,
                        ItemType.SCP1853,
                        ItemType.SCP207,
                        ItemType.SCP2176,
                        ItemType.SCP244a,
                        ItemType.SCP244b,
                        ItemType.SCP268,
                        ItemType.SCP330,
                        ItemType.SCP500,
                        ItemType.AntiSCP207
                    };

                    Item SCPItem = player.AddItem(Tools.GetRandomValue(SCPItems));

                    if (player.IsScp)
                        player.CurrentItem = SCPItem;
                    break;
                case "능수능란":
                    if (player.Role is Scp049Role Scp049_2)
                        Scp049_2.GoodSenseCooldown /= 2;
                    break;
                case "급식":
                    player.MaxHealth = player.MaxHealth * 1.5f;
                    break;
                case "원수":
                    if (player.Role is Scp096Role Scp096)
                        Scp096.ChargeCooldown /= 2;
                    break;
                case "흉내쟁이":
                    if (player.Role is Scp939Role Scp939)
                        Scp939.MimicryCooldown = 0;
                    break;
                case "안아줘요":
                    if (player.Role is Scp939Role Scp939_1)
                        Scp939_1.AmnesticCloudCooldown /= 2;
                    break;
                case "민첩한 사냥 도구":
                    if (player.Role is Scp939Role Scp939_2)
                        Scp939_2.AttackCooldown /= 2;
                    break;
                case "간이 충전기":
                    if (player.Role is Scp079Role scp079)
                        scp079.Experience += 20;
                    break;
                case "과전류":
                    for (int i = 1; i < 21; i++)
                    {
                        if (player.Role is Scp079Role scp0791)
                            scp0791.Energy = scp0791.MaxEnergy;

                        await Task.Delay(1000);
                    }
                    break;
                case "생존 전문가":
                    player.Health = player.Health * 5;

                    if (player.MaxHealth < player.Health)
                        player.MaxHealth = player.Health;
                    break;
                case "랜덤 컬렉션":
                    List<string> Randoms = new List<string>()
                    {
                        "[일반] 랜덤박스",
                        "[영웅] 랜덤상자",
                        "[전설] 랜덤택배"
                    };

                    AddAbility(player, Tools.GetRandomValue(Randoms));
                    break;
                case "4대 운동":
                    AddAbilityVote(player);
                    break;
            }
        }
    }
}
