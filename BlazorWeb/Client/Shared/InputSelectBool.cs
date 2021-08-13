using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWeb.Client.Shared
{
    public class InputSelectBool<T> : InputSelect<T>
    {


        //ejemplo de utilizacion
        //Ojo los valores tienen que tener la primer letra en mayuscula False y True
    //                <InputSelectBool class="form-control"
    //                                 @bind-Value="@tipo.Activo">
    //                    @if(tipo.Activo == false)
    //    {< option selected value = "False" > No </ option >}
    //                    else
    //                    {<option value = "False" > No </ option >}
    //@if(tipo.Activo == true)
    //{< option selected value = "True" > Si </ option >}
    //                    else
    //                    {<option value = "True" > Si </ option >}
    //                </InputSelectBool>
    //                <ValidationMessage For = "@(() => tipo.Activo)" />


        protected override bool TryParseValueFromString(string value, out T result, 
            out string validationErrorMessage)
        {
            if (typeof(T) == typeof(bool))
            {
                if (bool.TryParse(value, out var resultBool))
                {

                        result = (T)(object)resultBool;
                        validationErrorMessage = null;
                        return true;
          
                }
                else
                {
                    result = default;
                    validationErrorMessage = "Valor no Valido, Solo False y True";
                    return false;
                }
            }
            else
            {
                return base.TryParseValueFromString(value, out result, out validationErrorMessage);
            }
        }
    }
}
