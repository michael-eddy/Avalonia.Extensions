using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace Avalonia.Extensions.Controls
{
    public abstract class Adorner : InputElement
    {
        private bool _isClipEnabled;

        /// <summary>
        /// Constructor
        /// </summary>
        protected Adorner(Control adornedControl)
        {
            AdornedElement = adornedControl ?? throw new ArgumentNullException("adornedElement");
            _isClipEnabled = false;
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                //Adorner adorner = this;
                //Binding binding = new Binding("FlowDirection")
                //{
                //    Mode = BindingMode.OneWay,
                //    Source = adorner.AdornedElement
                //};
                //adorner.Bind(FlowDirectionProperty, binding);
            }, DispatcherPriority.Normal);
        }
        protected override Size MeasureOverride(Size constraint)
        {
            Size desiredSize;
            desiredSize = new Size(AdornedElement.Bounds.Width, AdornedElement.Bounds.Height);
            int count = VisualChildren.Count;
            for (int i = 0; i < count; i++)
            {
                if (VisualChildren.ElementAt(i) is Control ch)
                    ch.Measure(desiredSize);
            }
            return desiredSize;
        }
        /// <summary>
        /// Says if the Adorner needs update based on the 
        /// previously cached size if the AdornedElement.
        /// </summary>
        internal virtual bool NeedsUpdate(Size oldSize)
        {
            return !ControlUtils.AreClose(AdornedElement.DesiredSize, oldSize);
        }
        /// <summary>
        /// UIElement this Adorner adorns.
        /// </summary>
        public Control AdornedElement { get; }
        /// <summary>
        /// If set to true, the adorner will be clipped using the same clip geometry as the
        /// AdornedElement.  This is expensive, and therefore should not normally be used.
        /// Defaults to false.
        /// </summary>
        public bool IsClipEnabled
        {
            get => _isClipEnabled;
            set
            {
                _isClipEnabled = value;
                InvalidateArrange();
                AdornerLayer.GetAdornerLayer(AdornedElement).InvalidateArrange();
            }
        }
        /// <summary>
        /// Gets or sets the clip of this Visual.
        /// Needed by AdornerLayer
        /// </summary>
        internal Geometry AdornerClip
        {
            get => Clip;
            set => Clip = value;
        }
        /// <summary>
        /// Gets or sets the transform of this Visual.
        /// Needed by AdornerLayer
        /// </summary>
        internal ITransform AdornerTransform
        {
            get => RenderTransform;
            set => RenderTransform = value;
        }
    }
}