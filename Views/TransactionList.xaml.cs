using AppControleFinanceiro.Repositories;
using CommunityToolkit.Mvvm.Messaging;
using AppControleFinanceiro.Models;

namespace AppControleFinanceiro.Views;

public partial class TransactionList : ContentPage
{
    private ITransactionRepository _repository;
	public TransactionList(ITransactionRepository repository)
	{

        this._repository = repository;

		InitializeComponent();
        Reload();
        WeakReferenceMessenger.Default.Register<string>(this, (e, msg) =>
        {
            Reload();
        });
	}
    private void Reload()
    {
        var items = _repository.GetAll();
        CollectionViewTransactions.ItemsSource = items;

        double income = items.Where(a => a.Type == Models.TransactionType.Income).Sum(a => a.Value);
        double expense = items.Where(a => a.Type == Models.TransactionType.Expenses).Sum(a => a.Value);
        double balance = income - expense;

        LabelIncome.Text = income.ToString("C");
        LabelExpense.Text = expense.ToString("C");
        LabelBalance.Text = balance.ToString("C");

    }

    private void OnButtonClickedToTransactionAdd(object sender, EventArgs e)
    {
        var transactionAdd = Handler.MauiContext.Services.GetService<TransactionAdd>();
        Navigation.PushModalAsync(transactionAdd);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }

    private void ToTransactionEdit(object sender, TappedEventArgs e)
    {
        var grid = (Grid)sender;
        var gesture = (TapGestureRecognizer)grid.GestureRecognizers[0];
        Transaction transaction = (Transaction)gesture.CommandParameter;

        var transactionEdit = Handler.MauiContext.Services.GetService<TransactionEdit>();
        transactionEdit.SetTransactionToEdit(transaction);
        Navigation.PushModalAsync(transactionEdit);
    }

    private async void TapGestureRecognizer_ToDelete(object sender, TappedEventArgs e)
    {
        AnimationBorder((Border)sender, true);
        bool result = await App.Current.MainPage.DisplayAlert("Excluir", "Tem certeza que deseja excluir?", "Sim", "Não");
        if (result)
        {
            Transaction transaction = (Transaction)e.Parameter;
            _repository.Delete(transaction);

            Reload();
        }
        else
        {
            AnimationBorder((Border)sender, false);
        }
    }

    private Color _borderOriginalBackgroundColor;
    private String _labelOriginalText;
    private async void AnimationBorder(Border border, bool IsDeleteAnimation)
    {
            var label = (Label)border.Content;
        if (IsDeleteAnimation)
        {
            _borderOriginalBackgroundColor = border.BackgroundColor;
            _labelOriginalText = label.Text;

            await border.RotateYTo(90, 250);
            

            border.BackgroundColor = Colors.Red;

            label.TextColor = Colors.White;
            label.Text = "X";

            await border.RotateYTo(180, 250);
        }
        else
        {
            await border.RotateYTo(90, 250);
            border.BackgroundColor = _borderOriginalBackgroundColor;
            label.Text = _labelOriginalText;
            label.TextColor = Colors.Black;

            await border.RotateYTo(0, 250);

        }
    }
}