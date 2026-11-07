# RU
## HSK-RimThunder


Глобальная адаптация и интеграция линейки военной техники **RimThunder** под сборку **Hardcore-SK (HSK)** для RimWorld 1.5 / 1.6.

Пак объединяет 11 взаимосвязанных модулей техники, все необходимые фреймворки и пак советского вооружения, вводя в игру более **35 полностью рабочих видов техники**: от легких разведывательных броневиков, аэросаней и мотоциклов времен ВМВ до современных танковых платформ (Т-14 «Армата», Abrams, Т-90М, Bradley, БМП-3).

---

### 🛠 Состав сборки

#### 🚗 Модули бронетехники (RimThunder)
- **RimThunder - Core** — базовое ядро, производственные верстаки, библиотека компонентов, запчастей и авто-патчер Combat Extended.
- **RimThunder - Breakthrough** — БМП-3, БМПТ «Терминатор», танк Т-80БВМ.
- **RimThunder - Desert Sabre** — M1A2 SEP Abrams, легкий танк M10 Booker, БМП M2A3 Bradley.
- **RimThunder - Enduring Freedom** — бронеавтомобиль Oshkosh M-ATV, тактический грузовик M1078 LMTV.
- **RimThunder - Kon'** — армейский грузовик КамАЗ-5350, бронеавтомобиль «Тайфун-ВДВ».
- **RimThunder - Roaring Tiger** — танки M1A2 MBT и M48H, бронетранспортеры M113 и M113(M) с минометом, тяжелый тягач M1075 HEMTT.
- **RimThunder - Talvisota** — танки Т-34 обр. 1941, Т-28, БТ-5, армейский мотоцикл М-72, бронированные аэросани НКЛ-26.
- **RimThunder - Time Forward** — танк нового поколения Т-14 «Армата» и тяжелая гусеничная БМП Т-15 «Армата».
- **RimThunder - Way to Victory** — тяжелые танки ИС-2, ИС-3, легкий танк Т-70, Т-34/85, РСЗО БМ-13 «Катюша», бронеавтомобиль БА-64.
- **RimThunder - Zarya** — танк Т-90М «Прорыв», гусеничная БМП «Курганец-25», колесная БМП «Бумеранг».
- **RimThunder - Flying Chariot** — тяжелый транспортный самолет C-130 Hercules, вертолеты CH-53E Super Stallion и UH-60 Black Hawk.

#### ⚙️ Фреймворки и интеграция
- **Vehicle Framework** (`SmashPhil.VehicleFramework`) — базовый движок боевой техники, посадки экипажа, прицеливания турелей и физики движения.
- **VehicleRaid Framework** (`gabrieel1482.raidvehicleframework`) — генерация и спавн боевой техники в рейдах вражеских фракций (сбалансированы боеприпасы, очки рейдов и поведение пехоты прикрытия).
- **Vanilla Expanded Framework** (`OskarPotocki.VanillaFactionsExpanded.Core`) — расширенный системный фреймворк компонентов.

#### 🔫 Вооружение
- **HMC weapon pack – USSR** — пак советского стрелкового оружия и аутентичных калибров боеприпасов для экипажа и турелей. !!! ТЕХНИКА ИСПОЛЬЗУЕТ СНАРЯДЫ С ЭТОГО МОДА !!!

---

### 🌟 Ключевые особенности адаптации

1. **Реалистичная экономика HSK**:
   - Крафты всей техники переведены на ресурсы HSK: 
   - Техника собирается исключительно в промышленном сборочном кране (**Assembly gantry** / `RT_AssemblyCrane`).
2. **Отдельная ветка исследований («Бронетехника» / «Armored Vehicles»)**:
   - Собственная исследовательская ветка из последовательных тиров, логично продолжающих технологическое древо HSK (от ранней моторизации до платформы «Армата»).
3. **Интеграция с Combat Extended (CE)**:
   - Встроенный Harmony-патч устраняет баги навесного огня и минометов на шасси техники.
   - В описании каждого крафта (значок `i`) указана полная сводка: точные названия боеприпасов для всех турелей, экипаж и тип топлива.
4. **Откалиброванные рейды техники**:
   - Исключена авиация и безоружный транспорт из штурмовых рейдов.
   - Реалистичное соотношение снарядов главного калибра, спаренных пулеметов и дымовых гранатометов.

---

### 📥 Установка

1. Скопируйте все папки модов из этого репозитория в вашу директорию модов:  
   `<Папка с игрой RimWorld>/Mods/`
2. Активируйте моды в игре, Mod Assistant или RimPy.
3. Порядок загрузки фреймворков:
   - `Harmony`
   - `Core`
   - `Vanilla Expanded Framework`
   - `Vehicle Framework`
   - `Combat Extended`
   - `Hardcore-SK`
   - `HMC weapon pack – USSR`
   - `VehicleRaid Framework`
   - `RimThunder - Core`
   - `[Остальные модули RimThunder]`

---

### 👥 Авторы оригиналов и благодарности

- **RimThunder Team** (AOBA, Cannon Mutton, RimThunderTeams) — [RimThunder Workshop Collection](https://steamcommunity.com/workshop/filedetails/?id=3276606914)
- **SmashPhil** — [Vehicle Framework](https://steamcommunity.com/sharedfiles/filedetails/?id=3014915404)
- **gabrieel1482** — [VehicleRaid Framework](https://steamcommunity.com/sharedfiles/filedetails/?id=3667863988)
- **Oskar Potocki & Vanilla Expanded Team** — [Vanilla Expanded Framework](https://steamcommunity.com/sharedfiles/filedetails/?id=2023507013)
- **arpomo6** — HMC weapon pack – USSR
- **Команда Hardcore-SK**

# EN
## HSK-RimThunder

A global adaptation and integration of the **RimThunder** military vehicle series for the **Hardcore-SK (HSK)** modpack for RimWorld 1.5 / 1.6.

The pack combines 11 interconnected vehicle modules, all necessary frameworks, and a Soviet weapons pack, introducing over **35 fully functional types of vehicles** into the game: from light reconnaissance armored cars, aerosani, and WWII-era motorcycles to modern tank platforms (T-14 "Armata", Abrams, T-90M, Bradley, BMP-3).

---

### 🛠 Pack Contents

#### 🚗 Armored Vehicle Modules (RimThunder)
- **RimThunder - Core** — base core, production workbenches, component and spare parts library, and Combat Extended auto-patcher.
- **RimThunder - Breakthrough** — BMP-3, BMPT "Terminator", T-80BVM tank.
- **RimThunder - Desert Sabre** — M1A2 SEP Abrams, M10 Booker light tank, M2A3 Bradley IFV.
- **RimThunder - Enduring Freedom** — Oshkosh M-ATV armored vehicle, M1078 LMTV tactical truck.
- **RimThunder - Kon'** — KamAZ-5350 army truck, Typhoon-VDV armored vehicle.
- **RimThunder - Roaring Tiger** — M1A2 MBT and M48H tanks, M113 APC and M113(M) mortar carrier, M1075 HEMTT heavy tractor.
- **RimThunder - Talvisota** — T-34 mod. 1941, T-28, BT-5 tanks, M-72 army motorcycle, NKL-26 armored aerosani.
- **RimThunder - Time Forward** — next-generation T-14 "Armata" tank and T-15 "Armata" heavy tracked IFV.
- **RimThunder - Way to Victory** — IS-2, IS-3 heavy tanks, T-70 light tank, T-34/85, BM-13 "Katyusha" MLRS, BA-64 armored car.
- **RimThunder - Zarya** — T-90M "Proryv" tank, "Kurganets-25" tracked IFV, "Boomerang" wheeled IFV.
- **RimThunder - Flying Chariot** — C-130 Hercules heavy transport aircraft, CH-53E Super Stallion and UH-60 Black Hawk helicopters.

#### ⚙️ Frameworks and Integration
- **Vehicle Framework** (`SmashPhil.VehicleFramework`) — the core engine for combat vehicles, crew boarding, turret aiming, and movement physics.
- **VehicleRaid Framework** (`gabrieel1482.raidvehicleframework`) — generation and spawning of combat vehicles in enemy faction raids (balanced ammunition, raid points, and cover infantry behavior).
- **Vanilla Expanded Framework** (`OskarPotocki.VanillaFactionsExpanded.Core`) — extended system framework for components.

#### 🔫 Weaponry
- **HMC weapon pack – USSR** — a pack of Soviet small arms and authentic ammunition calibers for the crew and turrets. !!! VEHICLES USE SHELLS FROM THIS MOD !!!

---

### 🌟 Key Features of the Adaptation

1. **Realistic HSK economy**:
   - Crafting recipes for all vehicles have been converted to use HSK resources.
   - Vehicles are assembled exclusively in the industrial **Assembly gantry** (`RT_AssemblyCrane`).
2. **Dedicated research tree ("Armored Vehicles")**:
   - A custom research tree consisting of sequential tiers, logically extending the HSK technology tree (from early motorization to the "Armata" platform).
3. **Combat Extended (CE) Integration**:
   - A built-in Harmony patch fixes bugs related to indirect fire and vehicle-mounted mortars.
   - The description of every recipe (the `i` icon) provides a full summary: exact ammunition names for all turrets, crew capacity, and fuel type.
4. **Calibrated vehicle raids**:
   - Aviation and unarmed transport have been excluded from assault raids.
   - Realistic ratio of main caliber shells, coaxial machine guns, and smoke grenade launchers.

---

### 📥 Installation

1. Copy all mod folders from this repository into your mods directory:  
   `<RimWorld game folder>/Mods/`
2. Activate the mods in the game, Mod Assistant, or RimPy.
3. Framework load order:
   - `Harmony`
   - `Core`
   - `Vanilla Expanded Framework`
   - `Vehicle Framework`
   - `Combat Extended`
   - `Hardcore-SK`
   - `HMC weapon pack – USSR`
   - `VehicleRaid Framework`
   - `RimThunder - Core`
   - `[Other RimThunder modules]`

---

### 👥 Original Authors and Credits

- **RimThunder Team** (AOBA, Cannon Mutton, RimThunderTeams) — [RimThunder Workshop Collection](https://steamcommunity.com/workshop/filedetails/?id=3276606914)
- **SmashPhil** — [Vehicle Framework](https://steamcommunity.com/sharedfiles/filedetails/?id=3014915404)
- **gabrieel1482** — [VehicleRaid Framework](https://steamcommunity.com/sharedfiles/filedetails/?id=3667863988)
- **Oskar Potocki & Vanilla Expanded Team** — [Vanilla Expanded Framework](https://steamcommunity.com/sharedfiles/filedetails/?id=2023507013)
- **arpomo6** — HMC weapon pack – USSR
- **Hardcore-SK Team**