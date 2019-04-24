using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.BotBuilderSamples;
using QuickQuotationsService;

namespace ComplexDialogBot.Service
{
    public static class HealthInfo
    {
        public static string ConvertAge(int eta)
        {
            if (eta >= 0 && eta <= 10)
            {
                return "1-5404";
            }
            else if (eta > 10 && eta <= 20)
            {
                return "2-5405";
            }
            else if (eta > 20 && eta <= 30)
            {
                return "3-5406";
            }
            else if (eta > 30 && eta <= 40)
            {
                return "4-5407";
            }
            else if (eta > 40 && eta <= 45)
            {
                return "5-5408";
            }
            else if (eta > 45 && eta <= 50)
            {
                return "6-5409";
            }
            else if (eta > 50 && eta <= 55)
            {
                return "7-5410";
            }
            else if (eta > 55 && eta <= 60)
            {
                return "8-5411";
            }

            return "9-5412";
        }

        public static string ConvertStruct(string st)
        {
            st = st.ToUpper();
            if (st == "SSN")
            {
                return "1-5400";
            }
            else if (st == "STRUTTURE PRIVATE")
            {
                return "2-5401";
            }

            return null;
        }

        public static string ConvertDisease(string st)
        {
            st = st.ToUpper();
            if (st == "TUTTE")
            {
                return "2-5403";
            }
            else if (st == "SOLO GRAVI")
            {
                return "1-5402";
            }

            return null;
        }

        public static ProposteContract CreateProp(UserProfile user)
        {
            ProposteContract prop = new ProposteContract();
            prop.Email = user.Email;
            prop.Device = DEVICE.WEB;
            prop.Compagnia = "1";
            prop.CodiceApplicazione = "UL001P";
            prop.Privacy = true;
            prop.IdCategoria = "2";
            prop.ListaParametri = new Dictionary<string, string>();
            prop.ListaParametri.Add("5", user.Age);
            prop.ListaParametri.Add("7", user.ProvRes);
            prop.ListaParametri.Add("13", user.Strutture);
            prop.ListaParametri.Add("14", user.Patologie);
            return prop;
        }

        public static Quotation TCHealt(QuickQuotationsServiceClient quick, ProposteContract prop)
        {
            try
            {
               return quick.GetProposteAsync(prop).Result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
