# Predlog za izdvajanje zajedničkih modela u Common sloj

1. Napravi novi projekat/folder `Common` (već kreiran).
2. U `Common` premesti sledeće:
   - Sve domenske klase (entitete): Firma, FizickoLice, Kupac, Oprema, Prodavac, Skladiste, Racun, StavkaRacuna, itd.
   - Interfejs `IDomainObject`.
   - DTO klase: Request, Response.
   - Enum-e: Operation.
3. U svim ostalim projektima (Broker, Server, Client, SistemskeOperacije) referenciraj `Common` umesto `Models` za ove tipove.
4. U `Models` mogu ostati view modeli i eventualno pomoćne klase koje nisu deo zajedničkog domena.

Ovim dobijaš jasnu separaciju domena i komunikacionih objekata, kao u primeru seminarskog.
