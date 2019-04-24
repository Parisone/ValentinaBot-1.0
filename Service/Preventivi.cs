using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FullQuotationsService;
using Microsoft.BotBuilderSamples;
using PreventivoService;
using QuickQuotationsService;

namespace ComplexDialogBot.Service
{
    public static class Preventivi
    {
        public static string HealthPrev(Quotation q, UserProfile user)
        {
            string pre = string.Empty;
            pre += "Ecco le soluzioni pensate per il tuo caso nella provincia di " + user.ProvinciaNome + " \n\n";
            try
            {
                for (int i = 0; i < q.Proposte.Length; i++)
                {
                    pre += "- " + q.Proposte[i].Descrizione + ":\n";
                    pre += "  Prezzo: €" + q.Proposte[i].Soluzioni[0].Prezzo + "\n";
                }
            }
            catch (Exception ex)
            {
                return "Errore";
            }

            return pre;
        }

        public static string FamilyPrev(Quotation q, UserProfile user)
        {
            string pre = string.Empty;
            pre += "Ecco le soluzioni pensate per il tuo caso nel comune di " + user.Comune + ": \n\n";
            try
            {
                for (int i = 0; i < q.Proposte.Length; i++)
                {
                    pre += "- " + q.Proposte[i].Descrizione + ":\n";
                    pre += "  Prezzo: €" + q.Proposte[i].Soluzioni[0].Prezzo + "\n";
                }
            }
            catch (Exception ex)
            {
                return "Errore";
            }

            return pre;
        }

        public static string VeicoloPrev(PreventivatoreFullAutoContainer pfa, UserProfile user)
        {
            string pre = string.Empty;
            pre += "Ecco la soluzione pensata per il veicolo targato " + user.Targa + ": \n\n";
            try
            {
                pre += "  Il prezzo originario di €" + pfa.PreventivatoreFullAuto[0].listaCampi[18].valoreDefault + " è stato scontato ad €" + pfa.PreventivatoreFullAuto[0].listaCampi[14].valoreDefault;
                return pre;
            }
            catch (Exception ex)
            {
                return "Errore";
            }
        }

        public static string ViaggioPrev(DettaglioQuoteGaranzie dqg)
        {
            try
            {
                string pre = string.Empty;
                pre += "  Il prezzo originario di €" + dqg.PrezzoTotaleProposta + " è stato scontato ad €" + dqg.PrezzoWebProposta + ".\n";
                return pre;
            }
            catch (Exception ex)
            {
                return "Erroraccio";
            }
        }

        public static string HousePrev(DettaglioQuoteGaranzie dqg)
        {
            string pre = string.Empty;
            try
            {
                pre += "  Il prezzo originario di €" + dqg.PrezzoTotaleProposta + " è stato scontato ad €" + dqg.PrezzoWebProposta + ".\n";
            }
            catch (Exception ex)
            {
                return "Errore";
            }
            return pre;
        }

        public static void ToTxt(string prev, string id, int n)
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "Preventivo" + n + " " + id + ".txt"));
            outputFile.Write(prev);
            outputFile.Close();
        }
    }
}
