using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RGM.Modes
{
    public class Synergy
    {
        public void OnEnabled()
        {
            Timing.RunCoroutine(OnStarted());
        }

        public IEnumerator<float> OnStarted()
        {
            yield return 0f;
        }
    }
}
