using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.ViewModels
{
    public partial class ChequeCrearViewModel : ObservableObject, IQueryAttributable
    {
        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // ------------------------------------------------------------------

        [ObservableProperty]
        public string name = string.Empty;

        readonly IPopupService _popupService;


        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public ChequeCrearViewModel(IPopupService popupService)
        {
            _popupService = popupService;
        }


        // -------------------------------------------------------------------
        // ----------------------- Comandos y Consultas a DB -----------------
        // -------------------------------------------------------------------

        [RelayCommand]
        async Task OnCancel()
        {
            await _popupService.ClosePopupAsync(Shell.Current);
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        async Task OnSave()
        {
            await _popupService.ClosePopupAsync(Shell.Current, Name);
        }

        bool CanSave() => string.IsNullOrWhiteSpace(Name) is false;


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Name = (string)query[nameof(ChequeCrearViewModel.Name)];
        }

    }
}
