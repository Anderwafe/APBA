using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace APBA
{
    /// <summary>
    /// Логика взаимодействия для Equalizer.xaml
    /// </summary>
    public partial class Equalizer : Window
    {
        public Equalizer()
        {
            InitializeComponent();

            Slider[] sliders = { slrFX0, slrFX1, slrFX2, slrFX3, slrFX4, slrFX5, slrFX6, slrFX7, slrFX8, slrFX9};
            Label[] labels = { lblFX0, lblFX1, lblFX2, lblFX3, lblFX4, lblFX5, lblFX6, lblFX7, lblFX8, lblFX9 };

            for(int i = 0; i < sliders.Length; i++)
            {
                sliders[i].Value = EqualizerSettings.FXGain[i];
                labels[i].Content = Math.Round(EqualizerSettings.FXGain[i], 1);
            }

            /*slrFX0.Value = EqualizerSettings.FXGain[0];
            slrFX1.Value = EqualizerSettings.FXGain[1];
            slrFX2.Value = EqualizerSettings.FXGain[2];
            slrFX3.Value = EqualizerSettings.FXGain[3];
            slrFX4.Value = EqualizerSettings.FXGain[4];
            slrFX5.Value = EqualizerSettings.FXGain[5];
            slrFX6.Value = EqualizerSettings.FXGain[6];
            slrFX7.Value = EqualizerSettings.FXGain[7];
            slrFX8.Value = EqualizerSettings.FXGain[8];
            slrFX9.Value = EqualizerSettings.FXGain[9];*/

            btnEqualizerSave.Click += (e, a) =>
            {

            };

            slrFX0.MouseRightButtonDown += (s, e) =>
            {
                slrFX0.Value = 0;
                lblFX0.Content = 0;
                EqualizerSettings.FXGain[0] = 0f;
                EqualizerSettings.ChangeFXParam(0);
            };
            slrFX1.MouseRightButtonDown += (s, e) =>
            {
                slrFX1.Value = 0;
                lblFX1.Content = 0;
                EqualizerSettings.FXGain[1] = 0f;
                EqualizerSettings.ChangeFXParam(1);
            };
            slrFX2.MouseRightButtonDown += (s, e) =>
            {
                slrFX2.Value = 0;
                lblFX2.Content = 0;
                EqualizerSettings.FXGain[2] = 0f;
                EqualizerSettings.ChangeFXParam(2);
            };
            slrFX3.MouseRightButtonDown += (s, e) =>
            {
                slrFX3.Value = 0;
                lblFX3.Content = 0;
                EqualizerSettings.FXGain[3] = 0f;
                EqualizerSettings.ChangeFXParam(3);
            };
            slrFX4.MouseRightButtonDown += (s, e) =>
            {
                slrFX4.Value = 0;
                lblFX4.Content = 0;
                EqualizerSettings.FXGain[4] = 0f;
                EqualizerSettings.ChangeFXParam(4);
            };
            slrFX5.MouseRightButtonDown += (s, e) =>
            {
                slrFX5.Value = 0;
                lblFX5.Content = 0;
                EqualizerSettings.FXGain[5] = 0f;
                EqualizerSettings.ChangeFXParam(5);
            };
            slrFX6.MouseRightButtonDown += (s, e) =>
            {
                slrFX6.Value = 0;
                lblFX6.Content = 0;
                EqualizerSettings.FXGain[6] = 0f;
                EqualizerSettings.ChangeFXParam(6);
            };
            slrFX7.MouseRightButtonDown += (s, e) =>
            {
                slrFX7.Value = 0;
                lblFX7.Content = 0;
                EqualizerSettings.FXGain[7] = 0f;
                EqualizerSettings.ChangeFXParam(7);
            };
            slrFX8.MouseRightButtonDown += (s, e) =>
            {
                slrFX8.Value = 0;
                lblFX8.Content = 0;
                EqualizerSettings.FXGain[8] = 0f;
                EqualizerSettings.ChangeFXParam(8);
            };
            slrFX9.MouseRightButtonDown += (s, e) =>
            {
                slrFX9.Value = 0;
                lblFX9.Content = 0;
                EqualizerSettings.FXGain[9] = 0f;
                EqualizerSettings.ChangeFXParam(9);
            };

            slrFX0.ValueChanged += (s, e) =>
            {
                EqualizerSettings.FXGain[0] = (float)e.NewValue;
                EqualizerSettings.ChangeFXParam(0);
                lblFX0.Content = Math.Round(e.NewValue, 1);
            };
            slrFX1.ValueChanged += (s, e) =>
            {
                EqualizerSettings.FXGain[1] = (float)e.NewValue;
                EqualizerSettings.ChangeFXParam(1);
                lblFX1.Content = Math.Round(e.NewValue, 1);
            };
            slrFX2.ValueChanged += (s, e) =>
            {
                EqualizerSettings.FXGain[2] = (float)e.NewValue;
                EqualizerSettings.ChangeFXParam(2);
                lblFX2.Content = Math.Round(e.NewValue, 1);
            };
            slrFX3.ValueChanged += (s, e) =>
            {
                EqualizerSettings.FXGain[3] = (float)e.NewValue;
                EqualizerSettings.ChangeFXParam(3);
                lblFX3.Content = Math.Round(e.NewValue, 1);
            };
            slrFX4.ValueChanged += (s, e) =>
            {
                EqualizerSettings.FXGain[4] = (float)e.NewValue;
                EqualizerSettings.ChangeFXParam(4);
                lblFX4.Content = Math.Round(e.NewValue, 1);
            };
            slrFX5.ValueChanged += (s, e) =>
            {
                EqualizerSettings.FXGain[5] = (float)e.NewValue;
                EqualizerSettings.ChangeFXParam(5);
                lblFX5.Content = Math.Round(e.NewValue, 1);
            };
            slrFX6.ValueChanged += (s, e) =>
            {
                EqualizerSettings.FXGain[6] = (float)e.NewValue;
                EqualizerSettings.ChangeFXParam(6);
                lblFX6.Content = Math.Round(e.NewValue, 1);
            };
            slrFX7.ValueChanged += (s, e) =>
            {
                EqualizerSettings.FXGain[7] = (float)e.NewValue;
                EqualizerSettings.ChangeFXParam(7);
                lblFX7.Content = Math.Round(e.NewValue, 1);
            };
            slrFX8.ValueChanged += (s, e) =>
            {
                EqualizerSettings.FXGain[8] = (float)e.NewValue;
                EqualizerSettings.ChangeFXParam(8);
                lblFX8.Content = Math.Round(e.NewValue, 1);
            };
            slrFX9.ValueChanged += (s, e) =>
            {
                EqualizerSettings.FXGain[9] = (float)e.NewValue;
                EqualizerSettings.ChangeFXParam(9);
                lblFX9.Content = Math.Round(e.NewValue,1);
            };
        }
    }
}
