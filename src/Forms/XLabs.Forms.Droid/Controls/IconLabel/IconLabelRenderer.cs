using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using XLabs.Forms.Controls;

using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using XLabs.Forms.Services;
using XLabs.Enums;
using XLabs.Forms.Extensions;

[assembly: ExportRenderer(typeof(IconLabel), typeof(IconLabelRenderer))]
namespace XLabs.Forms.Controls
{
    /// <summary>
    /// Class IconLabelRenderer.
    /// </summary>
    public class IconLabelRenderer : LabelRenderer
    {
        Typeface iconFont;
        Typeface textFont;
        IconLabel iconLabel;
        /// <summary>
        /// Final span including font and icon size and color
        /// </summary>
        SpannableString iconSpan;
        int textStartIndex = -1;
        int textStopIndex = -1;
        Android.Widget.TextView nativeLabel;
        private TextViewRenderHelper helper;

        /// <summary>
        /// Called when [element changed].
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (this.Control != null && e.NewElement != null)
            {
                if (helper == null)
                    helper = new TextViewRenderHelper(Context);
                if (iconSpan == null)
                {
                    nativeLabel = (Android.Widget.TextView)this.Control;
                    iconLabel = (IconLabel)e.NewElement;
                    //Set default value
                    if (iconLabel.IconSize == 0)
                        iconLabel.IconSize = iconLabel.FontSize;

                    iconFont = helper.TrySetFont("fontawesome-webfont.ttf");
                    textFont = iconLabel.Font.ToTypeface();
                }
                SetText();
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (iconSpan != null)
            {
                if (e.PropertyName == IconLabel.IconColorProperty.PropertyName ||
                 e.PropertyName == IconLabel.IconProperty.PropertyName ||
                 e.PropertyName == IconLabel.TextProperty.PropertyName ||
                 e.PropertyName == IconLabel.TextColorProperty.PropertyName ||
                 e.PropertyName == IconLabel.IsVisibleProperty.PropertyName ||
                 e.PropertyName == IconLabel.IconSizeProperty.PropertyName ||
                 e.PropertyName == IconLabel.FontSizeProperty.PropertyName ||
                 e.PropertyName == IconLabel.OrientationProperty.PropertyName)
                {
                    SetText();
                }
            }
        }

        /// <summary>
        /// Rebuild the all span in and set it into the label
        /// </summary>
        private void SetText()
        {
            var computedString = BuildRawTextString();

            iconSpan = BuildSpannableString(computedString);
            if (iconLabel.TextAlignement == Xamarin.Forms.TextAlignment.Center)
            {
                nativeLabel.Gravity = Android.Views.GravityFlags.Center;

            }
            else if (iconLabel.TextAlignement == Xamarin.Forms.TextAlignment.End)
            {
                nativeLabel.Gravity = Android.Views.GravityFlags.Right;
            }
            else if (iconLabel.TextAlignement == Xamarin.Forms.TextAlignment.Start)
            {
                nativeLabel.Gravity = Android.Views.GravityFlags.Left;
            }
            nativeLabel.SetText(iconSpan, TextView.BufferType.Spannable);
        }

        /// <summary>
        /// Build the content string by concating icon and text according to control options
        /// </summary>
        /// <returns></returns>
        private string BuildRawTextString()
        {
            string computedText = string.Empty;
            if (!string.IsNullOrEmpty(iconLabel.Icon) && !string.IsNullOrEmpty(iconLabel.Text))
            {
                string iconSeparator = iconLabel.ShowIconSeparator ? " | " : " ";

                switch (iconLabel.Orientation)
                {
                    case ImageOrientation.ImageToLeft:

                        computedText = iconLabel.Icon + iconSeparator + iconLabel.Text;
                        textStartIndex = computedText.IndexOf(iconSeparator);
                        textStopIndex = computedText.Length;

                        break;
                    case ImageOrientation.ImageToRight:
                        computedText = iconLabel.Text + iconSeparator + iconLabel.Icon;
                        textStartIndex = 0;
                        textStopIndex = computedText.IndexOf(iconSeparator) + iconSeparator.Length;
                        break;
                    case ImageOrientation.ImageOnTop:
                        computedText = iconLabel.Icon + System.Environment.NewLine + iconLabel.Text;
                        textStartIndex = computedText.IndexOf(iconLabel.Text);
                        textStopIndex = computedText.Length - 1;
                        break;
                    case ImageOrientation.ImageOnBottom:
                        computedText = iconLabel.Text + System.Environment.NewLine + iconLabel.Icon;
                        textStartIndex = 0;
                        textStopIndex = computedText.IndexOf(System.Environment.NewLine) - 1;
                        break;
                }
            }
            else if (!string.IsNullOrEmpty(iconLabel.Text) && string.IsNullOrEmpty(iconLabel.Icon))
            {
                computedText = iconLabel.Text;
            }
            else if (string.IsNullOrEmpty(iconLabel.Text) && !string.IsNullOrEmpty(iconLabel.Icon))
            {
                computedText = iconLabel.Icon;
            }
            return computedText;
        }

        /// <summary>
        /// Build the spannable according to the computed text, meaning set the right font, color and size to the text and icon char index
        /// </summary>
        /// <param name="computedString"></param>
        private SpannableString BuildSpannableString(string computedString)
        {
            SpannableString span = new SpannableString(computedString);
            if (!string.IsNullOrEmpty(iconLabel.Icon))
            {
                //set icon
                span.SetSpan(new CustomTypefaceSpan("fontawesome", iconFont, helper.GetSpanColor(iconLabel.IconColor, Control.TextColors)),
                    computedString.IndexOf(iconLabel.Icon),
                    computedString.IndexOf(iconLabel.Icon) + iconLabel.Icon.Length,
                    SpanTypes.ExclusiveExclusive);
                //set icon size
                span.SetSpan(new AbsoluteSizeSpan((int)iconLabel.IconSize, true),
                     computedString.IndexOf(iconLabel.Icon),
                     computedString.IndexOf(iconLabel.Icon) + iconLabel.Icon.Length,
                     SpanTypes.ExclusiveExclusive);


            }
            if (!string.IsNullOrEmpty(iconLabel.Text))
            {
                span.SetSpan(new CustomTypefaceSpan("", textFont, helper.GetSpanColor(iconLabel.TextColor, Control.TextColors)),
                     textStartIndex,
                     textStopIndex,
                     SpanTypes.ExclusiveExclusive);
                span.SetSpan(new AbsoluteSizeSpan((int)iconLabel.FontSize, true),
                    textStartIndex,
                     textStopIndex,
                    SpanTypes.ExclusiveExclusive);
            }

            return span;
        }
    }
}
