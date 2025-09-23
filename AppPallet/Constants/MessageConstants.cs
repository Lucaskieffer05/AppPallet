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


        // Mensajes genéricos
        public static class Generic
        {
            public const string OperationSuccess = "Operación realizada correctamente.";
            public const string UnexpectedError = "Ha ocurrido un error inesperado.";
        }
    }

}
