// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples
{
#pragma warning disable SA1124 // Do not use regions
    #region USING
    using System;
#pragma warning restore SA1124 // Do not use regions
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using FullQuotationsService;
    using global::ComplexDialogBot;
    using global::ComplexDialogBot.Service;
    using global::ComplexDialogBot.SQL;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Choices;
    using Microsoft.Bot.Schema;
    using Prev;
    using PreventivoService;
    using QuickQuotationsService;
    using QuotatoriRichContract = FullQuotationsService.QuotatoriRichContract;

    #endregion

    public class ComplexDialogBot : IBot
    {
#pragma warning disable SA1124 // Do not use regions
        #region TEXT MESSAGES

        // Messaggi standard del bot
        private const string WelcomeText = "Ciao, sono Valentina e sono qui per aiutarti! ";
#pragma warning restore SA1124 // Do not use regions
        private const string InitText = "Desideri calcolare un nuovo preventivo?\n\n(Scrivi \"BACK\" in ogni momento per tornare a questo punto)";
        private const string ChoiceText = "Cosa vorresti assicurare?";
        private const string EndText = "Grazie per aver parlato con me.\nSe hai bisogno di me sono qui!\nArrivederci.";
        private const string VeicoloText = "Perfavore inserica la targa priva di spazi.";
        private const string BirthText = "Inserisci la tua data di nascita (dd/mm/yyyy): ";
        private const string AgeText = "Inserisci la tua età: ";
        private const string FamilyText1 = "Inserisci il cap della località in cui abiti:";
        private const string FamilyText2 = "Indica la rendita mensile che vorresti avere in caso di grave evento: ";
        private const string FamilyText3 = "Sei proprietario della casa in cui vivi?";
        private const string FamilyText4 = "Hai figli non ancora economicamente indipendenti?";
        private const string HealtText1 = "Inserisci la tua provincia di residenza: ";
        private const string HealtText2 = "Per quali patologie vuoi sostegno?";
        private const string HealtText3 = "Per ricoveri/interventi ti affidi a: ";
        private const string TripText1 = "Inserisci il numero di partecipanti: ";
        private const string TripText2 = "Inserisci la data di inizio (dd/mm/yyyy): ";
        private const string TripText3 = "Inserisci la data di fine (dd/mm/yyyy):\n(NB: la durata del viaggio non può superare i 123 giorni)";
        private const string TripText4 = "Dove si trova la tua destinazione?";
        private const string HouseText1 = "Inserire i metri quadri della tua abitazione.\n(Puoi trovare un indicazione di tale valore sull'atto di compravendita o sul contratto di affitto";
        private const string HouseText2 = "Inserisci il cap della località in cui si trova l'abitazione:";
        private const string HouseText3 = "Inserisci il tipo di fabricato della tua abitazione: ";
        private const string HouseText4 = "Inserisci la qualità delle finiture: ";
        private const string HouseText5 = "Il fabbricato è: ";
        private const string HouseText6 = "Indicare la tipologia di dimora: ";
        private const string EmailText = "Perfavore inserisci la tua e-mail: ";
        private const string RightsText = "Dichiari di aver letto l'informativa sulla privacy?";
        private const string PrevText = "Il preventivo che ho calcolato per te è di ";
        #endregion

#pragma warning disable SA1124 // Do not use regions
        #region DEF. DIALOGS

        // Definizione Dialog, necessari per dividere le varie fasi del bot
        private const string TopLevelDialog = "dialog-topLevel";
#pragma warning restore SA1124 // Do not use regions
        private const string VeicleDialog = "dialog-veicle";
        private const string TripDialog = "dialog-trip";
        private const string EndDialog = "dialog-end";
        private const string FamilyDialog = "dialog-family";
        private const string HealtDialog = "dialog-healt";
        private const string HouseDialog = "dialog-house";
        private const string Recap = "dialog-recap";
        #endregion

#pragma warning disable SA1124 // Do not use regions
        #region DEF. PROMPT

        // Definisco i prompt, necessari per verificare le risposte dell'utente
        private const string InitPrompt = "prompt-init";
#pragma warning restore SA1124 // Do not use regions
        private const string ChoicePrompt = "prompt-choice";
        private const string EndPrompt = "prompt-end";
        private const string VeicoloPrompt1 = "prompt-v1";
        private const string VeicoloPrompt2 = "prompt-v2";
        private const string HealtPrompt1 = "prompt-h1";
        private const string HealtPrompt2 = "prompt-h2";
        private const string HealtPrompt3 = "prompt-h3";
        private const string HealtPrompt4 = "prompt-h4";
        private const string TripPrompt1 = "prompt-t1";
        private const string TripPrompt2 = "prompt-t2";
        private const string TripPrompt3 = "prompt-t3";
        private const string TripPrompt4 = "prompt-t4";
        private const string FamilyPrompt1 = "prompt-f1";
        private const string FamilyPrompt2 = "prompt-f2";
        private const string FamilyPrompt3 = "prompt-f3";
        private const string FamilyPrompt4 = "prompt-f4";
        private const string FamilyPrompt5 = "prompt-f5";
        private const string HousePrompt1 = "prompt-c1";
        private const string HousePrompt2 = "prompt-c2";
        private const string HousePrompt3 = "prompt-c3";
        private const string HousePrompt4 = "prompt-c4";
        private const string HousePrompt5 = "prompt-c5";
        private const string HousePrompt6 = "prompt-c6";
        private const string EmailPrompt = "prompt-email";
        private const string RightsPrompt = "prompt-rights";
        private const string PrevPrompt = "prompt-rights";
        #endregion

#pragma warning disable SA1124 // Do not use regions
        #region SERVICE

        // Servizi da chiamare
        private static Service1Client prev;
#pragma warning restore SA1124 // Do not use regions
        private static QuickQuotationsServiceClient quick;
        private static FullQuotationsServiceClient full;
        private static PreventivatoriServiceClient ps;
        #endregion

        private static UserProfile user;
        private static Guid g;

        private readonly DialogSet _dialogs;
        private readonly ComplexDialogBotAccessors _accessors;

        private string choice;
        private BotValidator botval;

        public ComplexDialogBot(ComplexDialogBotAccessors accessors)
        {
            _accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));
            _dialogs = new DialogSet(accessors.DialogStateAccessor);
            botval = new BotValidator();

#pragma warning disable SA1123 // Do not place regions within elements
            #region PROMPT

            // Aggiungo i prompt necessari al dialogSet, inserendo i validatori dove secessario
            _dialogs
#pragma warning restore SA1123 // Do not place regions within elements
                .Add(new TextPrompt(InitPrompt, botval.YesNoECoValidatorAsync))
                .Add(new TextPrompt(ChoicePrompt, botval.ChoiceValidatorAsync))
                .Add(new TextPrompt(EndPrompt))
                .Add(new TextPrompt(VeicoloPrompt1, botval.TargaValidatorAsync))
                .Add(new TextPrompt(VeicoloPrompt2, botval.DateBValidatorAsync))
                .Add(new TextPrompt(HealtPrompt1, botval.AgeValidatorAsync))
                .Add(new TextPrompt(HealtPrompt2, botval.ProvValidatorAsync))
                .Add(new TextPrompt(HealtPrompt3, botval.DiseaseValidatorAsync))
                .Add(new TextPrompt(HealtPrompt4, botval.StructValidatorAsync))
                .Add(new TextPrompt(TripPrompt1, botval.PartyValidatorAsync))
                .Add(new TextPrompt(TripPrompt2, botval.DateValidatorAsync))
                .Add(new TextPrompt(TripPrompt3, botval.DateValidatorAsync))
                .Add(new TextPrompt(TripPrompt4, botval.LocationValidatorAsync))
                .Add(new TextPrompt(EmailPrompt, botval.MailValidatorAsync))
                .Add(new TextPrompt(RightsPrompt, botval.YesNoValidatorAsync))
                .Add(new TextPrompt(FamilyPrompt1, botval.AgeValidatorAsync))
                .Add(new TextPrompt(FamilyPrompt2, botval.CapValidatorAsync))
                .Add(new TextPrompt(FamilyPrompt3, botval.RenValidatorAsync))
                .Add(new TextPrompt(FamilyPrompt4, botval.YesNoValidatorAsync))
                .Add(new TextPrompt(FamilyPrompt5, botval.YesNoValidatorAsync))
                .Add(new TextPrompt(HousePrompt1, botval.MQValidatorAsync))
                .Add(new TextPrompt(HousePrompt2, botval.CapValidatorAsync))
                .Add(new TextPrompt(HousePrompt3, botval.FabricatoValidatorAsync))
                .Add(new TextPrompt(HousePrompt4, botval.FinitureValidatorAsync))
                .Add(new TextPrompt(HousePrompt5, botval.PAValidatorAsync))
                .Add(new TextPrompt(HousePrompt6, botval.ASValidatorAsync));
            #endregion

            // Dialog set.
#pragma warning disable SA1123 // Do not place regions within elements
            #region INIT

            // Dialogo iniziale
            _dialogs.Add(new WaterfallDialog(TopLevelDialog)
#pragma warning restore SA1123 // Do not place regions within elements
                .AddStep(InitStepAsync)
                .AddStep(ChoiceStepAsync)
                .AddStep(StartSelectionStepAsync));
            #endregion

#pragma warning disable SA1123 // Do not place regions within elements
            #region VEICOLO

            // Dialogo veicolo
            _dialogs.Add(new WaterfallDialog(VeicleDialog)
#pragma warning restore SA1123 // Do not place regions within elements
                .AddStep(VeicoloStep1Async)
                .AddStep(VeicoloStep2Async)
                .AddStep(GoOnStepAsync));
            #endregion

#pragma warning disable SA1123 // Do not place regions within elements
            #region CASA

            // Dialogo casa
            _dialogs.Add(new WaterfallDialog(HouseDialog)
#pragma warning restore SA1123 // Do not place regions within elements
                .AddStep(CasaStep1Async)
                .AddStep(CasaStep2Async)
                .AddStep(CasaStep3Async)
                .AddStep(CasaStep4Async)
                .AddStep(CasaStep5Async)
                .AddStep(CasaStep6Async)
                .AddStep(GoOnStepAsync));
            #endregion

#pragma warning disable SA1123 // Do not place regions within elements
            #region SALUTE

            // Dialogo Salute
            _dialogs.Add(new WaterfallDialog(HealtDialog)
#pragma warning restore SA1123 // Do not place regions within elements
                .AddStep(HealtStep1Async)
                .AddStep(HealtStep2Async)
                .AddStep(HealtStep3Async)
                .AddStep(HealtStep4Async)
                .AddStep(GoOnStepAsync));
            #endregion

#pragma warning disable SA1123 // Do not place regions within elements
            #region VIAGGI

            // Dialogo Viaggi
            _dialogs.Add(new WaterfallDialog(TripDialog)
#pragma warning restore SA1123 // Do not place regions within elements
                .AddStep(TripStep1Async)
                .AddStep(TripStep2Async)
                .AddStep(TripStep3Async)
                .AddStep(TripStep4Async)
                .AddStep(GoOnStepAsync));
            #endregion

#pragma warning disable SA1123 // Do not place regions within elements
            #region FAMIGLIA

            // Dialogo famiglia
            _dialogs.Add(new WaterfallDialog(FamilyDialog)
#pragma warning restore SA1123 // Do not place regions within elements
                .AddStep(FamilyStep1Async)
                .AddStep(FamilyStep2Async)
                .AddStep(FamilyStep3Async)
                .AddStep(FamilyStep4Async)
                .AddStep(FamilyStep5Async)
                .AddStep(GoOnStepAsync));
            #endregion

#pragma warning disable SA1123 // Do not place regions within elements
            #region RECAP

            _dialogs.Add(new WaterfallDialog(Recap)
#pragma warning restore SA1123 // Do not place regions within elements
                .AddStep(RecapStepAsync));

            #endregion

#pragma warning disable SA1123 // Do not place regions within elements
            #region END

            // Dialogo standar di fine preventivo
            _dialogs.Add(new WaterfallDialog(EndDialog)
#pragma warning restore SA1123 // Do not place regions within elements
                .AddStep(EmailStepAsync)
                .AddStep(RightsStepAsync)
                .AddStep(PreventivoStepAsync)
                .AddStep(LoopStepAsync));
            #endregion
        }

#pragma warning disable SA1124 // Do not use regions
        #region INIT
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
#pragma warning restore SA1124 // Do not use regions
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            DialogContext dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
            DialogTurnResult results = await dialogContext.ContinueDialogAsync(cancellationToken);

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                switch (results.Status)
                {
                    case DialogTurnStatus.Cancelled:
                    case DialogTurnStatus.Empty:
                        // If there is no active dialog, we should clear the user info and start a new dialog.
                        await _accessors.UserProfileAccessor.SetAsync(turnContext, new UserProfile(), cancellationToken);
                        await _accessors.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
                        await dialogContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
                        break;
                    case DialogTurnStatus.Complete:
                        // If we just finished the dialog, capture and display the results.
                        string status = EndText;
                        await turnContext.SendActivityAsync(status);

                        // await _accessors.UserProfileAccessor.SetAsync(turnContext, userInfo, cancellationToken);
                        await _accessors.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
                        break;
                    case DialogTurnStatus.Waiting:
                        // If there is an active dialog, we don't need to do anything here.
                        break;
                }

                await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            }

            // Processes ConversationUpdate Activities to welcome the user.
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    // Passo dialog context
                    await SendWelcomeMessageAsync(turnContext, cancellationToken, dialogContext);
                }

                // Salvo lo stato della waterfall
                await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }
        }

        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken, DialogContext dialogContext)
        {
            // Messaggio di benvenuto che apparirà una singola volta, lo uso per inizializzare le variabili globali
            prev = new Service1Client();
            user = new UserProfile();
            quick = new QuickQuotationsServiceClient();
            full = new FullQuotationsServiceClient();
            ps = new PreventivatoriServiceClient();
            user.Flagmail = false;
            user.Flagprivacy = 0;
            user.Flagerror = false;
            user.Nprev = 0;
            g = Guid.NewGuid();
            SQLManager.Connect();

            // Controllo nuovo accesso
            foreach (ChannelAccount member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    Activity reply = turnContext.Activity.CreateReply();
                    reply.Text = WelcomeText;
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    await dialogContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
                }
            }
        }

        private static async Task<DialogTurnResult> InitStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Inizio Preventivo, punto di partenza di ogni loop
            if (user.Nprev > 0)
            {
                return await stepContext.PromptAsync(
                InitPrompt,
                new PromptOptions
                {
                // Messaggio iniziale
                Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = InitText,
                        SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                    new CardAction() { Title = "Si", Type = ActionTypes.ImBack, Value = "Si" },
                    new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No" },
                    new CardAction() { Title = "Visualizza Preventivi Calcolati", Type = ActionTypes.ImBack, Value = "Visualizza Preventivi Calcolati" },
                            },
                        },
                    },

                // Messaggio in caso di inserimento errato da parte dell'utente
                RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Scegli una delle tre opzioni, grazie.",
                        SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                    new CardAction() { Title = "Si", Type = ActionTypes.ImBack, Value = "Si" },
                    new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No" },
                    new CardAction() { Title = "Visualizza Preventivi Calcolati", Type = ActionTypes.ImBack, Value = "Visualizza Preventivi Calcolati" },
                            },
                        },
                    },
                },
                cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync(
                InitPrompt,
                new PromptOptions
                {
                    // Messaggio iniziale
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = InitText,
                        SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                    new CardAction() { Title = "Si", Type = ActionTypes.ImBack, Value = "Si" },
                    new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No" },
                            },
                        },
                    },

                    // Messaggio in caso di inserimento errato da parte dell'utente
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Scegli una delle tre opzioni, grazie.",
                        SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                    new CardAction() { Title = "Si", Type = ActionTypes.ImBack, Value = "Si" },
                    new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No" },
                            },
                        },
                    },
                },
                cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ChoiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Scelta del tipo di preventivo
            string answer = (string)stepContext.Result;

            if (answer.ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(TopLevelDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            // n.b. la risposta di un passaggio può essere utilizzata solo nel passaggio successivo, mentre viene verificata dai validator definiti nei prompt.
            if (answer.ToUpper() == "SI")
            {
                return await stepContext.PromptAsync(
                    ChoicePrompt,
                    new PromptOptions
                    {
                        Prompt = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Text = ChoiceText,
                            SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                {
                                new CardAction() { Title = "Veicolo", Type = ActionTypes.ImBack, Value = "Veicolo" },
                                new CardAction() { Title = "Viaggio", Type = ActionTypes.ImBack, Value = "Viaggio" },
                                new CardAction() { Title = "Salute", Type = ActionTypes.ImBack, Value = "Salute" },
                                new CardAction() { Title = "Famiglia", Type = ActionTypes.ImBack, Value = "Famiglia" },
                                new CardAction() { Title = "Casa", Type = ActionTypes.ImBack, Value = "Casa" },
                                },
                            },
                        },
                        RetryPrompt = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Text = "Selezionare una delle cinque possibilità",
                            SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                {
                                new CardAction() { Title = "Veicolo", Type = ActionTypes.ImBack, Value = "Veicolo" },
                                new CardAction() { Title = "Viaggio", Type = ActionTypes.ImBack, Value = "Viaggio" },
                                new CardAction() { Title = "Salute", Type = ActionTypes.ImBack, Value = "Salute" },
                                new CardAction() { Title = "Famiglia", Type = ActionTypes.ImBack, Value = "Famiglia" },
                                new CardAction() { Title = "Casa", Type = ActionTypes.ImBack, Value = "Casa" },
                                },
                            },
                        },
                    }, cancellationToken);
            }
            else if (answer.ToUpper() == "NO")
            {
                return await stepContext.EndDialogAsync(TopLevelDialog, cancellationToken);
            }
            else if (answer.ToUpper() == "VISUALIZZA PREVENTIVI CALCOLATI")
            {
                await stepContext.EndDialogAsync(TopLevelDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(Recap, null, cancellationToken);
            }

            return null;
        }

        private async Task<DialogTurnResult> StartSelectionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // In questa istanza finisce il primo dialog (TopLevelDialog) e il bot si dirama nelle varie possibilità di preventivo
            choice = (string)stepContext.Result;
            Activity reply = stepContext.Context.Activity.CreateReply();

            if (choice.ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(TopLevelDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            if (choice == "Veicolo")
            {
                user.Flagstep = 1;
                return await stepContext.BeginDialogAsync(VeicleDialog, null, cancellationToken);
            }
            else if (choice == "Salute")
            {
                user.Flagstep = 2;
                return await stepContext.BeginDialogAsync(HealtDialog, null, cancellationToken);
            }
            else if (choice == "Viaggio")
            {
                user.Flagstep = 3;
                return await stepContext.BeginDialogAsync(TripDialog, null, cancellationToken);
            }
            else if (choice == "Famiglia")
            {
                user.Flagstep = 4;
                return await stepContext.BeginDialogAsync(FamilyDialog, null, cancellationToken);
            }
            else if (choice == "Casa")
            {
                user.Flagstep = 5;
                return await stepContext.BeginDialogAsync(HouseDialog, null, cancellationToken);
            }

            return null;
        }
        #endregion

#pragma warning disable SA1124 // Do not use regions
        #region FAMIGLIA

        // Vari passaggi per preventivo famiglia.
        private async Task<DialogTurnResult> FamilyStep1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
#pragma warning restore SA1124 // Do not use regions
        {
            return await stepContext.PromptAsync(
                FamilyPrompt1,
                new PromptOptions
                {
                    Prompt = new Activity()
                    {
                        Type = ActivityTypes.Message,
                        Text = AgeText,
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Inserire un valore compreso tra 0 e 120.",
                    },
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> FamilyStep2Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(FamilyDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            user.Age = stepContext.Result.ToString();
            return await stepContext.PromptAsync(
                FamilyPrompt2,
                new PromptOptions
                {
                    Prompt = new Activity()
                    {
                        Type = ActivityTypes.Message,
                        Text = FamilyText1,
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Il cap inserito non è valido.",
                    },
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> FamilyStep3Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(FamilyDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            user.Cap = stepContext.Result.ToString();
            user.Comune = Global.COMUNE;
            user.Istat = Global.ISTAT;
            return await stepContext.PromptAsync(
                    FamilyPrompt3,
                    new PromptOptions
                    {
                        Prompt = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Text = FamilyText2,
                            SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                {
                                new CardAction() { Title = "500", Type = ActionTypes.ImBack, Value = "500" },
                                new CardAction() { Title = "1000", Type = ActionTypes.ImBack, Value = "1000" },
                                new CardAction() { Title = "1500", Type = ActionTypes.ImBack, Value = "1500" },
                                new CardAction() { Title = "2000", Type = ActionTypes.ImBack, Value = "2000" },
                                },
                            },
                        },
                        RetryPrompt = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Text = "Selezionare una delle quattro possibilità",
                            SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                {
                                new CardAction() { Title = "500", Type = ActionTypes.ImBack, Value = "500" },
                                new CardAction() { Title = "1000", Type = ActionTypes.ImBack, Value = "1000" },
                                new CardAction() { Title = "1500", Type = ActionTypes.ImBack, Value = "1500" },
                                new CardAction() { Title = "2000", Type = ActionTypes.ImBack, Value = "2000" },
                                },
                            },
                        },
                    }, cancellationToken);
        }

        private async Task<DialogTurnResult> FamilyStep4Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(FamilyDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            user.Ren = FamilyInfo.ConvertImporto(stepContext.Result.ToString());
            return await stepContext.PromptAsync(
            FamilyPrompt5,
            new PromptOptions
            {
                // Messaggio iniziale
                Prompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = FamilyText3,
                    SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                    new CardAction() { Title = "Si", Type = ActionTypes.ImBack, Value = "Si" },
                    new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No" },
                        },
                    },
                },

                // Messaggio in caso di inserimento errato da parte dell'utente
                RetryPrompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = "Risondi di si o di no, grazie.",
                    SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                    new CardAction() { Title = "Si", Type = ActionTypes.ImBack, Value = "Si" },
                    new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No" },
                        },
                    },
                },
            },
            cancellationToken);
        }

        private async Task<DialogTurnResult> FamilyStep5Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(FamilyDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            string ans = stepContext.Result.ToString();
            ans = ans.ToUpper();
            if (ans == "SI")
            {
                user.Propietario = 2;
            }
            else
            {
                user.Propietario = 1;
            }

            return await stepContext.PromptAsync(
            FamilyPrompt4,
            new PromptOptions
            {
                // Messaggio iniziale
                Prompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = FamilyText4,
                    SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                    new CardAction() { Title = "Si", Type = ActionTypes.ImBack, Value = "Si" },
                    new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No" },
                        },
                    },
                },

                // Messaggio in caso di inserimento errato da parte dell'utente
                RetryPrompt = new Activity
                {
                    Type = ActivityTypes.Message,
                    Text = "Risondi di si o di no, grazie.",
                    SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                    new CardAction() { Title = "Si", Type = ActionTypes.ImBack, Value = "Si" },
                    new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No" },
                        },
                    },
                },
            },
            cancellationToken);
        }
        #endregion

#pragma warning disable SA1124 // Do not use regions
        #region VEICOLO

        // Vari passaggi per preventivo veicolo
        private async Task<DialogTurnResult> VeicoloStep1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
#pragma warning restore SA1124 // Do not use regions
        {
            return await stepContext.PromptAsync(
                VeicoloPrompt1,
                new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = VeicoloText,
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "La targa inserita non è valida, riprova",
                    },
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> VeicoloStep2Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(VeicleDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            user.Targa = stepContext.Result.ToString().ToUpper();
            return await stepContext.PromptAsync(
                VeicoloPrompt2,
                new PromptOptions
                {
                    Prompt = new Activity()
                    {
                        Type = ActivityTypes.Message,
                        Text = BirthText,
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "La data di nascita inserita non risulta valida. Devi essere maggiorenne per richiedere un preventivo per un veicolo. Se hai sbagliato a digitare, riprova: ",
                    },
                },
                cancellationToken);
        }
        #endregion

#pragma warning disable SA1124 // Do not use regions
        #region SALUTE

        // Vari passaggi per preventivo salute.
        private async Task<DialogTurnResult> HealtStep1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
#pragma warning restore SA1124 // Do not use regions
        {
            return await stepContext.PromptAsync(
                HealtPrompt1,
                new PromptOptions
                {
                    Prompt = new Activity()
                    {
                        Type = ActivityTypes.Message,
                        Text = AgeText,
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Inserire un valore compreso tra 0 e 120.",
                    },
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> HealtStep2Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(HealtDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            user.Age = HealthInfo.ConvertAge(int.Parse(stepContext.Result.ToString()));
            return await stepContext.PromptAsync(
                HealtPrompt2,
                new PromptOptions
                {
                    Prompt = new Activity()
                    {
                        Type = ActivityTypes.Message,
                        Text = HealtText1,
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Provincia non trovata, inseriscila nuovamente.",
                    },
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> HealtStep3Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(HealtDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            // Utilizzo variabili globali al progetto (?) poichè i validatori non permettono di passare alcun parametro
            user.ProvRes = Global.CODEPROV;
            user.ProvinciaNome = (string)stepContext.Result;
            return await stepContext.PromptAsync(
                HealtPrompt3,
                new PromptOptions
                {
                    Prompt = new Activity()
                    {
                        Type = ActivityTypes.Message,
                        Text = HealtText2,
                        SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "Tutte", Type = ActionTypes.ImBack, Value = "Tutte" },
                                new CardAction() { Title = "Solo gravi", Type = ActionTypes.ImBack, Value = "Solo gravi" },
                            },
                        },
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Inserire una delle due scelte",
                        SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "Tutte", Type = ActionTypes.ImBack, Value = "Tutte" },
                                new CardAction() { Title = "Solo gravi", Type = ActionTypes.ImBack, Value = "Solo gravi" },
                            },
                        },
                    },
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> HealtStep4Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(HealtDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            user.Patologie = HealthInfo.ConvertDisease((string)stepContext.Result);
            return await stepContext.PromptAsync(
                HealtPrompt4,
                new PromptOptions
                {
                    Prompt = new Activity()
                    {
                        Type = ActivityTypes.Message,
                        Text = HealtText3,
                        SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "SSN", Type = ActionTypes.ImBack, Value = "SSN" },
                                new CardAction() { Title = "Strutture private", Type = ActionTypes.ImBack, Value = "Strutture private" },
                            },
                        },
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Inserire una delle due scelte",
                        SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "SSN", Type = ActionTypes.ImBack, Value = "SSN" },
                                new CardAction() { Title = "Strutture private", Type = ActionTypes.ImBack, Value = "Strutture private" },
                            },
                        },
                    },
                },
                cancellationToken);
        }
        #endregion

#pragma warning disable SA1124 // Do not use regions
        #region VIAGGI

        // Vari passaggi per preventivo viaggi.
        private async Task<DialogTurnResult> TripStep1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
#pragma warning restore SA1124 // Do not use regions
        {
            // Per una migliore comprensione del flagerror leggere lo step 4
            // il flagError serve per reinserire le date, il valore dei partecipanti sarà corretto, quindi il passaggio va saltato.
            if (user.Flagerror == true)
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

            return await stepContext.PromptAsync(
                TripPrompt1,
                new PromptOptions
                {
                    Prompt = new Activity()
                    {
                        Type = ActivityTypes.Message,
                        Text = TripText1,
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Inserire un numero positivo e <= di 9.",
                    },
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> TripStep2Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(TripDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            string t;

            // Il Flagerror serve per verificare che la data di inizio sia prima della data di fine viaggio.
            if (user.Flagerror == true)
            {
                t = "La data di fine viaggio risulta precedente alla data di fine viaggio.\n" + TripText2;
                user.Flagerror = false;
            }
            else
            {
                t = TripText2;
                user.NumPartecipanti = int.Parse(stepContext.Result.ToString());
            }

            return await stepContext.PromptAsync(
                TripPrompt2,
                new PromptOptions
                {
                    Prompt = new Activity()
                    {
                        Type = ActivityTypes.Message,
                        Text = t,
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Inserire una data valida.",
                    },
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> TripStep3Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(TripDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            user.DataInizio = (string)stepContext.Result;
            return await stepContext.PromptAsync(
                TripPrompt3,
                new PromptOptions
                {
                    Prompt = new Activity()
                    {
                        Type = ActivityTypes.Message,
                        Text = TripText3,
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Inserire una data valida.",
                    },
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> TripStep4Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(TripDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            user.DataFine = (string)stepContext.Result;

            // Utilizzo metodo di user per confrontare le due date e, nel caso in cui le informazioni inserite siano errate, termino il dialogo e lo faccio ripartire dall'inizio modificanto il flagerror
            if (user.DateToDate() == false)
            {
                user.Flagerror = true;
                await stepContext.EndDialogAsync(TripDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TripDialog, null, cancellationToken);
            }

            return await stepContext.PromptAsync(
                TripPrompt4,
                new PromptOptions
                {
                    Prompt = new Activity()
                    {
                        Type = ActivityTypes.Message,
                        Text = TripText4,
                        SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "Italia", Type = ActionTypes.ImBack, Value = "Italia" },
                                new CardAction() { Title = "Europa", Type = ActionTypes.ImBack, Value = "Europa" },
                                new CardAction() { Title = "Mondo", Type = ActionTypes.ImBack, Value = "Mondo" },
                            },
                        },
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Inserire una delle tre scelte",
                        SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction() { Title = "Italia", Type = ActionTypes.ImBack, Value = "Italia" },
                                new CardAction() { Title = "Europa", Type = ActionTypes.ImBack, Value = "Europa" },
                                new CardAction() { Title = "Mondo", Type = ActionTypes.ImBack, Value = "Mondo" },
                            },
                        },
                    },
                },
                cancellationToken);
        }
        #endregion

#pragma warning disable SA1124 // Do not use regions
        #region CASA
        private async Task<DialogTurnResult> CasaStep1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
#pragma warning restore SA1124 // Do not use regions
        {
            // Stesso utilizzo del FlagError di Viaggi
            if (user.Flagerror == true)
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

            return await stepContext.PromptAsync(
                HousePrompt1,
                new PromptOptions
                {
                    Prompt = new Activity()
                    {
                        Type = ActivityTypes.Message,
                        Text = HouseText1,
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "La superficie minima per un'abitazione è di 28 mq, mentre la massima è di 600mq, inserire un valore corretto.",
                    },
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> CasaStep2Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(HouseDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            if (user.Flagerror == true)
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

            user.MQ = stepContext.Result.ToString();
            return await stepContext.PromptAsync(
                HousePrompt2,
                new PromptOptions
                {
                    Prompt = new Activity()
                    {
                        Type = ActivityTypes.Message,
                        Text = HouseText2,
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Il cap inserito non è corretto.",
                    },
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> CasaStep3Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(HouseDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            if (user.Flagerror == true)
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }

            user.Cap = stepContext.Result.ToString();
            user.Comune = Global.COMUNE;
            user.ProvRes = Global.CODEPROV;
            user.Istat = Global.ISTAT;
            return await stepContext.PromptAsync(
                    HousePrompt3,
                    new PromptOptions
                    {
                        Prompt = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Text = HouseText3,
                            SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                {
                                new CardAction() { Title = "Condominio o abitazione plurifamiliare NON in centro storico", Type = ActionTypes.ImBack, Value = "Condominio o abitazione plurifamiliare NON in centro storico" },
                                new CardAction() { Title = "Condominio o abitazione plurifamiliare in centro storico", Type = ActionTypes.ImBack, Value = "Condominio o abitazione plurifamiliare in centro storico" },
                                new CardAction() { Title = "Villa o abitazione monofamiliare", Type = ActionTypes.ImBack, Value = "Villa o abitazione monofamiliare" },
                                new CardAction() { Title = "Villa plurifamiliare", Type = ActionTypes.ImBack, Value = "Villa plurifamiliare" },
                                },
                            },
                        },
                        RetryPrompt = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Text = "Selezionare una delle quattro possibilità",
                            SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                {
                                new CardAction() { Title = "Condominio o abitazione plurifamiliare NON in centro storico", Type = ActionTypes.ImBack, Value = "Condominio o abitazione plurifamiliare NON in centro storico" },
                                new CardAction() { Title = "Condominio o abitazione plurifamiliare in centro storico", Type = ActionTypes.ImBack, Value = "Condominio o abitazione plurifamiliare in centro storico" },
                                new CardAction() { Title = "Villa o abitazione monofamiliare", Type = ActionTypes.ImBack, Value = "Villa o abitazione monofamiliare" },
                                new CardAction() { Title = "Villa plurifamiliare", Type = ActionTypes.ImBack, Value = "Villa plurifamiliare" },
                                },
                            },
                        },
                    }, cancellationToken);
        }

        private async Task<DialogTurnResult> CasaStep4Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(HouseDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            if (user.Flagerror == true)
            {
                Activity reply = stepContext.Context.Activity.CreateReply();
                reply.Text = "- Finiture di base, popolari: fabbricato con finiture interne e utilizzo materiali di modesta qualita\n- Finiture normali: fabbricato con finiture interne e utilizzo materiali di qualità ordinaria\n- Finiture di lusso, particolari d’epoca: fabbricato con finiture interne molto accurate e utilizzo materiali tecnologicamente e qualitativamente superiori. Può contenere elementi architettonici/artistici di pregio.\n- Dimora storica, attico: fabbricato dichiarato “bene culturale” a sensi di legge oppure con finiture interne molto accurate e utilizzo materiali tecnologicamente e qualitativamente superiori posto all’ultimo piano abitabile di un edificio.";
                await stepContext.Context.SendActivityAsync(reply, cancellationToken);
                user.Flagerror = false;
            }
            else
            {
                user.Fabbricato = HouseInfo.ConvertFabricato(stepContext.Result.ToString());
            }

            return await stepContext.PromptAsync(
                    HousePrompt4,
                    new PromptOptions
                    {
                        Prompt = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Text = HouseText4,
                            SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                {
                                new CardAction() { Title = "Finiture normali", Type = ActionTypes.ImBack, Value = "Finiture normali" },
                                new CardAction() { Title = "Finiture di lusso, particolari d'epoca", Type = ActionTypes.ImBack, Value = "Finiture di lusso, particolari d'epoca" },
                                new CardAction() { Title = "Finiture di base, popolari", Type = ActionTypes.ImBack, Value = "Finiture di base, popolari" },
                                new CardAction() { Title = "Dimora storica, attico", Type = ActionTypes.ImBack, Value = "Dimora storica, attico" },
                                new CardAction() { Title = "Non so come rispondere", Type = ActionTypes.ImBack, Value = "Non so come rispondere" },
                                },
                            },
                        },
                        RetryPrompt = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Text = "Selezionare una delle quattro possibilità",
                            SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                {
                                new CardAction() { Title = "Finiture normali", Type = ActionTypes.ImBack, Value = "Finiture normali" },
                                new CardAction() { Title = "Finiture di lusso, particolari d'epoca", Type = ActionTypes.ImBack, Value = "Finiture di lusso, particolari d'epoca" },
                                new CardAction() { Title = "Finiture di base, popolari", Type = ActionTypes.ImBack, Value = "Finiture di base, popolari" },
                                new CardAction() { Title = "Dimora storica, attico", Type = ActionTypes.ImBack, Value = "Dimora storica, attico" },
                                new CardAction() { Title = "Non so come rispondere", Type = ActionTypes.ImBack, Value = "Non so come rispondere" },
                                },
                            },
                        },
                    }, cancellationToken);
        }

        private async Task<DialogTurnResult> CasaStep5Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(HouseDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            // Selezionando "Non so come rispondere" utilizzeremo il flag error per tornare indietro ed aggiungere i dettagli desiderati
            if (stepContext.Result.ToString().ToUpper() == "NON SO COME RISPONDERE")
            {
                user.Flagerror = true;
                await stepContext.EndDialogAsync(HouseDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(HouseDialog, null, cancellationToken);
            }

            user.Finiture = HouseInfo.ConvertFiniture(stepContext.Result.ToString());
            return await stepContext.PromptAsync(
                    HousePrompt5,
                    new PromptOptions
                    {
                        Prompt = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Text = HouseText5,
                            SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                {
                                new CardAction() { Title = "Di proprietà", Type = ActionTypes.ImBack, Value = "Di proprieta" },
                                new CardAction() { Title = "In affitto", Type = ActionTypes.ImBack, Value = "In affitto" },
                                },
                            },
                        },
                        RetryPrompt = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Text = "Selezionare una delle due possibilità",
                            SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                {
                                new CardAction() { Title = "Di proprietà", Type = ActionTypes.ImBack, Value = "Di proprieta" },
                                new CardAction() { Title = "In affitto", Type = ActionTypes.ImBack, Value = "In affitto" },
                                },
                            },
                        },
                    }, cancellationToken);
        }

        private async Task<DialogTurnResult> CasaStep6Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            user.PA = HouseInfo.ConvertPA(stepContext.Result.ToString());
            return await stepContext.PromptAsync(
                    HousePrompt6,
                    new PromptOptions
                    {
                        Prompt = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Text = HouseText6,
                            SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                {
                                new CardAction() { Title = "Abituale", Type = ActionTypes.ImBack, Value = "Abituale" },
                                new CardAction() { Title = "Saltuaria", Type = ActionTypes.ImBack, Value = "Saltuaria" },
                                },
                            },
                        },
                        RetryPrompt = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Text = "Selezionare una delle due possibilità",
                            SuggestedActions = new SuggestedActions()
                            {
                                Actions = new List<CardAction>()
                                {
                                new CardAction() { Title = "Abituale", Type = ActionTypes.ImBack, Value = "Abituale" },
                                new CardAction() { Title = "Saltuaria", Type = ActionTypes.ImBack, Value = "Saltuaria" },
                                },
                            },
                        },
                    }, cancellationToken);
        }
        #endregion

#pragma warning disable SA1124 // Do not use regions
        #region END

        // Sezione finale, calcolo del preventivo
        private async Task<DialogTurnResult> GoOnStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
#pragma warning restore SA1124 // Do not use regions
        {
            // Metodo finale di tutti i dialoghi precedenti, serve a salvare l'ultimo dato inserito
            if (user.Flagstep == 1)
            {
                if (stepContext.Result.ToString().ToUpper() == "BACK")
                {
                    await stepContext.EndDialogAsync(VeicleDialog, cancellationToken);
                    return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
                }

                user.DataNascita = stepContext.Result.ToString();
            }
            else if (user.Flagstep == 2)
            {
                if (stepContext.Result.ToString().ToUpper() == "BACK")
                {
                    await stepContext.EndDialogAsync(HealtDialog, cancellationToken);
                    return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
                }

                user.Strutture = HealthInfo.ConvertStruct((string)stepContext.Result);
            }
            else if (user.Flagstep == 3)
            {
                if (stepContext.Result.ToString().ToUpper() == "BACK")
                {
                    await stepContext.EndDialogAsync(TripDialog, cancellationToken);
                    return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
                }

                user.Dest = ViaggioInfo.ConvertDest((string)stepContext.Result);
            }
            else if (user.Flagstep == 4)
            {
                if (stepContext.Result.ToString().ToUpper() == "BACK")
                {
                    await stepContext.EndDialogAsync(FamilyDialog, cancellationToken);
                    return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
                }

                string ans = stepContext.Result.ToString();
                ans = ans.ToUpper();
                if (ans == "SI")
                {
                    user.Figli = 2;
                }
                else
                {
                    user.Figli = 1;
                }
            }
            else if (user.Flagstep == 5)
            {
                if (stepContext.Result.ToString().ToUpper() == "BACK")
                {
                    await stepContext.EndDialogAsync(HouseDialog, cancellationToken);
                    return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
                }

                user.AS = HouseInfo.ConvertAS(stepContext.Result.ToString());
            }

            return await stepContext.BeginDialogAsync(EndDialog, null, cancellationToken);
        }

        private async Task<DialogTurnResult> EmailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Una volta inserita non sarà più chiesta nei successivi loop
            if (user.Flagmail == false)
            {
                Activity reply = stepContext.Context.Activity.CreateReply();
                reply.Text = EmailText;
                return await stepContext.PromptAsync(
                    EmailPrompt,
                    new PromptOptions
                    {
                        Prompt = new Activity()
                        {
                            Type = ActivityTypes.Message,
                            Text = EmailText,
                        },
                        RetryPrompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "E-Mail non valida, riprova.",
                        },
                    },
                    cancellationToken);
            }
            else
            {
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
        }

        private async Task<DialogTurnResult> RightsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(EndDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            if (user.Flagmail == false)
            {
                user.Email = (string)stepContext.Result;
                user.Flagmail = true;
            }

            string t;

            // Darà la possibilità di accettare i diritti per una volta dopo averli rifiutati, per poi tornare al punto di partenza (se rifiutati nuovamente).
            if (user.Flagprivacy == 0)
            {
                t = RightsText;
            }
            else
            {
                t = "ATTENZIONE! Se non accetti, NON ti sarà possibile continuare!\n" + RightsText;
            }

            return await stepContext.PromptAsync(
                RightsPrompt,
                new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = t,
                        SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                    new CardAction() { Title = "Si", Type = ActionTypes.ImBack, Value = "Si" },
                    new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No" },
                    new CardAction() { Title = "Leggi l'informativa sulla Privacy.", Type = ActionTypes.OpenUrl, Value = "https://www.realemutua.it/Shared%20Documents/Privacy/Nuova%20Informativa%20RMA%20PROSPECT.pdf", },
                            },
                        },
                    },
                    RetryPrompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "Risondi di si o di no, grazie.",
                        SuggestedActions = new SuggestedActions()
                        {
                            Actions = new List<CardAction>()
                            {
                    new CardAction() { Title = "Si", Type = ActionTypes.ImBack, Value = "Si" },
                    new CardAction() { Title = "No", Type = ActionTypes.ImBack, Value = "No" },
                            },
                        },
                    },
                },
                cancellationToken);
            }

        private async Task<DialogTurnResult> PreventivoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result.ToString().ToUpper() == "BACK")
            {
                await stepContext.EndDialogAsync(EndDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
            }

            Activity rep = stepContext.Context.Activity.CreateReply();
            rep.Text = "Attendere, sto calcolando il tuo preventivo...";
            Activity reply = stepContext.Context.Activity.CreateReply();
            if ((string)stepContext.Result == "Si")
            {
                await stepContext.Context.SendActivityAsync(rep, cancellationToken);
                user.Flagprivacy = 0;

                // Chiamata dei servizi.
                if (user.Flagstep == 2)
                {
                    // Salute
                    ProposteContract prop = HealthInfo.CreateProp(user);
                    Quotation q = HealthInfo.TCHealt(quick, prop);
                    reply.Text = Preventivi.HealthPrev(q,user);
                    SQLManager.AddDBSalute(user, g.ToString(), reply.Text);
                }
                else if (user.Flagstep == 4)
                {
                    // Famiglia
                    QuickFirstProtectionContract fpr = FamilyInfo.CreateFPR(user);
                    Quotation q = FamilyInfo.TCFamily(quick, fpr);
                    reply.Text = Preventivi.FamilyPrev(q,user);
                    SQLManager.AddDBFamiglia(user, g.ToString(), reply.Text);
                }
                else if (user.Flagstep == 1)
                {
                    // Veicolo
                    QuotatoriRichContract qrc = VeicoloInfo.Send();
                    PreventivatoreFullAutoContainer pfa = VeicoloInfo.TCVeicolo(full, qrc);
                    qrc = VeicoloInfo.CreateQRC(user, pfa.CookieValue);
                    pfa = VeicoloInfo.TCVeicolo(full, qrc);
                    reply.Text = Preventivi.VeicoloPrev(pfa,user);
                    SQLManager.AddDBAuto(user, g.ToString(), reply.Text);
                }
                else if (user.Flagstep == 3)
                {
                    // Viaggio
                    string pre = "Ecco le soluzioni pensate per il tuo viaggio per " + user.NumPartecipanti + " previsto dal " + user.DataInizio + " al " + user.DataFine + ": \n\n";
                    QuoteProdottoContract qpc = ViaggioInfo.CreateQCP();
                    DettaglioQuoteProdotto dqp = ViaggioInfo.TCViaggio(ps, qpc);
                    for (int i = 0; i < 2; i++)
                    {
                        if (i == 0)
                        {
                            pre += "- Formula Silver:\n";
                        }
                        else if (i == 1)
                        {
                            pre += "- Forumla Gold:\n";
                        }

                        InfoBase ib = ViaggioInfo.createIB(dqp, user, i + 1);
                        DettaglioQuoteGaranzie dqg = ViaggioInfo.TCDettaglio(ps, ib);
                        pre += Preventivi.ViaggioPrev(dqg);
                    }

                    reply.Text = pre;
                    SQLManager.AddDBViaggi(user, g.ToString(), pre);
                }
                else if (user.Flagstep == 5)
                {
                    // Casa
                    string pre = "Ecco le soluzioni pensate per la tua abitazione situata a " + user.Comune + ": \n\n";
                    QuoteProdottoContract qpc = HouseInfo.CreateQPC();
                    DettaglioQuoteProdotto dqp = HouseInfo.TCCasa(ps, qpc);
                    for (int i = 0; i < 4; i++)
                    {
                        if (i == 0)
                        {
                            pre += "- Formula Full:\n";
                        }
                        else if (i == 1)
                        {
                            pre += "- Formula furto: \n";
                        }
                        else if (i == 2)
                        {
                            pre += "- Formula casa: \n";
                        }
                        else if (i == 3)
                        {
                            pre += "- Formula persona: \n";
                        }

                        InfoBase ib = HouseInfo.createIB(dqp, user, i + 1);
                        DettaglioQuoteGaranzie dqg = HouseInfo.TCDettaglio(ps, ib);
                        pre += Preventivi.HousePrev(dqg);
                    }

                    reply.Text = pre;
                    SQLManager.AddDBCasa(user, g.ToString(), pre);
                }

                Preventivi.ToTxt(reply.Text, g.ToString(), user.Nprev);
                await stepContext.PromptAsync(PrevPrompt, new PromptOptions { Prompt = reply }, cancellationToken);
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else if (user.Flagprivacy > 0)
            {
                user.Flagprivacy = 0;
                await stepContext.EndDialogAsync(EndDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(TopLevelDialog, cancellationToken);
            }
            else
            {
                user.Flagprivacy = 1;
                await stepContext.EndDialogAsync(EndDialog, cancellationToken);
                return await stepContext.BeginDialogAsync(EndDialog, null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> LoopStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Si ricomincia dall'inizio
                return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
        }
        #endregion

#pragma warning disable SA1124 // Do not use regions
        #region RECAP
        private async Task<DialogTurnResult> RecapStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Activity reply = stepContext.Context.Activity.CreateReply();
            reply.Text = SQLManager.GetDBPreventivi(g.ToString());
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            await stepContext.EndDialogAsync(Recap, cancellationToken);
            return await stepContext.BeginDialogAsync(TopLevelDialog, null, cancellationToken);
        }
        #endregion
    }
#pragma warning restore SA1124 // Do not use regions
}
