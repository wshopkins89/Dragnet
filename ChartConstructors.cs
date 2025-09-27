using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace DragnetControl
{
    public static class ChartConstructor
    {
        public static ColumnSeries<double> BuildCpuSeries(ObservableCollection<double> cpuUsage)
        {
            var barGradient = new LinearGradientPaint(
                new SKColor[] { new SKColor(36, 242, 255), new SKColor(0, 100, 180) },
                new SKPoint(0, 0),
                new SKPoint(0, 1),
                new float[] { 0f, 1f });

            return new ColumnSeries<double>
            {
                Values = cpuUsage,
                Name = "CPU Core Usage",
                Fill = barGradient,
                MaxBarWidth = 32
            };
        }

        public static Axis[] BuildCpuXAxis(int coreCount)
        {
            return new[] {
                new Axis
                {
                    Labels = Enumerable.Range(1, coreCount).Select(i => $"Core {i}").ToArray(),
                    LabelsPaint = new SolidColorPaint
                    {
                        Color = SKColors.White,
                        SKTypeface = SKTypeface.FromFamilyName("Audiowide")
                    },
                    TextSize = 13
                }
            };
        }

        public static Axis[] BuildCpuYAxis()
        {
            return new[] {
                new Axis
                {
                    MinLimit = 0,
                    MaxLimit = 100,
                    Name = "%",
                    LabelsPaint = new SolidColorPaint
                    {
                        Color = SKColors.White,
                        SKTypeface = SKTypeface.FromFamilyName("Audiowide")
                    },
                    TextSize = 13
                }
            };
        }

        public static PieSeries<double> BuildDiskUsedSeries(ObservableCollection<double> usedValues)
        {
            var usedGradient = new RadialGradientPaint(
                new SKColor[] { new SKColor(255, 35, 35), new SKColor(128, 0, 0) },
                new SKPoint(0.5f, 0.5f),
                0.5f,
                new float[] { 0f, 1f });

            return new PieSeries<double>
            {
                Values = usedValues,
                Name = "Used",
                Fill = usedGradient
            };
        }

        public static PieSeries<double> BuildDiskFreeSeries(ObservableCollection<double> freeValues)
        {
            var freeGradient = new RadialGradientPaint(
                new SKColor[] { new SKColor(36, 242, 255), new SKColor(0, 100, 180) },
                new SKPoint(0.5f, 0.5f),
                0.5f,
                new float[] { 0f, 1f });

            return new PieSeries<double>
            {
                Values = freeValues,
                Name = "Free",
                Fill = freeGradient
            };
        }

        public static LineSeries<double> BuildRamSeries(ObservableCollection<double> ramHistory)
        {
            var ramGradient = new LinearGradientPaint(
                new SKColor[] { new SKColor(255, 35, 35, 100), new SKColor(128, 0, 0, 180) },
                new SKPoint(0, 0),
                new SKPoint(0, 1));

            return new LineSeries<double>
            {
                Values = ramHistory,
                GeometrySize = 0,
                Fill = ramGradient,
                LineSmoothness = 0.5,
                Stroke = new SolidColorPaint(new SKColor(255, 35, 35), 2),
                Name = "RAM Usage (%)"
            };
        }

        public static Axis[] BuildRamXAxis()
        {
            return new[] {
                new Axis
                {
                    IsVisible = false,
                    LabelsPaint = new SolidColorPaint
                    {
                        Color = SKColors.White,
                        SKTypeface = SKTypeface.FromFamilyName("Audiowide")
                    },
                    TextSize = 13
                }
            };
        }

        public static Axis[] BuildRamYAxis()
        {
            return new[] {
                new Axis
                {
                    MinLimit = 0,
                    MaxLimit = 100,
                    Name = "%",
                    LabelsPaint = new SolidColorPaint
                    {
                        Color = SKColors.White,
                        SKTypeface = SKTypeface.FromFamilyName("Audiowide")
                    },
                    TextSize = 13
                }
            };
        }

    }
}
