using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace EkseemMaarNet.Controls;

public partial class FadeHost
{
    private CAGradientLayer? _mask;

    // A gradient mask on the host's layer: alpha ramps from (1 - amount) at the top edge to fully
    // opaque at FadeHeight, so content becomes see-through as it reaches the header.
    partial void UpdateNative()
    {
        if (Handler?.PlatformView is not UIView view)
            return;

        var width = Width;
        var height = Height;
        if (FadeAmount <= 0.01 || width <= 0 || height <= 0)
        {
            view.Layer.Mask = null;
            return;
        }

        CATransaction.Begin();
        CATransaction.DisableActions = true;

        _mask ??= new CAGradientLayer();
        _mask.Frame = new CGRect(0, 0, width, height);
        _mask.Colors =
        [
            UIColor.Black.ColorWithAlpha((nfloat)(1 - FadeAmount)).CGColor,
            UIColor.Black.CGColor,
            UIColor.Black.CGColor
        ];
        _mask.Locations =
        [
            NSNumber.FromDouble(0),
            NSNumber.FromDouble(Math.Min(1, FadeHeight / height)),
            NSNumber.FromDouble(1)
        ];
        view.Layer.Mask = _mask;

        CATransaction.Commit();
    }
}
