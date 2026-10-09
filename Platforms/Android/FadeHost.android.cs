using Android.Graphics;
using Android.Graphics.Drawables;
using AView = Android.Views.View;
using AColor = Android.Graphics.Color;
using APaint = Android.Graphics.Paint;

namespace EkseemMaarNet.Controls;

public partial class FadeHost
{
    private TopFadeDrawable? _fade;

    // The host renders into its own hardware layer; a foreground drawable then "erases" (DST_OUT)
    // a gradient band along the top, leaving the content truly transparent there.
    partial void UpdateNative()
    {
        if (Handler?.PlatformView is not AView view)
            return;

        if (_fade is null)
        {
            _fade = new TopFadeDrawable();
            view.SetLayerType(Android.Views.LayerType.Hardware, null);
            view.Foreground = _fade;
        }

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

        public void Update(float height, float amount)
        {
            _height = height;
            _amount = amount;
            InvalidateSelf();
        }

        public override void Draw(Canvas canvas)
        {
            if (_amount <= 0.01f || _height <= 0)
                return;

            var erase = AColor.Argb((int)(255 * _amount), 0, 0, 0);
            _paint.SetShader(new LinearGradient(0, 0, 0, _height, erase, AColor.Transparent, Shader.TileMode.Clamp!));
            canvas.DrawRect(0, 0, Bounds.Width(), _height, _paint);
        }

        public override void SetAlpha(int alpha) { }
        public override void SetColorFilter(ColorFilter? colorFilter) { }

#pragma warning disable CS0672 // Opacity is deprecated upstream but still abstract, so it must be overridden.
        public override int Opacity => (int)Format.Translucent;
#pragma warning restore CS0672
    }
}
