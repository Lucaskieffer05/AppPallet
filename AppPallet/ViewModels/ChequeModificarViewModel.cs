using AppPallet.Controllers;
using AppPallet.Models;
using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.ViewModels
{
    public partial class ChequeModificarViewModel : ObservableObject, IQueryAttributable
    {

        // -------------------------------------------------------------------
        // ----------------------- Definiciones ------------------------------
        // -------------------------------------------------------------------

        [ObservableProperty]
        public Cheque? chequeSeleccionado;

        readonly IPopupService _popupService;

        readonly ChequeController _chequeController;

        // -------------------------------------------------------------------
        // ----------------------- Constructor -------------------------------
        // -------------------------------------------------------------------

        public ChequeModificarViewModel(IPopupService popupService, ChequeController chequeController)
        {
            _popupService = popupService;
            _chequeController = chequeController;
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
            bool resultado = await _chequeController.UpdateCheque(ChequeSeleccionado);
            await _popupService.ClosePopupAsync(Shell.Current, resultado);
        }

        bool CanSave() 
        {
            if (ChequeSeleccionado == null) return false;
            if (!ValidarCheque(ChequeSeleccionado)) return false;
            return true;
        }


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            ChequeSeleccionado = (Cheque)query["ChequeSeleccionado"];
        }

        bool ValidarCheque(Cheque cheque) 
        { 
            if(cheque.NumCheque == null || cheque.NumCheque.Trim() == "")return false;
            if (cheque.Proveedor == null || cheque.Proveedor.Trim() == "") return false;
            if (cheque.Tipo == null || cheque.Tipo.Trim() == "") return false;
            if (cheque.Monto <= 0) return false;
            return true;
        }

    }
}
