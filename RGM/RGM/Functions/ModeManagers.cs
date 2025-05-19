using ProjectMER.Features.Objects;
using RGM.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API.Features;

using static RGM.Variables.ServerManagers;
using RGM.API.DataBases;
using LabApi.Features.Wrappers;

namespace RGM.Functions
{
    public static class ModeManagers
    {
        public static void PickModes()
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
                foreach (var Pad in Pads[i])
                    Pad.GetComponent<PrimitiveObjectToy>().Color = ColorUtility.TryParseHtmlString("#" + ModeList[ModeVote.Keys.ToList()[i]].Color, out Color color) ? color : Color.white;
            }

            Color randomColor = Tools.GetRandomColor(true);

            Numbers.ForEach(x => x.GetComponent<PrimitiveObjectToy>().Color = randomColor);
            RandomColors.ForEach(x => x.GetComponent<PrimitiveObjectToy>().Color = randomColor);
            RandomLights.ForEach(x => x.GetComponent<Light>().color = Tools.GetRandomColor());
            Balls.ForEach(x => x.GetComponent<PrimitiveObjectToy>().Color = Tools.GetRandomColor(true));
        }
    }
}
