# TODO: Implementazione Attività IVA e Contabili

## ✅ Attività già implementate:
- **Redditi**: 730, 740, 750, 760, 770, CU, ENC, IRAP
- **IVA**: DRIVA ✅

## 🔄 Attività IVA da implementare:
- **LIPE** 
  - Tabella: `attivita_lipe`
  - Modello: `AttivitaLipe`
  - Controller: `AttivitaLipeController`
  - DbSet: `AttivitaLipe`

- **MOD TR IVA**
  - Tabella: `attivita_mod_tr_iva`
  - Modello: `AttivitaModTrIva`
  - Controller: `AttivitaModTrIvaController`
  - DbSet: `AttivitaModTrIva`

## 📊 Attività Contabili da implementare:
1. **INAIL** → `AttivitaInail`
2. **Cassetto Fiscale** → `AttivitaCassettoFiscale`
3. **Fatturazione Elettronica TS** → `AttivitaFatturazioneElettronicaTs`
4. **Conservazione** → `AttivitaConservazione`
5. **IMU** → `AttivitaImu`
6. **Registro IVA** → `AttivitaRegIva`
7. **Registro Cespiti** → `AttivitaRegCespiti`
8. **Inventari** → `AttivitaInventari`
9. **Libro Giornale** → `AttivitaLibroGiornale`
10. **Lettere d'Intento** → `AttivitaLettereIntento`
11. **Mod. INTRASTAT** → `AttivitaModIntrastat`
12. **Firma Digitale** → `AttivitaFirmaDigitale`
13. **Titolare Effettivo** → `AttivitaTitolareEffettivo`

## 🎯 Logica di conferma doppia:
La logica per la **doppia conferma di eliminazione** è già predisposta in `ClientiController.cs`:

### Passaggi per aggiungere nuove attività:
1. **Creare modello** (es. `AttivitaLipe.cs`)
2. **Aggiungere DbSet** in `ApplicationDbContext.cs`
3. **Scommentare i controlli** in `ClientiController.cs` righe 423-433
4. **Scommentare i case** in `RimuoviDalleListeAttivita` righe 2013-2043
5. **Creare controller** seguendo lo schema di `AttivitaDrivaController.cs`
6. **Creare view** seguendo lo schema di `Views/AttivitaDriva/Index.cshtml`
7. **Aggiungere al menu** in `Views/GestioneAttivita/Index.cshtml`

## 📝 Note:
- La **prima conferma JavaScript** è già implementata per tutte le attività
- La **seconda conferma server-side** sarà automatica quando si scommenteranno i TODO
- Utilizzare DRIVA come modello di riferimento per implementare le altre attività

## 🚀 Prossimi passi:
Quando sarà necessario implementare LIPE o altre attività, seguire il modello DRIVA e scommentare le righe TODO nel controller.
