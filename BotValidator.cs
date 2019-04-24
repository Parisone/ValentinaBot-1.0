using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ComplexDialogBot.Service;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.BotBuilderSamples;

namespace ComplexDialogBot
{
    public class BotValidator
    {
        public async Task<bool> YesNoValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Mi scusi non ho capito, selezionare si o no.",
                    cancellationToken: cancellationToken);
                return false;
            }

            string ans = promptContext.Recognized.Value;
            ans = ans.ToUpper();

            if (ans == "SI" || ans == "NO" || ans == "BACK")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> YesNoECoValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Mi scusi non ho capito, selezionare si o no.",
                    cancellationToken: cancellationToken);
                return false;
            }

            string ans = promptContext.Recognized.Value;
            ans = ans.ToUpper();

            if (ans == "SI" || ans == "NO" || ans == "VISUALIZZA PREVENTIVI CALCOLATI" || ans == "BACK")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ChoiceValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Mi scusi non ho capito, selezionare una delle scelte.",
                    cancellationToken: cancellationToken);
                return false;
            }

            string ans = promptContext.Recognized.Value;

            ans = ans.ToUpper();

            if (ans == "VEICOLO" || ans == "VIAGGIO" || ans == "SALUTE" || ans == "FAMIGLIA" || ans == "CASA" || ans == "BACK")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> TargaValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Inserire un formato valido.",
                    cancellationToken: cancellationToken);
                return false;
            }

            int f = 0;

            string ans = promptContext.Recognized.Value;

            if (ans.ToUpper() == "BACK")
            {
                return true;
            }

            if (ans.Length != 7)
            {
                return false;
            }

            string c1 = ans.Substring(0, 2);
            string n = ans.Substring(2, 3);
            string c2 = ans.Substring(5, 2);
            c1 = c1.ToUpper();
            c2 = c2.ToUpper();
            if (c1 == "EE" || c2 == "EE")
            {
                return false;
            }

            for (int i = 0; i < 2; i++)
            {
                if (Convert.ToInt32(c1[i]) >= 65 && Convert.ToInt32(c1[i]) <= 90 && Convert.ToInt32(c1[i]) != 73 && Convert.ToInt32(c1[i]) != 79)
                {
                    f++;
                }

                if (Convert.ToInt32(c2[i]) >= 65 && Convert.ToInt32(c2[i]) <= 90 && Convert.ToInt32(c2[i]) != 73 && Convert.ToInt32(c2[i]) != 79)
                {
                    f++;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (Convert.ToInt32(n[i]) >= 48 && Convert.ToInt32(n[i]) <= 57)
                {
                    f++;
                }
            }

            if (f < 7)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> AgeValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Inserire un valore numerico.",
                    cancellationToken: cancellationToken);
                return false;
            }

            if (promptContext.Recognized.Value.ToUpper() == "BACK")
            {
                return true;
            }

            int age;
            try
            {
                age = int.Parse(promptContext.Recognized.Value);
            }
            catch
            {
                return false;
            }

            if (age < 0 || age > 120)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CapValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Inserire un valore numerico di 5 cifre.",
                    cancellationToken: cancellationToken);
                return false;
            }

            if (promptContext.Recognized.Value.ToUpper() == "BACK")
            {
                return true;
            }

            try
            {
                int.Parse(promptContext.Recognized.Value);
            }
            catch
            {
                return false;
            }

            string cap = promptContext.Recognized.Value;
            if (cap.Length != 5)
            {
                return false;
            }

            if (!CapReader.FindComune(cap))
            {
                return false;
            }

            return true;
        }

        public async Task<bool> RenValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Inserire un valore numerico di 5 cifre.",
                    cancellationToken: cancellationToken);
                return false;
            }

            if (promptContext.Recognized.Value.ToUpper() == "BACK")
            {
                return true;
            }

            int r;
            try
            {
                r = int.Parse(promptContext.Recognized.Value);
            }
            catch
            {
                return false;
            }

            if (r == 500 || r == 1000 || r == 1500 || r == 2000)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> ProvValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Inserire un formato valido.",
                    cancellationToken: cancellationToken);

                return false;
            }

            if (promptContext.Recognized.Value.ToUpper() == "BACK")
            {
                return true;
            }

            return XmlManager.ReadXml(promptContext.Recognized.Value);
        }

        public async Task<bool> DiseaseValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Inserire un formato valido.",
                    cancellationToken: cancellationToken);

                return false;
            }

            string ans = promptContext.Recognized.Value;
            ans = ans.ToUpper();

            if (ans == "TUTTE" || ans == "SOLO GRAVI" || ans == "BACK")
            {
                return true;
            }

            return false;
        }

        public async Task<bool> StructValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Inserire un formato valido.",
                    cancellationToken: cancellationToken);

                return false;
            }

            string ans = promptContext.Recognized.Value;
            ans = ans.ToUpper();

            if (ans == "SSN" || ans == "STRUTTURE PRIVATE" || ans == "BACK")
            {
                return true;
            }

            return false;
        }

        public async Task<bool> PartyValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Inserire un valore numerico intero.",
                    cancellationToken: cancellationToken);
                return false;
            }

            if (promptContext.Recognized.Value.ToUpper() == "BACK")
            {
                return true;
            }

            int ans;
            try
            {
                ans = int.Parse(promptContext.Recognized.Value);
            }
            catch
            {
                return false;
            }

            if (ans > 0 && ans < 10)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> FinitureValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Formato non supportato.",
                    cancellationToken: cancellationToken);
                return false;
            }

            string ans = promptContext.Recognized.Value;
            ans = ans.ToUpper();
            if (ans == "FINITURE NORMALI" || ans == "FINITURE DI LUSSO, PARTICOLARI D'EPOCA" || ans == "FINITURE DI BASE, POPOLARI" || ans == "DIMORA STORICA, ATTICO" || ans == "NON SO COME RISPONDERE" || ans == "BACK")
            {
                return true;
            }

            return false;
        }

        public async Task<bool> ASValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Formato non supportato.",
                    cancellationToken: cancellationToken);
                return false;
            }

            string ans = promptContext.Recognized.Value;
            ans = ans.ToUpper();
            if (ans == "ABITUALE" || ans == "SALTUARIA" || ans == "BACK")
            {
                return true;
            }

            return false;
        }

        public async Task<bool> PAValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Formato non supportato.",
                    cancellationToken: cancellationToken);
                return false;
            }

            string ans = promptContext.Recognized.Value;
            ans = ans.ToUpper();
            if (ans == "DI PROPRIETA" || ans == "IN AFFITTO" || ans == "BACK")
            {
                return true;
            }

            return false;
        }

        public async Task<bool> FabricatoValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Formato non supportato.",
                    cancellationToken: cancellationToken);
                return false;
            }

            string ans = promptContext.Recognized.Value;
            ans = ans.ToUpper();
            if (ans == "CONDOMINIO O ABITAZIONE PLURIFAMILIARE NON IN CENTRO STORICO" || ans == "CONDOMINIO O ABITAZIONE PLURIFAMILIARE IN CENTRO STORICO" || ans == "VILLA O ABITAZIONE MONOFAMILIARE" || ans == "VILLA PLURIFAMILIARE" || ans == "BACK")
            {
                return true;
            }

            return false;
        }

        public async Task<bool> MQValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Inserire un valore numerico intero.",
                    cancellationToken: cancellationToken);
                return false;
            }

            if (promptContext.Recognized.Value.ToUpper() == "BACK")
            {
                return true;
            }

            int ans;
            try
            {
                ans = int.Parse(promptContext.Recognized.Value);
            }
            catch
            {
                return false;
            }

            if (ans >= 28 && ans <= 600)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DateValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Inserire una data valida (dd/mm/yyyy).",
                    cancellationToken: cancellationToken);

                return false;
            }

            if (promptContext.Recognized.Value.ToUpper() == "BACK")
            {
                return true;
            }

            // Check whether any of the recognized date-times are appropriate,
            // and if so, return the first appropriate date-time.
            DateTime d;

            try
            {
                d = DateTime.Parse(promptContext.Recognized.Value);
            }
            catch (Exception ex)
            {
                return false;
            }

            DateTime now = DateTime.Now;
            DateTime post = DateTime.Now.AddYears(1);

            if (d.Date > post.Date)
            {
                await promptContext.Context.SendActivityAsync(
                    "Non è possibile preventivare viaggi che inizieranno o finiranno tra più di un anno!",
                    cancellationToken: cancellationToken);
                return false;
            }

            if (d.Date > now.Date)
            {
                return true;
            }

            await promptContext.Context.SendActivityAsync(
                    "Mi dispiace, non posso ancora tornare indietro nel tempo. :(",
                    cancellationToken: cancellationToken);

            return false;
        }

        public async Task<bool> MailValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Inserire un formato valido.",
                    cancellationToken: cancellationToken);

                return false;
            }

            if (promptContext.Recognized.Value.ToUpper() == "BACK")
            {
                return true;
            }

            // lunghezza di un nome per mail 6<x<30 caratteri
            // lunghezza minima dominio (compresa estensione) 6
            string mail = promptContext.Recognized.Value;
            string name = string.Empty;
            string domain = string.Empty;
            int f = 0;
            for (int i = 0; i < mail.Length; i++)
            {
                if (mail[i] == '@')
                {
                    f = 1;
                    name = mail.Substring(0, i);
                    domain = mail.Substring(i + 1, mail.Length - i - 1);
                    break;
                }
            }

            if (f == 0)
            {
                return false;
            }

            if ((name.Length < 6 && name != "pippo") || name.Length > 30)
            {
                return false;
            }

            if (domain.Length < 6 && domain != "pippo")
            {
                return false;
            }

            return true;
        }

        public async Task<bool> LocationValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Inserire un formato valido.",
                    cancellationToken: cancellationToken);

                return false;
            }

            string ans = promptContext.Recognized.Value;
            ans = ans.ToUpper();

            if (ans == "ITALIA" || ans == "EUROPA" || ans == "MONDO" || ans == "BACK")
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DateBValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded)
            {
                await promptContext.Context.SendActivityAsync(
                    "Inserire un formato valido.",
                    cancellationToken: cancellationToken);

                return false;
            }

            if (promptContext.Recognized.Value.ToUpper() == "BACK")
            {
                return true;
            }

            DateTime d;
            try
            {
                d = DateTime.Parse(promptContext.Recognized.Value);
            }
            catch (Exception ex)
            {
                return false;
            }

            DateTime now = DateTime.Now;

            int anniBisestili = 0;

            for (int i = d.Year; i <= now.Year; i++)
            {
                if (DateTime.IsLeapYear(i))
                {
                    anniBisestili++;
                }
            }

            double diffDate = now.Subtract(d).TotalDays - anniBisestili;

            if (diffDate / 365 >= 18)
            {
                return true;
            }

            await promptContext.Context.SendActivityAsync(
                    "Devi avere più di 18 anni.",
                    cancellationToken: cancellationToken);

            return false;
        }
    }
}
