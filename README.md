# Palickaj

Palickaj to aplikacja webowa napisana w C# i .NET 8, przygotowana jako projekt portfolio pod stanowisko laczace helpdesk, serwis techniczny, obsluge klienta i rozwoj rozwiazan ERP.

Projekt symuluje codzienna prace firmy wdrozeniowej:

- przyjmowanie i obsluge zgloszen serwisowych
- prace z klientami korzystajacymi z systemow ERP
- monitoring integracji z systemami zewnetrznymi
- raportowanie backlogu i ryzyk operacyjnych

## Opis projektu

Aplikacja pelni role wewnetrznego panelu operacyjnego dla zespolu wsparcia technicznego. Pokazuje zgloszenia, klientow, integracje oraz podstawowy raport techniczny. Projekt zostal zaprojektowany tak, aby pokazywac praktyczne wykorzystanie C#, .NET, pracy z baza danych i logiki biznesowej w realiach wsparcia systemow ERP.

## Stack

- C#
- .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQLite
- Bootstrap 5
- wlasne style CSS

## Najwazniejsze funkcje

- dashboard z KPI i kolejka priorytetowa
- lista zgloszen z filtrowaniem
- dodawanie i edycja zgloszen
- export listy zgloszen do CSV
- karta klienta z aktywnymi tematami i integracjami
- monitoring integracji i synchronizacji
- raport operacyjny z rekomendacjami
- automatyczne seedowanie danych demo po pierwszym uruchomieniu

## Jak uruchomic

### Wymagania

- zainstalowany .NET SDK 8

### Kroki

```bash
cd palickaj
dotnet restore
dotnet build Palickaj.sln
dotnet run --project Palickaj.csproj
```

### Po uruchomieniu

- aplikacja uruchomi sie lokalnie
- domyslny adres HTTP: `http://localhost:5015`
- domyslny adres HTTPS: `https://localhost:7180`

Przy pierwszym starcie projekt automatycznie utworzy lokalna baze SQLite w katalogu `App_Data` i zaladuje dane demo.

Jesli domyslny port jest zajety, mozna uruchomic aplikacje np. tak:

```bash
dotnet run --project Palickaj.csproj --urls http://localhost:5080
```

## Jak sprawdzic, czy dziala

Po uruchomieniu otworz aplikacje w przegladarce i sprawdz:

- `http://localhost:5015` albo adres podany w terminalu
- dashboard z metrykami i kolejka priorytetowa
- zakladke `Zgloszenia`
- zakladke `Klienci`
- zakladke `Integracje`
- zakladke `Raport`

Szybki test manualny:

1. Wejdz w `Zgloszenia`
2. Otworz dowolne zgloszenie
3. Kliknij `Edytuj`
4. Zmien status albo liczbe godzin
5. Zapisz zmiany i sprawdz, czy dane sie odswiezyly

Mozesz tez dodac nowe zgloszenie i sprawdzic, czy pojawi sie na liscie backlogu.

## Struktura projektu

- `Controllers` - kontrolery MVC
- `Data` - kontekst bazy i seed danych
- `Models` - encje domenowe
- `Services` - logika biznesowa i agregacja danych
- `ViewModels` - modele pod widoki
- `Views` - warstwa interfejsu
- `wwwroot` - statyczne zasoby frontendu

## Co pokazuje ten projekt

Projekt ma pokazac, ze potrafie:

- pracowac w C# i .NET
- budowac aplikacje webowe MVC
- korzystac z bazy danych i warstwy ORM
- organizowac logike biznesowa w czytelnej strukturze
- projektowac interfejs pod narzedzie biznesowe
- laczyc obszar supportu technicznego z programowaniem
