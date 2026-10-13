using Android.Graphics;
using Android.Graphics.Drawables;
using AView = Android.Views.View;
using AColor = Android.Graphics.Color;
using APaint = Android.Graphics.Paint;

namespace EkseemMaarNet.Controls;

public partial class FadeHost
{
    private TopFadeDrawable? _fade;

    // While fading, the host renders into its own hardware layer and a foreground drawable "erases" (DST_OUT)
    // a gradient band along the top, leaving the content truly transparent there.
    partial void UpdateNative()
    {
        if (Handler?.PlatformView is not AView view)
            return;

        if (_fade is null)
        {
            _fade = new TopFadeDrawable();
            view.Foreground = _fade;
        }

        // The offscreen layer is only needed while something is being erased; at rest it's wasted memory.
        var layer = FadeAmount > 0.01 ? Android.Views.LayerType.Hardware : Android.Views.LayerType.None;
        if (view.LayerType != layer)
            view.SetLayerType(layer, null);

        var density = view.Resources?.DisplayMetrics?.Density ?? 1f;
        _fade.Update((float)(FadeHeight * density), (float)FadeAmount);
        view.Invalidate();
    }

    private sealed class TopFadeDrawable : Drawable
    {
        private readonly APaint _paint = new() { AntiAlias = true };
        private float _height;
        private float _amount;

        public TopFadeDrawable() => _paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.DstOut!));

        // The gradient only depends on height and amount, so build it here rather than on every frame.
        public void Update(float height, float amount)
        {
            if (height == _height && amount == _amount)
                return;

            _height = height;
            _amount = amount;
            var erase = AColor.Argb((int)(255 * amount), 0, 0, 0);
            _paint.SetShader(height > 0 ? new LinearGradient(0, 0, 0, height, erase, AColor.Transparent, Shader.TileMode.Clamp!) : null);
            InvalidateSelf();
        }

        public override void Draw(Canvas canvas)
        {
            if (_amount <= 0.01f || _height <= 0)
                return;

            canvas.DrawRect(0, 0, Bounds.Width(), _height, _paint);
        }

        public override void SetAlpha(int alpha) { }
        public override void SetColorFilter(ColorFilter? colorFilter) { }

#pragma warning disable CS0672 // Opacity is deprecated upstream but still abstract, so it must be overridden.
        public override int Opacity => (int)Format.Translucent;
#pragma warning restore CS0672
    }
}
