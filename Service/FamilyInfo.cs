using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.BotBuilderSamples;
using QuickQuotationsService;

namespace ComplexDialogBot.Service
{
    public class FamilyInfo
    {
        public static string ConvertImporto(string i)
        {
            if (i == "500")
            {
                return "1";
            }
            else if (i == "1000")
            {
                return "2";
            }
            else if (i == "1500")
            {
                return "3";
            }
            else if (i == "2000")
            {
                return "4";
            }

            return null;
        }

        public static QuickFirstProtectionContract CreateFPR(UserProfile user)
        {
            QuickFirstProtectionContract f = new QuickFirstProtectionContract();
            f.Cap = user.Cap;
            f.Privacy = true;
            f.ValFigli = user.Figli.ToString();
            f.ValAnni = user.Age;
            f.ValProprietario = user.Propietario.ToString();
            f.Email = user.Email;
            f.ValImporto = user.Ren;
            f.Compagnia = "1";
            f.Device = DEVICE.WEB;
            f.CodiceApplicazione = "UL001P";

            return f;
        }

        internal static Quotation TCFamily(QuickQuotationsServiceClient quick, QuickFirstProtectionContract fpr)
        {
            try
            {
                return quick.GetFirstProtectionAsync(fpr).Result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
