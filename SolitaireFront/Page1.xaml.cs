using SolitaireBack;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SolitaireFront
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {

        //#1
        public Page1() : this(GameManager.Instance)
        {
        }
        public Page1(GameManager gm)
        {
            InitializeComponent();
            DataContext = new Page1ViewModel(gm);
            //RenderGameTable();
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is Page1ViewModel viewModel)
            {
                viewModel.ResetGame();
                NavigationService.Navigate(new Login());
            }
        }

        //#1
        private void RenderGameTable()
        {
            GameGrid.Children.Clear(); // Start fresh

            //#1 Render the Tableaus (the 7 columns)
            // We access these via the GameManager singleton
            for (int i = 0; i < GameManager.Instance.Tableaus.Count; i++)
            {
                var tableau = GameManager.Instance.Tableaus[i];
                for (int j = 0; j < tableau.Cards.Count; j++)
                {
                    var cardModel = tableau.Cards[j];

                    // Create the visual UserControl
                    PlayingCard visualCard = CreateVisualCard(cardModel);

                    // Calculate position: Each column (i) is offset horizontally
                    // Each card in the stack (j) is offset vertically to show the stack
                    double x = 50 + (i * 110);
                    double y = 100 + (j * 25);

                    Canvas.SetLeft(visualCard, x);
                    Canvas.SetTop(visualCard, y);

                    GameGrid.Children.Add(visualCard);
                }
            }

            // 2. Repeat similar logic for Stock, Waste, and Foundations...
        }

        private void Stock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var gm = GameManager.Instance;
            //var lm = LoginManager.Instance;

            if (!gm.Stock.isEmpty())
            {
                // 1. Draw 1 card from the stock
                List<Card> drawn = gm.Stock.draw(1);
                Card card = drawn[0];

                // 2. Flip it face up before moving to Waste
                card.isFaceUp = true;

                //3. Add it to the Waste pile
                gm.Waste.addCard(card);

                // 4. Notify the UI that 'TopCard' has changed
                // (If Stock implements INotifyPropertyChanged)


                //int drawCount = (int)lm.difficulty;
                //List<Card> drawn = gm.Stock.draw(drawCount);

                //foreach (Card card in drawn)
                //{
                //    card.isFaceUp = true;
                //    gm.Waste.addCard(card);
                //}
            }
            else
            {
                gm.stockRecycle();
            }

        }


        private PlayingCard CreateVisualCard(Card model)
        {
            // If the backend says the card is face down, use the BackImage
            string frontFile = $"card-{model.suit}-{model.rank}.png";

            ImageSource frontImg = GetImageResource(frontFile);
            ImageSource backImg = GetImageResource("card-back2.png");

            return new PlayingCard(model, frontImg, backImg);
        }

        /// <summary>
        /// Creates a BitmapImage from an embedded resource file located in the application's Resources folder.
        /// </summary>
        /// <remarks>The image is loaded using a pack URI targeting the application's Resources folder.
        /// Ensure that the specified file exists and is properly included as a resource in the project.</remarks>
        /// <param name="fileName">The name of the image file to load from the Resources directory. Must include the file extension (e.g.,
        /// "icon.png").</param>
        /// <returns>A BitmapImage representing the specified resource file. If the file does not exist, an exception may be
        /// thrown.</returns>
        private BitmapImage GetImageResource(string fileName)
        {
            // This reduces the repetitive "pack://" boilerplate
            return new BitmapImage(new Uri($"pack://application:,,,/SolitaireFront;component/Recources/Playing Cards/{fileName}"));
        }

        private void DealNewGame()
        {
            GameManager.Instance.StartGame(); // Shuffles and deals to tableaus in backend

            ImageSource cardBackImage = GetImageResource("card-back2.png");
            foreach (var tableau in GameManager.Instance.Tableaus)
            {
                foreach (var cardModel in tableau.Cards)
                {
                    // Create the visual card for each backend model
                    var visualCard = new PlayingCard(
                        cardModel,
                        GetImageResource(cardModel.ImagePath),
                        cardBackImage
                    );

                    // Add to your Canvas based on tableau position
                    GameGrid.Children.Add(visualCard);
                }
            }
        }

        public void SyncUIWithBackend()
        {
            GameGrid.Children.Clear(); // Clear old cards

            // Offset constants for the layout
            double cardWidth = 110;
            double cardSpacing = 20;

            // 1. Draw Tableaus (the 7 main piles)
            for (int i = 0; i < GameManager.Instance.Tableaus.Count; i++)
            {
                var pile = GameManager.Instance.Tableaus[i];
                for (int j = 0; j < pile.Cards.Count; j++)
                {
                    var cardModel = pile.Cards[j];

                    // Create the visual card
                    var visualCard = CreateVisualCard(cardModel);

                    // Position: Column i, Row j (cascaded)
                    Canvas.SetLeft(visualCard, 50 + (i * cardWidth));
                    Canvas.SetTop(visualCard, 100 + (j * cardSpacing));

                    GameGrid.Children.Add(visualCard);
                }
            }

            // 2. Draw Stock and Waste
            // Stock is usually top left, Waste next to it
            if (!GameManager.Instance.Stock.isEmpty())
            {
                var stockCard = CreateVisualCard(GameManager.Instance.Stock.Cards.Last());
                Canvas.SetLeft(stockCard, 50);
                Canvas.SetTop(stockCard, 20);
                GameGrid.Children.Add(stockCard);
            }
        }
    }
}
