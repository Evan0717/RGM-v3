namespace RGM.Modes.PveExiledSystem
{
    public abstract class SpecialWave
    {
        public bool Ended = false;
        public abstract string SpecialWaveName { get; }
        public abstract string SoundtrackName { get; }
        public abstract void Enable(RoundHandler roundHandler, WaveConfig waveConfig, WaveConfig.WaveInfo waveInfo);
        public abstract void Disable();
    }
}