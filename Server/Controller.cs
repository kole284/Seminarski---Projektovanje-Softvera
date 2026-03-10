using System;
using System.Linq;
using System.Text.Json;
using Models;
using Broker;
using SistemskeOperacije;
namespace Server
{
    public class Controller
    {
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        // PrepoznajTip metoda mora biti public ili private u ovoj klasi
        public IDomainObject? PrepoznajTip(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            string lower = json.ToLowerInvariant();

            // Racun (check before Prodavac because Racun contains nested Prodavac)
            if (lower.Contains("idracun") || lower.Contains("stavke") || lower.Contains("datumizdavanja") || lower.Contains("konacan") || lower.Contains("konacaniznos") || lower.Contains("pdv") || lower.Contains("racun"))
            {
                try { return JsonSerializer.Deserialize<Racun>(json, jsonOptions)!; } catch { }
            }

            // ProdSklad (has both idProdavac and idSkladiste) - must detect before Prodavac
            if (lower.Contains("idprodavac") && lower.Contains("idskladiste"))
            {
                try { return JsonSerializer.Deserialize<ProdSklad>(json, jsonOptions)!; } catch { }
            }

            // FizickoLice - MUST be checked BEFORE Prodavac because both have similar properties
            // FizickoLice has unique properties: loyaltyClan, popust (without idProdavac or password)
            // Check for loyaltyClan OR (popust + imeprezime but NOT password/idprodavac)
            if (lower.Contains("loyaltyclan") || 
                (lower.Contains("popust") && lower.Contains("imeprezime") && !lower.Contains("idprodavac") && !lower.Contains("password")))
            {
                try { return JsonSerializer.Deserialize<FizickoLice>(json, jsonOptions)!; } catch { }
            }

            // Prodavac (has password, idProdavac, skladista - properties FizickoLice doesn't have)
            if (lower.Contains("idprodavac") || lower.Contains("password") || lower.Contains("skladista"))
            {
                try { return JsonSerializer.Deserialize<Prodavac>(json, jsonOptions)!; } catch { }
            }

            // Firma (PIB is unique to Firma)
            if (lower.Contains("pib"))
            {
                try { return JsonSerializer.Deserialize<Firma>(json, jsonOptions)!; } catch { }
            }

            // Oprema (kategorija + cena is unique to equipment)
            if ((lower.Contains("kategorija") && lower.Contains("cena")) || lower.Contains("idoprema"))
            {
                try { return JsonSerializer.Deserialize<Oprema>(json, jsonOptions)!; } catch { }
            }

            // Skladiste
            if ((lower.Contains("adresa") && lower.Contains("naziv")) || lower.Contains("idskladiste"))
            {
                try { return JsonSerializer.Deserialize<Skladiste>(json, jsonOptions)!; } catch { }
            }

            // Kupac view / generic kupac
            if (lower.Contains("popust") || lower.Contains("tip"))
            {
                try { return JsonSerializer.Deserialize<Kupac>(json, jsonOptions)!; } catch { }
            }

            // Last-resort fallback: try deserializing into common types
            try { return JsonSerializer.Deserialize<Prodavac>(json, jsonOptions)!; } catch { }
            try { return JsonSerializer.Deserialize<Firma>(json, jsonOptions)!; } catch { }
            try { return JsonSerializer.Deserialize<FizickoLice>(json, jsonOptions)!; } catch { }
            try { return JsonSerializer.Deserialize<Kupac>(json, jsonOptions)!; } catch { }
            try { return JsonSerializer.Deserialize<Oprema>(json, jsonOptions)!; } catch { }
            try { return JsonSerializer.Deserialize<Skladiste>(json, jsonOptions)!; } catch { }
            try { return JsonSerializer.Deserialize<Racun>(json, jsonOptions)!; } catch { }

            return null;
        }
        public Response HandleRequest(Request req)
        {
            Response res = new Response();
            try
            {
                object data = req.Data;
                if (data is JsonElement jsonElement)
                {
                    data = jsonElement.GetRawText();
                }
                switch (req.Operation)
                {
                    case Operation.PrijaviProdavac:
                        var prodavac = JsonSerializer.Deserialize<Prodavac>(data?.ToString() ?? string.Empty, jsonOptions);
                        var soPrijavi = new SOPrijaviProdavac(prodavac!);
                        soPrijavi.ExecuteTemplate();
                        if (soPrijavi.Result != null)
                        {
                            res.IsSuccessful = true;
                            res.Data = soPrijavi.Result;
                        }
                        else
                        {
                            res.IsSuccessful = false;
                            res.Message = "Neispravan email ili lozinka.";
                        }
                        break;
                    case Operation.VratiSve:
                        IDomainObject? helper = PrepoznajTip(data?.ToString() ?? string.Empty);
                        Console.WriteLine($"[DEBUG] VratiSve raw JSON: {data?.ToString()}");
                        Console.WriteLine($"[DEBUG] Prepoznat tip: {helper?.GetType().Name ?? "null"}");
                        if (helper != null)
                        {
                            Console.WriteLine($"[DEBUG] Helper.TableName={helper.TableName}, WhereCondition='{helper.WhereCondition}'");
                            var soVratiSve = new SOVratiSve(helper);
                            soVratiSve.ExecuteTemplate();
                            var lista = soVratiSve.Result;
                            if (lista != null && lista.Count > 0)
                            {
                                res.IsSuccessful = true;

                                // System.Text.Json does not automatically preserve concrete types
                                // when serializing collections typed as List<IDomainObject>.
                                // Convert to a strongly-typed list for known domain types so
                                // the serializer includes all concrete properties.
                                try
                                {
                                    if (helper is Prodavac)
                                        res.Data = lista.Cast<Prodavac>().ToList();
                                    else if (helper is Firma)
                                        res.Data = lista.Cast<Firma>().ToList();
                                    else if (helper is FizickoLice)
                                        res.Data = lista.Cast<FizickoLice>().ToList();
                                    else if (helper is Oprema)
                                        res.Data = lista.Cast<Oprema>().ToList();
                                    else if (helper is Skladiste)
                                        res.Data = lista.Cast<Skladiste>().ToList();
                                    else if (helper is ProdSklad)
                                        res.Data = lista.Cast<ProdSklad>().ToList();
                                    else if (helper is Racun)
                                        res.Data = lista.Cast<Racun>().ToList();
                                    else
                                    {
                                        // Fallback: serialize to JsonElement so full runtime shape is preserved
                                        string tmp = JsonSerializer.Serialize(lista, jsonOptions);
                                        res.Data = JsonSerializer.Deserialize<JsonElement>(tmp, jsonOptions);
                                    }
                                }
                                catch
                                {
                                    // In case casting fails, fallback to JsonElement serialization
                                    string tmp = JsonSerializer.Serialize(lista, jsonOptions);
                                    res.Data = JsonSerializer.Deserialize<JsonElement>(tmp, jsonOptions);
                                }
                            }
                            else
                            {
                                res.IsSuccessful = false;
                                res.Message = "Nema podataka ili neuspešno.";
                            }
                        }
                        else
                        {
                            res.IsSuccessful = false;
                            res.Message = "Nepoznat tip za VratiSve.";
                        }
                        break;
                    case Operation.DodajObjekat:
                        IDomainObject? noviObjekat = PrepoznajTip(data?.ToString() ?? string.Empty);
                        if (noviObjekat != null)
                        {
                            if (noviObjekat is Racun racun && racun.Prodavac != null && racun.Prodavac.Email == "admin@prodavnica.rs")
                            {
                                res.IsSuccessful = false;
                                res.Message = "ZABRANJENA OPERACIJA: Administratori ne smeju da dodaju račune!";
                                break;
                            }

                            // Provera duplikata za FizickoLice
                            if (noviObjekat is FizickoLice fizickoLice)
                            {
                                // Provera email-a
                                var fizickoPoEmailu = new FizickoLice { Email = fizickoLice.Email };
                                var soEmailCheck = new SOVratiSve(fizickoPoEmailu);
                                soEmailCheck.ExecuteTemplate();

                                if (soEmailCheck.Result != null && soEmailCheck.Result.Count > 0)
                                {
                                    res.IsSuccessful = false;
                                    res.Message = $"Fizičko lice sa email-om '{fizickoLice.Email}' već postoji u sistemu!";
                                    break;
                                }

                                // Provera telefona
                                var fizickoPoTelefonu = new FizickoLice { Telefon = fizickoLice.Telefon };
                                var soTelefonCheck = new SOVratiSve(fizickoPoTelefonu);
                                soTelefonCheck.ExecuteTemplate();

                                if (soTelefonCheck.Result != null && soTelefonCheck.Result.Count > 0)
                                {
                                    res.IsSuccessful = false;
                                    res.Message = $"Fizičko lice sa telefonom '{fizickoLice.Telefon}' već postoji u sistemu!";
                                    break;
                                }
                            }

                            // Provera duplikata za Prodavac
                            if (noviObjekat is Prodavac noviprodavac)
                            {
                                // Provera email-a
                                var prodavacPoEmailu = new Prodavac { Email = noviprodavac.Email };
                                var soEmailCheck = new SOVratiSve(prodavacPoEmailu);
                                soEmailCheck.ExecuteTemplate();

                                if (soEmailCheck.Result != null && soEmailCheck.Result.Count > 0)
                                {
                                    res.IsSuccessful = false;
                                    res.Message = $"Prodavac sa email-om '{noviprodavac.Email}' već postoji u sistemu!";
                                    break;
                                }

                                // Provera telefona
                                var prodavacPoTelefonu = new Prodavac { Telefon = noviprodavac.Telefon };
                                var soTelefonCheck = new SOVratiSve(prodavacPoTelefonu);
                                soTelefonCheck.ExecuteTemplate();

                                if (soTelefonCheck.Result != null && soTelefonCheck.Result.Count > 0)
                                {
                                    res.IsSuccessful = false;
                                    res.Message = $"Prodavac sa telefonom '{noviprodavac.Telefon}' već postoji u sistemu!";
                                    break;
                                }
                            }

                            var soDodaj = new SODodajObjekat(noviObjekat);
                            soDodaj.ExecuteTemplate();
                            res.IsSuccessful = soDodaj.Success;
                            if (soDodaj.Success)
                            {
                                // Return the created object (with identity fields populated) so client can update UI immediately
                                res.Data = soDodaj.Result;
                                res.Message = "Objekat je uspešno dodat u bazu.";
                            }
                            else
                            {
                                res.Message = "Greška pri dodavanju objekta u bazu.";
                            }
                        }
                        else
                        {
                            res.IsSuccessful = false;
                            res.Message = "Server nije prepoznao tip objekta.";
                        }
                        break;
                    case Operation.AzurirajObjekat:
                        IDomainObject? objektZaAzuriranje = PrepoznajTip(data?.ToString() ?? string.Empty);
                        if (objektZaAzuriranje != null)
                        {
                            if (objektZaAzuriranje is Racun racunZaAzuriranje && racunZaAzuriranje.Prodavac != null && racunZaAzuriranje.Prodavac.Email == "admin@prodavnica.rs")
                            {
                                res.IsSuccessful = false;
                                res.Message = "ZABRANJENA OPERACIJA: Administratori ne smeju da ažuriraju račune!";
                                break;
                            }

                            // Provera duplikata za FizickoLice (mora ignorisati sebe)
                            if (objektZaAzuriranje is FizickoLice fizickoZaAzuriranje)
                            {
                                // Provera email-a
                                var fizickoPoEmailu = new FizickoLice { Email = fizickoZaAzuriranje.Email };
                                var soEmailCheck = new SOVratiSve(fizickoPoEmailu);
                                soEmailCheck.ExecuteTemplate();

                                if (soEmailCheck.Result != null && soEmailCheck.Result.Count > 0)
                                {
                                    // Proveri da li je to neko drugi (ne isti objekat)
                                    var postojeci = soEmailCheck.Result.Cast<FizickoLice>().FirstOrDefault(f => f.IdKupac != fizickoZaAzuriranje.IdKupac);
                                    if (postojeci != null)
                                    {
                                        res.IsSuccessful = false;
                                        res.Message = $"Fizičko lice sa email-om '{fizickoZaAzuriranje.Email}' već postoji u sistemu!";
                                        break;
                                    }
                                }

                                // Provera telefona
                                var fizickoPoTelefonu = new FizickoLice { Telefon = fizickoZaAzuriranje.Telefon };
                                var soTelefonCheck = new SOVratiSve(fizickoPoTelefonu);
                                soTelefonCheck.ExecuteTemplate();

                                if (soTelefonCheck.Result != null && soTelefonCheck.Result.Count > 0)
                                {
                                    // Proveri da li je to neko drugi (ne isti objekat)
                                    var postojeci = soTelefonCheck.Result.Cast<FizickoLice>().FirstOrDefault(f => f.IdKupac != fizickoZaAzuriranje.IdKupac);
                                    if (postojeci != null)
                                    {
                                        res.IsSuccessful = false;
                                        res.Message = $"Fizičko lice sa telefonom '{fizickoZaAzuriranje.Telefon}' već postoji u sistemu!";
                                        break;
                                    }
                                }
                            }

                            // Provera duplikata za Prodavac (mora ignorisati sebe)
                            if (objektZaAzuriranje is Prodavac prodavacZaAzuriranje)
                            {
                                // Provera email-a
                                var prodavacPoEmailu = new Prodavac { Email = prodavacZaAzuriranje.Email };
                                var soEmailCheck = new SOVratiSve(prodavacPoEmailu);
                                soEmailCheck.ExecuteTemplate();

                                if (soEmailCheck.Result != null && soEmailCheck.Result.Count > 0)
                                {
                                    // Proveri da li je to neko drugi (ne isti objekat)
                                    var postojeci = soEmailCheck.Result.Cast<Prodavac>().FirstOrDefault(p => p.IdProdavac != prodavacZaAzuriranje.IdProdavac);
                                    if (postojeci != null)
                                    {
                                        res.IsSuccessful = false;
                                        res.Message = $"Prodavac sa email-om '{prodavacZaAzuriranje.Email}' već postoji u sistemu!";
                                        break;
                                    }
                                }

                                // Provera telefona
                                var prodavacPoTelefonu = new Prodavac { Telefon = prodavacZaAzuriranje.Telefon };
                                var soTelefonCheck = new SOVratiSve(prodavacPoTelefonu);
                                soTelefonCheck.ExecuteTemplate();

                                if (soTelefonCheck.Result != null && soTelefonCheck.Result.Count > 0)
                                {
                                    // Proveri da li je to neko drugi (ne isti objekat)
                                    var postojeci = soTelefonCheck.Result.Cast<Prodavac>().FirstOrDefault(p => p.IdProdavac != prodavacZaAzuriranje.IdProdavac);
                                    if (postojeci != null)
                                    {
                                        res.IsSuccessful = false;
                                        res.Message = $"Prodavac sa telefonom '{prodavacZaAzuriranje.Telefon}' već postoji u sistemu!";
                                        break;
                                    }
                                }
                            }

                            var soAzuriraj = new SOAzurirajObjekat(objektZaAzuriranje);
                            soAzuriraj.ExecuteTemplate();
                            res.IsSuccessful = soAzuriraj.Success;
                            res.Message = soAzuriraj.Success ? "Objekat je uspešno ažuriran." : "Greška pri ažuriranju objekta.";
                        }
                        else
                        {
                            res.IsSuccessful = false;
                            res.Message = "Server nije prepoznao tip objekta.";
                        }
                        break;
                    case Operation.ObrisiObjekat:
                        IDomainObject? objektZaBrisanje = PrepoznajTip(data?.ToString() ?? string.Empty);
                        Console.WriteLine($"[DEBUG ObrisiObjekat] Prepoznat tip: {objektZaBrisanje?.GetType().Name ?? "null"}");
                        if (objektZaBrisanje != null)
                        {
                            // Provera da li entitet postoji u računima pre brisanja
                            string? errorMessage = null;

                            if (objektZaBrisanje is Prodavac prodavacZaBrisanje)
                            {
                                // Proveri da li postoje računi sa ovim prodavcem
                                var racunZaProveru = new Racun { Prodavac = new Prodavac { IdProdavac = prodavacZaBrisanje.IdProdavac } };
                                var soRacuniCheck = new SOVratiSve(racunZaProveru);
                                soRacuniCheck.ExecuteTemplate();

                                if (soRacuniCheck.Result != null && soRacuniCheck.Result.Count > 0)
                                {
                                    errorMessage = $"Ne možete obrisati prodavca '{prodavacZaBrisanje.ImePrezime}' jer postoji {soRacuniCheck.Result.Count} račun(a) koji ga koriste!";
                                }
                            }
                            else if (objektZaBrisanje is FizickoLice fizickoZaBrisanje)
                            {
                                Console.WriteLine($"[DEBUG ObrisiObjekat] FizickoLice detektovano: IdKupac={fizickoZaBrisanje.IdKupac}, ImePrezime={fizickoZaBrisanje.ImePrezime}");
                                // Proveri da li postoje računi sa ovim kupcem
                                var racunZaProveru = new Racun { Kupac = new Kupac { IdKupac = fizickoZaBrisanje.IdKupac } };
                                var soRacuniCheck = new SOVratiSve(racunZaProveru);
                                soRacuniCheck.ExecuteTemplate();

                                if (soRacuniCheck.Result != null && soRacuniCheck.Result.Count > 0)
                                {
                                    errorMessage = $"Ne možete obrisati fizičko lice '{fizickoZaBrisanje.ImePrezime}' jer postoji {soRacuniCheck.Result.Count} račun(a) koji ga koriste!";
                                }
                                Console.WriteLine($"[DEBUG ObrisiObjekat] FizickoLice validacija: racuni={soRacuniCheck.Result?.Count ?? 0}, errorMessage={errorMessage ?? "null"}");
                            }
                            else if (objektZaBrisanje is Firma firmaZaBrisanje)
                            {
                                // Proveri da li postoje računi sa ovom firmom
                                var racunZaProveru = new Racun { Kupac = new Kupac { IdKupac = firmaZaBrisanje.IdKupac } };
                                var soRacuniCheck = new SOVratiSve(racunZaProveru);
                                soRacuniCheck.ExecuteTemplate();

                                if (soRacuniCheck.Result != null && soRacuniCheck.Result.Count > 0)
                                {
                                    errorMessage = $"Ne možete obrisati firmu '{firmaZaBrisanje.Naziv}' jer postoji {soRacuniCheck.Result.Count} račun(a) koji je koriste!";
                                }
                            }
                            else if (objektZaBrisanje is Oprema opremaZaBrisanje)
                            {
                                // Proveri da li postoje stavke računa sa ovom opremom
                                var stavkaZaProveru = new StavkaRacuna { Oprema = new Oprema { IdOprema = opremaZaBrisanje.IdOprema } };
                                var soStavkeCheck = new SOVratiSve(stavkaZaProveru);
                                soStavkeCheck.ExecuteTemplate();

                                if (soStavkeCheck.Result != null && soStavkeCheck.Result.Count > 0)
                                {
                                    errorMessage = $"Ne možete obrisati opremu '{opremaZaBrisanje.Ime}' jer postoji {soStavkeCheck.Result.Count} stavka računa koje je koriste!";
                                }
                            }
                            else if (objektZaBrisanje is Skladiste skladisteZaBrisanje)
                            {
                                // Proveri da li postoje ProdSklad veze sa ovim skladištem
                                var prodSkladZaProveru = new ProdSklad { IdSkladiste = skladisteZaBrisanje.IdSkladiste };
                                var soProdSkladCheck = new SOVratiSve(prodSkladZaProveru);
                                soProdSkladCheck.ExecuteTemplate();

                                if (soProdSkladCheck.Result != null && soProdSkladCheck.Result.Count > 0)
                                {
                                    errorMessage = $"Ne možete obrisati skladište '{skladisteZaBrisanje.Naziv}' jer {soProdSkladCheck.Result.Count} prodavac(a) je dodeljeno tom skladištu!";
                                }
                            }

                            // Ako postoji greška, vrati je
                            if (errorMessage != null)
                            {
                                Console.WriteLine($"[DEBUG ObrisiObjekat] Validacija neuspešna, vraćam grešku: {errorMessage}");
                                res.IsSuccessful = false;
                                res.Message = errorMessage;
                            }
                            else
                            {
                                Console.WriteLine($"[DEBUG ObrisiObjekat] Validacija prošla, pozivam SOObrisiObjekat za tip {objektZaBrisanje.GetType().Name}");
                                // Inače obriši objekat
                                var soObrisi = new SOObrisiObjekat(objektZaBrisanje);
                                soObrisi.ExecuteTemplate();
                                res.IsSuccessful = soObrisi.Success;
                                res.Message = soObrisi.Success ? "Objekat je uspešno obrisan." : "Greška pri brisanju objekta.";
                                Console.WriteLine($"[DEBUG ObrisiObjekat] SOObrisiObjekat rezultat: Success={soObrisi.Success}");
                            }
                        }
                        else
                        {
                            res.IsSuccessful = false;
                            res.Message = "Server nije prepoznao tip objekta.";
                        }
                        break;
                    default:
                        res.IsSuccessful = false;
                        res.Message = "Nepoznata operacija.";
                        break;
                }
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message = $"Greška: {ex.Message}";
            }
            return res;
        }
    }
}