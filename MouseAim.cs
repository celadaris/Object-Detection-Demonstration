using System;
using System.Numerics;
using Yolov7net;

namespace Bo3AimBot
{
    internal class MouseAim
    {
        public static Task AimingTask { get; set; }
        public static CancellationTokenSource tokenSource { get; set; }
        public static CancellationToken ct { get; set; }

        public static void AimAtTarget()
        {
            tokenSource = new CancellationTokenSource();
            ct = tokenSource.Token;

            AimingTask = Task.Run(() =>
            {
                while (!ct.IsCancellationRequested)
                {
                    Thread.Sleep(1);
                    switch (YoloCode.predictCount)
                    {
                        case 0:
                            if (YoloCode.aimPrediction3.Count > 0)
                            {
                                YoloPrediction closestTarget = FindClosestTarget(YoloCode.aimPrediction3);
                                MoveMouse(closestTarget);
                            }
                            break;
                        case 1:
                            if (YoloCode.aimPrediction1.Count > 0)
                            {
                                YoloPrediction closestTarget = FindClosestTarget(YoloCode.aimPrediction1);
                                MoveMouse(closestTarget);
                            }
                            break;
                        case 2:
                            if (YoloCode.aimPrediction2.Count > 0)
                            {
                                YoloPrediction closestTarget = FindClosestTarget(YoloCode.aimPrediction2);
                                MoveMouse(closestTarget);
                            }
                            break;
                    }
                }
            }, ct);

            AimingTask.ContinueWith(t => { Console.WriteLine(t.Exception); },
        TaskContinuationOptions.OnlyOnFaulted);


        }

        static YoloPrediction FindClosestTarget(List<YoloPrediction> predictions)
        {
            double closestTarget = double.MaxValue;
            YoloPrediction targetInfo = new YoloPrediction();

            predictions.FindAll(x => String.Equals(x.Label.Name, "Zombie Head")).ForEach(x =>
            {
                Vector2 targetCenter = GetCenter(x.Rectangle);
                double distance = GetDistance(new Vector2(960, 540), targetCenter);

                if (distance <= closestTarget)
                {
                    closestTarget = distance;
                    targetInfo = x;
                }
            });

            return targetInfo;
        }

        static void MoveMouse(YoloPrediction closestTarget)
        {
            if (closestTarget.Label == null)
            {
                return;
            }

            if (String.Equals(closestTarget.Label.Name, "Zombie Head"))
            {
                Vector2 enemyPos = GetCenter(closestTarget.Rectangle);
                enemyPos = Vector2.Normalize(enemyPos);
                WinAPI.Move((int)Math.Round(enemyPos.X), (int)Math.Round(enemyPos.Y));
            }
        }

        static Vector2 GetCenter(RectangleF box)
        {
            return new Vector2((box.X + (box.Width / 2)) - 960, (box.Y + (box.Height / 2)) - 540);
        }

        static double GetDistance(Vector2 center, Vector2 target)
        {
            //distance formula
            return Math.Sqrt(Math.Pow((target.X - center.X), 2) + Math.Pow((target.Y - center.Y), 2));
        }
    }
}
