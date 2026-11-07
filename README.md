# HSK-RimThunder

Глобальная адаптация и интеграция линейки военной техники **RimThunder** под сборку **Hardcore-SK (HSK)** для RimWorld 1.5 / 1.6.

Пак объединяет 11 взаимосвязанных модулей техники, все необходимые фреймворки и пак советского вооружения, вводя в игру более **35 полностью рабочих видов техники**: от легких разведывательных броневиков, аэросаней и мотоциклов времен ВМВ до современных танковых платформ (Т-14 «Армата», Abrams, Т-90М, Bradley, БМП-3).

---

## 🛠 Состав сборки

### 🚗 Модули бронетехники (RimThunder)
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

### ⚙️ Фреймворки и интеграция
- **Vehicle Framework** (`SmashPhil.VehicleFramework`) — базовый движок боевой техники, посадки экипажа, прицеливания турелей и физики движения.
- **VehicleRaid Framework** (`gabrieel1482.raidvehicleframework`) — генерация и спавн боевой техники в рейдах вражеских фракций (сбалансированы боеприпасы, очки рейдов и поведение пехоты прикрытия).
- **Vanilla Expanded Framework** (`OskarPotocki.VanillaFactionsExpanded.Core`) — расширенный системный фреймворк компонентов.

### 🔫 Вооружение
- **HMC weapon pack – USSR** — пак советского стрелкового оружия и аутентичных калибров боеприпасов для экипажа и турелей. !!! ТЕХНИКА ИСПОЛЬЗУЕТ СНАРЯДЫ С ЭТОГО МОДА !!!

---

## 🌟 Ключевые особенности адаптации

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

## 📥 Установка

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

## 👥 Авторы оригиналов и благодарности

- **RimThunder Team** (AOBA, Cannon Mutton, RimThunderTeams) — [RimThunder Workshop Collection](https://steamcommunity.com/workshop/filedetails/?id=3276606914)
- **SmashPhil** — [Vehicle Framework](https://steamcommunity.com/sharedfiles/filedetails/?id=3014915404)
- **gabrieel1482** — [VehicleRaid Framework](https://steamcommunity.com/sharedfiles/filedetails/?id=3667863988)
- **Oskar Potocki & Vanilla Expanded Team** — [Vanilla Expanded Framework](https://steamcommunity.com/sharedfiles/filedetails/?id=2023507013)
- **arpomo6** — HMC weapon pack – USSR
- **Команда Hardcore-SK**
