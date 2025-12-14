using ProjectMER.Features.Objects;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features;

using static RGM.Variables.Variable;
using RGM.API.DataBases;
using LabApi.Features.Wrappers;
using Exiled.API.Features.Toys;
using ProjectMER.Features;

namespace RGM.Functions
{
    public static class ModeManagers
    {
        public static void PickModes()
        {
            try
            {
                ModeVote.Clear();
                SubModeVote.Clear();

                for (int i = 1; i < 5; i++)
                {
                    var StaticModeList = ModeList.Keys.Where(x => ModeList[x].Category == ModeCategory.Public && !ModeVote.ContainsKey(x)).ToList();
                    var mode = StaticModeList[UnityEngine.Random.Range(0, StaticModeList.Count())];
                    ModeVote.Add(mode, new List<Exiled.API.Features.Player>());

                    if (UnityEngine.Random.Range(1, 11) == 1)
                        SubModeVote.Add(Tools.GetRandomValue(ModeList.Keys.Where(x => ModeList[x].Category != ModeCategory.Private && !ModeVote.ContainsKey(x) && ModeList.Keys.Where(x => x.GetModeData().Info != ModeInfo.Set).Contains(x)).ToList()));

                    else
                        SubModeVote.Add(ModeType.None);
                }
                List<List<Transform>> Pads = new List<List<Transform>>() { First, Second, Third, Fourth };

                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        foreach (var Pad in Pads[i])
                            Pad.GetComponent<AdminToys.PrimitiveObjectToy>().NetworkMaterialColor = ColorUtility.TryParseHtmlString("#" + ModeList[ModeVote.Keys.ToList()[i]].Color, out Color color) ? color : Color.white;
                    }
                    catch (Exception e) { }
                }

                Color randomColor = Tools.GetRandomColor(true);

                Numbers.ForEach(x => x.GetComponent<AdminToys.PrimitiveObjectToy>().NetworkMaterialColor = randomColor);
                RandomColors.ForEach(x => x.GetComponent<AdminToys.PrimitiveObjectToy>().NetworkMaterialColor = randomColor);
                RandomLights.ForEach(x => x.GetComponent<AdminToys.LightSourceToy>().NetworkLightColor = Tools.GetRandomColor());
                Balls.ForEach(x => x.GetComponent<AdminToys.PrimitiveObjectToy>().NetworkMaterialColor = Tools.GetRandomColor(true));
            }
            catch (Exception e)
            {
                Log.Error($"[RGM] {e}");
            }
        }
    }
}
