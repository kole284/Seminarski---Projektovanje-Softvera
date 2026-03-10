# Filteri za pretragu računa

## Šta je dodato

Dodati su novi filteri za pretragu računa koji omogućavaju filtriranje po prodavcu i kupcu.

## Izmene

### 1. Client/Main.Designer.cs
- Dodati novi ComboBox kontrole: `cmbProdavac` i `cmbKupac`
- Dodati Label kontrole: `lblProdavac` i `lblKupac`
- Premešteno dugme "Obriši filtere" na dole levo u grupi filtera
- Proširena forma za prikaz novih kontrola

### 2. Client/Main.cs

#### Nove metode:
- **`UcitajProdavce()`** - Učitava sve prodavce iz baze i popunjava `cmbProdavac`
- **`UcitajKupce()`** - Učitava sve kupce (i firme i fizička lica) iz baze i popunjava `cmbKupac`

#### Izmenjene metode:
- **`UpdateFilterLabels()`** - Proširena za prikaz/sakrivanje filtera prodavca i kupca kada je izabran entitet "Računi"
- **`button1_Click()`** - Dodato filtriranje po prodavcu i kupcu pri pretragama računa
- **`btnClearFilter_Click()`** - Dodato resetovanje ComboBox-eva za prodavca i kupca

### 3. Broker/DatabaseBroker.cs
**Napomena:** Ova funkcionalnost je već postojala! Broker je već podržavao filtriranje po `idProdavac` i `idKupac` u VratiSve metodi.

## Kako koristiti

1. Otvori glavni prozor aplikacije (Main form)
2. Izaberi "Računi" iz padajuće liste
3. Prikazaće se 4 filtera:
   - **ID Računa** - Tekstualno polje za pretragu po konkretnom ID-u
   - **Filter 2** - Trenutno neiskorišćen
   - **Prodavac** - Dropdown lista svih prodavaca
   - **Kupac** - Dropdown lista svih kupaca (i firmi i fizičkih lica)

4. Izaberi željenog prodavca i/ili kupca iz padajućih lista
5. Klikni "Prikaži" da prikaže filtrirane račune
6. Klikni "Obriši filtere" da resetuje sve filtere

## Primeri pretrage

- **Svi računi**: Ne unosi ništa, klikni "Prikaži"
- **Računi određenog prodavca**: Izaberi prodavca, ostavi ostalo prazno
- **Računi određenog kupca**: Izaberi kupca, ostavi ostalo prazno
- **Računi određenog prodavca i kupca**: Izaberi i prodavca i kupca
- **Konkretan račun**: Unesi ID računa u prvo polje

## Tehnički detalji

### Format prikaza
- **Prodavac**: Prikazuje se kao "Ime Prezime" (npr. "Nikola Kostić")
- **Kupac**: Prikazuje se kao "Naziv/Ime (Tip)" 
  - Za firme: "Microsoft (Firma)"
  - Za fizička lica: "Marko Marković (Fizičko lice)"

### Backend filtriranje
Filteri se šalju na server kroz `Racun` objekat:
```csharp
Racun racun = new Racun();
racun.Prodavac = new Prodavac { IdProdavac = izabraniId };
racun.Kupac = new Kupac { IdKupac = izabraniId };
```

DatabaseBroker koristi ove vrednosti za kreiranje WHERE uslova u SQL upitu.
