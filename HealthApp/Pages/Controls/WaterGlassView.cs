namespace HealthApp.Controls
{
    public class WaterGlassView : GraphicsView
    {
        public static readonly BindableProperty ProgressProperty =
            BindableProperty.Create(nameof(Progress), typeof(double), typeof(WaterGlassView), 0.0d,
                propertyChanged: (bindable, oldValue, newValue) => (bindable as WaterGlassView)?.Invalidate());

        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public WaterGlassView()
        {
            Drawable = new WaterGlassDrawable(this); // Przekazujemy referencję do WaterGlassView
        }

        private class WaterGlassDrawable : IDrawable
        {
            private readonly WaterGlassView _parent;

            public WaterGlassDrawable(WaterGlassView parent)
            {
                _parent = parent;
            }

            public void Draw(ICanvas canvas, RectF dirtyRect)
            {
                // Tło szklanki
                canvas.StrokeColor = Colors.White;
                canvas.StrokeSize = 3;
                canvas.DrawRoundedRectangle(10, 10, dirtyRect.Width - 20, dirtyRect.Height - 20, 10);

                // Woda (poprawione odwołanie do Progress)
                var waterHeight = dirtyRect.Height * (float)_parent.Progress;
                canvas.FillColor = Color.FromArgb("#4FC3F7"); // Poprawny kolor
                canvas.FillRoundedRectangle(12, dirtyRect.Height - waterHeight - 12, dirtyRect.Width - 24, waterHeight, 8);
            }
        }
    }
}
