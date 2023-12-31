using System.Text;
using AppControleFinanceiro.Models;
using AppControleFinanceiro.Repositories;
using CommunityToolkit.Mvvm.Messaging;

namespace AppControleFinanceiro.Views;

public partial class TransactionAdd : ContentPage
{
    private ITransactionRepository _repository;
	public TransactionAdd(ITransactionRepository repository)
	{
		InitializeComponent();
        _repository = repository;
	}

    private void TapGestureRecognizerTapped_ToClose(object sender, TappedEventArgs e)
    {
		Navigation.PopModalAsync();
    }

    private void OnButtonClickedToAdd(object sender, EventArgs e)
    {
        //Valida��o dos dados
        if (IsValidData() == false)
            return;

        //Salva no Banco (Cria um OBJETO TRANSACTION, Pega os dados inserido pelo usuario, Envia para o Repository)
        SaveTransactionInDataBase();

        //ToDo - Fechar a tela*
        Navigation.PopModalAsync();
        WeakReferenceMessenger.Default.Send<string>(string.Empty);

        var count = _repository.GetAll().Count;
        App.Current.MainPage.DisplayAlert("Mensagem!", $"Existem {count} registro(s) no banco!", "OK");
    }

    private void SaveTransactionInDataBase()
    {
        Transaction transaction = new Transaction()
        {
            Type = RadioIncome.IsChecked ? TransactionType.Income : TransactionType.Expenses,
            Name = EntryName.Text,
            Date = EntryDatePicker.Date,
            Value = double.Parse(EntryValue.Text)
        };

        _repository.Add(transaction);
    }

    private bool IsValidData()
    {
        bool valid = true;
        StringBuilder sb = new StringBuilder();

        if (string.IsNullOrEmpty(EntryName.Text) || string.IsNullOrWhiteSpace(EntryName.Text))
        {
            sb.AppendLine("O campo 'Nome' deve ser preenchido!");
            valid = false;
        }

        if (string.IsNullOrEmpty(EntryValue.Text) || string.IsNullOrWhiteSpace(EntryValue.Text))
        {
            sb.AppendLine("O campo 'Valor' deve ser preenchido!");
            valid = false;
        }
        double result;
        if(!string.IsNullOrEmpty(EntryValue.Text) && !double.TryParse(EntryValue.Text, out result)) 
        {
            sb.AppendLine("O campo 'Valor' � inv�lido!");
            valid = false;
        }
        if(valid == false)
        {
            LabelError.IsVisible = true;
            LabelError.Text = sb.ToString();
        }
        return valid;
    }
}