using CoreGraphics;
using Cirrious.MvvmCross.Touch.Views;
using CodeBucket.Core.ViewModels.Accounts;
using Foundation;
using UIKit;
using CodeBucket.Utils;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;

namespace CodeBucket.Views.Accounts
{
    public partial class AddAccountView : MvxViewController
    {
		private readonly IHud _hud;

        public new AddAccountViewModel ViewModel
        {
            get { return (AddAccountViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public AddAccountView()
            : base("AddAccountView", null)
        {
            Title = "Login";
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Theme.CurrentTheme.BackButton, UIBarButtonItemStyle.Plain, (s, e) => NavigationController.PopViewController(true));
			_hud = this.CreateHud();
        }

        public override void ViewDidLoad()
        {
            var set = this.CreateBindingSet<AddAccountView, AddAccountViewModel>();
            set.Bind(User).To(x => x.Username);
            set.Bind(Password).To(x => x.Password);
            set.Bind(LoginButton).To(x => x.LoginCommand);
            set.Apply();

            base.ViewDidLoad();

			ViewModel.Bind(x => x.IsLoggingIn, x =>
			{
				if (x)
					_hud.Show("Logging in...");
				else
					_hud.Hide();
			});

			View.BackgroundColor = UIColor.FromRGB(239, 239, 244);
            Logo.Image = Images.Logos.Bitbucket;

            LoginButton.SetBackgroundImage(Images.Buttons.GreyButton.CreateResizableImage(new UIEdgeInsets(18, 18, 18, 18)), UIControlState.Normal);

            //Hide the domain, slide everything up
            Domain.Hidden = true;
            var f = User.Frame;
            f.Y -= 39;
            User.Frame = f;

            var p = Password.Frame;
            p.Y -= 39;
            Password.Frame = p;

            var l = LoginButton.Frame;
            l.Y -= 39;
            LoginButton.Frame = l;

            //Set some generic shadowing
            LoginButton.Layer.ShadowColor = UIColor.Black.CGColor;
            LoginButton.Layer.ShadowOffset = new CGSize(0, 1);
            LoginButton.Layer.ShadowOpacity = 0.3f;

            Domain.ShouldReturn = delegate {
                User.BecomeFirstResponder();
                return true;
            };

            User.ShouldReturn = delegate {
                Password.BecomeFirstResponder();
                return true;
            };
            Password.ShouldReturn = delegate {
                Password.ResignFirstResponder();
                LoginButton.SendActionForControlEvents(UIControlEvent.TouchUpInside);
                return true;
            };


            ScrollView.ContentSize = new CGSize(View.Frame.Width, LoginButton.Frame.Bottom + 10f);
        }

        NSObject _hideNotification, _showNotification;
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _hideNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            _showNotification = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NSNotificationCenter.DefaultCenter.RemoveObserver(_hideNotification);
            NSNotificationCenter.DefaultCenter.RemoveObserver(_showNotification);
        }

        private void OnKeyboardNotification (NSNotification notification)
        {
            if (IsViewLoaded) {

                //Check if the keyboard is becoming visible
                bool visible = notification.Name == UIKeyboard.WillShowNotification;

                //Start an animation, using values from the keyboard
                UIView.BeginAnimations ("AnimateForKeyboard");
                UIView.SetAnimationBeginsFromCurrentState (true);
                UIView.SetAnimationDuration (UIKeyboard.AnimationDurationFromNotification (notification));
                UIView.SetAnimationCurve ((UIViewAnimationCurve)UIKeyboard.AnimationCurveFromNotification (notification));

                //Pass the notification, calculating keyboard height, etc.
                var nsValue = notification.UserInfo.ObjectForKey (UIKeyboard.FrameBeginUserInfoKey) as NSValue;
                if (nsValue != null) 
                {
                    var kbSize = nsValue.RectangleFValue.Size;
                    var view = View.Bounds;
                    var f = ScrollView.Frame;
                    f.Height = View.Bounds.Height - kbSize.Height;
                    ScrollView.Frame = f;
                }

                //Commit the animation
                UIView.CommitAnimations (); 
            }
        }
    }
}

