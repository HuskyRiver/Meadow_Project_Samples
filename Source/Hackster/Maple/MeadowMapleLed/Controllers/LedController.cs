﻿using Meadow;
using Meadow.Foundation.Leds;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowMapleLed.Controller
{
    public class LedController
    {
        private static readonly Lazy<LedController> instance =
            new Lazy<LedController>(() => new LedController());
        public static LedController Instance => instance.Value;

        RgbPwmLed rgbPwmLed;

        Task animationTask = null;
        CancellationTokenSource cancellationTokenSource = null;

        private LedController()
        {
            Initialize();
        }

        private void Initialize()
        {
            rgbPwmLed = new RgbPwmLed(
                redPwmPin: MeadowApp.Device.Pins.D12,
                greenPwmPin: MeadowApp.Device.Pins.D11,
                bluePwmPin: MeadowApp.Device.Pins.D10);
        }

        void Stop()
        {
            rgbPwmLed.StopAnimation();
            cancellationTokenSource?.Cancel();
        }

        public void SetColor(Color color)
        {
            Stop();
            rgbPwmLed.SetColor(color);
        }

        public void TurnOn()
        {
            Stop();
            rgbPwmLed.SetColor(GetRandomColor());
            rgbPwmLed.IsOn = true;
        }

        public void TurnOff()
        {
            Stop();
            rgbPwmLed.IsOn = false;
        }

        public void StartBlink()
        {
            Stop();
            rgbPwmLed.StartBlink(GetRandomColor());
        }

        public void StartPulse()
        {
            Stop();
            rgbPwmLed.StartPulse(GetRandomColor());
        }

        public void StartRunningColors()
        {
            rgbPwmLed.StopAnimation();

            animationTask = new Task(async () =>
            {
                cancellationTokenSource = new CancellationTokenSource();
                await StartRunningColors(cancellationTokenSource.Token);
            });
            animationTask.Start();
        }

        protected async Task StartRunningColors(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                rgbPwmLed.SetColor(GetRandomColor());
                await Task.Delay(1000);
            }
        }

        protected Color GetRandomColor()
        {
            var random = new Random();
            return Color.FromHsba(random.NextDouble(), 1, 1);
        }
    }
}