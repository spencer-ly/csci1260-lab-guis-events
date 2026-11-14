using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Metadata;

namespace GuiEventsLab;

// MainWindow: the code behind the window. It shows the coffee bar and asks
// it to do things. Every decision about orders stays in CoffeeBar.
//
// Part C and Part D are written along with the Session 2 demos.
// Exercise 3 and Exercise 4 are yours.
public partial class MainWindow : Window
{
    private readonly CoffeeBar bar = new CoffeeBar();
    private readonly Rewards rewards = new Rewards();

    public MainWindow()
    {
        InitializeComponent();

        // TODO: Part C, demo. Subscribe to bar.OrderPlaced.
        bar.OrderPlaced += Bar_OrderPlaced;
        // TODO: Exercise 3. Subscribe to bar.OrderReady and bar.AllCaughtUp.
        bar.OrderReady += Bar_OrderReady;
        bar.AllCaughtUp += Bar_AllCaughtUp;
        // TODO: Part D, demo. Subscribe to TextChanged on both boxes,
        // then set the buttons for an empty window.
        CustomerBox.TextChanged += Boxes_TextChanged;
        DrinkBox.TextChanged += Boxes_TextChanged;
        // TODO: Exercise 4. Connect a Rewards object to the bar and
        // subscribe to its FreeDrinkEarned event.
        rewards.Connect(bar);
        rewards.FreeDrinkEarned += Rewards_FreeDrinkEarned;

        UpdateButtons();
    }

    // Write the click handlers, the event handlers, and the helper
    // methods below this line.
    private void PlaceButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            bar.Place(CustomerBox.Text ?? "", DrinkBox.Text ?? "");
            CustomerBox.Text = "";
            DrinkBox.Text = "";
            StatusText.Text = "";
        }
        catch (ArgumentException ex)
        {
            StatusText.Text = ex.Message;
        }
    }

    //show new announced order
    private void Bar_OrderPlaced(object? sender, OrderEventArgs e)
    {
        ShowWaiting();

    }

    //ask bar to finish oldest order
    private void ReadyButton_Click(object? sender, RoutedEventArgs e)
    {
        bar.FinishNext();
        UpdateButtons();
    }

    //move finished order to pickup
    private void Bar_OrderReady(object? sender, OrderEventArgs e)
    {
        PickupList.Items.Add($"#{e.Order.Number} {e.Order.Customer}");
        ShowWaiting();

    }

    // Ex 3: when order line is empty
    private void Bar_AllCaughtUp(object? sender, EventArgs e)
    {
        StatusText.Text = "All caught up.";
    }

    //when text is changed
    private void Boxes_TextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        PlaceButton.IsEnabled = !string.IsNullOrWhiteSpace(CustomerBox.Text)
                                && !string.IsNullOrWhiteSpace(DrinkBox.Text);
        ReadyButton.IsEnabled = bar.WaitingCount > 0;
    }

    private void Rewards_FreeDrinkEarned(object? sender, OrderEventArgs e)
    {
        RewardText.Text = $"Free drink earned: {e.Order.Customer}";
    }

    private void ShowWaiting()
    {
        WaitingList.Items.Clear();
        foreach (Order order in bar.Waiting())
        {
            WaitingList.Items.Add(order.ToString());
        }
    }

}