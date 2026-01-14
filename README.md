# Tournament API - System Zarządzania Turniejami

API GraphQL do zarządzania turniejami w systemie pucharowym (bracket/knockout).

## Technologie

- **ASP.NET Core 9.0**
- **HotChocolate (GraphQL)**
- **Entity Framework Core**
- **SQLite**
- **JWT Authentication**
- **BCrypt** (hashowanie haseł)

## Jak uruchomić projekt

### 1. Przejdź do katalogu projektu
```bash
cd /home/mehow/Developer/graphqlprojekt/TournamentApi
```

### 2. Uruchom aplikację
```bash
dotnet run
```

### 3. Otwórz GraphQL IDE w przeglądarce
Aplikacja uruchomi się domyślnie na porcie wyświetlonym w konsoli (np. `http://localhost:5062`).

Otwórz: **http://localhost:5062/graphql**

Zobaczysz interfejs **Banana Cake Pop** do testowania API.

## Struktura bazy danych

- **User** - użytkownicy systemu
- **Tournament** - turnieje (Pending, InProgress, Finished)
- **Bracket** - drabinka turniejowa
- **Match** - pojedyncze mecze z rundami

## Przykładowe komendy GraphQL

### 1. Rejestracja użytkowników

```graphql
mutation {
  register(input: {
    firstName: "Jan"
    lastName: "Kowalski"
    email: "jan@example.com"
    password: "haslo123"
  }) {
    token
    user {
      id
      firstName
      email
    }
  }
}
```

Zarejestruj minimum 4 użytkowników (turniej wymaga 2, 4, 8, 16 uczestników).

### 2. Logowanie

```graphql
mutation {
  login(input: {
    email: "jan@example.com"
    password: "haslo123"
  }) {
    token
    user {
      id
      firstName
    }
  }
}
```

**Zapisz token!** Będzie potrzebny do autoryzowanych zapytań.

### 3. Pobranie wszystkich użytkowników

```graphql
query {
  users {
    id
    firstName
    lastName
    email
  }
}
```

### 4. Utworzenie turnieju

```graphql
mutation {
  createTournament(input: {
    name: "Mistrzostwa Wiosny 2026"
    startDate: "2026-03-01T10:00:00Z"
  }) {
    id
    name
    status
    startDate
  }
}
```

### 5. Dodanie uczestników do turnieju

```graphql
mutation {
  addParticipant(tournamentId: 1, userId: 1) {
    id
    name
    participants {
      id
      firstName
      lastName
    }
  }
}
```

Powtórz dla wszystkich uczestników (dodaj userId: 2, 3, 4).

### 6. Rozpoczęcie turnieju (generuje drabinkę)

```graphql
mutation {
  startTournament(tournamentId: 1) {
    id
    name
    status
    bracket {
      id
      matches {
        id
        round
        player1 { id firstName }
        player2 { id firstName }
        winner { firstName }
      }
    }
  }
}
```

**Uwaga:** Liczba uczestników musi być potęgą 2 (2, 4, 8, 16, 32...).

Dla 4 uczestników zostanie utworzona drabinka:
- **Runda 1:** 2 mecze
- **Runda 2:** 1 mecz (finał)

### 7. Pobranie szczegółów turnieju

```graphql
query {
  tournament(id: 1) {
    id
    name
    status
    startDate
    participants {
      id
      firstName
      lastName
    }
    bracket {
      id
      matches {
        id
        round
        player1 { firstName }
        player2 { firstName }
        winner { firstName }
      }
    }
  }
}
```

### 8. Pobranie drabinki

```graphql
query {
  bracket(id: 1) {
    id
    tournament {
      name
      status
    }
    matches {
      id
      round
      player1 { id firstName }
      player2 { id firstName }
      winner { firstName }
    }
  }
}
```

### 9. Rozegranie meczu

```graphql
mutation {
  playMatch(matchId: 1, winnerId: 1) {
    id
    round
    player1 { firstName }
    player2 { firstName }
    winner { firstName }
  }
}
```

**Zwycięzca automatycznie awansuje do następnej rundy!**

### 10. Zakończenie turnieju

```graphql
mutation {
  finishTournament(tournamentId: 1) {
    id
    name
    status
  }
}
```

Status zmieni się na `FINISHED`.

### 11. Moje mecze (wymaga autoryzacji)

**Krok 1:** Dodaj nagłówek HTTP z tokenem JWT

W **Banana Cake Pop** kliknij **"Headers"** (na dole po lewej) i dodaj:
- **Klucz:** `Authorization`
- **Wartość:** `Bearer TWOJ_TOKEN_TUTAJ`

**Krok 2:** Wykonaj zapytanie

```graphql
query {
  myMatches {
    id
    round
    player1 { firstName lastName }
    player2 { firstName lastName }
    winner { firstName }
    bracket {
      tournament {
        name
        status
      }
    }
  }
}
```

Zwróci tylko mecze, w których zalogowany użytkownik uczestniczy.

## Przykładowy przepływ testowy

### Scenariusz: Turniej 4-osobowy

```graphql
# 1. Zarejestruj 4 użytkowników
mutation { register(input: { firstName: "Anna", lastName: "Nowak", email: "anna@test.com", password: "pass123" }) { user { id } } }
mutation { register(input: { firstName: "Bartek", lastName: "Kowal", email: "bartek@test.com", password: "pass123" }) { user { id } } }
mutation { register(input: { firstName: "Celina", lastName: "Lis", email: "celina@test.com", password: "pass123" }) { user { id } } }
mutation { register(input: { firstName: "Dawid", lastName: "Wilk", email: "dawid@test.com", password: "pass123" }) { user { id } } }

# 2. Utwórz turniej
mutation { createTournament(input: { name: "Turniej Testowy", startDate: "2026-03-01T10:00:00Z" }) { id } }

# 3. Dodaj uczestników
mutation { addParticipant(tournamentId: 1, userId: 1) { id } }
mutation { addParticipant(tournamentId: 1, userId: 2) { id } }
mutation { addParticipant(tournamentId: 1, userId: 3) { id } }
mutation { addParticipant(tournamentId: 1, userId: 4) { id } }

# 4. Rozpocznij turniej
mutation { startTournament(tournamentId: 1) { bracket { matches { id round player1 { firstName } player2 { firstName } } } } }

# 5. Rozegraj mecze rundy 1
mutation { playMatch(matchId: 1, winnerId: 1) { winner { firstName } } }
mutation { playMatch(matchId: 2, winnerId: 3) { winner { firstName } } }

# 6. Rozegraj finał (runda 2)
mutation { playMatch(matchId: 3, winnerId: 1) { winner { firstName } } }

# 7. Zakończ turniej
mutation { finishTournament(tournamentId: 1) { status } }
```

## Walidacje

### ✅ Co zadziała:
- Rejestracja z unikalnym emailem
- Logowanie z poprawnymi danymi
- Dodawanie uczestników do turnieju ze statusem `Pending`
- Rozpoczęcie turnieju z 2, 4, 8, 16... uczestnikami
- Ustawienie zwycięzcy meczu (tylko gracze z tego meczu)
- Automatyczny awans zwycięzcy do następnej rundy
- Zakończenie turnieju ze statusem `InProgress`

### ❌ Co zwróci błąd:
- Rejestracja z istniejącym emailem
- Logowanie ze złym hasłem
- Rozpoczęcie turnieju z 3, 5, 6, 7... uczestnikami
- Dodawanie uczestników po rozpoczęciu turnieju
- Ponowne rozegranie już rozegranego meczu
- Ustawienie zwycięzcy, który nie gra w tym meczu
- Dostęp do `myMatches` bez tokenu JWT

## Statusy turnieju

- **Pending** - turniej utworzony, można dodawać uczestników
- **InProgress** - turniej rozpoczęty, drabinka wygenerowana
- **Finished** - turniej zakończony

## Baza danych

Baza SQLite znajduje się w: `tournament.db`

### Ponowne utworzenie bazy danych

Jeśli chcesz zresetować bazę:

```bash
rm tournament.db
export PATH="$PATH:/home/mehow/.dotnet/tools"
dotnet ef database update
```

## Konfiguracja JWT

Ustawienia JWT znajdują się w `appsettings.json`:

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm",
    "Issuer": "TournamentApi",
    "Audience": "TournamentApiUsers",
    "ExpiryInMinutes": 60
  }
}
```

**Uwaga:** W produkcji zmień `SecretKey` na bezpieczny losowy ciąg!

## Rozwiązywanie problemów

### Port już zajęty
Jeśli port jest zajęty, zatrzymaj poprzednią instancję (Ctrl+C) lub zmień port w `Properties/launchSettings.json`.

### Błąd migracji
Jeśli pojawią się błędy związane z bazą danych:
```bash
export PATH="$PATH:/home/mehow/.dotnet/tools"
dotnet ef migrations add InitialCreate --force
dotnet ef database update
```

### Token nie działa
Upewnij się, że:
1. Dodajesz pełny token (cały długi ciąg)
2. Format to: `Bearer <token>` (ze spacją po "Bearer")
3. Token nie wygasł (ważny 60 minut)

## Autor

Projekt stworzony jako API do zarządzania turniejami w systemie pucharowym z wykorzystaniem GraphQL.
