using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.BotBuilderSamples;

namespace ComplexDialogBot.Service
{
    public static class XmlManager
    {
        public static bool ReadXml(string value)
        {
            if (value.Length == 2)
            {
                value = value.ToUpper();
            }
            else
            {
                string i = value.Substring(0, 1);
                string tot = value.Substring(1, value.Length - 1);
                i = i.ToUpper();
                tot = tot.ToLower();
                value = i + tot;
            }

            int f = 0;
            XmlTextReader reader = new XmlTextReader("C:\\Users\\Daniele Parsi\\Desktop\\Progetto Daniele - Copia\\Progetto Daniele\\ComplexDialogBot\\Model\\province.xml");
            while (reader.Read())
            {
                if (f == 1)
                {
                    f = 0;
                    if (reader.Value == value)
                    {
                        f = 2;
                    }
                }
                else if (f == 3)
                {
                    Global.CODEPROV = reader.Value;
                    return true;
                }

                if (reader.HasAttributes)
                {
                    while (reader.MoveToNextAttribute())
                    {
                        if ((reader.Value == "nome" || reader.Value == "sigla_automobilistica") && f == 0)
                        {
                            f = 1;
                        }
                        else if (reader.Value == "value" && f == 2)
                        {
                            f = 3;
                        }
                    }

                    // Move the reader back to the element node.
                    reader.MoveToElement();
                }
            }

            return false;
        }

        public static string ParsificazioneXmlViaggi(string xmlResponse,  UserProfile user, int step, int proposalCode, bool cancellaNodoMessaggiErrore = false)
        {
            string xml = string.Empty;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlResponse);
            if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                doc.RemoveChild(doc.FirstChild);
            }

            #region Namespace xml
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            if (doc.DocumentElement.Attributes["xmlns:S"] != null)
            {
                string s = doc.DocumentElement.Attributes["xmlns:S"].Value;
                nsmgr.AddNamespace("S", s);
            }

            XmlNode nodeRoot = doc.SelectSingleNode("S:Envelope/S:Body", nsmgr);
            if (nodeRoot != null)
            {
                XmlNode nodeNamespace = nodeRoot.LastChild;

                if (nodeNamespace.Attributes["xmlns:ns3"] != null)
                {
                    string ns3 = nodeNamespace.Attributes["xmlns:ns3"].Value;
                    nsmgr.AddNamespace("ns3", ns3);
                }

                if (nodeNamespace.Attributes["xmlns:ns2"] != null)
                {
                    string ns2 = nodeNamespace.Attributes["xmlns:ns2"].Value;
                    nsmgr.AddNamespace("ns2", ns2);
                }

                if (nodeNamespace.Attributes["xmlns:ns7"] != null)
                {
                    string ns7 = nodeNamespace.Attributes["xmlns:ns7"].Value;
                    nsmgr.AddNamespace("ns7", ns7);
                }

                if (nodeNamespace.Attributes["xmlns:ns9"] != null)
                {
                    string ns9 = nodeNamespace.Attributes["xmlns:ns9"].Value;
                    nsmgr.AddNamespace("ns9", ns9);
                }
            }
            #endregion

            // nodo quote principale
            XmlNode nodoQuote = null;
            XmlNodeList listaNodiFattori = null;
            XmlNodeList ListaNodiSezioni = null;

            #region Inietto il numero di preventivo (appena staccato chiamando il SaveSHPQuote oppure re-iniettato ogni volta) e rimuovo eventuali nodi errore

            string stringaQuote = "S:Envelope/S:Body/{0}:{1}/return/{0}:quote";

            XmlNodeList nodoQuoteList = doc.SelectNodes(string.Format(stringaQuote, "ns3", "getSHPQuoteResponse"), nsmgr);

            if (nodoQuoteList != null & nodoQuoteList.Count <= 0)
            {
                nodoQuoteList = doc.SelectNodes(string.Format(stringaQuote, "ns3", "pricingSHPQuoteResponse"), nsmgr);
            }

            if (nodoQuoteList != null & nodoQuoteList.Count <= 0)
            {
                nodoQuoteList = doc.SelectNodes(string.Format(stringaQuote, "ns2", "getSHPQuoteResponse"), nsmgr);
            }

            if (nodoQuoteList != null & nodoQuoteList.Count <= 0)
            {
                nodoQuoteList = doc.SelectNodes(string.Format(stringaQuote, "ns2", "pricingSHPQuoteResponse"), nsmgr);
            }

            if (nodoQuoteList != null & nodoQuoteList.Count > 0)
            {
                nodoQuote = nodoQuoteList[0];

                if (cancellaNodoMessaggiErrore)
                {
                    // RIMUOVO IL PRECEDENTE ERRORE SE C'E' STATO
                    XmlNode nodoErrore = nodoQuote.SelectSingleNode("ns7:messages", nsmgr);

                    if (nodoErrore != null)
                    {
                        nodoErrore.RemoveAll();
                    }
                }
            }

            #endregion

            if (step == 2)
            {
                #region Iniettando la data di policyExpirationDate

                // Chat con Mirko del 24 settembre 2018

                /*
                 * di default è impostata a 2 giorni della data effetto
                es: data effetto 24/09/2018 data scadenza: 26/09/2018

                per il viaggi mettere la data di fine viaggio
                */

                XmlNode nodoPolicyExpirationDate = nodoQuote.SelectSingleNode("ns7:policyExpirationDate", nsmgr);
                XmlNode nodoDataPol = null;

                if (nodoPolicyExpirationDate != null)
                {
                    // nodoDataPol = nodoPolicyExpirationDate.SelectSingleNode("ns2:data", nsmgr);
                    nodoDataPol = nodoPolicyExpirationDate.ChildNodes[0];

                    string dataPolicy = string.Format("{0}-{1}-{2}T00:00:00+0{3}:00", user.DataFine.Split('/')[2],
                                                                              (user.DataFine.Split('/')[1].Length == 1) ? "0" + user.DataFine.Split('/')[1] : user.DataFine.Split('/')[1],
                                                                              user.DataFine.Split('/')[0],
                                                                              TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalHours.ToString());
                    if (nodoDataPol != null)
                    {
                        nodoDataPol.InnerText = dataPolicy;
                    }
                    else
                    {
                        nodoDataPol = doc.CreateElement("ns2:data", nsmgr.LookupNamespace("ns2"));
                        nodoDataPol.InnerText = dataPolicy;
                        nodoPolicyExpirationDate.AppendChild(nodoDataPol);
                    }
                }

                #endregion

                #region Iniettando nodo requesterEMailAddress per gestione tracking preventivi Casa/Viaggi (IM9950680 RGI). Mail del 02/01/2019 ore 11:07 da Mirko Rizzo

                XmlNode nodoRequesterEmail = nodoQuote.SelectSingleNode("ns7:requesterEMailAddress", nsmgr);

                if (nodoRequesterEmail == null)
                {
                    nodoRequesterEmail = doc.CreateElement("ns7:requesterEMailAddress", nsmgr.LookupNamespace("ns7"));
                    nodoRequesterEmail.InnerText = user.Email;
                    nodoQuote.AppendChild(nodoRequesterEmail);
                }
                else
                {
                    nodoRequesterEmail.InnerText = user.Email;
                }

                #endregion

                #region parsificazione xml con i valori scelti dall'utente

                listaNodiFattori = nodoQuote.SelectNodes("ns7:product/ns9:assets/ns9:instances/ns9:factors", nsmgr);
                ListaNodiSezioni = nodoQuote.SelectNodes("ns7:product/ns9:assets/ns9:instances/ns9:sections", nsmgr);

                if (listaNodiFattori != null & listaNodiFattori.Count > 0)
                {
                    string codiceFattore = string.Empty;
                    XmlNode nodoValue;
                    XmlNode nodoManuallySet;
                    XmlNode nodoBooleanManuallySet;

                    foreach (XmlNode nodoChildFactor in listaNodiFattori)
                    {
                        // per ogni elemento attivo del form Step1 bisogna verificare l'associazione con il codice che si sta esaminando dell'xml e nel nodo "value" settare il valore corrispondente
                        codiceFattore = nodoChildFactor.SelectSingleNode("ns9:code", nsmgr).InnerText;

                        // non faccio nulla per i due fattori di Pacchetto
                        if (codiceFattore == "2PACC3" || codiceFattore == "2PACC4")
                        {
                            continue;
                        }

                        nodoValue = nodoChildFactor.SelectSingleNode("ns9:value", nsmgr);

                        nodoManuallySet = nodoChildFactor.SelectSingleNode("ns9:manuallySet", nsmgr);

                        if (nodoManuallySet != null)
                        {
                            // nodoBooleanManuallySet = nodoManuallySet.SelectSingleNode("ns2:boolean", nsmgr);
                            nodoBooleanManuallySet = nodoManuallySet.ChildNodes[0];

                            if (nodoBooleanManuallySet == null)
                            {
                                nodoBooleanManuallySet = nodoManuallySet.SelectSingleNode("ns3:boolean", nsmgr);
                            }

                            if (nodoBooleanManuallySet != null)
                            {
                                nodoBooleanManuallySet.InnerText = "true";
                            }
                        }

                        switch (codiceFattore)
                        {
                            case "2LICOP": // Fattore Concept di prodotto (Proposta)

                                if (nodoValue != null)
                                {
                                    nodoValue.InnerText = Convert.ToString(proposalCode);
                                }
                                else
                                {
                                    nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                    nodoValue.InnerText = Convert.ToString(proposalCode);
                                    nodoChildFactor.AppendChild(nodoValue);
                                }

                                break;

                            case "3ESTER": // Destinazione (estensione territoriale)

                                if (nodoValue != null)
                                {
                                    nodoValue.InnerText = user.Dest;
                                }
                                else
                                {
                                    nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                    nodoValue.InnerText = user.Dest;
                                    nodoChildFactor.AppendChild(nodoValue);
                                }

                                break;

                            case "1DEFCV": // Data inizio viaggio (data effetto copertura viaggio)

                                if (nodoValue != null)
                                {
                                    nodoValue.InnerText = GetDataRGI(user.DataInizio); // RGI vuole es: 2018/05/29, io ho 29/05/2018
                                }
                                else
                                {
                                    nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                    nodoValue.InnerText = GetDataRGI(user.DataInizio); // RGI vuole es: 2018/05/29, io ho 29/05/2018
                                    nodoChildFactor.AppendChild(nodoValue);
                                }

                                break;

                            case "1DSCCO": // Data fine viaggio (data scadenza copertura viaggio)

                                if (nodoValue != null)
                                {
                                    nodoValue.InnerText = GetDataRGI(user.DataFine); // RGI vuole es: 2018/05/29, io ho 29/05/2018
                                }
                                else
                                {
                                    nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                    nodoValue.InnerText = GetDataRGI(user.DataFine); // RGI vuole es: 2018/05/29, io ho 29/05/2018
                                    nodoChildFactor.AppendChild(nodoValue);
                                }

                                break;

                            case "1NUPER": // Numero persone

                                if (nodoValue != null)
                                {
                                    nodoValue.InnerText = user.NumPartecipanti.ToString();
                                }
                                else
                                {
                                    nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                    nodoValue.InnerText = user.NumPartecipanti.ToString();
                                    nodoChildFactor.AppendChild(nodoValue);
                                }

                                break;
                        }
                    }

                    // NB => il doc.Innerxml ha già le variazioni
                }
                #endregion
            }

            xml = doc.InnerXml;

            return xml;
        }

        public static string ParsificazioneXmlCasa(string xmlResponse, UserProfile user, string codiceCompagnia, int proposalCode, bool cancellaNodoMessaggiErrore = false)
        {
            List<string> _listaCodiciFattoreCASA_RMA = new List<string>()
            {
            "2CNPRO",   // codice proposta (full/furto/.../...)
            "2FABPA",   // fabbricato di proprietà o affitto
            "2SUPMQ",   // superficie mq
            "2TIFAB",   // tipo di fabbricato
            "2QLTAB",   // qualità delle finiture
            "2COMUN",   // comune
            "2PROVI",   // provincia
            "2CAP",     // cap
            "2TIDIM",   // tipo dimora
            "4ZTEC1",   // zona territoriale
            "_2UPIS",   // cap + istat
            };

            List<string> _listaCodiciFattoreCASA_ITA = new List<string>()
            {
            "3SUPEM",   // superficie mq
            "2CAPQQ",   // CAP
            "2CNTRP",   // contraente della polizza
            "2ABWEB",   // tipo abitazione
            "2QLTAB",    // qualità delle finiture
            };
            string dataDecorrenza = DateTime.Now.AddMonths(2).ToString("dd/MM/yyyy");
            string xml = string.Empty;
            List<string> listaCodiciFattorePerCompagnia = codiceCompagnia == "1" ? _listaCodiciFattoreCASA_RMA : _listaCodiciFattoreCASA_ITA;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlResponse);
            if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                doc.RemoveChild(doc.FirstChild);
            }

            #region Namespace xml
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            if (doc.DocumentElement.Attributes["xmlns:S"] != null)
            {
                string s = doc.DocumentElement.Attributes["xmlns:S"].Value;
                nsmgr.AddNamespace("S", s);
            }

            XmlNode nodeRoot = doc.SelectSingleNode("S:Envelope/S:Body", nsmgr);
            if (nodeRoot != null)
            {
                XmlNode nodeNamespace = nodeRoot.LastChild;

                if (nodeNamespace.Attributes["xmlns:ns3"] != null)
                {
                    string ns3 = nodeNamespace.Attributes["xmlns:ns3"].Value;
                    nsmgr.AddNamespace("ns3", ns3);

                }
                if (nodeNamespace.Attributes["xmlns:ns2"] != null)
                {
                    string ns2 = nodeNamespace.Attributes["xmlns:ns2"].Value;
                    nsmgr.AddNamespace("ns2", ns2);

                }
                if (nodeNamespace.Attributes["xmlns:ns7"] != null)
                {
                    string ns7 = nodeNamespace.Attributes["xmlns:ns7"].Value;
                    nsmgr.AddNamespace("ns7", ns7);

                }
                if (nodeNamespace.Attributes["xmlns:ns9"] != null)
                {
                    string ns9 = nodeNamespace.Attributes["xmlns:ns9"].Value;
                    nsmgr.AddNamespace("ns9", ns9);
                }
            }
            #endregion

            // nodo quote principale
            XmlNode nodoQuote = null;
            XmlNode nodoInfoRGI = null;

            string stringaQuote = "S:Envelope/S:Body/{0}:{1}/return/{0}:quote";

            XmlNodeList nodoQuoteList = doc.SelectNodes(string.Format(stringaQuote, "ns3", "getSHPQuoteResponse"), nsmgr);

            if (nodoQuoteList != null & nodoQuoteList.Count <= 0)
            {
                nodoQuoteList = doc.SelectNodes(string.Format(stringaQuote, "ns3", "pricingSHPQuoteResponse"), nsmgr);
            }

            if (nodoQuoteList != null & nodoQuoteList.Count <= 0)
            {
                nodoQuoteList = doc.SelectNodes(string.Format(stringaQuote, "ns2", "getSHPQuoteResponse"), nsmgr);
            }

            if (nodoQuoteList != null & nodoQuoteList.Count <= 0)
            {
                nodoQuoteList = doc.SelectNodes(string.Format(stringaQuote, "ns2", "pricingSHPQuoteResponse"), nsmgr);
            }

            if (nodoQuoteList != null & nodoQuoteList.Count > 0)
            {
                nodoQuote = nodoQuoteList[0];
                nodoInfoRGI = nodoQuote.NextSibling;

                if (cancellaNodoMessaggiErrore)
                {
                    // RIMUOVO IL PRECEDENTE ERRORE SE C'E' STATO
                    XmlNode nodoErrore = nodoQuote.SelectSingleNode("ns7:messages", nsmgr);

                    if (nodoErrore != null)
                    {
                        nodoErrore.RemoveAll();
                    }
                }
            }

            #region Iniettando la data decorrenza polizza

            if (string.IsNullOrEmpty(dataDecorrenza) == false)
            {
                XmlNode nodoEffectiveDate = nodoQuote.SelectSingleNode("ns7:effectiveDate", nsmgr);
                XmlNode nodoData = null;

                if (nodoEffectiveDate != null)
                {
                    nodoData = nodoEffectiveDate.ChildNodes[0];

                    string dataEffective = string.Format("{0}-{1}-{2}T00:00:00.000+0{3}:00", dataDecorrenza.Split('/')[2],
                                                                          (dataDecorrenza.Split('/')[1].Length == 1) ? "0" + dataDecorrenza.Split('/')[1] : dataDecorrenza.Split('/')[1],
                                                                          (dataDecorrenza.Split('/')[0].Length == 1) ? "0" + dataDecorrenza.Split('/')[0] : dataDecorrenza.Split('/')[0],
                                                                          TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalHours.ToString());
                    if (nodoData != null)
                    {
                        nodoData.InnerText = dataEffective;
                    }
                    else
                    {
                        nodoData = doc.CreateElement("ns2:data", nsmgr.LookupNamespace("ns2"));
                        nodoData.InnerText = dataEffective;
                        nodoEffectiveDate.AppendChild(nodoData);
                    }
                }
            }

            #endregion

            XmlNodeList listaNodiFattori = nodoQuote.SelectNodes("ns7:product/ns9:assets/ns9:instances/ns9:factors", nsmgr);

            if (listaNodiFattori != null & listaNodiFattori.Count > 0)
            {
                string codiceFattore = string.Empty;
                XmlNode nodoValue;
                XmlNode nodoManuallySet;
                XmlNode nodoBooleanManuallySet;
                    #region Iniettando la data di policyExpirationDate con data attuale + 1 anno

                    // Chat con Mirko del 24 settembre 2018

                    /*
                     * di default è impostata a 2 giorni della data effetto
                    es: data effetto 24/09/2018 data scadenza: 26/09/2018

                    solo per il casa, bisognerebbe impostarla ad un anno dalla data effetto
                    */

                XmlNode nodoPolicyExpirationDate = nodoQuote.SelectSingleNode("ns7:policyExpirationDate", nsmgr);
                XmlNode nodoDataPol = null;

                if (nodoPolicyExpirationDate != null)
                {
                        // nodoDataPol = nodoPolicyExpirationDate.SelectSingleNode("ns2:data", nsmgr);
                        nodoDataPol = nodoPolicyExpirationDate.ChildNodes[0];

                        string dataPolicy = string.Format("{0}-{1}-{2}T00:00:00+0{3}:00", DateTime.Now.AddYears(1).Year.ToString(),
                                                                              (DateTime.Now.Month.ToString().Length == 1) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString(),
                                                                              (DateTime.Now.Day.ToString().Length == 1) ? "0" + DateTime.Now.Day : DateTime.Now.Day.ToString(),
                                                                              TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalHours.ToString());

                        if (nodoDataPol != null)
                        {
                            nodoDataPol.InnerText = dataPolicy;
                        }
                        else
                        {
                            nodoDataPol = doc.CreateElement("ns2:data", nsmgr.LookupNamespace("ns2"));
                            nodoDataPol.InnerText = dataPolicy;
                            nodoPolicyExpirationDate.AppendChild(nodoDataPol);
                        }
                }

                    #endregion

                #region Iniettando nodo requesterEMailAddress per gestione tracking preventivi Casa/Viaggi (IM9950680 RGI). Mail del 02/01/2019 ore 11:07 da Mirko Rizzo

                XmlNode nodoRequesterEmail = nodoQuote.SelectSingleNode("ns7:requesterEMailAddress", nsmgr);

                    if (nodoRequesterEmail == null)
                    {
                        nodoRequesterEmail = doc.CreateElement("ns7:requesterEMailAddress", nsmgr.LookupNamespace("ns7"));
                        nodoRequesterEmail.InnerText = user.Email;
                        nodoQuote.AppendChild(nodoRequesterEmail);
                    }
                    else
                    {
                        nodoRequesterEmail.InnerText = user.Email;
                    }

                    #endregion

                    #region " Iniettando i valori del form "

                    foreach (XmlNode nodoChildFactor in listaNodiFattori)
                    {
                        // per ogni elemento attivo del form Step1 bisogna verificare l'associazione con il codice che si sta esaminando dell'xml e nel nodo "value" settare il valore corrispondente
                        codiceFattore = nodoChildFactor.SelectSingleNode("ns9:code", nsmgr).InnerText;

                        if (listaCodiciFattorePerCompagnia.Contains(codiceFattore))
                        {
                            nodoValue = nodoChildFactor.SelectSingleNode("ns9:value", nsmgr);

                            if (codiceFattore == "2CNPRO")
                            {
                                continue;
                            }

                            nodoManuallySet = nodoChildFactor.SelectSingleNode("ns9:manuallySet", nsmgr);

                            if (nodoManuallySet != null)
                            {
                                // nodoBooleanManuallySet = nodoManuallySet.SelectSingleNode("ns2:boolean", nsmgr);
                                nodoBooleanManuallySet = nodoManuallySet.ChildNodes[0];

                                if (nodoBooleanManuallySet == null)
                                {
                                    nodoBooleanManuallySet = nodoManuallySet.SelectSingleNode("ns3:boolean", nsmgr);
                                }

                                if (nodoBooleanManuallySet != null)
                                {
                                    nodoBooleanManuallySet.InnerText = "true";
                                }
                            }

                            switch (codiceFattore)
                            {
                                case "2FABPA": // Fabbricato di proprietà o affitto

                                    if (nodoValue != null)
                                    {
                                        nodoValue.InnerText = user.PA;
                                    }
                                    else
                                    {
                                        nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                        nodoValue.InnerText = user.PA;
                                        nodoChildFactor.AppendChild(nodoValue);
                                    }

                                    break;

                                case "2SUPMQ": // Superficie fabbricato (mq commerciali)
                                case "3SUPEM":

                                    if (nodoValue != null)
                                    {
                                        nodoValue.InnerText = user.MQ.Replace("mq", string.Empty);
                                    }
                                    else
                                    {
                                        nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                        nodoValue.InnerText = user.MQ.Replace("mq", string.Empty);
                                        nodoChildFactor.AppendChild(nodoValue);
                                    }

                                    break;

                                case "2TIFAB": // Tipo di fabbricato

                                    if (nodoValue != null)
                                    {
                                        nodoValue.InnerText = user.Fabbricato;
                                    }
                                    else
                                    {
                                        nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                        nodoValue.InnerText = user.Fabbricato;
                                        nodoChildFactor.AppendChild(nodoValue);
                                    }

                                    break;

                                case "2QLTAB": // Qualità delle finiture

                                    if (nodoValue != null)
                                    {
                                        nodoValue.InnerText = user.Finiture;
                                    }
                                    else
                                    {
                                        nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                        nodoValue.InnerText = user.Finiture;
                                        nodoChildFactor.AppendChild(nodoValue);
                                    }

                                    break;

                                case "2COMUN": // Comune

                                    if (nodoValue != null)
                                    {
                                        nodoValue.InnerText = user.Comune;
                                    }
                                    else
                                    {
                                        nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                        nodoValue.InnerText = user.Comune;
                                        nodoChildFactor.AppendChild(nodoValue);
                                    }

                                    break;

                                case "2PROVI": // Provincia (stesso valore di zona territoriale)

                                    if (nodoValue != null)
                                    {
                                        nodoValue.InnerText = user.ProvRes;
                                    }
                                    else
                                    {
                                        nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                        nodoValue.InnerText = user.ProvRes;
                                        nodoChildFactor.AppendChild(nodoValue);
                                    }

                                    break;

                                case "2CAP": // CAP
                                case "2CAPQQ":
                                    if (nodoValue != null)
                                    {
                                        nodoValue.InnerText = user.Cap;
                                    }
                                    else
                                    {
                                        nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                        nodoValue.InnerText = user.Cap;
                                        nodoChildFactor.AppendChild(nodoValue);
                                    }

                                    break;

                                case "2TIDIM": // Tipo Dimora

                                    if (nodoValue != null)
                                    {
                                        nodoValue.InnerText = user.AS;
                                    }
                                    else
                                    {
                                        nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                        nodoValue.InnerText = user.AS;
                                        nodoChildFactor.AppendChild(nodoValue);
                                    }

                                    break;

                                case "4ZTEC1": // Zona territoriale

                                    if (nodoValue != null)
                                    {
                                        nodoValue.InnerText = user.ProvRes;
                                    }
                                    else
                                    {
                                        nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                        nodoValue.InnerText = user.ProvRes;
                                        nodoChildFactor.AppendChild(nodoValue);
                                    }

                                    break;

                                case "_2UPIS": // CAP + Codice Istat comune

                                    if (nodoValue != null)
                                    {
                                        nodoValue.InnerText = user.Cap + user.Istat; // Mirko: il cap generico della città concatenato l'ISTAT della città (per esempio Torino: 10100001272)
                                    }
                                    else
                                    {
                                        nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                        nodoValue.InnerText = user.Istat;
                                        nodoChildFactor.AppendChild(nodoValue);
                                    }

                                    break;

                                case "2CNTRP": // Contraente della polizza

                                    if (nodoValue != null)
                                    {
                                        nodoValue.InnerText = user.PA;
                                    }
                                    else
                                    {
                                        nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                        nodoValue.InnerText =user.PA;
                                        nodoChildFactor.AppendChild(nodoValue);
                                    }

                                    break;

                                case "2ABWEB": // Tipo abitazione

                                    if (nodoValue != null)
                                    {
                                        nodoValue.InnerText = user.AS;
                                    }
                                    else
                                    {
                                        nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                        nodoValue.InnerText = user.AS;
                                        nodoChildFactor.AppendChild(nodoValue);
                                    }

                                    break;
                            }
                        }
                    }

                    #endregion
                
                    #region " Inietto valore codice proposta e nient'altro "

                    foreach (XmlNode nodoChildFactor in listaNodiFattori)
                    {
                        codiceFattore = nodoChildFactor.SelectSingleNode("ns9:code", nsmgr).InnerText;

                        if (codiceFattore != "2CNPRO")
                        {
                            continue;
                        }
                        else
                        {
                            nodoManuallySet = nodoChildFactor.SelectSingleNode("ns9:manuallySet", nsmgr);

                            if (nodoManuallySet != null)
                            {
                                //nodoBooleanManuallySet = nodoManuallySet.SelectSingleNode("ns2:boolean", nsmgr);
                                nodoBooleanManuallySet = nodoManuallySet.ChildNodes[0];

                                if (nodoBooleanManuallySet == null)
                                {
                                    nodoBooleanManuallySet = nodoManuallySet.SelectSingleNode("ns3:boolean", nsmgr);
                                }

                                if (nodoBooleanManuallySet != null)
                                {
                                    nodoBooleanManuallySet.InnerText = "true";
                                }
                            }

                            nodoValue = nodoChildFactor.SelectSingleNode("ns9:value", nsmgr);

                            if (nodoValue != null)
                            {
                                nodoValue.InnerText = Convert.ToString(proposalCode);
                            }
                            else
                            {
                                nodoValue = doc.CreateElement("ns9:value", nsmgr.LookupNamespace("ns9"));
                                nodoValue.InnerText = Convert.ToString(proposalCode);
                                nodoChildFactor.AppendChild(nodoValue);
                            }

                            break;
                        }
                    }

                    #endregion

                    #region Inietto informazione per RGI cambio formula (chat con Mirko)

                    /*
                     *   <ns2:serviceInfo>
                           ...  <- qui c'è il resto del serviceInfo
                            <ns3:properties>
                                    <ns3:chiave>CAMBIO_FORMULA</ns3:chiave>
                                    <ns3:valore>1</ns3:valore>
                            </ns3:properties>
                        </ns2:serviceInfo>
                     */

                    if (nodoInfoRGI != null)
                    {
                    bool esiste = false;
                        XmlNode nodoProperties = nodoInfoRGI.SelectSingleNode("ns2:properties", nsmgr);

                        foreach (XmlNode nodoProp in nodoInfoRGI.ChildNodes)
                        {
                            if (nodoProp.LocalName == "properties")
                            {
                                XmlNode nodoChiave = nodoProp.FirstChild;
                                XmlNode nodoValore = nodoProp.LastChild;

                                if (nodoChiave != null)
                                {
                                    if (nodoChiave.InnerText == "CAMBIO_FORMULA")
                                    {
                                        esiste = true;

                                        if (nodoValore != null)
                                        {
                                            nodoValore.InnerText = "1";
                                        }

                                        break;
                                    }
                                }
                            }
                        }

                        if (esiste == false)
                        {
                            XmlElement nodoPropertiesNuovo = doc.CreateElement("ns2:properties", nsmgr.LookupNamespace("ns2"));
                            XmlElement nodoChiave = doc.CreateElement("ns2:chiave", nsmgr.LookupNamespace("ns2"));
                            XmlElement nodoValore = doc.CreateElement("ns2:valore", nsmgr.LookupNamespace("ns2"));
                            nodoChiave.InnerText = "CAMBIO_FORMULA";
                            nodoValore.InnerText = "1";

                            nodoPropertiesNuovo.AppendChild(nodoChiave);
                            nodoPropertiesNuovo.AppendChild(nodoValore);
                            nodoInfoRGI.AppendChild(nodoPropertiesNuovo);
                        }
                    }

                    #endregion

                    #region Iniettando la data di policyExpirationDate con data decorrenza polizza (scelta dall'utente) + 1 anno

                    nodoPolicyExpirationDate = nodoQuote.SelectSingleNode("ns7:policyExpirationDate", nsmgr);
                    nodoDataPol = null;

                    if (nodoPolicyExpirationDate != null)
                    {
                        nodoDataPol = nodoPolicyExpirationDate.ChildNodes[0];

                        String dataPolicy = String.Format("{0}-{1}-{2}T00:00:00+0{3}:00", DateTime.Parse(dataDecorrenza).AddYears(1).Year.ToString(),
                                                                              (DateTime.Parse(dataDecorrenza).Month.ToString().Length == 1) ? "0" + DateTime.Parse(dataDecorrenza).Month.ToString() : DateTime.Parse(dataDecorrenza).Month.ToString(),
                                                                              (DateTime.Parse(dataDecorrenza).Day.ToString().Length == 1) ? "0" + DateTime.Parse(dataDecorrenza).Day : DateTime.Parse(dataDecorrenza).Day.ToString(),
                                                                              TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Parse(dataDecorrenza)).TotalHours.ToString());

                        if (nodoDataPol != null)
                        {
                            nodoDataPol.InnerText = dataPolicy;
                        }
                        else
                        {
                            nodoDataPol = doc.CreateElement("ns2:data", nsmgr.LookupNamespace("ns2"));
                            nodoDataPol.InnerText = dataPolicy;
                            nodoPolicyExpirationDate.AppendChild(nodoDataPol);
                        }
                    }
                    #endregion
            }

            xml = doc.InnerXml;

            return xml;
        }

        private static string GetDataRGI(string dataInput)
        {
            string nuova = "{0}/{1}/{2}";

            return string.Format(nuova, dataInput.Split('/')[2], dataInput.Split('/')[1], dataInput.Split('/')[0]);
        }
    }
}