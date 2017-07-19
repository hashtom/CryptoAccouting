using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;

namespace CryptoAccouting
{
    public partial class PerfomViewController : UIViewController
    {
        //private UIPopoverController popover;
        public PickerModel PickerModel;
        public int SelIndx { get; set; }

        public PerfomViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var offset = 0;

            offset = 50;
            var button = UIButton.FromType(UIButtonType.RoundedRect);
            button.Frame = new CGRect(this.View.Bounds.X, this.View.Bounds.Y, this.View.Bounds.Width, offset);
            button.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            button.SetTitle("取引所！！！！！！！", UIControlState.Normal);
            this.View.AddSubview(button);
            button.TouchUpInside += (sender, args) => this.DismissViewController(true, null);

            var picker = new UIPickerView
            {
                Frame =
                    new CGRect(this.View.Bounds.X, this.View.Bounds.Y + offset, this.View.Bounds.Width,
                        this.View.Bounds.Height - offset),
                AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth //| UIViewAutoresizing.FlexibleRightMargin
            };

            var pickerdata = new List<string>();
            pickerdata.Add("ZAIF");
            pickerdata.Add("Kraken");

            PickerModel = new PickerModel(pickerdata);
            picker.Model = PickerModel;
            picker.ShowSelectionIndicator = true;

            this.View.AddSubview(picker);

            if (SelIndx >= 0)
                picker.Select(SelIndx, 0, false);

        }
    }

    public class PickerChangedEventArgs : EventArgs
    {
        public object SelectedValue { get; set; }
    }

    public class PickerModel : UIPickerViewModel
    {
        private readonly List<string> values;

        public event EventHandler<PickerChangedEventArgs> PickerChanged;

        public PickerModel(List<string> values)
        {
            this.values = values;
        }

        public override nint GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return values.Count;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return values[(int)row];
        }

        public override nfloat GetRowHeight(UIPickerView pickerView, nint component)
        {
            return 40f;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            if (this.PickerChanged != null)
            {
                this.PickerChanged(this, new PickerChangedEventArgs { SelectedValue = values[(int)row] });
            }
        }
    }
}