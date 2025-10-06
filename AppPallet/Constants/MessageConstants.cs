using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPallet.Constants
{
    public class MessageResult
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public MessageResult(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }

    public static class MessageConstants
    {
        // Títulos
        public static class Titles
        {
            public const string Success = "Éxito";
            public const string Error = "Error";
            public const string Warning = "Advertencia";
        }

        // Mensajes de ActivoPasivo
        public static class ActivoPasivo
        {
            public const string ModifiedSuccess = "El activo/pasivo ha sido modificado correctamente.";
            public const string ModifiedNoChanges = "El activo/pasivo ha sido guardado correctamente, pero sin cambios.";
            public const string NotFound = "No se encontró el activo/pasivo para modificar.";
            public const string ModifyError = "No se pudo modificar el activo/pasivo. Intente nuevamente.";
            public const string ValidationError = "Por favor, complete todos los campos correctamente.";
            public const string DeleteSuccess = "El activo/pasivo ha sido eliminado correctamente.";
            public const string DeleteError = "No se pudo eliminar el activo/pasivo.";

            public const string CreateError = "No se pudo crear el activo/pasivo.";
            public const string CreateSuccess = "El activo/pasivo ha sido creado correctamente.";

        }

        public static class Venta 
        {
            
            public const string CreateSuccess = "La venta ha sido creada correctamente.";
            public const string CreateError = "No se pudo crear la venta. Intente nuevamente.";
            public const string NotFound = "No se encontró la venta.";
            public const string DeleteSuccess = "La venta ha sido eliminada correctamente.";
            public const string DeleteError = "No se pudo eliminar la venta. Intente nuevamente.";
            public const string ValidationError = "Por favor, complete todos los campos correctamente.";
            public const string ModifySuccess = "La venta ha sido modificada correctamente.";
            public const string ModifyNoChanges = "No se detectarion cambios para guardar.";
            public const string ModifyError = "No se pudo modificar la venta. Intente nuevamente.";

        }

        public static class Presupuesto
        {
            public const string CreateSuccess = "El presupuesto ha sido creado correctamente.";
            public const string CreateError = "No se pudo crear el presupuesto. Intente nuevamente.";
            public const string NotFound = "No se encontró el presupuesto.";
            public const string DeleteSuccess = "El presupuesto ha sido eliminado correctamente.";
            public const string DeleteError = "No se pudo eliminar el presupuesto. Intente nuevamente.";
            public const string ValidationError = "Por favor, complete todos los campos correctamente.";
            public const string ModifySuccess = "El presupuesto ha sido modificado correctamente.";
            public const string ModifyNoChanges = "No se detectarion cambios para guardar.";
            public const string ModifyError = "No se pudo modificar el presupuesto. Intente nuevamente.";
        }

        public static class  HistorialHumedad
        {
            public const string CreateSuccess = "El historial de humedad ha sido creado correctamente.";
            public const string CreateError = "No se pudo crear el historial de humedad. Intente nuevamente.";
            public const string NotFound = "No se encontró el historial de humedad.";
            public const string DeleteSuccess = "El historial de humedad ha sido eliminado correctamente.";
            public const string DeleteError = "No se pudo eliminar el historial de humedad. Intente nuevamente.";
            public const string ValidationError = "Por favor, complete todos los campos correctamente.";
            public const string ModifySuccess = "El historial de humedad ha sido modificado correctamente.";
            public const string ModifyNoChanges = "No se detectarion cambios para guardar.";
            public const string ModifyError = "No se pudo modificar el historial de humedad. Intente nuevamente.";

        }

        public static class Ingreso
        {
            public const string CreateSuccess = "El ingreso ha sido creado correctamente.";
            public const string CreateError = "No se pudo crear el ingreso. Intente nuevamente.";
            public const string NotFound = "No se encontró el ingreso.";
            public const string DeleteSuccess = "El ingreso ha sido eliminado correctamente.";
            public const string DeleteError = "No se pudo eliminar el ingreso. Intente nuevamente.";
            public const string ValidationError = "Por favor, complete todos los campos correctamente.";
            public const string ModifySuccess = "El ingreso ha sido modificado correctamente.";
            public const string ModifyNoChanges = "No se detectarion cambios para guardar.";
            public const string ModifyError = "No se pudo modificar el ingreso. Intente nuevamente.";

        }

        public static class Stock
        {
            public const string CreateSuccess = "El stock ha sido creado correctamente.";
            public const string CreateError = "No se pudo crear el stock. Intente nuevamente.";
            public const string NotFound = "No se encontró el stock.";
            public const string DeleteSuccess = "El stock ha sido eliminado correctamente.";
            public const string DeleteError = "No se pudo eliminar el stock. Intente nuevamente.";
            public const string ValidationError = "Por favor, complete todos los campos correctamente.";
            public const string ModifySuccess = "El stock ha sido modificado correctamente.";
            public const string ModifyNoChanges = "No se detectarion cambios para guardar.";
            public const string ModifyError = "No se pudo modificar el stock. Intente nuevamente.";
        }

        public static class Egreso
        {
            public const string CreateSuccess = "El egreso ha sido creado correctamente.";
            public const string CreateError = "No se pudo crear el egreso. Intente nuevamente.";
            public const string NotFound = "No se encontró el egreso.";
            public const string DeleteSuccess = "El egreso ha sido eliminado correctamente.";
            public const string DeleteError = "No se pudo eliminar el egreso. Intente nuevamente.";
            public const string ValidationError = "Por favor, complete todos los campos correctamente.";
            public const string ModifySuccess = "El egreso ha sido modificado correctamente.";
            public const string ModifyNoChanges = "No se detectarion cambios para guardar.";
            public const string ModifyError = "No se pudo modificar el egreso. Intente nuevamente.";

        }

        public static class Lote
        {
            public const string CreateSuccess = "El lote ha sido creado correctamente.";
            public const string CreateError = "No se pudo crear el lote. Intente nuevamente.";
            public const string NotFound = "No se encontró el lote.";
            public const string DeleteSuccess = "El lote ha sido eliminado correctamente.";
            public const string DeleteError = "No se pudo eliminar el lote. Intente nuevamente.";
            public const string ValidationError = "Por favor, complete todos los campos correctamente.";
            public const string ModifySuccess = "El lote ha sido modificado correctamente.";
            public const string ModifyNoChanges = "No se detectarion cambios para guardar.";
            public const string ModifyError = "No se pudo modificar el lote. Intente nuevamente.";
            public static string DeleteHasPedidos= "No se puede eliminar el lote porque tiene pedidos asociados.";
        }

        public static class Pallet
        {
            public const string CreateSuccess = "El pallet ha sido creado correctamente.";
            public const string CreateError = "No se pudo crear el pallet. Intente nuevamente.";
            public const string NotFound = "No se encontró el pallet.";
            public const string DeleteSuccess = "El pallet ha sido eliminado correctamente.";
            public const string DeleteError = "No se pudo eliminar el pallet. Intente nuevamente.";
            public const string ValidationError = "Por favor, complete todos los campos correctamente.";
            public const string ModifySuccess = "El pallet ha sido modificado correctamente.";
            public const string ModifyNoChanges = "No se detectarion cambios para guardar.";
            public const string ModifyError = "No se pudo modificar el pallet. Intente nuevamente.";
            public const string AddStockSuccess = "El stock del pallet ha sido actualizado correctamente.";
        }

        public static class CostoPorPallet
        {
            public const string CreateSuccess = "El costo por pallet ha sido creado correctamente.";
            public const string CreateError = "No se pudo crear el costo por pallet. Intente nuevamente.";
            public const string NotFound = "No se encontró el costo por pallet.";
            public const string DeleteSuccess = "El costo por pallet ha sido eliminado correctamente.";
            public const string DeleteError = "No se pudo eliminar el costo por pallet. Intente nuevamente.";
            public const string ValidationError = "Por favor, complete todos los campos correctamente.";
            public const string ModifySuccess = "El costo por pallet ha sido modificado correctamente.";
            public const string ModifyNoChanges = "No se detectarion cambios para guardar.";
            public const string ModifyError = "No se pudo modificar el costo por pallet. Intente nuevamente.";
        }

        public static class CostoPorCamion
        {
            public const string CreateSuccess = "El costo por camión ha sido creado correctamente.";
            public const string CreateError = "No se pudo crear el costo por camión. Intente nuevamente.";
            public const string NotFound = "No se encontró el costo por camión.";
            public const string DeleteSuccess = "El costo por camión ha sido eliminado correctamente.";
            public const string DeleteError = "No se pudo eliminar el costo por camión. Intente nuevamente.";
            public const string ValidationError = "Por favor, complete todos los campos correctamente.";
            public const string ModifySuccess = "El costo por camión ha sido modificado correctamente.";
            public const string ModifyNoChanges = "No se detectarion cambios para guardar.";
            public const string ModifyError = "No se pudo modificar el costo por camión. Intente nuevamente.";
        }

        public static class Pedido
        {
            public const string CreateSuccess = "El pedido ha sido creado correctamente.";
            public const string CreateError = "No se pudo crear el pedido. Intente nuevamente.";
            public const string NotFound = "No se encontró el pedido.";
            public const string DeleteSuccess = "El pedido ha sido eliminado correctamente.";
            public const string DeleteError = "No se pudo eliminar el pedido. Intente nuevamente.";
            public const string ValidationError = "Por favor, complete todos los campos correctamente.";
            public const string ModifySuccess = "El pedido ha sido modificado correctamente.";
            public const string ModifyNoChanges = "No se detectarion cambios para guardar.";
            public const string ModifyError = "No se pudo modificar el pedido. Intente nuevamente.";
        }


        public static class Empresa
        {
            public const string CreateSuccess = "La empresa ha sido creada correctamente.";
            public const string CreateError = "No se pudo crear la empresa. Intente nuevamente.";
            public const string NotFound = "No se encontró la empresa.";
            public const string DeleteSuccess = "La empresa ha sido eliminada correctamente.";
            public const string DeleteError = "No se pudo eliminar la empresa. Intente nuevamente.";
            public const string ValidationError = "Por favor, complete todos los campos correctamente.";
            public const string ModifySuccess = "La empresa ha sido modificada correctamente.";
            public const string ModifyNoChanges = "No se detectarion cambios para guardar.";
            public const string ModifyError = "No se pudo modificar la empresa. Intente nuevamente.";
            public static string DeleteHasContactos = "No se puede eliminar la empresa porque tiene contactos asociados.";
            public static string DeleteHasPedidos = "No se puede eliminar la empresa porque tiene pedidos asociados.";
            public static string Deleted = "La empresa ha sido eliminada correctamente.";
            public static string NoDeleted = "La empresa no pudo ser eliminada";
        }


        // Mensajes genéricos
        public static class Generic
        {
            public const string OperationSuccess = "Operación realizada correctamente.";
            public const string UnexpectedError = "Ha ocurrido un error inesperado.";
        }
    }

}
