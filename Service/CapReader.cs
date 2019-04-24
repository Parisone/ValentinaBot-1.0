using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PreventivoService;

namespace ComplexDialogBot.Service
{
    public class CapReader
    {
        private static PreventivatoriServiceClient ps;

        public static bool FindComune(string cap)
        {
            ps = new PreventivatoriServiceClient();
            BaseCapContract bcc = CreateBCC(cap);
            InfoComune[] ic = TCCap(ps, bcc);
            return FindCap(ic, cap);
        }

        private static bool FindCap(InfoComune[] ic, string cap)
        {
            if (ic.Length == 0)
            {
                return false;
            }

            string temp = ic[0].Cap;
            int f = 0;
            for (int i = 0; i < 3; i++)
            {
                if (cap[i] == temp[i])
                {
                    f++;
                }
            }

            if (f == 3)
            {
                Global.COMUNE = ic[0].DescrizioneComune;
                Global.CODEPROV = ic[0].CodiceProvincia;
                Global.ISTAT = ic[0].CodiceIstatComune;
                return true;
            }

            return false;
        }

        private static InfoComune[] TCCap(PreventivatoriServiceClient ps, BaseCapContract bcc)
        {
            try
            {
                return ps.GetComuniProvinciaByCapAsync(bcc).Result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static BaseCapContract CreateBCC(string cap)
        {
            BaseCapContract bcc = new BaseCapContract();
            bcc.Cap = cap;
            bcc.Device = DEVICE.WEB;
            bcc.Compagnia = "1";
            bcc.CodiceApplicazione = "UL001P";
            return bcc;
        }
    }
}
