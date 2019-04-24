using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FullQuotationsService;
using Microsoft.BotBuilderSamples;
using QuickQuotationsService;
using DEVICE = FullQuotationsService.DEVICE;
using METHOD_TYPE = FullQuotationsService.METHOD_TYPE;
using QuotatoriRichContract = FullQuotationsService.QuotatoriRichContract;

namespace ComplexDialogBot.Service
{
    public class VeicoloInfo
    {
        public static QuotatoriRichContract Send()
        {
            QuotatoriRichContract q = new QuotatoriRichContract();
            q.CodiceApplicazione = "UL001P";
            q.Compagnia = "1";
            q.Device = DEVICE.WEB;
            q.KeyValueParameters = "codiceCanale=001|quotatore=true";
            q.Method = METHOD_TYPE.NOT_SET;
            q.Step = "1";
            return q;
        }

        public static PreventivatoreFullAutoContainer TCVeicolo(FullQuotationsServiceClient full, QuotatoriRichContract qrc)
        {
            try
            {
                return full.GetQuotatoreFullAutoAsync(qrc).Result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static QuotatoriRichContract CreateQRC(UserProfile user, string cookieValue)
        {
            QuotatoriRichContract q = new QuotatoriRichContract();
            q.CodiceApplicazione = "UL001P";
            q.Compagnia = "1";
            q.Device = DEVICE.WEB;
            q.Step = "1";
            q.Method = METHOD_TYPE.NEXT;
            q.CookieValue = cookieValue;
            q.KeyValueParameters = "valoriCampo['polizza.datadecorrenza']=" + DateTime.Now.ToString("dd/MM/yyyy") + "|valoriCampo['bene.dati.codiceTarga']=" + user.Targa + "|valoriCampo['bene.assicurato.datanascita']=" + user.DataNascita + "|valoriCampo['preventivo.anagraficaCanale']=691";
            return q;
        }
    }
}
