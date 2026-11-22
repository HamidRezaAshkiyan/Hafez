using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace HafezWPFUI.Helper.Handlers
{
    /// <summary>
    /// Hardware-accelerated marquee animator using TranslateTransform (GPU-composited)
    /// instead of CPU layout-thrashing ThicknessAnimation.
    /// </summary>
    public static class SmoothMarqueeAnimator
    {
        public static DoubleAnimation StartHorizontalMarquee(
            FrameworkElement element,
            double fromX,
            double toX,
            double pixelsPerSecond,
            bool repeatForever = false,
            EventHandler onCompleted = null)
        {
            if (element.RenderTransform is not TranslateTransform transform)
            {
                transform = new TranslateTransform();
                element.RenderTransform = transform;
            }

            double distance = Math.Abs(toX - fromX);
            double speed = Math.Max(20.0, pixelsPerSecond);
            double durationSeconds = Math.Max(0.5, distance / speed);

            var animation = new DoubleAnimation
            {
                From = fromX,
                To = toX,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                RepeatBehavior = repeatForever ? RepeatBehavior.Forever : new RepeatBehavior(1)
            };

            if (onCompleted != null)
            {
                animation.Completed += onCompleted;
            }

            transform.BeginAnimation(TranslateTransform.XProperty, animation);
            return animation;
        }

        public static void StopMarquee(FrameworkElement element)
        {
            if (element.RenderTransform is TranslateTransform transform)
            {
                transform.BeginAnimation(TranslateTransform.XProperty, null);
            }
        }
    }
}
