// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples
{
    using System;
    using System.Collections.Generic;

    /// <summary>Contains information about a user.</summary>
    public class UserProfile
    {
        public string Email { get; set; }

        public int Nprev { get; set; }

        public bool Flagmail { get; set; }

        public bool Flagerror { get; set; }

        public int Flagprivacy { get; set; }

        public int Flagstep { get; set; }

        public string Targa { get; set; }

        public string DataNascita { get; set; }

        public int NumPartecipanti { get; set; }

        public string DataInizio { get; set; }

        public string DataFine { get; set; }

        public string Dest { get; set; }

        public string ProvRes { get; set; }

        public string Patologie { get; set; }

        public string Strutture { get; set; }

        public string Age { get; set; }

        public string Cap { get; set; }

        public string Ren { get; set; }

        public string MQ { get; set; }

        public string Comune { get; set; }

        public string Istat { get; set; }

        public string Fabbricato { get; set; }

        public string Finiture { get; set; }

        public string PA { get; set; }

        public string AS { get; set; }

        public int Propietario { get; set; }

        public int Figli { get; set; }

        public string ProvinciaNome { get; set; }

        // Verifica che la data d'inizio sia prima della data di fine viaggio
        public bool DateToDate()
        {
            DateTime start = DateTime.Parse(this.DataInizio);
            DateTime end = DateTime.Parse(this.DataFine);
            if (end.Date > start.Date)
            {
                return true;
            }

            return false;
        }
    }
}
