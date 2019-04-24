using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.BotBuilderSamples;
using PreventivoService;

namespace ComplexDialogBot.Service
{
    public class HouseInfo
    {
        public static string ConvertPA(string v)
        {
            v = v.ToUpper();
            if (v == "DI PROPRIETA")
            {
                return "1";
            }
            else
            {
                return "2";
            }
        }

        public static string ConvertAS(string v)
        {
            v = v.ToUpper();
            if (v == "ABITUALE")
            {
                return "1";
            }
            else
            {
                return "2";
            }
        }

        public static string ConvertFabricato(string v)
        {
            v = v.ToUpper();
            if (v == "CONDOMINIO O ABITAZIONE PLURIFAMILIARE NON IN CENTRO STORICO")
            {
                return "36";
            }
            else if (v == "CONDOMINIO O ABITAZIONE PLURIFAMILIARE IN CENTRO STORICO")
            {
                return "35";
            }
            else if (v == "VILLA O ABITAZIONE MONOFAMILIARE")
            {
                return "33";
            }
            else if (v == "VILLA PLURIFAMILIARE")
            {
                return "34";
            }

            return null;
        }

        public static string ConvertFiniture(string v)
        {
            v = v.ToUpper();
            if (v == "FINITURE NORMALI")
            {
                return "2";
            }
            else if (v == "FINITURE DI LUSSO, PARTICOLARI D'EPOCA")
            {
                return "3";
            }
            else if (v == "FINITURE DI BASE, POPOLARI")
            {
                return "1";
            }
            else if (v == "DIMORA STORICA, ATTICO")
            {
                return "4";
            }

            return null;
        }

        public static QuoteProdottoContract CreateQPC()
        {
            QuoteProdottoContract q = new QuoteProdottoContract();
            q.CodiceApplicazione = "UL001P";
            q.Compagnia = "1";
            q.Device = DEVICE.WEB;
            q.CodiceProdotto = "000156";
            q.IdPuntoVendita = 175210000000000000;
            q.Username = "UTENTEWEB";
            q.Multicanale = 5;
            return q;
        }

        public static DettaglioQuoteProdotto TCCasa(PreventivatoriServiceClient ps, QuoteProdottoContract qpc)
        {
            try
            {
                return ps.GetSHPQuoteAsync(qpc).Result;
            }
            catch (Exception)
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
            ib.StringInfo = XmlManager.ParsificazioneXmlCasa(dqp.XmlResponse, user, "1", i);
            return ib;
        }

        public static DettaglioQuoteGaranzie TCDettaglio(PreventivatoriServiceClient ps, InfoBase ib)
        {
            try
            {
                return ps.PricingSHPQuoteAsync(ib).Result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
