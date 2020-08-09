using IMBA;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;

namespace APBA
{
    static class EqualizerSettings
    {
        static private BASS_DX8_PARAMEQ FX = new BASS_DX8_PARAMEQ();
        static private int[] fx = new int[10];
        static public float[] FXGain = new float[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        static private float[] FXCenter = new float[10] { 80, 170, 310, 600, 1000, 3000, 6000, 10000, 12000, 14000 };
        static public string SettingsPath = Environment.CurrentDirectory + $@"\EqualizerProfiles.ini";

        static public void SetFX(in int stream)
        {
            for (int i = 0; i < 10; i++)
            {
                fx[i] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 1);
                FX.fGain = FXGain[i];
                FX.fBandwidth = 18;
                FX.fCenter = FXCenter[i];
                Bass.BASS_FXSetParameters(fx[i], FX);
            }
        }

        static public void LoadPreset(string preset)
        {
            IniReader IR = new IniReader(SettingsPath);
            FXGain = IR.GetValuebyParam(preset, true).Select(x => Convert.ToSingle(x)).ToArray();
            for (int i = 0; i < 10; i++)
                ChangeFXParam(i);
        }

        static public void ChangeFXParam(in int FXParam)
        {
            //fx[FXParam] = Bass.BASS_ChannelSetFX(stream, BASSFXType.BASS_FX_DX8_PARAMEQ, 1);
            FX.fGain = FXGain[FXParam];
            FX.fBandwidth = 8;
            FX.fCenter = FXCenter[FXParam];
            Bass.BASS_FXSetParameters(fx[FXParam], FX);
        }
    }
}
