-- Aggiunta nuovi campi alla tabella clienti
-- Per dati cliente e sezione mandati
-- Database: IT15\SQLEXPRESS - Consulting

USE [Consulting]

-- Aggiungo i nuovi campi dati cliente
ALTER TABLE clienti ADD 
    cf_cliente NVARCHAR(16) NULL,
    piva_cliente NVARCHAR(20) NULL,
    indirizzo NVARCHAR(200) NULL,
    citta NVARCHAR(100) NULL,
    provincia NVARCHAR(5) NULL,
    cap NVARCHAR(10) NULL,
    legale_rappresentante NVARCHAR(200) NULL,
    cf_legale_rappresentante NVARCHAR(16) NULL;

-- Aggiungo i campi per la sezione mandati
ALTER TABLE clienti ADD
    data_mandato DATE NULL,
    importo_mandato_annuo DECIMAL(10,2) NULL,
    proforma_tipo NVARCHAR(20) DEFAULT 'trimestrale' CHECK (proforma_tipo IN ('trimestrale', 'mensile'));

SELECT 'Campi aggiunti alla tabella clienti con successo!' as Risultato;
