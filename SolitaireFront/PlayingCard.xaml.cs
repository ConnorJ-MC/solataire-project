using SolitaireBack;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;

namespace SolitaireFront
{
    /// <summary>
    /// Interaction logic for PlayingCard.xaml
    /// </summary>
    public partial class PlayingCard : UserControl
    {
        // The backend model
        public Card Model { get; private set; }

        //public CardSuit Suit { get; set; }
        //public CardValue Value { get; set; }

        // Fields to track dragging state and click position
        private bool _isDragging;
        private Point _clickPosition;
        private Point _startPoint;      // For threshold check (relative to Canvas)
        private const double MoveThreshold = 5.0; // Pixels to move before it's a drag

        // Field to track whether the card is face up or down
        private bool _isFaceUp = false;
        public bool IsFaceUp { get; set; }

        private static int _globalZIndex = 180;

        /// <summary>
        /// Need this empty constructor for XAML designer support.
        /// </summary>
        public PlayingCard()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Takes a Card model and the corresponding front/back images to create a visual card that is in sync with the backend state.
        /// The images are already loaded by the caller to avoid redundant loading when creating multiple cards. The card's initial 
        /// flip state is set based on the model's isFaceUp property.
        /// </summary>
        /// <param name="cardModel">Back end data model contains everything that it needed for a card</param>
        /// <param name="front">Previously extracted from model and passes separately</param>
        /// <param name="back">Previously extracted from model and passes separately</param>
        public PlayingCard(Card cardModel, ImageSource front, ImageSource back)
        {
            InitializeComponent();

            this.Model = cardModel; // Store the logic object

            this.FrontImage = front;
            this.BackImage = back;

            // Synchronise the visual flip state with the backend state
            this._isFaceUp = cardModel.isFaceUp;

            CardRotation.Angle = cardModel.isFaceUp ? 0 : 180;

            // Listen for changes in the backend Model
            this.Model.PropertyChanged += Model_PropertyChanged;

            /*if (!this._isFaceUp)
            {
                CardRotation.Angle = 180;
            }*/
        }
        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // If the model says the angle changed, run the animation
            if (e.PropertyName == nameof(Card.CurrentAngle))
            {
                RunFlipAnimation(Model.isFaceUp ? 0 : 180);
            }
        }

        public static readonly DependencyProperty FrontImageProperty =
            DependencyProperty.Register("FrontImage", typeof(ImageSource), typeof(PlayingCard));

        public static readonly DependencyProperty BackImageProperty =
            DependencyProperty.Register("BackImage", typeof(ImageSource), typeof(PlayingCard));

        public ImageSource FrontImage
        {
            get => (ImageSource)GetValue(FrontImageProperty);
            set => SetValue(FrontImageProperty, value);
        }

        public ImageSource BackImage
        {
            get => (ImageSource)GetValue(BackImageProperty);
            set => SetValue(BackImageProperty, value);
        }

        // Event handlers for dragging the card around
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            /*if (GameManager.Instance.TryMove(this.Model, someTarget)) { 
                e.Handled = true; // Prevent further processing (like dragging or flipping)
                return;
            }
            else*/
            {
                _isDragging = false;
                // Get the position of the mouse relative to the card itself
                _clickPosition = e.GetPosition(this);

                _startPoint = e.GetPosition(this.Parent as UIElement);
                // Capture the mouse so it stays tracked even if moved quickly
                this.CaptureMouse();

                // Increment and apply the global Z-Index so this card stays on top
                _globalZIndex++;

                // Move this card to the front of all others
                Panel.SetZIndex(this, _globalZIndex);
            }
        }

        // This event is triggered when the mouse button is released, ending the drag
        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.ReleaseMouseCapture();
                //Panel.SetZIndex(this, 0);

                // Hide the shadow when dropped
                CardShadow.Opacity = 0;
               

                if (_isDragging)
                {
                    // 1. Drop Animation: Return to original size
                    DoubleAnimation scaleDown = new DoubleAnimation(1.0, TimeSpan.FromSeconds(0.1));
                    CardScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleDown);
                    CardScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleDown);

                    // 2. Hide Shadow
                    CardShadow.BeginAnimation(DropShadowEffect.OpacityProperty, new DoubleAnimation(0, TimeSpan.FromSeconds(0.1)));
                    CardShadow.ShadowDepth = 5;
                }
                else
                {
                    FlipCard();
                }
            }
        }

        private void RunFlipAnimation(double targetAngle)
        {
            DoubleAnimation flipAnimation = new DoubleAnimation
            {
                To = targetAngle,
                Duration = TimeSpan.FromSeconds(0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            CardRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, flipAnimation);
        }

        private void FlipCard()
        {
            // Determine the target angle based on current state
            double targetAngle = _isFaceUp ? 180 : 0;
            _isFaceUp = !_isFaceUp;

            // Create the animation
            DoubleAnimation flipAnimation = new DoubleAnimation
            {
                To = targetAngle,
                Duration = TimeSpan.FromSeconds(0.5),
                // Use Easing for a "natural" bouncy feel
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            // Apply the animation to the Angle property of our Rotation object
            CardRotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, flipAnimation);

        }

        // This event is triggered whenever the mouse moves while dragging
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                Point currentPoint = e.GetPosition(this.Parent as UIElement);

                // Calculate the distance moved
                Vector delta = currentPoint - _startPoint;

                // If the movement exceeds the threshold, we are officially dragging
                if (!_isDragging && (Math.Abs(delta.X) > MoveThreshold || Math.Abs(delta.Y) > MoveThreshold))
                {
                    _isDragging = true;

                    // 1. Lift Animation: Scale the card up to 110%
                    DoubleAnimation scaleUp = new DoubleAnimation(1.1, TimeSpan.FromSeconds(0.1));
                    CardScale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleUp);
                    CardScale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleUp);

                    // 2. Shadow Animation: Show shadow and increase depth
                    DoubleAnimation shadowFade = new DoubleAnimation(0.5, TimeSpan.FromSeconds(0.1));
                    CardShadow.BeginAnimation(DropShadowEffect.OpacityProperty, shadowFade);
                    CardShadow.ShadowDepth = 15;
                }

                if (_isDragging)
                {
                    Canvas.SetLeft(this, currentPoint.X - _clickPosition.X);
                    Canvas.SetTop(this, currentPoint.Y - _clickPosition.Y);
                }
            }
        }
    }
}
