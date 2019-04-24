using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.BotBuilderSamples;
using PreventivoService;

namespace ComplexDialogBot.Service
{
    public class ViaggioInfo
    {
        public static QuoteProdottoContract CreateQCP()
        {
            QuoteProdottoContract q = new QuoteProdottoContract();
            q.CodiceApplicazione = "UL001P";
            q.Compagnia = "1";
            q.Device = DEVICE.WEB;
            q.CodiceProdotto = "000145";
            q.IdPuntoVendita = 175210000000000000;
            q.Username = "UTENTEWEB";
            q.Multicanale = 5;
            return q;
        }

        public static DettaglioQuoteProdotto TCViaggio(PreventivatoriServiceClient ps, QuoteProdottoContract qpc)
        {
            try
            {
                return ps.GetSHPQuoteAsync(qpc).Result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static InfoBase createIB(DettaglioQuoteProdotto dqp, UserProfile user, int i)
        {
            InfoBase ib = new InfoBase();
            ib.Canale = CANALE.WEB;
            ib.CodiceApplicazione = "UL001P";
            ib.Compagnia = "1";
            ib.Device = DEVICE.WEB;
            ib.CodiceProdotto = "000156";
            ib.StringInfo = XmlManager.ParsificazioneXmlViaggi(dqp.XmlResponse, user, 2, i);
            return ib;
        }

        public static DettaglioQuoteGaranzie TCDettaglio(PreventivatoriServiceClient ps, InfoBase ib)
        {
            try
            {
                return ps.PricingSHPQuoteAsync(ib).Result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal static string ConvertDest(string result)
        {
            result = result.ToUpper();
            if (result == "ITALIA")
            {
                return "1";
            }
            else if (result == "EUROPA")
            {
                return "2";
            }
            else if (result == "MONDO")
            {
                return "3";
            }

            return null;
        }
    }
}
