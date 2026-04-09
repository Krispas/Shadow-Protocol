# Shadow Protocol – dokumentace projektu

---

## 1. O projektu

Projekt **Shadow Protocol** je konzolová RPG / stealth hra vytvořená v jazyce C#.

Hráč se ujímá role agenta, který:

* plní mise
* vyhýbá se nepřátelům
* sbírá předměty
* plní cíle mise

---

## 2. Implementované části

### 2.1 Main Menu

* navigace pomocí šipek (↑ ↓)
* výběr pomocí Enter

Možnosti:

1. Nová hra
2. Načíst hru (zatím neimplementováno)
3. Tutorial
4. Ukončit

---

### 2.2 Gameplay systém

* pohyb:

    * WASD
    * šipky
* interakce:

    * klávesa **E**
* základní herní smyčka (game loop)

---

### 2.3 Mapový systém

* mapy jsou definované v souboru `missions.json`
* typy polí:

    * `#` = zeď
    * `.` = volné pole
* mapa se dynamicky vykresluje do konzole

---

### 2.4 Detekční systém

#### Kamery:

* mají směr (up, down, left, right)
* mají dosah
* jsou blokovány zdmi

#### Nepřátelé:

* mají směr pohledu
* mají detection range

Pokud je hráč detekován:

* zatím mise končí neúspěchem, pozdeji bude moct s nepřítelem bojovat

---

### 2.5 Nepřátelé

Typy:

* guard (běžný nepřítel)
* target (hlavní cíl mise)

Každý nepřítel bude mít:

* HP
* attack
* směr
* detection range

---

### 2.6 Item systém

* `K` = keycard
* `=` = dokumenty

Funkce:

* sběr pomocí interakce nebo vstupu na tile

---

### 2.7 Objective systém

Pro dokončení mise musí hráč:

1. získat keycard
2. získat dokumenty
3. eliminovat cíl
4. dojít na exit

---

---

### 2.9 JSON systém

Mise jsou načítány ze souboru `missions.json`.

Obsah:

* layout mapy
* nepřátelé
* itemy
* kamery
* legenda
* instrukce

---

### 2.10 Render systém

* barevné vykreslení:

    * hráč
    * nepřátelé
    * kamery
    * itemy

* legenda:

    * zobrazená vedle mapy

* instrukce:

    * zobrazené pod legendou

* HUD:

    * stav itemů
    * detekce
    * progress mise

---

## 3. Co zbývá dokončit

### 3.1 Save / Load systém

* ukládání hry
* načítání hry

---

### 3.2 Výběr mise

* menu pro výběr misí
* odemykání dalších misí

---

### 3.3 Class / loadout systém

* výběr třídy před misí
* různé staty

---

### 3.4 Combat systém

* souboje místo okamžité eliminace
* akce:

    * attack
    * defend
    * special

---

### 3.5 AI vylepšení

* pohyb nepřátel
* patrolování
* reakce na hráče

---

### 3.6 UI vylepšení

* lepší HUD
* zvýraznění objective
* případné animace

---

### 3.7 Balancování

* úprava obtížnosti
* ladění map
* testování

---

## 4. Shrnutí

Projekt obsahuje:

* funkční herní smyčku
* práci s JSON daty
* vykreslování v konzoli

Zbývá především:

* rozšíření funkcionality
* dokončení systémů
* vylepšení gameplaye
* přidaní misí

---

