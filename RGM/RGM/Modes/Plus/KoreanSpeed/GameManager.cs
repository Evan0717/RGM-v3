using System;
using System.Collections.Generic;
using MEC;
using static RGM.Modes.SpeedStore;

namespace RGM.Modes;

public class GameManager
{
    public static GameManager Instance { get; private set; }
    public ushort Count { get; private set; }
    public event Action CountControlSent;

    public static GameManager GetInstance()
    {
        return IsEnabled switch
        {
            false => null,
            true => Instance ??= new GameManager(),
        };
    }
    
    public void Initialize()
    {
        if (!IsEnabled) return;
        CountControlSent += OnCountControlSent;
    }

    private IEnumerator<float> Counting(ushort amount = 10)
    {
        Count = amount;
        for (ushort i = 0; i < amount; i++)
        {
            yield return Timing.WaitForSeconds(1f);
            Count--;
            CountControlSent?.Invoke();
        }
    }
    
    private void OnCountControlSent()
    {
        
    }
}