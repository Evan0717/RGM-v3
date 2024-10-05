using Exiled.API.Features.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;
using Exiled.API.Features.Roles;
using Exiled.API.Enums;
using PlayerRoles;
using MultiBroadcast.API;
using RGM.API;
using Mirror;

namespace RGM.Modes
{
    public class HotPotato
    {
        public static HotPotato Instance;

        public List<Player> pl = new List<Player>();
        public List<Player> BomberMans = new List<Player>();

        ReferenceHub dj;

        public void OnEnabled()
        {
            Round.IsLocked = true;
            Respawn.TimeUntilNextPhase = 10000;

            Timing.RunCoroutine(OnModeStarted());

            Exiled.Events.Handlers.Player.Hurting += OnHurting;
        }

        public IEnumerator<float> OnModeStarted()
        {
            Server.ExecuteCommand($"/mp load hp");

            Player.List.CopyTo(pl);

            dj = GGUtils.Gtool.Spawn(RoleTypeId.Tutorial, new Vector3(82.51834f, 1014.692f, -50.10588f));

            Dictionary<ReferenceHub, string> register = new Dictionary<ReferenceHub, string>()
            {
                { dj, "dj" }
            };

            foreach (var reg in register)
            {
                try
                {
                    GGUtils.Gtool.Register(reg.Key, reg.Value);
                }
                catch
                {
                }
            }

            GGUtils.Gtool.PlayerGet("dj").DisplayNickname = "DJ";
            GGUtils.Gtool.PlaySound("dj", "Skeleton", VoiceChat.VoiceChatChannel.Intercom, 25, true);

            Timing.RunCoroutine(DJHeadBanging());

            foreach (var player in Player.List.Where(x => !x.IsNPC))
            {
                player.Role.Set(RoleTypeId.Scientist);
                player.Position = new Vector3(83.91287f, 1014.692f, -37.13322f);
                player.ClearInventory();
            }

            while (true)
            {
                for (int i = 1; i < pl.Count / 10 + 2; i++)
                {
                    Player BomberMan = Tools.GetRandomValue(pl.Where(x => !BomberMans.Contains(x)).ToList());

                    BomberMan.Role.Set(RoleTypeId.Scp049, SpawnReason.ForceClass, RoleSpawnFlags.None);
                    BomberMans.Add(BomberMan);
                }

                for (int i = 1; i < 11; i++)
                {
                    Player.List.ToList().ForEach(x => x.ShowHint($"현재 폭탄 : {string.Join(" & ", BomberMans.Select(b => b.Nickname))}\n{11 - i}초 후 폭탄이 터집니다.", 1.2f));

                    yield return Timing.WaitForSeconds(1f);
                }

                foreach (var bomber in BomberMans)
                {
                    if (pl.Contains(bomber))
                    {
                        pl.Remove(bomber);

                        if (pl.Count < 2)
                            Round.IsLocked = false;

                        bomber.Role.Set(RoleTypeId.ClassD);
                        bomber.Position = new Vector3(83.82303f, 1026.691f, -37.06291f);

                        var g = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE, Server.Host);
                        g.FuseTime = 0;
                        g.SpawnActive(bomber.Position, Server.Host);
                    }
                }
            }
        }

        public IEnumerator<float> DJHeadBanging()
        {
            yield return Timing.WaitForSeconds(1f);

            bool HeadUp = true;

            while (true)
            {
                if (HeadUp)
                {
                    GGUtils.Gtool.Rotate(dj, new Vector3(0, -1f, 0));

                    HeadUp = false;

                    yield return Timing.WaitForSeconds(0.2f);
                }
                else
                {
                    GGUtils.Gtool.Rotate(dj, new Vector3(0, 1f, 0));

                    HeadUp = true;

                    yield return Timing.WaitForSeconds(0.15f);
                }
            }
        }

        public void OnHurting(Exiled.Events.EventArgs.Player.HurtingEventArgs ev)
        {
            ev.IsAllowed = false;

            if (BomberMans.Contains(ev.Attacker) && !BomberMans.Contains(ev.Player))
            {
                BomberMans.Remove(ev.Attacker);
                BomberMans.Add(ev.Player);

                ev.Attacker.Role.Set(RoleTypeId.Scientist, SpawnReason.ForceClass, RoleSpawnFlags.None);
                ev.Player.Role.Set(RoleTypeId.Scp049, SpawnReason.ForceClass, RoleSpawnFlags.None);
            }
        }
    }
}
